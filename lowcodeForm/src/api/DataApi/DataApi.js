import { businessInstance,lowCodeFormInstance } from '../../utils/request'



// //获取接口分类字典
export const getApiCategoryDictApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetDictInfoByCode', {
    params: obj
  })
}

// 获取数据库连接串
export const getDbConStrApi = () => {
  return lowCodeFormInstance.get('/api/DBConfig/GetDbSelect')
}
// 获取数据库表信息
export const getDbTablesApi = (obj) => {
  return lowCodeFormInstance.get('/api/DBConfig/GetAllTables', {
    params: obj
  })
}



//保存
export const saveApi = (obj) => {
  return lowCodeFormInstance.post('/api/DataApi/Save', obj)
}


//删除
export const deleteApi = (obj) => {
  return lowCodeFormInstance.get('/api/DataApi/Delete', {
    params: obj
  })
}


// 详情
export const getDetailApi = (obj) => {
  return lowCodeFormInstance.get('/api/DataApi/GetById', {
    params: obj
  })
}


// 列表
export const getListApi = (obj) => {
  return lowCodeFormInstance.get('/api/DataApi/GetList', {
    params: obj
  })
}

//测试连接
export const testApiApi = (id,obj) => {
  return lowCodeFormInstance.post(`/api/DataApi/TestApi/${id}`, obj)
}
//测试连接
export const getApiDataApi = (id,obj) => {
  return lowCodeFormInstance.post(`/api/DataApi/GetApiData/${id}`, obj)
}



export const getFormListApi = () => {
  return lowCodeFormInstance.get('/api/DataApi/GetFormList')
}