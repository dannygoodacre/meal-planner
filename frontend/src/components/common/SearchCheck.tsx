import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { AlertCircle, CheckCircle2, Search } from 'lucide-react';
import { useFormContext, useWatch } from 'react-hook-form';

import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useDebounce } from '@/hooks/useDebounce';
import { cn, toNormalizedString } from '@/utils';

import type { AddFoodRequest, SearchableItem, SearchResponse } from '@/types';

interface SearchCheckProps<T extends SearchableItem> {
  label: string;
  placeholder?: string;
  isNameEditable?: boolean;
  search: (query: string) => Promise<SearchResponse<T>>;
  onMatchChange?: (hasMatch: boolean) => void;
  excludeName?: string;
}

export default function SearchCheck<T extends SearchableItem>({
  label,
  placeholder,
  isNameEditable = true,
  search,
  onMatchChange,
  excludeName
}: SearchCheckProps<T>) {
  const { control } = useFormContext<AddFoodRequest>();

  const nameValue = useWatch({ control, name: 'name' }) || '';

  const debouncedName = useDebounce(nameValue, 500);

  const { data, isFetching } = useQuery({
    queryKey: [label, debouncedName],
    queryFn: () => search(debouncedName),
    enabled: debouncedName.length >= 3 && isNameEditable
  });

  const filteredItems =
    data?.items?.filter(item => !excludeName || toNormalizedString(item.name) !== toNormalizedString(excludeName)) ||
    [];

  const hasExactMatch = filteredItems
    .map((x: T) => toNormalizedString(x.name))
    .includes(toNormalizedString(debouncedName));

  useEffect(() => {
    onMatchChange?.(hasExactMatch || false);
  }, [hasExactMatch, onMatchChange]);

  return (
    <>
      <FormField
        control={control}
        name='name'
        render={({ field }) => (
          <FormItem>
            <FormLabel>{label}</FormLabel>

            <FormControl>
              <div className='relative'>
                <Input
                  placeholder={placeholder}
                  autoComplete='off'
                  readOnly={!isNameEditable}
                  className={cn(!isNameEditable && 'bg-muted cursor-not-allowed opacity-70')}
                  {...field}
                />
                {isFetching && isNameEditable && (
                  <Search className='absolute right-3 top-2.5 h-4 w-4 animate-pulse text-muted-foreground' />
                )}
              </div>
            </FormControl>

            <FormMessage />
          </FormItem>
        )}
      />

      {isNameEditable && debouncedName.length >= 3 && (
        <div className='animate-in fade-in slide-in-from-top-1'>
          {data && hasExactMatch ? (
            <Alert variant='destructive' className='bg-destructive/5 border-destructive/20'>
              <AlertCircle className='h-4 w-4' />

              <AlertTitle>Duplicate Found</AlertTitle>

              <AlertDescription>
                <ul className='mt-2 space-y-2'>
                  {/* 3. Map over filtered items instead of raw data */}
                  {filteredItems
                    .filter(item => toNormalizedString(item.name) === toNormalizedString(debouncedName))
                    .map(item => (
                      <li key={item.id} className='overflow-hidden bg-background/50 rounded border'>
                        <div className='flex justify-between items-center p-2'>
                          <span className='text-xs font-bold'>{item.name}</span>
                        </div>
                      </li>
                    ))}
                </ul>
              </AlertDescription>
            </Alert>
          ) : data && filteredItems.length > 0 ? (
            <Alert className='bg-amber-500/10 dark:bg-amber-500/5 border-amber-500/30 text-amber-700 dark:text-amber-400'>
              <AlertCircle className='h-4 w-4 text-amber-600 dark:text-amber-400' />

              <AlertTitle className='text-amber-700 dark:text-amber-300'>
                {filteredItems.length > 1 ? 'Potential duplicates found' : 'Potential duplicate found'}
              </AlertTitle>

              <AlertDescription className='text-amber-700/90 dark:text-amber-400/80'>
                <ul className='mt-2 space-y-2'>
                  {filteredItems.slice(0, 3).map(item => (
                    <li key={item.id} className='overflow-hidden bg-background/60 rounded border border-amber-500/20'>
                      <div className='flex justify-between items-center p-2'>
                        <span className='text-xs font-bold text-amber-700'>{item.name}</span>
                      </div>
                    </li>
                  ))}
                </ul>
              </AlertDescription>
            </Alert>
          ) : !isFetching ? (
            <Alert className='bg-emerald-500/10 dark:bg-emerald-500/5 border-emerald-500/30 text-emerald-700 dark:text-emerald-400'>
              <CheckCircle2 className='h-4 w-4 text-emerald-600 dark:text-emerald-400' />

              <AlertTitle className='text-emerald-700 dark:text-emerald-300'>Name available</AlertTitle>
            </Alert>
          ) : null}
        </div>
      )}
    </>
  );
}
