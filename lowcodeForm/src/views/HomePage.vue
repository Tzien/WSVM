<template>
  <div style="overflow-wrap: break-word; color: black">
    <div>我是低代码Demo</div>
    <h2>drawerStoreMain</h2>
    {{ drawerStoreMain }}
    <h2>globalState</h2>
    {{ globalState }}
    <h2>drawerStoreSub</h2>
    {{ drawerStoreSub }}
  </div>
</template>

<script setup>
import { ref, watchEffect } from 'vue'
import { useDrawerStore } from '../store/index'
const drawerStoreSub = useDrawerStore().$state
import { useGlobalState } from '../shared/useGlobalState'
const { globalStore } = useGlobalState()
const drawerStoreMain = ref(null)
const globalState = ref(null)
//获取全局状态
watchEffect(() => {
  if (globalStore.value) {
    drawerStoreMain.value = globalStore.value.drawerStore
    globalState.value = globalStore.value
  }
})
</script>
