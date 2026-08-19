import { useWatch } from 'react-hook-form';

import type { AddFoodRequest } from '@/types';
import type { Control, Path } from 'react-hook-form';

export default function useDoesSubMacroExceedMacro(
  control: Control<AddFoodRequest>,
  subMacro: Path<AddFoodRequest>,
  macro: Path<AddFoodRequest>
) {
  const subMacroValue = useWatch({ control: control, name: subMacro });

  const macroValue = useWatch({ control: control, name: macro });

  return (subMacroValue || 0) > (macroValue || 0);
}
