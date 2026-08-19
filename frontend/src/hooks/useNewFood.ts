import { useMutation, useQueryClient } from '@tanstack/react-query';

import { addFood } from '@/api/food';

import type { AddFoodRequest, ApiError } from '@/types';

export default function useNewFood() {
  const queryClient = useQueryClient();

  return useMutation<string, ApiError, AddFoodRequest>({
    mutationFn: async (food: AddFoodRequest) => await addFood(food),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['foods'] })
  });
}
