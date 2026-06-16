import dayjs from 'dayjs'

export function formatDate(date) {
  if (date) {
    return dayjs(date).format('YYYY-MM-DD')
  }
  return ''
}

export function formatDateTime(date) {
  if (date) {
    return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
  }
  return ''
}
