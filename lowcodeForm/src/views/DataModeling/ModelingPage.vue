<template>
  <div class="SurelyTableDemo" style="width: 100%; height: 100%; background-color: #FFFFFF;position: relative">
    <div style="display: flex; align-items: center; justify-content: space-between; height: 36px; line-height: 36px; margin-top: 17px">
      <a-typography-paragraph style="overflow: auto; padding-left: 20px">
        <blockquote style="color: black; border-inline-start: 5px solid rgb(37, 97, 239) !important">
          <span style="font-weight: bold; color: #111111; font-size: 16px">数据建模</span>
        </blockquote>
      </a-typography-paragraph>
   
      <div style="padding-right: 20px">
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(FilterOutlined)" @click="toggleAccordion()"/>
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
        <a-button :icon="h(PlusOutlined)" style="margin: 5px" type="primary" @click="showDrawer()">新增</a-button>
      </div>
    </div>
    <div>
      <a-divider />
    </div>
    <div class="accordion" v-show="isExpand">
      <div ref="accordionRef" class="accordion-content">
      <a-input v-model:value="projectNameInput" placeholder="请输入关键词" style="width: 180px;margin-right: 3px;" />
      <a-select v-model:value="xiavalue" show-search placeholder="数据库连接"  @change="handleSelectChange" style="width: 200px" :options="options"></a-select>
      <a-button :icon="h(SearchOutlined)" style="margin-left: 5px" @click="getTableData()">查询</a-button>
      <a-button style="margin-left: 5px" @click="requery()">重置查询</a-button>
     
      <a-button :icon="h(PlusOutlined)" style="margin: 3px" type="primary" @click="showModal">常用字段</a-button>
      </div>
    </div>
    <s-table :columns="columns"
     :loading="tableLoading"  
      :height="calcHeight"
      headerAlign="center"
      align="center"
      size="small"
      stripe
      :pagination="false"
      rowKey="name"
      :data-source="dataSource"><template #bodyCell="{ text, column, record }">
        <template v-if="column.key === 'operation'">
          <a @click="edit(record)"  style="color: #2461a6;">编辑</a>
          <a-popconfirm title="确定删除吗?" ok-text="是" cancel-text="否" @confirm="deltable(record)">
            <a style="color: red; margin-left: 2px">删除</a>
          </a-popconfirm>

          <!-- <a-button :icon="h(EditOutlined)" @click="Edit(record)" style="display: inline; margin-right: 5px"></a-button> -->
        </template>
      </template></s-table
    >
    <div style="height: 40px;  background-color: #FFFFFF;position: absolute; bottom: 10px; right: 10px; margin-right: 5px; display: flex; align-items: center; justify-content: flex-end">
      <a-pagination
        v-model:current="pagination.current"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :show-total="() => `共 ${pagination.total} 条数据`"
        :show-size-changer="pagination.showSizeChanger"
        :show-quick-jumper="pagination.showQuickJumper"
        :page-size-options="pagination.pageSizeOptions"
        @showSizeChange="handlePageSizeChange"
        @change="handlePageChange"
      >
        <template #buildOptionText="props">
          <span>{{ props.value }}条/页</span>
        </template>
      </a-pagination>
    </div>
  </div>
  <a-drawer :title="title" :width="1400" :size="'large'" :open="open" @close="onClose">
    <template #extra>
      <!-- <a-button style="margin-right: 8px" @click="onClose">重置</a-button> -->
      <a-button type="primary" @click="tijiao">提交</a-button>
    </template>
    <div style="width: 100%; height: 100%">
      <a-form :label-col="labelCol" :wrapper-col="wrapperCol" style="width: 100%; height: 100%" ref="formPackage" :model="projectForm">
        <a-row>
          <a-col :span="24"><a-divider>当前数据库:{{ selectedLabel }}</a-divider></a-col>
        </a-row>
        <a-row>
          <a-col :span="8"
            ><a-form-item label="表名称"> <a-input v-model:value="projectForm.tableInfo.newTable" /> </a-form-item
          ></a-col>
          <a-col :span="8">
            <a-form-item label="表说明">
              <a-input v-model:value="projectForm.tableInfo.tableName" />
            </a-form-item>
          </a-col>
          <a-col :span="8">
            <a-form-item label="常用字段">
              <a-select v-model:value="fievalue" mode="tags" style="width: 100%" placeholder="常用字段" :options="fieoptions" @change="handleChange"></a-select>
            </a-form-item>
          </a-col>
        </a-row>
        <a-row>
          <a-col :span="24">
            <a-divider
              >表格列配置<a-button :size="'small'" type="primary" :icon="h(PlusOutlined)" style="margin-left: 5px" @click="zancunAdd()"
                >新增列</a-button
              ></a-divider
            ></a-col
          >
        </a-row>
        <div style="width: 100%">
          <a-table
            :columns="zancuncolumns"
            :scroll="{ x: 1300, y: 'calc(100vh - 300px)' }"
            :data-source="projectForm.tableFieldList"
            :pagination="false"
            size="small"
          >
            <template #bodyCell="{ text, column, index, record }">
              <template v-if="['primaryKey', 'allowNull'].includes(column.dataIndex)">
                <a-form-item style="display: flex; justify-content: center">
                  <a-switch v-model:checked="record[column.dataIndex]" />
                </a-form-item>
              </template>
              <template v-if="['dataType'].includes(column.dataIndex)">
                <a-form-item style="display: flex; justify-content: center">
                  <a-select
                    v-model:value="record[column.dataIndex]"
                    placeholder="请选择"
                    style="width: 120px; height: 35px; margin-right: 10px"
                    :options="typeOptions"
                  ></a-select>
                </a-form-item>
              </template>

              <template v-if="['dataLength', 'field', 'fieldName'].includes(column.dataIndex)">
                <a-form-item style="display: flex; justify-content: center">
                  <a-input style="" v-model:value="record[column.dataIndex]" />
                </a-form-item>
              </template>

              <template v-if="column.key === 'operation'">
                <!-- <a-button :size="'small'" :icon="h(EditOutlined)" @click="zanEdit(record)" style="display: inline; margin-right: 10px"></a-button> -->
                <a-button
                  :size="'small'"
                  :icon="h(DeleteOutlined)"
                  @click="zanDel(record)"
                  type="primary"
                  danger
                  style="display: inline; margin-right: 10px"
                ></a-button>
              </template>
            </template>
          </a-table>
        </div>
      </a-form>
    </div>
  </a-drawer>
  <div>
    <a-modal width="1500px" style="top: 40px" v-model:open="openmodel" title="常用字段" @ok="handleOk">
      <template #footer>
        <!-- <a-button key="back" @click="handleCancel">Return</a-button>
        <a-button key="submit" type="primary" :loading="loading" @click="handleOk">Submit</a-button> -->
      </template>
      <CommonFields></CommonFields>
    </a-modal>
  </div>
</template>
<script setup>
import {
  VerticalAlignBottomOutlined,
  SearchOutlined,
  SaveOutlined,
  PlusOutlined,
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  FilterOutlined,
  RedoOutlined
} from '@ant-design/icons-vue'
import { _guid } from '@/utils/guid.js'
import { ref, h, watch, createVNode, onMounted,computed,nextTick } from 'vue'
import { useGetFieldsXiaLa } from '@/api/axiosExample.js'
import CommonFields from '@/views/DataModeling/CommonFields.vue'
import { useUpdateTable, getDBTables, useAddUserAsync, getDBXia, usedelAsync, useGetTableAsync } from '@/api/datamodeling/modelingpage.js'
import { message, Form, Modal } from 'ant-design-vue'
import dayjs from 'dayjs'

const loading = ref(false)
const openmodel = ref(false)
const showModal = () => {
  openmodel.value = true
}
const handleOk = () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    openmodel.value = false
  }, 2000)
}
const handleCancel = () => {
  openmodel.value = false
}
const labelCol = {
  style: {
    width: '100px'
  }
}
const wrapperCol = {
  span: 15
}
const deltable = (record) => {
  if (record.name == null || record.name == '') {
    message.warning('表名不能为空')
  }
  if (xiavalue.value == null || xiavalue.value == '') {
    message.warning('请选择数据库')
  }
  usedelAsync(xiavalue.value, record.name).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      getTableData()
    }
  })
}
const title = ref('新增')
const edit = async (record) => {
  await fieidsXiaLa()
  st.value = 'update'
  title.value = '修改'
  useGetTableAsync(xiavalue.value, record.name).then((data) => {
    if (data.code === 200 && data.success) {
      open.value = true
      resetFields()
      projectForm.value.linkId = xiavalue.value
      projectForm.value.tableInfo = data.data.tableInfo
      data.data.tableFieldList.forEach((element) => {
        element.primaryKey = element.primaryKey == 1 ? true : false
        element.allowNull = element.allowNull == 1 ? true : false
        projectForm.value.tableFieldList.push({ index: _guid(), ...element })

        if (fieoptions.value.filter((item) => item.value == element.field).length == 1) {
          fievalue.value.push(element.field)
        }
      })
    }
  })
}
const xialaarry = ref([])
const fieidsXiaLa = async () => {
  const data = await useGetFieldsXiaLa()
  if (data.code === 200 && data.success) {
    fieoptions.value = []
    xialaarry.value = []
    data.data.forEach((element) => {
      xialaarry.value.push(element.columnName)
      fieoptions.value.push({
        value: element.columnName,
        label: element.columnName + '(' + element.remarks + ')',
        fdata: { ...element }
      })
    })
  }
}

const zanDel = (record) => {
  fievalue.value = fievalue.value.filter((item) => item != record.field)

  projectForm.value.tableFieldList = projectForm.value.tableFieldList.filter((p) => p.index != record.index)
}
const st = ref('add')
const tijiao = () => {
  projectForm.value.tableFieldList.forEach((element) => {
    element.primaryKey = element.primaryKey === true ? 1 : 0
    element.allowNull = element.allowNull === true ? 1 : 0
  })
  
  if(st.value === 'add')
  {
  useAddUserAsync(projectForm.value).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      resetFields()
      getTableData()
      open.value = false
      fievalue.value = []
    } else {
      message.error(data.message)
    }
  })

  }else{
onCommit()

  }

}

onMounted(() => {
  document.querySelectorAll('.ant-table-body').forEach((element) => {
    element.style.height = element.style.maxHeight
  })
})

const tableLoading = ref(false)  // 添加 loading 状态
const projectNameInput = ref('')
async function getTableData() {
  var obj = {
    linkId: xiavalue.value,
    tableName: projectNameInput.value,
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }
 tableLoading.value = true  // 开始加载，显示动画
  try {
    await getDBTables(obj).then((res) => {
      if (res.code == 200 && res.success) {
        dataSource.value = res.data
        pagination.value.total = res.total
      }
    })
  } catch (error) {
    dataSource.value = []
    message.error(`查询失败`)
  }finally {
    tableLoading.value = false  // 结束加载，隐藏动画
  }
}

const dataSource = ref([])
const columns = [
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    customRender: (obj) => {
      return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
    },
    align: 'center'
  },
  {
    title: '表名',
    dataIndex: 'name',
    align: 'center',
    key: 'name',
    width: 100
  },
  {
    title: '描述',
    dataIndex: 'description',
    align: 'center',
    key: 'type',
    width: 100
  },
  {
    title: '操作',
    key: 'operation',
    align: 'center',
    fixed: 'right',
    width: 70
  }
]
const requery = () => {
  projectNameInput.value = ''
  getTableData()
}
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  getTableData()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.pageSize = currentPage
  getTableData()
}
const pagination = ref({
  position: ['bottomRight'],
  current: 1, // 当前页数
  pageSize: 10, // 每页条数
  total: 0, // 数据总数
  showSizeChanger: true, // 是否可以改变每页条数
  showQuickJumper: true, // 是否可以快速跳转至某页
  onChange: handlePageChange, // 页码改变的回调
  onShowSizeChange: handlePageSizeChange, // 每页条数改变的回调
  showTotal: (total) => `共 ${total} 条数据`,
  pageSizeOptions: ['10', '20', '30', '40', '50']
})

const options = ref([{
      value: '0',
      label: '默认数据库'
    }])
import { getConnectList } from '@/api/demoApi/configDB'
const xiavalue = ref(undefined)
const xiala = () => {
  // getDBXia().then((res) => {
  //   if (res.code == 200 && res.success) {
  //     options.value = []
  //     res.data.forEach((element) => {
  //       options.value.push({
  //         value: element.dbConfigId,
  //         label: element.name
  //       })
  //     })
  //     xiavalue.value = res.data[0].dbConfigId
  //     selectedLabel.value = res.data[0].name
  //     getTableData()
  //   }
  // })
  getConnectList().then((res) => {
    if (res.code == 200 && res.success) {
      options.value = []
      options.value.push(
        ...res.data.map((group) => ({
          label: group.dbType,
          options: group.items.map((item) => ({
            value: item.dbConfigId,
            label: item.name
          }))
        }))
      )
      xiavalue.value = res.data[0].items[0].dbConfigId
      selectedLabel.value =  res.data[0].items[0].name
      getTableData()
    } else {
      message.error('获取数据连接异常')
    }
  })

}
xiala()

const open = ref(false)
const showDrawer = async (val) => {
  await fieidsXiaLa()
  st.value = 'add'
  title.value = '新增'
  resetFields()
  projectForm.value.linkId = xiavalue.value
  open.value = true
}
const onClose = () => {
  open.value = false
  fievalue.value = []
  resetFields()
}

const zancuncolumns = ref([
  {
    title: '列名',
    dataIndex: 'field',
    align: 'center',
    key: 'field',
    width: 140
  },
  {
    title: '类型',
    dataIndex: 'dataType',
    align: 'center',
    key: 'dataType',
    width: 140
  },
  {
    title: '长度',
    dataIndex: 'dataLength',
    align: 'center',
    key: 'dataLength',
    width: 100
  },
  {
    title: '是否主键',
    dataIndex: 'primaryKey',
    align: 'center',
    key: 'primaryKey',
    width: 120
  },
  {
    title: '允许为空',
    dataIndex: 'allowNull',
    align: 'center',
    key: 'allowNull',
    width: 120
  },
  {
    title: '说明',
    dataIndex: 'fieldName',
    align: 'center',
    key: 'fieldName',
    width: 100
  },

  {
    title: '操作',
    key: 'operation',
    align: 'center',
    fixed: 'right',
    width: 50
  }
])

const projectForm = ref({
  linkId: '',
  tableInfo: {
    table: '',
    newTable: '',
    tableName: ''
  },
  tableFieldList: [],
  hasTableData: true
})
const resetFields = () => {
  projectForm.value = {
    linkId: '',
    tableInfo: {
      table: '',
      newTable: '',
      tableName: ''
    },
    tableFieldList: [],
    hasTableData: true
  }
}
const tableFieldOutput = ref({
  index: '',
  primaryKey: false,
  allowNull: true,
  dataLength: 0,
  dataType: '',
  field: '',
  fieldName: '',
  identity: 0
})
const zancunAdd = () => {
  tableFieldOutput.value.index = _guid()
  projectForm.value.tableFieldList.unshift({ ...tableFieldOutput.value })
}

const typeOptions = ref([
  {
    value: 'varchar',
    label: '字符串'
  },
  {
    value: 'int',
    label: '整型'
  },
  {
    value: 'datetime',
    label: '日期时间'
  },
  {
    value: 'decimal',
    label: '浮点'
  },
  {
    value: 'bigint',
    label: '长整型'
  },
  {
    value: 'text',
    label: '文本'
  }
])

const fievalue = ref([])
const fieoptions = ref([])
const handleChange = (value, option) => {
  projectForm.value.tableFieldList = projectForm.value.tableFieldList.filter((item) => !xialaarry.value.includes(item.field))
  option.forEach((element) => {
    projectForm.value.tableFieldList.unshift({
      index: _guid(),
      primaryKey: false,
      allowNull: element.fdata.isEmpty,
      dataLength: element.fdata.length,
      dataType: element.fdata.type,
      field: element.fdata.columnName,
      fieldName: element.fdata.remarks,
      identity: 0
    })
  })
}


const panelHeight = ref(0)
const calcHeight = computed(() => {
  return `calc(100vh - 96px - 53px - 21px - ${panelHeight.value}px - 50px - 35px)`
})
// 监听窗口变化，动态调整面板高度
const updatePanelHeight = async () => {
  if (isExpand.value) {
    panelHeight.value = 42
  } else {
    panelHeight.value = 0
  }
}

const isExpand = ref(false)
const accordionRef = ref(null)
async function toggleAccordion() {
  isExpand.value = !isExpand.value
  if (accordionRef.value) {
    accordionRef.value.classList.toggle('open')
  }
  await nextTick() // 确保 DOM 更新后获取新高度
  updatePanelHeight()
}

function onCommit() {
  Modal.confirm({
    title: '提交提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '更改表信息将清空该表数据，确认更改吗?',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
         projectForm.value.tableFieldList.forEach((element) => {
    element.primaryKey = element.primaryKey === true ? 1 : 0
    element.allowNull = element.allowNull === true ? 1 : 0
  })
          useUpdateTable(projectForm.value).then((data) => {
    if (data.code === 200 && data.success) {
      resolve()
      message.success(data.message)
      resetFields()
      getTableData()
      open.value = false
      fievalue.value = []
    } else {
      message.error(data.message)
    }
  })
      }).catch(() => message.error('更改异常!'))
    }
  })
}

const selectedLabel = ref('')
// 监听选择变化
const handleSelectChange = (value, option) => {
  selectedLabel.value = option?.label
}


</script>
<style lang="scss">
.SurelyTableDemo {
  background-color: #fff;
  height: 100%;
  overflow: auto;
  position: relative;

  .ant-typography {
    margin-bottom: 0px;
  }

  .ant-divider-horizontal {
    margin: 10px 0px;
  }
}
.accordion{

}
.accordion-content{
   display: flex;
   justify-content:flex-end
}
 </style>