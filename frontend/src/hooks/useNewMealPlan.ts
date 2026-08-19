import { useMutation, useQueryClient } from '@tanstack/react-query';

import { addMealPlan } from '@/api/mealPlan';

import type { AddMealPlanRequest } from '@/types';

export default function useNewMealPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (mealPlan: AddMealPlanRequest) => await addMealPlan(mealPlan),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['meal-plans'] }),

    onError: () => {
      console.log('error occurred');
    }
  });
}
