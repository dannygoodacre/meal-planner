import { TableCell, TableHead } from '@/components/ui/table';
import { formatMacro, MACRO_KEYS } from '@/macros';
import { cn } from '@/utils';

import type { Macros } from '@/types';
import type { ReactNode } from 'react';

const COLUMNS: Record<keyof Macros, { label: string; className: string; muted?: boolean }> = {
  calories: { label: 'Calories', className: 'text-right w-[11%]' },
  protein: { label: 'Protein', className: 'text-right w-[11%]' },
  carbohydrates: { label: 'Carbs', className: 'text-right w-[7%] pr-0.5' },
  sugar: { label: '(Sugars)', className: 'text-left w-[4%] pl-0.5', muted: true },
  fat: { label: 'Fat', className: 'text-right w-[7%] pr-0.5' },
  saturatedFat: { label: '(Sat)', className: 'text-left w-[4%] pl-0.5', muted: true },
  fibre: { label: 'Fibre', className: 'text-right w-[7%]' },
  salt: { label: 'Salt', className: 'text-right w-[7%]' }
};

function Muted({ muted, children }: { muted?: boolean; children: ReactNode }) {
  return muted ? <span className='text-muted-foreground'>{children}</span> : children;
}

export function MacroHeaderCells() {
  return MACRO_KEYS.map(key => (
    <TableHead key={key} className={cn('text-xs', COLUMNS[key].className)}>
      <Muted muted={COLUMNS[key].muted}>{COLUMNS[key].label}</Muted>
    </TableHead>
  ));
}

export function MacroCells({ macros, className }: { macros: Macros; className?: string }) {
  return MACRO_KEYS.map(key => (
    <TableCell key={key} className={cn(COLUMNS[key].className, className)}>
      <Muted muted={COLUMNS[key].muted}>{formatMacro(key, macros[key])}</Muted>
    </TableCell>
  ));
}
