import { businessInstance } from '@/utils/request'

//获取用户角色弹窗信息详情
export const getModalTableData = (obj) => {
  return businessInstance.get('/api/RoleUser/GetUserByroleIdAsync', {
    params: obj
  })
}

//添加用户到角色
export const addRoleUser = (selectRoleId, userIdList) => {
  return businessInstance.post(`/api/RoleUser/AddRoleUserAsync?roleid=${selectRoleId}`, userIdList)
}

//根据角色Id获取用户信息
export const getUserListByRoleId = (obj) => {
  return businessInstance.get('/api/RoleUser/GetUserByroleIdAsync', {
    params: obj
  })
}

//根据ids从角色中移除用户
export const delUserToRoleByIds = (selectRoleId, userIdList) => {
  return businessInstance.post(`/api/RoleUser/DeleteRoleUserAsync?roleid=${selectRoleId}`, userIdList)
}
