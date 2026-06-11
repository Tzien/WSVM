import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/DataSource',
  SyncPrefix = '/api/system/DataSync',
}

// 获取数据连接下拉框列表
export function getDataSourceSelector(type = '') {
  const data:any = type ? { type } : {};
  return lowCodeInstance.get(Api.Prefix + `/Selector`, data);
}