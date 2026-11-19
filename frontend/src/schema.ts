import { z } from 'zod';

// These schemas validate form input. API responses are cast to the inferred types in api/, not parsed.

const requiredString = z.string().min(1, 'Required');

// Number inputs hold '' while being typed; show "Required" instead of zod's type message.
const number = z.number({ error: issue => (issue.code === 'invalid_type' ? 'Required' : undefined) });

const nonNegativeNumber = number.min(0, 'Must be 0 or more');

const positiveInteger = number.int('Must be a whole number').positive('Must be more than 0');

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
  referenceQuantity: positiveInteger,
  calories: number.int('Must be a whole number').min(0, 'Must be 0 or more'),
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

export const MealSchema = z.object({
  id: z.guid(),
  name: requiredString,
  ingredients: z.array(IngredientSchema)
});

// Form rows hold the whole picked food (for live macros), or null until one is picked; only its id is sent.
export const IngredientFormSchema = z.object({
  food: z
    .custom<z.infer<typeof FoodSchema> | null>()
    .refine(food => food !== null, 'Pick a food')
    .transform(food => food as z.infer<typeof FoodSchema>),
  quantity: positiveInteger
});

const IngredientsFormSchema = z
  .array(IngredientFormSchema)
  .min(1, 'Add at least one ingredient')
  .superRefine((ingredients, ctx) => {
    const seen = new Set<string>();

    ingredients.forEach((ingredient, index) => {
      if (seen.has(ingredient.food.id)) {
        ctx.addIssue({ code: 'custom', message: 'This food is already in the meal', path: [index, 'food'] });
      }

      seen.add(ingredient.food.id);
    });
  });

export const MealFormSchema = z.object({
  name: requiredString,
  ingredients: IngredientsFormSchema
});

export const MealPlanFormSchema = z.object({
  name: requiredString,
  meals: z
    .array(
      z.object({
        id: z.guid().nullable(),
        name: requiredString,
        ingredients: IngredientsFormSchema
      })
    )
    .min(1, 'Add at least one meal')
});

export const AddMealPlanMealDtoSchema = z.object({
  name: requiredString,
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
  name: requiredString,
  ingredients: z.array(IngredientDtoSchema)
});

export const UpdateMealPlanRequestSchema = z.object({
  name: requiredString,
  meals: z.array(UpdateMealPlanMealRequestSchema)
});
