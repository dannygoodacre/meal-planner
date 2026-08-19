import { get, post, put } from '@/api/client';

import type { AddMealPlanRequest, MealPlan, SearchResponse, UpdateMealPlanRequest } from '@/types';

export const addMealPlan = (food: AddMealPlanRequest): Promise<string> => post<string>(`/api/mealplan`, food);

export const getMealPlan = (id: string): Promise<MealPlan> => get<MealPlan>(`/api/mealplan/${id}`);

export const updateMealPlan = (id: string, mealPlan: UpdateMealPlanRequest): Promise<string> =>
  put<string>(`/api/mealplan/${id}`, mealPlan);

export const searchMealPlansByName = (query: string): Promise<SearchResponse<MealPlan>> =>
  get<SearchResponse<MealPlan>>(`/api/mealplan/search?q=${query}`);
