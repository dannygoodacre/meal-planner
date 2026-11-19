import { useMutation, useQueryClient } from '@tanstack/react-query';

import { addMealPlan } from '@/api/mealPlan';
import { queryKeys } from '@/api/queryKeys';

import type { AddMealPlanRequest, ApiError } from '@/types';

export default function useNewMealPlan() {
  const queryClient = useQueryClient();

  return useMutation<string, ApiError, AddMealPlanRequest>({
    mutationFn: async (mealPlan: AddMealPlanRequest) => await addMealPlan(mealPlan),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: queryKeys.mealPlan.all }),

    onError: () => {
      console.log('error occurred');
    }
  });
}
