import { createRouter, createWebHistory } from 'vue-router'
import { useRouteStore } from '../store/index'
import { getGlobalState } from '../shared/globalStateManager'
import { reactive } from 'vue'

// 递归添加路由的函数
function addRoutesRecursively(route) {
  // 直接添加当前路由到 Vue Router
  router.addRoute(route)
  // 如果存在子路由，递归添加每个子路由
  if (route.children && route.children.length > 0) {
    route.children.forEach((childRoute) => {
      // 递归调用，继续处理子路由
      addRoutesRecursively({
        ...childRoute,
        path: childRoute.path
      })
    })
  }
}
const routes = [
  {
    name: '登录',
    path: '/Login',
    icon: () => h(AppleFilled),
    component: () => import(/* webpackChunkName: "login" */ '../views/Login.vue')
  },
  {
    path: '/:catchAll(.*)',
    name: 'not-found',
    component: () => import(/* webpackChunkName: "not-found" */ '../views/NotFond.vue')
  }
]
function findRootPath(routes, targetPath) {
  function recursiveSearch(routeList, currentRootPath) {
    for (const route of routeList) {
      if (route.path === targetPath) {
        return currentRootPath || route.path // 找到目标路径，返回当前根节点路径
      }
      if (route.children && route.children.length > 0) {
        // 继续在子节点中搜索
        const result = recursiveSearch(route.children, currentRootPath || route.path)
        if (result) {
          return result // 如果在子节点中找到了，返回结果
        }
      }
    }
    return null // 未找到目标路径
  }

  // 从根节点开始搜索
  return recursiveSearch(routes, '') || '/'
}

function getHomepageFoldmenu() {
  const configuredFoldmenu = `${import.meta.env.VITE_HOMEPAGEPATHFOLDMENU || ''}`.trim()
  return configuredFoldmenu ? [configuredFoldmenu] : []
}

function setDrawerFoldmenu(drawerStore, foldmenu) {
  if (!drawerStore) return
  if (typeof drawerStore.setFoldmenu === 'function') {
    drawerStore.setFoldmenu(foldmenu)
  } else {
    drawerStore.foldmenu = foldmenu
  }
}
const router = createRouter({
  history: createWebHistory(`/${import.meta.env.VITE_APP_APPNAME}`),
  routes
})

let lastUserRoles = null
let lastLanguage = ''

router.beforeEach(async (to, from, next) => {
  let userStore = reactive({})
  let drawerStore = reactive({})
  let navigationStore = reactive({})
  const routeStore = useRouteStore()

  // 获取全局状态
  const globalState = getGlobalState()
  if (globalState) {
    userStore = globalState.userStore
    drawerStore = globalState.drawerStore
    navigationStore = globalState.navigationStore
  }

  // 统一从 userStore.userRoles 派生出真正的角色数组
  const rolesSource = userStore && userStore.userRoles
  const roles = Array.isArray(rolesSource) ? rolesSource : rolesSource?.value
  const currentLanguage = `${navigationStore?.language || ''}`.trim()

  // 检查是否需要重新加载路由
  const needReload = !routeStore.loaded || lastUserRoles !== roles || lastLanguage !== currentLanguage || routeStore.routes.length === 0

  if (needReload && roles && roles.length) {
    await routeStore.loadRoutes(roles, currentLanguage)
    lastUserRoles = roles
    lastLanguage = currentLanguage

    // 添加动态路由到 router
    routeStore.routes.forEach((route) => {
      addRoutesRecursively(route)
    })
  }

  if (to.path !== '/Login') {
    const sysCode = import.meta.env.VITE_APP_APPNAME
    const innerPath = `${import.meta.env.VITE_HOMEPAGEPATH}`
    const tabKey = sysCode ? `/${sysCode}${innerPath}` : innerPath
    const homepageFoldmenu = getHomepageFoldmenu()

    if (to.path === '/') {
      next(innerPath)
      if (drawerStore) {
        setDrawerFoldmenu(drawerStore, homepageFoldmenu)
        if (typeof drawerStore.changeSelected === 'function') {
          drawerStore.changeSelected([innerPath])
        } else if (Array.isArray(drawerStore.selected)) {
          drawerStore.selected = [innerPath]
        }
      }
      if (navigationStore) {
        const newTab = {
          key: tabKey,
          title: `${import.meta.env.VITE_HOMEPAGETABS_title}`,
          path: innerPath,
          sysCode,
          i18nKey: `${import.meta.env.VITE_HOMEPAGETABS_i18nKey}`
        }
        if (typeof navigationStore.addTabs === 'function') {
          // 子应用本地运行场景：直接使用自身的 navigationStore 方法
          navigationStore.addTabs(newTab)
        } else if (typeof window !== 'undefined') {
          // qiankun 场景：通过事件通知主应用添加 tab，由主应用负责去重
          window.dispatchEvent(
            new CustomEvent('sub-app-add-tab', {
              detail: { tab: newTab }
            })
          )
        }
      }
      return
    }

    if (to.path === innerPath) {
      if (drawerStore) {
        setDrawerFoldmenu(drawerStore, homepageFoldmenu)
        if (typeof drawerStore.changeSelected === 'function') {
          drawerStore.changeSelected([innerPath])
        } else if (Array.isArray(drawerStore.selected)) {
          drawerStore.selected = [innerPath]
        }
      }
      if (navigationStore) {
        const newTab = {
          key: tabKey,
          title: `${import.meta.env.VITE_HOMEPAGETABS_title}`,
          path: innerPath,
          sysCode,
          i18nKey: `${import.meta.env.VITE_HOMEPAGETABS_i18nKey}`
        }
        if (typeof navigationStore.addTabs === 'function') {
          navigationStore.addTabs(newTab)
        } else if (typeof window !== 'undefined') {
          window.dispatchEvent(
            new CustomEvent('sub-app-add-tab', {
              detail: { tab: newTab }
            })
          )
        }
      }
      next()
      return
    }

    const rootPath = findRootPath(routeStore.routes, to.path)

    if (drawerStore) {
      if (typeof drawerStore.AddSelectFirstMenuPath === 'function') {
        drawerStore.AddSelectFirstMenuPath([rootPath])
      } else if (Array.isArray(drawerStore.selectFirstMenuPath)) {
        drawerStore.selectFirstMenuPath = [rootPath]
      }

      if (typeof drawerStore.changeSelected === 'function') {
        drawerStore.changeSelected([to.path])
      } else if (Array.isArray(drawerStore.selected)) {
        drawerStore.selected = [to.path]
      }
    }

    if (to.matched.length > 1) {
      const foldmenuStr = []
      for (let index = 1; index < to.matched.length; index++) {
        const element = to.matched[index]
        foldmenuStr.push(element.path)
      }
      if (drawerStore) {
        if (typeof drawerStore.setFoldmenu === 'function') {
          drawerStore.setFoldmenu(foldmenuStr)
        } else if (Array.isArray(drawerStore.foldmenu)) {
          drawerStore.foldmenu = foldmenuStr
        }
      }
    }

    next()
  } else {
    next()
  }
})
export default router
