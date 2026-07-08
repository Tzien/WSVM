import { businessInstance } from '../../utils/request'

//保存
export const saveApi = (obj) => {
  return businessInstance.post('/api/WeChatMsg/SaveWeChatConfig', obj)
}
//查询列表数据
export const getAllApi = (obj) => {
  return businessInstance.get('/api/WeChatMsg/GetAllWeChatConfig', {
    params: obj
  })
}

//详情
export const getDetailApi = (obj) => {
  return businessInstance.get('/api/WeChatMsg/GetByWeChatConfigID', {
    params: obj
  })
}

//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/WeChatMsg/DeleteAsyncWeChatConfig', {
    params: obj
  })
}

//设为默认选中
export const setSelectApi = (obj) => {
  return businessInstance.get('/api/WeChatMsg/UpdateSelectState', {
    params: obj
  })
}
