import { useState } from 'react';
import { Carrot } from 'lucide-react';
import { toast } from 'sonner';

import { updateFood } from '@/api/food';
import { queryKeys } from '@/api/queryKeys';
import FormActions from '@/components/FormActions';
import FormCard from '@/components/FormCard';
import { Form } from '@/components/ui/form';
import FoodFormFields from '@/features/food/FoodFormFields';
import useFoodForm from '@/features/food/useFoodForm';
import useInvalidatingMutation from '@/hooks/useInvalidatingMutation';
import { showNameConflictOrToast } from '@/toasts';

import type { AddFoodRequest, Food } from '@/types';

interface UpdateFoodProps {
  initialState: Food;
}

export default function UpdateFood({ initialState }: UpdateFoodProps) {
  const [isEditable, setIsEditable] = useState(false);
  const [isDuplicate, setIsDuplicate] = useState(false);
  const [savedName, setSavedName] = useState(initialState.name);

  // Meals and meal plans embed live food data, so their cached macros change too.
  const { mutate, isPending } = useInvalidatingMutation(
    ({ id, food }: { id: string; food: AddFoodRequest }) => updateFood(id, food),
    [queryKeys.food.all, queryKeys.meal.all, queryKeys.mealPlan.all]
  );

  const form = useFoodForm({ ...initialState, source: initialState.source || '' });

  function onSubmit(payload: AddFoodRequest) {
    mutate(
      { id: initialState.id, food: payload },
      {
        onSuccess: () => {
          setIsEditable(false);
          form.reset(payload);
          setSavedName(payload.name);
          toast.success('Food updated successfully!');
        },
        onError: showNameConflictOrToast(form.setError)
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
          icon={Carrot}
          title='Update food'
          description='Update nutrition details'
          actions={
            <FormActions
              isEditable={isEditable}
              isPending={isPending}
              disabled={isDuplicate}
              onEdit={() => setIsEditable(true)}
              onCancel={handleCancel}
            />
          }
        >
          <FoodFormFields readOnly={!isEditable} onDuplicateChange={setIsDuplicate} excludeName={savedName} />
        </FormCard>
      </form>
    </Form>
  );
}
