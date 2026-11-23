import { useState } from 'react';
import { Carrot } from 'lucide-react';
import { toast } from 'sonner';

import { addFood } from '@/api/food';
import { queryKeys } from '@/api/queryKeys';
import FormActions from '@/components/FormActions';
import FormCard from '@/components/FormCard';
import { Form } from '@/components/ui/form';
import FoodFormFields from '@/features/food/FoodFormFields';
import useFoodForm from '@/features/food/useFoodForm';
import useInvalidatingMutation from '@/hooks/useInvalidatingMutation';
import { showNameConflictOrToast } from '@/toasts';

import type { AddFoodRequest } from '@/types';

interface AddNewFoodProps {
  name?: string;
  isNameEditable?: boolean;
  onCreated?: (id: string) => void;
}

export default function AddNewFood({ name = '', isNameEditable = true, onCreated }: AddNewFoodProps) {
  const form = useFoodForm({
    name,
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

  const { mutate, isPending } = useInvalidatingMutation(addFood, [queryKeys.food.all]);

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
    <Form {...form}>
      <form
        onSubmit={e => {
          // the dialog portal sits inside the meal/plan form; stop the submit bubbling to it
          e.stopPropagation();

          return form.handleSubmit(onSubmit)(e);
        }}
      >
        <FormCard
          icon={Carrot}
          title='New food'
          description='Enter nutrition details per reference quantity (e.g., per 100g)'
          actions={<FormActions isEditable isPending={isPending} disabled={isDuplicate} />}
        >
          <FoodFormFields isNameEditable={isNameEditable} onDuplicateChange={setIsDuplicate} />
        </FormCard>
      </form>
    </Form>
  );
}
