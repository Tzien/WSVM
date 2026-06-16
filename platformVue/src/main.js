// import './public-path' // 注意需要引入public-path
import { createApp, ref } from 'vue'
import { initAxiosInstances } from './utils/request.js'
import App from './App.vue'
import 'ant-design-vue/dist/reset.css'
/* 引入全局css配置文件 */
import commonCssText from '@/assets/css/common.css?inline'
import { useInsertButtonOperationLogAsync } from './api/buttonOperationLog.js'
// import { useInsertSysMenuRecord } from '@/api/MenuSysLog'

import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import router from './router'
import pinia from './store/index'
import { setupI18n, loadLocaleMessages } from './lang/i18n.js'

import actions from './shared/actions'
import { initGlobalStateListener, clearGlobalStateListener, addGlobalStateListener, getGlobalState } from './shared/globalStateManager'
import { renderWithQiankun, qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore, useDrawerStore, useRouteStore, useNavigationStore } from './store/index'
const routeStore = useRouteStore(pinia)
// var userStore = ref({})
// var navigationStore = ref({})
var userStore = useUserStore(pinia).$state
var navigationStore = useNavigationStore(pinia).$state
var drawerStore = useDrawerStore(pinia).$state
var signalRStore = ref({})
let instance = null

function ensurePlatformCommonCssInjected() {
  const styleId = 'platform-common-css'
  if (document.getElementById(styleId)) return
  const styleEl = document.createElement('style')
  styleEl.id = styleId
  styleEl.setAttribute('data-app', 'platform-subApp')
  styleEl.textContent = commonCssText
  document.head.appendChild(styleEl)
}

// 无论是否 qiankun，都确保 platform 的关键全局样式存在（避免生产环境只加载主应用 css 时丢样式）
ensurePlatformCommonCssInjected()

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

async function render(props = {}) {
  const { container } = props
  let i18nSnapshot
  if (qiankunWindow.__POWERED_BY_QIANKUN__) {
    const globalActions = props.actions || props
    actions.setActions(globalActions)
    // 优先从全局状态中读取一次，保证首次渲染就能拿到主应用的 store
    const globalState = await waitForGlobalStateReady()
    if (globalState && globalState.userStore) {
      userStore = globalState.userStore
      drawerStore = globalState.drawerStore || globalState.useDrawerStore
      navigationStore = globalState.navigationStore
      signalRStore = globalState.signalRStore
      i18nSnapshot = globalState.i18nSnapshot
    }

    // 监听后续全局状态变更
    addGlobalStateListener((state, prev) => {
      userStore = state.userStore
      drawerStore = state.drawerStore || state.useDrawerStore
      navigationStore = state.navigationStore
      signalRStore = state.signalRStore
    })
  }
  initAxiosInstances(userStore)
  const currentLanguage = `${navigationStore?.language || ''}`.trim() || 'zh'
  const i18n = await setupI18n(currentLanguage, i18nSnapshot)
  instance = createApp(App)
  for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    instance.component(key, component)
  }

  if (userStore.access_token != '') {
    //获取动态路由
    await routeStore.loadRoutes(userStore.userRoles, currentLanguage)

    await loadLocaleMessages(i18n, currentLanguage, {
      includeCommonMessages: true,
      includeRouteMessages: false
    })

    await loadLocaleMessages(i18n, currentLanguage, {
      includeCommonMessages: false,
      includeRouteMessages: true,
      routeSourceData: routeStore.permissionSystemsCache
    })

    routeStore.routes.forEach((route) => {
      addRoutesRecursively(route)
    })
  }

  instance.directive('btnlog', {
    mounted: (el, binding) => {
      {
        el.addEventListener('click', (event) => {
          if (typeof binding.value !== 'object') return
          useInsertButtonOperationLogAsync({
            buttonOperationId: binding.value.buttonOperationId,
            belongPage: binding.value.belongPage,
            buttonName: binding.value.buttonName,
            operationPersonId: userStore.userid
          }).then((data) => {
            if (data.code === 200 && data.success) {
              console.log('操作成功')
            } else {
              console.log('操作失败')
            }
          })
        })
      }
    }
  })

  instance.use(router)
  instance.use(pinia)
  instance.use(i18n)
  // 在 qiankun 场景下，直接挂载到主应用传入的 container（例如 #platform-container）
  // 独立运行时则挂载到自身 index.html 中的 #${VITE_APP_APPNAME}-subApp
  instance.mount(container || `#${import.meta.env.VITE_APP_APPNAME}-subApp`)
  
  // 预加载常用组件，提升用户体验
  setTimeout(() => {
    preloadCommonComponents()
  }, 2000)
}

// 预加载常用组件
function preloadCommonComponents() {
  const commonRoutes = ['/Home', '/Login']
  commonRoutes.forEach(route => {
    try {
      import(`./views${route}.vue`).catch(() => {
        // 忽略加载失败的情况
      })
    } catch (error) {
      // 忽略预加载错误
    }
  })
}

// 如何独立运行微应用？
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  console.log('并不是qiankun渲染')
  render()
}

const handleMainAppEvent = (event) => {
  const { menu, path } = event.detail
  if (menu == `/${import.meta.env.VITE_APP_APPNAME}`) {
    // 使用 Vue Router 跳转路由
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
  },
  update() {
  },
  unmount(props) {
    instance.unmount()
    instance = null
    clearGlobalStateListener()
    window.removeEventListener('main-app-trigger-route', handleMainAppEvent)
  }
})
