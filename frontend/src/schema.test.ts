import { describe, expect, it } from 'vitest';

import { AddFoodRequestSchema, FoodSchema, MealFormSchema, MealPlanFormSchema } from '@/schema';

import type { z } from 'zod';

// Each case mirrors a rule in the backend's FoodDetailsValidator, MealDetailsRules or MealPlanDetailsValidator.

const errorPaths = (schema: z.ZodType, value: unknown): string[] => {
  const result = schema.safeParse(value);

  return result.success ? [] : result.error.issues.map(issue => issue.path.join('.'));
};

const food = {
  name: 'Oats',
  unit: 'Grams',
  referenceQuantity: 100,
  calories: 379,
  protein: 13.2,
  carbohydrates: 67.7,
  sugar: 1,
  fat: 6.5,
  saturatedFat: 1.1,
  fibre: 10.1,
  salt: 0,
  source: 'https://example.com/oats'
};

const savedFood = { ...food, id: '0b6b9c1e-3f1a-4d6e-9c55-2a1d4b0f7e21' };
const otherFood = { ...savedFood, id: '5d1f0a7c-8e2b-4c3d-a9f6-1b7e4c2d9a30' };

describe('AddFoodRequestSchema', () => {
  it('accepts a valid food', () => {
    expect(errorPaths(AddFoodRequestSchema, food)).toEqual([]);
  });

  it.each([
    ['name', { name: '' }],
    ['name', { name: '   ' }],
    ['unit', { unit: 'Litres' }],
    ['referenceQuantity', { referenceQuantity: 0 }],
    ['calories', { calories: -1 }],
    ['protein', { protein: -0.1 }],
    ['carbohydrates', { carbohydrates: -0.1 }],
    ['sugar', { sugar: -0.1 }],
    ['fat', { fat: -0.1 }],
    ['saturatedFat', { saturatedFat: -0.1 }],
    ['fibre', { fibre: -0.1 }],
    ['salt', { salt: -0.1 }],
    ['source', { source: 'not a url' }],
    ['source', { source: 'ftp://example.com/oats' }],
    ['source', { source: 'mailto:oats@example.com' }],
    ['sugar', { sugar: 70, carbohydrates: 67.7 }],
    ['saturatedFat', { saturatedFat: 7, fat: 6.5 }]
  ])('rejects an invalid %s', (path, override) => {
    // toContain: a negative total also trips its sub-macro refinement, as on the backend.
    expect(errorPaths(AddFoodRequestSchema, { ...food, ...override })).toContain(path);
  });

  it('allows sugar equal to carbohydrates and saturated fat equal to fat', () => {
    expect(errorPaths(AddFoodRequestSchema, { ...food, sugar: 5, carbohydrates: 5, saturatedFat: 2, fat: 2 })).toEqual(
      []
    );
  });
});

describe('FoodSchema', () => {
  it('requires a guid id', () => {
    expect(errorPaths(FoodSchema, savedFood)).toEqual([]);
    expect(errorPaths(FoodSchema, { ...savedFood, id: 'nope' })).toEqual(['id']);
  });
});

describe('MealFormSchema', () => {
  const meal = { name: 'Breakfast', ingredients: [{ food: savedFood, quantity: 40 }] };

  it('accepts a valid meal', () => {
    expect(errorPaths(MealFormSchema, meal)).toEqual([]);
  });

  it.each([
    ['name', { name: '' }],
    ['ingredients', { ingredients: [] }],
    ['ingredients.0.food', { ingredients: [{ food: null, quantity: 40 }] }],
    ['ingredients.0.quantity', { ingredients: [{ food: savedFood, quantity: 0 }] }],
    ['ingredients.0.quantity', { ingredients: [{ food: savedFood, quantity: 1.5 }] }],
    [
      'ingredients.1.food',
      {
        ingredients: [
          { food: savedFood, quantity: 40 },
          { food: savedFood, quantity: 10 }
        ]
      }
    ]
  ])('rejects an invalid %s', (path, override) => {
    expect(errorPaths(MealFormSchema, { ...meal, ...override })).toEqual([path]);
  });

  it('allows different foods', () => {
    const ingredients = [
      { food: savedFood, quantity: 40 },
      { food: otherFood, quantity: 10 }
    ];

    expect(errorPaths(MealFormSchema, { ...meal, ingredients })).toEqual([]);
  });
});

describe('MealPlanFormSchema', () => {
  const plan = {
    name: 'Week 1',
    meals: [{ id: null, name: 'Breakfast', ingredients: [{ food: savedFood, quantity: 40 }] }]
  };

  it('accepts a valid plan', () => {
    expect(errorPaths(MealPlanFormSchema, plan)).toEqual([]);
  });

  it.each([
    ['name', { name: '' }],
    ['meals', { meals: [] }],
    ['meals.0.name', { meals: [{ ...plan.meals[0], name: '' }] }],
    ['meals.0.ingredients', { meals: [{ ...plan.meals[0], ingredients: [] }] }]
  ])('rejects an invalid %s', (path, override) => {
    expect(errorPaths(MealPlanFormSchema, { ...plan, ...override })).toEqual([path]);
  });
});
