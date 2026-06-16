<template>
  <div class="UserLoginLogs">
    <div style="line-height: 34px; margin: 0px 0px 5px 20px; padding-top: 5px">
      <span>
        IP地址：
        <a-input v-model:value="ipKey" placeholder="请输入IP地址" style="width: 250px" />
      </span>
      <span style="margin-left: 20px">
        登录时间：
        <a-date-picker
          show-time
          v-model:value="beginTime"
          valueFormat="YYYY-MM-DD HH:mm:ss"
          placeholder="选择开始时间"
          @change="onChangeBegin"
          style="margin-right: 5px"
        />
        <a-date-picker v-model:value="endTime" valueFormat="YYYY-MM-DD HH:mm:ss" show-time placeholder="选择截止时间" @change="onChangeEnd" />
      </span>
      <span style="margin-left: 20px">
        登录人：
        <a-select
          v-model:value="loginNameValue"
          show-search
          allowClear
          placeholder="选择登陆姓名"
          style="width: 200px"
          :options="filteredOptions"
          :filter-option="false"
          @search="handleSearch"
          @change="handleChange"
        ></a-select>
      </span>
      <CustomButtonList
        :ParamsRoleId="inputRoleId"
        :ParamsFunctionName="inputFunctionName"
        :BtnFunctionNameArray="[]"
        @buttonClick="handleButtonClick"
      ></CustomButtonList>
      <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData(true)">查询数据</a-button>
      <a-button style="margin-left: 10px" :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      <a-button style="margin-left: 10px" type="primary" danger :icon="h(CloseOutlined)" @click="delAllData">删除全部</a-button>
      <a-button style="margin-left: 10px" type="primary" danger :icon="h(CloseOutlined)" @click="delSelectData">删除所选</a-button> -->
    </div>
    <a-table
      :row-selection="rowSelection"
      :columns="dataColumns"
      :data-source="data"
      :pagination="false"
      :scroll="{ y: 'calc(100vh - 64px - 36px - 34px - 10px - 55px - 35px - 40px - 10px)' }"
    >
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'loginTime' || column.dataIndex === 'exitTime'">
          {{ formatDateTime(text) }}
        </template>
      </template>
    </a-table>
    <div class="paginationStyle">
      <a-pagination
        v-model:current="currentPage"
        v-model:page-size="currentPageSize"
        :total="totalCount"
        :show-total="(total) => `共 ${totalCount} 条`"
        @change="pageSizeChange"
      />
    </div>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ExclamationCircleOutlined } from '@ant-design/icons-vue'
// import { SearchOutlined, RedoOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { ref, h, onMounted, createVNode, computed, watchEffect, defineOptions } from 'vue'
import { getuserLoginLogs, deleteAllApi, deleteSelectApi } from '@/api/userLoginLogs'
import { message, Modal } from 'ant-design-vue'
import { getUserInfo } from '@/api/commonFun'
import { debounce } from 'lodash-es'
import { formatDateTime } from '@/utils/dateUtils'
/* 页面缓存 */
defineOptions({
  name: 'A0302'
})
/* 动态按钮相关 */
import { useUserStore } from '@/store/user'

import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
const { globalStore } = useGlobalState()
var userStore = ref({})
var drawerStore = ref({})
var inputRoleId = []
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
  drawerStore = useDrawerStore()
  inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      drawerStore.value = globalStore.value.drawerStore
      inputRoleId = userStore.value.userRoles
    }
  })
}

/* 下拉选择器 */
const loginNameValue = ref(null)
const searchQuery = ref('')
const sysUserOptions = ref([])
const filteredOptions = computed(() => {
  if (searchQuery.value) {
    return sysUserOptions.value.filter((option) => option.label.toLowerCase().includes(searchQuery.value.toLowerCase()))
  }
  return sysUserOptions.value
})

const handleSearch = debounce((value) => {
  searchQuery.value = value
}, 500)
const handleChange = (value) => {
  loginNameValue.value = value
}

const inputFunctionName = ref('A0302')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData(true)') {
    getTableData(true)
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'delAllData') {
    delAllData()
  }
  if (functionName === 'delSelectData') {
    delSelectData()
  }
}
const dataColumns = [
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    customRender: (obj) => {
      return (currentPage.value - 1) * currentPageSize.value + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: 'IP地址',
    dataIndex: 'ipAddress',
    key: 'ipAddress'
  },
  {
    title: '登录时间',
    dataIndex: 'loginTime',
    key: 'loginTime'
  },
  {
    title: '退出时间',
    dataIndex: 'exitTime',
    key: 'exitTime'
  },
  {
    title: '登录账号',
    dataIndex: 'loginName',
    key: 'loginName'
  },
  {
    title: '姓名',
    dataIndex: 'realName',
    key: 'realName'
  }
]
const data = ref([])
const ipKey = ref('')
const beginTime = ref(null)
const endTime = ref(null)
var currentPageSize = ref(10)
var currentPage = ref(1)
var totalCount = ref(0)

const onChangeBegin = (value, dateString) => {
  beginTime.value = dateString
}
const onChangeEnd = (value, dateString) => {
  endTime.value = dateString
}

var selectedRowIds = [] //选中的Id集合
const rowSelection = {
  onChange: (selectedRowKeys, selectedRows) => {},
  onSelect: (record, selected, selectedRows) => {
    if (selectedRows.length > 0) {
      selectedRowIds = []
      selectedRows.forEach((element) => {
        selectedRowIds.push(element.id)
      })
    }
  },
  onSelectAll: (selected, selectedRows, changeRows) => {
    selectedRowIds = []
    if (selected) {
      changeRows.forEach((element) => {
        selectedRowIds.push(element.id)
      })
    }
  }
}

getTableData(true)
getUserList()
onMounted(() => {})

async function getUserList() {
  await getUserInfo().then((res) => {
    if (res.code == 200 && res.success) {
      sysUserOptions.value = []
      res.data.forEach((element) => {
        sysUserOptions.value.push({ value: element.id, label: element.realName })
      })
    }
  })
}
function resetSearch() {
  ipKey.value = ''
  beginTime.value = null
  endTime.value = null
  loginNameValue.value = null
}

function pageSizeChange(page, pageSize) {
  currentPage.value = page
  currentPageSize.value = pageSize
  getTableData(true)
}

async function getTableData(isSerch) {
  var obj = {
    ip: ipKey.value.trim(),
    userid: loginNameValue.value,
    loginTime: beginTime.value,
    exitTime: endTime.value,
    pageIndex: currentPage.value,
    pageSize: currentPageSize.value
  }
  await getuserLoginLogs(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = item.id
      })
      data.value = res.data
      totalCount.value = res.total
      if (isSerch) {
        message.success('查询完成')
      }
    }
  })
}

/* 删除全部 */
function delAllData() {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除全部数据吗？该操作不可撤回！',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = delAll(resolve, reject)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}
async function delAll(resolve, reject) {
  await deleteAllApi().then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData(false)
    }
  })
}
/* 删除所选 */
function delSelectData() {
  if (selectedRowIds.length == 0) {
    message.warning('请选择数据后重试！')
    return
  }
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除选中的这些数据吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = delSelect(resolve, reject)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}
async function delSelect(resolve, reject) {
  await deleteSelectApi(selectedRowIds).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData(false)
    }
  })
}
</script>

<style lang="scss">
.UserLoginLogs {
  background-color: #fff;
  height: 100%;
  position: relative;

  .paginationStyle {
    text-align: right;
    position: absolute;
    right: 30px;
    bottom: 10px;
    width: 100%;
  }

  .ant-table-container {
    width: 100%;
  }
  .ant-table-body {
    overflow-y: auto !important;
  }
}
</style>
