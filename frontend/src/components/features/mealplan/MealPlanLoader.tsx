import { useQuery } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';

import { getMealPlan } from '@/api/mealPlan';
import MealPlanEditor from '@/components/features/mealplan/MealPlanEditor';

export default function MealPlanLoader() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, dataUpdatedAt } = useQuery({
    queryKey: ['meal-plan', id],
    queryFn: () => getMealPlan(id ?? ''),
    enabled: !!id
  });

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!data) {
    return <div>No data found</div>;
  }

  return <MealPlanEditor key={`${id}-${dataUpdatedAt}`} initialState={data} />;
}
