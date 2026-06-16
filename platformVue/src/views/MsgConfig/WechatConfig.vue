<template>
  <div class="WechatConfig">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>微信消息配置</p>
      </div>
      <div class="headerRight">
        <a-p>corpId：</a-p>
        <a-input style="width: 150px" v-model:value="corpId" placeholder="corpId" />
        <a-p>agentId：</a-p>
        <a-input style="width: 150px" v-model:value="agentId" placeholder="agentId" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A040202', 'A040206', 'A040205', 'A040204']"
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
        <template v-if="column.dataIndex === 'corpSecret'">
          <div class="ellipsis">{{ record.corpSecret }}</div>
        </template>
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="detail(record.configId)" v-if="allDyBtn.find((item) => item.functionCode === 'A040201')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A040201')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.configId)" v-if="allDyBtn.find((item) => item.functionCode === 'A040203')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A040203')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
        </template>
        <!-- 处理状态列 -->
        <template v-if="column.dataIndex === 'isSelect'">
          <span v-if="text === true"> 是 </span>
          <span v-else> 否 </span>
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
      <a-form-item label="corpId" name="corpId">
        <a-input v-model:value="formState.corpId" />
      </a-form-item>
      <a-form-item label="CorpSecret" name="corpSecret">
        <a-input v-model:value="formState.corpSecret" />
      </a-form-item>
      <a-form-item label="AgentId" name="agentId">
        <a-input v-model:value="formState.agentId"  />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button class="NewAddBtnBG" type="primary" @click="save" style="margin-right: 50px">保存</a-button>
        <a-button @click="exit">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ref, onMounted, h, reactive, createVNode, watchEffect } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import { saveApi, getAllApi, getDetailApi, deleteApi, setSelectApi } from '@/api/Msg/wechatconfig'
import { ExclamationCircleOutlined } from '@ant-design/icons-vue'
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
  name: 'A0402'
})
const inputFunctionName = ref('A0402')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0402',
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
  if (functionName === 'setSelect') {
    setSelect()
  }
}
const rowSelection = ref({
  hideSelectAll: true,
  type: 'checkbox',
  selectedRowKeys: [],
  selectedRow: '',
  onChange: (selectedRowKeys, selectedRows) => {
    rowSelection.value.selectedRowKeys = selectedRowKeys.slice(-1)
    rowSelection.value.selectedRow = selectedRows.slice(-1)
  }
})
onMounted(() => {
  getAll(current.value, pageSize.value, corpId.value, agentId.value)
  getAllButton()
})
const myTable = ref(null)

//抽屉
const open = ref(false)
//打开抽屉
const showDrawer = () => {
  open.value = true
  drawerTitle.value = '新增'
  formState.configId = ''
  formState.corpId = ''
  formState.corpSecret = ''
  formState.agentId = ''
}
const drawerTitle = ref('')
const formState = reactive({
  configId: '',
  corpId: '',
  corpSecret: '',
  agentId: ''
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
  corpId: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  corpSecret: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  agentId: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ]
}
//保存配置信息
const save = async () => {
  formRef.value.validate().then(async () => {
    const response = await saveApi(formState)
    if (response.code === 200 && response.success) {
      message.success('保存成功')
      getAll(current.value, pageSize.value, corpId.value, agentId.value)
      open.value = false
    }
  })
}
//获取微信配置列表数据
const corpId = ref()
const agentId = ref()
async function getAll(page, size, corpId, agentId) {
  const query = {
    pageIndex: page,
    pageSize: size,
    corpId: corpId,
    agentId: agentId
  }
  const response = await getAllApi(query)
  if (response.code === 200 && response.success) {
    response.data.forEach((e) => {
      e.key = e.configId
    })
    data.value = response.data
    total.value = response.total
  }
}

//查询
function search() {
  getAll(current.value, pageSize.value, corpId.value, agentId.value)
}
//重置查询
function resetSearch() {
  corpId.value = ''
  agentId.value = ''
  getAll(current.value, pageSize.value, corpId.value, agentId.value)
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAll(current, pageSize, corpId.value, agentId.value)
}
const pageSizeChange = (page, pageSize) => {
  getAll(page, pageSize, corpId.value, agentId.value)
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
    title: 'CorpId',
    dataIndex: 'corpId',
    key: 'corpId',
    align: 'center'
  },
  {
    title: 'CorpSecret',
    dataIndex: 'corpSecret',
    key: 'corpSecret',
    align: 'center'
  },
  {
    title: 'AgentId',
    dataIndex: 'agentId',
    key: 'agentId',
    align: 'center'
  },
  {
    title: '是否为默认选中配置',
    dataIndex: 'isSelect',
    key: 'isSelect',
    align: 'center'
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
  open.value = true
  drawerTitle.value = '详情'
  formState.configId = ''
  formState.corpId = ''
  formState.corpSecret = ''
  formState.agentId = ''
  const params = {
    id: id
  }
  const response = await getDetailApi(params)
  if (response.code === 200 && response.success) {
    formState.configId = response.data.configId
    formState.corpId = response.data.corpId
    formState.corpSecret = response.data.corpSecret
    formState.agentId = response.data.agentId
  }
}

//删除
async function del(resolve, reject, id) {
  const params = {
    id: id
  }
  console.info(params)
  await deleteApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getAll(current.value, pageSize.value, corpId.value, agentId.value)
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
async function setSelect() {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要设置的数据！')
    return
  }
  const params = {
    id: rowSelection.value.selectedRow[0].configId
  }
  const response = await setSelectApi(params)
  if (response.code === 200 && response.success) {
    message.success('设置成功')
    getAll(current.value, pageSize.value, corpId.value, agentId.value)
  }
}
</script>

<style lang="scss">
.WechatConfig {
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

  .ellipsis {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
</style>
