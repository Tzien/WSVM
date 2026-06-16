import { businessInstance } from '@/utils/request'

//获取所有字典类型
export const getDictTypetApi = () => {
  return businessInstance.get('/api/DataDict/GetDictType')
}

//查询子系统名称树数据
export const getAllSysInfoTreeApi = () => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfoTree')
}

//获取数据字典列表
export const getAllDictApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetAllDict', {
    params: obj
  })
}

//保存数据字典
export const saveDictApi = (obj) => {
  return businessInstance.post('/api/DataDict/SaveDict', obj)
}

//获取数据字典详情
export const getDictInfoByIdApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetDictInfoById', {
    params: obj
  })
}

//保存数据字典子项
export const saveDictItemApi = (obj) => {
  return businessInstance.post('/api/DataDict/SaveDictItem', obj)
}

//获取数据字典子项列表
export const getAllDictItemApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetAllDictItem', {
    params: obj
  })
}
//获取数据字典子项详情
export const getDictItemInfoByIdApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetDictItemInfoById', {
    params: obj
  })
}

//删除数据字典
export const deleteDictApi = (obj) => {
  return businessInstance.get('/api/DataDict/DeleteDict', {
    params: obj
  })
}
//删除数据字典子项
export const deleteDictItemApi = (obj) => {
  return businessInstance.get('/api/DataDict/DeleteDictItem', {
    params: obj
  })
}

//导出
export const exportApi = (obj) => {
  return businessInstance.get('/api/DataDict/Export', {
    params: obj,
    responseType: 'blob'
  })
}
//导入
export const importApi = (obj) => {
  return businessInstance.post('/api/DataDict/Import', obj)
}



//添加分类
export const saveCategoryApi = (obj) => {
  return businessInstance.post('/api/DataDict/SaveCategory', obj)
}
//删除分类
export const deleteCategoryApi = (obj) => {
  return businessInstance.get('/api/DataDict/DeleteCategory', {
    params: obj
  })
}
//修改分类
export const updateCategoryApi = (obj) => {
  return businessInstance.post('/api/DataDict/UpdateCategory', obj)
}
//获取分类详情
export const getCategoryByIdApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetCategoryById', {
    params: obj
  })
}
//获取分类列表
export const getAllDictCategorytApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetAllDictCategoryt', {
    params: obj
  })
}
//获取所有分类
export const getAllDictCategoryNoPageApi = (obj) => {
  return businessInstance.get('/api/DataDict/GetAllDictCategoryNoPage', {
    params: obj
  })
}