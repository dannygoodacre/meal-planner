import { get, post, put } from './client';

import type { AddFoodRequest, Food, SearchResponse } from '@/types';

export const addFood = (food: AddFoodRequest): Promise<string> => post<string>('/api/food', food);

export const getFood = (id: string): Promise<Food> => get<Food>(`/api/food/${id}`);

export const updateFood = (id: string, food: AddFoodRequest): Promise<string> => put<string>(`/api/food/${id}`, food);

export const searchFoodsByName = (query: string): Promise<SearchResponse<Food>> =>
  get<SearchResponse<Food>>(`/api/food/search?q=${query}`);
