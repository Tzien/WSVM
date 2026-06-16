import { businessInstance } from '../utils/request'

//获取用户登录日志详情
export const getuserLoginLogs = (obj) => {
  return businessInstance.get('/api/SysLoginUserLog/GetSysLoginUserLogAsync', {
    params: obj
  })
}

//删除全部
export const deleteAllApi = () => {
  return businessInstance.get('/api/SysLoginUserLog/SoftDeleteAllSysLoginUserLogAsync')
}

//删除所选
export const deleteSelectApi = (obj) => {
  return businessInstance.post('/api/SysLoginUserLog/SoftDeleteSysLoginUserLogAsync', obj)
}

//添加日志
export const useInsertSysMenuRecord = (obj) => {
  return businessInstance.post('/api/SysMenuRecord/InsertSysMenuRecord', obj)
}
