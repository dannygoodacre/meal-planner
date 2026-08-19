import { z } from 'zod';

import {
  AddFoodRequestSchema,
  AddMealPlanRequestSchema,
  AddMealRequestSchema,
  FoodSchema,
  IngredientSchema,
  MacrosSchema,
  MealFormSchema,
  MealPlanSchema,
  MealSchema,
  UpdateMealPlanRequestSchema
} from '@/schema';

export interface SearchableItem {
  id: string;
  name: string;
}

export interface SearchResponse<TResponse> {
  query: string;
  count: number;
  items: TResponse[];
}

export class ApiError extends Error {
  details?: ValidationProblemDetails;

  constructor(message: string, details?: ValidationProblemDetails) {
    super(message);
    this.name = 'ApiError';
    this.details = details;
  }
}

export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  traceId?: string;
  errors?: Record<string, string[]>;
}

export type AddFoodRequest = z.infer<typeof AddFoodRequestSchema>;

export type AddMealRequest = z.infer<typeof AddMealRequestSchema>;

export type AddMealPlanRequest = z.infer<typeof AddMealPlanRequestSchema>;

export type Food = z.infer<typeof FoodSchema>;

export type Ingredient = z.infer<typeof IngredientSchema>;

export type Meal = z.infer<typeof MealSchema>;

export type MealPlan = z.infer<typeof MealPlanSchema>;

export type MealFormData = z.infer<typeof MealFormSchema>;

export type UpdateMealPlanRequest = z.infer<typeof UpdateMealPlanRequestSchema>;

export type Macros = z.infer<typeof MacrosSchema>;
