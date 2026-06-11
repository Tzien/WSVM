// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance, lowCodeFormInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/visualdev/Base',
  GenPrefix = '/api/visualdev/Generater',
  OnlinePrefix = '/api/visualdev/OnlineDev',
  LogPrefix = '/api/visualdev/OnlineLog',
  PersonalPrefix = '/api/visualdev/personal',
  AiPrefix = '/api/visualdev/ai/form',
  FormDb = '/api/FormDb'
}

// 获取功能下拉框列表
export function getVisualDevSelector(data) {
  return lowCodeInstance.get(Api.Prefix + `/Selector`, data)
}

// 获取列表表单配置JSON
export function getConfigData(modelId, data = {}) {
  return lowCodeInstance.get(Api.OnlinePrefix + `/${modelId}/Config`, data)
}

// 获取表单主表属性列表
export function getFormDataFields(id, filterType = 0) {
  return lowCodeInstance.get(Api.Prefix + `/${id}/FormDataFields?filterType=${filterType}`)
}
// 通过菜单id获取列表表单配置JSON
export function getConfigDataByMenuId(data = {}) {
  return lowCodeInstance.get(Api.OnlinePrefix + `/Config`, data)
}
// 获取功能
export function getInfo(id) {
  return lowCodeInstance.get(Api.Prefix + '/' + id)
}
// 新建功能
export function create(data) {
  return lowCodeFormInstance.post(Api.FormDb + '/InsertFormDbAsync', data)
}
// 修改功能
export function update(data) {
  return lowCodeFormInstance.put(Api.FormDb + '/UpdateFormDbAsync', data)
}

// 新建数据
export function createModel(modelId, data) {
  return lowCodeFormInstance.post(Api.OnlinePrefix + `/${modelId}`, data)
}
// 修改数据
export function updateModel(modelId, data) {
  return lowCodeFormInstance.put(Api.OnlinePrefix + `/${modelId}/${data.id}`, data)
}
// 获取数据信息
export function getModelInfo(modelId, id, menuId = '') {
  const obj: any = {
    menuId: menuId
  }
  return lowCodeFormInstance.get(Api.OnlinePrefix + `/${modelId}/${id}`, obj)
}
// 获取数据详情
export function getDataChange(modelId, data) {
  return lowCodeFormInstance.post( Api.OnlinePrefix + `/${modelId}/DataChange`, data);
}
// 自定义按钮发起流程
export function launchFlow(modelId, data) {
  return lowCodeFormInstance.post(Api.OnlinePrefix + `/${modelId}/actionLaunchFlow`, data );
}
// 获取数据列表
export function getModelList(data) {
  return lowCodeFormInstance.post(Api.OnlinePrefix + `/${data.modelId}/List`, data);
}
// 获取视图列表
export function getViewList(obj) {
  return lowCodeFormInstance.get(`/api/FormDb/GetViewList?menuId=${obj.menuId}&userId=${obj.userId}`);
}
// 导出数据
export function exportModel(modelId, data) {
  return lowCodeFormInstance.post(Api.OnlinePrefix + `/${modelId}/Actions/ExportData`, data);
}
// 批量删除数据
export function batchDelete(modelId, data) {
  return lowCodeFormInstance.post(Api.OnlinePrefix + `/batchDelete/${modelId}`, data );
}
// 新建视图
export function createView(data) {
   return lowCodeFormInstance.post(`/api/FormDb/CreateView`,data);
}
// 修改视图
export function updateView(data) {
  return lowCodeFormInstance.post(`/api/FormDb/UpdateView?id=${data.id}`,data);
}
// 删除视图
export function delView(id, menuId) {
  return lowCodeFormInstance.delete(`/api/FormDb/DelView?id=${id}&menuId=${menuId}`);

}
// 设置默认视图
export function setDefaultView(id, menuId,userId) {
  return lowCodeFormInstance.post(`/api/FormDb/setDefault?id=${id}&menuId=${menuId}&userId=${userId}`);
}


// 代码下载
export function downloadCode(id, data) {
  return lowCodeFormInstance.post(`/api/FormDb/DownloadCode/${id}`, data);
}