import { createApp } from 'vue'
/* 引入全局css配置文件 */
import commonCssText from '@/assets/css/common.css?inline'
import App from './App.vue'
import 'ant-design-vue/dist/reset.css'
import '@/design/index.less'
// Register icon sprite
import 'virtual:svg-icons-register'
import router from './router'
import pinia from './store/index'
import { setupI18n, loadLocaleMessages } from './lang/i18n.js'
import { initAxiosInstances } from './utils/request.js'
import actions from './shared/actions'
import { renderWithQiankun, qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore, useDrawerStore, useRouteStore, useNavigationStore } from './store/index'
import { initGlobalStateListener, clearGlobalStateListener, addGlobalStateListener, getGlobalState } from './shared/globalStateManager'

import { registerGlobComp } from '@/components/registerGlobComp'
import { setSubAppContainer } from '@/utils'

// 引入SurelyVue
import '@surely-vue/table/dist/index.less'
import STable from '@surely-vue/table'
import { setLicenseKey } from '@surely-vue/table'
setLicenseKey(import.meta.env.VITE_STABLESECRIT)

let instance = null
let userStore = {}
let navigationStore = {}
let drawerStore = {}
const normalizeLocale = (locale) => `${locale || ''}`.trim()

function ensureIds4CommonCssInjected() {
  var subappName = import.meta.env.VITE_APP_APPNAME
  const styleId = `${subappName}-common-css`
  if (document.getElementById(styleId)) return
  const styleEl = document.createElement('style')
  styleEl.id = styleId
  styleEl.setAttribute('data-app', `${subappName}-subApp`)
  styleEl.textContent = commonCssText
  document.head.appendChild(styleEl)
}
ensureIds4CommonCssInjected()

async function waitForGlobalStateReady(maxWaitMs = 3000) {
  const start = Date.now()
  return new Promise((resolve) => {
    const check = () => {
      const globalState = getGlobalState()
      if (globalState && globalState.userStore && globalState.userStore.access_token) {
        resolve(globalState)
      } else if (Date.now() - start >= maxWaitMs) {
        resolve(globalState)
      } else {
        setTimeout(check, 50)
      }
    }
    check()
  })
}

async function render(props = {}) {
  const { container } = props
  if (qiankunWindow.__POWERED_BY_QIANKUN__ && container) {
    // 弹窗等 teleport 组件需要挂到带 data-qiankun 作用域标记的节点内，样式才能命中。
    // 不能直接挂进主应用布局容器（会受其 overflow/transform 影响），
    // 因此在 body 下创建一个带同名 data-qiankun 标记的独立宿主节点
    const scopedEl = container.closest?.('[data-qiankun]')
    const appName = scopedEl?.getAttribute('data-qiankun') || import.meta.env.VITE_APP_APPNAME
    const hostId = `${appName}-popup-host`
    let host = document.getElementById(hostId)
    if (!host) {
      host = document.createElement('div')
      host.id = hostId
      host.setAttribute('data-qiankun', appName)
      document.body.appendChild(host)
    }
    setSubAppContainer(host)
  }
  instance = createApp(App)
  instance.use(STable)
  instance.use(pinia)
  // 注册全局组件
  registerGlobComp(instance)
  const routeStore = useRouteStore()
  if (qiankunWindow.__POWERED_BY_QIANKUN__) {
    const globalActions = props.actions || props
    actions.setActions(globalActions)
    console.log('获取全局Store')
    // 优先从全局状态中读取一次，保证首次渲染就能拿到主应用的 store
    const globalState = await waitForGlobalStateReady()
    if (globalState && globalState.userStore) {
      userStore = globalState.userStore
      // 兼容主应用中全局状态字段名：优先使用 drawerStore，没有时再回退到 useDrawerStore
      drawerStore = globalState.drawerStore || globalState.useDrawerStore
      navigationStore = globalState.navigationStore
    }
    addGlobalStateListener((state, prev) => {
      userStore = state.userStore
      drawerStore = state.drawerStore || state.useDrawerStore
      navigationStore = state.navigationStore
    })
  } else {
    userStore = useUserStore()
    navigationStore = useNavigationStore()
    drawerStore = useDrawerStore()
  }

  initAxiosInstances(userStore)

  const currentLanguage = normalizeLocale(navigationStore?.language || 'zh')
  const i18n = await setupI18n(currentLanguage)

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
  if (userStore.access_token != '') {
    //获取动态路由
    await routeStore.loadRoutes(userStore.userRoles, currentLanguage)

    if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
      await loadLocaleMessages(i18n, currentLanguage, {
        includeCommonMessages: true,
        includeRouteMessages: true,
        routeSourceData: routeStore.permissionSystemsCache
      })
    }

    routeStore.routes.forEach((route) => {
      addRoutesRecursively(route)
    })
  }

  instance.use(router)
  instance.use(i18n)
  // 在 qiankun 场景下，直接挂载到主应用传入的 container（例如 #ids4-container）
  // 独立运行时则挂载到自身 index.html 中的 #${VITE_APP_APPNAME}-subApp
  instance.mount(container || `#${import.meta.env.VITE_APP_APPNAME}-subApp`)
}

// 如何独立运行微应用？
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  console.log('并不是qiankun渲染')
  render()
}

// 监听主应用发出的 tab 切换事件，在当前子应用内切换路由
const handleMainAppEvent = (event) => {
  const { menu, path } = event.detail
  if (menu === `/${import.meta.env.VITE_APP_APPNAME}`) {
    router.push(path)
  }
}

renderWithQiankun({
  mount(props) {
    if (qiankunWindow.__POWERED_BY_QIANKUN__) {
      const globalActions = props.actions || props
      initGlobalStateListener(globalActions)
    }
    render(props)
    if (qiankunWindow.__POWERED_BY_QIANKUN__) {
      window.addEventListener('main-app-trigger-route', handleMainAppEvent)
    }
  },
  bootstrap() {
    console.log('%c', 'color:green;', ' ChildOne bootstrap')
  },
  update() {
    console.log('%c', 'color:green;', ' ChildOne update')
  },
  unmount(props) {
    instance.unmount()
    instance = null
    if (qiankunWindow.__POWERED_BY_QIANKUN__) {
      clearGlobalStateListener()
      window.removeEventListener('main-app-trigger-route', handleMainAppEvent)
    }
  }
})
