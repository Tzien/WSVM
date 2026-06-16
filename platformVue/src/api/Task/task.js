import { taskInstance } from '../../utils/request'

//获取未来五次执行时间
export const getTaskeFireTimeApi = (obj) => {
  return taskInstance.get('/api/Task/GetTaskeFireTime', {
    params: obj
  })
}

//添加任务
export const addTaskApi = (obj) => {
  return taskInstance.post('/api/Task/AddTask', obj)
}
//删除任务
export const deleteTaskApi = (obj) => {
  return taskInstance.get('/api/Task/DeleteTask', {
    params: obj
  })
}
//修改任务
export const updateTaskApi = (obj) => {
  return taskInstance.post('/api/Task/UpdateTask', obj)
}
//获取任务详情
export const getOneByIdApi = (obj) => {
  return taskInstance.get('/api/Task/GetOneById', {
    params: obj
  })
}
//获取任务列表
export const getAllTaskApi = (obj) => {
  return taskInstance.get('/api/Task/GetAllTask', {
    params: obj
  })
}
//开启/停止任务
export const startOrStopTaskApi = (obj) => {
  return taskInstance.get('/api/Task/StartOrStopTask', {
    params: obj
  })
}
//启动选中
export const startSelectApi = (obj) => {
  return taskInstance.get('/api/Task/StartSelect', {
    params: obj
  })
}


//获取任务日志列表
export const getAllTaskLogApi = (obj) => {
  return taskInstance.get('/api/Task/GetAllTaskLog', {
    params: obj
  })
}
//删除日志
export const deleteTaskLogsApi = (obj) => {
  return taskInstance.get('/api/Task/DeleteTaskLogs', {
    params: obj
  })
}