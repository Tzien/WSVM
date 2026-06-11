import { lowCodeFormInstance } from '@/utils/request'

export const getConfigDBList = (obj) => {
  return lowCodeFormInstance.get('/api/DBConfig/GetDBConfigAsync', {
    params: obj
  })
}

export const addConfigDBAsync = (obj) => {
  return lowCodeFormInstance.post('/api/DBConfig/InsertDBConfigAsync', obj)
}

//删除
export const deleteConfigDB = (obj) => {
  return lowCodeFormInstance.get('/api/DBConfig/DeleteDBConfigAsync', {
    params: obj
  })
}
//修改
export const modifyConfigDB = (sysjob) => {
  return lowCodeFormInstance.post('/api/DBConfig/UpdateDBConfigAsync', sysjob)
}

export const testDB = (obj) => {
  return lowCodeFormInstance.post('/api/DBConfig/TestDb', obj)
}

export const getConnectList = () => {
  return lowCodeFormInstance.get('/api/DBConfig/GetDbSelect')
}

// 获取数据库表列表
export function getDataModelList(data) {
  return lowCodeFormInstance.get(`/api/DBConfig/GetAllTables?dbConfigId=${data.linkId}`)
}

export function getDataModelFieldList(linkId, table) {
  return lowCodeFormInstance.get(`/api/DBConfig/GetAllTables?dbConfigId=${linkId}&tableName=${table}`)
}
