// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/workflow/template',
  CommentPrefix = '/api/workflow/comment',
  WebhookPrefix = '/api/workflow/Hooks'
}

// 获取流程发起节点表单信息(小流程id)
export function getFlowFormInfo(id) {
  return lowCodeInstance.get(Api.Prefix + `/${id}/FormInfo`)
}
// 子流程带权限的可选用户
export function getSubFlowUserList(id, data) {
  return lowCodeInstance.get(Api.Prefix + `/${id}/SubFlowUserList`, data)
}
// 列表流程列表(分页)
export function getFlowSelector(data) {
  return lowCodeInstance.get(Api.Prefix + `/Selector`, data)
}
