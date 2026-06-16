import { idsInstance, businessInstance } from '../utils/request'

/* 系统下拉 */
export const getSysinfoList = () => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfoTree')
}

/* 获取人员下拉 */
export const getUserInfo = () => {
  return businessInstance.get('/api/User/GetUserDropDownAsync')
}

/* 获取角色下拉 */
export const getRoleInfo = (roleName) => {
  return idsInstance.get('/api/Role/GetRolesAsync', {
    params: {
      roleName
    }
  })
}

/* 获取组织树(纯组织) */
export const getSysOrgInfo = (id, orgName) => {
  return businessInstance.get(`/api/SysOraganize/GetOraganizeTreeAsync`, { params: { id, orgName } })
}

/* 获取组织树(包括用户) */
export const getSysOrgUserInfo = (username) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeUserTreeAsync', {
    params: { username }
  })
}

//获取页面按钮列表
export const getButtonList = (obj) => {
  return businessInstance.post(`/api/SysInfo/GetMenuByRouteAsync`, obj)
}

export const getCommonI18nValueList = (langCode) => {
  const params = {}
  if (langCode !== undefined && langCode !== null && langCode !== '') {
    params.langCode = langCode
  }

  return businessInstance.get('/api/I18n/GetCommonI18nValueList', {
    params
  })
}

export const getPageI18nValueList = (langCode, pageCode) => {
  const params = {}
  if (langCode !== undefined && langCode !== null && langCode !== '') {
    params.langCode = langCode
  }
  if (pageCode !== undefined && pageCode !== null && pageCode !== '') {
    params.pageCode = pageCode
  }

  return businessInstance.get('/api/I18n/GetPageI18nValueList', {
    params
  })
}

export const getLanguageList = () => {
  return businessInstance.get('/api/I18n/GetLanguageList')
}

export const setDefaultLanguage = (langCode) => {
  return businessInstance.post('/api/I18n/SetDefaultLanguage', null, {
    params: {
      langCode
    }
  })
}
