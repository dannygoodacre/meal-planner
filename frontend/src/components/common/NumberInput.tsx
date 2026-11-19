import { useState } from 'react';

import { Input } from '@/components/ui/input';

import type { ChangeEvent, ComponentProps, FocusEvent, KeyboardEvent } from 'react';

// Other input props (id, aria-*, name, ref, onBlur) pass through so it works inside FormControl.
type NumberInputProps = Omit<ComponentProps<typeof Input>, 'value' | 'onChange' | 'type' | 'step'> & {
  value?: number;
  onChange: (value: number) => void;
  step?: number | string;
};

export default function NumberInput({
  value,
  onChange,
  onBlur,
  className,
  step = 'any',
  min = 0,
  placeholder = '0',
  ...props
}: NumberInputProps) {
  const [prevValue, setPrevValue] = useState<number | undefined>(value);

  const [displayValue, setDisplayValue] = useState<string>(value === undefined || value === 0 ? '' : String(value));

  const isInteger = Number.isInteger(step);

  if (value !== prevValue) {
    setPrevValue(value);

    const numericDisplay = displayValue === '' ? 0 : parseFloat(displayValue);
    if (value !== numericDisplay) {
      setDisplayValue(value === undefined || value === 0 ? '' : String(value));
    }
  }

  const handleOnBlur = (e: FocusEvent<HTMLInputElement>) => {
    onBlur?.(e);

    const value = e.target.value;

    if (value === '' || isNaN(parseFloat(value))) {
      setDisplayValue('');

      onChange(0);
    }
  };

  const handleOnChange = (e: ChangeEvent<HTMLInputElement>) => {
    let value = e.target.value;

    if (value === '') {
      setDisplayValue('');

      onChange(0);

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

    setDisplayValue(value);

    const numericVal = parseFloat(value);

    if (!isNaN(numericVal)) {
      onChange(numericVal);
    }
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
    <Input
      {...props}
      className={`no-spinner ${className || ''}`}
      min={min}
      placeholder={placeholder}
      step={step}
      type='number'
      value={displayValue}
      onBlur={handleOnBlur}
      onChange={handleOnChange}
      onFocus={e => e.target.select()}
      onKeyDown={handleOnKeyDown}
    />
  );
}
