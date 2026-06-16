<template>
  <a-button
    v-for="(item, index) in btnList"
    :key="item.id"
    :type="isChecked && item.icon === 'FilterOutlined' ? 'primary' : ''"
    :icon="getIconComponent(item.icon)"
    shape="circle"
    @click="handleClick(item.buttonActionName)"
    style="margin: 0px 5px"
  >
  </a-button>
</template>

<script setup>
import { ref, h, onMounted, watchEffect } from 'vue'
import { getButtonList } from '../../api/commonFun'
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
  ParamsRoleId: Array,
  ParamsFunctionName: String,
  BtnFunctionNameArray: Array,
  isChecked: Boolean
})
const emit = defineEmits(['buttonClick'])

const btnList = ref([])

//获取按钮列表
getAllButton()
onMounted(() => {})

const getIconComponent = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}

const handleClick = (functionName) => {
  // 触发自定义事件，传递 functionName 给父组件
  emit('buttonClick', functionName)
}

async function getAllButton() {
  const obj = {
    menuCode: props.ParamsFunctionName,
    roleids: props.ParamsRoleId,
    btnCodes: props.BtnFunctionNameArray
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      btnList.value = res.data.buttonDtos
    }
  })
}
</script>
<style lang="less"></style>
