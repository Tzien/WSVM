// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/Signature'
}

// 获取签章列表(带分页)
export function getSignatureList(data) {
  return lowCodeInstance.get(Api.Prefix, data)
}
// 获取签章下拉框列表
export function getSignatureSelector() {
  return lowCodeInstance.get(Api.Prefix + `/Selector`)
}
// 新建签章
export function create(data) {
  return lowCodeInstance.post(Api.Prefix, data)
}
// 修改签章
export function update(data) {
  return lowCodeInstance.put(Api.Prefix + '/' + data.id, data)
}
// 获取签章
export function getInfo(id) {
  return lowCodeInstance.get(Api.Prefix + '/' + id)
}
// 删除签章
export function delSignature(id) {
  return lowCodeInstance.delete(Api.Prefix + '/' + id)
}
// 通过id获取签章下拉框列表
export function getListByIds(ids) {
  return lowCodeInstance.post(Api.Prefix + `/ListByIds`, { ids })
}
