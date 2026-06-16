<template>
  <div class="TaskLog">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>任务执行日志</p>
      </div>
      <div class="headerRight">
        <a-p>任务名称：</a-p>
        <a-input style="width:150px" v-model:value="name" placeholder="请输入任务名称" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A050301', 'A050303', 'A050302']"
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
      :scroll="{ y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)'}"
          :expand-column-width="100"
          :row-selection="rowSelection"
        >
          <template #bodyCell="{ text, column, record }">
          
            <!-- 处理状态列 -->
            <template v-if="column.dataIndex === 'isSuccess'">
              <span v-if="text === true"> 成功 </span>
              <span v-else> 失败 </span>
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
  
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ref, onMounted, h, reactive, createVNode, watchEffect } from 'vue'
import { message, Modal } from 'ant-design-vue'
import {getAllTaskLogApi, deleteTaskLogsApi } from '@/api/Task/task'
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
  name: 'A0503'
})
const inputFunctionName = ref('A0503')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'search') {
    search()
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'deleteLogs') {
    deleteLogs()
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
})
const myTable = ref(null)

//获取列表数据
const name = ref()
async function getAll(page, size, name) {
  const query = {
    page: page,
    size: size,
    name: name
  }
  const response = await getAllTaskLogApi(query)
  if (response.code === 200 && response.success) {
    response.data.forEach((e) => {
      e.key = e.taskLogId
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
    title: '耗时',
    dataIndex: 'useTime',
    key: 'useTime',
    align: 'center'
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
    title: '是否成功',
    dataIndex: 'isSuccess',
    key: 'isSuccess',
    align: 'center'
  },
  // {
  //   title: '返回内容',
  //   dataIndex: 'result',
  //   key: 'result',
  //   align: 'center'
  // },
  {
    title: '异常信息',
    dataIndex: 'ex',
    key: 'ex',
    align: 'center'
  }
]
const data = ref(null)

//删除
async function del(resolve, reject) {
  const params = {
    ids: rowSelection.value.selectedRowKeys.join(',')
  }
  await deleteTaskLogsApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getAll(current.value, pageSize.value, name.value)
    } else if (res.code === 200 && !res.success) {
      message.error(res.message)
    }
  })
}
function deleteLogs() {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要删除的数据！')
    return
  }
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = del(resolve, reject)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

</script>


<style lang="scss">
.TaskLog {
  height: 100%;
  position: relative;

  .ant-table-container {
    width: 100%;
  }
  .ant-table-body {
    overflow-y: scroll !important;
    height: calc(100vh - 96px - 40px  - 40px - 35px);
  }
  .ant-table-content {
    
    height: calc(100vh - 96px - 40px  - 40px - 35px);
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
