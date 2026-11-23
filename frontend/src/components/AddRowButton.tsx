import { Plus } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { cn } from '@/utils';

interface AddRowButtonProps {
  label: string;
  onClick: () => void;
  className?: string;
}

export default function AddRowButton({ label, onClick, className }: AddRowButtonProps) {
  return (
    <Button
      type='button'
      variant='outline'
      onClick={onClick}
      className={cn('w-full border-dashed border-2 h-12 cursor-pointer', className)}
    >
      <Plus className='mr-2 h-4 w-4' /> {label}
    </Button>
  );
}
