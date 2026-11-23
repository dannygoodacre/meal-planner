import { useFormContext } from 'react-hook-form';

import { foodSearchQuery } from '@/api/queryKeys';
import NumberFormField from '@/components/NumberFormField';
import SearchCheck from '@/components/SearchCheck';
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import UnitSelect from '@/features/food/UnitSelect';

import type { AddFoodRequest } from '@/types';

interface FoodFormFieldsProps {
  readOnly?: boolean;
  isNameEditable?: boolean;
  excludeName?: string;
  onDuplicateChange: (isDuplicate: boolean) => void;
}

export default function FoodFormFields({
  readOnly = false,
  isNameEditable = true,
  excludeName,
  onDuplicateChange
}: FoodFormFieldsProps) {
  const { control } = useFormContext<AddFoodRequest>();

  return (
    <>
      <SearchCheck
        label='Food name'
        placeholder='e.g. chicken breast'
        isNameEditable={isNameEditable && !readOnly}
        searchQuery={foodSearchQuery}
        onMatchChange={onDuplicateChange}
        excludeName={excludeName}
      />

      <div className='flex flex-row items-start gap-4 mb-2'>
        <div className='flex-2 min-w-0'>
          <NumberFormField
            control={control}
            name='referenceQuantity'
            label='Reference quantity'
            step={1}
            disabled={readOnly}
          />
        </div>

        <div className='flex-1 min-w-0'>
          <UnitSelect
            disabled={readOnly}
            options={[
              ['grams (g)', 'Grams'],
              ['pieces (pcs)', 'Count']
            ]}
          />
        </div>
      </div>

      <FormField
        control={control}
        name='source'
        render={({ field }) => (
          <FormItem>
            <FormLabel>Source</FormLabel>
            <FormControl>
              <Input placeholder='e.g. https://www.tesco.com/...' autoComplete='off' disabled={readOnly} {...field} />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <NumberFormField control={control} name='calories' label='Calories (kcal)' step={1} disabled={readOnly} />

      <NumberFormField control={control} name='protein' label='Protein (g)' disabled={readOnly} />

      <div className='space-y-2'>
        <NumberFormField control={control} name='carbohydrates' label='Carbohydrates (g)' disabled={readOnly} />

        <div className='pl-6 border-l-2 border-muted'>
          <NumberFormField control={control} name='sugar' label='Of which sugars (g)' disabled={readOnly} />
        </div>
      </div>

      <div className='space-y-2'>
        <NumberFormField control={control} name='fat' label='Fat (g)' disabled={readOnly} />

        <div className='pl-6 border-l-2 border-muted'>
          <NumberFormField control={control} name='saturatedFat' label='Of which saturates (g)' disabled={readOnly} />
        </div>
      </div>

      <NumberFormField control={control} name='fibre' label='Fibre (g)' disabled={readOnly} />

      <NumberFormField control={control} name='salt' label='Salt (g)' disabled={readOnly} />
    </>
  );
}
