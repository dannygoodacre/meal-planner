import type { AddMealRequest, Food } from '@/types';

export const toIngredientRequest = ({
  food,
  quantity
}: {
  food: Food;
  quantity: number;
}): AddMealRequest['ingredients'][number] => ({ foodId: food.id, quantity });
