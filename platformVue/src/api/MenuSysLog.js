import { businessInstance } from '@/utils/request'

//获取菜单访问日志详情
export const getSysMenuRecord = (obj) => {
  return businessInstance.get('/api/SysMenuRecord/GetMenuRecordAsync', {
    params: obj
  })
}

//获取系统访问日志详情
export const getSysInfoRecord = (obj) => {
  return businessInstance.get('/api/SysMenuRecord/GetSysRecordAsync', {
    params: obj
  })
}

//删除全部
export const deleteAllApi = (boolObj) => {
  return businessInstance.get(`/api/SysMenuRecord/SoftDeleteAllSysMenuRecordAsync?IsSysInfoId=${boolObj}`)
}

//删除所选
export const deleteSelectApi = (obj) => {
  return businessInstance.post('/api/SysMenuRecord/SoftDeleteSysMenuRecordAsync', obj)
}

//添加日志
export const useInsertSysMenuRecord = (obj) => {
  return businessInstance.post('/api/SysMenuRecord/InsertSysMenuRecord', obj)
}
