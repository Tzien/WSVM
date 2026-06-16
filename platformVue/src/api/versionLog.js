import { businessInstance } from '@/utils/request'

export const getVersionInfo = (obj) => {
  return businessInstance.get('/api/SysVersionLog/GetSysVersionLogAsync', {
    params: obj
  })
}

//添加版本更新记录
export const addVersionInfo = (obj) => {
  return businessInstance.post('/api/SysVersionLog/AddSysVersionLogAsync', obj)
}

//根据Id获取
export const getVersionInfoById = (obj) => {
  return businessInstance.get('/api/SysVersionLog/GetSysVersionLogByIdAsync', {
    params: obj
  })
}

//删除
export const deleteApi = (obj) => {
  return businessInstance.post('/api/SysVersionLog/SoftDeleteSysVersionLogAsync', obj)
}
//修改
export const modifyApi = (sysjob) => {
  return businessInstance.post('/api/SysVersionLog/UpdateSysVersionLogAsync', sysjob)
}
