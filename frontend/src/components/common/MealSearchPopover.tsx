import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Plus, Search } from 'lucide-react';

import { searchMealsByName } from '@/api/meal';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { useDebounce } from '@/hooks/useDebounce';
import { calculateMealMacros, round, toNormalizedString } from '@/utils';

import type { Meal } from '@/types';

export interface MealSearchPopoverProps {
  mealName: string;
  onSearchChange: (newValue: string) => void;
  onSelect: (meal: Meal) => void;
  disabled?: boolean;
  showCreateMealFromScratch?: boolean;
}

export default function MealSearchPopover({
  mealName,
  onSearchChange,
  onSelect,
  disabled = false,
  showCreateMealFromScratch = true
}: MealSearchPopoverProps) {
  const [open, setOpen] = useState(false);

  const debouncedSearch = useDebounce(mealName, 300);

  const isValidSearch = !!debouncedSearch && debouncedSearch.trim().length >= 2;

  const { data: results, isFetching } = useQuery({
    queryKey: ['meal-search', debouncedSearch],
    queryFn: () => searchMealsByName(debouncedSearch),
    enabled: isValidSearch && open && !disabled
  });

  const hasExactMatch = results?.items
    ?.map((x: Meal) => toNormalizedString(x.name))
    .includes(toNormalizedString(debouncedSearch));

  const handleSelectExisting = (meal: Meal) => {
    onSelect(meal);

    setOpen(false);
  };

  const handleCreateFromScratch = () => {
    setOpen(false);
  };

  const renderMacroSummary = (meal: Meal) => {
    const macros = calculateMealMacros(meal.ingredients);

    return `${round(macros.calories, 0)} kcal | ${round(macros.protein, 0)}g P | ${round(macros.carbohydrates, 0)}g C | ${round(macros.fat, 0)}g F`;
  };

  return (
    <Popover open={open && isValidSearch && !disabled} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <div className='relative'>
          <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
          <Input
            value={mealName}
            onChange={e => {
              onSearchChange(e.target.value);

              setOpen(true);
            }}
            onClick={e => {
              e.stopPropagation();

              if (isValidSearch) {
                setOpen(true);
              }
            }}
            disabled={disabled}
            className='pl-9'
            placeholder='Search standalone meals...'
          />
        </div>
      </PopoverTrigger>

      <PopoverContent
        className='p-0 border rounded-md shadow-lg overflow-hidden w-(--radix-popover-trigger-width)'
        align='start'
        sideOffset={5}
        onOpenAutoFocus={e => e.preventDefault()}
      >
        <div className='max-h-60 overflow-y-auto bg-popover'>
          {isFetching && <div className='p-4 text-xs text-center animate-pulse'>Searching...</div>}

          {!isFetching && results?.items?.length === 0 && (
            <div className='p-4 text-xs text-muted-foreground text-center italic'>No matching meal found.</div>
          )}

          {results?.items?.map((meal: Meal) => (
            <button
              key={meal.id}
              type='button'
              onClick={() => handleSelectExisting(meal)}
              className='w-full text-left p-3 hover:bg-muted flex flex-wrap items-center justify-between gap-x-4 gap-y-2 border-b last:border-0 text-sm font-medium transition-colors cursor-pointer'
            >
              <div className='flex flex-wrap items-baseline gap-x-3 gap-y-1 grow'>
                <span className='text-foreground'>{meal.name}</span>

                <div className='text-muted-foreground font-normal text-xs'>{renderMacroSummary(meal)}</div>
              </div>

              <span className='text-xs font-normal text-muted-foreground/80 bg-background border border-border/50 shadow-sm px-1.5 py-0.5 rounded w-fit'>
                {meal.ingredients?.length || 0} {meal.ingredients?.length === 1 ? 'ingredient' : 'ingredients'}
              </span>
            </button>
          ))}

          {showCreateMealFromScratch && !hasExactMatch && !isFetching && (
            <button
              type='button'
              onClick={handleCreateFromScratch}
              className='w-full text-left p-3 text-blue-600 hover:bg-blue-50 text-xs font-bold flex items-center gap-2 cursor-pointer transition-colors'
            >
              <Plus className='h-4 w-4 shrink-0' />

              <span className='truncate'>Create "{mealName}" from scratch</span>
            </button>
          )}
        </div>
      </PopoverContent>
    </Popover>
  );
}
