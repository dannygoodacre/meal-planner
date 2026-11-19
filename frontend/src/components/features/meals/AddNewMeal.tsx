import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Plus, Utensils } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { toast } from 'sonner';

import { mealSearchQuery } from '@/api/queryKeys';
import FormArrayMessage from '@/components/common/FormArrayMessage';
import NutritionBox from '@/components/common/NutritionBox';
import SearchCheck from '@/components/common/SearchCheck';
import SubmitButton from '@/components/common/SubmitButton';
import { showNameConflictOrToast } from '@/components/common/ValidationErrorToast.tsx';
import IngredientRow from '@/components/features/meals/IngredientRow';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import useNewMeal from '@/hooks/useNewMeal';
import { MealFormSchema } from '@/schema';
import { calculateMealMacros } from '@/utils';

import type { AddMealRequest, MealFormData, MealFormResult } from '@/types';

interface AddNewMealProps {
  name?: string;
  isNameEditable?: boolean;
}

export default function AddNewMeal({ name = '', isNameEditable = true }: AddNewMealProps) {
  const form = useForm<MealFormData, unknown, MealFormResult>({
    resolver: zodResolver(MealFormSchema),
    defaultValues: {
      name: name,
      ingredients: [{ food: null, quantity: 0 }]
    }
  });

  const { fields, append, remove } = useFieldArray({ control: form.control, name: 'ingredients' });

  const ingredients = useWatch({ control: form.control, name: 'ingredients' }) || [];

  const [isDuplicate, setIsDuplicate] = useState(false);

  const { mutate, isPending } = useNewMeal();

  const onSubmit = (formData: MealFormResult) => {
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
      onError: showNameConflictOrToast(form.setError)
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
                  searchQuery={mealSearchQuery}
                  onMatchChange={setIsDuplicate}
                />
              </div>
            </div>

            <div className='rounded-md border'>
              <Table className='table-fixed w-full'>
                <TableHeader className='bg-muted/50'>
                  <TableRow>
                    <TableHead className='w-[6%]' />
                    <TableHead className='text-xs w-[24%]'>Ingredient</TableHead>
                    <TableHead className='text-xs w-[6%]'>Qty</TableHead>
                    <TableHead className='text-xs text-right w-[11%]'>Calories</TableHead>
                    <TableHead className='text-xs text-right w-[11%]'>Protein</TableHead>
                    <TableHead className='text-xs text-right w-[7%] pr-0.5'>Carbs</TableHead>
                    <TableHead className='text-xs text-left w-[4%] pl-0.5'>
                      <span className='text-muted-foreground'>(Sugars)</span>
                    </TableHead>
                    <TableHead className='text-xs text-right w-[7%] pr-0.5'>Fat</TableHead>
                    <TableHead className='text-xs text-left w-[4%] pl-0.5'>
                      <span className='text-muted-foreground'>(Sat)</span>
                    </TableHead>
                    <TableHead className='text-xs text-right w-[7%]'>Fibre</TableHead>
                    <TableHead className='text-xs text-right w-[7%]'>Salt</TableHead>
                    <TableHead className='w-[6%]' />
                  </TableRow>
                </TableHeader>

                <TableBody>
                  {fields.map((field, index) => (
                    <IngredientRow
                      key={field.id}
                      name={`ingredients.${index}`}
                      isEditable={true}
                      onRemove={() => remove(index)}
                    />
                  ))}
                </TableBody>
              </Table>
            </div>

            <FormArrayMessage error={form.formState.errors.ingredients} />

            <Button
              type='button'
              variant='outline'
              className='w-full border-dashed border-2 h-12 my-6'
              onClick={() => append({ food: null, quantity: 0 })}
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
