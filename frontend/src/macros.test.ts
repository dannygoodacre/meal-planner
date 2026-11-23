import { describe, expect, it } from 'vitest';

import { round, scaleMacro } from '@/macros';

describe('round', () => {
  it('rounds to 1 decimal by default', () => {
    expect(round(1.25)).toBe(1.3);
    expect(round(1.24)).toBe(1.2);
  });

  it('rounds to the given decimals', () => {
    expect(round(123.456, 0)).toBe(123);
    expect(round(1.236, 2)).toBe(1.24);
  });
});

describe('scaleMacro', () => {
  it('scales linearly from the reference quantity', () => {
    expect(scaleMacro(10, 250, 100)).toBe(25);
    expect(scaleMacro(80, 2, 1)).toBe(160);
  });

  it.each([
    [0, 100],
    [-5, 100],
    [NaN, 100],
    [50, 0]
  ])('returns 0 for quantity %s and reference %s', (quantity, reference) => {
    expect(scaleMacro(10, quantity, reference)).toBe(0);
  });
});
