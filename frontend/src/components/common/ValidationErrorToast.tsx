import { Fragment } from 'react';
import { toast } from 'sonner';

import { ApiError } from '@/types';

import type { UseFormSetError } from 'react-hook-form';

export function showValidationErrorToast(error: ApiError): void {
  toast.error(
    <div className='flex flex-col gap-2 text-sm max-w-sm'>
      <span className='font-semibold text-destructive'>
        {error.details?.title || error.message || 'An API error occurred'}
      </span>

      {error.details?.errors && Object.keys(error.details.errors).length > 0 && (
        <div className='grid grid-cols-[auto_1fr] gap-x-3 gap-y-1 mt-1 text-xs items-start'>
          {Object.entries(error.details.errors).flatMap(([field, messages]) =>
            messages.map((msg, idx) => (
              <Fragment key={`${field}-${idx}`}>
                <span className='font-semibold text-right capitalize select-none whitespace-nowrap'>{field}:</span>

                <span className='text-muted-foreground text-left wrap-break-word'>{msg}</span>
              </Fragment>
            ))
          )}
        </div>
      )}
    </div>
  );
}

// The client-side duplicate check is only a hint; the backend's 409 is authoritative.
export const showNameConflictOrToast =
  (setError: UseFormSetError<{ name: string }>) =>
  (error: ApiError): void => {
    if (error.status === 409) {
      setError('name', { message: error.message }, { shouldFocus: true });
    } else {
      showValidationErrorToast(error);
    }
  };
