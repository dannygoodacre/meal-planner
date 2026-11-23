import { Card, CardAction, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { cn } from '@/utils';

import type { LucideIcon } from 'lucide-react';
import type { ReactNode } from 'react';

interface FormCardProps {
  icon: LucideIcon;
  title: ReactNode;
  description?: string;
  actions?: ReactNode;
  // Table-based forms (meals, plans) need the extra width.
  wide?: boolean;
  children: ReactNode;
}

export default function FormCard({ icon: Icon, title, description, actions, wide = false, children }: FormCardProps) {
  return (
    <Card className={cn('w-full mx-auto', wide ? 'max-w-5xl' : 'max-w-xl')}>
      <CardHeader>
        <CardTitle className='flex items-center gap-2 text-2xl font-bold min-w-0'>
          <Icon className='h-6 w-6 text-primary shrink-0' />
          {title}
        </CardTitle>

        {description && <CardDescription>{description}</CardDescription>}

        {actions && <CardAction className='flex items-center gap-2'>{actions}</CardAction>}
      </CardHeader>

      <CardContent className='space-y-4'>{children}</CardContent>
    </Card>
  );
}
