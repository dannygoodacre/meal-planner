import { Outlet } from 'react-router-dom';

export default function Layout() {
  return (
    <div className='bg-slate-50 min-h-screen w-full p-6'>
      <Outlet />
    </div>
  );
}
