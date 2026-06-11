<template>
  <div style="padding: 20px">
    <a-form ref="formRef" :model="formState" :label-col="labelCol" @finish="onFinish" @finishFailed="onFinishFailed">
      <a-form-item label="表单名称" name="name" :rules="[{ required: true, message: '请输入表单名称!' }]">
        <a-input allowClear v-model:value="formState.name" />
      </a-form-item>
      <a-form-item label="表单编码" name="code" :rules="[{ required: true, message: '请输入表单编码!' }]">
        <a-input allowClear v-model:value="formState.code" />
      </a-form-item>
      <a-form-item label="表单分类" name="formCategoryId" :rules="[{ required: true, message: '请选择表单分类!' }]">
        <a-select
          v-model:value="formState.formCategoryId"
          show-search
          allowClear
          style="width: 100%"
          :options="categoryOptions"
          :filter-option="formclassifyFilterOption"
        ></a-select>
      </a-form-item>
      <a-form-item label="表单排序" name="sort">
        <a-input-number allowClear style="width: 100%" v-model:value="formState.sort" />
      </a-form-item>
      <a-form-item label="表单说明" name="remark">
        <a-textarea allowClear :rows="3" v-model:value="formState.remark" />
      </a-form-item>
      <a-form-item label="数据连接" name="dbId">
        <a-select v-model:value="formState.dbId" :options="ConnectOptions" style="width: 100%"></a-select>
      </a-form-item>
      <div style="width: 100%">
        <a-table bordered :data-source="ConnectDataSource" :columns="ConnectColumns" size="small" :pagination="false">
          <template #bodyCell="{ column, text, record }">
            <template v-if="column.dataIndex === 'typeId'">
              <a-tag style="width: 100%; text-align: center" color="#2db7f5" v-if="text == 1">主表</a-tag>
              <a-tag style="width: 100%; text-align: center" color="orange" v-else>从表</a-tag>
            </template>
            <template v-else-if="column.dataIndex === 'foreignKey'">
              <a-select
                v-if="record.typeId == 0"
                v-model:value="dbTable[record.index].fk"
                placeholder="请选择"
                style="width: 100%"
                :options="getForeignOptions(record)"
              ></a-select>
            </template>
            <template v-else-if="column.dataIndex === 'primaryKey'">
              <a-select
                v-if="record.typeId == 0"
                v-model:value="dbTable[record.index].pk"
                placeholder="请选择"
                style="width: 100%"
                :options="primaryKeyOptions"
              ></a-select>
            </template>

            <template v-else-if="column.dataIndex === 'operation'">
              <a-popconfirm v-if="ConnectDataSource.length" title="确定要移除当前行??" @confirm="onDeleteConnectData(record)">
                <a>移除</a>
              </a-popconfirm>
            </template>
          </template>
          <template #emptyText>
            <div style="height: 100px; line-height: 100px">点击“新增”可选择1条(单表)或2条以上(多表)</div>
          </template>
        </a-table>
        <a-button
          @click="addNewConnectData"
          type="text"
          style="color: rgb(24, 144, 255); margin-top: 10px; border: 1px dashed lightgray; border-radius: 2px; width: 100%"
        >
          <template #icon>
            <PlusOutlined />
          </template>
          新增一行
        </a-button>
      </div>
    </a-form>
    <TableModal @register="registerTableModal" @select="onTableSelect" />
  </div>
</template>
<script setup>
import { ref, reactive } from 'vue'
import { PlusOutlined } from '@ant-design/icons-vue'
import { useModal } from '@/components/Modal'
import TableModal from './components/TableModal.vue'
import { getConnectList } from '@/api/demoApi/configDB'
import { getDataModelFieldList } from '@/api/demoApi/configDB'
import { useMessage } from '@/hooks/web/useMessage'
import { message } from 'ant-design-vue'
const { createMessage, createConfirm } = useMessage()

const [registerTableModal, { openModal: openTableModal }] = useModal()

const props = defineProps({
  categoryOptions: {
    type: Object,
    default: []
  }
})

let formState = reactive({
  formDesignId: null,
  name: '',
  code: '',
  formCategoryId: null,
  sort: 0,
  remark: '',
  dbId: '0',
  formDbs: []
})

let dbTable = ref([])

const labelCol = {
  span: 3
}
const primaryKeyOptions = ref([])

function getForeignOptions(row) {
  return row.fields.map((item) => ({
    value: item.name,
    label: item.name
  }))
}

// /* 视图分类传参了 */
// const formclassifyOptions = ref([
//   {
//     value: 'GZ',
//     label: '钢种管理'
//   },
//   {
//     value: 'HJ',
//     label: '合金管理'
//   },
//   {
//     value: 'Other',
//     label: '其他材料'
//   }
// ])
const ConnectOptions = ref([
  {
    value: '0',
    label: '默认数据库'
  }
])

const ConnectColumns = [
  {
    title: '类别',
    dataIndex: 'typeId',
    width: '8%',
    align: 'center'
  },
  {
    title: '表名',
    dataIndex: 'tableName',
    width: '40%'
  },
  {
    title: '外键字段',
    dataIndex: 'foreignKey',
    align: 'center'
  },
  {
    title: '关联主键',
    dataIndex: 'primaryKey',
    align: 'center'
  },
  {
    title: '操作',
    dataIndex: 'operation',
    width: '8%'
  }
]
const ConnectDataSource = ref([])
const relationTable = ref(null)

const formRef = ref(null)
// 校验方法
const validate = async () => {
  try {
    await formRef.value.validate()
    return { valid: true, data: formState }
  } catch (err) {
    return { valid: false, error: err }
  }
}

// 暴露表单回填方法
const setFormData = (data) => {
  //赋值dbTable
  if (data.formDbs.length > 0) {
    ConnectDataSource.value = []
    dbTable.value = []

    for (let i = 0; i < data.formDbs.length; i++) {
      const e = data.formDbs[i]
      const setRtable = data.formDbs[0].tableName
      const item = {
        index: i,
        relationField: '',
        relationTable: i == 0 ? '' : setRtable,
        table: e.tableName,
        tableName: e.tableName,
        tableField: '',
        typeId: e.typeId,
        fields: e.fields && JSON.parse(e.fields)
      }
      ConnectDataSource.value.push(item)

      const itemDB = {
        pk: e.pk,
        fk: e.fk
      }
      dbTable.value.push(itemDB)
    }
  }
  formState = Object.assign(formState, data) // 保持响应性不然校验失败
}

const resetFields = async () => {
  await formRef.value.resetFields()
}

const getConnectListData = async () => {
  ConnectOptions.value = [
    {
      value: '0',
      label: '默认数据库'
    }
  ]

  await getConnectList().then((res) => {
    if (res.code == 200 && res.success) {
      ConnectOptions.value.push(
        ...res.data.map((group) => ({
          label: group.dbType,
          options: group.items.map((item) => ({
            value: item.dbConfigId,
            label: item.name
          }))
        }))
      )
    } else {
      message.error('获取数据连接异常')
    }
  })
}

function getDbTableValue() {
  return ConnectDataSource.value.map((item) => {
    const db = dbTable.value[item.index] || { pk: null, fk: null }
    return {
      formDbId:formState.formDesignId ? formState.formDbs[item.index]?.formDbId : "",
      formDesignId: formState.formDesignId || null,
      typeId: item.typeId,
      tableName: item.tableName,
      fields: item.fields && JSON.stringify(item.fields),
      pk: db.pk,
      fk: db.fk
    }
  })
}

defineExpose({ validate, setFormData, resetFields, getConnectListData, getDbTableValue })

async function addNewConnectData() {
  if (!formState.dbId) return createMessage.error('请先选择数据库')
  openTableModal(true, { dbId: formState.dbId })
}
async function onTableSelect(data) {
  data = Array.from(new Map(data.map((item) => [item.name, item])).values())
  // const values = getFieldsValue()
  // const type = state.dataForm.type
  const queryType = '0'
  const checkList = []
  if (!ConnectDataSource.value.length) {
    for (let i = 0; i < data.length; i++) {
      const e = data[i]
      const relationTable = data[0].name
      const typeId = i == 0 ? '1' : '0'
      const res = await getDataModelFieldList(formState.dbId, e.name)
      if (!res.success || res.code != 200) {
        message.error('表字段数据为空，请检查数据库')
        return
      }
      const fields = res.data[0].columnInfos
      const item = {
        index: i,
        relationField: '',
        relationTable: i == 0 ? '' : relationTable,
        table: e.name,
        tableName: e.name,
        tableField: '',
        typeId,
        fields
      }
      checkList.push(item)
    }
    relationTable.value = checkList[0].name
    // state.mainTableFields = checkList[0].fields
    ConnectDataSource.value = checkList
  } else {
    for (let i = 0; i < data.length; i++) {
      const e = data[i]
      let boo = ConnectDataSource.value.some((o) => o.table == e.name)
      if (!boo) {
        const res = await getDataModelFieldList(formState.dbId, e.name)
        if (!res.success || res.code != 200) {
          message.error('表字段数据为空，请检查数据库')
          return
        }
        const fields = res.data[0].columnInfos
        const item = {
          index: i,
          relationField: '',
          relationTable: relationTable.value,
          table: e.name,
          tableName: e.name,
          tableField: '',
          typeId: '0',
          fields
        }
        checkList.push(item)
      }
    }
    ConnectDataSource.value = [...ConnectDataSource.value, ...checkList]
  }
  // state.loading = false

  //当选择的数据表大于一张的时候，主外键赋值
  if (ConnectDataSource.value.length > 1) {
    //主键
    let boo = ConnectDataSource.value.filter((o) => o.typeId == 1)
    if (boo.length > 0) {
      primaryKeyOptions.value = boo[0].fields.map((item) => ({
        value: item.name,
        label: item.name
      }))
    }

    //dbTable数据初始化
    dbTable.value = ConnectDataSource.value.map(() => ({
      pk: null,
      fk: null
    }))
  }
}

function onDeleteConnectData(row) {
  const index = ConnectDataSource.value.findIndex((item) => item.index === row.index)
  if (index !== -1) {
    ConnectDataSource.value.splice(index, 1)
    dbTable.value[index] = {
      pk: null,
      fk: null
    }
  }
  if (row.typeId == 1 && ConnectDataSource.value.length > 0) {
    ConnectDataSource.value[0].typeId = 1
    ConnectDataSource.value[0].foreignKey = null
    ConnectDataSource.value[0].primaryKey = null
    dbTable.value[ConnectDataSource.value[0].index] = {
      pk: null,
      fk: null
    }
  }
}

const formclassifyFilterOption = (input, option) => {
  return option.value.toLowerCase().indexOf(input.toLowerCase()) >= 0
}

const onFinish = (values) => {
  console.log('Success:', values)
}
const onFinishFailed = (errorInfo) => {
  console.log('Failed:', errorInfo)
}
</script>
<style></style>
