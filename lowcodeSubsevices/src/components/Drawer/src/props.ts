import type { PropType } from 'vue';

import { getI18n } from '@/lang/i18n'

function getT() {
  try {
    return getI18n().global.t as (key: string, ...args: any[]) => string;
  } catch (e) {
    return ((key: string) => key) as (key: string, ...args: any[]) => string;
  }
}

export const footerProps = {
  confirmLoading: { type: Boolean },
  /**
   * @description: Show close button
   */
  showCancelBtn: { type: Boolean, default: true },
  cancelButtonProps: Object as PropType<Recordable>,
  cancelText: { type: String, default: () => getT()('common.cancelText') },
  /**
   * @description: Show confirmation button
   */
  showOkBtn: { type: Boolean, default: true },
  okButtonProps: Object as PropType<Recordable>,
  okText: { type: String, default: () => getT()('common.okText') },
  okType: { type: String, default: 'primary' },
  continueText: { type: String, default: () => getT()('common.continueText') },
  continueType: { type: String, default: 'default' },
  showContinueBtn: { type: Boolean, default: false },
  continueButtonProps: Object as PropType<Recordable>,
  continueLoading: { type: Boolean },
  showFooter: { type: Boolean },
  footerHeight: {
    type: [String, Number] as PropType<string | number>,
    default: 60,
  },
};
export const basicProps = {
  isDetail: { type: Boolean },
  title: { type: String, default: '' },
  loadingText: { type: String },
  showDetailBack: { type: Boolean, default: true },
  open: { type: Boolean },
  loading: { type: Boolean },
  maskClosable: { type: Boolean, default: true },
  keyboard: { type: Boolean, default: false },
  getContainer: {
    type: [Object, String] as PropType<any>,
  },
  closeFunc: {
    type: [Function, Object] as PropType<any>,
    default: null,
  },
  destroyOnClose: { type: Boolean },
  ...footerProps,
};
