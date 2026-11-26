import { get, post, put } from '@/api/client';

import type { AddMealRequest, Meal, SearchResponse } from '@/types';

export const addMeal = (meal: AddMealRequest): Promise<string> => post<string>('/api/meals', meal);

export const getMeal = (id: string): Promise<Meal> => get<Meal>(`/api/meals/${id}`);

export const updateMeal = (id: string, meal: AddMealRequest): Promise<void> => put<void>(`/api/meals/${id}`, meal);

export const searchMealsByName = (query: string): Promise<SearchResponse<Meal>> =>
  get<SearchResponse<Meal>>(`/api/meals?q=${encodeURIComponent(query)}`);
