import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <div className='text-center space-y-2'>
      <h1 className='text-2xl font-bold'>Page not found</h1>
      <Link to='/' className='text-primary underline-offset-4 hover:underline'>
        Back to home
      </Link>
    </div>
  );
}
