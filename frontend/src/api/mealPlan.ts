import { get, post, put } from '@/api/client';

import type { AddMealPlanRequest, MealPlan, SearchResponse, UpdateMealPlanRequest } from '@/types';

export const addMealPlan = (mealPlan: AddMealPlanRequest): Promise<string> => post<string>('/api/meal-plans', mealPlan);

export const getMealPlan = (id: string): Promise<MealPlan> => get<MealPlan>(`/api/meal-plans/${id}`);

export const updateMealPlan = (id: string, mealPlan: UpdateMealPlanRequest): Promise<void> =>
  put<void>(`/api/meal-plans/${id}`, mealPlan);

export const searchMealPlansByName = (query: string): Promise<SearchResponse<MealPlan>> =>
  get<SearchResponse<MealPlan>>(`/api/meal-plans?q=${encodeURIComponent(query)}`);
