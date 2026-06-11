// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/printDev'
}

// 获取打印模板列表下拉框
export function getPrintDevSelector() {
  return lowCodeInstance.get(Api.Prefix + `/Selector`)
}
