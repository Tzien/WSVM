<template>
  <a-button
    :key="ObjItem.id"
    :type="ObjItem.icon == 'DeleteOutlined' || ObjItem.icon == 'PlusOutlined' || ObjItem.icon == 'SaveOutlined' ? 'primary' : 'default'"
    :icon="getIconComponent(ObjItem.icon)"
    :danger="ObjItem.icon == 'DeleteOutlined'"
  >
    <span v-if="navigationStore.language == 'zh' && ObjItem.functionName && !IsOnlyIcon">{{ ObjItem.functionName }}</span>
    <span v-if="navigationStore.language == 'en' && ObjItem.englishName && !IsOnlyIcon">{{ ObjItem.englishName }}</span>
  </a-button>
</template>

<script setup>
import { ref, h, watchEffect } from 'vue'
import * as Icons from '@ant-design/icons-vue'
import { useNavigationStore } from '../../store/navigation'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useGlobalState } from '@/shared/useGlobalState'

var isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__
const { globalStore } = useGlobalState()

var navigationStore = ref({})
if (!isQiankun) {
  navigationStore = useNavigationStore()
} else {
  watchEffect(() => {
    if (globalStore.value) {
      navigationStore.value = globalStore.value.navigationStore
    }
  })
}
const props = defineProps({
  ObjItem: Object, //传入的按钮权限对象
  IsOnlyIcon: Boolean //是否只显示图标
})

const getIconComponent = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}
</script>
<style lang="less"></style>
