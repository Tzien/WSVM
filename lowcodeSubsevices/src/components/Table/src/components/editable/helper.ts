import { ComponentType } from '../../types/componentType';
// import { useI18n } from '@/hooks/web/useI18n';


// const { t } = useI18n();

import { getSafeT } from '@/lang/safeI18n';

/**
 * @description: 生成placeholder
 */
export function createPlaceholderMessage(component: ComponentType) {
  const t = getSafeT();
  if (component.includes('Input') || component.includes('AutoComplete')) {
    return t('common.inputText');
  }
  if (component.includes('Picker')) {
    return t('common.chooseText');
  }

  if (
    component.includes('Select') ||
    component.includes('Checkbox') ||
    component.includes('Radio') ||
    component.includes('Switch') ||
    component.includes('DatePicker') ||
    component.includes('TimePicker')
  ) {
    return t('common.chooseText');
  }
  return '';
}
