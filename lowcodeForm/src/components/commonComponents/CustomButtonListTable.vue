<template>
  <a-button
    :key="ObjItem.id"
    :type="ObjItem.icon == 'DeleteOutlined' || ObjItem.icon == 'PlusOutlined' || ObjItem.icon == 'SaveOutlined' ? 'primary' : 'default'"
    :icon="getIconComponent(ObjItem.icon)"
    :danger="ObjItem.icon == 'DeleteOutlined'"
  >
    <span v-if="ObjItem.functionName && !IsOnlyIcon">{{ ObjItem.functionName }}</span>
  </a-button>
</template>

<script setup>
import { h } from 'vue'
import * as Icons from '@ant-design/icons-vue'
const props = defineProps({
  ObjItem: Object, //传入的按钮权限对象
  IsOnlyIcon: Boolean //是否只显示图标
})

const getIconComponent = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}
</script>
<style lang="less"></style>
