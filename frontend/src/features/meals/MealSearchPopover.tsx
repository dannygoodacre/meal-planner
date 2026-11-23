import { mealSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary, SearchFooterAction } from '@/components/SearchPopover';
import { calculateMealMacros, macroSummary } from '@/macros';

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
      renderItem={meal => (
        <>
          <ItemSummary name={meal.name} summary={macroSummary(calculateMealMacros(meal.ingredients))} />
          <ItemBadge>
            {meal.ingredients.length} {meal.ingredients.length === 1 ? 'ingredient' : 'ingredients'}
          </ItemBadge>
        </>
      )}
      footer={({ hasExactMatch, close }) =>
        !hasExactMatch && <SearchFooterAction onClick={close}>Use "{value}" as the meal name</SearchFooterAction>
      }
    />
  );
}
