<template>
  <div style="overflow-wrap: break-word; color: #fff">
    <div>我是vue3平台子项目</div>
    <a-button @click="revampData">revampData</a-button>
    <h2>drawerStoreMain</h2>
    {{ drawerStoreMain }}
    <h2>mainState</h2>
    {{ globalStore }}
    <h2>drawerStoreSub</h2>
    {{ drawerStoreSub }}
    <h2>navigationStoreMain</h2>
    {{ navigationStoreMain }}
  </div>
</template>

<script setup>
import { ref, watchEffect } from 'vue'
import { useGlobalState } from '../shared/useGlobalState'
const { globalStore } = useGlobalState()
import { useDrawerStore } from '../store/index'
const drawerStoreSub = useDrawerStore()

const drawerStoreMain = ref({})
const navigationStoreMain = ref({})
watchEffect(() => {
  if (globalStore.value) {
    drawerStoreMain.value = globalStore.value.drawerStore
    navigationStoreMain.value = globalStore.value.navigationStore
  }
})

//修改全局状态
const revampData = () => {
  if (!globalStore.value || !globalStore.value.drawerStore) {
    console.log('[PLATFORM Home] revampData called but no globalStore, skip')
    return
  }
  console.log('[PLATFORM Home] revampData clicked', {
    theme: globalStore.value.drawerStore.theme
  })
}
</script>
