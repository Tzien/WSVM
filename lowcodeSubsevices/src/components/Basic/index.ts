import { withInstall } from '@/utils';

import basicArrow from './src/BasicArrow.vue';
import basicHelp from '@/components/Basic/src/BasicHelp.vue';
import basicTitle from './src/BasicTitle.vue';
import basicCaption from '@/components/Basic/src/BasicCaption.vue';
export const BasicTitle = withInstall(basicTitle);
export const BasicHelp = withInstall(basicHelp);
export const BasicArrow = withInstall(basicArrow);
export const BasicCaption = withInstall(basicCaption);