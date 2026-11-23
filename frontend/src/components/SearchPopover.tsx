import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Plus, Search } from 'lucide-react';

import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { useDebounce } from '@/hooks/useDebounce';
import { MIN_SEARCH_LENGTH } from '@/schema';
import { toNormalizedString } from '@/utils';

import type { SearchQuery } from '@/api/queryKeys';
import type { SearchableItem } from '@/types';
import type { ComponentProps, ReactNode } from 'react';

interface SearchPopoverProps<T extends SearchableItem> {
  value: string;
  onValueChange: (value: string) => void;
  onSelect: (item: T) => void;
  searchQuery: (term: string) => SearchQuery<T>;
  renderItem: (item: T) => ReactNode;
  footer?: (state: { hasExactMatch: boolean; close: () => void }) => ReactNode;
  placeholder: string;
  disabled?: boolean;
  // Set for form rows: outside clicks don't close it (the add-food dialog sits inside), Escape does.
  selected?: boolean;
}

export default function SearchPopover<T extends SearchableItem>({
  value,
  onValueChange,
  onSelect,
  searchQuery,
  renderItem,
  footer,
  placeholder,
  disabled = false,
  selected
}: SearchPopoverProps<T>) {
  const [isOpen, setIsOpen] = useState(false);

  const debouncedTerm = useDebounce(value);

  const isValidTerm = debouncedTerm.trim().length >= MIN_SEARCH_LENGTH;

  const isSticky = selected !== undefined;

  const open = isValidTerm && !disabled && isOpen && !selected;

  const { data, isFetching } = useQuery({ ...searchQuery(debouncedTerm), enabled: open });

  const hasExactMatch = !!data?.items.some(item => toNormalizedString(item.name) === toNormalizedString(debouncedTerm));

  const handleSelect = (item: T) => {
    onSelect(item);

    setIsOpen(false);
  };

  return (
    <Popover open={open} onOpenChange={setIsOpen}>
      <PopoverTrigger asChild>
        <div className='relative'>
          <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
          <Input
            value={value}
            disabled={disabled}
            className='pl-9'
            placeholder={placeholder}
            onChange={e => {
              onValueChange(e.target.value);
              setIsOpen(true);
            }}
            onClick={e => {
              e.stopPropagation();
              setIsOpen(true);
            }}
          />
        </div>
      </PopoverTrigger>

      <PopoverContent
        className='p-0 border rounded-md shadow-lg overflow-hidden w-(--radix-popover-trigger-width) min-w-80'
        align='start'
        sideOffset={5}
        onOpenAutoFocus={e => e.preventDefault()}
        onInteractOutside={e => isSticky && e.preventDefault()}
      >
        <div className='max-h-60 overflow-y-auto bg-popover'>
          {isFetching && <div className='p-4 text-xs text-center animate-pulse'>Searching...</div>}

          {!isFetching && data?.items.length === 0 && (
            <div className='p-4 text-xs text-muted-foreground text-center italic'>No matches found.</div>
          )}

          {data?.items.map(item => (
            <button
              key={item.id}
              type='button'
              onClick={() => handleSelect(item)}
              className='w-full text-left p-3 hover:bg-muted flex flex-wrap items-center justify-between gap-x-4 gap-y-2 border-b last:border-0 text-sm font-medium transition-colors cursor-pointer'
            >
              {renderItem(item)}
            </button>
          ))}

          {!isFetching && footer?.({ hasExactMatch, close: () => setIsOpen(false) })}
        </div>
      </PopoverContent>
    </Popover>
  );
}

export function ItemSummary({ name, summary }: { name: string; summary: string }) {
  return (
    <div className='flex flex-wrap items-baseline gap-x-3 gap-y-1 grow'>
      <span className='text-foreground'>{name}</span>
      <div className='text-muted-foreground font-normal text-xs'>{summary}</div>
    </div>
  );
}

export function ItemBadge({ children }: { children: ReactNode }) {
  return (
    <span className='text-xs font-normal text-muted-foreground/80 bg-background border border-border/50 shadow-sm px-1.5 py-0.5 rounded w-fit'>
      {children}
    </span>
  );
}

export function SearchFooterAction({ children, ...props }: ComponentProps<'button'>) {
  return (
    <button
      type='button'
      className='flex w-full cursor-pointer items-center gap-2 p-3 text-left text-xs font-bold text-primary transition-colors hover:bg-muted'
      {...props}
    >
      <Plus className='h-4 w-4 shrink-0' />
      <span className='truncate'>{children}</span>
    </button>
  );
}
