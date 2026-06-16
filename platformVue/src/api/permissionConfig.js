import { businessInstance } from '@/utils/request'

//查询系统名称树数据
export const getAllSysInfoTreeApi = () => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfoTree')
}

//通过系统id获取接口列表
export const getSysInterfaceBySysIdApi = (obj) => {
  return businessInstance.get('/api/SysInterface/GetSysInterfaceBySysId', {
    params: obj
  })
}

//查询权限列表
export const getSysPermissionListApi = (obj) => {
  return businessInstance.get('/api/SysPermission/GetSysPermissionList', {
    params: obj
  })
}

//保存权限
export const savePermissionApi = (obj) => {
  return businessInstance.post('/api/SysPermission/SavePermission', obj)
}

//删除权限
export const deleteApi = (obj) => {
  return businessInstance.get('/api/SysPermission/DeletePermission', {
    params: obj
  })
}

//获取权限详情
export const getPermissionDetailApi = (obj) => {
  return businessInstance.get('/api/SysPermission/GetPermissionDetail', {
    params: obj
  })
}

//表格导出
export const tableExportApi = (obj) => {
  return businessInstance.get('/api/SysPermission/Export', {
    params: obj,
    responseType: 'blob'
  })
}
//表格导入
export const tableImportApi = (obj) => {
  return businessInstance.post('/api/SysPermission/Import', obj, {
    responseType: 'blob'
  })
}

//查询用户
export const getUserListApi = (obj) => {
  return businessInstance.get('/api/SysPermission/GetUserPermissionList', {
    params: obj
  })
}
