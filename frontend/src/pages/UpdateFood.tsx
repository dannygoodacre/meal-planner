import { getFood } from '@/api/food';
import { queryKeys } from '@/api/queryKeys';
import DetailLoader from '@/components/DetailLoader';
import UpdateFood from '@/features/food/UpdateFood';

export default function UpdateFoodPage() {
  return (
    <DetailLoader queryKey={queryKeys.food.detail} queryFn={getFood}>
      {food => <UpdateFood initialState={food} />}
    </DetailLoader>
  );
}
