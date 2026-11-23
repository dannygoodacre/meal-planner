import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { List } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

import { addMealPlan, updateMealPlan } from '@/api/mealPlan';
import { mealPlanSearchQuery, queryKeys } from '@/api/queryKeys';
import AddRowButton from '@/components/AddRowButton';
import FormActions from '@/components/FormActions';
import FormArrayMessage from '@/components/FormArrayMessage';
import FormCard from '@/components/FormCard';
import { MacroHeaderCells } from '@/components/MacroColumns';
import NutritionBox from '@/components/NutritionBox';
import SearchCheck from '@/components/SearchCheck';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import MealRow from '@/features/mealplan/MealRow';
import { toIngredientRequest } from '@/features/meals/toIngredientRequest';
import useInvalidatingMutation from '@/hooks/useInvalidatingMutation';
import { calculatePlanMacros } from '@/macros';
import { MealPlanFormSchema } from '@/schema';
import { showNameConflictOrToast } from '@/toasts';

import type { MealPlan, MealPlanFormData, MealPlanFormResult, UpdateMealPlanRequest } from '@/types';

interface MealPlanFormProps {
  initialState?: MealPlan;
}

export default function MealPlanForm({ initialState }: MealPlanFormProps) {
  const isNew = !initialState;

  const [isEditable, setIsEditable] = useState(isNew);
  const [isDuplicate, setIsDuplicate] = useState(false);
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

  const create = useInvalidatingMutation(addMealPlan, [queryKeys.mealPlan.all]);
  const update = useInvalidatingMutation(
    ({ id, mealPlan }: { id: string; mealPlan: UpdateMealPlanRequest }) => updateMealPlan(id, mealPlan),
    [queryKeys.mealPlan.all]
  );
  const isPending = create.isPending || update.isPending;

  const navigate = useNavigate();

  const form = useForm<MealPlanFormData, unknown, MealPlanFormResult>({
    resolver: zodResolver(MealPlanFormSchema),
    mode: 'onTouched',
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
        ingredients: meal.ingredients.map(toIngredientRequest)
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
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <FormCard
          icon={List}
          wide
          actions={
            <FormActions
              isEditable={isEditable}
              isPending={isPending}
              disabled={isDuplicate}
              onEdit={isNew ? undefined : () => setIsEditable(true)}
              onCancel={isNew ? undefined : handleCancel}
            />
          }
          title={
            isNew ? (
              'New meal plan'
            ) : isEditable ? (
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
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : (
              <h2 className='truncate'>{planName || 'Untitled Meal Plan'}</h2>
            )
          }
        >
          {isNew && (
            <div className='space-y-4 mb-6'>
              <SearchCheck
                label='Meal plan name'
                placeholder='e.g. Bulking week day'
                searchQuery={mealPlanSearchQuery}
                onMatchChange={setIsDuplicate}
              />
            </div>
          )}

          <div className='rounded-md border'>
            <Table className='table-fixed w-full'>
              <TableHeader className='bg-muted/50'>
                <TableRow>
                  <TableHead className='w-[6%]' />

                  <TableHead className='text-xs w-[30%]'>Meal</TableHead>

                  <MacroHeaderCells />

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
          </div>

          <FormArrayMessage error={form.formState.errors.meals} />

          {isEditable && <AddRowButton label='Add another meal' onClick={handleAddNewMeal} />}

          <div className='pt-6 mt-4 border-t'>
            <NutritionBox totals={calculatePlanMacros(allLiveMeals)} />
          </div>
        </FormCard>
      </form>
    </Form>
  );
}
