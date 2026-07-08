import { idsInstance, businessInstance } from '../utils/request'

export const useGetRolesAsync = (rolename) => {
  return idsInstance.get('/api/Role/GetRolesAsync', {
    params: { rolename }
  })
}

export const useGetPermissionAsync = (obj) => {
  return businessInstance.post('/api/SysInfo/GetPermissionAsync', obj)
}
//根据角色ID查询系统ID
export const userGetSysInfoIdByRoleIdAsync = (roleId) => {
  return businessInstance.get('/api/RolePermission/GetSysInfoIdByRoleIdAsync', {
    params: { roleId }
  })
}

export const GetPermissionBySysInfoIDAsync = (roleId, sysInfoID, showinterface) => {
  return businessInstance.get('/api/SysInfo/GetPermissionBySysInfoIDAsync', {
    params: { roleId, sysInfoID, showinterface }
  })
}

export const useEditRolePermissionAsync = (obj) => {
  return businessInstance.post('/api/RolePermission/EditRolePermissionAsync', obj)
}
