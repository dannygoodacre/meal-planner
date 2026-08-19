import { useMutation, useQueryClient } from '@tanstack/react-query';

import { addMeal } from '@/api/meal';

import type { AddMealRequest } from '@/types';

export default function useNewMeal() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (meal: AddMealRequest) => await addMeal(meal),

    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['meals'] }),

    onError: () => {
      console.log('error occurred');
    }
  });
}
