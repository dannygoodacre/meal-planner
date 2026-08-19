import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Search } from 'lucide-react';

import { searchMealPlansByName } from '@/api/mealPlan';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { useDebounce } from '@/hooks/useDebounce';
import { calculatePlanMacros, round } from '@/utils';

import type { MealPlan } from '@/types';

export interface MealPlanSearchPopoverProps {
  searchTerm: string;
  onSearchChange: (newValue: string) => void;
  onPlanSelect: (plan: MealPlan) => void;
  disabled?: boolean;
}

export default function MealPlanSearchPopover({
  searchTerm,
  onSearchChange,
  onPlanSelect,
  disabled = false
}: MealPlanSearchPopoverProps) {
  const [open, setOpen] = useState(false);

  const debouncedSearch = useDebounce(searchTerm, 300);

  const isValidSearch = !!debouncedSearch && debouncedSearch.trim().length >= 2;

  const { data: results, isFetching } = useQuery({
    queryKey: ['meal-plan-search', debouncedSearch],
    queryFn: () => searchMealPlansByName(debouncedSearch),
    enabled: isValidSearch && open && !disabled
  });

  const handleSelectExisting = (plan: MealPlan) => {
    onPlanSelect(plan);

    setOpen(false);
  };

  return (
    <Popover open={open && isValidSearch && !disabled} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <div className='relative'>
          <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
          <Input
            value={searchTerm}
            disabled={disabled}
            className='pl-9'
            placeholder='Search meal plans...'
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
            <div className='p-4 text-xs text-muted-foreground text-center italic'>No matching meal plans found.</div>
          )}

          {results?.items?.map((plan: MealPlan) => {
            console.log(plan);
            const macros = calculatePlanMacros(plan.meals || []);

            return (
              <button
                key={plan.id}
                type='button'
                onClick={() => handleSelectExisting(plan)}
                className='w-full text-left p-3 hover:bg-muted flex flex-wrap items-center justify-between gap-x-4 gap-y-2 border-b last:border-0 text-sm font-medium transition-colors cursor-pointer'
              >
                <div className='flex flex-wrap items-baseline gap-x-3 gap-y-1 grow'>
                  <span className='text-foreground'>{plan.name}</span>

                  <div className='text-muted-foreground font-normal text-xs'>
                    {round(macros.calories, 0)} kcal | {round(macros.protein, 0)}g P | {round(macros.carbohydrates, 0)}g
                    C | {round(macros.fat, 0)}g F
                  </div>
                </div>

                <span className='text-xs font-normal text-muted-foreground/80 bg-background border border-border/50 shadow-sm px-1.5 py-0.5 rounded w-fit'>
                  {plan.meals?.length || 0} {plan.meals?.length === 1 ? 'meal' : 'meals'}
                </span>
              </button>
            );
          })}
        </div>
      </PopoverContent>
    </Popover>
  );
}
