import { useState } from 'react';

import { Input } from '@/components/ui/input';

import type { ChangeEvent, FocusEvent, KeyboardEvent } from 'react';

interface NumberInputProps {
  value?: number;
  onChange: (value: number) => void;
  className?: string;
  step?: number | string;
  min?: number;
  placeholder?: string;
  disabled?: boolean;
}

export default function NumberInput({
  value,
  onChange,
  className,
  step = 'any',
  min = 0,
  placeholder = '0',
  disabled = false
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
      disabled={disabled}
    />
  );
}
