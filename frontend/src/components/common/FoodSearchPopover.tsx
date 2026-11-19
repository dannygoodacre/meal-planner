import { useState } from 'react';
import { Plus } from 'lucide-react';

import { getFood } from '@/api/food';
import { foodSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary } from '@/components/common/SearchPopover';
import { showValidationErrorToast } from '@/components/common/ValidationErrorToast.tsx';
import AddNewFood from '@/components/features/food/AddNewFood';
import { Dialog, DialogContent, DialogHeader, DialogTrigger } from '@/components/ui/dialog';
import { unitLabel } from '@/utils';

import type { Food } from '@/types';

interface FoodSearchPopoverProps {
  value: string;
  onValueChange: (value: string) => void;
  onSelect: (food: Food) => void;
  selected?: boolean;
  allowAddNew?: boolean;
}

export default function FoodSearchPopover({
  value,
  onValueChange,
  onSelect,
  selected,
  allowAddNew = true
}: FoodSearchPopoverProps) {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const handleCreated = (id: string) => {
    setIsDialogOpen(false);

    getFood(id).then(onSelect, showValidationErrorToast);
  };

  return (
    <SearchPopover
      value={value}
      onValueChange={onValueChange}
      onSelect={onSelect}
      selected={selected}
      searchQuery={foodSearchQuery}
      placeholder='Search food...'
      renderItem={food => (
        <>
          <ItemSummary
            name={food.name}
            summary={`${food.calories} kcal/${food.referenceQuantity}${food.unit === 'Count' ? ' ' : ''}${unitLabel(food.unit)} | ${food.protein}g P | ${food.carbohydrates}g C | ${food.fat}g F`}
          />
          {hostname(food.source) && <ItemBadge>{hostname(food.source)}</ItemBadge>}
        </>
      )}
      footer={
        allowAddNew
          ? ({ hasExactMatch }) =>
              !hasExactMatch && (
                <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                  <DialogTrigger asChild>
                    <button
                      type='button'
                      className='flex w-full cursor-pointer items-center gap-2 p-3 text-left text-xs font-bold text-blue-600 transition-colors hover:bg-blue-50'
                    >
                      <Plus className='h-4 w-4 shrink-0' />
                      <span className='truncate'>Add "{value}" as new food</span>
                    </button>
                  </DialogTrigger>

                  <DialogContent className='sm:max-w-120'>
                    <DialogHeader />
                    <AddNewFood name={value} isNameEditable={false} onCreated={handleCreated} />
                  </DialogContent>
                </Dialog>
              )
          : undefined
      }
    />
  );
}

const hostname = (source: string): string => {
  try {
    return new URL(source).hostname.replace(/^www\./, '');
  } catch {
    return '';
  }
};
