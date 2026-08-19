import { useMutation, useQueryClient } from '@tanstack/react-query';

import { updateMealPlan } from '@/api/mealPlan';

import type { ApiError, UpdateMealPlanRequest } from '@/types';

export default function useUpdateMealPlan() {
  const queryClient = useQueryClient();

  return useMutation<string, ApiError, { id: string; mealPlan: UpdateMealPlanRequest }>({
    mutationFn: ({ id, mealPlan }) => updateMealPlan(id, mealPlan),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['meal-plan'] })
  });
}
