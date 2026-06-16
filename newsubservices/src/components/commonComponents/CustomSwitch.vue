<template>
  <!-- <a-switch v-for="(item, index) in btnList" v-model:checked="checked"> -->
    <a-switch :disabled="disabled"  v-model:checked="checked">
    <!-- <span v-if="navigationStore.language == 'zh' && item.functionName && !IsOnlyIcon">{{ item.functionName }}</span>
    <span v-if="navigationStore.language == 'en' && item.englishName && !IsOnlyIcon">{{ item.englishName }}</span> -->
  </a-switch>
</template>

<script setup>
import { ref, h, onMounted,watch } from 'vue'
import { getButtonList } from '../../api/commonFun'
import * as Icons from '@ant-design/icons-vue'
import { useNavigationStore } from '../../store/navigation'

const props = defineProps({
  ParamsRoleId: Array,
  ParamsFunctionName: String,
  BtnFunctionNameArray: Array,
  IsOnlyIcon: Boolean,
  IsStart: Boolean
})

const disabled = ref(true);
var checked = props.IsStart

watch(
  () => props.IsStart,
  (New, Old) => {
    checked = New
  },
  { deep: true }
)

const emit = defineEmits(['buttonClick'])

const navigationStore = useNavigationStore()
const btnList = ref([])

//获取按钮列表
getAllButton()
onMounted(() => {})


async function getAllButton() {
  const obj = {
    menuCode: props.ParamsFunctionName,
    roleids: props.ParamsRoleId,
    btnCodes: props.BtnFunctionNameArray
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      disabled.value=false
      btnList.value = res.data.buttonDtos
    }
  })
}
</script>
<style lang="less"></style>
