import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';

import { searchFoodsByName } from '@/api/food';
import NumberFormField from '@/components/common/NumberFormField';
import SearchCheck from '@/components/common/SearchCheck';
import SubmitButton from '@/components/common/SubmitButton';
import DropdownSelect from '@/components/common/UnitSelect.tsx';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import useDoesSubMacroExceedMacro from '@/hooks/useDoesSubMacroExceedMacro';
import useNewFood from '@/hooks/useNewFood';
import { AddFoodRequestSchema } from '@/schema';

import type { AddFoodRequest } from '@/types';

interface AddNewFoodProps {
  name?: string;
  isNameEditable?: boolean;
}

export default function AddNewFood({ name = '', isNameEditable = true }: AddNewFoodProps) {
  const form = useForm<AddFoodRequest>({
    resolver: zodResolver(AddFoodRequestSchema),
    mode: 'onSubmit',
    reValidateMode: 'onChange',
    defaultValues: {
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
    }
  });

  const doesSugarExceedCarbs = useDoesSubMacroExceedMacro(form.control, 'sugar', 'carbohydrates');

  const doesSaturatesExceedFat = useDoesSubMacroExceedMacro(form.control, 'saturatedFat', 'fat');

  const [isDuplicate, setIsDuplicate] = useState(false);

  const { mutate, isPending } = useNewFood();

  function onSubmit(payload: AddFoodRequest) {
    mutate(payload, {
      onSuccess: () => {
        toast.success('Food added successfully!');

        form.reset();
      },
      onError: showValidationErrorToast
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
          <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-4'>
            <SearchCheck
              label='Food name'
              placeholder='e.g. chicken breast'
              isNameEditable={isNameEditable}
              search={searchFoodsByName}
              onMatchChange={setIsDuplicate}
            />

            <div className='flex flex-row items-start gap-4 mb-2'>
              <div className='flex-2 min-w-0'>
                <NumberFormField control={form.control} name='referenceQuantity' label='Reference quantity' step={1} />
              </div>

              <div className='flex-1 min-w-0'>
                <DropdownSelect
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
                    <Input placeholder='e.g. https://www.tesco.com/...' autoComplete='off' {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <NumberFormField control={form.control} name='calories' label='Calories (kcal)' step={1} />

            <NumberFormField control={form.control} name='protein' label='Protein (g)' />

            <div>
              <div className='space-y-2'>
                <NumberFormField control={form.control} name='carbohydrates' label='Carbohydrates (g)' />

                <div className='pl-6 border-l-2 border-muted'>
                  <NumberFormField control={form.control} name='sugar' label='Of which sugars (g)' />

                  {doesSugarExceedCarbs && (
                    <p className='text-sm font-medium text-destructive'>Sugars cannot exceed total carbohydrates</p>
                  )}
                </div>
              </div>
            </div>

            <div>
              <div className='space-y-2'>
                <NumberFormField control={form.control} name='fat' label='Fat (g)' />

                <div className='pl-6 border-l-2 border-muted'>
                  <NumberFormField control={form.control} name='saturatedFat' label='Of which saturates (g)' />

                  {doesSaturatesExceedFat && (
                    <p className='text-sm font-medium text-destructive'>Saturates cannot exceed total fat</p>
                  )}
                </div>
              </div>
            </div>

            <NumberFormField control={form.control} name='fibre' label='Fibre (g)' />

            <NumberFormField control={form.control} name='salt' label='Salt (g)' />

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
