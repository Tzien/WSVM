// // import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'


enum Api {
  Prefix = '/api/workflow/task',
  operatorPrefix = '/api/workflow/operator',
}

// // 获取流程发起列表
// export function getFlowLaunchList(data) {
//   return lowCodeInstance.get({ url: Api.Prefix, data });
// }
// // 获取待我审批详情
// export function getFlowTaskInfo(id, data) {
//   return lowCodeInstance.get({ url: Api.Prefix + `/${id}`, data });
// }
// 新建表单
export function create(data) {
  return lowCodeInstance.post(Api.Prefix, data);
}
// 更新表单
export function update(data) {
  return lowCodeInstance.put(Api.Prefix + `/${data.id}`, data);
}
// // 删除流程发起
// export function delFlowLaunch(id) {
//   return lowCodeInstance.delete({ url: Api.Prefix + '/' + id });
// }
// // 流程发起撤回
// export function launchRecall(id, data) {
//   return lowCodeInstance.put({ url: Api.Prefix + `/Recall/${id}`, data });
// }
// // 流程发起撤销
// export function revoke(id, data) {
//   return lowCodeInstance.put({ url: Api.Prefix + `/Revoke/${id}`, data });
// }
// // 流程发起催办
// export function press(id) {
//   return lowCodeInstance.post({ url: Api.Prefix + `/Press/${id}` });
// }
// //事件日志
// export function getLogList(data) {
//   return lowCodeInstance.get({ url: Api.Prefix + `/${data.id}/EventLog` });
// }
// // 获取流程实例相关人员（分页）
// export function getTaskUserList(taskId, data) {
//   return lowCodeInstance.get({ url: Api.Prefix + `/TaskUserList/${taskId}`, data });
// }
// // 查看子流程
// export function getSubFlowInfo(nodeCode, flowId) {
//   return lowCodeInstance.get({ url: Api.Prefix + `/SubFlowInfo?taskId=${flowId}&nodeCode=${nodeCode}` });
// }
// // 经办
// // 获取候选人列表（分页）
// export function getCandidateUser(data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/CandidateUser/${data.operatorId || 0}`, data });
// }
// 判断是否有候选人
export function getCandidates(id, data) {
  return lowCodeInstance.post(Api.operatorPrefix + `/CandidateNode/${id}`, data );
}
// // 获取待我审核
// export function getFlowBeforeList(data) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/List/${data.category}`, data });
// }
// // 待签收批量签收
// export function batchSign(data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Sign`, data });
// }
// // 待签收签收 type 0 - 签收  1 - 退签
// export function sign(id) {
//   return batchSign({ ids: [id], type: 0 });
// }
// // 待签收退签
// export function cancelSign(id) {
//   return batchSign({ ids: [id], type: 1 });
// }
// // 待办理批量办理
// export function batchHandle(data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Transact`, data });
// }
// // 待办理开始办理
// export function handle(id) {
//   return batchHandle({ ids: [id] });
// }
// // 待我审核审核(同意、拒绝)
// export function audit(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Audit/${id}`, data });
// }
// // 获取加签人员列表
// export function getFreeApproverUserList(data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/AddSignUserIdList/${data.id}`, data });
// }
// // 减签
// export function reduceApprover(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/ReduceApprover/${id}`, data });
// }
// // 暂存
// export function saveAudit(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/SaveAudit/${id}`, data });
// }
// // 协办保存
// export function saveAssist(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/AssistSave/${id}`, data });
// }
// // 撤回审核
// export function auditRecall(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Recall/${id}`, data });
// }
// // 待我审核转审
// export function transfer(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Transfer/${id}`, data });
// }
// // 待我审核转审
// export function assist(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/Assist/${id}`, data });
// }
// // 批量通过、退回、转审  batchType 0-通过 1-退回 2-转审
// export function batchOperation(data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/BatchOperation`, data });
// }
// // 获取批量审批候选人
// export function getBatchCandidate(data) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/BatchCandidate`, data });
// }
// // 获取批量审批流程
// export function getBatchFlowSelector() {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/BatchFlowSelector` });
// }
// // 获取批量审批流程版本
// export function getBatchVersionSelector(id) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/BatchVersionSelector/${id}` });
// }
// // 获取批量审批流程节点
// export function getBatchNodeSelector(id) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/BatchNodeSelector/${id}` });
// }
// // 获取批量审批流程节点属性
// export function getBatchNode(flowId, nodeCode) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/BatchNode?flowId=${flowId}&nodeCode=${nodeCode}` });
// }
// // 判断是否有查看权限(消息通知用)
// export function checkInfo(taskOperatorId, opType) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/${taskOperatorId}/Info?opType=${opType}` });
// }
// // 退回审核
// export function back(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/SendBack/${id}`, data });
// }
// // 获取退回下拉接口
// export function getBackNodeCodeList(id) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/SendBackNodeList/${id}` });
// }
// // 加签
// export function freeApprover(id, data) {
//   return lowCodeInstance.post({ url: Api.operatorPrefix + `/AddSign/${id}`, data });
// }
// // 进度节点经办列表
// export function getRecordList(taskId, nodeId) {
//   return lowCodeInstance.get({ url: Api.operatorPrefix + `/RecordList?taskId=${taskId}&nodeId=${nodeId}` });
// }
// // 查看发起节点表单
// export function getStartFormInfo(taskId) {
//   return lowCodeInstance.get({ url: Api.Prefix + `/ViewStartForm/${taskId}` });
// }
