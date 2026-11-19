import { useQuery } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';

import { getMealPlan } from '@/api/mealPlan';
import { queryKeys } from '@/api/queryKeys';
import Loading from '@/components/common/Loading';
import MealPlanForm from '@/components/features/mealplan/MealPlanForm';

export default function MealPlanLoader() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, isError, error, dataUpdatedAt } = useQuery({
    queryKey: queryKeys.mealPlan.detail(id ?? ''),
    queryFn: () => getMealPlan(id ?? ''),
    enabled: !!id,
    staleTime: Infinity
  });

  if (isLoading) {
    return <Loading />;
  }

  if (isError) {
    return <div>{error.message}</div>;
  }

  if (!data) {
    return <div>No data found</div>;
  }

  return <MealPlanForm key={`${id}-${dataUpdatedAt}`} initialState={data} />;
}
