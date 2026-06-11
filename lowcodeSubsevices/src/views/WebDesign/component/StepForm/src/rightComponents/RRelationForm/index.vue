<template>
  <a-form-item label="弹窗标题">
    <a-input v-model:value="activeData.popupTitle" placeholder="请输入" />
  </a-form-item>
  <a-form-item label="弹窗类型" v-if="showType === 'pc'">
    <ceri-select v-model:value="activeData.popupType" placeholder="请选择" :options="popupTypeOptions" />
  </a-form-item>
  <a-form-item label="弹窗宽度" v-if="showType === 'pc'">
    <a-select v-model:value="activeData.popupWidth" placeholder="请选择">
      <a-select-option v-for="item in popupWidthOptions" :key="item" :value="item">{{ item }}</a-select-option>
    </a-select>
  </a-form-item>
  <a-form-item label="关联功能2">
    <CeriTreeSelect v-model:value="activeData.modelId" :options="treeData" placeholder="请选择" lastLevel allowClear @change="onModeIdChange" />
  </a-form-item>
  <a-form-item>
    <template #label>存储字段<BasicHelp text="请选择具有唯一性、稳定性的字段" /></template>
    <ceri-select
      v-model:value="activeData.propsValue"
      placeholder="请选择"
      :options="propsValueOptions"
      :fieldNames="{ value: 'field', label: 'fieldName' }"
      showSearch
      @dropdownVisibleChange="visibleChange"
    />
  </a-form-item>
  <a-form-item label="回显字段">
    <ceri-select v-model:value="activeData.relationField" placeholder="请选择" :options="fieldOptions1" showSearch @dropdownVisibleChange="visibleChange" />
  </a-form-item>
  <a-form-item>
    <template #label>填充字段<BasicHelp text="选择数据后，将关联表单的字段值填充到当前表单字段，不支持代码生成。" /></template>
    <a-button block @click="handleTransferRules()">{{ activeData.__config__.transferList?.length ? '编辑填充规则' : '填充规则配置' }}</a-button>
  </a-form-item>
  <a-divider v-if="!activeData.__config__.isSubTable">显示字段</a-divider>
  <div class="options-list" v-if="!activeData.__config__.isSubTable">
    <draggable v-model="activeData.extraOptions" :animation="300" group="extraItem" handle=".option-drag" itemKey="uuid">
      <template #item="{ element, index }">
        <div class="select-item">
          <div class="select-line-icon option-drag">
            <i class="icon-ym icon-ym-darg" />
          </div>
          <ceri-select
            v-model:value="element.value"
            placeholder="请选择"
            :options="fieldOptions"
            showSearch
            @dropdownVisibleChange="visibleChange"
            @change="onChange($event, element)"
          />
          <div class="close-btn select-line-icon" @click="activeData.extraOptions.splice(index, 1)">
            <i class="icon-ym icon-ym-btn-clearn" />
          </div>
        </div>
      </template>
    </draggable>
    <div class="add-btn">
      <a-button type="link" preIcon="icon-ym icon-ym-btn-add" @click="addExtraItem">添加字段</a-button>
    </div>
  </div>
  <a-divider>列表字段</a-divider>
  <div class="options-list">
    <draggable v-model="activeData.columnOptions" :animation="300" group="selectItem" handle=".option-drag" itemKey="uuid">
      <template #item="{ element, index }">
        <div class="select-item">
          <div class="select-line-icon option-drag">
            <i class="icon-ym icon-ym-darg" />
          </div>
          <ceri-select
            v-model:value="element.value"
            placeholder="请选择"
            :options="fieldOptions1"
            showSearch
            @dropdownVisibleChange="visibleChange"
            @change="onChange($event, element)"
          />
          <div class="close-btn select-line-icon" @click="activeData.columnOptions.splice(index, 1)">
            <i class="icon-ym icon-ym-btn-clearn" />
          </div>
        </div>
      </template>
    </draggable>
    <div class="add-btn">
      <a-button type="link" preIcon="icon-ym icon-ym-btn-add" @click="addSelectItem">添加字段</a-button>
    </div>
  </div>
  <a-divider>列表分页</a-divider>
  <a-form-item label="分页设置">
    <a-switch v-model:checked="activeData.hasPage" />
  </a-form-item>
  <a-form-item label="分页条数" v-if="activeData.hasPage">
    <ceri-radio v-model:value="activeData.pageSize" :options="pageSizeOptions" optionType="button" button-style="solid" class="right-radio" />
  </a-form-item>
  <a-form-item>
    <template #label
      >查询设置<BasicHelp :text="['简易查询：关键词查询只匹配”单行“、”多行“、”数字“、”下拉补全“列表字段;', '全字段查询：关键词查询匹配所有列表字段;']"
    /></template>
    <ceri-radio v-model:value="activeData.queryType" :options="queryOptions" optionType="button" button-style="solid" class="right-radio" />
  </a-form-item>
  <a-divider></a-divider>
  <a-form-item label="能否清空">
    <a-switch v-model:checked="activeData.clearable" />
  </a-form-item>
  <RuleModal @register="registerModal" @change="onRuleChange" />
</template>
<script lang="ts" setup>
import { getVisualDevSelector, getConfigData } from '@/api/onlineDev/visualDev'
import { computed, ref, inject, onMounted, unref } from 'vue'
import draggable from 'vuedraggable'
import { useMessage } from '@/hooks/web/useMessage'
import { useModal } from '@/components/Modal'
import RuleModal from './RuleModal.vue'
import { useDynamic } from '../../hooks/useDynamic'

import { getFormListApi } from '@/api/DataApi/DataApi'
defineOptions({ inheritAttrs: false })
const props = defineProps(['activeData'])
const emit = defineEmits(['relationChange'])
const { createMessage } = useMessage()
const [registerModal, { openModal }] = useModal()
const { formFieldsSelectOptions } = useDynamic(props.activeData)

const getShowType: (() => string | undefined) | undefined = inject('getShowType')
const showType = computed(() => (getShowType as () => string | undefined)())

const popupTypeOptions = [
  { id: 'dialog', fullName: '居中弹窗' },
  { id: 'drawer', fullName: '右侧弹窗' }
]
const popupWidthOptions = ['600px', '800px', '1000px', '40%', '50%', '60%', '70%', '80%']
const pageSizeOptions = [
  { id: 20, fullName: '20条' },
  { id: 50, fullName: '50条' },
  { id: 80, fullName: '80条' },
  { id: 100, fullName: '100条' }
]
const queryOptions = [
  { id: 0, fullName: '简易查询' },
  { id: 1, fullName: '全字段查询' }
]
const treeData = ref([])
const fieldOptions = ref<any[]>([])
const fieldOptions1 = ref([])
const propsValueOptions = ref([])

function onModeIdChange(val) {
  props.activeData.relationField = ''
  props.activeData.__config__.transferList = []
  props.activeData.extraOptions = []
  props.activeData.columnOptions = []
  props.activeData.propsValue = undefined
  emit('relationChange', props.activeData.__vModel__)
  if (!val) {
    fieldOptions.value = []
    fieldOptions1.value = []
    propsValueOptions.value = []
    return
  }
  console.info(222,val)
  propsValueOptions.value = formData.value.flatMap(category => category.formList).find(form => form.formId === val)?.saveField.map(field => ({ field, fieldName: field })) || [];
  fieldOptions1.value = formData.value.flatMap(category => category.formList).find(form => form.formId === val)?.viewField.map(id => ({ id, fullName: id })) || [];

  // getFieldOptions()
}
function getFieldOptions() {
  if (!props.activeData.modelId) return
  getConfigData(props.activeData.modelId).then((res) => {
    const { formData, propsValueList } = res.data
    let formJson: any = {},
      fieldList: any = []
    if (formData) formJson = JSON.parse(formData)
    fieldList = formJson.fields || []
    let list: any[] = transformFieldList(fieldList)
    fieldOptions.value = list
    fieldOptions1.value = list.filter((o) => o.id.indexOf('-') < 0)
    propsValueOptions.value = propsValueList || []
    !props.activeData.propsValue && (props.activeData.propsValue = propsValueOptions.value[0]?.field)
  })
}
function transformFieldList(formFieldList) {
  let list: any[] = []
  const loop = (data, parent?) => {
    if (!data) return
    if (data.__vModel__ && data.__config__.ceriKey !== 'table') {
      const isTableChild = parent && parent.__config__ && parent.__config__.ceriKey === 'table'
      const item = {
        id: isTableChild ? parent.__vModel__ + '-' + data.__vModel__ : data.__vModel__,
        fullName: isTableChild ? parent.__config__.label + '-' + data.__config__.label : data.__config__.label
      }
      list.push(item)
    }
    if (Array.isArray(data)) data.forEach((d) => loop(d, parent))
    if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
      loop(data.__config__.children, data)
    }
  }
  loop(formFieldList)
  return list
}
function visibleChange(val) {
  if (!val) return
  if (!props.activeData.modelId) createMessage.warning('请先选择关联功能')
}
function onChange(val, item) {
  const list = fieldOptions.value.filter((o) => o.id === val) || []
  if (!list.length) return
  const active = list[0]
  item.label = active.fullName
}
function addExtraItem() {
  if (!props.activeData.extraOptions) props.activeData.extraOptions = []
  props.activeData.extraOptions.push({
    value: '',
    label: ''
  })
}
function addSelectItem() {
  props.activeData.columnOptions.push({
    value: '',
    label: ''
  })
}
function getFeatureList() {
  getVisualDevSelector({ type: 1, webType: '2', isRelease: 1 }).then((res) => {
    treeData.value = res.data.list
  })
}
var formData=ref([])
async function getFormList() {
  await getFormListApi().then((res) => {
    formData.value=res.data
    res.data.forEach((category) => {
      // 将每个 category 的数据推送到 treeData 中
      const categoryNode = {
        disabled:false,
        hasChildren:true,
        isLeaf:false,
        fullName: category.categoryName,
        id: category.categoryName, // 这里 id 设为 categoryName
        children: category.formList.map((form) => ({
          fullName: form.formName,  // fullName 设为 formName
          id: form.formId,          // id 设为 formId
          parentId: category.categoryName  // parentId 为 categoryName
        }))
      }

      // 推送到 treeData
      treeData.value.push(categoryNode)
    })
  })
}

function handleTransferRules() {
  if (!props.activeData.modelId) return createMessage.warning('请先选择关联功能')
  openModal(true, {
    transferList: props.activeData.__config__.transferList,
    fieldOptions: fieldOptions.value,
    formFieldsOptions: unref(formFieldsSelectOptions).filter((o) => isSameSource(o))
  })
}
function isSameSource(data) {
  const isSubTable = props.activeData.__config__.isSubTable
  if (isSubTable) return data.__config__.isSubTable && props.activeData.__config__.parentVModel === data.__config__.parentVModel
  return !data.__config__.isSubTable
}
function onRuleChange(data) {
  props.activeData.__config__.transferList = data
}

onMounted(() => {
  getFormList()
  getFieldOptions()
})
</script>
