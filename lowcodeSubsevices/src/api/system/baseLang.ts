// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/BaseLang'
}

// 获取语言列表
export function getBaseLangList(data) {
  return lowCodeInstance.get(Api.Prefix, data)
}

// 获取语言内容
export function getLangJson() {
  return lowCodeInstance.get(Api.Prefix + '/LangJson')
}

// 新建语言
export function create(data) {
  return lowCodeInstance.get(Api.Prefix, data)
  // return defHttp.post({ url: Api.Prefix, data });
}