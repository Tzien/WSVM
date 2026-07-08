import { defineStore } from 'pinia'
import { ref } from 'vue'
import { message } from 'ant-design-vue'

export const useNavigationStore = defineStore(
  'navigation',
  () => {
    //语言
    const language = ref('zh')
    const setLanguage = (newLanguage) => {
      language.value = newLanguage
    }
    //面包屑
    const bread = ref()
    const setBread = (newBread) => {
      bread.value = newBread
    }
    //标签页
    const tabs = ref([
      // {
      //   key: '/',
      //   title: '综合首页',
      //   path: '/',
      //   i18nKey: 'message.route.ComprehensiveHomePage'
      // }
    ])
    const MAX_TABS = 10
    //添加标签页（按 key 去重）
    const addTabs = (newTab) => {
      if (!newTab || !newTab.key) return
      if (tabs.value.some((tab) => tab.key === newTab.key)) {
        return
      }
      if (tabs.value.length >= MAX_TABS) {
        message.warn('最多打开10个选项卡，请关闭一些后重试')
        return
      }
      tabs.value.push(newTab)
    }
    //根据key删除Tab
    const removeTab = (delkey) => {
      tabs.value = tabs.value.filter((item) => item.key != delkey)
    }
    //删除所有的Tab
    const removeAllTab = () => {
      tabs.value = [
        // {
        //   key: '/',
        //   title: '综合首页',
        //   path: '/',
        //   i18nKey: 'message.route.ComprehensiveHomePage'
        // }
      ]
    }
    // const I18nMessages = ref([])
    // //存储主应用获取到的I18nMessage国际化数据
    // const setI18nMessage = (newMessages) => {
    //   I18nMessages.value = newMessages
    // }

    return {
      bread,
      setBread,
      language,
      setLanguage,
      tabs,
      addTabs,
      removeTab,
      removeAllTab
      // I18nMessages,
      // setI18nMessage
    }
  },
  {
    persist: true
  }
)
