import { getI18n } from '@/lang/i18n'

function getT() {
  try {
    return getI18n().global.t as (key: string, ...args: any[]) => string;
  } catch (e) {
    return ((key: string) => key) as (key: string, ...args: any[]) => string;
  }
}

export const popupSelectProps = {
  value: [String, Number, Array] as PropType<String | number | string[] | number[] | [string, number][]>,
  interfaceId: { type: String, default: '' },
  templateJson: { type: Array, default: () => [] },
  relationField: { type: String, default: 'fullName' },
  propsValue: { type: String, default: 'id' },
  field: { type: String, default: '' },
  placeholder: { type: String, default: () => getT()('common.chooseText') },
  extraOptions: { type: Array, default: () => [] },
  columnOptions: { type: Array, default: () => [] },
  hasPage: { type: Boolean, default: false },
  pageSize: { type: Number, default: 20 },
  allowClear: { type: Boolean, default: true },
  size: { type: String },
  disabled: { type: Boolean, default: false },
  multiple: { type: Boolean, default: false },
  popupType: { type: String, default: 'dialog' },
  popupTitle: { type: String, default: () => getT()('component.ceri.popupSelect.modalTitle') },
  popupWidth: { type: String, default: '800px' },
  rowIndex: { default: null },
  formData: { type: Object },
};
