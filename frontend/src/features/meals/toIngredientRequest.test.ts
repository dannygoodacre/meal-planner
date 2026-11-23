import { expect, it } from 'vitest';

import { toIngredientRequest } from '@/features/meals/toIngredientRequest';

import type { Food } from '@/types';

it('sends only the food id and quantity', () => {
  const food = { id: '0b6b9c1e-3f1a-4d6e-9c55-2a1d4b0f7e21', name: 'Oats' } as Food;

  expect(toIngredientRequest({ food, quantity: 40 })).toEqual({ foodId: food.id, quantity: 40 });
});
