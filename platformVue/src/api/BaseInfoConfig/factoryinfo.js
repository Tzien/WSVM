import { businessInstance } from '@/utils/request'

//保存
export const saveFactoryInfoApi = (obj) => {
  return businessInstance.post('/api/FactoryInfo/SaveFactoryInfo', obj)
}
//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/FactoryInfo/DeleteFactoryInfo', {
    params: obj
  })
}
//列表
export const getListAsyncApi = (obj) => {
  return businessInstance.get('/api/FactoryInfo/GetFactoryInfoList', {
    params: obj
  })
}
//详情
export const getDetailByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/FactoryInfo/GetFactoryInfoDetail', {
    params: obj
  })
}


//标签列表
export const getAllLabelInfoApi = (obj) => {
  return businessInstance.get('/api/LabelInfo/GetLabelSelectList', {
    params: obj
  })
}


//查询工厂树
export const getFactoryTreeDataApi = (obj) => {
  return businessInstance.get('/api/FactoryInfo/GetSysGroupTreeAsync', {
    params: obj
  })
}


//查询介质树
export const getMediaTreeDataApi = (obj) => {
  return businessInstance.get('/api/MediaInfo/GetSysMediaTreeAsync', {
    params: obj
  })
}


//保存角色工厂
export const insertSysRoleGroupAsyncApi = (obj) => {
  return businessInstance.post('/api/FactoryInfo/InsertSysRoleGroupAsync', obj)
}

//保存角色介质
export const insertSysMediaAsyncApi = (obj) => {
  return businessInstance.post('/api/MediaInfo/InsertSysMediaAsync', obj)
}

//查询角色已绑定工厂和介质
export const selectedGroupAndMediaAsyncApi = (obj) => {
  return businessInstance.get('/api/FactoryInfo/SelectedGroupAndMediaAsync', {
    params: obj
  })
}

//查询工厂下拉框
export const useGetFactorySelect = (btnid) => {
  return businessInstance.get('/api/FactoryInfo/GetFactorySelect',{
    headers: {
      'buttonoperationid': btnid
    }
  })
}