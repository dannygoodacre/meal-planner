import { useState } from 'react';
import { Pencil, Save } from 'lucide-react';
import { toast } from 'sonner';

import { showNameConflictOrToast } from '@/components/common/ValidationErrorToast.tsx';
import FoodFormFields from '@/components/features/food/FoodFormFields';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form } from '@/components/ui/form';
import useFoodForm from '@/hooks/useFoodForm';
import useUpdateFood from '@/hooks/useUpdateFood';

import type { AddFoodRequest, Food } from '@/types';

interface UpdateFoodProps {
  initialState: Food;
}

export default function UpdateFood({ initialState }: UpdateFoodProps) {
  const [isEditable, setIsEditable] = useState<boolean>(false);
  const [isDuplicate, setIsDuplicate] = useState<boolean>(false);
  const [savedName, setSavedName] = useState<string>(initialState.name);

  const { mutate, isPending } = useUpdateFood();

  const form = useFoodForm({
    name: initialState.name,
    unit: initialState.unit,
    referenceQuantity: initialState.referenceQuantity,
    calories: initialState.calories,
    protein: initialState.protein,
    carbohydrates: initialState.carbohydrates,
    sugar: initialState.sugar,
    fat: initialState.fat,
    saturatedFat: initialState.saturatedFat,
    fibre: initialState.fibre,
    salt: initialState.salt,
    source: initialState.source || ''
  });

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
    <Card>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)}>
          <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-4'>
            <div className='space-y-1'>
              <CardTitle>Update food</CardTitle>
              <CardDescription>Update nutrition details</CardDescription>
            </div>

            <div className='flex items-center gap-2 shrink-0'>
              {isEditable && (
                <Button type='submit' disabled={isPending || isDuplicate} variant='default' className='cursor-pointer'>
                  <Save className='mr-2 h-4 w-4' /> {isPending ? 'Saving...' : 'Save'}
                </Button>
              )}

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
            </div>
          </CardHeader>

          <CardContent className='space-y-4 mt-2'>
            <FoodFormFields readOnly={!isEditable} onDuplicateChange={setIsDuplicate} excludeName={savedName} />
          </CardContent>
        </form>
      </Form>
    </Card>
  );
}
