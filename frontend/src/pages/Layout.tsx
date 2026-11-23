import { Outlet } from 'react-router-dom';

export default function Layout() {
  return (
    <div className='bg-slate-50 min-h-screen w-full p-6 flex flex-col'>
      <Outlet />

      <footer className='mt-auto pt-8 text-center text-xs text-muted-foreground'>
        v{import.meta.env.VITE_APP_VERSION ?? '0.0.0-dev'}
      </footer>
    </div>
  );
}
