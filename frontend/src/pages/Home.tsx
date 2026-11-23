import { useState } from 'react';
import { Carrot, Hamburger, List, UtensilsCrossed } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

import FoodSearchPopover from '@/components/FoodSearchPopover';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import MealPlanSearchPopover from '@/features/mealplan/MealPlanSearchPopover';

import type { Food, MealPlan } from '@/types';
import type { ReactNode } from 'react';

const createActions = [
  {
    title: 'New Meal Plan',
    description: 'Plan a full day of eating',
    icon: List,
    path: '/mealplan/new'
  },
  {
    title: 'New Meal',
    description: 'Assemble foods together',
    icon: Hamburger,
    path: '/meal/new'
  },
  {
    title: 'New Food',
    description: 'Add a base ingredient',
    icon: Carrot,
    path: '/food/new'
  }
];

export default function HomePage() {
  const [planSearchTerm, setPlanSearchTerm] = useState<string>('');

  const [foodSearchTerm, setFoodSearchTerm] = useState<string>('');

  const navigate = useNavigate();

  const handlePlanSelect = (selectedPlan: MealPlan) => {
    navigate(`/mealplan/${selectedPlan.id}`);
  };

  const handleFoodSelect = (selectedFood: Food) => {
    navigate(`/food/${selectedFood.id}`);
  };

  return (
    <div className='w-full max-w-2xl mx-auto pt-10'>
      <Card>
        <CardHeader className='text-center space-y-2 pb-6'>
          <div className='mx-auto bg-primary/10 w-12 h-12 rounded-full flex items-center justify-center mb-1'>
            <UtensilsCrossed className='h-6 w-6 text-primary' />
          </div>
          <CardTitle className='text-3xl font-extrabold tracking-tight'>Meal Planner</CardTitle>
          <CardDescription className='text-sm text-muted-foreground max-w-sm mx-auto'>
            Search existing food, meals, and plans or create new ones from scratch.
          </CardDescription>
        </CardHeader>

        <CardContent className='space-y-6'>
          <SectionDivider label='Search existing' />

          <div className='space-y-4'>
            <SearchField label='Meal Plans'>
              <MealPlanSearchPopover
                value={planSearchTerm}
                onValueChange={setPlanSearchTerm}
                onSelect={handlePlanSelect}
              />
            </SearchField>

            <SearchField label='Food'>
              <FoodSearchPopover
                value={foodSearchTerm}
                onValueChange={setFoodSearchTerm}
                onSelect={handleFoodSelect}
                allowAddNew={false}
              />
            </SearchField>
          </div>

          <SectionDivider label='Create from scratch' />

          <div className='grid grid-cols-1 gap-3'>
            {createActions.map(({ title, description, icon: Icon, path }) => (
              <Button
                key={path}
                type='button'
                variant='outline'
                onClick={() => navigate(path)}
                className='h-16 justify-start px-4 border-dashed border-2 hover:border-primary hover:bg-primary/5 cursor-pointer group transition-all'
              >
                <div className='bg-primary/10 p-2 rounded-lg mr-3 text-primary group-hover:scale-110 transition-transform'>
                  <Icon className='h-5 w-5' />
                </div>
                <div className='text-left'>
                  <div className='font-bold text-foreground text-sm'>{title}</div>
                  <div className='text-xs text-muted-foreground font-normal'>{description}</div>
                </div>
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

function SectionDivider({ label }: { label: string }) {
  return (
    <div className='relative py-2'>
      <div className='absolute inset-0 flex items-center'>
        <span className='w-full border-t' />
      </div>
      <div className='relative flex justify-center'>
        <span className='bg-card px-2 text-muted-foreground font-semibold uppercase tracking-widest text-[10px]'>
          {label}
        </span>
      </div>
    </div>
  );
}

function SearchField({ label, children }: { label: string; children: ReactNode }) {
  return (
    <label className='block space-y-1.5'>
      <span className='text-xs font-bold uppercase tracking-wider text-muted-foreground'>{label}</span>
      {children}
    </label>
  );
}
