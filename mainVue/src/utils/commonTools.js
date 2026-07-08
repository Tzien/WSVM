//公共函数

export const buildPermissionQuery = ({ accurate = 0, roleids = [], langCode = '' } = {}) => {
  const normalizedRoleIds = Array.isArray(roleids) ? roleids : roleids ? [roleids] : []
  return {
    accurate,
    roleids: normalizedRoleIds,
    langCode: langCode ?? ''
  }
}