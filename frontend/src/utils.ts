import { clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

import type { Ingredient, Macros, Meal } from '@/types';
import type { ClassValue } from 'clsx';

export const cn = (...inputs: ClassValue[]): string => twMerge(clsx(inputs));

export const toNormalizedString = (value: string | null | undefined): string =>
  !value || value.trim().length === 0
    ? ''
    : value
        .trim()
        .toLowerCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .normalize('NFC');

export const round = (num: number, decimals: number = 1): number => {
  const factor = Math.pow(10, decimals);

  return Math.round(num * factor) / factor;
};

export const scaleMacro = (macro: number, currentQuantity: number, referenceQuantity: number): number => {
  if (!currentQuantity || currentQuantity <= 0 || !referenceQuantity) {
    return 0;
  }
  return (macro / referenceQuantity) * currentQuantity;
};

export const calculateMealMacros = (ingredients: Ingredient[]): Macros =>
  ingredients.reduce(
    (totals, ingredient) => {
      const food = ingredient.food;

      const referenceQuantity = ingredient.food?.referenceQuantity || 100;

      const quantity = ingredient.quantity || 0;

      totals.calories += scaleMacro(food.calories, quantity, referenceQuantity);
      totals.protein += scaleMacro(food.protein, quantity, referenceQuantity);
      totals.carbohydrates += scaleMacro(food.carbohydrates, quantity, referenceQuantity);
      totals.sugar += scaleMacro(food.sugar, quantity, referenceQuantity);
      totals.fat += scaleMacro(food.fat, quantity, referenceQuantity);
      totals.saturatedFat += scaleMacro(food.saturatedFat, quantity, referenceQuantity);
      totals.fibre += scaleMacro(food.fibre, quantity, referenceQuantity);
      totals.salt += scaleMacro(food.salt, quantity, referenceQuantity);

      return totals;
    },
    { calories: 0, protein: 0, carbohydrates: 0, sugar: 0, fat: 0, saturatedFat: 0, fibre: 0, salt: 0 }
  );

export const calculatePlanMacros = (meals: Meal[]): Macros =>
  meals.reduce(
    (totals, meal) => {
      const mealTotals = calculateMealMacros(meal.ingredients || []);

      totals.calories += mealTotals.calories;
      totals.protein += mealTotals.protein;
      totals.carbohydrates += mealTotals.carbohydrates;
      totals.sugar += mealTotals.sugar;
      totals.fat += mealTotals.fat;
      totals.saturatedFat += mealTotals.saturatedFat;
      totals.fibre += mealTotals.fibre;
      totals.salt += mealTotals.salt;

      return totals;
    },
    { calories: 0, protein: 0, carbohydrates: 0, sugar: 0, fat: 0, saturatedFat: 0, fibre: 0, salt: 0 }
  );
