import { Fragment } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';

import Loading from '@/components/Loading';

import type { QueryKey } from '@tanstack/react-query';
import type { ReactNode } from 'react';

interface DetailLoaderProps<T> {
  queryKey: (id: string) => QueryKey;
  queryFn: (id: string) => Promise<T>;
  children: (data: T) => ReactNode;
}

export default function DetailLoader<T>({ queryKey, queryFn, children }: DetailLoaderProps<T>) {
  const { id = '' } = useParams<{ id: string }>();

  const { data, isLoading, isError, error, dataUpdatedAt } = useQuery({
    queryKey: queryKey(id),
    queryFn: () => queryFn(id),
    enabled: !!id,
    staleTime: Infinity
  });

  if (isLoading) {
    return <Loading />;
  }

  if (isError) {
    return <LoadError message={error.message} />;
  }

  if (!data) {
    return <LoadError message='No data found' />;
  }

  // Remount on refetch so forms pick up fresh defaults.
  return <Fragment key={`${id}-${dataUpdatedAt}`}>{children(data)}</Fragment>;
}

function LoadError({ message }: { message: string }) {
  return (
    <div className='p-6 text-center text-sm text-muted-foreground' role='alert'>
      {message}
    </div>
  );
}
