import { clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';

import type { IngredientFormData, Macros } from '@/types';
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

// matches the backend SearchTermValidator
export const MIN_SEARCH_LENGTH = 3;

export const defaultQuantity = (unit: 'Grams' | 'Count'): number => (unit === 'Count' ? 1 : 100);

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

export const MACRO_KEYS = [
  'calories',
  'protein',
  'carbohydrates',
  'sugar',
  'fat',
  'saturatedFat',
  'fibre',
  'salt'
] as const satisfies readonly (keyof Macros)[];

const mapMacros = (fn: (key: keyof Macros) => number): Macros =>
  Object.fromEntries(MACRO_KEYS.map(key => [key, fn(key)])) as Macros;

// Display precision per macro (6.7): used everywhere macros are shown.
export const MACRO_DECIMALS: Macros = {
  calories: 0,
  protein: 1,
  carbohydrates: 1,
  sugar: 1,
  fat: 1,
  saturatedFat: 1,
  fibre: 1,
  salt: 2
};

export const formatMacro = (key: keyof Macros, value: number): number => round(value, MACRO_DECIMALS[key]);

export const unitLabel = (unit: 'Grams' | 'Count'): string => (unit === 'Count' ? 'pcs' : 'g');

export const ZERO_MACROS: Macros = mapMacros(() => 0);

export const sumMacros = (a: Macros, b: Macros): Macros => mapMacros(key => a[key] + b[key]);

export const ingredientMacros = ({ food, quantity }: IngredientFormData): Macros =>
  food ? mapMacros(key => scaleMacro(food[key], quantity, food.referenceQuantity)) : ZERO_MACROS;

export const calculateMealMacros = (ingredients: IngredientFormData[]): Macros =>
  ingredients.map(ingredientMacros).reduce(sumMacros, ZERO_MACROS);

export const calculatePlanMacros = (meals: { ingredients: IngredientFormData[] }[]): Macros =>
  meals.map(meal => calculateMealMacros(meal.ingredients)).reduce(sumMacros, ZERO_MACROS);
