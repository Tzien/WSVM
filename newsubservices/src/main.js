import { createApp, ref } from 'vue'
/* 引入全局css配置文件 */
import commonCssText from '@/assets/css/common.css?inline'
import App from './App.vue'
import 'ant-design-vue/dist/reset.css'
import router from './router'
import pinia from './store/index'
import { setupI18n, loadLocaleMessages } from './lang/i18n.js'
import { initAxiosInstances } from './utils/request.js'
import actions from './shared/actions'
import { renderWithQiankun, qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore, useDrawerStore, useRouteStore, useNavigationStore } from './store/index'
import { initGlobalStateListener, clearGlobalStateListener, addGlobalStateListener, getGlobalState } from './shared/globalStateManager'

// 引入SurelyVue
import '@surely-vue/table/dist/index.less'
import STable from '@surely-vue/table'
import { setLicenseKey } from '@surely-vue/table'
setLicenseKey(import.meta.env.VITE_STABLESECRIT)

let instance = null
let i18nInstance = null
var userStore = ref({})
var navigationStore = ref({})
var drawerStore = ref({})

const isPlainObject = (value) => Object.prototype.toString.call(value) === '[object Object]'

const deepMerge = (target, source) => {
  if (!isPlainObject(target) || !isPlainObject(source)) {
    return source
  }

  const merged = { ...target }
  Object.keys(source).forEach((key) => {
    const targetValue = merged[key]
    const sourceValue = source[key]
    merged[key] = isPlainObject(targetValue) && isPlainObject(sourceValue) ? deepMerge(targetValue, sourceValue) : sourceValue
  })

  return merged
}

const applyI18nSnapshot = (i18n, i18nSnapshot) => {
  if (!i18n || !i18nSnapshot?.locale || !isPlainObject(i18nSnapshot?.messages)) {
    return
  }

  const composer = i18n.global ?? i18n
  const snapshotLocale = i18nSnapshot.locale
  const hasSnapshotMessages = Object.keys(i18nSnapshot.messages).length > 0

  if (hasSnapshotMessages && typeof composer?.setLocaleMessage === 'function') {
    const currentMessages = typeof composer?.getLocaleMessage === 'function' ? composer.getLocaleMessage(snapshotLocale) || {} : {}
    const mergedMessages = deepMerge(currentMessages, i18nSnapshot.messages)
    composer.setLocaleMessage(snapshotLocale, mergedMessages)
  }

  if (hasSnapshotMessages && composer?.locale && typeof composer.locale === 'object' && 'value' in composer.locale) {
    composer.locale.value = snapshotLocale
  }
}

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
  let initialGlobalState = null
  instance = createApp(App)
  instance.use(STable)
  instance.use(pinia)
  const routeStore = useRouteStore()

  if (qiankunWindow.__POWERED_BY_QIANKUN__) {
    const globalActions = props.actions || props
    actions.setActions(globalActions)
    // 优先从全局状态中读取一次，保证首次渲染就能拿到主应用的 store
    const globalState = await waitForGlobalStateReady()
    initialGlobalState = globalState
    if (globalState && globalState.userStore) {
      userStore = globalState.userStore
      // 兼容主应用中全局状态字段名：优先使用 drawerStore，没有时再回退到 useDrawerStore
      drawerStore = globalState.drawerStore || globalState.useDrawerStore
      navigationStore = globalState.navigationStore
    }

    // 监听后续全局状态变更
    addGlobalStateListener(async (state, prev) => {
      userStore = state.userStore
      drawerStore = state.drawerStore || state.useDrawerStore
      navigationStore = state.navigationStore

      if (i18nInstance && state?.i18nSnapshot) {
        applyI18nSnapshot(i18nInstance, state.i18nSnapshot)
      }

    })
  } else {
    userStore = useUserStore()
    navigationStore = useNavigationStore()
    drawerStore = useDrawerStore()
  }

  initAxiosInstances(userStore)

  i18nInstance = await setupI18n(navigationStore.language, initialGlobalState?.i18nSnapshot)
  applyI18nSnapshot(i18nInstance, initialGlobalState?.i18nSnapshot)

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
    const currentLanguage = navigationStore?.language || i18nInstance?.global?.locale?.value || 'zh'
    await routeStore.loadRoutes(userStore.userRoles, currentLanguage)

    if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
      await loadLocaleMessages(i18nInstance, currentLanguage, {
        includeCommonMessages: true,
        includeRouteMessages: false
      })
      await loadLocaleMessages(i18nInstance, currentLanguage, {
        includeCommonMessages: false,
        includeRouteMessages: true,
        routeSourceData: routeStore.permissionSystemsCache
      })
    }

    routeStore.routes.forEach((route) => {
      addRoutesRecursively(route)
    })
  }

  instance.use(router)
  instance.use(i18nInstance)
  // 在 qiankun 场景下，直接挂载到主应用传入的 container（例如 #ids4-container）
  // 独立运行时则挂载到自身 index.html 中的 #${VITE_APP_APPNAME}-subApp
  instance.mount(container || `#${import.meta.env.VITE_APP_APPNAME}-subApp`)
}

// 如何独立运行微应用？
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
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
  bootstrap() {},
  update() {},
  unmount(props) {
    instance.unmount()
    instance = null
    if (qiankunWindow.__POWERED_BY_QIANKUN__) {
      clearGlobalStateListener()
      window.removeEventListener('main-app-trigger-route', handleMainAppEvent)
    }
  }
})
