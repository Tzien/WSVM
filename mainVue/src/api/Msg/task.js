import { taskInstance } from '../../utils/request'

//获取任务日志列表
export const getAllTaskLogApi = (obj) => {
  return taskInstance.get('/api/Task/GetAllTaskLog', {
    params: obj
  })
}