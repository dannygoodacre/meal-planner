import { Loader2, Pencil, Save } from 'lucide-react';

import { Button } from '@/components/ui/button';

interface FormActionsProps {
  isEditable: boolean;
  isPending: boolean;
  disabled?: boolean;
  onEdit?: () => void;
  onCancel?: () => void;
}

export default function FormActions({ isEditable, isPending, disabled = false, onEdit, onCancel }: FormActionsProps) {
  return (
    <>
      {isEditable && (
        <Button type='submit' disabled={isPending || disabled} className='cursor-pointer'>
          {isPending ? <Loader2 className='mr-2 h-4 w-4 animate-spin' /> : <Save className='mr-2 h-4 w-4' />}
          {isPending ? 'Saving...' : 'Save'}
        </Button>
      )}

      {isEditable && onCancel && (
        <Button type='button' variant='outline' onClick={onCancel} className='cursor-pointer'>
          Cancel
        </Button>
      )}

      {!isEditable && onEdit && (
        <Button type='button' onClick={onEdit} className='cursor-pointer'>
          <Pencil className='mr-2 h-4 w-4' /> Edit
        </Button>
      )}
    </>
  );
}
