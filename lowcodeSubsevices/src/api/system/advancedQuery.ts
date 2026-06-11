// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/AdvancedQuery',
}

// 获取方案列表
export function getAdvancedQueryList(moduleId) {
  return lowCodeInstance.get( Api.Prefix + `/${moduleId}/List` );
}
// 新建方案
export function create(data) {
  return lowCodeInstance.post(Api.Prefix, data);
}
// 修改方案
export function update(data) {
  return lowCodeInstance.put( Api.Prefix + '/' + data.id, data );
}
// 获取方案
export function getInfo(id) {
  return lowCodeInstance.get(Api.Prefix + '/' + id);
}
// 删除方案
export function delAdvancedQuery(id) {
  return lowCodeInstance.delete(Api.Prefix + '/' + id);
}
