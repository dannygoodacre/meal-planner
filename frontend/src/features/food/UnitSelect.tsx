import { useFormContext } from 'react-hook-form';

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { defaultQuantity } from '@/macros';

import type { AddFoodRequest } from '@/types';

type Option = [label: string, value: string];

interface UnitSelectProps {
  options: Option[];
  disabled?: boolean;
}

export default function UnitSelect({ options, disabled = false }: UnitSelectProps) {
  const { control, getValues, setValue } = useFormContext<AddFoodRequest>();

  const handleUnitChange = (unit: AddFoodRequest['unit']) => {
    // Swap the other unit's default for this one's, but keep a quantity the user typed.
    if (getValues('referenceQuantity') === defaultQuantity(getValues('unit'))) {
      setValue('referenceQuantity', defaultQuantity(unit), { shouldValidate: true });
    }

    setValue('unit', unit, { shouldDirty: true, shouldValidate: true });
  };

  return (
    <FormField
      control={control}
      name='unit'
      render={({ field }) => (
        <FormItem>
          <FormLabel>Measurement</FormLabel>

          <Select disabled={disabled} onValueChange={handleUnitChange} value={field.value?.toString()}>
            <FormControl>
              <SelectTrigger className='w-full'>
                <SelectValue placeholder='Select unit' />
              </SelectTrigger>
            </FormControl>

            <SelectContent>
              {options.map(([label, value]) => (
                <SelectItem key={value} value={value}>
                  {label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <FormMessage />
        </FormItem>
      )}
    />
  );
}
