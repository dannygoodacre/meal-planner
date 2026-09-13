import { useState } from 'react';
import { List, Pencil, Plus, Save } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { toast } from 'sonner';

import NutritionBox from '@/components/common/NutritionBox';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import { MealRow } from '@/components/features/mealplan/MealRow';
import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import useUpdateMealPlan from '@/hooks/useUpdateMealPlan';
import { calculatePlanMacros } from '@/utils';

import type { MealPlan, UpdateMealPlanRequest } from '@/types';

interface MealPlanEditorProps {
  initialState: MealPlan;
}

export default function MealPlanEditor({ initialState }: MealPlanEditorProps) {
  const [isEditable, setIsEditable] = useState<boolean>(false);
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

  const { mutate, isPending } = useUpdateMealPlan();

  const form = useForm<MealPlan>({
    mode: 'onSubmit',
    defaultValues: initialState
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
  const planName = useWatch({ control: form.control, name: 'name' }) || '';

  const toggleRow = (mealId: string) => setExpandedRows(prev => ({ ...prev, [mealId]: !prev[mealId] }));

  const setRowExpandedState = (mealId: string, isExpanded: boolean) =>
    setExpandedRows(prev => ({ ...prev, [mealId]: isExpanded }));

  const handleAddNewMeal = () => {
    const newMealId = `temp-${crypto.randomUUID()}`;

    appendMeal({ id: newMealId, name: '', ingredients: [] });

    setExpandedRows(prev => ({ ...prev, [newMealId]: true }));
  };

  function onSubmit(formData: MealPlan) {
    const requestData: UpdateMealPlanRequest = {
      name: formData.name,
      meals: formData.meals.map(meal => ({
        name: meal.name,
        id: meal.id.startsWith('temp-') ? null : meal.id,
        templateMealId: null,
        ingredients: meal.ingredients.map(ingredient => ({
          foodId: ingredient.food.id,
          quantity: ingredient.quantity
        }))
      }))
    };

    mutate(
      { id: formData.id, mealPlan: requestData },
      {
        onSuccess: () => {
          setIsEditable(false);

          form.reset(formData);

          toast.success('Meal plan updated');
        },
        onError: showValidationErrorToast
      }
    );
  }

  const handleCancel = () => {
    form.reset(initialState);
    setIsEditable(false);
  };

  return (
    <Form {...form}>
      <form
        onSubmit={form.handleSubmit(onSubmit)}
        className='w-full max-w-5xl mx-auto p-4 border rounded-xl shadow-sm bg-card'
      >
        <div className='flex justify-between items-center mb-4 gap-4'>
          <div className='flex items-center gap-2 flex-1 min-w-0'>
            <List className='h-6 w-6 text-primary shrink-0' />

            {isEditable ? (
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
                      />
                    </FormControl>
                  </FormItem>
                )}
              />
            ) : (
              <h2 className='text-2xl font-bold truncate'>{planName || 'Untitled Meal Plan'}</h2>
            )}
          </div>

          <div className='flex items-center gap-2 shrink-0'>
            {isEditable && (
              <Button type='submit' disabled={isPending} variant='default' className='cursor-pointer'>
                <Save className='mr-2 h-4 w-4' /> {isPending ? 'Saving...' : 'Save'}
              </Button>
            )}

            <Button
              type='button'
              onClick={() => (isEditable ? handleCancel() : setIsEditable(true))}
              variant={isEditable ? 'destructive' : 'default'}
              className='cursor-pointer'
            >
              {isEditable ? (
                'Cancel'
              ) : (
                <>
                  <Pencil className='mr-2 h-4 w-4' /> Edit
                </>
              )}
            </Button>
          </div>
        </div>

        <Table className='table-fixed w-full'>
          <TableHeader className='bg-muted/50'>
            <TableRow>
              <TableHead className='w-[3%]' />

              {/* Updated to 36% */}
              <TableHead className='text-xs w-[36%]'>Meal</TableHead>

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

              <TableHead className='w-[3%]' />
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
                  isEditable={isEditable}
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

        {isEditable && (
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
        )}

        <div className='pt-6 mt-4 border-t'>
          <NutritionBox totals={calculatePlanMacros(allLiveMeals)} />
        </div>
      </form>
    </Form>
  );
}
