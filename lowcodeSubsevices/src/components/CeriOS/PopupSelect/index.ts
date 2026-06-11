import { withInstall } from '@/utils';
import type { ExtractPropTypes } from 'vue';
import PopupSelect from './src/PopupSelect.vue';
import PopupTableSelect from './src/PopupTableSelect.vue';
import { popupSelectProps } from './src/props';

export const CeriPopupSelect = withInstall(PopupSelect);
export const CeriPopupTableSelect = withInstall(PopupTableSelect);
export declare type PopupSelectProps = Partial<ExtractPropTypes<typeof popupSelectProps>>;
