import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Pencil, Save } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';

import { searchFoodsByName } from '@/api/food';
import NumberFormField from '@/components/common/NumberFormField';
import SearchCheck from '@/components/common/SearchCheck';
import DropdownSelect from '@/components/common/UnitSelect.tsx';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import useDoesSubMacroExceedMacro from '@/hooks/useDoesSubMacroExceedMacro';
import useUpdateFood from '@/hooks/useUpdateFood';
import { AddFoodRequestSchema } from '@/schema';

import type { AddFoodRequest, Food } from '@/types';

interface UpdateFoodProps {
  initialState: Food;
}

export default function UpdateFood({ initialState }: UpdateFoodProps) {
  const [isEditable, setIsEditable] = useState<boolean>(false);
  const [isDuplicate, setIsDuplicate] = useState<boolean>(false);

  const { mutate, isPending } = useUpdateFood();

  const form = useForm<AddFoodRequest>({
    resolver: zodResolver(AddFoodRequestSchema),
    mode: 'onSubmit',
    reValidateMode: 'onChange',
    defaultValues: {
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
    }
  });

  const doesSugarExceedCarbs = useDoesSubMacroExceedMacro(form.control, 'sugar', 'carbohydrates');
  const doesSaturatesExceedFat = useDoesSubMacroExceedMacro(form.control, 'saturatedFat', 'fat');

  function onSubmit(payload: AddFoodRequest) {
    mutate(
      { id: initialState.id, food: payload },
      {
        onSuccess: () => {
          setIsEditable(false);
          form.reset(payload);
          toast.success('Food updated successfully!');
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
          </CardHeader>

          <CardContent className='space-y-4 mt-2'>
            <SearchCheck
              label='Food name'
              placeholder='e.g. chicken breast'
              isNameEditable={isEditable}
              search={searchFoodsByName}
              onMatchChange={setIsDuplicate}
              excludeName={initialState.name}
            />

            <div className='flex flex-row items-start gap-4 mb-2'>
              <div className='flex-2 min-w-0'>
                <NumberFormField
                  control={form.control}
                  name='referenceQuantity'
                  label='Reference quantity'
                  disabled={!isEditable}
                />
              </div>

              <div className='flex-1 min-w-0'>
                <DropdownSelect
                  disabled={!isEditable}
                  options={[
                    ['grams (g)', 'Grams'],
                    ['pieces', 'Count']
                  ]}
                />
              </div>
            </div>

            <FormField
              control={form.control}
              name='source'
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Source</FormLabel>
                  <FormControl>
                    <Input placeholder='e.g. https://www.tesco.com/...' disabled={!isEditable} {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <NumberFormField
              control={form.control}
              name='calories'
              label='Calories (kcal)'
              step={1}
              disabled={!isEditable}
            />

            <NumberFormField control={form.control} name='protein' label='Protein (g)' disabled={!isEditable} />

            <div>
              <div className='space-y-2'>
                <NumberFormField
                  control={form.control}
                  name='carbohydrates'
                  label='Carbohydrates (g)'
                  disabled={!isEditable}
                />

                <div className='pl-6 border-l-2 border-muted'>
                  <NumberFormField
                    control={form.control}
                    name='sugar'
                    label='Of which sugars (g)'
                    disabled={!isEditable}
                  />

                  {doesSugarExceedCarbs && (
                    <p className='text-sm font-medium text-destructive'>Sugars cannot exceed total carbohydrates</p>
                  )}
                </div>
              </div>
            </div>

            <div>
              <div className='space-y-2'>
                <NumberFormField control={form.control} name='fat' label='Fat (g)' disabled={!isEditable} />

                <div className='pl-6 border-l-2 border-muted'>
                  <NumberFormField
                    control={form.control}
                    name='saturatedFat'
                    label='Of which saturates (g)'
                    disabled={!isEditable}
                  />

                  {doesSaturatesExceedFat && (
                    <p className='text-sm font-medium text-destructive'>Saturates cannot exceed total fat</p>
                  )}
                </div>
              </div>
            </div>

            <NumberFormField control={form.control} name='fibre' label='Fibre (g)' disabled={!isEditable} />

            <NumberFormField control={form.control} name='salt' label='Salt (g)' disabled={!isEditable} />
          </CardContent>
        </form>
      </Form>
    </Card>
  );
}
