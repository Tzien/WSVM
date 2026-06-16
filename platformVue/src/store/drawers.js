import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useDrawerStore = defineStore(
  'drawersSub',
  () => {
    //抽屉是否开启
    const open = ref(false)
    const setOpen = (newOpen) => {
      open.value = newOpen
    }
    //皮肤颜色
    const theme = ref('bluessss')
    const setTheme = (newTheme) => {
      theme.value = newTheme
    }
    //菜单样式
    const menumode = ref('inline')
    const chengeMode = (newMode) => {
      menumode.value = newMode
    }
    //基于菜单宽度
    var menuwidth = ref(200)
    var menuwidthCopy = ref(200)
    const setMenuWidth = (newVal) => {
      menuwidth.value = newVal
      menuwidthCopy.value = newVal
    }
    //菜单收起宽度自动调整为80
    const setMenuWidthByCollapsed = () => {
      menuwidth.value = 80
    }
    const setMenuWidthByCollapsedFalse = () => {
      menuwidth.value = menuwidthCopy.value
    }
    //菜单是否收起
    const menuCollapsed = ref(false)
    const changeCollapsed = () => {
      menuCollapsed.value = !menuCollapsed.value
    }
    //当前菜单选中
    const selected = ref(['/'])
    const changeSelected = (newSelected) => {
      selected.value = newSelected
    }
    //当前菜单展开
    const foldmenu = ref([])
    const setFoldmenu = (newFoldmenu) => {
      foldmenu.value = newFoldmenu
    }
    //面包屑的显示与隐藏
    const displaybread = ref(false)
    const setBread = (newbread) => {
      displaybread.value = newbread
    }
    //存储选中一级菜单
    var selectFirstMenuPath = ref(['/'])
    const AddSelectFirstMenuPath = (pathName) => {
      selectFirstMenuPath.value = pathName
    }
    //同步主应用pinia实例
    const SyncMainDrawer = (mainDrawer) => {
      open.value = mainDrawer.open
      theme.value = mainDrawer.theme
    }

    //是否添加了动态路由
    // var isDynamicRoutesAdded = ref(false)
    // const changeDRAdded = (state) => {
    //   isDynamicRoutesAdded.value = state
    // }
    return {
      open,
      setOpen,
      theme,
      setTheme,
      menumode,
      chengeMode,
      menuwidth,
      setMenuWidth,
      menuwidthCopy,
      setMenuWidthByCollapsed,
      setMenuWidthByCollapsedFalse,
      menuCollapsed,
      changeCollapsed,
      selected,
      changeSelected,
      foldmenu,
      setFoldmenu,
      displaybread,
      setBread,
      selectFirstMenuPath,
      AddSelectFirstMenuPath,
      // isDynamicRoutesAdded,
      // changeDRAdded,
      SyncMainDrawer
    }
  },
  {
    persist: true
  }
)
