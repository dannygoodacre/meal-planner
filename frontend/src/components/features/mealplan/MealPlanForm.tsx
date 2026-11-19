import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { List, Pencil, Plus, Save } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

import FormArrayMessage from '@/components/common/FormArrayMessage';
import NutritionBox from '@/components/common/NutritionBox';
import { showNameConflictOrToast } from '@/components/common/ValidationErrorToast.tsx';
import { MealRow } from '@/components/features/mealplan/MealRow';
import { Button } from '@/components/ui/button';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import useNewMealPlan from '@/hooks/useNewMealPlan';
import useUpdateMealPlan from '@/hooks/useUpdateMealPlan';
import { MealPlanFormSchema } from '@/schema';
import { calculatePlanMacros } from '@/utils';

import type { MealPlan, MealPlanFormData, MealPlanFormResult, UpdateMealPlanRequest } from '@/types';

interface MealPlanFormProps {
  // Absent: create a new plan. Present: view it, with an Edit toggle.
  initialState?: MealPlan;
}

export default function MealPlanForm({ initialState }: MealPlanFormProps) {
  const isNew = !initialState;

  const [isEditable, setIsEditable] = useState<boolean>(isNew);
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

  const create = useNewMealPlan();
  const update = useUpdateMealPlan();
  const isPending = create.isPending || update.isPending;

  const navigate = useNavigate();

  const form = useForm<MealPlanFormData, unknown, MealPlanFormResult>({
    resolver: zodResolver(MealPlanFormSchema),
    defaultValues: initialState
      ? { name: initialState.name, meals: initialState.meals }
      : { name: '', meals: [{ id: null, name: '', ingredients: [] }] }
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

  const setRowExpandedState = (fieldId: string, isExpanded: boolean) =>
    setExpandedRows(prev => ({ ...prev, [fieldId]: isExpanded }));

  const handleAddNewMeal = () => appendMeal({ id: null, name: '', ingredients: [] });

  function onSubmit(formData: MealPlanFormResult) {
    const requestData: UpdateMealPlanRequest = {
      name: formData.name,
      meals: formData.meals.map(meal => ({
        name: meal.name,
        id: meal.id,
        ingredients: meal.ingredients.map(ingredient => ({
          foodId: ingredient.food.id,
          quantity: ingredient.quantity
        }))
      }))
    };

    const onError = showNameConflictOrToast(form.setError);

    if (!initialState) {
      create.mutate(
        { name: requestData.name, meals: requestData.meals.map(({ name, ingredients }) => ({ name, ingredients })) },
        {
          onSuccess: id => {
            toast.success('Meal plan created');
            navigate(`/mealplan/${id}`);
          },
          onError
        }
      );

      return;
    }

    update.mutate(
      { id: initialState.id, mealPlan: requestData },
      {
        onSuccess: () => {
          setIsEditable(false);

          form.reset(formData);

          toast.success('Meal plan updated');
        },
        onError
      }
    );
  }

  const handleCancel = () => {
    form.reset();
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
                        autoFocus={isNew}
                      />
                    </FormControl>
                    <FormMessage />
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

            {!isNew && (
              <Button
                type='button'
                onClick={() => (isEditable ? handleCancel() : setIsEditable(true))}
                variant={isEditable ? 'outline' : 'default'}
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
            )}
          </div>
        </div>

        <Table className='table-fixed w-full'>
          <TableHeader className='bg-muted/50'>
            <TableRow>
              <TableHead className='w-[6%]' />

              <TableHead className='text-xs w-[30%]'>Meal</TableHead>

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
            {meals.map((mealField, index) => {
              const currentMeal = allLiveMeals[index] || mealField;

              // new meals start open; meals with errors stay open so the errors are visible
              const isExpanded =
                !!form.formState.errors.meals?.[index] || (expandedRows[mealField.id] ?? !currentMeal.id);

              return (
                <MealRow
                  key={mealField.id}
                  control={form.control}
                  mealIndex={index}
                  mealName={currentMeal.name}
                  isEditable={isEditable}
                  isExpanded={isExpanded}
                  onToggleExpand={() => setRowExpandedState(mealField.id, !isExpanded)}
                  onForceExpand={() => setRowExpandedState(mealField.id, true)}
                  onDeleteMeal={() => removeMeal(index)}
                />
              );
            })}
          </TableBody>
        </Table>

        <FormArrayMessage error={form.formState.errors.meals} />

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
