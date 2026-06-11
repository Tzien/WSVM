<template>
  <div>
    <a-drawer root-class-name="WebDegisnStepDrawer" :open="isShowStepDrawer" width="100%" :closable="false" @close="onClose" placement="right">
      <template #title>
        <div class="StepDesignContainerTop">
          <div class="StepDesignContainerStep">
            <a-steps v-model:current="activeStep" type="navigation" size="small" class="header-steps">
              <a-step title="基础设计" />
              <a-step title="表单设计" :disabled="activeStep == 1" />
              <a-step title="列表设计" :disabled="activeStep == 2" />
            </a-steps>
          </div>
          <div class="StepDesignContainerRight">
            <a-button @click="toggleWebType(1)" style="background-color: rgb(250, 173, 20); color: #fff" v-show="activeStep == 2 && dataForm.webType == 2">
              <!-- {{ t('common.closeList') }} -->
              关闭列表
            </a-button>
            <ValidatePopover ref="validatePopoverRef" :errorList="errorList" @select="handleSelect" v-if="activeStep == 1" />
            <a-button v-if="activeStep > 0" style="margin-left: 10px" @click="prev">上一步</a-button>
            <a-button v-if="activeStep < 2" type="primary" style="margin-left: 10px" @click="next()">下一步</a-button>
            <a-button type="primary" style="margin-left: 10px" @click="handleSubmit()"> 保存 </a-button>
            <a-button style="margin-left: 10px" @click="onClose">关闭</a-button>
          </div>
        </div>
      </template>
      <div style="background-color: rgb(234, 236, 240); height: 100%; padding: 10px 0px; display: flex; justify-content: center">
        <div v-show="activeStep == 0" style="background-color: #fff; border-radius: 5px; height: 100%; width: 45%">
          <StepBasic ref="stepBasicRef" :categoryOptions="categoryOptions"></StepBasic>
        </div>
        <div v-show="activeStep == 1" style="border-radius: 5px; height: 100%; width: 100%">
          <StepForm ref="setpFormRef" :conf="formData" :formInfo="dataForm" :dbType="dbType"></StepForm>
        </div>
        <div v-if="activeStep == 2" style="background-color: #fff; border-radius: 5px; height: 100%; width: calc(100vw - 20px)">
          <StepTable ref="stepTableRef" :columnData="columnData" :appColumnData="appColumnData" :formInfo="dataForm" @toggleWebType="toggleWebType"></StepTable>
        </div>
      </div>
    </a-drawer>
  </div>
</template>
<script setup>
import { onMounted, reactive, ref, toRefs, unref, watch, nextTick } from 'vue'
import { message } from 'ant-design-vue'
import { StepBasic } from './StepBasic'
import { StepForm } from './StepForm'
import { StepTable } from './StepTable'
// import { BasicModal, useModal, useModalInner } from '@/components/Modal'
import { ValidatePopover } from '@/components/CommonModal'
import { create, update } from '@/api/demoApi/demo'
import { useGeneratorStore } from '@/store/generator'
// import { getDataSourceSelector } from '@/api/systemData/dataSource'
import { useMessage } from '@/hooks/web/useMessage'
import { useUserStore } from '@/store'
const { createMessage, createConfirm } = useMessage()

const userStore = useUserStore()
const generatorStore = useGeneratorStore()

// const [registerTableModal, { openModal: openTableModal }] = useModal()

const props = defineProps({
  isShowStepDrawer: {
    type: Boolean
  },
  categoryOptions: {
    type: Object,
    default: []
  },
  rowData: {
    type: Object,
    default: null
  }
})

const state = reactive({
  activeStep: 0,
  maxStep: 0,
  dbOptions: [],
  relationTable: false,
  mainTableFields: [],
  dataForm: {
    formDesignId: '',
    name: '',
    code: '',
    formCategoryId: null,
    sort: 0,
    remark: '',
    dbId: '0',
    formJson: '',
    tableJson: '',
    status: 1,
    webType: 2, //列表表单2，纯表单1
    columnDataStr: '',
    appColumnDataStr: '',
    formDbs: []
  },
  formData: null,
  dbType: 'MySQL',
  tables: [],
  defaultTable: [],
  errorList: [],
  columnData: null,
  appColumnData: null
})

function resetState() {
  state.activeStep = 0
  state.maxStep = 0
  state.dbOptions = []
  state.relationTable = false
  state.mainTableFields = []
  state.dataForm = {
    formDesignId: '',
    name: '',
    code: '',
    formCategoryId: null,
    sort: 0,
    remark: '',
    dbId: '0',
    formJson: '',
    tableJson: '',
    status: 1,
    webType: 2,
    columnDataStr: '',
    appColumnDataStr: '',
    formDbs: []
  }
  state.formData = null
  state.dbType = 'MySQL'
  state.tables = []
  state.defaultTable = []
  state.errorList = []
  state.columnData = null
  state.appColumnData = null

  init()
}

const { activeStep, dbType, formData, dataForm, tables, errorList, columnData, appColumnData } = toRefs(state)

function handleSelect(id) {
  unref(setpFormRef)?.setTabActiveKey(id == 'formAttr' ? 'form' : 'field')
  unref(setpFormRef)?.activeFormItemById(id)
}

const stepBasicRef = ref(null)
const setpFormRef = ref(null)
const stepTableRef = ref(null)
const validatePopoverRef = ref(null)
onMounted(() => {})

watch(
  () => props.isShowStepDrawer,
  (newVal) => {
    if (newVal) {
      nextTick(() => {
        stepBasicRef.value?.getConnectListData()
        resetState()
      })
    }
  },
  { immediate: true }
)

function resetFields() {
  stepBasicRef.value?.resetFields()
}

async function init() {
  state.activeStep = 0
  state.tables = []
  state.defaultTable = []
  state.errorList = []
  state.formData = null
  state.columnData = null
  state.appColumnData = null
  getDbOptions()
  resetFields()
  state.dataForm.formDesignId = props.rowData?.formDesignId
  if (state.dataForm.formDesignId) {
    state.maxStep = props.rowData.maxStep
    const { name, code, formCategoryId, sort, remark, dbId, formJson, tableJson, status, webType, columnDataStr, appColumnDataStr, formDbs } = props.rowData
    Object.assign(state.dataForm, { name, code, formCategoryId, sort, remark, dbId, formJson, tableJson, status, webType, columnDataStr, appColumnDataStr })
    state.dataForm.formDbs = Array.isArray(formDbs) ? formDbs : []
    if (state.dataForm.formDbs.length) {
      generatorStore.setHasTable(true)
      generatorStore.setAllTable(state.dataForm.formDbs)
      generatorStore.setSubTable(state.dataForm.formDbs.filter((o) => o && o.typeId == '0'))
      generatorStore.setFormItemList([])
    } else {
      generatorStore.setHasTable(false)
      generatorStore.setAllTable([])
      generatorStore.setSubTable([])
      generatorStore.setFormItemList([])
    }
    /* 表单赋值 */
    stepBasicRef.value?.setFormData(props.rowData)
    //设计器赋值
    state.formData = state.dataForm.formJson && JSON.parse(state.dataForm.formJson)
    //table赋值
    state.columnData = state.dataForm.columnDataStr && JSON.parse(state.dataForm.columnDataStr)
    state.appColumnData = state.dataForm.appColumnDataStr && JSON.parse(state.dataForm.appColumnDataStr)

    if (state.maxStep > 0) {
      await nextTick()
      await next()
    }
  } else {
    // state.dataForm.type = data.type
    // state.dataForm.webType = data.webType || 2
    // state.maxStep = state.dataForm.webType == 4 ? 1 : 2
    // state.loading = false
  }
}

function getDbOptions() {
  // getDataSourceSelector().then((res) => {
  //   let list = res.data.list || []
  //   list = list.filter((o) => o.children && o.children.length)
  //   if (list[0] && list[0].children && list[0].children.length) list[0] = list[0].children[0]
  //   delete list[0].children
  //   state.dbOptions = list
  //   // updateSchema([{ field: 'dbLinkId', componentProps: { options: state.dbOptions } }])
  // })
}

function toggleWebType(type) {
  createConfirm({
    iconType: 'warning',
    title: '提示',
    content: type == '1' ? '关闭后，将切换为纯表单模式' : '开启后，将切换为表单+列表模式',
    onOk: () => {
      state.dataForm.webType = type
    }
  })
}

function getDbType() {
  for (let i = 0; i < state.dbOptions.length; i++) {
    const item = state.dbOptions[i]
    if (state.dataForm.dbId === item.formDbId) {
      state.dbType = item.dbType
      break
    }
    const e = state.dbOptions[i].children || []
    for (let j = 0; j < e.length; j++) {
      if (state.dataForm.dbId === e[j].formDbId) {
        state.dbType = e[j].dbType
        break
      }
    }
  }
}

const stepFormState = ref({})

function buildFormIdPropMap(formJsonStr) {
  if (!formJsonStr) return {}
  let formData
  try {
    formData = JSON.parse(formJsonStr)
  } catch {
    return {}
  }
  const fields = formData?.fields
  if (!Array.isArray(fields)) return {}

  const map = {}
  const loop = (data, parent) => {
    if (!data) return
    const cfg = data.__config__
    if (cfg && Array.isArray(cfg.children)) {
      cfg.children.forEach((d) => loop(d, data))
    }
    if (Array.isArray(data)) {
      data.forEach((d) => loop(d, parent))
      return
    }
    if (!cfg) return
    const visibility = !cfg.visibility || (Array.isArray(cfg.visibility) && cfg.visibility.includes('pc'))
    if (cfg.layout === 'colFormItem' && data.__vModel__ && visibility) {
      const isTableChild = parent && parent.__config__ && parent.__config__.ceriKey === 'table'
      const propId = isTableChild ? parent.__vModel__ + '-' + data.__vModel__ : data.__vModel__
      if (cfg.formId) {
        map[cfg.formId] = {
          propId,
          vModel: data.__vModel__
        }
      }
    }
  }
  loop(fields)
  return map
}

function applyReplaceMapInPlace(target, replaceMap) {
  if (!target || !replaceMap) return
  if (typeof target === 'string') {
    return replaceMap[target] || target
  }
  if (Array.isArray(target)) {
    for (let i = 0; i < target.length; i++) {
      const replaced = applyReplaceMapInPlace(target[i], replaceMap)
      if (replaced !== undefined) target[i] = replaced
    }
    return
  }
  if (typeof target === 'object') {
    for (const key in target) {
      const replaced = applyReplaceMapInPlace(target[key], replaceMap)
      if (replaced !== undefined) target[key] = replaced
    }
  }
}

function syncColumnDataByFormJsonChange(oldFormJsonStr, newFormJsonStr) {
  if (!state.columnData && !state.appColumnData) return
  const oldMap = buildFormIdPropMap(oldFormJsonStr)
  const newMap = buildFormIdPropMap(newFormJsonStr)
  const replaceMap = {}
  for (const formId in oldMap) {
    if (!newMap[formId]) continue
    const oldProp = oldMap[formId].propId
    const newProp = newMap[formId].propId
    if (oldProp && newProp && oldProp !== newProp) {
      replaceMap[oldProp] = newProp
    }
  }
  if (!Object.keys(replaceMap).length) return
  if (state.columnData) applyReplaceMapInPlace(state.columnData, replaceMap)
  if (state.appColumnData) applyReplaceMapInPlace(state.appColumnData, replaceMap)
}

const next = async () => {
  if (state.activeStep == 0) {
    /* 校验表单 */
    const formvalidate = await stepBasicRef.value?.validate()
    if (!formvalidate.valid) {
      return createMessage.warning('请补充完全表单必填项!')
    }

    state.dataForm = { ...state.dataForm, ...formvalidate.data }
    getDbType()
    // const type = state.dataForm.type

    const formDbs = await stepBasicRef.value?.getDbTableValue()

    if (!formDbs.length) {
      message.warning('请至少选择一个数据表')
      return
      generatorStore.setHasTable(false)
      generatorStore.setAllTable([])
      generatorStore.setFormItemList([])
    } else {
      generatorStore.setHasTable(true)
      generatorStore.setAllTable(formDbs)
      generatorStore.setFormItemList([])
    }
    state.dataForm.formDbs = formDbs
    stepFormState.value = formvalidate.data
    state.activeStep++
  } else if (activeStep.value == 1) {
    await unref(setpFormRef)
      .getData()
      .then((res) => {
        state.errorList = res.errorList || []
        if (state.errorList.length) {
          setTimeout(() => {
            unref(validatePopoverRef.value)?.setVisible(true)
          }, 10)
          return
        }
        state.formData = res.formData
        state.dataForm.formJson = state.formData ? JSON.stringify(state.formData) : null
        state.activeStep++
      })
  } else if (activeStep.value == 2) {
    await unref(stepTableRef)
      .getData()
      .then((res) => {
        state.columnData = res.columnData
        state.appColumnData = res.appColumnData
        state.activeStep++
      })
      .catch((err) => {
        err.msg && message.warning(err.msg)
      })
  }
}
const prev = () => {
  if (activeStep.value == 1) {
    /* 表单赋值 */
    stepBasicRef.value?.setFormData(stepFormState.value)
  }

  state.activeStep--
}

const emit = defineEmits(['update:isShowStepDrawer'])
function onClose() {
  emit('update:isShowStepDrawer', false) // 手动通知父组件关闭抽屉
}

async function handleSubmit() {
  if (activeStep.value == 0) {
    // if (!state.tables.length && (state.defaultTable.length || type == 3 || type == 4)) return message.warning('请至少选择一个数据表')
    const formvalidate = await stepBasicRef.value?.validate()
    if (!formvalidate.valid) {
      return createMessage.warning('请补充完全表单必填项!')
    }
    state.dataForm = { ...state.dataForm, ...formvalidate.data }
    state.dataForm.formDbs = await stepBasicRef.value?.getDbTableValue()
    handleRequest()
  } else if (activeStep.value == 1) {
    unref(setpFormRef)
      .getData()
      .then((res) => {
        state.errorList = res.errorList || []
        if (state.errorList.length) {
          setTimeout(() => {
            unref(validatePopoverRef)?.setVisible(true)
          }, 10)
          return
        }
        state.formData = res.formData
        const oldFormJsonStr = state.dataForm.formJson
        const newFormJsonStr = state.formData ? JSON.stringify(state.formData) : null
        state.dataForm.formJson = newFormJsonStr
        syncColumnDataByFormJsonChange(oldFormJsonStr, newFormJsonStr)
        state.dataForm.columnDataStr = state.columnData ? JSON.stringify(state.columnData) : null
        state.dataForm.appColumnDataStr = state.appColumnData ? JSON.stringify(state.appColumnData) : null
        handleRequest()
      })
  } else {
    if (state.dataForm.webType == 1) return handleRequest()
    unref(stepTableRef)
      .getData()
      .then((res) => {
        state.columnData = res.columnData
        state.appColumnData = res.appColumnData
        state.dataForm.columnDataStr = res.columnData ? JSON.stringify(res.columnData) : null
        state.dataForm.appColumnDataStr = res.appColumnData ? JSON.stringify(res.appColumnData) : null
        // state.dataForm.tableJson = state.tables.length > 0 ? JSON.stringify(state.tables) : null
        handleRequest()
      })
      .catch((err) => {
        err.msg && createMessage.warning(err.msg)
      })
  }
}

function handleRequest() {
  // state.btnLoading = true
  // 统一规范提交数据格式：
  // 1. 确保 formDbs[*].fields 为字符串（后端字段类型为 string）
  // 2. 补充必填的 parameter 字段，使用空数组 JSON 字符串
  if (Array.isArray(state.dataForm.formDbs)) {
    state.dataForm.formDbs = state.dataForm.formDbs.map((item) => {
      const fields = item && Object.prototype.hasOwnProperty.call(item, 'fields') ? item.fields : undefined
      return {
        ...item,
        fields:
          typeof fields === 'string'
            ? fields
            : fields != null
            ? JSON.stringify(fields)
            : null
      }
    })
  }
  if (!('parameter' in state.dataForm) || state.dataForm.parameter == null) {
    // 后端要求 parameter 必填，使用空数组 JSON 字符串占位
    // @ts-ignore
    state.dataForm.parameter = '[]'
  }

  const query = {
    ...state.dataForm,
    createId:userStore.userid,
    createName:userStore.loginname
  }
  const formMethod = state.dataForm.formDesignId ? update : create
  formMethod(query).then((res) => {
    if (res.code == 200 && res.success) {
      message.success(res.message)
      return
    }
    message.error(res.message)
  })
}
</script>
<style lang="scss">
.WebDegisnStepDrawer {
  .StepDesignContainerTop {
    position: relative;
    display: flex;
    justify-content: flex-end;
    align-items: center;
    padding: 0 20px;
  }

  .StepDesignContainerStep {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
  }

  .StepDesignContainerRight {
    display: flex;
  }

  .ant-drawer-body {
    // 固定 body 高度 = 总高度 - 头部高度（约 60px），方便内部使用 100% 高度做滚动
    height: calc(100% - 60px);
    padding: 0px !important;
  }
}

.header-steps {
  width: 500px;
  height: 50px;
  line-height: 50px;
}
</style>
