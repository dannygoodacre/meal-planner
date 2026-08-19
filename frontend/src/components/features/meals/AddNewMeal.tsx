import { useState } from 'react';
import { Plus, Utensils } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { toast } from 'sonner';

import { searchMealsByName } from '@/api/meal';
import NutritionBox from '@/components/common/NutritionBox';
import SearchCheck from '@/components/common/SearchCheck';
import SubmitButton from '@/components/common/SubmitButton';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import IngredientRow from '@/components/features/meals/IngredientRow';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import useNewMeal from '@/hooks/useNewMeal';
import { calculateMealMacros } from '@/utils';

import type { AddMealRequest, MealFormData } from '@/types';

interface AddNewMealProps {
  name?: string;
  isNameEditable?: boolean;
}

export default function AddNewMeal({ name = '', isNameEditable = true }: AddNewMealProps) {
  const form = useForm<MealFormData>({
    defaultValues: {
      name: name,
      ingredients: [
        {
          quantity: 0,
          food: {
            id: '',
            name: '',
            unit: 'Grams',
            referenceQuantity: 0,
            calories: 0,
            protein: 0,
            carbohydrates: 0,
            sugar: 0,
            fat: 0,
            saturatedFat: 0,
            fibre: 0,
            salt: 0,
            source: ''
          }
        }
      ]
    }
  });

  const { fields, append, remove } = useFieldArray({ control: form.control, name: 'ingredients' });

  const ingredients = useWatch({ control: form.control, name: 'ingredients' }) || [];

  const [isDuplicate, setIsDuplicate] = useState(false);

  const { mutate, isPending } = useNewMeal();

  const onSubmit = (formData: MealFormData) => {
    const payload: AddMealRequest = {
      name: formData.name,
      ingredients: formData.ingredients.map(ingredient => {
        return {
          foodId: ingredient.food.id,
          quantity: ingredient.quantity
        };
      })
    };

    mutate(payload, {
      onSuccess: () => {
        toast.success('Successfully added meal');

        form.reset();
      },
      onError: showValidationErrorToast
    });
  };

  return (
    <Card>
      <CardHeader className='flex flex-row items-center justify-between'>
        <div>
          <CardTitle className='text-2xl flex items-center gap-2'>
            <Utensils className='h-6 w-6 text-primary' /> Create new meal
          </CardTitle>
        </div>
      </CardHeader>

      <CardContent className='space-y-6'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <div className='grid grid-cols-1 md:grid-cols-2 gap-4 mb-6'>
              <div className='col-span-1 md:col-span-2 space-y-4'>
                <SearchCheck
                  label='Meal name'
                  placeholder='e.g. Leg day steak & rice'
                  isNameEditable={isNameEditable}
                  search={searchMealsByName}
                  onMatchChange={setIsDuplicate}
                />
              </div>
            </div>

            <div className='rounded-md border'>
              <Table>
                <TableHeader className='bg-muted/50'>
                  <TableRow>
                    <TableHead className='w-2.5'></TableHead>
                    <TableHead>Ingredients</TableHead>
                    <TableHead className='w-35'>Quantity</TableHead>
                    <TableHead className='w-10'></TableHead>
                  </TableRow>
                </TableHeader>

                <TableBody>
                  {fields.map((field, index) => {
                    const currentIngredient = ingredients[index];

                    return (
                      <IngredientRow
                        key={field.id}
                        index={index}
                        remove={remove}
                        setValue={form.setValue}
                        foodName={currentIngredient?.food.name || ''}
                        unit={currentIngredient?.food.unit || 'Grams'}
                        control={form.control}
                      />
                    );
                  })}
                </TableBody>
              </Table>
            </div>

            <Button
              type='button'
              variant='outline'
              className='w-full border-dashed border-2 h-12 my-6'
              onClick={() =>
                append({
                  quantity: 0,
                  food: {
                    id: '',
                    name: '',
                    unit: 'Grams',
                    referenceQuantity: 0,
                    calories: 0,
                    protein: 0,
                    carbohydrates: 0,
                    sugar: 0,
                    fat: 0,
                    saturatedFat: 0,
                    fibre: 0,
                    salt: 0,
                    source: ''
                  }
                })
              }
            >
              <Plus className='mr-2 h-4 w-4' /> Add another ingredient
            </Button>

            <div className='py-6 border-t'>
              <NutritionBox totals={calculateMealMacros(ingredients)} />
            </div>

            <SubmitButton
              actionMessage='Save Meal'
              loadingMessage='Saving'
              isLoading={isPending}
              disabled={isPending || isDuplicate}
            />
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
