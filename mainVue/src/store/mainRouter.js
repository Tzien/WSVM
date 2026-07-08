import { defineStore } from 'pinia'
import { useGetPermissionAsync } from '../api/permission'
import { h, ref } from 'vue'
import { useNavigationStore } from './navigation'
import { buildPermissionQuery } from '../utils/commonTools'

const buildChildren = (item) => {
  return item.map((subItem) => {
    if (subItem.menuDtos && subItem.menuDtos.length > 0) {
      return {
        name: subItem.functionCode,
        icon: () => h(subItem.icon),
        path: subItem.route,
        i18nKey: `message.route.XT${subItem.functionCode}`,
        children: buildChildren(subItem.menuDtos)
      }
    } else {
      // return {
      //   name: subItem.functionName,
      //   icon: () => h(subItem.icon),
      //   path: subItem.route,
      //   i18nKey: `message.route.XT${subItem.id}`,
      //   component: () => import(/* @vite-ignore */ `${subItem.routeFile}`)
      // }
      const modules = import.meta.glob('/src/views/**/**.vue')
      return {
        name: subItem.functionCode,
        icon: () => h(subItem.icon),
        path: subItem.route,
        i18nKey: `message.route.XT${subItem.functionCode}`,
        // component: modules[`/src/views${subItem.route}.vue`]
        component: modules[`${subItem.routeFile}`]
      }
    }
  })
}

export const useRouteStore = defineStore(
  'mainRouter',
  () => {
    var routes = ref([])
    const permissionSystemsCache = ref([])
    const permissionCacheMeta = ref({
      roleKey: '',
      language: ''
    })
    let loadingPermissionPromise = null
    let loadingPermissionKey = ''

    const normalizeRoleKey = (roles) => {
      if (!Array.isArray(roles)) return ''
      return [...roles].map((role) => `${role ?? ''}`).sort().join('|')
    }

    const buildRouteMap = (permissionSystems) => {
      return permissionSystems.map((item) => {
        return {
          name: item.subSysName,
          path: `/${item.sysCode}`,
          i18nKey: `message.route.XT${item.sysCode}`,
          icon: () => h(item.SubSysIcon),
          children: buildChildren(item.menuDtos)
        }
      })
    }

    const applyRouteData = (permissionSystems) => {
      var dynamicRoutes = []
      var routeMap = buildRouteMap(permissionSystems)
      routeMap.forEach((element) => {
        dynamicRoutes.push(element)
      })
      routes.value = dynamicRoutes
    }

    const getCachedPermissionSystems = (userRoles, explicitLanguage) => {
      const navigationStore = useNavigationStore()
      const currentLanguage = explicitLanguage ?? navigationStore?.language ?? ''
      const roleKey = normalizeRoleKey(userRoles)
      const isMatched = permissionCacheMeta.value.roleKey === roleKey && permissionCacheMeta.value.language === currentLanguage

      if (!isMatched || !Array.isArray(permissionSystemsCache.value) || permissionSystemsCache.value.length === 0) {
        return null
      }

      return permissionSystemsCache.value
    }

    const waitForPermissionSystems = async (userRoles, explicitLanguage) => {
      const navigationStore = useNavigationStore()
      const currentLanguage = explicitLanguage ?? navigationStore?.language ?? ''
      const roleKey = normalizeRoleKey(userRoles)
      const fetchKey = `${roleKey}@@${currentLanguage}`

      if (loadingPermissionPromise && loadingPermissionKey === fetchKey) {
        await loadingPermissionPromise
      }

      return getCachedPermissionSystems(userRoles, explicitLanguage)
    }

    const loadRoutes = async (userRoles, explicitLanguage) => {
      const navigationStore = useNavigationStore()
      const currentLanguage = explicitLanguage ?? navigationStore?.language ?? ''
      const roleKey = normalizeRoleKey(userRoles)
      const fetchKey = `${roleKey}@@${currentLanguage}`

      const cachedPermissionSystems = getCachedPermissionSystems(userRoles, currentLanguage)
      if (cachedPermissionSystems) {
        applyRouteData(cachedPermissionSystems)
        return cachedPermissionSystems
      }

      if (loadingPermissionPromise && loadingPermissionKey === fetchKey) {
        await loadingPermissionPromise
        const afterLoadCache = getCachedPermissionSystems(userRoles, currentLanguage)
        if (afterLoadCache) {
          applyRouteData(afterLoadCache)
          return afterLoadCache
        }
      }

      const obj = buildPermissionQuery({
        accurate: 1,
        roleids: userRoles,
        langCode: currentLanguage
      })

      loadingPermissionKey = fetchKey
      loadingPermissionPromise = (async () => {
        var routeData = await useGetPermissionAsync(obj)
        const permissionSystems = Array.isArray(routeData?.data) ? routeData.data : []
        permissionSystemsCache.value = permissionSystems
        permissionCacheMeta.value = {
          roleKey,
          language: currentLanguage
        }
        applyRouteData(permissionSystems)
      })()

      try {
        await loadingPermissionPromise
      } finally {
        if (loadingPermissionKey === fetchKey) {
          loadingPermissionPromise = null
          loadingPermissionKey = ''
        }
      }

      return permissionSystemsCache.value
    }
    const clearnRoutes = () => {
      routes.value = []
      permissionSystemsCache.value = []
      permissionCacheMeta.value = {
        roleKey: '',
        language: ''
      }
      loadingPermissionPromise = null
      loadingPermissionKey = ''
    }
    return { routes, loadRoutes, clearnRoutes, getCachedPermissionSystems, waitForPermissionSystems }
  },
  {
    persist: true
  }
)
