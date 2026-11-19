import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';

import { AddFoodRequestSchema } from '@/schema';

import type { AddFoodRequest } from '@/types';
import type { UseFormReturn } from 'react-hook-form';

export default function useFoodForm(defaultValues: AddFoodRequest): UseFormReturn<AddFoodRequest> {
  return useForm<AddFoodRequest>({ resolver: zodResolver(AddFoodRequestSchema), mode: 'onTouched', defaultValues });
}
