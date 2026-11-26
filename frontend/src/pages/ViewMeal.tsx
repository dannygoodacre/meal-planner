import { getMeal } from '@/api/meal';
import { queryKeys } from '@/api/queryKeys';
import DetailLoader from '@/components/DetailLoader';
import MealForm from '@/features/meals/MealForm';

export default function ViewMealPage() {
  return (
    <DetailLoader queryKey={queryKeys.meal.detail} queryFn={getMeal}>
      {meal => <MealForm initialState={meal} />}
    </DetailLoader>
  );
}
