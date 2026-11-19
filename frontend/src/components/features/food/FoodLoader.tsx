import { useQuery } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';

import { getFood } from '@/api/food';
import { queryKeys } from '@/api/queryKeys';
import Loading from '@/components/common/Loading';
import UpdateFood from '@/components/features/food/UpdateFood';

export default function UpdateFoodLoader() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, isError, error, dataUpdatedAt } = useQuery({
    queryKey: queryKeys.food.detail(id ?? ''),
    queryFn: () => getFood(id ?? ''),
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

  return <UpdateFood key={`${id}-${dataUpdatedAt}`} initialState={data} />;
}
