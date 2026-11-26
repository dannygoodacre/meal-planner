import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import { ApiError } from '@/api/client';
import { Toaster } from '@/components/ui/sonner';
import AddNewFoodPage from '@/pages/AddNewFood';
import AddNewMealPage from '@/pages/AddNewMeal';
import AddNewMealPlanPage from '@/pages/AddNewMealPlan';
import HomePage from '@/pages/Home';
import Layout from '@/pages/Layout';
import NotFoundPage from '@/pages/NotFound';
import UpdateFoodPage from '@/pages/UpdateFood';
import ViewMealPage from '@/pages/ViewMeal';
import ViewMealPlanPage from '@/pages/ViewMealPlan';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: (failureCount, error) => !(error instanceof ApiError && error.status < 500) && failureCount < 3
    }
  }
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Toaster richColors closeButton position='bottom-center' />

        <Routes>
          <Route element={<Layout />}>
            <Route path='/' element={<HomePage />} />
            <Route path='/mealplan/:id' element={<ViewMealPlanPage />} />
            <Route path='/mealplan/new' element={<AddNewMealPlanPage />} />
            <Route path='/food/new' element={<AddNewFoodPage />} />
            <Route path='/food/:id' element={<UpdateFoodPage />} />
            <Route path='/meal/new' element={<AddNewMealPage />} />
            <Route path='/meal/:id' element={<ViewMealPage />} />
            <Route path='*' element={<NotFoundPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
