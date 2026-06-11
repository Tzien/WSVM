// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/DataInterface'
}

// 获取接口数据
export function getDataInterfaceRes(id, data = {}) {
  return lowCodeInstance.post(Api.Prefix + `/${id}/Actions/Preview`, data)
}
// 获取接口
export function getDataInterfaceInfo(id) {
  return lowCodeInstance.get(Api.Prefix + '/' + id)
}

// 获取接口列表(工作流选择时调用，带分页)
export function getDataInterfaceSelectorList(data) {
  return lowCodeInstance.get(Api.Prefix + `/getList`, data)
}
