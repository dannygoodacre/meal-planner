import { useState } from 'react';
import { useFormContext, useWatch } from 'react-hook-form';

import FoodSearchPopover from '@/components/FoodSearchPopover';
import { MacroCells } from '@/components/MacroColumns';
import NumberInput from '@/components/NumberInput';
import RemoveButton from '@/components/RemoveButton';
import { FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { TableCell, TableRow } from '@/components/ui/table';
import { defaultQuantity, ingredientMacros, unitLabel } from '@/macros';

import type { Food, IngredientFormData } from '@/types';

interface IngredientRowProps {
  name: `ingredients.${number}` | `meals.${number}.ingredients.${number}`;
  isEditable: boolean;
  onRemove: () => void;
}

export default function IngredientRow({ name, isEditable, onRemove }: IngredientRowProps) {
  const { control, setValue } = useFormContext();

  const ingredient: IngredientFormData = useWatch({ control, name }) ?? { food: null, quantity: 0 };

  const [search, setSearch] = useState(ingredient.food?.name ?? '');
  const [prevFood, setPrevFood] = useState(ingredient.food);

  // Keep the search text in step when the food is set from outside (reset, meal copied in).
  if (ingredient.food !== prevFood) {
    setPrevFood(ingredient.food);

    if (ingredient.food) {
      setSearch(ingredient.food.name);
    }
  }

  const handleSearchChange = (value: string) => {
    setSearch(value);

    if (ingredient.food) {
      setValue(`${name}.food`, null);
    }
  };

  const handleSelect = (food: Food) => {
    setSearch(food.name);

    setValue(name, { food, quantity: defaultQuantity(food.unit) }, { shouldValidate: true });
  };

  const unit = unitLabel(ingredient.food?.unit ?? 'Grams');

  return (
    <TableRow className='h-8'>
      <TableCell className='w-[6%]' />

      <TableCell className='py-2 font-medium w-[24%]'>
        {isEditable ? (
          <FormField
            control={control}
            name={`${name}.food`}
            render={() => (
              <FormItem>
                <FoodSearchPopover
                  value={search}
                  onValueChange={handleSearchChange}
                  onSelect={handleSelect}
                  selected={!!ingredient.food}
                />
                <FormMessage />
              </FormItem>
            )}
          />
        ) : (
          <span className='truncate block'>{ingredient.food?.name}</span>
        )}
      </TableCell>

      <TableCell className='py-1 w-[6%]'>
        {isEditable ? (
          <FormField
            control={control}
            name={`${name}.quantity`}
            render={({ field }) => (
              <FormItem>
                <div className='flex items-center gap-1.5 w-full'>
                  <FormControl>
                    <NumberInput
                      {...field}
                      step={1}
                      disabled={!ingredient.food}
                      className='h-7 w-12 px-1 py-0 text-xs text-center shrink-0 min-w-0'
                    />
                  </FormControl>
                  <span className='text-xs shrink-0 whitespace-nowrap text-muted-foreground'>{unit}</span>
                </div>
                <FormMessage />
              </FormItem>
            )}
          />
        ) : (
          <div className='flex items-center h-7 py-2 text-xs'>
            {ingredient.quantity} {unit}
          </div>
        )}
      </TableCell>

      <MacroCells macros={ingredientMacros(ingredient)} className='py-2' />

      <TableCell className='py-1 w-[6%] pr-4 text-right'>
        {isEditable && <RemoveButton label='Remove ingredient' onClick={onRemove} />}
      </TableCell>
    </TableRow>
  );
}
