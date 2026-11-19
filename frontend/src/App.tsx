import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import { Toaster } from '@/components/ui/sonner';
import AddNewFood from '@/pages/AddNewFood';
import AddNewMeal from '@/pages/AddNewMeal';
import AddNewMealPlanPage from '@/pages/AddNewMealPlan';
import HomePage from '@/pages/Home';
import Layout from '@/pages/Layout';
import NotFoundPage from '@/pages/NotFound';
import UpdateFoodPage from '@/pages/UpdateFood';
import ViewMealPlan from '@/pages/ViewMealPlan';
import { ApiError } from '@/types';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      // 4xx won't change on retry; retry only server and network failures.
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
            <Route path='/mealplan/:id' element={<ViewMealPlan />} />
            <Route path='/mealplan/new' element={<AddNewMealPlanPage />} />
            <Route path='/food/new' element={<AddNewFood />} />
            <Route path='/food/:id' element={<UpdateFoodPage />} />
            <Route path='/meal/new' element={<AddNewMeal />} />
            <Route path='*' element={<NotFoundPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}
