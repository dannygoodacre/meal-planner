import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Hamburger } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

import { addMeal, updateMeal } from '@/api/meal';
import { mealSearchQuery, queryKeys } from '@/api/queryKeys';
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
import IngredientRow from '@/features/meals/IngredientRow';
import { toIngredientRequest } from '@/features/meals/toIngredientRequest';
import useInvalidatingMutation from '@/hooks/useInvalidatingMutation';
import { calculateMealMacros } from '@/macros';
import { MealFormSchema } from '@/schema';
import { showNameConflictOrToast } from '@/toasts';

import type { AddMealRequest, Meal, MealFormData, MealFormResult } from '@/types';

interface MealFormProps {
  initialState?: Meal;
}

export default function MealForm({ initialState }: MealFormProps) {
  const isNew = !initialState;

  const [isEditable, setIsEditable] = useState(isNew);
  const [isDuplicate, setIsDuplicate] = useState(false);

  const create = useInvalidatingMutation(addMeal, [queryKeys.meal.all]);
  const update = useInvalidatingMutation(
    ({ id, meal }: { id: string; meal: AddMealRequest }) => updateMeal(id, meal),
    [queryKeys.meal.all]
  );
  const isPending = create.isPending || update.isPending;

  const navigate = useNavigate();

  const form = useForm<MealFormData, unknown, MealFormResult>({
    resolver: zodResolver(MealFormSchema),
    mode: 'onTouched',
    defaultValues: initialState
      ? { name: initialState.name, ingredients: initialState.ingredients }
      : { name: '', ingredients: [{ food: null, quantity: 0 }] }
  });

  const { fields, append, remove } = useFieldArray({ control: form.control, name: 'ingredients' });

  const ingredients = useWatch({ control: form.control, name: 'ingredients' }) || [];
  const mealName = useWatch({ control: form.control, name: 'name' }) || '';

  function onSubmit(formData: MealFormResult) {
    const requestData: AddMealRequest = {
      name: formData.name,
      ingredients: formData.ingredients.map(toIngredientRequest)
    };

    const onError = showNameConflictOrToast(form.setError);

    if (!initialState) {
      create.mutate(requestData, {
        onSuccess: id => {
          toast.success('Meal created');
          navigate(`/meal/${id}`);
        },
        onError
      });

      return;
    }

    update.mutate(
      { id: initialState.id, meal: requestData },
      {
        onSuccess: () => {
          setIsEditable(false);

          form.reset(formData);

          toast.success('Meal updated');
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
          icon={Hamburger}
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
              'New meal'
            ) : isEditable ? (
              <FormField
                control={form.control}
                name='name'
                render={({ field }) => (
                  <FormItem className='w-full max-w-md'>
                    <FormControl>
                      <Input {...field} type='text' placeholder='Meal name...' className='h-10 text-xl font-bold' />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            ) : (
              <h2 className='truncate'>{mealName || 'Untitled Meal'}</h2>
            )
          }
        >
          {isNew && (
            <div className='space-y-4 mb-6'>
              <SearchCheck
                label='Meal name'
                placeholder='e.g. Leg day steak & rice'
                searchQuery={mealSearchQuery}
                onMatchChange={setIsDuplicate}
              />
            </div>
          )}

          <div className='rounded-md border'>
            <Table className='table-fixed w-full'>
              <TableHeader className='bg-muted/50'>
                <TableRow>
                  <TableHead className='w-[6%]' />
                  <TableHead className='text-xs w-[24%]'>Ingredient</TableHead>
                  <TableHead className='text-xs w-[6%]'>Qty</TableHead>
                  <MacroHeaderCells />
                  <TableHead className='w-[6%]' />
                </TableRow>
              </TableHeader>

              <TableBody>
                {fields.map((field, index) => (
                  <IngredientRow
                    key={field.id}
                    name={`ingredients.${index}`}
                    isEditable={isEditable}
                    onRemove={() => remove(index)}
                  />
                ))}
              </TableBody>
            </Table>
          </div>

          <FormArrayMessage error={form.formState.errors.ingredients} />

          {isEditable && (
            <AddRowButton label='Add another ingredient' onClick={() => append({ food: null, quantity: 0 })} />
          )}

          <div className='pt-6 mt-4 border-t'>
            <NutritionBox totals={calculateMealMacros(ingredients)} />
          </div>
        </FormCard>
      </form>
    </Form>
  );
}
