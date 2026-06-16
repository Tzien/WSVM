<template>
  <div v-show="userFormSt" class="right_upper">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol" name="nest-messages">
      <a-row>
        <a-col :span="24">
          <a-form-item label="部门名称" v-bind="validateInfos.name">
            <a-input v-model:value="formState.name" />
          </a-form-item>
        </a-col>
        <a-col :span="24">
          <a-form-item label="编码" v-bind="validateInfos.code">
            <a-input v-model:value="formState.code" />
          </a-form-item>
        </a-col>
      </a-row>
      <a-row>
        <a-col :span="24"
          ><a-form-item label="部门类型" v-bind="validateInfos.typeId">
            <div>
              <a-radio-group v-model:value="formState.typeId">
                <a-radio v-for="(option, index) in radioOptions" :key="index" :value="option.value">
                  {{ option.label }}
                </a-radio>
              </a-radio-group>
            </div>
          </a-form-item></a-col
        >
        <a-col :span="24"
          ><a-form-item label="是否激活">
            <div>
              <a-radio-group v-model:value="formState.status">
                <a-radio :value="true">正常</a-radio>
                <a-radio :value="false">冻结</a-radio>
              </a-radio-group>
            </div>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row>
        <a-col :span="24">
          <a-form-item label="排序" v-bind="validateInfos.sort"> <a-input v-model:value="formState.sort" /> </a-form-item
        ></a-col>
        <a-col :span="24">
          <a-form-item label="上级部门">
            <div>
              <a-tree-select
                :disabled="selecttree"
                v-model:value="formState.pid"
                v-model:searchValue="searchValue"
                show-search
                style="width: 100%"
                :dropdown-style="{ overflow: 'auto' }"
                placeholder="请选择上级部门"
                allow-clear
                tree-default-expand-all
                :tree-data="treeData"
                tree-node-filter-prop="label"
              >
                <template #title="{ value: val, label }">
                  <b v-if="val === 'parent 1-1'" style="color: #08c">sss</b>
                  <template v-else>
                    {{ label }}
                  </template>
                </template>
              </a-tree-select>
            </div>
          </a-form-item>
        </a-col>
      </a-row>
      <a-row>
        <a-col :span="24">
          <a-form-item label="备注">
            <a-textarea v-model:value="formState.remark" auto-size />
          </a-form-item>
        </a-col>
      </a-row>
    </a-form>
  </div>
  <div v-show="userTableSt" class="right_below">
    <div class="biao">
      <!-- :pagination="pagination" -->
      <a-table
        :customRow="classCustomRow"
        :columns="columns"
        :data-source="userdata"
        :pagination="false"
        :scroll="{ x: 1600, y: 'calc(100vh - 300px)' }"
        :expand-column-width="200"
      >
        <template #bodyCell="{ column, text }">
          <template v-if="column.key === 'sex'">
            <template v-if="text == 0">
              <span>男</span>
            </template>
            <template v-else>
              <span>女</span>
            </template>
          </template>
          <template v-if="column.key === 'enabled'">
            <template v-if="text">
              <span>正常</span>
            </template>
            <template v-else>
              <span>冻结</span>
            </template>
          </template>
        </template>
      </a-table>
    </div>
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
  <UserDetails :open="open" :userid="userid" @closeDetails="closeDetails" />
</template>
<script setup>
import { ref, onMounted, reactive, inject, watch } from 'vue'
import { Form } from 'ant-design-vue'
import { message } from 'ant-design-vue'
import {
  useGetUsersAsyncAsync,
  useGetOraganizeByIdAsync,
  useGetOraganizeTreeAsync,
  useUpdateSysOraganizeAsync,
  useAddSysOraganizeAsync
} from '@/api/BaseInfoConfig/organization'
import { useGetAllSysInfoTypeAsyncAsync } from '@/api/BaseInfoConfig/organizationType'

const labelCol = {
  style: {
    width: '100px'
  }
}
const wrapperCol = {
  span: 14
}
const userTableSt = ref(true)
const userFormSt = ref(true)
const useForm = Form.useForm
const props = defineProps({
  organizationid: String
})

const addoraganize = () => {
  resetFields()
  userTableSt.value = false
  oraganize(props.organizationid)
}
const detailsoraganize = () => {
  resetFields()
  userTableSt.value = true
}

const refreshTree = inject('refreshTree')

const saveoraganize = async () => {
  let data
  if (!userTableSt.value) {
    data = await useAddSysOraganizeAsync(formState.value)
    if (data.code === 200 && data.success) {
      refreshTree()
      message.success(data.message)
      oraganize()
      fetchRadioOptions()
      resetFields()
    } else {
      message.error(data.message)
    }
  } else {
    data = await useUpdateSysOraganizeAsync(formState.value)
    if (data.code === 200 && data.success) {
      refreshTree()
      message.success(data.message)
      oraganize()
      fetchRadioOptions()
      oraganizebyid(props.organizationid)
    } else {
      message.error(data.message)
    }
  }
}
//************************单选********************************* */
const fetchRadioOptions = async () => {
  const data = await useGetAllSysInfoTypeAsyncAsync()
  if (data.code === 200 && data.success) {
    if (data.data.length > 0) {
      radioOptions.value = data.data.map((x) => {
        return {
          label: x.name,
          value: x.id
        }
      })
    }
  } else {
    message.error('部门类型查询失败!')
  }
}
//************************表单树********************************* */
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    label: data.name,
    value: data.id,
    disabled: data.isReadonly,
    children: []
  }
  if (data.oraganizeTrees && data.oraganizeTrees.length) {
    data.oraganizeTrees.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}
const oraganize = async (oraganizeid) => {
  const data = await useGetOraganizeTreeAsync(oraganizeid)
  if (data.code === 200 && data.success) {
    treeData.value = []
    data.data.forEach((or) => {
      treeData.value.push(convertTree(or))
    })
  } else {
    message.error(data.message)
  }
}
//注意改位置
fetchRadioOptions()

const treeData = ref([])
const searchValue = ref('')
//************************************************ */
const formState = ref({
  id: '',
  name: '',
  code: '',
  typeId: '',
  status: true,
  pid: '',
  sort: 1,
  codepath: '',
  remark: ''
})
//*********************校验*********************** */
const rulesRef = reactive({
  name: [
    {
      required: true,
      validator: (rule, value) => {
        if (!value) {
          return Promise.reject(new Error('部门名称不能为空'))
        } else if (/\s/.test(value)) {
          return Promise.reject(new Error('输入不能包含空格'))
        } else if (value.length > 50) {
          return Promise.reject(new Error('部门名称不能超过50个字符'))
        } else {
          return Promise.resolve()
        }
      },
      trigger: 'change'
    }
  ],
  code: [
    {
      required: true,
      validator: (rule, value) => {
        if (!value) {
          return Promise.reject(new Error('部门编码不能为空'))
        } else if (/\s/.test(value)) {
          return Promise.reject(new Error('输入不能包含空格'))
        } else if (value.length > 10) {
          return Promise.reject(new Error('部门编码不能超过10个字符'))
        } else {
          return Promise.resolve()
        }
      },
      trigger: 'change'
    }
  ],
  typeId: [
    {
      required: true,
      message: '部门类型为必选项',
      trigger: 'change'
    }
  ],
  sort: [
    {
      required: true,
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

const { validate, validateInfos, resetFields } = useForm(formState, rulesRef)

const onOraganizeSubmit = () => {
  validate().then(() => {
    saveoraganize()
  })
}
//*********************部门表单控件信息*************************** */
const radioOptions = ref([])
defineExpose({
  detailsoraganize,
  addoraganize,
  onOraganizeSubmit
})
const selecttree = ref(false)
//************************************************ */
onMounted(async () => {
  if (props.organizationid !== '23300') {
    oraganizebyid(props.organizationid)
  } else {
    addoraganize()
  }
})
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  users()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  users()
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
const columns = [
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    customRender: (obj) => {
      return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: '用户名称',
    dataIndex: 'realName',
    key: 'realName',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '用户性别',
    dataIndex: 'sex',
    key: 'sex',
    align: 'center'
  },
  {
    title: '用户状态',
    dataIndex: 'enabled',
    key: 'enabled',
    align: 'center'
  },
  {
    title: '所在角色组',
    dataIndex: 'roles',
    key: 'roles',
    align: 'center'
  },
  {
    title: '用户邮箱',
    dataIndex: 'email',
    key: 'email',
    align: 'center'
  },
  {
    title: '用户手机',
    dataIndex: 'phone',
    key: 'phone',
    align: 'center'
  },
  {
    title: '用户微信',
    dataIndex: 'webChat',
    key: 'webChat',
    align: 'center'
  },
  {
    title: '用户钉钉',
    dataIndex: 'dingTalk',
    key: 'dingTalk',
    align: 'center'
  },
  {
    title: '用户备注',
    dataIndex: 'remark',
    key: 'remark',
    align: 'center'
  }
]
const userdata = ref([])
const oid = ref('')
const oraganizebyid = async (oraganizeid) => {
  oid.value = oraganizeid
  pagination.value.current = 1
  pagination.value.pageSize = 10
  const data = await useGetOraganizeByIdAsync(oraganizeid)
  if (data.code === 200 && data.success) {
    formState.value = data.data
    await oraganize(oraganizeid)
    await users()
  } else {
    message.error(data.message)
  }
}
const users = async () => {
  const query = {
    oraganizeid: oid.value,
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }
  const data = await useGetUsersAsyncAsync(query)
  if (data.code === 200 && data.success) {
    userdata.value = data.data
    pagination.value.total = data.total
  } else {
    message.error(data.message)
  }
}

watch(
  () => props.organizationid,
  (newValue) => {
    if (newValue === '23300') {
      addoraganize()
    } else {
      userTableSt.value = true
      oraganizebyid(newValue)
    }
  }
)

const open = ref(false)
const userid = ref('')
const classCustomRow = (record, index) => {
  return {
    onClick: (e) => {
      if (record.id) {
        open.value = true
        userid.value = record.id
      }
    }
  }
}

const closeDetails = (val) => {
  open.value = false
}

onMounted(() => {
  document.querySelectorAll('.ant-table-body').forEach((element) => {
    element.style.height = element.style.maxHeight
  })
})
</script>
<style scoped lang="scss">
:where(.css-dev-only-do-not-override-1hsjdkk).ant-form-item .ant-form-item-control-input-content {
  max-width: 112.5%;
}

.right_upper {
  padding-top: 10px;
  width: 35%;
  height: 100%;
  color: #999;
  padding-left: 20px;
  overflow: auto;
}

.right_below {
  position: relative;
  width: 65%;
  height: 100%;
  color: #999;
  overflow: auto;
}

.fenye {
  position: absolute;
  bottom: 5px;
  right: 5px;
  height: 33px;
}
</style>
