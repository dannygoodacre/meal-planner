import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';

import type { ChangeEvent, FocusEvent, KeyboardEvent } from 'react';
import type { Control, ControllerRenderProps, FieldPath, FieldValues } from 'react-hook-form';

interface NumberFormFieldProps<T extends FieldValues> {
  control: Control<T>;
  name: FieldPath<T>;
  label: string;
  step?: number | string;
  min?: number;
  placeholder?: string;
  disabled?: boolean;
}

export default function NumberFormField<T extends FieldValues>({
  control,
  name,
  label,
  step = 'any',
  min = 0,
  placeholder = '0',
  disabled = false
}: NumberFormFieldProps<T>) {
  const isInteger = Number.isInteger(step);

  const handleOnBlur = (e: FocusEvent<HTMLInputElement>, field: ControllerRenderProps<T, FieldPath<T>>) => {
    field.onBlur();

    const value = e.target.value;

    if (value === '' || isNaN(parseFloat(value))) {
      field.onChange(0);
    }
  };

  const handleOnChange = (e: ChangeEvent<HTMLInputElement>, field: ControllerRenderProps<T, FieldPath<T>>) => {
    let value = e.target.value;

    if (value === '') {
      field.onChange('');

      return;
    }

    if (isInteger) {
      value = value.replace(/\D/g, '');
    } else {
      const parts = value.split('.');

      if (parts.length > 2) {
        value = `${parts[0]}.${parts.slice(1).join('')}`;
      }
    }

    const numericVal = parseFloat(value);

    field.onChange(isNaN(numericVal) ? '' : numericVal);
  };

  const handleOnKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key.length > 1 || e.ctrlKey || e.metaKey || e.altKey) {
      return;
    }

    if (e.key === '.') {
      if (isInteger) {
        e.preventDefault();
        return;
      }

      if (e.currentTarget.value.includes('.')) {
        e.preventDefault();
      }

      return;
    }

    if (!/^\d$/.test(e.key)) {
      e.preventDefault();
    }
  };

  return (
    <FormField
      control={control}
      name={name}
      render={({ field }) => (
        <FormItem>
          <FormLabel>{label}</FormLabel>
          <FormControl>
            <Input
              className='no-spinner'
              min={min}
              placeholder={placeholder}
              step={step}
              type='number'
              disabled={disabled}
              {...field}
              onBlur={e => handleOnBlur(e, field)}
              onChange={e => handleOnChange(e, field)}
              onFocus={e => e.target.select()}
              onKeyDown={handleOnKeyDown}
            />
          </FormControl>
          <FormMessage />
        </FormItem>
      )}
    />
  );
}
