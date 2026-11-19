import { mealPlanSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary } from '@/components/common/SearchPopover';
import { calculatePlanMacros, formatMacro } from '@/utils';

import type { MealPlan } from '@/types';

interface MealPlanSearchPopoverProps {
  value: string;
  onValueChange: (value: string) => void;
  onSelect: (plan: MealPlan) => void;
}

export default function MealPlanSearchPopover({ value, onValueChange, onSelect }: MealPlanSearchPopoverProps) {
  return (
    <SearchPopover
      value={value}
      onValueChange={onValueChange}
      onSelect={onSelect}
      searchQuery={mealPlanSearchQuery}
      placeholder='Search meal plans...'
      renderItem={plan => {
        const macros = calculatePlanMacros(plan.meals);

        return (
          <>
            <ItemSummary
              name={plan.name}
              summary={`${formatMacro('calories', macros.calories)} kcal | ${formatMacro('protein', macros.protein)}g P | ${formatMacro('carbohydrates', macros.carbohydrates)}g C | ${formatMacro('fat', macros.fat)}g F`}
            />
            <ItemBadge>
              {plan.meals.length} {plan.meals.length === 1 ? 'meal' : 'meals'}
            </ItemBadge>
          </>
        );
      }}
    />
  );
}
