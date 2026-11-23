import { X } from 'lucide-react';

import { Button } from '@/components/ui/button';

import type { MouseEventHandler } from 'react';

interface RemoveButtonProps {
  label: string;
  onClick: MouseEventHandler<HTMLButtonElement>;
}

export default function RemoveButton({ label, onClick }: RemoveButtonProps) {
  return (
    <Button
      type='button'
      variant='ghost'
      size='icon'
      className='h-7 w-7 text-muted-foreground hover:text-destructive cursor-pointer'
      onClick={onClick}
      title={label}
      aria-label={label}
    >
      <X className='h-4 w-4' />
    </Button>
  );
}
