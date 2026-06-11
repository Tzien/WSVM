import { withInstall } from '@/utils';
import type { ExtractPropTypes } from 'vue';
import Button from './src/Button.vue';
import { buttonProps } from './src/props';

export const CeriButton = withInstall(Button);
export declare type ButtonProps = Partial<ExtractPropTypes<typeof buttonProps>>;
