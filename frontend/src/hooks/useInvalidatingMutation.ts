import { useMutation, useQueryClient } from '@tanstack/react-query';

import type { ApiError } from '@/api/client';
import type { QueryKey, UseMutationResult } from '@tanstack/react-query';

export default function useInvalidatingMutation<TData, TVariables>(
  mutationFn: (variables: TVariables) => Promise<TData>,
  invalidates: QueryKey[]
): UseMutationResult<TData, ApiError, TVariables> {
  const queryClient = useQueryClient();

  return useMutation<TData, ApiError, TVariables>({
    mutationFn,

    // Not returned: awaiting the refetch would delay the caller's onSuccess (form reset) until it lands.
    onSuccess: () => {
      for (const queryKey of invalidates) {
        void queryClient.invalidateQueries({ queryKey });
      }
    }
  });
}
