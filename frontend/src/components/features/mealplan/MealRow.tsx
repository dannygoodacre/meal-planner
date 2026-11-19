import { Fragment } from 'react';
import { ChevronDown, ChevronRight, Plus, X } from 'lucide-react';
import { useFieldArray, useFormState, useWatch } from 'react-hook-form';

import FormArrayMessage from '@/components/common/FormArrayMessage';
import MealSearchPopover from '@/components/common/MealSearchPopover';
import IngredientRow from '@/components/features/meals/IngredientRow';
import { Button } from '@/components/ui/button';
import { FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Table, TableBody, TableCell, TableRow } from '@/components/ui/table';
import { calculateMealMacros, formatMacro } from '@/utils';

import type { MealPlanFormData } from '@/types';
import type { Control } from 'react-hook-form';

interface MealRowViewProps {
  control: Control<MealPlanFormData>;
  mealIndex: number;
  mealName: string;
  isEditable: boolean;
  isExpanded: boolean;
  onToggleExpand: () => void;
  onForceExpand: () => void;
  onDeleteMeal: () => void;
}

export function MealRow({
  control,
  mealIndex,
  mealName,
  isEditable,
  isExpanded,
  onToggleExpand,
  onForceExpand,
  onDeleteMeal
}: MealRowViewProps) {
  const {
    fields: ingredients,
    append,
    remove,
    replace
  } = useFieldArray({
    control,
    name: `meals.${mealIndex}.ingredients`
  });

  const liveIngredients =
    useWatch({
      control,
      name: `meals.${mealIndex}.ingredients`
    }) || [];

  const mealMacros = calculateMealMacros(liveIngredients);

  const { errors } = useFormState({ control, name: `meals.${mealIndex}.ingredients` });

  const handleAddIngredient = () => append({ food: null, quantity: 0 });

  return (
    <Fragment>
      {/* PARENT MEAL ROW */}
      <TableRow
        className={`cursor-pointer group transition-colors hover:bg-muted/50 ${isExpanded ? 'border-b-0 bg-muted/30' : ''}`}
        onClick={onToggleExpand}
      >
        <TableCell className='w-[3%]'>
          {isExpanded ? <ChevronDown className='h-4 w-4' /> : <ChevronRight className='h-4 w-4' />}
        </TableCell>

        <TableCell className='w-[36%] font-medium truncate'>
          {isEditable ? (
            <FormField
              control={control}
              name={`meals.${mealIndex}.name`}
              render={({ field }) => (
                <FormItem>
                  <MealSearchPopover
                    value={field.value}
                    onValueChange={field.onChange}
                    onSelect={selectedMeal => {
                      field.onChange(selectedMeal.name);
                      const copiedIngredients = (selectedMeal.ingredients || []).map(ing => ({
                        quantity: ing.quantity,
                        food: ing.food
                      }));
                      replace(copiedIngredients);
                      onForceExpand();
                    }}
                  />
                  <FormMessage />
                </FormItem>
              )}
            />
          ) : (
            mealName
          )}
        </TableCell>

        <TableCell className='text-right w-[11%]'>{formatMacro('calories', mealMacros.calories)}</TableCell>
        <TableCell className='text-right w-[11%]'>{formatMacro('protein', mealMacros.protein)}</TableCell>

        <TableCell className='text-right w-[7%] pr-0.5'>
          {formatMacro('carbohydrates', mealMacros.carbohydrates)}
        </TableCell>
        <TableCell className='text-left w-[4%] pl-0.5'>
          <span className='text-muted-foreground'>{formatMacro('sugar', mealMacros.sugar)}</span>
        </TableCell>

        <TableCell className='text-right w-[7%] pr-0.5'>{formatMacro('fat', mealMacros.fat)}</TableCell>
        <TableCell className='text-left w-[4%] pl-0.5'>
          <span className='text-muted-foreground'>{formatMacro('saturatedFat', mealMacros.saturatedFat)}</span>
        </TableCell>

        <TableCell className='text-right w-[7%]'>{formatMacro('fibre', mealMacros.fibre)}</TableCell>
        <TableCell className='text-right w-[7%]'>{formatMacro('salt', mealMacros.salt)}</TableCell>

        <TableCell className='w-[3%] pr-4 text-right'>
          {isEditable && (
            <Button
              variant='ghost'
              size='icon'
              className='h-7 w-7 text-muted-foreground hover:text-destructive cursor-pointer'
              onClick={e => {
                e.stopPropagation();
                onDeleteMeal();
              }}
              title='Delete meal'
            >
              <X className='h-4 w-4' />
            </Button>
          )}
        </TableCell>
      </TableRow>

      {isExpanded && (
        <TableRow className='hover:bg-transparent bg-muted/30' onClick={e => e.stopPropagation()}>
          <TableCell colSpan={11} className='pt-0 pb-6 px-0'>
            <div className='border-y bg-background overflow-hidden animate-in fade-in slide-in-from-top-2 duration-200'>
              <Table className='table-fixed w-full'>
                <TableBody>
                  {ingredients.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={11} className='text-center py-4 text-xs text-muted-foreground italic'>
                        No ingredients added yet.
                      </TableCell>
                    </TableRow>
                  )}
                  {ingredients.map((ingField, ingIndex) => (
                    <IngredientRow
                      key={ingField.id}
                      name={`meals.${mealIndex}.ingredients.${ingIndex}`}
                      isEditable={isEditable}
                      onRemove={() => remove(ingIndex)}
                    />
                  ))}
                </TableBody>
              </Table>
            </div>

            <FormArrayMessage error={errors.meals?.[mealIndex]?.ingredients} />

            {isEditable && (
              <Button
                type='button'
                variant='secondary'
                onClick={handleAddIngredient}
                className='w-full border-dashed border-2 h-12 mt-4 cursor-pointer'
              >
                <Plus className='mr-2 h-4 w-4' /> Add another ingredient
              </Button>
            )}
          </TableCell>
        </TableRow>
      )}
    </Fragment>
  );
}
