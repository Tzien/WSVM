<template>
  <BasicModal v-bind="$attrs" @register="registerModal" title="数据选择" @ok="handleSubmit" :width="800" class="ceri-list-modal">
    <BasicTable style="height: calc(750px - 150px)" :searchInfo="searchInfo" @register="registerTable" class="ceri-sub-table"></BasicTable>
  </BasicModal>
</template>
<script lang="ts" setup>
import { getDataModelList } from '@/api/demoApi/configDB'
import { reactive } from 'vue'
import { BasicModal, useModalInner } from '@/components/Modal'
import { BasicTable, useTable } from '@/components/Table'
import { useI18n } from 'vue-i18n'

const emit = defineEmits(['register', 'select'])
const { t } = useI18n()
const [registerModal, { closeModal }] = useModalInner(init)
const searchInfo = reactive<Recordable>({
  linkId: '0'
})
const [registerTable, { getForm, getSelectRows, clearSelectedRowKeys, setSelectedRowKeys }] = useTable({
  api: getDataModelList,
  columns: [
    { title: '表名', dataIndex: 'name' },
    { title: '说明', dataIndex: 'description' }
  ],
  rowKey: 'name',
  useSearchForm: true,
  formConfig: {
    baseColProps: { span: 8 },
    schemas: [
      {
        field: 'tableName',
        label: t('common.keyword'),
        component: 'Input',
        componentProps: {
          placeholder: t('common.enterKeyword'),
          submitOnPressEnter: true
        }
      }
    ]
  },
  tableSetting: { size: false, setting: false },
  isCanResizeParent: true,
  resizeHeightOffset: -73,
  immediate: false,
  rowSelection: { type: 'checkbox' }
})

function init(data) {
  searchInfo.linkId = data.dbId ?? '0'
  getForm().resetFields()
  clearSelectedRowKeys()
  if (Array.isArray(data?.selectedTables) && data.selectedTables.length) {
    setSelectedRowKeys(data.selectedTables)
  }
}
function handleSubmit() {
  const selectedData = getSelectRows()
  emit('select', selectedData)
  closeModal()
}
</script>
