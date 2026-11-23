export class ApiError extends Error {
  status: number;
  details?: ValidationProblemDetails;

  constructor(message: string, status: number, details?: ValidationProblemDetails) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.details = details;
  }
}

export interface ValidationProblemDetails {
  type?: string;
  title?: string;
  detail?: string;
  status?: number;
  traceId?: string;
  errors?: Record<string, string[]>;
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (res.status === 204) {
    return undefined as T;
  }

  if (res.ok) {
    try {
      return await res.json();
    } catch {
      return undefined as T;
    }
  }

  let errorTitle = `API Error ${res.status}`;

  let problemDetails: ValidationProblemDetails | undefined;

  try {
    const rawText = await res.text();

    if (rawText) {
      try {
        const parsed: unknown = JSON.parse(rawText);

        problemDetails = parsed as ValidationProblemDetails;

        errorTitle = problemDetails.detail || problemDetails.title || errorTitle;
      } catch {
        errorTitle = rawText;
      }
    }
  } catch {
    // Ignore stream reading errors and fallback to status code.
  }

  throw new ApiError(errorTitle, res.status, problemDetails);
}

async function request<T>(method: 'GET' | 'POST' | 'PUT', path: string, body?: unknown): Promise<T> {
  const res = await fetch(path, {
    method,
    headers: body !== undefined ? { 'Content-Type': 'application/json' } : undefined,
    body: body !== undefined ? JSON.stringify(body) : undefined
  });

  return handleResponse<T>(res);
}

export const get = <T>(path: string): Promise<T> => request<T>('GET', path);

export const post = <T>(path: string, body: unknown): Promise<T> => request<T>('POST', path, body);

export const put = <T>(path: string, body: unknown): Promise<T> => request<T>('PUT', path, body);
