// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/BillRule',
}
// 获取单据规则下拉框列表
export function getBillRuleSelector(data) {
  return lowCodeInstance.get( Api.Prefix + `/Selector`, data);
}