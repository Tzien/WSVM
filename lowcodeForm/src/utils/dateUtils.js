import dayjs from 'dayjs'
import { getDateFormat } from '@/utils/ceri'

const DATE_FORMAT = 'YYYY-MM-DD'

export function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

export function formatDateTime(date) {
  return date ? dayjs(date).format('YYYY-MM-DD HH:mm:ss') : null
}

export function formatToDate(date = undefined, format = DATE_FORMAT) {
  return dayjs(date).format(getDateFormat(format))
}

export const dateUtil = dayjs
