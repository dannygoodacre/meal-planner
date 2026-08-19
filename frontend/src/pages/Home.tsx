import { useState } from 'react';
import { Carrot, Hamburger, List, UtensilsCrossed } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

import FoodSearchPopover from '@/components/common/FoodSearchPopover';
import MealPlanSearchPopover from '@/components/common/MealPlanSearchPopover';
import MealSearchPopover from '@/components/common/MealSearchPopover';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

import type { Food, Meal, MealPlan } from '@/types';

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

  const [mealSearchTerm, setMealSearchTerm] = useState<string>('');

  const [foodSearchTerm, setFoodSearchTerm] = useState<string>('');

  const navigate = useNavigate();

  const handlePlanSelect = (selectedPlan: MealPlan) => {
    setPlanSearchTerm(selectedPlan.name);

    navigate(`/mealplan/${selectedPlan.id}`);
  };

  const handleMealSelect = (selectedMeal: Meal) => {
    setMealSearchTerm(selectedMeal.name);

    navigate(`/meal/${selectedMeal.id}`);
  };

  const handleFoodSelect = (selectedFood: Food) => {
    setFoodSearchTerm(selectedFood.name);

    navigate(`/food/${selectedFood.id}`);
  };

  return (
    <div className='w-full max-w-2xl mx-auto pt-16 px-4'>
      <Card className='border shadow-md bg-card'>
        <CardHeader className='text-center space-y-2 pb-6'>
          <div className='mx-auto bg-primary/10 w-12 h-12 rounded-full flex items-center justify-center mb-1'>
            <UtensilsCrossed className='h-6 w-6 text-primary' />
          </div>
          <CardTitle className='text-3xl font-extrabold tracking-tight'>Meal Planner</CardTitle>
          <CardDescription className='text-sm text-muted-foreground max-w-sm mx-auto'>
            Search existing food, meals, and plans or create new ones from scratch.
          </CardDescription>
        </CardHeader>

        <CardContent className='space-y-6 pb-8'>
          <div className='relative py-2'>
            <div className='absolute inset-0 flex items-center'>
              <span className='w-full border-t' />
            </div>
            <div className='relative flex justify-center text-xs uppercase'>
              <span className='bg-card px-2 text-muted-foreground font-semibold tracking-widest text-[10px]'>
                Search existing
              </span>
            </div>
          </div>

          <div className='space-y-4'>
            <div className='space-y-1.5'>
              <label className='text-xs font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-1.5'>
                Meal Plans
              </label>
              <MealPlanSearchPopover
                searchTerm={planSearchTerm}
                onSearchChange={setPlanSearchTerm}
                onPlanSelect={handlePlanSelect}
              />
            </div>

            <div className='space-y-1.5'>
              <label className='text-xs font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-1.5'>
                Meals
              </label>
              <MealSearchPopover
                mealName={mealSearchTerm}
                onSearchChange={setMealSearchTerm}
                onSelect={handleMealSelect}
                showCreateMealFromScratch={false}
              />
            </div>

            <div className='space-y-1.5'>
              <label className='text-xs font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-1.5'>
                Food
              </label>
              <FoodSearchPopover
                isSelected={false}
                foodName={foodSearchTerm}
                onSearchChange={setFoodSearchTerm}
                onSelect={handleFoodSelect}
                doShowAddNewMeal={false}
                keepOpenUntilSelection={false}
              />
            </div>
          </div>

          <div className='relative py-2'>
            <div className='absolute inset-0 flex items-center'>
              <span className='w-full border-t' />
            </div>
            <div className='relative flex justify-center text-xs uppercase'>
              <span className='bg-card px-2 text-muted-foreground font-semibold tracking-widest text-[10px]'>
                Create from scratch
              </span>
            </div>
          </div>

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
