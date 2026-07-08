import { createApp } from 'vue'
/* 引入全局css配置文件 */
import '@/assets/css/common.css'
import '@/css/fonts/ym/iconfont.css'
import '@/css/fonts/ym-custom/iconfont.css'
import App from './App.vue'
import router from './router/router'
import 'ant-design-vue/dist/reset.css'
import pinia from './store/index'

import { setupI18n } from './lang/i18n.js'
import { registerMicroApps, start } from 'qiankun'
// import { useGetPermissionAsync } from './api/permission'
import { useUserStore } from './store/index.js'
import actions from './actions'

// 引入SurelyVue
import '@surely-vue/table/dist/index.less'
import STable from '@surely-vue/table'
import { setLicenseKey } from '@surely-vue/table'
setLicenseKey(import.meta.env.VITE_STABLESECRIT)
/* 全局超时监测 */
import InactivityMonitor from '@/utils/inactivity-monitor'
import { message } from 'ant-design-vue'

const app = createApp(App)
app.use(router)
app.use(pinia)
app.use(STable)
const i18n = await setupI18n()
await app.use(i18n)
/* 全局超时监测 */
app.use(InactivityMonitor, {
  inactivityLimit: 20 * 60 * 1000, // 20 分钟
  warningTime: 10 * 60 * 1000, // 提前 10 分钟提醒
  onWarning: () => {
    if (userStore.access_token == '') {
      return
    }
    message.warn('即将超时退出...')
  },
  onTimeout: () => {
    if (userStore.access_token == '') {
      return
    }
    message.warn('超时退出')
    localStorage.clear()
    window.location.href = '/Login'
  }
})
await app.mount('#mainApp')

const userStore = useUserStore()

// 封装 qiankun 初始化逻辑，方便在登录成功后重复调用
function initQiankun() {
  const microApps = []
  const nameSet = new Set()
  userStore.allSysCodeUrl.forEach((data) => {
    const item = data.split('_')
    const sysCode = item[1]
    if (!sysCode) return

    const name = `${sysCode}-subApp`
    if (nameSet.has(name)) {
      // 已经存在同名微应用，跳过，避免 single-spa #21
      return
    }
    nameSet.add(name)
    const microApp = {
      name,
      entry: process.env.NODE_ENV === 'development' ? item[0] : `/subapp/${sysCode}/`,
      activeRule: `/${sysCode}`,
      container: `#${sysCode}-container`,
      // 把 qiankun 的全局状态 actions 通过 props 传给子应用
      props: {
        actions
      }
    }
    // 初始化时认为该子应用尚未准备好
    userStore.setSubAppReady(sysCode, false)
    microApps.push(microApp)
  })

  // 没有任何微应用信息时，不做初始化
  if (microApps.length === 0) {
    return
  }

  // 避免在同一次页面生命周期内重复初始化 qiankun
  if (window.__QIANKUN_STARTED__) {
    return
  }

  window.__QIANKUN_STARTED__ = true
  registerMicroApps(microApps, {
    beforeMount(app) {
      console.log('挂载前', app.name)
      // 子应用开始挂载时，打开全局 loading，并标记为未就绪
      userStore.isLoading = true
      const sysCode = app.name.replace('-subApp', '')
      userStore.setSubAppReady(sysCode, false)
    },
    afterMount(app) {
      console.log('挂载完成', app.name)
      const sysCode = app.name.replace('-subApp', '')
      // 子应用挂载完成，标记为就绪，并关闭 loading
      userStore.setSubAppReady(sysCode, true)
      userStore.isLoading = false
    }
  })
  start({
    prefetch: false, // 关闭激进的预加载行为
    sandbox: {
      strictStyleIsolation: false, // 允许样式共享
      experimentalStyleIsolation: true
    },
    // 预加载配置
    fetch: (url, options) => {
      return window.fetch(url, options)
    }
  })
}

// 暴露到全局，方便登录成功后主动触发
window.__init_qiankun__ = initQiankun

// 初次加载时尝试初始化（例如已登录并且有缓存的微应用信息）
initQiankun()
