import dayjs from 'dayjs'

export function formatDate(date) {
  return dayjs(date).format('YYYY-MM-DD')
}

export function formatDateTime(date) {
  return dayjs(date).format('YYYY-MM-DD HH:mm:ss')
}

// 获取当前日期的函数
export function getLast7DaysDates() {
  const currentDate = new Date() // 获取当前日期
  const dates = []

  for (let i = 0; i < 7; i++) {
    const date = new Date()
    date.setDate(currentDate.getDate() - i) // 每次减去i天
    dates.unshift(`${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`)
  }
  return dates
}
