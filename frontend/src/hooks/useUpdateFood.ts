import { useMutation, useQueryClient } from '@tanstack/react-query';

import { updateFood } from '@/api/food';
import { queryKeys } from '@/api/queryKeys';

import type { AddFoodRequest, ApiError } from '@/types';

export default function useUpdateFood() {
  const queryClient = useQueryClient();

  return useMutation<void, ApiError, { id: string; food: AddFoodRequest }>({
    mutationFn: async ({ id, food }) => await updateFood(id, food),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.food.all })
  });
}
