import NumberInput from '@/components/common/NumberInput';
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';

import type { Control, FieldPath, FieldValues } from 'react-hook-form';

interface NumberFormFieldProps<T extends FieldValues> {
  control: Control<T>;
  name: FieldPath<T>;
  label: string;
  step?: number | string;
  disabled?: boolean;
}

export default function NumberFormField<T extends FieldValues>({
  control,
  name,
  label,
  step,
  disabled = false
}: NumberFormFieldProps<T>) {
  return (
    <FormField
      control={control}
      name={name}
      render={({ field }) => (
        <FormItem>
          <FormLabel>{label}</FormLabel>
          <FormControl>
            <NumberInput {...field} step={step} disabled={disabled} />
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}
