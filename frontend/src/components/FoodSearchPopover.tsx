import { useState } from 'react';

import { getFood } from '@/api/food';
import { foodSearchQuery } from '@/api/queryKeys';
import SearchPopover, { ItemBadge, ItemSummary, SearchFooterAction } from '@/components/SearchPopover';
import { Dialog, DialogContent, DialogHeader, DialogTrigger } from '@/components/ui/dialog';
import AddNewFood from '@/features/food/AddNewFood';
import { macroSummary, unitLabel } from '@/macros';
import { showValidationErrorToast } from '@/toasts';

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
      renderItem={food => {
        const site = hostname(food.source);

        return (
          <>
            <ItemSummary
              name={food.name}
              summary={macroSummary(
                food,
                `/${food.referenceQuantity}${food.unit === 'Count' ? ' ' : ''}${unitLabel(food.unit)}`
              )}
            />
            {site && <ItemBadge>{site}</ItemBadge>}
          </>
        );
      }}
      footer={
        allowAddNew
          ? ({ hasExactMatch }) =>
              !hasExactMatch && (
                <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
                  <DialogTrigger asChild>
                    <SearchFooterAction>Add "{value}" as new food</SearchFooterAction>
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
