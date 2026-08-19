import { useState } from 'react';
import { X } from 'lucide-react';
import { Controller } from 'react-hook-form';

import FoodSearchPopover from '@/components/common/FoodSearchPopover';
import NumberInput from '@/components/common/NumberInput';
import { Button } from '@/components/ui/button';
import { TableCell, TableRow } from '@/components/ui/table';
import { cn } from '@/utils';

import type { Food, MealFormData } from '@/types';
import type { Control, UseFieldArrayRemove, UseFormSetValue } from 'react-hook-form';

export interface IngredientRowProps {
  index: number;
  setValue: UseFormSetValue<MealFormData>;
  remove: UseFieldArrayRemove;
  foodName: string;
  unit: 'Grams' | 'Count';
  control: Control<MealFormData>;
}

export default function IngredientRow({ index, setValue, remove, foodName, unit, control }: IngredientRowProps) {
  const [isExpanded] = useState(false);
  const [isSelected, setIsSelected] = useState(false);

  const handleSearchChange = (newValue: string) => {
    setValue(`ingredients.${index}.food.name`, newValue);

    setIsSelected(false);

    setValue(`ingredients.${index}.quantity`, 0);
    setValue(`ingredients.${index}.food.id`, '');
    setValue(`ingredients.${index}.food.calories`, 0);
    setValue(`ingredients.${index}.food.protein`, 0);
    setValue(`ingredients.${index}.food.carbohydrates`, 0);
    setValue(`ingredients.${index}.food.sugar`, 0);
    setValue(`ingredients.${index}.food.fat`, 0);
    setValue(`ingredients.${index}.food.saturatedFat`, 0);
    setValue(`ingredients.${index}.food.fibre`, 0);
    setValue(`ingredients.${index}.food.salt`, 0);
  };

  const handleFoodSelect = (food: Food) => {
    setValue(`ingredients.${index}`, {
      quantity: 0,
      food: food
    });

    setIsSelected(true);
  };

  return (
    <TableRow className={cn('group', isExpanded && 'bg-muted/20 border-b-0')}>
      <TableCell />

      <TableCell>
        <FoodSearchPopover
          foodName={foodName}
          isSelected={isSelected}
          disabledSearch={isExpanded}
          onSearchChange={handleSearchChange}
          onSelect={handleFoodSelect}
        />
      </TableCell>

      <TableCell>
        <div className='relative flex items-center'>
          <Controller
            control={control}
            name={`ingredients.${index}.quantity`}
            render={({ field: { onChange, value } }) => (
              <NumberInput value={value} onChange={onChange} className='pr-13' disabled={!isSelected} />
            )}
          />

          <span
            className={cn(
              'absolute right-3 text-sm text-muted-foreground pointer-events-none transition-opacity',
              !isSelected && 'opacity-40 cursor-not-allowed'
            )}
          >
            {unit === 'Grams' ? 'g' : 'count'}
          </span>
        </div>
      </TableCell>

      <TableCell className='flex justify-end'>
        <Button
          variant='ghost'
          type='button'
          size='icon'
          onClick={() => remove(index)}
          className='h-8 w-8 text-muted-foreground hover:text-destructive cursor-pointer'
        >
          <X className='h-4 w-4 stroke-[2.5]' />
        </Button>
      </TableCell>
    </TableRow>
  );
}
