<template>
  <div class="PlatForm">
    <a-layout style="height: calc(100vh - 96px)" v-if="userStore.access_token"
      v-memo="[userStore.access_token, navigationStore.language]">
      <a-layout-sider :trigger="null" v-model:collapsed="drawerStore.menuCollapsed" collapsible class="PlatformSider"
        :class="drawerStore.theme == 'light' ? 'lightColor' : 'darkColor'" :width="drawerStore.menuwidth">
        <!-- 左侧菜单水印背景，作为侧边栏内部的绝对定位背景层 -->
        <div class="menuBGWaterLogo" :style="menuBgStyle"></div>

        <a-menu class="PlatformMenuStyle" v-model:openKeys="openKeys" v-model:selectedKeys="selectedKeys"
          :style="menuStyle" mode="inline" :theme="drawerStore.theme" :items="items" @click="platformMenuClick"
          @select="platformMenuSelect"></a-menu>
        <div class="MenuColBtn" :class="!drawerStore.menuCollapsed ? 'RightIcon' : 'CenterIcon'"
          :style="menuColBtnStyle">
          <MenuUnfoldOutlined class="MenuColBtnIcon" @click="toggleCollapsed" v-if="drawerStore.menuCollapsed" />
          <MenuFoldOutlined class="MenuColBtnIcon" @click="toggleCollapsed" v-else />
        </div>
      </a-layout-sider>
      <a-layout>
        <a-layout-content>
          <a-config-provider :locale="currentAntdLocale">
            <div style="height: calc(100vh - 96px - 35px)">
              <router-view v-slot="{ Component }">
                <keep-alive :include="includedComponents">
                  <component :is="Component" :key="route.path"></component>
                </keep-alive>
              </router-view>
            </div>
          </a-config-provider>
        </a-layout-content>
        <a-layout-footer style="
            text-align: right !important;
            height: 35px !important;
            line-height: 35px !important;
            background-color: #eef3f9 !important;
            bottom: 0px;
            right: 0px;
            padding: 0px !important;
          ">
          <span style="margin-right: 20px">版权 © 2025 中冶京诚数字科技（北京）有限公司 all rights reserved.</span></a-layout-footer>
      </a-layout>
    </a-layout>
  </div>
  <div v-if="isPageI18nLoading" class="page-i18n-loading-mask">
    <a-spin size="large" />
  </div>
</template>

<script setup>
import { ref, watch, h, watchEffect, computed, nextTick, onMounted } from 'vue'
import { useRouteStore, useUserStore, useDrawerStore, useNavigationStore } from './store/index.js'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useGlobalState } from './shared/useGlobalState'
import actions from './shared/actions'
import { useRouter, useRoute } from 'vue-router'
import { useInsertSysMenuRecord } from '@/api/MenuSysLog'
import { useI18n } from 'vue-i18n'
import { loadLocaleMessages, loadPageLocaleMessages } from './lang/i18n.js'
import { message } from 'ant-design-vue'
import * as Icons from '@ant-design/icons-vue'
import { MenuFoldOutlined, MenuUnfoldOutlined } from '@ant-design/icons-vue'
import menuBgUrl from '@/assets/images/menuBG.png'

/* 防抖 */
import { debounce } from 'lodash-es'

/* 国际化相关 */
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import 'dayjs/locale/en'
import 'dayjs/locale/id'
import 'dayjs/locale/ru'
const currentAntdLocale = ref(null)
let lastProcessedLanguage = ''  // 记录最后处理的语言
// 语言包映射表（Ant Design Vue）
const antdLocaleMap = {
  'zh': () => import('ant-design-vue/es/locale/zh_CN'),
  'en': () => import('ant-design-vue/es/locale/en_US'),
  'id': () => import('ant-design-vue/es/locale/id_ID'),
  'ru': () => import('ant-design-vue/es/locale/ru_RU'),
}
// dayjs 语言包映射表
const dayjsLocaleMap = {
  'zh': 'zh-cn',
  'en': 'en',
  'id': 'id',
  'ru': 'ru',
}
// 创建防抖版本的 loadAntdLocale
const debouncedLoadAntdLocale = debounce(async (language) => {
  if (!language) return
  try {
    const loader = antdLocaleMap[language]
    if (loader) {
      const module = await loader()
      currentAntdLocale.value = module.default
      const dayjsLocale = dayjsLocaleMap[language]
      if (dayjsLocale) {
        dayjs.locale(dayjsLocale)
      }
    } else {
      console.warn(`未找到语言包: ${language}，使用中文降级`)
      const fallback = await import('ant-design-vue/es/locale/zh_CN')
      currentAntdLocale.value = fallback.default
      dayjs.locale('zh-cn')
    }
  } catch (error) {
    console.error('加载 Ant Design 语言包失败:', error)
    const fallback = await import('ant-design-vue/es/locale/zh_CN')
    currentAntdLocale.value = fallback.default
    dayjs.locale('zh-cn')
  }
}, 100)

const i18n = useI18n({ useScope: 'global' })
const router = useRouter()
const route = useRoute()
const { globalStore } = useGlobalState()
var isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__

var userStore = ref({})
var drawerStore = ref({})
var navigationStore = ref({})
// let isApplyingLanguageFromGlobal = false
let localeRefreshPromise = null
let localeRefreshKey = ''
const routeStore = useRouteStore()
const isPageI18nLoading = ref(false)
let pageI18nLoadingId = 0
const normalizeLocale = (locale) => `${locale || ''}`.trim()

const resolveRoles = () => {
  const rolesSource = userStore.value?.userRoles
  return Array.isArray(rolesSource) ? rolesSource : []
}

const buildLocaleRefreshKey = (locale) => {
  const hasToken = !!userStore.value?.access_token
  const roleKey = resolveRoles()
    .map((role) => `${role}`)
    .join(',')
  return `${locale}@@${hasToken ? roleKey : 'guest'}`
}

const getCurrentPageCode = () => {
  const pageCode = route.meta?.KeepAliveName
  const normalizedPageCode = `${pageCode || ''}`.trim()
  return normalizedPageCode && normalizedPageCode !== 'unknown' ? normalizedPageCode : ''
}

const loadCurrentPageI18n = async (locale) => {
  const pageCode = getCurrentPageCode()
  if (!pageCode) return
  const loadingId = ++pageI18nLoadingId
  isPageI18nLoading.value = true
  try {
    await loadPageLocaleMessages(i18n, locale, pageCode)
  } finally {
    if (loadingId === pageI18nLoadingId) {
      isPageI18nLoading.value = false
    }
  }
}

const applyLocaleAndRefreshMenu = async (locale) => {
  const targetLocale = normalizeLocale(locale) || 'zh'
  if (!targetLocale) return

  const refreshKey = buildLocaleRefreshKey(targetLocale)
  if (localeRefreshPromise && localeRefreshKey === refreshKey) {
    await localeRefreshPromise
    return
  }

  localeRefreshKey = refreshKey
  localeRefreshPromise = (async () => {
    const hasToken = !!userStore.value?.access_token
    const roles = resolveRoles()

    if (hasToken && roles.length > 0) {
      const routeSourceData = await routeStore.loadRoutes(roles, targetLocale)
      await loadLocaleMessages(i18n, targetLocale, {
        includeCommonMessages: true,
        includeRouteMessages: false
      })
      await loadLocaleMessages(i18n, targetLocale, {
        includeCommonMessages: false,
        includeRouteMessages: true,
        routeSourceData
      })
      await loadCurrentPageI18n(targetLocale)
    }

    if (i18n.locale.value !== targetLocale) {
      i18n.locale.value = targetLocale
    }

    await nextTick()
    loadMenuItems()
  })()

  try {
    await localeRefreshPromise
  } finally {
    if (localeRefreshKey === refreshKey) {
      localeRefreshPromise = null
      localeRefreshKey = ''
    }
  }
}

const applyI18nSnapshot = async (snapshot) => {
  if (!snapshot || typeof snapshot !== 'object') return
  const snapshotLocale = snapshot.locale || navigationStore.value?.language
  const hasSnapshotMessages = snapshot.messages && typeof snapshot.messages === 'object' && Object.keys(snapshot.messages).length > 0

  if (snapshotLocale && hasSnapshotMessages && typeof i18n.mergeLocaleMessage === 'function') {
    i18n.mergeLocaleMessage(snapshotLocale, snapshot.messages)

    if (i18n.locale.value !== snapshotLocale) {
      i18n.locale.value = snapshotLocale
    }

    await nextTick()
    loadMenuItems()
    return
  }

  if (snapshotLocale) {
    await applyLocaleAndRefreshMenu(snapshotLocale)
  }
}

if (!isQiankun) {
  userStore.value = useUserStore()
  drawerStore.value = useDrawerStore()
  navigationStore.value = useNavigationStore()
} else {
  watchEffect(() => {
    // 非 qiankun 场景下，同步全局 store 到本地引用
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      drawerStore.value = globalStore.value.drawerStore
      navigationStore.value = globalStore.value.navigationStore
    }
  })
}

// 本地菜单选中 keys，用于驱动左侧菜单高亮
const selectedKeys = ref([])
const openKeys = ref([])

const normalizeKeys = (keys) => {
  return Array.isArray(keys) ? [...keys] : []
}

const isSameKeys = (left, right) => {
  const leftKeys = normalizeKeys(left)
  const rightKeys = normalizeKeys(right)
  return leftKeys.length === rightKeys.length && leftKeys.every((key, index) => key === rightKeys[index])
}

const getCurrentDrawerStore = () => {
  return drawerStore.value
}

const getFoldmenuKeys = () => {
  const ds = getCurrentDrawerStore()
  return ds ? normalizeKeys(ds.foldmenu) : []
}

const isMenuCollapsed = () => {
  const ds = getCurrentDrawerStore()
  return !!(ds && ds.menuCollapsed)
}

const syncDrawerStore = () => {
  const ds = getCurrentDrawerStore()
  if (isQiankun && ds) {
    actions.setGlobalState({
      drawerStore: ds
    })
  }
}

const setDrawerFoldmenu = (keys) => {
  const ds = getCurrentDrawerStore()
  if (!ds) return

  const nextKeys = normalizeKeys(keys)
  if (isSameKeys(getFoldmenuKeys(), nextKeys)) return

  if (typeof ds.setFoldmenu === 'function') {
    ds.setFoldmenu(nextKeys)
  } else {
    ds.foldmenu = nextKeys
  }
  syncDrawerStore()
}

const setOpenKeysFromFoldmenu = (keys) => {
  const nextKeys = normalizeKeys(keys)
  if (!isMenuCollapsed() && !isSameKeys(openKeys.value, nextKeys)) {
    openKeys.value = nextKeys
  }
}

const getFoldmenuKeysFromSelect = (val) => {
  const currentOpenKeys = normalizeKeys(openKeys.value)
  if (currentOpenKeys.length > 0) return currentOpenKeys

  const selectedKeySet = new Set(normalizeKeys(val.selectedKeys))
  return normalizeKeys(val.keyPath).filter((key) => !selectedKeySet.has(key))
}

watch(
  () => getFoldmenuKeys(),
  (keys) => {
    setOpenKeysFromFoldmenu(keys)
  },
  { immediate: true, deep: true }
)

watch(
  openKeys,
  (keys) => {
    if (!isMenuCollapsed()) {
      setDrawerFoldmenu(keys)
    }
  },
  { deep: true }
)

watch(
  () => route.path,
  (path) => {
    if (!path) return

    // 左侧菜单高亮始终跟随当前路由 path
    selectedKeys.value = [path]

    // 兼容原有依赖 drawerStore.selected 的逻辑（如果有）
    if (isQiankun) {
      if (drawerStore.value && drawerStore.value.changeSelected) {
        drawerStore.value.changeSelected([path])
      } else if (drawerStore.value && Array.isArray(drawerStore.value.selected)) {
        drawerStore.value.selected = [path]
      }
    } else {
      if (drawerStore && drawerStore.changeSelected) {
        drawerStore.changeSelected([path])
      } else if (drawerStore && Array.isArray(drawerStore.selected)) {
        drawerStore.selected = [path]
      }
    }
  },
  { immediate: true }
)

const keepAliveComponents = ref([])
const includedComponents = computed(() => {
  const name = route.meta.KeepAliveName
  if (!name) return keepAliveComponents.value

  const index = keepAliveComponents.value.indexOf(name)
  if (route.meta.IsKeepAlive && index === -1) {
    // 限制缓存组件数量，避免内存泄漏
    if (keepAliveComponents.value.length >= 10) {
      keepAliveComponents.value.shift()
    }
    keepAliveComponents.value.push(name)
  } else if (!route.meta.IsKeepAlive && index !== -1) {
    keepAliveComponents.value.splice(index, 1)
  }
  return keepAliveComponents.value
})

// const syncDrawerToMain = () => {
//   if (!isQiankun) return
//   if (!globalStore.value) return
//   actions.setGlobalState({
//     ...globalStore.value,
//     drawerStore: drawerStore.value
//   })
// }

const menuStyle = computed(() => ({
  width: drawerStore.value.menuwidth,
  maxHeight: 'calc(100vh - 96px - 35px)',
  overflowY: 'auto',
  backgroundColor: '#043684'
}))

const menuBgStyle = computed(() => ({
  width: drawerStore.value.menuCollapsed ? '50px' : '120px',
  backgroundImage: `url(${menuBgUrl})`,
  backgroundSize: 'contain',
  backgroundRepeat: 'no-repeat',
  backgroundPosition: 'center',
  pointerEvents: 'none',
  position: 'absolute',
  bottom: '0',
  left: '0',
  height: 'calc(100vh - 180px)'
}))

const menuColBtnStyle = {
  position: 'absolute',
  left: '0px',
  bottom: '0px',
  width: '100%',
  height: '35px',
  display: 'flex',
  alignItems: 'center',
  background: 'linear-gradient(to top, #1a335b, #053784)'
}

// 使用防抖优化菜单点击事件
const platformMenuClick = (val) => {
  useInsertSysMenuRecord({
    CreateId: userStore.value.userid,
    MenuId: val.item.menuid,
    IsSysInfoId: val.item.isSysInfo
  })
  if (
    navigationStore.value.tabs.length < 10 ||
    navigationStore.value.tabs.some((element) => {
      return element.key === val.key
    })
  ) {
    router.push({ path: val.item.path })
  }
}

function toggleCollapsed() {
  const ds = drawerStore.value || {}
  // 本地切换快照中的收起状态
  const prevCollapsed = !!ds.menuCollapsed
  ds.menuCollapsed = !prevCollapsed

  // 根据收起状态调整本地侧边栏宽度和展开的菜单 key
  if (!ds.menuCollapsed) {
    // 展开：还原为上次记录的宽度（如果有），并恢复折叠菜单
    if (typeof ds.menuwidthCopy === 'number') {
      ds.menuwidth = ds.menuwidthCopy
    }
    openKeys.value = getFoldmenuKeys()
  } else {
    // 收起：统一使用收起宽度 80，并清空展开项
    ds.menuwidth = 80
    openKeys.value = []
  }

  // 将最新的抽屉状态通过 qiankun 全局状态回推给主应用的 Pinia drawerStore
  actions.setGlobalState({
    drawerStore: ds
  })
}

const platformMenuSelect = (val) => {
  const sysCode = import.meta.env.VITE_APP_APPNAME
  const innerPath = val.item.path
  const tabKey = sysCode ? `/${sysCode}${innerPath}` : innerPath

  const newTab = {
    key: tabKey,
    title: val.item.title,
    path: innerPath,
    sysCode,
    i18nKey: val.item.i18nKey
  }

  if (!isQiankun) {
    const nav = navigationStore.value || {}
    const tabs = Array.isArray(nav.tabs) ? nav.tabs : []

    const exists = tabs.some((element) => {
      return element && element.key === tabKey
    })

    if (!exists) {
      // if (tabs.length >= 10) {
      //   message.warn('最多打开10个选项卡，请关闭一些后重试')
      //   return
      // }
      if (typeof nav.addTabs === 'function') {
        nav.addTabs(newTab)
      }
    }

    if (drawerStore.value) {
      if (typeof drawerStore.value.changeSelected === 'function') {
        drawerStore.value.changeSelected(val.selectedKeys)
      } else if (Array.isArray(drawerStore.value.selected)) {
        drawerStore.value.selected = val.selectedKeys
      }

      setDrawerFoldmenu(getFoldmenuKeysFromSelect(val))
    }
  } else {
    const navSnapshot = navigationStore.value || {}
    const tabs = Array.isArray(navSnapshot.tabs) ? navSnapshot.tabs : []

    const exists = tabs.some((element) => {
      return element && element.key === tabKey
    })

    if (!exists) {
      if (tabs.length >= 10) {
        message.warn('最多打开10个选项卡，请关闭一些后重试')
        return
      }

      if (typeof window !== 'undefined') {
        setTimeout(() => {
          window.dispatchEvent(
            new CustomEvent('sub-app-add-tab', {
              detail: { tab: newTab }
            })
          )
        }, 0)
      }
    }

    if (drawerStore.value) {
      if (typeof drawerStore.value.changeSelected === 'function') {
        drawerStore.value.changeSelected(val.selectedKeys)
      } else if (Array.isArray(drawerStore.value.selected)) {
        drawerStore.value.selected = val.selectedKeys
      }

      setDrawerFoldmenu(getFoldmenuKeysFromSelect(val))
      syncDrawerStore()
    }
  }
}

// 动态映射图标
const getIconComponent = (iconName) => Icons[iconName] || Icons['QuestionCircleOutlined']

//构造菜单Item数据
const generateMenuData = (routes) =>
  routes.map((route) => {
    // 确保所有字段都有默认值，避免undefined
    const menuItem = {
      menuid: route.id || null,
      isSysInfo: route.isSysInfo || false,
      key: route.path || route.name || 'unknown',
      path: route.path || '/',
      i18nKey: route.i18nKey || null,
      icon: route.icon ? () => h(getIconComponent(route.icon)) : undefined,
      children: route.children && route.children.length > 0 ? generateMenuData(route.children) : undefined
    }

    // 处理label和title，确保不为undefined
    // 使用i18n翻译，如果翻译失败则使用原始名称
    const fallbackName = route.originalName || route.name || '未命名'

    if (route.i18nKey) {
      const translated = i18n.t(route.i18nKey)
      const resolvedTitle = translated && translated !== route.i18nKey ? translated : fallbackName
      menuItem.label = resolvedTitle
      menuItem.title = resolvedTitle
    } else {
      menuItem.label = fallbackName
      menuItem.title = fallbackName
    }

    return menuItem
  })

const items = ref([])

// 使用computed优化菜单数据生成
const menuItems = computed(() => {
  const currentLocale = i18n.locale.value
  if (!currentLocale) return []
  if (!userStore.value.access_token) return []
  if (!routeStore.routes || routeStore.routes.length === 0) return []
  const matched = routeStore.routes.find((item) => item.path === `/${import.meta.env.VITE_APP_APPNAME}`)
  return generateMenuData(matched?.children || [])
})

// 加载菜单项的函数
function loadMenuItems() {
  if (!userStore.value.access_token || !routeStore.routes || routeStore.routes.length === 0) {
    items.value = []
    return
  }
  items.value = menuItems.value
}

watch(
  () => userStore.value.access_token,
  async (token) => {
    if (!token) return
    const currentLanguage = normalizeLocale(navigationStore.value?.language) || 'zh'
    await Promise.all([  // 改为并行执行两个任务
      applyLocaleAndRefreshMenu(currentLanguage),
      debouncedLoadAntdLocale(currentLanguage)  // 新增：加载 Ant Design 语言包
    ])
  },
  { immediate: true } // 初始化立即执行一次
)

// 监听路由数据变化，重新生成菜单
watch(
  () => routeStore.routes,
  (newRoutes) => {
    if (newRoutes && newRoutes.length > 0 && userStore.value.access_token) {
      loadMenuItems()
    }
  }
)

// 优化store订阅，使用浅层监听减少性能开销
watch(
  () => globalStore.value,
  (newGlobalStore) => {
    if (!isQiankun || !newGlobalStore) return
    const { userStore: us, drawerStore: ds, navigationStore: ns } = newGlobalStore

    // 使用浅比较，只在关键属性变化时更新
    if (userStore.value?.access_token !== us?.access_token || userStore.value?.userRoles !== us?.userRoles) {
      userStore.value = us
    }
    if (
      drawerStore.value?.selected !== ds?.selected ||
      drawerStore.value?.menuCollapsed !== ds?.menuCollapsed ||
      drawerStore.value?.menuwidth !== ds?.menuwidth
    ) {
      drawerStore.value = ds
    }
    if (navigationStore.value?.language !== ns?.language || navigationStore.value?.tabs !== ns?.tabs) {
      navigationStore.value = ns
    }
  },
  { immediate: true }
)

/* 监听语言变化重新加载Menu菜单 */
watch(
  () => navigationStore.value.language,
  async (newLanguage, oldLanguage) => {
    try {

      // 如果已经处理过这个语言，跳过
      if (newLanguage === lastProcessedLanguage) {
        console.log('[语言切换] 已经处理过，跳过')
        return
      }

      if (!newLanguage) return
      if (newLanguage === oldLanguage) return

      // 标记正在处理
      lastProcessedLanguage = newLanguage

      await Promise.all([
        applyLocaleAndRefreshMenu(newLanguage),
        debouncedLoadAntdLocale(newLanguage)
      ])
      console.log('[语言切换] 完成')
    } catch (error) {
      console.error('[语言切换] 失败:', error)
      // 失败时重置，允许重试
      lastProcessedLanguage = ''
    }
  },
  { immediate: true, deep: true }
)

watch(
  () => globalStore.value?.i18nSnapshot,
  async (snapshot) => {
    if (!isQiankun || !snapshot) return

    try {
      await applyI18nSnapshot(snapshot)
      if (snapshot.locale && navigationStore.value && navigationStore.value.language !== snapshot.locale) {
        navigationStore.value.language = snapshot.locale
      }
    } catch (error) {
      console.error('应用 i18n 快照失败:', error)
    }
  },
  { immediate: true }
)

onMounted(() => {
  const initialLanguage = normalizeLocale(navigationStore.value?.language) || 'zh'
  debouncedLoadAntdLocale(initialLanguage)
})
</script>
<style lang="scss">
.PlatForm {

  // 左侧菜单和水印背景，仅在子应用内部生效
  .PlatformSider {
    position: relative;

    // 左侧菜单水印背景，作为侧边栏内部的绝对定位背景层
    .menuBGWaterLogo {
      pointer-events: none;
    }

    .PlatformMenuStyle {
      max-height: calc(100vh - 96px - 35px);
      overflow-y: auto;
      background-color: #043684;

      .ant-menu-sub {
        background-color: #051a5a;
      }

      .ant-menu-item-selected {
        background: linear-gradient(to right, #80c3f7, #2e72ba, #2a6cb3);
        border-radius: 5px;
      }
    }

 
  }

   // 整个滚动条区域
    ::-webkit-scrollbar {
      width: 5px;
      height: 7px; 
      background-color: rgb(202, 202, 202);
      border-radius: 5px;
    }

    // 滚动轴区域
    ::-webkit-scrollbar-thumb {
      background-color: rgb(161, 162, 162);
      border-radius: 5px;
    }

  //  .platformfootercss {
  //    text-align: right !important;
  //    height: 35px !important;
  //    line-height: 35px !important;
  //    background-color: #eef3f9 !important;
  //    bottom: 0px;
  //    right: 0px;
  //    padding: 0px !important;
  //  }
}

.page-i18n-loading-mask {
  position: fixed;
  inset: 0;
  z-index: 3000;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.45);
}
</style>
