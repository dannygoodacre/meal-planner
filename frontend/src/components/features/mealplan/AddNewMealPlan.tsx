import { useState } from 'react';
import { List, Plus, Save } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { toast } from 'sonner';

import NutritionBox from '@/components/common/NutritionBox';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import { MealRow } from '@/components/features/mealplan/MealRow';
import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import useNewMealPlan from '@/hooks/useNewMealPlan';
import { calculatePlanMacros } from '@/utils';

import type { AddMealPlanRequest, MealPlan } from '@/types';

export default function AddNewMealPlan() {
  const [initialMealId] = useState(() => `temp-${crypto.randomUUID()}`);

  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({
    [initialMealId]: true
  });

  const { mutate, isPending } = useNewMealPlan();

  const form = useForm<MealPlan>({
    mode: 'onSubmit',
    defaultValues: {
      id: '',
      name: '',
      meals: [
        {
          id: initialMealId,
          name: '',
          ingredients: []
        }
      ]
    }
  });

  const {
    fields: meals,
    append: appendMeal,
    remove: removeMeal
  } = useFieldArray({
    control: form.control,
    name: 'meals'
  });

  const allLiveMeals = useWatch({ control: form.control, name: 'meals' }) || [];

  const toggleRow = (mealId: string) => setExpandedRows(prev => ({ ...prev, [mealId]: !prev[mealId] }));

  const setRowExpandedState = (mealId: string, isExpanded: boolean) =>
    setExpandedRows(prev => ({ ...prev, [mealId]: isExpanded }));

  const handleAddNewMeal = () => {
    const newMealId = `temp-${crypto.randomUUID()}`;

    appendMeal({ id: newMealId, name: '', ingredients: [] });

    setExpandedRows(prev => ({ ...prev, [newMealId]: true }));
  };

  function onSubmit(formData: MealPlan) {
    const requestData: AddMealPlanRequest = {
      name: formData.name,
      meals: formData.meals.map(meal => ({
        name: meal.name,
        mealId: null,
        ingredients: meal.ingredients.map(ingredient => ({
          foodId: ingredient.food.id,
          quantity: ingredient.quantity
        }))
      }))
    };

    mutate(requestData, {
      onSuccess: () => {
        toast.success('Meal plan created');
      },
      onError: showValidationErrorToast
    });
  }

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className='w-full max-w-5xl mx-auto p-4 border rounded-xl shadow-sm bg-card'
      >
        <div className='flex justify-between items-center mb-4 gap-4'>
          <div className='flex items-center gap-2 flex-1 min-w-0'>
            <List className='h-6 w-6 text-primary shrink-0' />

            <FormField
              control={form.control}
              name='name'
              render={({ field }) => (
                <FormItem className='w-full max-w-md'>
                  <FormControl>
                    <Input
                      {...field}
                      type='text'
                      placeholder='Meal plan name...'
                      className='h-10 text-xl font-bold'
                      autoFocus
                    />
                  </FormControl>
                </FormItem>
              )}
            />
          </div>

          <div className='flex items-center gap-2 shrink-0'>
            <Button type='submit' disabled={isPending} variant='default' className='cursor-pointer'>
              <Save className='mr-2 h-4 w-4' /> {isPending ? 'Creating...' : 'Save Plan'}
            </Button>
          </div>
        </div>

        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className='w-10' />
              <TableHead>Meal</TableHead>
              <TableHead>Calories (kcal)</TableHead>
              <TableHead>Protein (g)</TableHead>
              <TableHead>Carbs (g)</TableHead>
              <TableHead>Fat (g)</TableHead>
              <TableHead>Fibre (g)</TableHead>
              <TableHead className='w-10' />
            </TableRow>
          </TableHeader>

          <TableBody>
            {meals.map((mealField, index) => {
              const currentMeal = allLiveMeals[index] || mealField;

              return (
                <MealRow
                  key={mealField.id}
                  control={form.control}
                  mealIndex={index}
                  mealName={currentMeal.name}
                  isEditable={true}
                  isExpanded={!!expandedRows[mealField.id]}
                  setValue={form.setValue}
                  onToggleExpand={() => toggleRow(mealField.id)}
                  onForceExpand={() => setRowExpandedState(mealField.id, true)}
                  onDeleteMeal={() => removeMeal(index)}
                />
              );
            })}
          </TableBody>
        </Table>

        <div className='mt-4'>
          <Button
            type='button'
            variant='secondary'
            onClick={handleAddNewMeal}
            className='w-full border-dashed border-2 h-12 cursor-pointer'
          >
            <Plus className='mr-2 h-4 w-4' /> Add another meal
          </Button>
        </div>

        <div className='pt-6 mt-4 border-t'>
          <NutritionBox totals={calculatePlanMacros(allLiveMeals)} />
        </div>
      </form>
    </Form>
  );
}
