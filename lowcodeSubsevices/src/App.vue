<template>
  <div class="SubVue3Demo">
    <layout v-if="!isQiankun && userStore.access_token" style="height: 96px; width: 100vw">
      <a-layout-header class="DemoAppHeader" :class="drawerStore.theme == 'light' ? 'lightColor' : 'darkColor'">
        <div class="HeaderContent">
          <div style="width: 220px; margin-left: 10px">
            <a-image :src="avatarImageURL" :preview="false" />
          </div>

          <div class="IconF">
            <div class="IconS" @click="showDrawer">
              <div><SettingOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''" style="font-size: 16px" /></div>
            </div>
            <div class="IconS" @click="logoff">
              <div><PoweroffOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''" style="font-size: 16px" /></div>
            </div>
            <div class="loginname">{{ userStore.loginname }}</div>
          </div>
        </div>
      </a-layout-header>
    </layout>
    <a-layout style="height: calc(100vh - 96px)" v-if="userStore.access_token">
      <a-layout-sider
        :trigger="null"
        v-model:collapsed="drawerStore.menuCollapsed"
        collapsible
        class="SubSider"
        :class="drawerStore.theme == 'light' ? 'lightColor' : 'darkColor'"
        :width="drawerStore.menuwidth"
      >
        <div class="menuBGWaterLogo" :style="menuBgStyle"></div>
        <a-menu
          class="SubMenuStyle"
          v-model:openKeys="openKeys"
          v-model:selectedKeys="selectedKeys"
          :style="{ width: drawerStore.menuwidth }"
          mode="inline"
          :theme="drawerStore.theme"
          :items="items"
          @click="menuClick"
          @select="menuSelect"
        ></a-menu>
        <div class="MenuColBtn" :class="!drawerStore.menuCollapsed ? 'RightIcon' : 'CenterIcon'">
          <MenuUnfoldOutlined class="MenuColBtnIcon" @click="toggleCollapsed" v-if="drawerStore.menuCollapsed" />
          <MenuFoldOutlined class="MenuColBtnIcon" @click="toggleCollapsed" v-else />
        </div>
      </a-layout-sider>
      <a-layout>
        <a-layout-content>
          <a-config-provider :locale="`${navigationStore.language || ''}`.toLowerCase().startsWith('zh') ? zhCN : enUS">
            <div style="height: calc(100vh - 96px - 35px)">
              <router-view v-slot="{ Component }">
                <keep-alive :include="includedComponents">
                  <component :is="Component" :key="route.path"></component>
                </keep-alive>
              </router-view>
            </div>
          </a-config-provider>
        </a-layout-content>
        <a-layout-footer class="SubFooterCss">
          <span style="margin-right: 20px">版权 © 2025 中冶京诚数字科技（北京）有限公司 all rights reserved</span></a-layout-footer
        >
      </a-layout>
    </a-layout>
    <a-layout v-else>
      <router-view></router-view>
    </a-layout>
  </div>
  <!-- 退出弹窗 -->
  <div>
    <a-modal style="height: 500px" v-model:open="exitModal" title="提示" @ok="handleOk">
      <p>确认退出吗？</p>
    </a-modal>
  </div>
  <div>
    <PublicDrawers></PublicDrawers>
  </div>
</template>

<script setup>
import { ref, watch, watchEffect, h, computed } from 'vue'
import * as Icons from '@ant-design/icons-vue'
import { useDrawerStore, useUserStore, useRouteStore, useNavigationStore } from './store/index.js'
import { useGlobalState } from './shared/useGlobalState'
import actions from './shared/actions'
import { useI18n } from 'vue-i18n'
import { useRouter, useRoute } from 'vue-router'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useInsertSysMenuRecord } from '@/api/userLoginLogs.js'
import { loadLocaleMessages } from './lang/i18n.js'

/* 获取Logo图片 */
import { MenuFoldOutlined, MenuUnfoldOutlined, createFromIconfontCN, SettingOutlined, PoweroffOutlined } from '@ant-design/icons-vue'
import PublicDrawers from '@/components/PublicDrawers.vue'
import avatarImageW from '@/assets/images/NewLogo.png'
import avatarImageB from '@/assets/images/NewLogoW.png'
import menuBgUrl from '@/assets/images/menuBG.png'
import { Layout } from 'ant-design-vue'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import enUS from 'ant-design-vue/es/locale/en_US'
import 'dayjs/locale/zh-cn'
import { message } from 'ant-design-vue'

const i18n = useI18n({ useScope: 'global' })
const normalizeLocale = (locale) => `${locale || ''}`.trim()
var isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__
const router = useRouter()
const route = useRoute()
const { globalStore } = useGlobalState()

var userStore = ref({})
var drawerStore = ref({})
var navigationStore = ref({})
var avatarImageURL = ref('')
const MyIcon = ref({})
const routeStore = useRouteStore()
if (!isQiankun) {
  userStore = useUserStore()
  drawerStore = useDrawerStore()
  navigationStore = useNavigationStore()
  avatarImageURL.value = drawerStore.theme == 'light' ? avatarImageW : avatarImageB
  /* 自定义Icon图标 */
  MyIcon.value = createFromIconfontCN({
    scriptUrl: '//at.alicdn.com/t/c/font_4403016_8cg3gq786vb.js' // 在 iconfont.cn 上生成
  })
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      drawerStore.value = globalStore.value.drawerStore
      navigationStore.value = globalStore.value.navigationStore
    }
  })
}

const menuBgStyle = computed(() => {
  const collapsed = isQiankun ? !!drawerStore.value?.menuCollapsed : !!drawerStore?.menuCollapsed
  return {
    width: collapsed ? '50px' : '120px',
    backgroundImage: `url(${menuBgUrl})`,
    backgroundSize: 'contain',
    backgroundRepeat: 'no-repeat',
    backgroundPosition: 'center',
    pointerEvents: 'none',
    position: 'absolute',
    bottom: '0px',
    left: '0px',
    height: 'calc(100vh - 180px)'
  }
})

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
  return isQiankun ? drawerStore.value : drawerStore
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
  () => route.path,
  (path) => {
    if (!path) return

     // 左侧菜单高亮始终跟随当前路由 path
    selectedKeys.value = [path]

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
// 计算属性：根据路由元信息动态决定缓存的组件
const includedComponents = computed(() => {
  // 可以根据路由的 meta 信息或其他业务逻辑来控制缓存
  // 假设路由有 meta 信息控制是否缓存
  if (route.meta.IsKeepAlive) {
    keepAliveComponents.value.push(route.meta.KeepAliveName) // 路由名称作为缓存组件
  } else {
    //判断是不是存在缓存
    var index = keepAliveComponents.value.indexOf(route.meta.KeepAliveName)
    if (index !== -1) {
      keepAliveComponents.value.splice(index, 1)
    }
  }
  // 也可以在此根据其他条件进行动态添加/删除组件
  return keepAliveComponents.value
})

const menuClick = (val) => {
  if (isQiankun) {
    useInsertSysMenuRecord({
      CreateId: userStore.value.userid,
      MenuId: val.item.menuid,
      IsSysInfoId: val.item.isSysInfo
    })
    const sysCode = import.meta.env.VITE_APP_APPNAME
    const innerPath = val.item.path
    const tabKey = sysCode ? `/${sysCode}${innerPath}` : innerPath

    if (
      navigationStore.value.tabs.length < 9 ||
      navigationStore.value.tabs.some((element) => {
        return element.key === tabKey
      })
    ) {
      router.push({ path: innerPath })
    }
  } else {
    router.push({ path: val.item.path })
  }
}

const menuSelect = (val) => {
  const sysCode = import.meta.env.VITE_APP_APPNAME
  const innerPath = val.item.path
  const tabKey = sysCode ? `/${sysCode}${innerPath}` : innerPath

  const newTab = {
    key: tabKey,
    title: val.item.title,
    path: innerPath,
    sysCode: sysCode,
    i18nKey: val.item.i18nKey
  }

  if (isQiankun) {
    const navSnapshot = navigationStore.value || {}
    const tabs = Array.isArray(navSnapshot.tabs) ? navSnapshot.tabs : []

    const exists = tabs.some((element) => {
      return element && element.key === tabKey
    })

    if (!exists) {
      if (tabs.length >= 9) {
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

    const ds = drawerStore.value
    if (ds) {
      if (typeof ds.changeSelected === 'function') {
        ds.changeSelected(val.selectedKeys)
      } else if (Array.isArray(ds.selected)) {
        ds.selected = val.selectedKeys
      }

      setDrawerFoldmenu(getFoldmenuKeysFromSelect(val))
      syncDrawerStore()
    }
  } else {
    const nav = navigationStore
    const tabs = Array.isArray(nav.tabs) ? nav.tabs : []

    const exists = tabs.some((element) => {
      return element && element.key === tabKey
    })

    if (!exists) {
      if (typeof nav.addTabs === 'function') {
        nav.addTabs(newTab)
      }
    }

    const dsLocal = drawerStore
    if (dsLocal) {
      if (typeof dsLocal.changeSelected === 'function') {
        dsLocal.changeSelected(val.selectedKeys)
      } else if (Array.isArray(dsLocal.selected)) {
        dsLocal.selected = val.selectedKeys
      }

      setDrawerFoldmenu(getFoldmenuKeysFromSelect(val))
    }
  }
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

function toggleCollapsed() {
  if (isQiankun) {
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
  } else {
    drawerStore.changeCollapsed()
    if (!drawerStore.menuCollapsed) {
      drawerStore.setMenuWidthByCollapsedFalse()
      openKeys.value = getFoldmenuKeys()
    } else {
      drawerStore.setMenuWidthByCollapsed()
      openKeys.value = []
    }
  }
}

// 动态映射图标
const getIconComponent = (iconName) => {
  return Icons[iconName] ? Icons[iconName] : Icons['QuestionCircleOutlined'] // 提供一个默认图标
}

// 统一处理菜单标题：优先用多语言，失败时回退到原始 name
const getMenuTitle = (route) => {
  const fallbackTitle = route.originalName || route.name || ''
  const key = route.i18nKey
  if (!key) return fallbackTitle
  const translated = i18n.t(key)
  return translated && translated !== key ? translated : fallbackTitle
}

//构造菜单Item数据
const generateMenuData = (routes) => {
  return routes.map((route) => {
    const menuItem = {
      menuid: route.id,
      isSysInfo: route.isSysInfo,
      key: route.path || route.name,
      label: getMenuTitle(route),
      title: getMenuTitle(route), //route.name
      path: route.path,
      i18nKey: route.i18nKey
    }
    if (route.icon) {
      // 在你的动态菜单项配置中使用
      menuItem.icon = () => {
        const IconComponent = getIconComponent(route.icon) // 获取对应的图标组件
        return h(IconComponent)
      }
    }
    if (route.children && route.children.length > 0) {
      menuItem.children = generateMenuData(route.children)
    }
    return menuItem
  })
}

var items = ref([])
const hasAccessToken = () => {
  if (isQiankun) {
    return !!userStore.value?.access_token
  }
  return !!userStore.access_token
}

const getCurrentLanguage = () => {
  if (isQiankun) {
    return normalizeLocale(navigationStore.value?.language || i18n.locale.value || 'zh')
  }
  return normalizeLocale(navigationStore.language || i18n.locale.value || 'zh')
}

const refreshStandaloneLocaleMessages = async (language, shouldReloadRoutes = false) => {
  if (isQiankun || !userStore.access_token) {
    return
  }

  if (shouldReloadRoutes) {
    await routeStore.loadRoutes(userStore.userRoles, language)
  }

  await loadLocaleMessages(i18n, language, {
    includeCommonMessages: true,
    includeRouteMessages: true,
    routeSourceData: routeStore.permissionSystemsCache
  })
}

async function loadMenuItems() {
  // 仅在本地还没有路由时才向后端请求，避免语言切换时重复 loadRoutes 导致卡顿
  const hasRoutes = Array.isArray(routeStore.routes) && routeStore.routes.length > 0
  const currentLanguage = getCurrentLanguage()
  if (!hasRoutes) {
    if (!isQiankun) {
      if (!userStore.access_token) {
        return
      }
      await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
    } else {
      if (!userStore.value.access_token) {
        return
      }
      await routeStore.loadRoutes(userStore.value.userRoles, currentLanguage)
    }
  }
  const buildRouteSource = routeStore.routes.filter((item) => item.path == `/${import.meta.env.VITE_APP_APPNAME}`)
  const matchedItem = buildRouteSource.length > 0 ? buildRouteSource[0].children : []
  items.value = generateMenuData(matchedItem)
}

loadMenuItems()

/* 监听语言变化重新加载Menu菜单 */
if (!isQiankun) {
  watch(
    () => navigationStore.language,
    async (language, previousLanguage) => {
      if (!language || language === previousLanguage) return
      const normalizedLanguage = normalizeLocale(language)
      i18n.locale.value = normalizedLanguage
      await refreshStandaloneLocaleMessages(normalizedLanguage, true)
      await loadMenuItems()
    }
  )

  watch(
    () => userStore.access_token,
    async (token) => {
      if (!token) return
      const currentLanguage = getCurrentLanguage()
      await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
      await refreshStandaloneLocaleMessages(currentLanguage)
      await loadMenuItems()
    }
  )

  // 当动态路由（menu 权限）加载完成后，自动刷新左侧菜单
  watch(
    () => routeStore.routes,
    (newRoutes) => {
      if (!Array.isArray(newRoutes) || newRoutes.length === 0) return
      loadMenuItems()
    },
    { deep: true }
  )

  /* 监听主题变化加载背景图片 */
  watch(
    () => drawerStore.theme,
    () => {
      avatarImageURL.value = drawerStore.theme == 'dark' ? avatarImageW : avatarImageB
    }
  )
} else {
  watch(
    () => globalStore.value && globalStore.value.userStore && globalStore.value.userStore.access_token,
    async (token) => {
      if (!token) return
      const lang = normalizeLocale(
        (globalStore.value && globalStore.value.navigationStore && globalStore.value.navigationStore.language) || i18n.locale.value || 'zh'
      )
      const qiankunRoles =
        (globalStore.value && globalStore.value.userStore && globalStore.value.userStore.userRoles) ||
        (userStore.value && userStore.value.userRoles) ||
        []
      const routeSourceData = await routeStore.loadRoutes(qiankunRoles, lang)
      await loadLocaleMessages(i18n, lang, {
        includeCommonMessages: true,
        includeRouteMessages: true,
        routeSourceData
      })
      await loadMenuItems()
    },
    { immediate: true }
  )

  // 在 qiankun 场景下监听主应用 navigationStore.language，同步子应用语言和菜单
  watch(
    () =>
      globalStore.value && globalStore.value.navigationStore && globalStore.value.navigationStore.language,
    async (lang) => {
      if (!lang) return
      const normalizedLanguage = normalizeLocale(lang)
      i18n.locale.value = normalizedLanguage
      if (hasAccessToken()) {
        const qiankunRoles =
          (globalStore.value && globalStore.value.userStore && globalStore.value.userStore.userRoles) ||
          (userStore.value && userStore.value.userRoles) ||
          []
        const routeSourceData = await routeStore.loadRoutes(qiankunRoles, normalizedLanguage)
        await loadLocaleMessages(i18n, normalizedLanguage, {
          includeCommonMessages: true,
          includeRouteMessages: true,
          routeSourceData
        })
      }
      await loadMenuItems()
    },
    { immediate: true }
  )
}

// 打开设置抽屉
const showDrawer = () => {
  if (isQiankun) {
    if (drawerStore.value && typeof drawerStore.value.setOpen === 'function') {
      drawerStore.value.setOpen(true)
    }
  } else {
    if (drawerStore && typeof drawerStore.setOpen === 'function') {
      drawerStore.setOpen(true)
    }
  }
}

const exitModal = ref(false)
const logoff = () => {
  exitModal.value = true
}

const handleOk = () => {
  const mainRouter = useRouteStore()
  navigationStore.removeAllTab()
  exitModal.value = false
  userStore.removeUserMessage()
  mainRouter.clearnRoutes()
  router.push({ path: '/Login' })
  drawerStore.AddSelectFirstMenuPath(['/'])
}
</script>
<style lang="scss">
.SubVue3Demo {
  .SubSider {
    // 整个滚动条区域
    ::-webkit-scrollbar {
      width: 5px;
      background-color: rgb(161, 162, 162);
      border-radius: 5px;
    }

    // 滚动轴区域
    ::-webkit-scrollbar-thumb {
      background-color: rgb(202, 202, 202);
      border-radius: 5px;
    }
  }

  // .tabsClass {
  //   .ant-tabs-nav {
  //     margin: 0px !important;
  //   }
  // }

  .DemoAppHeader {
    padding-inline: 0px !important;
    height: 96px;
    line-height: 96px;

    .HeaderContent {
      width: 100vw;
      display: flex;
      justify-content: space-between;
      z-index: 99;
      background-image: url('/src/assets/images/HeaderBG.png');
    }

    .IconF {
      display: flex;
      margin-right: 20px;
      .IconS {
        text-align: center;
        width: 40px;
      }
      .IconS:hover {
        background-color: rgb(230, 229, 229);
        cursor: pointer;
      }

      .loginname {
        font-size: 18px;
        font-weight: 700;
        text-align: center;
        line-height: 96px;
        margin: 0 10px;
        cursor: pointer;
        color: #fff;
        width: 130px;
      }
    }
  }
}
</style>
