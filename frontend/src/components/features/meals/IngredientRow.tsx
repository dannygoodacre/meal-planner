import { useState } from 'react';
import { X } from 'lucide-react';
import { useFormContext, useWatch } from 'react-hook-form';

import FoodSearchPopover from '@/components/common/FoodSearchPopover';
import NumberInput from '@/components/common/NumberInput';
import { Button } from '@/components/ui/button';
import { FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { TableCell, TableRow } from '@/components/ui/table';
import { defaultQuantity, formatMacro, ingredientMacros, unitLabel } from '@/utils';

import type { Food, IngredientFormData } from '@/types';

interface IngredientRowProps {
  // Shared by the meal form and the meal plan form, so fields are addressed by path.
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

  const macros = ingredientMacros(ingredient);

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

      <TableCell className='py-2 text-right w-[11%]'>{formatMacro('calories', macros.calories)}</TableCell>
      <TableCell className='py-2 text-right w-[11%]'>{formatMacro('protein', macros.protein)}</TableCell>

      <TableCell className='py-2 text-right w-[7%] pr-0.5'>
        {formatMacro('carbohydrates', macros.carbohydrates)}
      </TableCell>
      <TableCell className='py-2 text-left w-[4%] pl-0.5'>
        <span className='text-muted-foreground'>{formatMacro('sugar', macros.sugar)}</span>
      </TableCell>

      <TableCell className='py-2 text-right w-[7%] pr-0.5'>{formatMacro('fat', macros.fat)}</TableCell>
      <TableCell className='py-2 text-left w-[4%] pl-0.5'>
        <span className='text-muted-foreground'>{formatMacro('saturatedFat', macros.saturatedFat)}</span>
      </TableCell>

      <TableCell className='py-2 text-right w-[7%]'>{formatMacro('fibre', macros.fibre)}</TableCell>
      <TableCell className='py-2 text-right w-[7%]'>{formatMacro('salt', macros.salt)}</TableCell>

      <TableCell className='py-1 w-[6%] pr-4 text-right'>
        {isEditable && (
          <Button
            variant='ghost'
            size='icon'
            className='h-6 w-6 text-muted-foreground hover:text-destructive cursor-pointer'
            onClick={onRemove}
            title='Remove ingredient'
          >
            <X className='h-3.5 w-3.5' />
          </Button>
        )}
      </TableCell>
    </TableRow>
  );
}
