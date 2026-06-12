import { lowCodeDemoInstance } from '@/utils/request'
enum Api {
  Prefix = '/api/AllpropsService',
}

// 获取列表
export function getList(data) {
  return lowCodeDemoInstance.post( Api.Prefix + `/List`, data );
}
// 获取
export function getInfo(id) {
  return lowCodeDemoInstance.get( Api.Prefix + `/` + id );
}

// 新建
export function create(data) {
  return lowCodeDemoInstance.post( Api.Prefix, data );
}

// 修改
export function update(data) {
  return lowCodeDemoInstance.put( Api.Prefix + `/` + data.id, data );
}

// 删除
export function del(id) {
  return lowCodeDemoInstance.delete( Api.Prefix + `/` + id );
}
// 批量删除数据
export function batchDelete(ids) {
  return lowCodeDemoInstance.post( Api.Prefix + `/batchRemove`, ids );
}
