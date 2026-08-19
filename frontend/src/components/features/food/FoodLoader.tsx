import { useQuery } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';

import { getFood } from '@/api/food';
import UpdateFood from '@/components/features/food/UpdateFood';

export default function UpdateFoodLoader() {
  const { id } = useParams<{ id: string }>();

  const { data, isLoading, dataUpdatedAt } = useQuery({
    queryKey: ['food', id],
    queryFn: () => getFood(id ?? ''),
    enabled: !!id
  });

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (!data) {
    return <div>No data found</div>;
  }

  return <UpdateFood key={`${id}-${dataUpdatedAt}`} initialState={data} />;
}
