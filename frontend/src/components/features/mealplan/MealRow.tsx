import { Fragment } from 'react';
import { ChevronDown, ChevronRight, Plus, X } from 'lucide-react';
import { Controller, useFieldArray, useWatch } from 'react-hook-form';

import FoodSearchPopover from '@/components/common/FoodSearchPopover';
import MealSearchPopover from '@/components/common/MealSearchPopover';
import NumberInput from '@/components/common/NumberInput';
import { Button } from '@/components/ui/button';
import { Table, TableBody, TableCell, TableRow } from '@/components/ui/table';
import { calculateMealMacros, round, scaleMacro } from '@/utils';

import type { MealPlan } from '@/types';
import type { Control, UseFormSetValue } from 'react-hook-form';

interface MealRowViewProps {
  control: Control<MealPlan>;
  mealIndex: number;
  mealName: string;
  isEditable: boolean;
  isExpanded: boolean;
  setValue: UseFormSetValue<MealPlan>;
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
  setValue,
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

  const handleAddIngredient = () =>
    append({
      quantity: 0,
      food: {
        id: `temp-${crypto.randomUUID()}`,
        name: '',
        unit: 'Grams',
        referenceQuantity: 100,
        calories: 0,
        protein: 0,
        carbohydrates: 0,
        fat: 0,
        fibre: 0,
        sugar: 0,
        saturatedFat: 0,
        salt: 0,
        source: ''
      }
    });

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
            <Controller
              control={control}
              name={`meals.${mealIndex}.name`}
              render={({ field }) => (
                <MealSearchPopover
                  mealName={field.value}
                  onSearchChange={field.onChange}
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
              )}
            />
          ) : (
            mealName
          )}
        </TableCell>

        <TableCell className='text-right w-[11%]'>{round(mealMacros.calories, 0)}</TableCell>
        <TableCell className='text-right w-[11%]'>{round(mealMacros.protein)}</TableCell>

        <TableCell className='text-right w-[7%] pr-0.5'>{round(mealMacros.carbohydrates)}</TableCell>
        <TableCell className='text-left w-[4%] pl-0.5'>
          <span className='text-muted-foreground'>{round(mealMacros.sugar)}</span>
        </TableCell>

        <TableCell className='text-right w-[7%] pr-0.5'>{round(mealMacros.fat)}</TableCell>
        <TableCell className='text-left w-[4%] pl-0.5'>
          <span className='text-muted-foreground'>{round(mealMacros.saturatedFat)}</span>
        </TableCell>

        <TableCell className='text-right w-[7%]'>{round(mealMacros.fibre)}</TableCell>
        <TableCell className='text-right w-[7%]'>{round(mealMacros.salt)}</TableCell>

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

      {/* EXPANDED INGREDIENT SUB-TABLE */}
      {isExpanded && (
        <TableRow className='hover:bg-transparent bg-muted/30' onClick={e => e.stopPropagation()}>
          <TableCell colSpan={11} className='pt-0 pb-6 px-0'>
            {/*<div className='rounded-lg border bg-background overflow-hidden animate-in fade-in slide-in-from-top-2 duration-200'>*/}
            <div className='border-y bg-background overflow-hidden animate-in fade-in slide-in-from-top-2 duration-200'>
              {/* Fixed: Added table-fixed w-full here */}
              <Table className='table-fixed w-full'>
                {/*<TableHeader className='bg-muted/50'>*/}
                {/*  <TableRow>*/}
                {/*    <TableHead className='w-[3%]' />*/}

                {/*    <TableHead className='text-xs w-[26%]' />*/}
                {/*    <TableHead className='text-xs w-[10%]' />*/}

                {/*    <TableHead className='text-xs text-right w-[11%]' />*/}
                {/*    <TableHead className='text-xs text-right w-[11%]' />*/}

                {/*    <TableHead className='text-xs text-right w-[7%] pr-0.5' />*/}
                {/*    <TableHead className='text-xs text-left w-[4%] pl-0.5' />*/}

                {/*    <TableHead className='text-xs text-right w-[7%] pr-0.5' />*/}
                {/*    <TableHead className='text-xs text-left w-[4%] pl-0.5' />*/}

                {/*    <TableHead className='text-xs text-right w-[7%]' />*/}
                {/*    <TableHead className='text-xs text-right w-[7%]' />*/}

                {/*    <TableHead className='w-[3%]' />*/}
                {/*  </TableRow>*/}
                {/*</TableHeader>*/}
                <TableBody>
                  {ingredients.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={11} className='text-center py-4 text-xs text-muted-foreground italic'>
                        No ingredients added yet.
                      </TableCell>
                    </TableRow>
                  )}
                  {ingredients.map((ingField, ingIndex) => {
                    const currentIng = liveIngredients[ingIndex] || ingField;
                    const referenceQuantity = currentIng.food?.referenceQuantity || 100;
                    const quantity = currentIng.quantity || 0;

                    const liveCals = scaleMacro(currentIng.food?.calories || 0, quantity, referenceQuantity);
                    const liveProtein = scaleMacro(currentIng.food?.protein || 0, quantity, referenceQuantity);
                    const liveCarbs = scaleMacro(currentIng.food?.carbohydrates || 0, quantity, referenceQuantity);
                    const liveSugars = scaleMacro(currentIng.food?.sugar || 0, quantity, referenceQuantity);
                    const liveFat = scaleMacro(currentIng.food?.fat || 0, quantity, referenceQuantity);
                    const liveSats = scaleMacro(currentIng.food?.saturatedFat || 0, quantity, referenceQuantity);
                    const liveFibre = scaleMacro(currentIng.food?.fibre || 0, quantity, referenceQuantity);
                    const liveSalt = scaleMacro(currentIng.food?.salt || 0, quantity, referenceQuantity);

                    return (
                      <TableRow key={ingField.id} className='h-8 group/ing'>
                        <TableCell className='w-[3%]' />

                        <TableCell className='py-2 font-medium w-[26%] truncate'>
                          {isEditable ? (
                            <FoodSearchPopover
                              foodName={currentIng.food?.name || ''}
                              isSelected={!!currentIng.food?.id}
                              onSearchChange={newValue => {
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.name`, newValue);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.quantity`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.id`, '');
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.calories`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.protein`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.carbohydrates`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.sugar`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.fat`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.saturatedFat`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.fibre`, 0);
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}.food.salt`, 0);
                              }}
                              onSelect={selectedFood => {
                                setValue(`meals.${mealIndex}.ingredients.${ingIndex}`, {
                                  quantity: 100,
                                  food: selectedFood
                                });
                              }}
                            />
                          ) : (
                            currentIng.food?.name || ''
                          )}
                        </TableCell>

                        <TableCell className='py-1 w-[10%]'>
                          {isEditable ? (
                            <div className='flex items-center gap-1.5 w-full'>
                              <Controller
                                control={control}
                                name={`meals.${mealIndex}.ingredients.${ingIndex}.quantity`}
                                render={({ field }) => (
                                  <NumberInput
                                    value={field.value}
                                    onChange={field.onChange}
                                    className='h-7 px-1.5 py-0 text-xs flex-1 min-w-0'
                                  />
                                )}
                              />
                              <span className='text-xs shrink-0 whitespace-nowrap'>
                                {currentIng.food?.unit === 'Grams' ? 'g' : 'pcs'}
                              </span>
                            </div>
                          ) : (
                            <div className='flex items-center h-7 py-2'>
                              {quantity}
                              {currentIng.food?.unit === 'Grams' ? ' g' : ' pcs'}
                            </div>
                          )}
                        </TableCell>

                        <TableCell className='py-2 text-right w-[11%]'>{round(liveCals)}</TableCell>
                        <TableCell className='py-2 text-right w-[11%]'>{round(liveProtein)}</TableCell>

                        {/* Carbs + Sugars split */}
                        <TableCell className='py-2 text-right w-[7%] pr-0.5'>{round(liveCarbs)}</TableCell>
                        <TableCell className='py-2 text-left w-[4%] pl-0.5'>
                          <span className='text-muted-foreground'>{round(liveSugars)}</span>
                        </TableCell>

                        {/* Fat + Sats split */}
                        <TableCell className='py-2 text-right w-[7%] pr-0.5'>{round(liveFat)}</TableCell>
                        <TableCell className='py-2 text-left w-[4%] pl-0.5'>
                          <span className='text-muted-foreground'>{round(liveSats)}</span>
                        </TableCell>

                        <TableCell className='py-2 text-right w-[7%]'>{round(liveFibre)}</TableCell>
                        <TableCell className='py-2 text-right w-[7%]'>{round(liveSalt, 2)}</TableCell>

                        <TableCell className='py-1 w-[3%] pr-4 text-right'>
                          {isEditable && (
                            <Button
                              variant='ghost'
                              size='icon'
                              className='h-6 w-6 text-muted-foreground hover:text-destructive cursor-pointer'
                              onClick={() => remove(ingIndex)}
                              title='Remove ingredient'
                            >
                              <X className='h-3.5 w-3.5' />
                            </Button>
                          )}
                        </TableCell>
                      </TableRow>
                    );
                  })}
                </TableBody>
              </Table>
            </div>

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
