import { withInstall } from '@/utils';
import DatePicker from './src/DatePicker.vue';
import DateRange from './src/DateRange.vue';
import TimePicker from './src/TimePicker.vue';
import TimeRange from './src/TimeRange.vue';

export const CeriDatePicker = withInstall(DatePicker);
export const CeriDateRange = withInstall(DateRange);
export const CeriTimePicker = withInstall(TimePicker);
export const CeriTimeRange = withInstall(TimeRange);
