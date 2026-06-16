<template>
  <div style="overflow-wrap: break-word; color: black;overflow: auto;height: calc(100vh - 96px - 35px);">
    <div>我是vue3子项目Demo</div>
    <h2>drawerStoreMain</h2>
    <pre>{{ drawerStoreMainInfo }}</pre>
    <h2>globalState</h2>
    <pre>{{ globalStateInfo }}</pre>
    <h2>drawerStoreSub</h2>
    <pre>{{ drawerStoreSubInfo }}</pre>
  </div>
</template>

<script setup>
import { ref, watchEffect, computed, unref, toRaw } from 'vue'
import { useDrawerStore } from '../store/index'
const drawerStoreSub = useDrawerStore().$state
import { useGlobalState } from '../shared/useGlobalState'
const { globalStore } = useGlobalState()
const drawerStoreMain = ref(null)
const globalState = ref(null)

const describeValue = (value) => {
  const v = toRaw(unref(value))
  if (v == null) return { type: String(v), keys: [] }
  if (Array.isArray(v)) return { type: 'Array', length: v.length }
  if (typeof v === 'object') return { type: 'Object', keys: Object.keys(v) }
  return { type: typeof v, value: v }
}

const drawerStoreMainInfo = computed(() => describeValue(drawerStoreMain.value))
const globalStateInfo = computed(() => describeValue(globalState.value))
const drawerStoreSubInfo = computed(() => describeValue(drawerStoreSub))
//获取全局状态
watchEffect(() => {
  if (globalStore.value) {
    drawerStoreMain.value = globalStore.value.drawerStore
    globalState.value = globalStore.value
  }
})
</script>
