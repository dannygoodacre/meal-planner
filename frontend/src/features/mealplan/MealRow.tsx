import { ChevronDown, ChevronRight } from 'lucide-react';
import { useFieldArray, useFormState, useWatch } from 'react-hook-form';

import AddRowButton from '@/components/AddRowButton';
import FormArrayMessage from '@/components/FormArrayMessage';
import { MacroCells } from '@/components/MacroColumns';
import RemoveButton from '@/components/RemoveButton';
import { FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Table, TableBody, TableCell, TableRow } from '@/components/ui/table';
import IngredientRow from '@/features/meals/IngredientRow';
import MealSearchPopover from '@/features/meals/MealSearchPopover';
import { calculateMealMacros } from '@/macros';
import { cn } from '@/utils';

import type { MealPlanFormData } from '@/types';
import type { Control } from 'react-hook-form';

interface MealRowProps {
  control: Control<MealPlanFormData>;
  mealIndex: number;
  mealName: string;
  isEditable: boolean;
  isExpanded: boolean;
  onToggleExpand: () => void;
  onForceExpand: () => void;
  onDeleteMeal: () => void;
}

export default function MealRow({
  control,
  mealIndex,
  mealName,
  isEditable,
  isExpanded,
  onToggleExpand,
  onForceExpand,
  onDeleteMeal
}: MealRowProps) {
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

  const { errors } = useFormState({ control, name: `meals.${mealIndex}.ingredients` });

  const handleAddIngredient = () => append({ food: null, quantity: 0 });

  return (
    <>
      <TableRow
        className={cn(
          'cursor-pointer group transition-colors hover:bg-muted/50',
          isExpanded && 'border-b-0 bg-muted/30'
        )}
        onClick={onToggleExpand}
      >
        <TableCell className='w-[6%]'>
          {isExpanded ? <ChevronDown className='h-4 w-4' /> : <ChevronRight className='h-4 w-4' />}
        </TableCell>

        <TableCell className='w-[30%] font-medium truncate'>
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
                      replace(selectedMeal.ingredients.map(({ food, quantity }) => ({ food, quantity })));
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

        <MacroCells macros={calculateMealMacros(liveIngredients)} />

        <TableCell className='w-[6%] pr-4 text-right'>
          {isEditable && (
            <RemoveButton
              label='Delete meal'
              onClick={e => {
                e.stopPropagation();
                onDeleteMeal();
              }}
            />
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
              <div className='px-4 mt-4'>
                <AddRowButton label='Add another ingredient' onClick={handleAddIngredient} />
              </div>
            )}
          </TableCell>
        </TableRow>
      )}
    </>
  );
}
