import { get, post } from '@/api/client';

import type { AddMealRequest, Meal, SearchResponse } from '@/types';

export const addMeal = (meal: AddMealRequest): Promise<string> => post<string>('/api/meals', meal);

export const searchMealsByName = (query: string): Promise<SearchResponse<Meal>> =>
  get<SearchResponse<Meal>>(`/api/meals?q=${encodeURIComponent(query)}`);
