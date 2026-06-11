import { businessInstance } from '../../utils/request'

//保存
export const saveApi = (obj) => {
  return businessInstance.post('/api/DingTalkMsg/SaveDingTalkConfig', obj)
}
//查询列表数据
export const getAllApi = (obj) => {
  return businessInstance.get('/api/DingTalkMsg/GetAllDingTalkConfig', {
    params: obj
  })
}

//详情
export const getDetailApi = (obj) => {
  return businessInstance.get('/api/DingTalkMsg/GetByDingTalkConfigID', {
    params: obj
  })
}

//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/DingTalkMsg/DeleteAsyncDingTalkConfig', {
    params: obj
  })
}

//设为默认选中
export const setSelectApi = (obj) => {
  return businessInstance.get('/api/DingTalkMsg/UpdateSelectState', {
    params: obj
  })
}
