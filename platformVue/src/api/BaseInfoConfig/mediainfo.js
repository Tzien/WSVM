import { businessInstance } from '@/utils/request'

//保存
export const saveMediaInfoApi = (obj) => {
  return businessInstance.post('/api/MediaInfo/SaveMediaInfo', obj)
}
//删除
export const deleteApi = (obj) => {
  return businessInstance.get('/api/MediaInfo/DeleteMediaInfo', {
    params: obj
  })
}
//列表
export const getListAsyncApi = (obj) => {
  return businessInstance.get('/api/MediaInfo/GetMediaInfoList', {
    params: obj
  })
}
//详情
export const getDetailByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/MediaInfo/GetMediaInfoDetail', {
    params: obj
  })
}