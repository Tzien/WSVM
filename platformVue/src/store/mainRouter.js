import { defineStore } from 'pinia'
import { useGetPermissionAsync } from '../api/permission'

// 预加载所有视图组件，供 routeFile 匹配使用
const modules = import.meta.glob('/src/views/**/*.vue')

// 记录已使用的路由名称，避免 name 冲突
const usedRouteNames = new Set()

const generateUniqueRouteName = (rawName, level = 0) => {
  const base = rawName || 'unnamed'
  let uniqueName = level > 0 ? `${base}_level${level}` : base
  let counter = 1

  while (usedRouteNames.has(uniqueName)) {
    uniqueName = `${base}_${counter++}`
  }

  usedRouteNames.add(uniqueName)
  return uniqueName
}

const normalizeLocale = (locale) => `${locale || ''}`.trim()

const buildRoleKey = (roles = []) => (Array.isArray(roles) ? roles : []).map((role) => `${role}`).join(',')

const resolveMenuList = (item) => {
  if (Array.isArray(item?.menuDtos)) return item.menuDtos
  if (Array.isArray(item?.MenuDtos)) return item.MenuDtos
  return []
}

const resolveMenuName = (subItem) => {
  return (
    subItem?.functionName ||
    subItem?.FunctionName ||
    subItem?.menuName ||
    subItem?.MenuName ||
    subItem?.name ||
    subItem?.title ||
    subItem?.functionCode ||
    subItem?.FunctionCode ||
    '未命名'
  )
}

const resolveSystemName = (item) => {
  return (
    item?.subSysName ||
    item?.SubSysName ||
    item?.sysCode ||
    item?.SysCode ||
    '未命名'
  )
}

const buildChildren = (items, level = 0) => {
  if (!Array.isArray(items) || items.length === 0) {
    return []
  }

  return items
    .map((subItem) => {
      if (!subItem) return null

      const menuName = resolveMenuName(subItem)
      const uniqueName = generateUniqueRouteName(menuName, level)
      const functionCode = subItem.functionCode || subItem.FunctionCode || 'unknown'

      // 有子菜单：继续递归构建 children
      const childMenus = resolveMenuList(subItem)
      if (childMenus.length > 0) {
        return {
          id: subItem.id || null,
          isSysInfo: false,
          name: uniqueName,
          originalName: menuName,
          icon: subItem.icon || null,
          path: subItem.route || '/',
          i18nKey: `message.route.XT${functionCode}`,
          children: buildChildren(childMenus, level + 1)
        }
      }

      // 叶子节点：根据 routeFile 绑定组件
      let componentPath = subItem.routeFile
      if (!componentPath) {
        componentPath = '/src/views/NotFond.vue'
      } else if (!componentPath.startsWith('/src/views/')) {
        if (componentPath.startsWith('/views/')) {
          componentPath = '/src' + componentPath
        } else if (componentPath.startsWith('views/')) {
          componentPath = '/src/' + componentPath
        } else if (componentPath.startsWith('/')) {
          componentPath = '/src/views' + componentPath
        } else {
          componentPath = '/src/views/' + componentPath
        }
      }

      return {
        id: subItem.id || null,
        isSysInfo: false,
        name: uniqueName,
        originalName: menuName,
        icon: subItem.icon || null,
        path: subItem.route || '/',
        i18nKey: `message.route.XT${functionCode}`,
        component:
          modules[componentPath] ||
          (() => {
            return import('../views/NotFond.vue')
          }),
        meta: {
          IsKeepAlive: subItem.isCache || false,
          KeepAliveName: functionCode
        }
      }
    })
    .filter(Boolean)
}

export const useRouteStore = defineStore({
  id: 'mainRouter',
  state: () => ({
    routes: [],
    loaded: false,
    userRolesKey: '',
    loadedLanguage: '',
    permissionSystemsCache: [],
    permissionSystemsByKey: {},
    loadingPermissionPromises: {}
  }),
  actions: {
    async loadRoutes(userRoles, language = '') {
      const roles = Array.isArray(userRoles) ? userRoles : []
      const roleKey = buildRoleKey(roles)
      const languageKey = normalizeLocale(language)
      const fetchKey = `${roleKey}@@${languageKey}`

      const cachedPermissionSystems = this.permissionSystemsByKey[fetchKey]
      if (Array.isArray(cachedPermissionSystems)) {
        usedRouteNames.clear()
        this.userRolesKey = roleKey
        this.loadedLanguage = languageKey
        this.permissionSystemsCache = cachedPermissionSystems
        this.routes = cachedPermissionSystems.map((item) => {
          const systemName = resolveSystemName(item)
          const uniqueName = generateUniqueRouteName(systemName, 0)
          const sysCode = item.sysCode || item.SysCode

          return {
            id: null,
            isSysInfo: true,
            name: uniqueName,
            originalName: systemName,
            path: `/${sysCode}`,
            i18nKey: `message.route.XT${sysCode}`,
            icon: item.SubSysIcon || null,
            children: buildChildren(resolveMenuList(item), 1)
          }
        })
        this.loaded = true
        return this.permissionSystemsCache
      }

      if (this.loadingPermissionPromises[fetchKey]) {
        await this.loadingPermissionPromises[fetchKey]
        return this.loadRoutes(roles, languageKey)
      }

      if (this.loaded && this.userRolesKey === roleKey && this.loadedLanguage === languageKey && this.routes.length > 0) {
        return this.permissionSystemsCache
      }

      usedRouteNames.clear()
      this.userRolesKey = roleKey
      this.loadedLanguage = languageKey

      const obj = {
        accurate: 1,
        roleids: roles
      }

      if (languageKey) {
        obj.langCode = languageKey
      }

      this.loadingPermissionPromises[fetchKey] = (async () => {
        const routeData = await useGetPermissionAsync(obj)
        const permissionSystems = Array.isArray(routeData?.data) ? routeData.data : []
        this.permissionSystemsByKey[fetchKey] = permissionSystems
        this.permissionSystemsCache = permissionSystems

        this.routes = permissionSystems.map((item) => {
          const systemName = resolveSystemName(item)
          const uniqueName = generateUniqueRouteName(systemName, 0)
          const sysCode = item.sysCode || item.SysCode

          return {
            id: null,
            isSysInfo: true,
            name: uniqueName,
            originalName: systemName,
            path: `/${sysCode}`,
            i18nKey: `message.route.XT${sysCode}`,
            icon: item.SubSysIcon || null,
            children: buildChildren(resolveMenuList(item), 1)
          }
        })

        this.loaded = true
      })()

      try {
        await this.loadingPermissionPromises[fetchKey]
      } finally {
        delete this.loadingPermissionPromises[fetchKey]
      }

      return this.permissionSystemsCache
    },
    clearnRoutes() {
      this.routes = []
      this.loaded = false
      this.userRolesKey = ''
      this.loadedLanguage = ''
      this.permissionSystemsCache = []
      this.permissionSystemsByKey = {}
      this.loadingPermissionPromises = {}
    }
  }
})
