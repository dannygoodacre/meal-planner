import { ChevronLeft } from 'lucide-react';
import { Link, Outlet, useLocation } from 'react-router-dom';

import { Button } from '@/components/ui/button';

export default function Layout() {
  const { pathname } = useLocation();

  return (
    <div className='bg-slate-50 min-h-screen w-full p-6 flex flex-col'>
      {pathname !== '/' && (
        <header className='mb-6'>
          <Button asChild variant='outline' className='h-10 px-3 text-lg font-bold text-primary'>
            <Link to='/'>
              <ChevronLeft className='size-6' />
              Home
            </Link>
          </Button>
        </header>
      )}

      <Outlet />

      <footer className='mt-auto pt-8 text-center text-xs text-muted-foreground'>
        v{import.meta.env.VITE_APP_VERSION ?? '0.0.0-dev'}
      </footer>
    </div>
  );
}
