import { businessInstance } from '@/utils/request'

//保存
export const saveLabelInfoApi = (obj) => {
  return businessInstance.post('/api/LabelInfo/SaveLabelInfo', obj)
}
//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/LabelInfo/DeleteLabelInfo', {
    params: obj
  })
}
//列表
export const getListAsyncApi = (obj) => {
  return businessInstance.get('/api/LabelInfo/GetLabelInfoList', {
    params: obj
  })
}
//详情
export const getDetailByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/LabelInfo/GetLabelInfoDetail', {
    params: obj
  })
}