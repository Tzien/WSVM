import { defineStore } from 'pinia'
import { ref } from 'vue'

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
    const tabs = ref([])
    //添加标签页
    const addTabs = (newTab) => {
      tabs.value.push(newTab)
    }
    //根据key删除Tab
    const removeTab = (delkey) => {
      tabs.value = tabs.value.filter((item) => item.key != delkey)
    }
    //删除所有的Tab
    const removeAllTab = () => {
      tabs.value = []
    }
    return {
      bread,
      setBread,
      language,
      setLanguage,
      tabs,
      addTabs,
      removeTab,
      removeAllTab,
    }
  },
  {
    persist: true,
  }
)
