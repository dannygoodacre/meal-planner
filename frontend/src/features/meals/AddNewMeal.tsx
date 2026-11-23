import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Hamburger } from 'lucide-react';
import { useFieldArray, useForm, useWatch } from 'react-hook-form';
import { toast } from 'sonner';

import { addMeal } from '@/api/meal';
import { mealSearchQuery, queryKeys } from '@/api/queryKeys';
import AddRowButton from '@/components/AddRowButton';
import FormActions from '@/components/FormActions';
import FormArrayMessage from '@/components/FormArrayMessage';
import FormCard from '@/components/FormCard';
import { MacroHeaderCells } from '@/components/MacroColumns';
import NutritionBox from '@/components/NutritionBox';
import SearchCheck from '@/components/SearchCheck';
import { Form } from '@/components/ui/form';
import { Table, TableBody, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import IngredientRow from '@/features/meals/IngredientRow';
import { toIngredientRequest } from '@/features/meals/toIngredientRequest';
import useInvalidatingMutation from '@/hooks/useInvalidatingMutation';
import { calculateMealMacros } from '@/macros';
import { MealFormSchema } from '@/schema';
import { showNameConflictOrToast } from '@/toasts';

import type { AddMealRequest, MealFormData, MealFormResult } from '@/types';

interface AddNewMealProps {
  name?: string;
  isNameEditable?: boolean;
}

export default function AddNewMeal({ name = '', isNameEditable = true }: AddNewMealProps) {
  const form = useForm<MealFormData, unknown, MealFormResult>({
    resolver: zodResolver(MealFormSchema),
    mode: 'onTouched',
    defaultValues: {
      name,
      ingredients: [{ food: null, quantity: 0 }]
    }
  });

  const { fields, append, remove } = useFieldArray({ control: form.control, name: 'ingredients' });

  const ingredients = useWatch({ control: form.control, name: 'ingredients' }) || [];

  const [isDuplicate, setIsDuplicate] = useState(false);

  const { mutate, isPending } = useInvalidatingMutation(addMeal, [queryKeys.meal.all]);

  const onSubmit = (formData: MealFormResult) => {
    const payload: AddMealRequest = {
      name: formData.name,
      ingredients: formData.ingredients.map(toIngredientRequest)
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
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <FormCard
          icon={Hamburger}
          title='New meal'
          wide
          actions={<FormActions isEditable isPending={isPending} disabled={isDuplicate} />}
        >
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
                  <MacroHeaderCells />
                  <TableHead className='w-[6%]' />
                </TableRow>
              </TableHeader>

              <TableBody>
                {fields.map((field, index) => (
                  <IngredientRow
                    key={field.id}
                    name={`ingredients.${index}`}
                    isEditable
                    onRemove={() => remove(index)}
                  />
                ))}
              </TableBody>
            </Table>
          </div>

          <FormArrayMessage error={form.formState.errors.ingredients} />

          <AddRowButton label='Add another ingredient' onClick={() => append({ food: null, quantity: 0 })} />

          <div className='pt-6 mt-4 border-t'>
            <NutritionBox totals={calculateMealMacros(ingredients)} />
          </div>
        </FormCard>
      </form>
    </Form>
  );
}
