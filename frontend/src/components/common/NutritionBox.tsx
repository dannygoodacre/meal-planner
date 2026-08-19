import { Apple, Beef, Droplet, Flame, Leaf } from 'lucide-react';

import { Card, CardContent } from '@/components/ui/card';
import { cn } from '@/utils';

import type { LucideIcon } from 'lucide-react';

interface MacroTotals {
  calories: number;
  protein: number;
  carbohydrates: number;
  sugar: number;
  fat: number;
  saturatedFat: number;
  fibre: number;
  salt: number;
}

interface NutritionBoxProps {
  totals: MacroTotals;
}

export default function NutritionBox({ totals }: NutritionBoxProps) {
  const proteinCalories = 4 * totals.protein;
  const carbCalories = 4 * totals.carbohydrates;
  const fatCalories = 9 * totals.fat;

  const totalMacroCalories = proteinCalories + carbCalories + fatCalories;

  const proteinPercentage = totalMacroCalories > 0 ? Math.round((proteinCalories / totalMacroCalories) * 100) : 0;

  const carbPercentage = totalMacroCalories > 0 ? Math.round((carbCalories / totalMacroCalories) * 100) : 0;

  const fatPercentage = totalMacroCalories > 0 ? Math.round((fatCalories / totalMacroCalories) * 100) : 0;

  const round = (val: number) => Math.round(val * 10) / 10;

  return (
    <div className='grid grid-cols-2 md:grid-cols-4 gap-4'>
      <div className='flex flex-col gap-4'>
        <NutrientBox label='Calories' total={round(totals.calories)} unit='kcal' icon={Flame} theme='orange' />

        <NutrientBox label='Fibre' total={round(totals.fibre)} unit='g' icon={Leaf} theme='blue' />
      </div>

      <MacroBox
        label='Protein'
        total={round(totals.protein)}
        caloriePercentage={proteinPercentage}
        icon={Beef}
        theme='red'
      />

      <MacroBox
        label='Carbs'
        total={round(totals.carbohydrates)}
        caloriePercentage={carbPercentage}
        subTotalLabel='Sugars'
        subTotal={round(totals.sugar)}
        icon={Apple}
        theme='amber'
      />

      <MacroBox
        label='Fat'
        total={round(totals.fat)}
        caloriePercentage={fatPercentage}
        subTotalLabel='Saturates'
        subTotal={round(totals.saturatedFat)}
        icon={Droplet}
        theme='green'
      />
    </div>
  );
}

interface NutrientBoxProps {
  label: string;
  total: number;
  unit: string;
  decimalPlaces?: number;
  icon: LucideIcon;
  theme: MacroTheme;
}

function NutrientBox({ label, total, unit, decimalPlaces = 0, icon: Icon, theme }: NutrientBoxProps) {
  const styles = themeMap[theme];

  return (
    <Card className={cn(styles.card)}>
      <CardContent className='y-4 flex items-center gap-2 sm:gap-3 min-w-0 h-full'>
        <div className={cn('p-2 rounded-lg', styles.iconBackground, styles.iconText)}>
          <Icon className='h-5 w-5' />
        </div>

        <div>
          <p className='text-xs font-medium text-muted-foreground uppercase tracking-wider'>{label}</p>

          <p className='text-xl font-bold whitespace-nowrap tracking-tight'>
            {total.toFixed(decimalPlaces)} {unit}
          </p>
        </div>
      </CardContent>
    </Card>
  );
}

interface MacroBoxProps {
  label: string;
  total: number;
  caloriePercentage: number;
  subTotalLabel?: string;
  subTotal?: number;
  icon: LucideIcon;
  theme: MacroTheme;
}

function MacroBox({ label, total, caloriePercentage, subTotalLabel, subTotal, icon: Icon, theme }: MacroBoxProps) {
  const styles = themeMap[theme];

  return (
    <Card className={cn(styles.card)}>
      <CardContent className='p-4 flex items-center gap-2 sm:gap-3 min-w-0 h-full'>
        <div className={cn('p-2 rounded-lg shrink-0 self-start mt-5', styles.iconBackground, styles.iconText)}>
          <Icon className='h-5 w-5' />
        </div>

        <div className='min-w-0 w-full'>
          <div className='flex items-center justify-between gap-1 mb-0.5'>
            <p className='text-xs font-medium text-muted-foreground uppercase tracking-wider'>{label}</p>

            <span className={cn('text-[10px] font-bold px-1.5 py-0.5 rounded-full whitespace-nowrap', styles.badge)}>
              {caloriePercentage}% kcal
            </span>
          </div>

          <p className='text-xl font-bold whitespace-nowrap tracking-tight'>{total.toFixed(1)} g</p>

          <div
            className={cn(
              'mt-1 pt-1 border-t flex justify-between items-center text-[11px] text-muted-foreground/80 font-medium',
              styles.borderDivider,
              (!subTotalLabel || false) && 'invisible select-none'
            )}
          >
            <span className='truncate pr-1'>{subTotalLabel}:</span>
            <span className='font-semibold text-foreground whitespace-nowrap'>{subTotal?.toFixed(1)} g</span>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

type MacroTheme = 'amber' | 'blue' | 'green' | 'red' | 'orange';

const themeMap: Record<
  MacroTheme,
  {
    card: string;
    iconBackground: string;
    iconText: string;
    badge: string;
    borderDivider: string;
  }
> = {
  green: {
    card: 'bg-green-50/50 dark:bg-green-950/20 border-green-100 dark:border-green-900/50',
    iconBackground: 'bg-green-500/10',
    iconText: 'text-green-600 dark:text-green-400',
    badge: 'bg-green-500/10 text-green-700 dark:text-green-400',
    borderDivider: 'border-green-100/50 dark:border-green-900/30'
  },
  blue: {
    card: 'bg-blue-50/50 dark:bg-blue-950/20 border-blue-100 dark:border-blue-900/50',
    iconBackground: 'bg-blue-500/10',
    iconText: 'text-blue-600 dark:text-blue-400',
    badge: 'bg-blue-500/10 text-blue-700 dark:text-blue-400',
    borderDivider: 'border-blue-100/50 dark:border-blue-900/30'
  },
  amber: {
    card: 'bg-amber-50/50 dark:bg-amber-950/20 border-amber-100 dark:border-amber-900/50',
    iconBackground: 'bg-amber-500/10',
    iconText: 'text-amber-600 dark:text-amber-400',
    badge: 'bg-amber-500/10 text-amber-700 dark:text-amber-400',
    borderDivider: 'border-amber-100/50 dark:border-amber-900/30'
  },
  red: {
    card: 'bg-red-50/50 dark:bg-red-950/20 border-red-100 dark:border-red-900/50',
    iconBackground: 'bg-red-500/10',
    iconText: 'text-red-600 dark:text-red-400',
    badge: 'bg-red-500/10 text-red-700 dark:text-red-400',
    borderDivider: 'border-red-100/50 dark:border-red-900/30'
  },
  orange: {
    card: 'bg-orange-50/50 dark:bg-orange-950/20 border-orange-100 dark:border-orange-900/50',
    iconBackground: 'bg-orange-500/10',
    iconText: 'text-orange-600 dark:text-orange-400',
    badge: 'bg-orange-500/10 text-orange-700 dark:text-orange-400',
    borderDivider: 'border-orange-100/50 dark:border-orange-900/30'
  }
};
