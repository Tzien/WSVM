import { businessInstance } from '@/utils/request'

//查询系统名称树数据
export const getAllSysInfoTreeApi = () => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfoTree')
}

//新增接口信息
export const addInterfaceInfoApi = (obj) => {
  return businessInstance.post('/api/SysInterface/AddSysInterfaceAsync', obj)
}
//修改接口信息
export const updateInterfaceInfoApi = (obj) => {
  return businessInstance.post('/api/SysInterface/UpdateSysInterfaceAsync', obj)
}
//删除信息
export const deleteApi = (obj) => {
  return businessInstance.get(
    '/api/SysInterface/SoftDeletionSysInterfaceAsync',
    {
      params: obj
    }
  )
}
//获取接口列表
export const getListAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInterface/GetSysInterfaceAsync', {
    params: obj
  })
}
//获取接口信息详情
export const getDetailByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInterface/GetSysInterfaceByIdAsync', {
    params: obj
  })
}
//解析swagger
export const getSwaggerInfoApi = (obj) => {
  return businessInstance.get('/api/SysInterface/GetSwaggerInfo', {
    params: obj
  })
}
//swagger导入
export const importInterfaceApi = (obj) => {
  return businessInstance.post('/api/SysInterface/ImportInterface', obj)
}
//表格导出
export const tableExportApi = (obj) => {
  return businessInstance.get('/api/SysInterface/TableExport', {
    params: obj,
    responseType: 'blob'
  })
}
//表格导入
export const tableImportApi = (obj) => {
  return businessInstance.post('/api/SysInterface/TableImport', obj, {
    responseType: 'blob'
  })
}
