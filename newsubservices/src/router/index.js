import { createRouter, createWebHistory } from 'vue-router'
import pinia, { useUserStore, useDrawerStore, useNavigationStore, useRouteStore } from '../store/index'
import { getGlobalState } from '../shared/globalStateManager'
import { h } from 'vue'
import { AppleFilled } from '@ant-design/icons-vue'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'

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
const isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__

const router = createRouter({
  history: createWebHistory(isQiankun ? `/${import.meta.env.VITE_APP_APPNAME}` : '/'),
  routes
})

let lastLoadSignature = ''

router.beforeEach(async (to, from, next) => {
  const routeStore = useRouteStore(pinia)

  let userStore
  let drawerStore
  let navigationStore

  if (isQiankun) {
    // qiankun 场景：从主应用注入的全局状态中获取 store 状态
    const globalState = getGlobalState()
    if (!globalState || !globalState.userStore) {
      // 主应用全局状态尚未就绪，直接放行，避免访问 undefined
      next()
      return
    }
    userStore = globalState.userStore
    drawerStore = globalState.drawerStore || globalState.useDrawerStore
    navigationStore = globalState.navigationStore
  } else {
    // 独立运行 Ids 子应用：使用本地 Pinia store
    userStore = useUserStore(pinia)
    drawerStore = useDrawerStore(pinia)
    navigationStore = useNavigationStore(pinia)
  }

  // 独立运行：无 token 或 token 过期时，统一跳回 Login
  if (!isQiankun && to.path !== '/Login') {
    const current = Math.floor(Date.now() / 1000)
    const expiration = userStore.expiration_timestamp || 0
    const hasToken = !!userStore.access_token
    if (!hasToken || current >= expiration) {
      userStore.removeUserMessage()
      next('/Login')
      return
    }
  }

  // 统一从 userStore.userRoles 派生出真正的角色数组（兼容 Pinia store 和 plain state）
  const rolesSource = userStore && userStore.userRoles
  const roles = Array.isArray(rolesSource) ? rolesSource : rolesSource?.value

  // 如果此时还拿不到有效角色信息：
  // - qiankun 场景：说明全局状态尚未完全就绪，先放行，避免向后端发送 roleids: undefined 导致 400
  // - 独立场景：说明还未登录，同样直接放行，让上面的未登录判断接管后续导航
  if (!roles || (Array.isArray(roles) && roles.length === 0)) {
    next()
    return
  }

  const currentLanguage = navigationStore?.language || 'zh'
  const roleKey = Array.isArray(roles) ? roles.join(',') : ''
  const nextSignature = `${roleKey}__${currentLanguage}`

  // 检查是否需要重新加载路由
  const needReload = !routeStore.loaded || routeStore.routes.length === 0 || lastLoadSignature !== nextSignature

  if (needReload) {
    await routeStore.loadRoutes(roles, currentLanguage)
    lastLoadSignature = nextSignature

    // 添加动态路由到 router
    routeStore.routes.forEach((route) => {
      addRoutesRecursively(route)
    })
  }

  if (to.path !== '/Login') {
    const sysCode = import.meta.env.VITE_APP_APPNAME
    const innerPath = `${import.meta.env.VITE_HOMEPAGEPATH}`
    const tabKey = sysCode ? `/${sysCode}${innerPath}` : innerPath

    if (to.path === '/') {
      next(innerPath)
      if (drawerStore) {
        if (typeof drawerStore.setFoldmenu === 'function') {
          drawerStore.setFoldmenu([
            `${import.meta.env.VITE_HOMEPAGEPATHFOLDMENU}`,
            innerPath
          ])
        } else if (Array.isArray(drawerStore.foldmenu)) {
          drawerStore.foldmenu = [
            `${import.meta.env.VITE_HOMEPAGEPATHFOLDMENU}`,
            innerPath
          ]
        }

        if (typeof drawerStore.changeSelected === 'function') {
          drawerStore.changeSelected([innerPath])
        } else if (Array.isArray(drawerStore.selected)) {
          drawerStore.selected = [innerPath]
        }
      }

      const newTab = {
        key: tabKey,
        title: `${import.meta.env.VITE_HOMEPAGETABS_title}`,
        path: innerPath,
        sysCode,
        i18nKey: `${import.meta.env.VITE_HOMEPAGETABS_i18nKey}`
      }

      if (typeof navigationStore.addTabs === 'function') {
        // 独立运行 ids4 子应用：直接使用自身的 navigationStore 方法
        navigationStore.addTabs(newTab)
      } else if (typeof window !== 'undefined') {
        // qiankun 场景：通过事件通知主应用添加 tab，由主应用负责去重
        window.dispatchEvent(
          new CustomEvent('sub-app-add-tab', {
            detail: { tab: newTab }
          })
        )
      }
      return
    }

    if (to.path === innerPath) {
      if (drawerStore) {
        if (typeof drawerStore.setFoldmenu === 'function') {
          drawerStore.setFoldmenu([
            `${import.meta.env.VITE_HOMEPAGEPATHFOLDMENU}`,
            innerPath
          ])
        } else if (Array.isArray(drawerStore.foldmenu)) {
          drawerStore.foldmenu = [
            `${import.meta.env.VITE_HOMEPAGEPATHFOLDMENU}`,
            innerPath
          ]
        }

        if (typeof drawerStore.changeSelected === 'function') {
          drawerStore.changeSelected([innerPath])
        } else if (Array.isArray(drawerStore.selected)) {
          drawerStore.selected = [innerPath]
        }
      }

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

    // 判断展开节点
    if (to.matched.length > 1) {
      const foldmenuStr = []
      for (let index = 1; index < to.matched.length; index++) {
        const element = to.matched[index]
        foldmenuStr.push(element.path)
      }
      if (drawerStore) {
        const currentFoldmenu = Array.isArray(drawerStore.foldmenu) ? drawerStore.foldmenu : []
        const mergedFoldmenu = [...new Set([...currentFoldmenu, ...foldmenuStr])]
        if (typeof drawerStore.setFoldmenu === 'function') {
          drawerStore.setFoldmenu(mergedFoldmenu)
        } else if (Array.isArray(drawerStore.foldmenu)) {
          drawerStore.foldmenu = mergedFoldmenu
        }
      }
    }

    next()
  } else {
    next()
  }
})
export default router
