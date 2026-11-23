import { searchFoodsByName } from '@/api/food';
import { searchMealsByName } from '@/api/meal';
import { searchMealPlansByName } from '@/api/mealPlan';

import type { Food, Meal, MealPlan, SearchResponse } from '@/types';
import type { QueryKey } from '@tanstack/react-query';

// Every key starts with its entity's `all`, so invalidating `all` covers details and searches.
export const queryKeys = {
  food: {
    all: ['food'] as const,
    detail: (id: string) => ['food', 'detail', id] as const,
    search: (term: string) => ['food', 'search', term] as const
  },
  meal: {
    all: ['meal'] as const,
    detail: (id: string) => ['meal', 'detail', id] as const,
    search: (term: string) => ['meal', 'search', term] as const
  },
  mealPlan: {
    all: ['meal-plan'] as const,
    detail: (id: string) => ['meal-plan', 'detail', id] as const,
    search: (term: string) => ['meal-plan', 'search', term] as const
  }
};

export interface SearchQuery<T> {
  queryKey: QueryKey;
  queryFn: () => Promise<SearchResponse<T>>;
}

export const foodSearchQuery = (term: string): SearchQuery<Food> => ({
  queryKey: queryKeys.food.search(term),
  queryFn: () => searchFoodsByName(term)
});

export const mealSearchQuery = (term: string): SearchQuery<Meal> => ({
  queryKey: queryKeys.meal.search(term),
  queryFn: () => searchMealsByName(term)
});

export const mealPlanSearchQuery = (term: string): SearchQuery<MealPlan> => ({
  queryKey: queryKeys.mealPlan.search(term),
  queryFn: () => searchMealPlansByName(term)
});
