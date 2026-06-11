import { lowCodeInstance } from '@/utils/request'

enum Api {
  Prefix = '/api/system/Menu'
}

// 获取菜单表单
export function getMenuSelectorFormTree() {
  return lowCodeInstance.get(Api.Prefix + `/getSystemMenu`)
}
