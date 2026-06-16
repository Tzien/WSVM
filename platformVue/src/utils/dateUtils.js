import dayjs from 'dayjs'

export function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

export function formatDateTime(date) {
  if (date) {
    return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
  } else {
    return ''
  }
}
