import { get, post, put } from '@/api/client';

import type { AddFoodRequest, Food, SearchResponse } from '@/types';

export const addFood = (food: AddFoodRequest): Promise<string> => post<string>('/api/foods', food);

export const getFood = (id: string): Promise<Food> => get<Food>(`/api/foods/${id}`);

export const updateFood = (id: string, food: AddFoodRequest): Promise<void> => put<void>(`/api/foods/${id}`, food);

export const searchFoodsByName = (query: string): Promise<SearchResponse<Food>> =>
  get<SearchResponse<Food>>(`/api/foods?q=${encodeURIComponent(query)}`);
