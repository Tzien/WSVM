<template>
  <div class="SystemAccessLog">
    <div style="line-height: 34px; margin: 0px 0px 0px 20px; padding-top: 5px">
      <span>
        系统名称：
        <a-select v-model:value="sysNameValue" show-search allow-clear placeholder="请选择系统名称" style="width: 200px" :options="sysNameOptions"></a-select>
      </span>
      <span style="margin-left: 20px">
        IP地址：
        <a-input v-model:value="ipKey" placeholder="请输入IP地址" style="width: 250px" />
      </span>
      <span style="margin-left: 20px">
        访问时间：
        <a-date-picker
          show-time
          v-model:value="beginTime"
          valueFormat="YYYY-MM-DD HH:mm:ss"
          placeholder="起始于"
          @change="onChangeBegin"
          style="margin-right: 5px"
        />
        <a-date-picker show-time v-model:value="endTime" valueFormat="YYYY-MM-DD HH:mm:ss" placeholder="终止于" @change="onChangeEnd" />
      </span>
      <span style="margin-left: 20px">
        访问人姓名：
        <a-select
          v-model:value="inputPersonnelValue"
          show-search
          allowClear
          placeholder="请选择访问人"
          style="width: 200px"
          :options="filteredOptions"
          :filter-option="false"
          @search="handleSearch"
          @change="handleChange"
        ></a-select>
      </span>
    </div>
    <div style="line-height: 34px; padding-top: 5px; margin-bottom: 5px">
      <CustomButtonList
        :ParamsRoleId="inputRoleId"
        :ParamsFunctionName="inputFunctionName"
        :BtnFunctionNameArray="[]"
        @buttonClick="handleButtonClick"
      ></CustomButtonList>
      <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData(true)">查询数据</a-button>
      <a-button style="margin-left: 10px" :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      <a-button style="margin-left: 10px" type="primary" danger :icon="h(CloseOutlined)" @click="delAllData">删除全部</a-button>
      <a-button style="margin-left: 10px" type="primary" danger :icon="h(CloseOutlined)" @click="delSelectData">删除所选</a-button>
      <a-button style="margin-left: 10px" :icon="h(CloseOutlined)" @click="exportData">导出数据</a-button> -->
    </div>
    <a-table
      :row-selection="rowSelection"
      :columns="dataColumns"
      :data-source="data"
      :pagination="false"
      :scroll="{ y: 'calc(100vh - 64px - 36px - 34px - 34px - 5px - 10px - 55px - 35px - 40px - 10px)' }"
    >
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'createTime'">
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
import { message, Modal } from 'ant-design-vue'
import { getSysInfoRecord, deleteAllApi, deleteSelectApi } from '@/api/MenuSysLog'
import { getSysinfoList, getUserInfo } from '@/api/commonFun'
import { ref, onMounted, createVNode, computed, watchEffect, defineOptions } from 'vue'
import { debounce } from 'lodash-es'
import { formatDateTime } from '@/utils/dateUtils'
/* 页面缓存 */
defineOptions({
  name: 'A0303'
})
/* 引入sheet.js-xlsx 插件实现前端导出 */
import * as XLSX from 'xlsx'
// import * as XLSX_STYLE from 'xlsx-style-vite'

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

const inputFunctionName = ref('A0303')
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
  if (functionName === 'exportData') {
    exportData()
  }
}

const dataColumns = [
  {
    title: '序号',
    width: 80,
    dataIndex: 'index',
    key: 'nonEditable',
    customRender: (obj) => {
      return (currentPage.value - 1) * currentPageSize.value + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: 'IP地址',
    dataIndex: 'ip'
  },
  {
    title: '系统名称',
    dataIndex: 'menuName'
  },
  {
    title: '访问时间',
    dataIndex: 'createTime'
  },
  {
    title: '访问账号',
    dataIndex: 'loginName'
  },
  {
    title: '访问姓名',
    dataIndex: 'realName'
  }
]
const data = ref([])
const ipKey = ref('')
const menuName = ref('')
const beginTime = ref(null)
const endTime = ref(null)
const onChangeBegin = (value, dateString) => {
  beginTime.value = dateString
}
const onChangeEnd = (value, dateString) => {
  endTime.value = dateString
}

var currentPageSize = ref(10)
var currentPage = ref(1)
var totalCount = ref(0)

var selectedRowIds = [] //选中的Id集合
const rowSelection = {
  onChange: (selectedRowKeys, selectedRows) => {
    console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows)
  },
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

/* 下拉选择器 */
const sysNameValue = ref(null)
const sysNameOptions = ref([])

const inputPersonnelValue = ref(null)
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
  inputPersonnelValue.value = value
}

getTableData(true)
getSysList()
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

async function getSysList() {
  await getSysinfoList().then((res) => {
    if (res.code == 200 && res.success) {
      sysNameOptions.value = []
      res.data.forEach((element) => {
        sysNameOptions.value.push({ value: element.sysInfoId, label: element.subSysName })
      })
    }
  })
}

function resetSearch() {
  sysNameValue.value = null
  ipKey.value = ''
  inputPersonnelValue.value = null
  beginTime.value = null
  endTime.value = null
}

function pageSizeChange(page, pageSize) {
  currentPage.value = page
  currentPageSize.value = pageSize
  getTableData(true)
}

async function getTableData(isSerch) {
  var obj = {
    sysid: sysNameValue.value,
    ip: ipKey.value.trim(),
    userid: inputPersonnelValue.value,
    startTime: beginTime.value,
    endTime: endTime.value,
    pageIndex: currentPage.value,
    pageSize: currentPageSize.value
  }
  await getSysInfoRecord(obj).then((res) => {
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
  await deleteAllApi(true).then((res) => {
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
/* 导出数据 */
function exportData() {
  const tableData = tranceData(dataColumns.value, data.value)
  //将数据数组转化为工作表
  const ws = XLSX.utils.aoa_to_sheet(tableData)
  //创建workbook
  const wb = XLSX.utils.book_new()
  ws['!ref'] = `A1:AI${tableData.length}`
  //设置列宽
  ws['!cols'] = [{ wpx: 120 }, { wpx: 100 }, { wpx: 110 }, { wpx: 110 }]
  //合并单元格
  // ws['!merges'] = [{ s: { r: 0, c: 0 }, e: { r: 0, c: 1 } }]
  // 将 工作表 添加到 workbook
  XLSX.utils.book_append_sheet(wb, ws, 'Sheet1')
  // 将 workbook 写入文件
  XLSX.writeFile(wb, 'tablename.xlsx')
}

function tranceData(columns, tableList) {
  const obj = columns.reduce((acc, cur) => {
    if (!acc.titles && !acc.keys) {
      acc.titles = []
      acc.keys = []
    }
    acc.titles.push(cur.title)
    acc.keys.push(cur.dataIndex)
    return acc
  }, {})
  const tableBody = tableList.map((item, i) => {
    item.index = i + 1
    return obj.keys.map((key, index) => item[key])
  })
  return [obj.titles, ...tableBody]
}
</script>

<style lang="scss">
.SystemAccessLog {
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
