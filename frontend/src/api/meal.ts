import { get, post } from './client';

import type { AddMealRequest, Meal, SearchResponse } from '@/types';

export const addMeal = (food: AddMealRequest): Promise<string> => post<string>(`/api/meals`, food);

export const getMeal = (id: string): Promise<Meal> => get<Meal>(`/api/meals/${id}`);

export const searchMealsByName = (query: string): Promise<SearchResponse<Meal>> =>
  get<SearchResponse<Meal>>(`/api/meals?q=${encodeURIComponent(query)}`);
