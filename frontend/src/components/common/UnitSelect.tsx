import { useFormContext } from 'react-hook-form';

import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

import type { AddFoodRequest } from '@/types';

type Option = [label: string, value: string];

interface UnitSelectProps {
  options: Option[];
  disabled?: boolean;
}

export default function UnitSelect({ options, disabled = false }: UnitSelectProps) {
  const { control } = useFormContext<AddFoodRequest>();

  return (
    <FormField
      control={control}
      name='unit'
      render={({ field }) => (
        <FormItem>
          <FormLabel>Measurement</FormLabel>

          <Select disabled={disabled} onValueChange={field.onChange} value={field.value?.toString()}>
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
