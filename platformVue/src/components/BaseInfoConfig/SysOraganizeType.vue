<template>
  <div class="right_type">
    <div class="right_type_btn">
      <a-input v-model:value="type_input" style="width: 230px; height: 32px; margin: 3px 4px 3px 8px" placeholder="请输入部门类型名称" />
      <a-button v-if="typeQuery.isShow" style="margin: 3px 4px" :icon="showicon(typeQuery.Icon)" @click="type_qurety">{{ typeQuery.name }}</a-button>
      <a-button v-if="typeReset.isShow" style="margin: 3px 4px" :icon="showicon(typeReset.Icon)" @click="type_resetqurety">{{ typeReset.name }}</a-button>
      <a-button v-if="addType.isShow" class="editable-add-btn" type="primary" :icon="showicon(addType.Icon)" style="margin: 3px 4px" @click="type_add">{{
        addType.name
      }}</a-button>
    </div>

    <a-table :columns="columns" :data-source="dataSource" :scroll="{ y: 'calc(100vh - 325px)' }" :pagination="false" bordered>
      <template #bodyCell="{ column, text, record }">
        <template v-if="['name', 'sort', 'remark'].includes(column.dataIndex)">
          <div>
            <a-input v-if="editableData[record.id]" v-model:value="editableData[record.id][column.dataIndex]" style="margin: -5px 0" />
            <template v-else> {{ text }} </template>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'operation'">
          <div class="editable-row-operations">
            <span v-if="editableData[record.id]">
              <a-button @click="save(record.id)" :icon="h(SaveOutlined)" style="display: inline; margin-right: 10px"></a-button>
              <a-popconfirm title="确定取消?" @confirm="cancel(record.id)">
                <a-button :icon="h(CloseOutlined)" style="display: inline"></a-button>
              </a-popconfirm>
            </span>
            <span v-else>
              <a-button v-if="uptType.isShow" @click="edit(record.id)" :icon="showicon(uptType.Icon)" style="display: inline; margin-right: 10px"></a-button>
              <a-button
                @click="deleteType(record.id)"
                type="primary"
                danger
                v-if="delType.isShow"
                :icon="showicon(delType.Icon)"
                style="display: inline"
              ></a-button>
            </span>
          </div>
        </template>
      </template>
    </a-table>
    <div class="fenye">
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
      </a-pagination>
    </div>
  </div>
  <div>
    <a-modal v-model:open="open" title="部门类型添加" @ok="handleOk" @cancel="handlecancel" ok-text="确认" cancel-text="取消">
      <a-divider style="height: 1px; background-color: #70716c" />
      <div>
        <a-form :model="formState" name="basic" :label-col="{ span: 8 }" :wrapper-col="{ span: 16 }" autocomplete="off">
          <a-row>
            <a-col :span="20">
              <a-form-item label="部门类型" v-bind="validateInfos.name">
                <a-input v-model:value="formState.name" />
              </a-form-item>
            </a-col>
          </a-row>
          <a-row
            ><a-col :span="20">
              <a-form-item label="排序" v-bind="validateInfos.sort">
                <a-input v-model:value="formState.sort" />
              </a-form-item> </a-col
          ></a-row>
          <a-row
            ><a-col :span="20">
              <a-form-item label="备注" v-bind="validateInfos.remark">
                <a-input v-model:value="formState.remark" />
              </a-form-item> </a-col
          ></a-row>
        </a-form>
      </div>
    </a-modal>
  </div>
</template>

<script setup>
import {
  useGetSysOraganizeTypeAsync,
  AddSysInfoTypeAsync,
  useUpdateSysInfoTypeAsync,
  useSoftSysOraganizeTypeAsync
} from '@/api/BaseInfoConfig/organizationType'
import { message, Modal } from 'ant-design-vue'
import { Form } from 'ant-design-vue'
import { cloneDeep } from 'lodash-es'
import { h, onMounted, reactive, ref, createVNode } from 'vue'
import { SaveOutlined, SearchOutlined, EditOutlined, CloseOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
onMounted(() => {
  document.querySelectorAll('.ant-table-body').forEach((element) => {
    element.style.height = element.style.maxHeight
  })
})

const props = defineProps({
  typeQuery: Object,
  typeReset: Object,
  addType: Object,
  uptType: Object,
  delType: Object,
  showicon: Function
})

const useForm = Form.useForm
const type_input = ref('')
const type_qurety = () => {
  systype(type_input.value)
}
const type_resetqurety = () => {
  type_input.value = ''
  systype(type_input.value)
}

const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  systype(type_input.value)
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  systype(type_input.value)
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

const deleteType = async (id) => {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该部门类型吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        useSoftSysOraganizeTypeAsync(id)
          .then((data) => {
            if (data.code === 200 && data.success) {
              message.success(data.message)
              systype(type_input.value)
              resolve()
            } else {
              message.error(data.message)
            }
          })
          .catch((error) => {
            console.error(error)
            message.error('删除异常!')
            reject(error)
          })
      })
    }
  })
}

let typedata = []
const systype = async (typename) => {
  const query = {
    name: typename,
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }
  const data = await useGetSysOraganizeTypeAsync(query)
  if (data.code === 200 && data.success) {
    dataSource.value = typedata = data.data
    pagination.value.total = data.total
  } else {
    message.error(data.message)
  }
}
systype(type_input.value)
const columns = [
  {
    title: '部门类型名称',
    dataIndex: 'name',
    width: '15%'
  },
  {
    title: '排序',
    dataIndex: 'sort'
  },
  {
    title: '备注',
    dataIndex: 'remark'
  },
  {
    title: '修改人',
    dataIndex: 'modifyName',
    width: '12%'
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    width: '20%',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    dataIndex: 'operation',
    width: '15%'
  }
]

const dataSource = ref([])
const editableData = reactive({})

const edit = (key) => {
  editableData[key] = cloneDeep(typedata.filter((item) => key === item.id)[0])
}

const save = async (key) => {
  if (editableData[key].name == null || editableData[key].name == undefined || editableData[key].name.trim() == '') {
    message.warning('部门类型不能为空!')
    return
  }
  if (isNaN(editableData[key].sort)) {
    message.warning('排序必须是数字!')
    return
  }
  const data = await useUpdateSysInfoTypeAsync(editableData[key])
  if (data.code === 200 && data.success) {
    message.success(data.message)
    systype(type_input.value)
  } else {
    message.error(data.message)
  }
  delete editableData[key]
}

const cancel = (key) => {
  delete editableData[key]
}

//******************************************************
const add = async () => {
  const data = await AddSysInfoTypeAsync(formState.value)
  if (data.code === 200 && data.success) {
    message.success(data.message)
    open.value = false
    handlecancel()
    systype(type_input.value)
  } else {
    message.error(data.message)
  }
}
const formState = ref({
  name: '',
  remark: '',
  sort: '0'
})
const open = ref(false)
const type_add = () => {
  open.value = true
}
const handleOk = () => {
  validate()
    .then(() => {
      add()
    })
    .catch((err) => {
      console.log('error', err)
    })
}

const handlecancel = () => {
  formState.value = {
    name: '',
    remark: '',
    sort: '0'
  }
}

const rulesRef = reactive({
  name: [
    {
      required: true,
      message: '请输入部门类型'
    }
  ],
  sort: [
    {
      validator: (rule, value) => {
        if (value === '' || /^[0-9]+$/.test(value)) {
          if (value === '' || (value >= 0 && value <= 9999999)) {
            return Promise.resolve()
          } else {
            return Promise.reject(new Error('排序号必须在 0 到 9999999 之间'))
          }
        } else {
          return Promise.reject(new Error('排序号必须为自然数'))
        }
      }
    }
  ]
})
const { validate, validateInfos } = useForm(formState, rulesRef)
</script>
<style scoped lang="scss">
:where(.css-dev-only-do-not-override-1hsjdkk).ant-table-wrapper .ant-table {
  font-size: 13px;
}
.right_type {
  position: relative;
  width: 100%;
  height: 100%;
  overflow: auto;
}
.editable-row-operations a {
  height: 100%;
  margin-right: 8px;
}
.right_type_btn {
  display: flex;
}

.fenye {
  position: absolute;
  bottom: 5px;
  right: 5px;
  height: 33px;
}
</style>
