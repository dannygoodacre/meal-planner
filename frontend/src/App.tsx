import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import { Toaster } from '@/components/ui/sonner';
import AddNewFood from '@/pages/AddNewFood';
import AddNewMeal from '@/pages/AddNewMeal';
import AddNewMealPlanPage from '@/pages/AddNewMealPlan';
import HomePage from '@/pages/Home';
import UpdateFoodPage from '@/pages/UpdateFood';
import ViewMealPlan from '@/pages/ViewMealPlan';

const queryClient = new QueryClient();

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Toaster richColors closeButton position='bottom-center' />

        <Routes>
          <Route path='/' element={<HomePage />} />
          <Route path='/mealplan/:id' element={<ViewMealPlan />} />
          <Route path='/mealplan/new' element={<AddNewMealPlanPage />} />
          <Route path='/food/new' element={<AddNewFood />} />
          <Route path='/food/:id' element={<UpdateFoodPage />} />
          <Route path='/meal/new' element={<AddNewMeal />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
