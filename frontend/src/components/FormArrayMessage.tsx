interface FormArrayMessageProps {
  // zodResolver puts array-level errors on the array itself, or on `.root` once rows are registered.
  error?: { message?: string; root?: { message?: string } };
}

export default function FormArrayMessage({ error }: FormArrayMessageProps) {
  const message = error?.message ?? error?.root?.message;

  return message ? <p className='text-sm font-medium text-destructive'>{message}</p> : null;
}
