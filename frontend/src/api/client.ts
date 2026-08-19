import { API_URL } from '@/config';
import { ApiError } from '@/types';

import type { ValidationProblemDetails } from '@/types';

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';

type Headers = Record<string, string>;

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
        problemDetails = JSON.parse(rawText) as ValidationProblemDetails;

        errorTitle = problemDetails.title || errorTitle;
      } catch {
        errorTitle = rawText;
      }
    }
  } catch {
    // Ignore stream reading errors and fallback to status code.
  }

  throw new ApiError(errorTitle, problemDetails);
}

async function request<TResponse, TBody = unknown>(
  method: HttpMethod,
  path: string,
  body?: TBody,
  headers?: Record<string, string>
): Promise<TResponse> {
  console.log(`${API_URL}${path}`);

  const res = await fetch(`${API_URL}${path}`, {
    method,
    credentials: 'include',
    headers: {
      ...(body !== undefined ? { 'Content-Type': 'application/json' } : {}),
      ...headers
    },
    body: body !== undefined ? JSON.stringify(body) : undefined
  });

  return handleResponse<TResponse>(res);
}

export const get = <TResponse>(path: string, headers?: Headers): Promise<TResponse> =>
  request<TResponse>('GET', path, undefined, headers);

export const post = <TResponse, TBody = unknown>(path: string, body?: TBody, headers?: Headers): Promise<TResponse> =>
  request<TResponse, TBody>('POST', path, body, headers);

export const put = <TResponse, TBody = unknown>(path: string, body?: TBody, headers?: Headers): Promise<TResponse> =>
  request<TResponse, TBody>('PUT', path, body, headers);

export const del = <TResponse>(path: string, headers?: Headers): Promise<TResponse> =>
  request<TResponse>('DELETE', path, undefined, headers);
