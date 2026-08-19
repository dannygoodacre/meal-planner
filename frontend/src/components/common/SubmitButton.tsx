import { Loader2, Save } from 'lucide-react';

import { Button } from '@/components/ui/button';

interface SubmitButtonProps {
  actionMessage: string;
  loadingMessage: string;
  isLoading: boolean;
  disabled: boolean;
}

export default function SubmitButton({ actionMessage, loadingMessage, isLoading, disabled }: SubmitButtonProps) {
  return (
    <Button type='submit' className='w-full bg-green-600 hover:bg-green-700' disabled={isLoading || disabled}>
      {isLoading ? (
        <>
          <Loader2 className='mr-2 h-4 w-4 animate-spin' /> {loadingMessage}
        </>
      ) : (
        <>
          <Save className='mr-1 h-4 w-4' /> {actionMessage}
        </>
      )}
    </Button>
  );
}
