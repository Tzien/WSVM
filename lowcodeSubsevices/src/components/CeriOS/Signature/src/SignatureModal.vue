<template>
  <a-modal v-model:open="visible" title="选择签章" class="ceri-signature-modal" centered :maskClosable="false" :keyboard="false" :width="600">
    <template #closeIcon>
      <ModalClose :canFullscreen="false" @cancel="handleCancel" />
    </template>
    <template #footer>
      <a-button @click="handleCancel">取消</a-button>
      <a-button type="primary" @click="handleSubmit">确定</a-button>
    </template>
    <div class="sign-main">
      <a-form :colon="false" :labelCol="{ style: { width: '80px' } }" :model="dataForm" ref="formElRef">
        <a-form-item label="电子签章" name="innerValue">
          <ceri-select v-model:value="dataForm.innerValue" :options="options" showSearch allowClear />
        </a-form-item>
      </a-form>
    </div>
  </a-modal>
</template>
<script lang="ts" setup>
import { getListApi } from '@/api/signature'
import { Modal as AModal } from 'ant-design-vue'
import { reactive, ref, toRefs ,onMounted,watchEffect} from 'vue'
import ModalClose from '@/components/Modal/src/components/ModalClose.vue'
import { useI18n } from '@/hooks/web/useI18n'
import type { FormInstance } from 'ant-design-vue'



interface State {
  dataForm: any
  visible: boolean
  options: any[]
}
const emit = defineEmits(['register', 'confirm'])
defineExpose({ openModal })
const { t } = useI18n()
const formElRef = ref<FormInstance>()
const state = reactive<State>({
  dataForm: {
    name:'',
    innerValue: ''
  },
  visible: false,
  options: []
})
const { dataForm, visible, options } = toRefs(state)

async function openModal(data) {
  state.visible = true
  state.dataForm.name = data.name || ''
  if (!data.ableIds || !data.ableIds.length) return
  // getListByIds(data.ableIds).then(res => {
  //   const list = res?.data?.list || [];
  //   state.options = list.map(o => ({ id: o.icon, fullName: o.fullName }));
  // });
  
}
onMounted(() => {
  var obj = {
    name: '',
    userIds: '',
    pageIndex: 1,
    pageSize: 99
  }
  getListApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      console.info(111)
      state.options = res.data.map(o => ({ id: o.signatureId, fullName: o.name,src:o.img }));
    }
  })
})
function handleCancel() {
  state.visible = false
}
async function handleSubmit() {
  const values = await formElRef.value?.validate()
  if (!values) return
  if(state.dataForm.innerValue)
    emit('confirm', state.options.filter(m=>m.id==state.dataForm.innerValue)[0])
  else 
    emit('confirm', '')
  // emit('confirm', values.innerValue || '')
  handleCancel()
}
</script>
<style lang="less">
.ceri-signature-modal {
  .ant-modal-body {
    padding: 40px 50px !important;
  }
}
</style>
