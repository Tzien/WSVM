<template>
  <div class="Task">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>任务调度管理</p>
      </div>
      <div class="headerRight">
        <a-p>任务名称：</a-p>
        <a-input style="width: 150px" v-model:value="name" placeholder="请输入任务名称" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A050103', 'A050102', 'A050107', 'A050105']"
          @buttonClick="handleButtonClick"
        >
        </CustomButtonList>
      </div>
    </div>
    <a-table
      :pagination="false"
      size="large"
      :columns="columns"
      :data-source="data"
      bordered
      ref="myTable"
      :scroll="{ y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
      :expand-column-width="100"
      :row-selection="rowSelection"
    >
      <template #bodyCell="{ text, column, record }">
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="detail(record.taskDetailId)" v-if="allDyBtn.find((item) => item.functionCode === 'A050106')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A050106')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.taskDetailId)" v-if="allDyBtn.find((item) => item.functionCode === 'A050101')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A050101')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
        </template>
        <!-- 处理状态列 -->
        <template v-if="column.dataIndex === 'isStart'">
          <a @click="setStart(record.taskDetailId, record.isStart)">
            <CustomSwitch
              :ParamsRoleId="inputRoleId"
              :ParamsFunctionName="inputFunctionName"
              :BtnFunctionNameArray="['A050104']"
              :IsOnlyIcon="true"
              :IsStart="record.isStart"
            >
            </CustomSwitch>
          </a>
        </template>
      </template>
    </a-table>

    <div class="paginationStyle">
      <a-pagination
        v-model:current="current"
        v-model:page-size="pageSize"
        :total="total"
        :show-total="(total) => `总条数：${total} `"
        class="pagination"
        show-size-changer
        @showSizeChange="onShowSizeChange"
        @change="pageSizeChange"
      />
    </div>
  </div>

  <!-- 抽屉 -->
  <a-drawer
    v-model:open="open"
    :title="drawerTitle"
    class="custom-class"
    root-class-name="root-class-name"
    :root-style="{ color: 'blue' }"
    style="color: red"
    size = 'large'
    placement="right"
  >
    <a-form ref="formRef" :model="formState" :label-col="labelCol" :rules="rules">
      <a-form-item label="任务名称" name="taskName">
        <a-input v-model:value="formState.taskName"  :disabled="!!formState.taskDetailId && formState.taskDetailId !== ''"/>
      </a-form-item>
      <a-form-item label="任务分组" name="taskGroup">
        <a-input v-model:value="formState.taskGroup"  :disabled="!!formState.taskDetailId && formState.taskDetailId !== ''" />
      </a-form-item>
      <a-form-item label="接口地址" name="apiUrl">
        <a-input v-model:value="formState.apiUrl"  />
      </a-form-item>
      <a-form-item label="接口类型" name="apiType">
        <a-select v-model:value="formState.apiType" :size="size" >
          <a-select-option value="GET">GET</a-select-option>
          <a-select-option value="POST">POST</a-select-option>
        </a-select>
      </a-form-item>
      <a-form-item label="接口参数" >
        <a-input v-model:value="formState.apiParams"  />
      </a-form-item>
      <a-form-item label="cron表达式" name="cron">
        <a-input v-model:value="formState.cron"  />
      </a-form-item>
      <a-form-item label="开始时间" name="startDate">
        <a-date-picker style="width: 100%;" format="YYYY-MM-DD HH:mm:ss" show-time v-model:value="formState.startDate"  :format="dateFormat" placeholder="" />
      </a-form-item>
      <a-form-item label="结束时间" name="endDate">
        <a-date-picker style="width: 100%;" format="YYYY-MM-DD HH:mm:ss" show-time v-model:value="formState.endDate"  :format="dateFormat" placeholder="" />
      </a-form-item>
      <!-- <a-form-item label="是否启动">
        <a-switch v-model:checked="formState.isStart" style="width: 200px" />
      </a-form-item> -->
      <a-form-item label="任务说明">
        <a-input v-model:value="formState.taskDesc"  />
      </a-form-item>
      <a-form-item label="最近执行记录">
        <a-input v-model:value="formState.history"  />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button class="NewAddBtnBG" type="primary" @click="save" style="margin-right: 50px">保存</a-button>
        <a-button @click="exit">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
</template>
<script setup>
import dayjs from 'dayjs'
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomSwitch from '@/components/commonComponents/CustomSwitch.vue'
import { ref, onMounted, h, reactive, createVNode, watchEffect } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import { addTaskApi, deleteTaskApi, updateTaskApi, getOneByIdApi, getAllTaskApi, startOrStopTaskApi, startSelectApi } from '@/api/Task/task'
import {
  SearchOutlined,
  RedoOutlined,
  PlusOutlined,
  DeleteOutlined,
  EditOutlined,
  TeamOutlined,
  ExclamationCircleOutlined,
  ShrinkOutlined
} from '@ant-design/icons-vue'
import { useUserStore } from '@/store/user'

import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
var userStore = ref({})
const { globalStore } = useGlobalState()
var inputRoleId = []
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
  inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      inputRoleId = userStore.value.userRoles
    }
  })
}
/* 页面缓存 */
defineOptions({
  name: 'A0501'
})
const inputFunctionName = ref('A0501')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0501',
    roleids: inputRoleId,
    btnCodes: []
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
      console.log('btns', allDyBtn.value)
    }
  })
}
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'showDrawer') {
    showDrawer()
  }
  if (functionName === 'search') {
    search()
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'startSelect') {
    startSelect()
  }
}
const rowSelection = ref({
  hideSelectAll: true,
  type: 'checkbox',
  selectedRowKeys: [],
  selectedRow: '',
  onChange: (selectedRowKeys, selectedRows) => {
    rowSelection.value.selectedRowKeys = selectedRowKeys
  }
})
onMounted(() => {
  getAll(current.value, pageSize.value, name.value)
  getAllButton()
})
const myTable = ref(null)

//抽屉
const open = ref(false)
//打开抽屉
const showDrawer = () => {
  open.value = true
  drawerTitle.value = '新增'
  formState.taskDetailId = ''
  formState.taskName = ''
  formState.taskGroup = ''
  formState.apiUrl = ''
  formState.apiType = ''
  formState.apiParams = ''
  formState.cron = ''
  formState.startDate = ''
  formState.endDate = ''
  formState.isStart = false
  formState.taskDesc = ''
  formState.history = ''
}
const drawerTitle = ref('')
const formState = reactive({
  taskDetailId: '',
  taskName: '',
  taskGroup: '',
  apiUrl: '',
  apiType: '',
  apiParams: '',
  cron: '',
  startDate: '',
  endDate: '',
  isStart: '',
  taskDesc: '',
  history: ''
})
const formRef = ref()
const labelCol = {
  style: {
    width: '100px'
  }
}

//退出子项抽屉
function exit() {
  open.value = false
}

const validate = async (_rule, value) => {
  if (value.trim() === '') {
    return Promise.reject('这是必填项')
  } else {
    return Promise.resolve()
  }
}
const rules = {
  taskName: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  taskGroup: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  apiUrl: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  apiType: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  cron: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  startDate: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  endDate: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ]
}
const taskDetailId = ref()
//保存配置信息
const save = async () => {
  formRef.value.validate().then(async () => {
    if (!taskDetailId.value) {
      //新增
      const response = await addTaskApi(formState)
      if (response.code === 200 && response.success) {
        message.success('保存成功')
        getAll(current.value, pageSize.value, name.value)
        open.value = false
      } else if (response.code === 200 && !response.success) {
        message.error(response.message)
      }
    } else {
      //编辑
      const response = await updateTaskApi(formState)
      if (response.code === 200 && response.success) {
        message.success('保存成功')
        getAll(current.value, pageSize.value, name.value)
        open.value = false
      } else if (response.code === 200 && !response.success) {
        message.error(response.message)
      }
    }
  })
}
//获取列表数据
const name = ref('')
async function getAll(page, size, name) {
  const query = {
    page: page,
    size: size,
    name: name.trim()
  }
  const response = await getAllTaskApi(query)
  if (response.code === 200 && response.success) {
    response.data.forEach((e) => {
      e.key = e.taskDetailId
    })
    data.value = response.data
    total.value = response.total
  }
}

//查询
function search() {
  getAll(current.value, pageSize.value, name.value)
}
//重置查询
function resetSearch() {
  name.value = ''
  getAll(current.value, pageSize.value, name.value)
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAll(current, pageSize, name.value)
}
const pageSizeChange = (page, pageSize) => {
  getAll(page, pageSize, name.value)
}
const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    fixed: 'left',
    width: 80,
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '任务名称',
    dataIndex: 'taskName',
    key: 'taskName',
    align: 'center'
  },
  {
    title: '任务分组',
    dataIndex: 'taskGroup',
    key: 'taskGroup',
    align: 'center'
  },
  {
    title: '接口地址',
    dataIndex: 'apiUrl',
    key: 'apiUrl',
    align: 'center'
  },
  {
    title: 'Cron表达式',
    dataIndex: 'cron',
    key: 'cron',
    align: 'center'
  },
  {
    title: '任务说明',
    dataIndex: 'taskDesc',
    key: 'taskDesc',
    align: 'center'
  },
  {
    title: '任务状态',
    dataIndex: 'isStart',
    key: 'isStart',
    align: 'center'
  },
  {
    title: '执行次数',
    dataIndex: 'executeCount',
    key: 'executeCount',
    align: 'center',
    width: '90px'
  },
  {
    title: '开始时间',
    dataIndex: 'startDate',
    key: 'startDate',
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '结束时间',
    dataIndex: 'endDate',
    key: 'endDate',
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    dataIndex: 'operation',
    width: 180,
    fixed: 'right',
    align: 'center'
  }
]
const data = ref(null)

//详情
async function detail(id) {
  const dateFormat = 'YYYY/MM/DD HH:mm:ss'
  open.value = true
  drawerTitle.value = '详情'
  formState.taskDetailId = ''
  formState.taskName = ''
  formState.taskGroup = ''
  formState.apiUrl = ''
  formState.apiType = ''
  formState.apiParams = ''
  formState.cron = ''
  formState.startDate = ''
  formState.endDate = ''
  // formState.isStart = false
  formState.taskDesc = ''
  formState.history = ''
  const params = {
    id: id
  }
  const response = await getOneByIdApi(params)
  if (response.code === 200 && response.success) {
    taskDetailId.value = response.data.taskDetailId
    formState.taskDetailId = response.data.taskDetailId
    formState.taskName = response.data.taskName
    formState.taskGroup = response.data.taskGroup
    formState.apiUrl = response.data.apiUrl
    formState.apiType = response.data.apiType
    formState.apiParams = response.data.apiParams
    formState.cron = response.data.cron
    formState.startDate = ref(dayjs(response.data.startDate, dateFormat))
    formState.endDate = ref(dayjs(response.data.endDate, dateFormat))
    formState.isStart = response.data.isStart
    formState.taskDesc = response.data.taskDesc
    formState.history = response.data.history
  }
}
//设置任务启停
async function setStart(id, isStart) {
  const params = {
    id: id
  }
  const response = await startOrStopTaskApi(params)
  if (response.code === 200 && response.success) {
    message.success(isStart ? '任务已停止' : '任务已启动')
    getAll(current.value, pageSize.value, name.value)
  }
}
//删除
async function del(resolve, reject, id) {
  const params = {
    id: id
  }
  console.info(params)
  await deleteTaskApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getAll(current.value, pageSize.value, name.value)
    } else if (res.code === 200 && !res.success) {
      message.error(res.message)
    }
  })
}
function onDelete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = del(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

//设置默认选中
async function startSelect() {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要设置的数据！')
    return
  }
  const params = {
    ids: rowSelection.value.selectedRowKeys.join(',')
  }
  const response = await startSelectApi(params)
  if (response.code === 200 && response.success) {
    message.success('启动成功')
    getAll(current.value, pageSize.value, name.value)
  }
}
</script>

<style lang="scss">
.Task {
  height: 100%;
  position: relative;

  .ant-table-container {
    width: 100%;
  }
  .ant-table-body {
    overflow-y: scroll !important;
    height: calc(100vh - 96px - 40px - 40px - 35px);
  }
  .ant-table-content {
    height: calc(100vh - 96px - 40px - 40px - 35px);
  }
  .headerLeft {
    float: left;
    height: 100%;
    display: flex;
    align-items: center;
  }

  .headerLeft > p {
    font-size: 16px;
    margin: 0;
    padding-left: 10px;
    font-weight: 600;
    color: black;
  }

  .headerRight {
    float: right;
    height: 100%;
    display: flex;
    align-items: center;
  }

  .headerRight > button {
    margin-left: 10px;
  }

  .headerRight > input {
    margin-right: 5px;
  }

  .paginationStyle {
    height: 40px;
    padding-right: 10px;
    position: absolute;
    right: 0px;
    bottom: 0px;
    width: 100%;
    align-items: center;
    display: flex;
    justify-content: flex-end;
  }
}
</style>
