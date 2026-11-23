import { mealPlanSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary } from '@/components/SearchPopover';
import { calculatePlanMacros, macroSummary } from '@/macros';

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
      renderItem={plan => (
        <>
          <ItemSummary name={plan.name} summary={macroSummary(calculatePlanMacros(plan.meals))} />
          <ItemBadge>
            {plan.meals.length} {plan.meals.length === 1 ? 'meal' : 'meals'}
          </ItemBadge>
        </>
      )}
    />
  );
}
