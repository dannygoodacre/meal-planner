import { z } from 'zod';

const requiredString = z.string().min(1, 'Required');

const nonNegativeNumber = z.number().min(0);

const positiveInteger = z.number().int().positive();

const positiveNumber = z.number().positive();

interface SubMacroFields {
  sugar: number;
  carbohydrates: number;
  saturatedFat: number;
  fat: number;
}

const applyFoodMacroRefinements = <T extends z.ZodType<SubMacroFields>>(schema: T): T =>
  schema
    .refine((x: SubMacroFields) => x.sugar <= x.carbohydrates, {
      message: 'Sugars must not exceed total carbohydrates',
      path: ['sugar']
    })
    .refine((x: SubMacroFields) => x.saturatedFat <= x.fat, {
      message: 'Saturated fat must not exceed total fat',
      path: ['saturatedFat']
    });

export const IngredientDtoSchema = z.object({
  foodId: z.guid(),
  quantity: positiveInteger
});

const BaseFoodSchema = z.object({
  name: requiredString,
  unit: z.enum(['Grams', 'Count']),
  referenceQuantity: z.number().int().min(1),
  calories: z.number().int().min(0),
  protein: nonNegativeNumber,
  carbohydrates: nonNegativeNumber,
  sugar: nonNegativeNumber,
  fat: nonNegativeNumber,
  saturatedFat: nonNegativeNumber,
  fibre: nonNegativeNumber,
  salt: nonNegativeNumber,
  source: z.url('Must be a valid URL')
});

export const AddFoodRequestSchema = applyFoodMacroRefinements(BaseFoodSchema);

export const FoodSchema = applyFoodMacroRefinements(
  BaseFoodSchema.extend({
    id: z.guid()
  })
);

export const IngredientSchema = z.object({
  quantity: positiveInteger,
  food: FoodSchema
});

export const AddMealRequestSchema = z.object({
  name: requiredString,
  ingredients: z.array(IngredientDtoSchema)
});

export const MealFormSchema = z.object({
  name: requiredString,
  ingredients: z.array(IngredientSchema)
});

export const MealSchema = MealFormSchema.extend({
  id: z.guid()
});

export const AddMealPlanMealDtoSchema = z.object({
  name: z.string().min(1).nullable(),
  mealId: z.guid().nullable(),
  ingredients: z.array(IngredientDtoSchema).default([])
});

export const AddMealPlanRequestSchema = z.object({
  name: requiredString,
  meals: z.array(AddMealPlanMealDtoSchema)
});

export const MealPlanSchema = z.object({
  id: z.guid(),
  name: requiredString,
  meals: z.array(MealSchema)
});

export const UpdateMealPlanMealRequestSchema = z.object({
  id: z.guid().nullable(),
  templateMealId: z.guid().nullable(),
  name: requiredString,
  ingredients: z.array(IngredientDtoSchema)
});

export const UpdateMealPlanRequestSchema = z.object({
  name: requiredString,
  meals: z.array(UpdateMealPlanMealRequestSchema)
});

export const MacrosSchema = z.object({
  calories: positiveNumber,
  protein: positiveNumber,
  carbohydrates: positiveNumber,
  sugar: positiveNumber,
  fat: positiveNumber,
  saturatedFat: positiveNumber,
  fibre: positiveNumber,
  salt: positiveNumber
});
