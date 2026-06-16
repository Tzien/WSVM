import { businessInstance } from '../utils/request'

//获取岗位信息详情
export const getJobInfo = (obj) => {
  return businessInstance.get('/api/SysJob/GetSysJobAsync', {
    params: obj
  })
}

//添加岗位信息详情
export const addSysJob = (sysjob) => {
  return businessInstance.post('/api/SysJob/AddSysJobAsync', sysjob)
}
//根据Id获取
export const getJobInfoById = (obj) => {
  return businessInstance.get('/api/SysJob/GetSysJobByIdAsync', {
    params: obj
  })
}
//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/SysJob/SoftDeletionSysJobAsync', {
    params: obj
  })
}
//修改
export const modifySysJob = (sysjob) => {
  return businessInstance.post('/api/SysJob/UpdateSysJobAsync', sysjob)
}
