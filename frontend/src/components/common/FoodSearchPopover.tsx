import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Plus, Search } from 'lucide-react';

import { searchFoodsByName } from '@/api/food';
import AddNewFood from '@/components/features/food/AddNewFood';
import { Dialog, DialogContent, DialogHeader, DialogTrigger } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { useDebounce } from '@/hooks/useDebounce';
import { toNormalizedString } from '@/utils';

import type { Food } from '@/types';

export interface FoodSearchPopoverProps {
  foodName: string;
  isSelected: boolean;
  onSearchChange: (newValue: string) => void;
  onSelect: (food: Food) => void;
  disabledSearch?: boolean;
  doShowAddNewMeal?: boolean;
  keepOpenUntilSelection?: boolean;
}

export default function FoodSearchPopover({
  foodName,
  isSelected,
  onSearchChange,
  onSelect,
  disabledSearch = false,
  doShowAddNewMeal = true,
  keepOpenUntilSelection = true
}: FoodSearchPopoverProps) {
  const [showNewFoodForm, setShowNewFoodForm] = useState(false);

  const [userClosed, setUserClosed] = useState(false);

  const debouncedSearch = useDebounce(foodName, 400);

  const isValidSearchTerm = Boolean(debouncedSearch && debouncedSearch.trim().length >= 3);

  const { data: results, isFetching } = useQuery({
    queryKey: ['food-search', debouncedSearch],
    queryFn: () => searchFoodsByName(debouncedSearch),
    enabled: isValidSearchTerm && !disabledSearch
  });

  const hasExactMatch = results?.items
    ?.map((x: Food) => toNormalizedString(x.name))
    .includes(toNormalizedString(debouncedSearch));

  const isPopoverOpen = keepOpenUntilSelection
    ? (!isSelected || showNewFoodForm) && isValidSearchTerm
    : isValidSearchTerm && !userClosed;

  const handleSelectExisting = (food: Food) => {
    onSelect(food);

    setShowNewFoodForm(false);

    setUserClosed(true);
  };

  const handleOpenChange = (open: boolean) => {
    setShowNewFoodForm(open);

    if (!open) {
      setUserClosed(true);
    }
  };

  const renderMacroSummary = (food: Food) => {
    const unitText = food.unit === 'Grams' ? `${food.referenceQuantity}g` : 'item';

    return `${food.calories}kcal/${unitText} | ${food.protein}g P | ${food.carbohydrates}g C | ${food.fat}g F`;
  };

  return (
    <Popover open={isPopoverOpen} onOpenChange={handleOpenChange}>
      <PopoverTrigger asChild>
        <div className='relative'>
          <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
          <Input
            value={foodName}
            className='pl-9'
            placeholder='Search food...'
            onChange={e => {
              onSearchChange(e.target.value);
              setUserClosed(false);
            }}
            onClick={() => setUserClosed(false)}
          />
        </div>
      </PopoverTrigger>

      <PopoverContent
        align='start'
        sideOffset={5}
        className='w-(--radix-popover-trigger-width) overflow-hidden rounded-md border p-0 shadow-lg'
        onOpenAutoFocus={e => e.preventDefault()}
        onInteractOutside={e => {
          if (keepOpenUntilSelection) {
            e.preventDefault();
          }
        }}
        onEscapeKeyDown={e => {
          if (keepOpenUntilSelection) {
            e.preventDefault();
          }
        }}
      >
        <div className='max-h-60 overflow-y-auto bg-popover'>
          {isFetching && <div className='p-4 text-center text-xs animate-pulse'>Searching...</div>}

          {results?.items.map((food: Food) => (
            <button
              key={food.id}
              type='button'
              onClick={() => handleSelectExisting(food)}
              className='flex w-full flex-wrap items-center justify-between gap-x-4 gap-y-2 border-b p-3 text-left text-sm font-medium transition-colors hover:bg-muted last:border-0'
            >
              <div className='flex grow flex-wrap items-baseline gap-x-3 gap-y-1'>
                <span className='text-foreground'>{food.name}</span>
                <div className='text-xs font-normal text-muted-foreground'>{renderMacroSummary(food)}</div>
              </div>

              <span className='w-fit rounded border border-border/50 bg-background px-1.5 py-0.5 text-xs font-normal text-muted-foreground/80 shadow-sm'>
                {food.source}
              </span>
            </button>
          ))}

          {doShowAddNewMeal && isValidSearchTerm && !hasExactMatch && !isFetching && (
            <Dialog>
              <DialogTrigger asChild>
                <button
                  type='button'
                  onClick={() => setShowNewFoodForm(true)}
                  className='flex w-full cursor-pointer items-center gap-2 p-3 text-left text-xs font-bold text-blue-600 transition-colors hover:bg-blue-50'
                >
                  <Plus className='h-4 w-4 shrink-0' />
                  <span className='truncate'>Add "{foodName}" as new food</span>
                </button>
              </DialogTrigger>

              <DialogContent className='sm:max-w-120'>
                <DialogHeader />
                <AddNewFood name={foodName} isNameEditable={false} />
              </DialogContent>
            </Dialog>
          )}
        </div>
      </PopoverContent>
    </Popover>
  );
}
