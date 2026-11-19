import { Plus } from 'lucide-react';

import { mealSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary } from '@/components/common/SearchPopover';
import { calculateMealMacros, formatMacro } from '@/utils';

import type { Meal } from '@/types';

interface MealSearchPopoverProps {
  value: string;
  onValueChange: (value: string) => void;
  onSelect: (meal: Meal) => void;
}

export default function MealSearchPopover({ value, onValueChange, onSelect }: MealSearchPopoverProps) {
  return (
    <SearchPopover
      value={value}
      onValueChange={onValueChange}
      onSelect={onSelect}
      searchQuery={mealSearchQuery}
      placeholder='Search standalone meals...'
      renderItem={meal => {
        const macros = calculateMealMacros(meal.ingredients);

        return (
          <>
            <ItemSummary
              name={meal.name}
              summary={`${formatMacro('calories', macros.calories)} kcal | ${formatMacro('protein', macros.protein)}g P | ${formatMacro('carbohydrates', macros.carbohydrates)}g C | ${formatMacro('fat', macros.fat)}g F`}
            />
            <ItemBadge>
              {meal.ingredients.length} {meal.ingredients.length === 1 ? 'ingredient' : 'ingredients'}
            </ItemBadge>
          </>
        );
      }}
      footer={({ hasExactMatch, close }) =>
        !hasExactMatch && (
          <button
            type='button'
            onClick={close}
            className='w-full text-left p-3 text-blue-600 hover:bg-blue-50 text-xs font-bold flex items-center gap-2 cursor-pointer transition-colors'
          >
            <Plus className='h-4 w-4 shrink-0' />
            <span className='truncate'>Use "{value}" as the meal name</span>
          </button>
        )
      }
    />
  );
}
