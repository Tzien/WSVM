import type { Rule } from 'ant-design-vue/es/form';
import type { ComponentType } from './types/index';
import { dateUtil } from '@/utils/dateUtils';
import { isNumber, isObject } from '@/utils/is';
// import { useI18n } from '@/hooks/web/useI18n';
// const { t } = useI18n();
import { getSafeT } from '@/lang/safeI18n';

/**
 * @description: 生成placeholder
 */
export function createPlaceholderMessage(component: ComponentType) {
  const t = getSafeT();
  if (component.includes('Input') || component.includes('Complete')) {
    return t('common.inputText');
  }
  if (component.includes('Picker')) {
    return t('common.chooseText');
  }
  if (
    component.includes('Select') ||
    component.includes('Cascader') ||
    component.includes('Checkbox') ||
    component.includes('Radio') ||
    component.includes('Switch')
  ) {
    // return `请选择${label}`;
    return t('common.chooseText');
  }
  return '';
}

const DATE_TYPE = ['DatePicker', 'MonthPicker', 'WeekPicker'];

function genType() {
  return [...DATE_TYPE, 'DateRange'];
}

export function setComponentRuleType(rule: Rule, component: ComponentType, valueFormat: string) {
  if (['MonthPicker', 'WeekPicker'].includes(component)) {
    rule.type = valueFormat ? 'string' : 'object';
  } else if (['DateRange', 'TimeRange', 'Upload', 'CheckboxGroup'].includes(component)) {
    rule.type = 'array';
  } else if (['InputNumber', 'Switch', 'DatePicker'].includes(component)) {
    rule.type = 'number';
  }
}

export function processDateValue(attr: Recordable, component: string) {
  const { valueFormat, value } = attr;
  if (valueFormat) {
    attr.value = isObject(value) ? dateUtil(value as any).format(valueFormat) : value;
  } else if (DATE_TYPE.includes(component) && value) {
    attr.value = dateUtil(attr.value);
  }
}

export function handleInputNumberValue(component?: ComponentType, val?: any) {
  if (!component) return val;
  if (['Input', 'InputPassword', 'InputSearch', 'TextArea'].includes(component)) {
    return val && isNumber(val) ? `${val}` : val;
  }
  return val;
}

/**
 * 时间字段
 */
export const dateItemType = genType();

export const defaultValueComponents = ['Input', 'InputPassword', 'InputSearch', 'TextArea'];
export const noFieldComponents = ['Button', 'Divider', 'GroupTitle', 'Link', 'Text', 'Alert', 'Qrcode', 'Barcode'];
export const numberItemType = ['Slider', 'Switch'];
export const useInputComponents = ['BillRule', 'ModifyUser', 'ModifyTime'];
