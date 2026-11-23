import { getMealPlan } from '@/api/mealPlan';
import { queryKeys } from '@/api/queryKeys';
import DetailLoader from '@/components/DetailLoader';
import MealPlanForm from '@/features/mealplan/MealPlanForm';

export default function ViewMealPlanPage() {
  return (
    <DetailLoader queryKey={queryKeys.mealPlan.detail} queryFn={getMealPlan}>
      {plan => <MealPlanForm initialState={plan} />}
    </DetailLoader>
  );
}
