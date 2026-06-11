<template>
  <BasicModal v-bind="$attrs" @register="registerModal" title="输出设置" okText="下载代码" @ok="handleSubmit">
    <template #insertFooter>
      <a-space :size="10" style="float: left">
        <a-button @click="handlePreview">代码预览</a-button>
        <a-button @click="handleAlias" v-if="webType != 4">命名规范</a-button>
      </a-space>
    </template>
    <a-alert message="注意：以下只能包含数字、字母、下划线、小圆点，不能用数字开头，不能是关键字或保留字。" type="warning" showIcon :style="{ marginBottom: '18px' }" />
    <a-form :colon="false" :labelCol="{ style: { width: '100px' } }" :model="dataForm" ref="formElRef">
      <a-form-item label="模块命名" name="module" v-if="type != 3" :rules="[{ required: true, message: '必填', trigger: 'change' }]">
        <ceri-select v-model:value="dataForm.module" :options="moduleOptions" placeholder="请选择" showSearch />
      </a-form-item>
      <a-form-item name="modulePackageName" :rules="[{ required: true, message: '必填', trigger: 'blur' }]" v-if="hasPackage">
        <template #label>模块包名<BasicHelp text="修改包名需要调整controller和mapper扫描配置" /></template>
        <a-input v-model:value="dataForm.modulePackageName" placeholder="请输入" />
      </a-form-item>
      <a-form-item label="功能描述" name="description" :rules="[{ required: true, message: '必填', trigger: 'blur' }]">
        <a-input v-model:value="dataForm.description" placeholder="请输入" />
      </a-form-item>
      <a-form-item label="流程模板" name="enableFlow" v-if="webType != 4">
        <ceri-switch v-model:value="dataForm.enableFlow" />
      </a-form-item>
    </a-form>
  </BasicModal>
  <PreviewModal @register="registerPreviewModal" />
  <AliasModal @register="registerAliasModal" />
</template>
<script lang="ts" setup>
import { reactive, ref, toRefs, nextTick } from 'vue'
import { downloadCode } from '@/api/onlineDev/visualDev'
import { BasicModal, useModal, useModalInner } from '@/components/Modal'
import type { FormInstance } from 'ant-design-vue'
import { downloadByUrl } from '@/utils/file/download'
import { useBaseStore } from '@/store/base'
import PreviewModal from './PreviewModal.vue'
import AliasModal from './AliasModal.vue'

interface State {
  dataForm: any
  moduleOptions: any[]
  tables: any[]
  type: number
  id: string
  hasPackage: boolean
  fullName: string
  webType: number
}

defineEmits(['register'])
const baseStore = useBaseStore()
const [registerModal, { changeOkLoading, closeModal }] = useModalInner(init)
const [registerPreviewModal, { openModal: openPreviewModal }] = useModal()
const [registerAliasModal, { openModal: openAliasModal }] = useModal()
const formElRef = ref<FormInstance>()
const state = reactive<State>({
  dataForm: {
    module: '',
    modulePackageName: 'ceri',
    description: '',
    enableFlow: 0
  },
  moduleOptions: [],
  tables: [],
  type: 0,
  id: '',
  hasPackage: false,
  fullName: '',
  webType: 2
})
const { dataForm, type, moduleOptions, hasPackage, webType } = toRefs(state)

function init(data) {
  state.tables = data.tables ? data.tables : []
  state.id = data.id
  state.type = data.type || 0
  state.hasPackage = !!data.hasPackage
  state.fullName = data.fullName || ''
  state.webType = data.webType || 2
  state.dataForm.description = ''
  state.dataForm.enableFlow = 0
  getOptions()
  nextTick(() => {
    formElRef.value?.clearValidate()
    if (data.webType == 4) return
    const mainTable = state.tables.length ? state.tables.filter((o) => o.typeId == '1')[0].tableName : ''
    state.dataForm.description = mainTable
  })
}
function getOptions() {
  baseStore.getDictionaryData('BuildModule').then((res) => {
    state.moduleOptions = res as any
    if (state.moduleOptions.length) state.dataForm.module = state.moduleOptions[0].id
  })
}
async function handleSubmit() {
  try {
    const values = await formElRef.value?.validate()
    if (!values) return
    changeOkLoading(true)
    downloadCode(state.id, state.dataForm)
      .then((res) => {
        downloadByUrl({ url: res.data.url })
        closeModal()
      })
      .catch(() => {
        changeOkLoading(false)
      })
  } catch (_) {}
}
function handlePreview() {
  openPreviewModal(true, { id: state.id, fullName: state.fullName, ...state.dataForm })
}

function handleAlias() {
  openAliasModal(true, { id: state.id, fullName: state.fullName })
}
</script>
