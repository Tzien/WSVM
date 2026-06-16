<template>
  <a-button v-for="(item, index) in btnList" :key="item.id"
    :type="item.icon == 'SearchOutlined' || item.icon == 'DeleteOutlined' || item.icon == 'PlusOutlined' || item.icon == 'SaveOutlined' ? 'primary' : 'default'"
    :icon="getIconComponent(item.icon)" :danger="item.icon == 'DeleteOutlined'"
    @click="handleClick(item.buttonActionName)" style="margin: 0px 5px">
    <span v-if="!IsOnlyIcon">{{ item.functionName }}</span>
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
var navigationStoreLanguage = ref('zh')
if (!isQiankun) {
  navigationStore = useNavigationStore()
  navigationStoreLanguage.value = navigationStore.language
} else {
  watchEffect(() => {
    if (globalStore.value) {
      navigationStore.value = globalStore.value.navigationStore
      navigationStoreLanguage.value = navigationStore.value.language
    }
  })
}
const props = defineProps({
  ParamsRoleId: Array,
  ParamsFunctionName: String,
  BtnFunctionNameArray: Array,
  IsOnlyIcon: Boolean
})
const emit = defineEmits(['buttonClick'])

const btnList = ref([])



// 使用 watchEffect 自动追踪 navigationStoreLanguage 的变化
watchEffect(() => {
  if (navigationStoreLanguage.value) {
    //获取按钮列表
    getAllButton()
  }
})

onMounted(() => { })

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
    btnCodes: props.BtnFunctionNameArray,
    langCode: navigationStoreLanguage.value
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      btnList.value = res.data.buttonDtos
    }
  })
}
</script>
<style lang="less"></style>
