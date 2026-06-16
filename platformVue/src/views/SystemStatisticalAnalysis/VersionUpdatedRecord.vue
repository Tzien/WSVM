<template>
  <div class="VersionUpdatedRecord">
    <div style="line-height: 34px; margin: 0px 0px 0px 20px; padding-top: 5px">
      <span>
        系统名称：
        <a-select v-model:value="sysNameValue" show-search allow-clear placeholder="请选择系统" style="width: 200px" :options="sysNameOptions"></a-select>
      </span>
      <span style="margin-left: 20px">
        更新内容：
        <a-input v-model:value="contentKey" placeholder="请输入更新内容" style="width: 250px" />
      </span>
      <span style="margin-left: 20px">
        录入人员：
        <a-select
          v-model:value="inputPersonnelValue"
          show-search
          allowClear
          placeholder="选择录入人"
          style="width: 200px"
          :options="filteredOptions"
          :filter-option="false"
          @search="handleSearch"
          @change="handleChange"
        ></a-select>
      </span>
      <span style="margin-left: 20px">
        更新时间：
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
    </div>
    <div style="line-height: 34px; padding-top: 5px; margin-bottom: 5px">
      <CustomButtonList
        :ParamsRoleId="inputRoleId"
        :ParamsFunctionName="inputFunctionName"
        :BtnFunctionNameArray="['A030402', 'A030403', 'A030404']"
        @buttonClick="handleButtonClick"
      ></CustomButtonList>
      <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData(true)">查询数据</a-button>
      <a-button style="margin-left: 10px" :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      <a-button style="margin-left: 10px" :icon="h(PlusOutlined)" type="primary" @click="addRecord">新增记录</a-button> -->
    </div>
    <a-table
      :columns="dataColumns"
      :data-source="data"
      :pagination="false"
      :scroll="{
        y: 'calc(100vh - 64px - 36px - 34px - 34px - 5px - 10px - 55px - 35px - 40px - 10px)'
      }"
    >
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'createTime' || column.dataIndex === 'modifyTime'">
          {{ formatDateTime(text) }}
        </template>
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="detail(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A030401')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A030401')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A030405')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A030405')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
        </template>
        <template v-if="column.dataIndex === 'sysId'">
          <span>{{ getSysName(record.sysId) }}</span>
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
    <!-- 弹窗 -->
    <div class="modalStyle">
      <a-modal v-model:open="openRecordModal" :title="modalState.modalTitle" :keyboard="false" :maskClosable="false" width="1000px" :footer="null">
        <RichText ref="RichRef" :modalState="modalState" @childEvent="emitSuccess"></RichText>
      </a-modal>
    </div>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ExclamationCircleOutlined } from '@ant-design/icons-vue'
// import { SearchOutlined, RedoOutlined, CloseOutlined, PlusOutlined, EditOutlined } from '@ant-design/icons-vue'
import { message, Modal } from 'ant-design-vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { ref, h, onMounted, createVNode, reactive, computed, watchEffect, nextTick, defineOptions } from 'vue'
import { getSysinfoList, getUserInfo } from '@/api/commonFun'
import { getVersionInfo, deleteApi } from '@/api/versionLog'
import RichText from './components/RichText.vue'
import { debounce } from 'lodash-es'
import { formatDateTime } from '@/utils/dateUtils'
/* 页面缓存 */
defineOptions({
  name: 'A0304'
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
const inputFunctionName = ref('A0304')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0304',
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
  if (functionName === 'getTableData(true)') {
    getTableData(true)
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'addRecord') {
    addRecord()
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
    title: 'Jenkins构建容器',
    dataIndex: 'jenkinsUrl',
    align: 'center'
  },
  {
    title: '系统名称',
    dataIndex: 'sysId'
  },
  {
    title: '版本号',
    dataIndex: 'version'
  },
  {
    title: '更新标题',
    dataIndex: 'title'
  },
  {
    title: '录入时间',
    dataIndex: 'createTime'
  },
  {
    title: '录入人员',
    dataIndex: 'createName'
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime'
  },
  {
    title: '修改人员',
    dataIndex: 'modifyName'
  },
  {
    title: '操作',
    width: 160,
    dataIndex: 'operation',
    fixed: 'right',
    align: 'center'
  }
]
const data = ref([])

const RichRef = ref(null)
var currentPageSize = ref(10)
var currentPage = ref(1)
var totalCount = ref(0)
const contentKey = ref('')

const beginTime = ref(null)
const endTime = ref(null)
const onChangeBegin = (value, dateString) => {
  beginTime.value = dateString
}
const onChangeEnd = (value, dateString) => {
  endTime.value = dateString
}

/* 下拉选择器 */
const sysNameValue = ref(null)
const sysNameOptions = ref([])
var modalState = reactive({
  isAdd: true,
  modalTitle: '新增版本更新记录',
  id: ''
})

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

/* modal弹出框 */
const openRecordModal = ref(false)
const addRecord = () => {
  openRecordModal.value = true
  modalState.modalTitle = '新增版本更新记录'
  modalState.isAdd = true
  modalState.id = ''
}

getTableData(true)
getSysList()
getUserList()
onMounted(() => {
  getAllButton()
})

async function getSysList() {
  await getSysinfoList().then((res) => {
    if (res.code == 200 && res.success) {
      sysNameOptions.value = []
      res.data.forEach((element) => {
        sysNameOptions.value.push({
          value: element.sysInfoId,
          label: element.subSysName
        })
      })
    }
  })
}
async function getUserList() {
  await getUserInfo().then((res) => {
    if (res.code == 200 && res.success) {
      sysUserOptions.value = []
      res.data.forEach((element) => {
        sysUserOptions.value.push({
          value: element.id,
          label: element.realName
        })
      })
    }
  })
}

function resetSearch() {
  contentKey.value = ''
  sysNameValue.value = null
  beginTime.value = null
  endTime.value = null
  inputPersonnelValue.value = null
}

function pageSizeChange(page, pageSize) {
  currentPage.value = page
  currentPageSize.value = pageSize
  getTableData(true)
}

async function getTableData(isSerch) {
  var obj = {
    sysid: sysNameValue.value,
    content: contentKey.value.trim(),
    startTime: beginTime.value,
    endTime: endTime.value,
    pageIndex: currentPage.value,
    pageSize: currentPageSize.value
  }
  await getVersionInfo(obj).then((res) => {
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

function emitSuccess(value) {
  switch (value) {
    case 'success':
      openRecordModal.value = false
      getTableData(false)
      break
    case 'cancel':
      openRecordModal.value = false
      break
  }
}

function getSysName(sysId) {
  if (sysId == null) {
    return ''
  }
  const resultObj = sysNameOptions.value.find((item) => item.value === sysId)
  if (resultObj) {
    return resultObj.label
  } else {
    return ''
  }
}

//编辑
async function detail(id) {
  modalState.modalTitle = '编辑版本更新记录'
  modalState.isAdd = false
  modalState.id = id
  openRecordModal.value = true
  await nextTick()
  RichRef.value?.getDataById(id)
}

/* 删除 */
function onDelete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该更新记录吗？',
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
async function del(resolve, reject, id) {
  await deleteApi([id]).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData(false)
    }
  })
}
</script>

<style lang="scss">
.VersionUpdatedRecord {
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
