import { z } from 'zod';

import {
  AddFoodRequestSchema,
  AddMealPlanRequestSchema,
  AddMealRequestSchema,
  FoodSchema,
  IngredientFormSchema,
  IngredientSchema,
  MealFormSchema,
  MealPlanFormSchema,
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
  status: number;
  details?: ValidationProblemDetails;

  constructor(message: string, status: number, details?: ValidationProblemDetails) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
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

// *FormData is what the form holds (food may be null); *FormResult is what a valid submit yields.
export type IngredientFormData = z.input<typeof IngredientFormSchema>;

export type MealFormData = z.input<typeof MealFormSchema>;

export type MealFormResult = z.output<typeof MealFormSchema>;

export type MealPlanFormData = z.input<typeof MealPlanFormSchema>;

export type MealPlanFormResult = z.output<typeof MealPlanFormSchema>;

export type UpdateMealPlanRequest = z.infer<typeof UpdateMealPlanRequestSchema>;

export type Macros = {
  calories: number;
  protein: number;
  carbohydrates: number;
  sugar: number;
  fat: number;
  saturatedFat: number;
  fibre: number;
  salt: number;
};
