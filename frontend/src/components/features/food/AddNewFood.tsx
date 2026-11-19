import { useState } from 'react';
import { toast } from 'sonner';

import SubmitButton from '@/components/common/SubmitButton';
import { showNameConflictOrToast } from '@/components/common/ValidationErrorToast.tsx';
import FoodFormFields from '@/components/features/food/FoodFormFields';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import useFoodForm from '@/hooks/useFoodForm';
import useNewFood from '@/hooks/useNewFood';

import type { AddFoodRequest } from '@/types';

interface AddNewFoodProps {
  name?: string;
  isNameEditable?: boolean;
  onCreated?: (id: string) => void;
}

export default function AddNewFood({ name = '', isNameEditable = true, onCreated }: AddNewFoodProps) {
  const form = useFoodForm({
    name: name,
    unit: 'Grams',
    referenceQuantity: 100,
    calories: 0,
    protein: 0,
    carbohydrates: 0,
    sugar: 0,
    fat: 0,
    saturatedFat: 0,
    fibre: 0,
    salt: 0,
    source: ''
  });

  const [isDuplicate, setIsDuplicate] = useState(false);

  const { mutate, isPending } = useNewFood();

  function onSubmit(payload: AddFoodRequest) {
    mutate(payload, {
      onSuccess: id => {
        toast.success('Food added successfully!');

        form.reset();

        onCreated?.(id);
      },
      onError: showNameConflictOrToast(form.setError)
    });
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Add new food</CardTitle>

        <CardDescription>Enter nutrition details per reference quantity (e.g., per 100g)</CardDescription>
      </CardHeader>

      <CardContent className='mt-2'>
        <Form {...form}>
          <form
            onSubmit={e => {
              // the dialog portal sits inside the meal/plan form; stop the submit bubbling to it
              e.stopPropagation();
              return form.handleSubmit(onSubmit)(e);
            }}
            className='space-y-4'
          >
            <FoodFormFields isNameEditable={isNameEditable} onDuplicateChange={setIsDuplicate} />

            <SubmitButton
              actionMessage='Save'
              loadingMessage='Saving...'
              isLoading={isPending}
              disabled={isPending || isDuplicate}
            />
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
