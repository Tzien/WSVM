<template>
  <div class="common-container">
    <!---->
    <a-select v-model:value="innerValue" v-bind="getSelectBindValue" :options="options2" @change="onChange" @click="openSelectModal" />
    <a-modal
      v-model:open="visible"
      :title="popupTitle"
      :width="1000"
      class="common-container-modal"
      @ok="handleSubmit"
      @cancel="handleCancel"
      :maskClosable="false"
    >
      <template #closeIcon>
        <ModalClose :canFullscreen="false" @cancel="handleCancel" />
      </template>
      <div class="ceri-content-wrapper">
        <div class="ceri-content-wrapper-left">
          <BasicLeftTree ref="leftTreeRef" :showSearch="false" :treeData="treeData" :loading="treeLoading" @select="handleTreeSelect" />
        </div>
        <div class="ceri-content-wrapper-center">
          <div class="ceri-content-wrapper-content">
            <div style="margin-bottom: 5px">
              <span style="margin-left: 20px">
                名称：
                <a-input v-model:value="name" placeholder="请输入名称" style="width: 150px; margin-right: 5px" allowClear />
              </span>
              <span style="margin-left: 20px">
                类型：
                <a-select
                  v-model:value="type"
                  allowClear
                  placeholder="请选择类型"
                  style="width: 150px; margin-right: 5px"
                  :options="dataApiTypeOptions"
                ></a-select>
              </span>
              <a-button style="margin-right: 5px" @click="getTableData(false)">查询</a-button>
              <a-button @click="resetBtn()">重置</a-button>
            </div>
            <s-table
              :columns="columns"
              height="500px"
              headerAlign="center"
              align="center"
              size="small"
              stripe
              type="radio"
              :pagination="false"
              rowKey="dataApiId"
              :row-selection="rowSelection"
              :data-source="dataSource"
              style="padding-right: 10px"
            >
            </s-table>
            <div class="paginationStyle" style="margin-top: 10px">
              <a-pagination
                v-model:current="modalCurrentPage"
                v-model:page-size="modalCurrentPageSize"
                :total="modalTotalCount"
                align="right"
                :pageSizeOptions="['20', '30', '45', '60']"
                showSizeChanger
                :show-total="(total) => `${$t('message.drawer.TotalOf')} ${total} ${$t('message.drawer.Items')}`"
                @change="modalPageSizeChange"
              />
            </div>
            <!-- <BasicTable @register="registerTable" :searchInfo="searchInfo" class="ceri-sub-table">
              <template   #expandedRowRender="{   record }" >
                <BasicTable @register="registerSubTable" :data-source="record.templateJson">
                  <template #bodyCell="{ column, record }">
                    <template v-if="column.key === 'field'">
                      <span class="required-sign">{{ record.required ? '*' : '' }}</span>
                      {{ record.field }}{{ record.fieldName ? '(' + record.fieldName + ')' : '' }}
                    </template>
                    <template v-if="column.key === 'dataType'">
                      {{ getTypeText(record.dataType) }}
                    </template>
                  </template>
                </BasicTable>
              </template>
            </BasicTable> -->
          </div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts" setup>
import { Form, Modal as AModal, message } from 'ant-design-vue'
import { reactive, ref, unref, watch, computed } from 'vue'
import ModalClose from '@/components/Modal/src/components/ModalClose.vue'
import { BasicLeftTree, TreeActionType } from '@/components/Tree'
import { BasicTable, useTable, BasicColumn } from '@/components/Table'
import { useI18n } from 'vue-i18n'
import { useBaseStore } from '@/store/base'
import { pick } from 'lodash-es'
import { getApiCategoryDictApi, getListApi, deleteApi } from '@/api/DataApi/DataApi'

const name = ref()
const type = ref()
const category = ref('')
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(10)
var modalTotalCount = ref(0)
const dataSource = ref([])
const dataApiTypeOptions = ref([
  { value: '0', label: '静态数据' },
  { value: '1', label: 'SQL操作' },
  { value: '2', label: 'API操作' }
])
const selectedRowKeys = ref([])
const selectedRows = ref([])
const handleRowSelectionChange = (keys, rows) => {
  selectedRowKeys.value = keys
  selectedRows.value = rows


}
const options2 = ref()
const rowSelection = computed(() => ({
  type: 'radio',
  onChange: handleRowSelectionChange
}))

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData(false)
}
async function getTableData(showMes) {
  var obj = {
    name: name.value,
    type: type.value,
    category: category.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getListApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = item.dataApiId
      })
      dataSource.value = res.data
      modalTotalCount.value = res.total

      if (!!showMes) {
        message.success(res.message)
      }
    } else {
      message.warning(res.message)
    }
  })
}
function resetBtn() {
  name.value = ''
  type.value = null
  getTableData(false)
}
defineOptions({ inheritAttrs: false })
const props = defineProps({
  value: { default: '' },
  title: { type: String, default: '' },
  popupTitle: { type: String, default: '数据接口' },
  dataType: { type: String, default: '' },
  disabled: { type: Boolean, default: false },
  allowClear: { type: Boolean, default: true },
  size: { type: String, default: 'default' },
  /**
   * sourceType
   * 1 - 过滤掉：鉴权、真分页、SQL的增加、修改、删除类型
   * 2 - 过滤掉：鉴权、SQL的增加、修改、删除类型
   * 3 - 过滤掉：鉴权、真分页、SQL的查询类型
   */
  sourceType: { type: Number, default: 1 }
})
const emit = defineEmits(['update:value', 'change'])
const formItemContext = Form.useInjectFormItemContext()
const { t } = useI18n()
const baseStore = useBaseStore()
const innerValue = ref(undefined)
const visible = ref(false)
const options = ref<any[]>([])

const columns: BasicColumn[] = [
  { title: '名称', dataIndex: 'name' },
  { title: '编码', dataIndex: 'code' },
  { title: '类型', dataIndex: 'type' }
]
const searchInfo = reactive({
  category: '',
  sourceType: props.sourceType
})
const leftTreeRef = ref<Nullable<TreeActionType>>(null)
const treeLoading = ref(false)
const treeData = ref<any[]>([])
const [registerTable, { getForm, getSelectRows, setSelectedRowKeys, getSelectRowKeys }] = useTable({
  api: getListApi,
  columns,
  immediate: false,
  useSearchForm: true,
  // formConfig: {
  //   baseColProps: { span: 8 },
  //   schemas: [
  //     {
  //       field: 'name',
  //       label: t('common.keyword'),
  //       component: 'Input',
  //       componentProps: {
  //         placeholder: t('common.enterKeyword'),
  //         submitOnPressEnter: true
  //       }
  //     },
  //     {
  //       field: 'type',
  //       label: '类型',
  //       component: 'Select',
  //       componentProps: {
  //         options: [
  //           { id: '2', fullName: '静态数据' },
  //           { id: '1', fullName: 'SQL操作' },
  //           { id: '3', fullName: 'API操作' }
  //         ],
  //         submitOnPressEnter: true
  //       }
  //     }
  //   ]
  // },
  tableSetting: { size: false, setting: false },
  isCanResizeParent: true,
  resizeHeightOffset: -74,
  // rowSelection: { type: 'radio' },
  afterFetch: (data) => {
    const list = data.map((o) => {
      let templateJson = o.parameterJson ? JSON.parse(o.parameterJson) : []
      if (!templateJson) templateJson = []
      return {
        ...o,
        templateJson
      }
    })
    return list
  }
})
const [registerSubTable] = useTable({
  columns: [
    { title: '参数名称', dataIndex: 'field', key: 'field' },
    { title: '参数类型', dataIndex: 'dataType', key: 'dataType' },
    { title: '默认值', dataIndex: 'defaultValue', key: 'defaultValue', ellipsis: false }
  ],
  pagination: false,
  showTableSetting: false,
  canResize: false,
  scroll: { x: undefined }
})

const typeOptions: any[] = [
  { fullName: '字符串', id: 'varchar' },
  { fullName: '整型', id: 'int' },
  { fullName: '日期时间', id: 'datetime' },
  { fullName: '浮点', id: 'decimal' },
  { fullName: '长整型', id: 'bigint' },
  { fullName: '文本', id: 'text' }
]

const getSelectBindValue = computed(() => {
  return {
    ...pick(props, ['disabled', 'size', 'allowClear']),
    // fieldNames: { label: 'name', value: 'id' },
    placeholder: '请选择',
    open: false,
    showSearch: false,
    showArrow: true
  }
})

watch(
  () => props.value,
  (val) => {
    setValue(val)
  },
  { immediate: true }
)
// watch(
//   () => props.title,
//   () => {
//     setValue(props.value)
//   }
// )

function getTypeText(type) {
  let item = typeOptions.filter((o) => o.id == type)[0]
  return item && item.fullName ? item.fullName : ''
}
function setValue(value) {
  innerValue.value = value || undefined
  options.value = [{ id: innerValue.value, fullName: props.title }]
}
function onChange() {
  options.value = []
  emit('change', '', {})
}
async function getApiCategoryDict() {
  var obj = {
    dictCode: 'DataApiCategory'
  }
  await getApiCategoryDictApi(obj).then((res) => {
    const children = []
    res.data.items.forEach((c) => {
      children.push({
        fullName: c.itemName,
        id: c.dictDetailId
      })
    })
    treeData.value = children
  })
}
async function openSelectModal() {
  if (props.disabled) return
  visible.value = true
  treeLoading.value = true
  // treeData.value = (await baseStore.getDictionaryData('DataInterfaceType')) as any[];
  await getApiCategoryDict()
  if (!treeData.value.length) return (treeLoading.value = false)
  searchInfo.category = treeData.value[0].id
  const leftTree = unref(leftTreeRef)
  leftTree?.setSelectedKeys([searchInfo.category])
  treeLoading.value = false
  // getForm().resetFields()
  // setSelectedRowKeys(innerValue.value ? [innerValue.value] : [])

  category.value = searchInfo.category
  getTableData(false)
}
function handleTreeSelect(id) {
  if (!id || searchInfo.category === id) return
  searchInfo.category = id
  searchInfo.sourceType = props.sourceType
  // getForm().resetFields()

  category.value = searchInfo.category
  getTableData(false)
}
function handleCancel() {
  visible.value = false
}
function handleSubmit() {
  // console.log('Selected Row Keys:', selectedRowKeys.value[0])
  // console.log('Selected Rows:', selectedRows.value[0])
  if (!selectedRowKeys.value.length && !selectedRows.value.length) return
  // if (!getSelectRows().length) {
  //   emit('update:value', innerValue.value)
  //   emit('change', innerValue.value, options.value[0])
  //   formItemContext.onFieldChange()
  //   handleCancel()
  //   return
  // }
  // const selectRow = getSelectRows()[0]
  // options.value = getSelectRows()
  // innerValue.value = selectRow.id

  options2.value = [
    {
      value: selectedRowKeys.value[0],
      label: selectedRows.value[0].name
    }
  ]
  innerValue.value = selectedRowKeys.value[0]
  console.info(selectedRows.value[0].name)
  emit('update:value', selectedRowKeys.value[0])
  emit('change', selectedRowKeys.value[0], selectedRows.value[0])
  formItemContext.onFieldChange()
  handleCancel()
}
</script>
