// import { defHttp } from '@/utils/http/axios';
// import { omit } from 'lodash-es';
import { lowCodeInstance } from '@/utils/request'
import { omit } from 'lodash-es'

enum Api {
  Prefix = '/api/system/DataModel'
}

// 获取数据库表
export function getDataModelFieldList(linkId, table, type = '0') {
  return lowCodeInstance.get(Api.Prefix + `/${linkId}/Tables/${table}/Fields?type=${type}`)
}

// 获取数据库表
export function getDataModelInfo(linkId, table) {
  return lowCodeInstance.get(Api.Prefix + `/${linkId}/Table/${table}`)
}

//  新增字段
export function addTableFields(linkId, data) {
  return lowCodeInstance.put(Api.Prefix + `/${linkId}/addFields`, data)
}

// // 获取数据库表列表
// export function getDataModelList(data) {
//   debugger
//   return lowCodeInstance.get(Api.Prefix + `/${data.linkId}/Tables`, omit(data, ['linkId']))
// }
