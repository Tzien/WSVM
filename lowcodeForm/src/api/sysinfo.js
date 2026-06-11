import { businessInstance } from '../utils/request'

export const useGetAllSysInfo = (name) => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfo', {
    params: { name }
  })
}

//获取平台信息配置详情
export const getPtInfoConfigDetailApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetPtInfoConfigDetail', {
    params: obj
  })
}

//添加/修改 平台信息配置
export const savePtInfoConfigApi = (obj) => {
  return businessInstance.post('/api/SysInfo/AddOrUpdatePtInfoConfig', obj)
}

//新增系统信息
export const addSysInfoApi = (obj) => {
  return businessInstance.post('/api/SysInfo/AddSysInfoAsync', obj)
}
//修改系统信息
export const updateSysInfoApi = (obj) => {
  return businessInstance.post('/api/SysInfo/UpdateSysInfoAsync', obj)
}
//删除系统信息
export const deletionSysInfoApi = (obj) => {
  return businessInstance.get('/api/SysInfo/SoftDeletionSysInfoAsync', {
    params: obj
  })
}
//获取系统信息列表
export const getSysInfoAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetSysInfoAsync', {
    params: obj
  })
}
//获取系统信息详情
export const getSysInfoByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetSysInfoByIdAsync', {
    params: obj
  })
}

//上传图片
export const uploadApi = (obj) => {
  return businessInstance.post('/api/SysInfo/upload', obj)
}
