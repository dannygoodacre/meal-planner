import { useMutation, useQueryClient } from '@tanstack/react-query';

import { addMeal } from '@/api/meal';
import { queryKeys } from '@/api/queryKeys';

import type { AddMealRequest, ApiError } from '@/types';

export default function useNewMeal() {
  const queryClient = useQueryClient();

  return useMutation<string, ApiError, AddMealRequest>({
    mutationFn: async (meal: AddMealRequest) => await addMeal(meal),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.meal.all }),

    onError: () => {
      console.log('error occurred');
    }
  });
}
