<!-- 角色用户配置 -->
<template>
  <div class="roleUserConfig">
    <a-flex :gap="0" style="height: 100%">
      <div class="treeStyle">
        <div style="height: 30px; line-height: 30px; padding-left: 10px; font-size: 14px; font-weight: bold">选择角色</div>
        <a-input-search v-model:value="searchTreeKey" placeholder="请输入角色名称模糊查询" @search="onSearchTreeKey" />
        <a-table
          size="small"
          :expandIconColumnIndex="3"
          :indentSize="10"
          :columns="treeColumns"
          :data-source="treeData"
          :row-selection="rowSelection"
          :pagination="false"
          :scroll="{
            y: 'calc(100vh - 96px - 10px - 30px - 32px - 39px - 35px - 40px)'
          }"
          :custom-row="customRow"
        >
          <!-- 全屏幕 减去 系统菜单栏  Tab栏 内边距上下 选择角色文本 搜索框 table表头 底部footer 合计-->
          <template #expandIcon="props">
            <span v-if="props.record.children">
              <div
                v-if="props.expanded"
                style="display: inline-block; margin-right: 10px"
                @click="
                  (e) => {
                    props.onExpand(props.record, e)
                  }
                "
              >
                <CaretDownOutlined />
              </div>
              <div
                v-else
                style="display: inline-block; margin-right: 10px"
                @click="
                  (e) => {
                    props.onExpand(props.record, e)
                  }
                "
              >
                <CaretRightOutlined />
              </div>
            </span>
            <span v-else style="margin-right: 15px"></span>
          </template>
        </a-table>
        <div class="treeTableSummary">全部显示，共{{ treeData.length }}条</div>
      </div>
      <div class="tableStyle">
        <div style="height: 30px; line-height: 30px; padding-left: 10px; font-size: 14px; font-weight: bold">用户信息</div>
        <div style="margin-left: 20px; line-height: 34px; margin-bottom: 5px">
          <span>
            用户账号：
            <a-input-search v-model:value="loginNameKey" allowClear placeholder="请输入用户账号" style="width: 200px" @search="onSearchLoginName" />
          </span>
          <span style="margin-left: 10px; margin-right: 10px">
            用户名称：
            <a-input-search v-model:value="userNameKey" allowClear placeholder="请输入用户名称" style="width: 200px" @search="onSearchUserName" />
          </span>
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010206', 'A010207']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
          <!-- <a-button style="margin-left: 10px" type="primary" :icon="h(PlusOutlined)" @click="openModalByAdd">添加用户到当前角色</a-button>
          <a-button style="margin-left: 10px" type="primary" danger :icon="h(CloseOutlined)" @click="delUserToRole">从当前角色移除选中用户</a-button> -->
        </div>
        <a-table
          :style="{
            width: `calc(100vw - ${drawerStore.menuwidth}px - 310px - 10px)`
          }"
          :columns="dataColumns"
          :data-source="data"
          :pagination="false"
          :scroll="{
            x: `calc(100vw - ${drawerStore.menuwidth}px - 310px - 10px)`,
            y: 'calc(100vh - 96px - 30px - 39px - 55px - 35px - 35px)'
          }"
          :row-selection="tableRowSelection"
        >
          <template #bodyCell="{ text, column, record }">
            <template v-if="column.dataIndex === 'lastLoginTime'">
              {{ formatDateTime(text) }}
            </template>
          </template>
        </a-table>
        <div class="paginationStyle">
          <a-pagination
            v-model:current="currentPage"
            v-model:page-size="currentPageSize"
            :defaultPageSize="currentPageSize"
            showQuickJumper
            :pageSizeOptions="['15', '30', '45', '60']"
            showSizeChanger
            :total="totalCount"
            align="right"
            :show-total="(total) => `${$t('message.drawer.TotalOf')} ${totalCount} ${$t('message.drawer.Items')}`"
            @change="pageSizeChange"
          />
        </div>
      </div>
    </a-flex>

    <div class="modalStyle1">
      <a-drawer
        title="添加用户到当前角色"
        :width="1080"
        :open="openModal"
        :body-style="{ paddingBottom: '80px' }"
        :footer-style="{ textAlign: 'right' }"
        @close="cancelModal"
      >
        <div style="line-height: 34px; margin: 0px 0px 0px 5px; padding-top: 5px">
          <span>
            用户所在部门：
            <a-tree-select
              v-model:value="modalSysOrgNameSelect"
              show-search
              style="width: 200px"
              :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
              placeholder="请选择部门"
              allow-clear
              tree-default-expand-all
              :tree-data="sysOrgNameOptions"
              tree-node-filter-prop="label"
            >
            </a-tree-select>
          </span>
          <span style="margin-left: 20px">
            用户账号：
            <a-input v-model:value="modalLoginName" placeholder="请输入用户账号" style="width: 250px" />
          </span>
          <span style="margin-left: 20px">
            用户名称：
            <a-input v-model:value="modalUserName" placeholder="请输入用户名称" style="width: 250px" />
          </span>
        </div>
        <div style="margin: 5px 0px">
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010201', 'A010202', 'A010203', 'A010204', 'A010205']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
          <!-- <a-button :icon="h(CloseOutlined)" @click="cancelModal">关闭</a-button>
          <a-button style="margin-left: 10px" :icon="h(CheckOutlined)" @click="addUserToRole(false)">选择</a-button>
          <a-button style="margin-left: 10px" :icon="h(CheckOutlined)" @click="addUserToRole(true)">选择后关闭</a-button>
          <a-button style="margin-left: 10px" :icon="h(SearchOutlined)" @click="searchModalData">查询数据</a-button>
          <a-button style="margin-left: 10px" :icon="h(RedoOutlined)" @click="modalResetSearch">重置查询</a-button> -->
        </div>
        <a-table size="small" :columns="modalColumns" :row-selection="modalRowSelection" :data-source="modalData" :pagination="false" :scroll="{ x: 1500 }">
          <template #bodyCell="{ text, column, record }">
            <template v-if="column.dataIndex === 'lastLoginTime'">
              {{ formatDateTime(text) }}
            </template>
          </template>
        </a-table>
        <div class="paginationStyle">
          <a-pagination
            v-model:current="modalCurrentPage"
            v-model:page-size="modalCurrentPageSize"
            :total="modalTotalCount"
            align="right"
            :show-total="(total) =>  `${$t('message.drawer.TotalOf')} ${modalTotalCount} ${$t('message.drawer.Items')}`"
            @change="modalPageSizeChange"
          />
        </div>
      </a-drawer>
    </div>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
// import { CheckOutlined, SearchOutlined, RedoOutlined, PlusOutlined, CloseOutlined } from '@ant-design/icons-vue'
import { CaretRightOutlined, CaretDownOutlined } from '@ant-design/icons-vue'
import { ref, computed, onMounted, watchEffect, defineOptions } from 'vue'
import { getRoleInfo, getSysOrgInfo } from '@/api/commonFun'
import { getModalTableData, addRoleUser, getUserListByRoleId, delUserToRoleByIds } from '@/api/BaseInfoConfig/roleUser'
import { message } from 'ant-design-vue'
import { formatDateTime } from '@/utils/dateUtils'
import { useDrawerStore, useUserStore } from '@/store/index'
import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
/* 页面缓存 */
defineOptions({
  name: 'A0102'
})

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
/* 动态按钮相关 */
const inputFunctionName = ref('A0102')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'openModalByAdd') {
    openModalByAdd()
  }
  if (functionName === 'delUserToRole') {
    delUserToRole()
  }
  if (functionName === 'addUserToRole(false)') {
    addUserToRole(false)
  }
  if (functionName === 'cancelModal') {
    cancelModal()
  }
  if (functionName === 'modalResetSearch') {
    modalResetSearch()
  }
  if (functionName === 'searchModalData') {
    searchModalData()
  }
  if (functionName === 'addUserToRole(true)') {
    addUserToRole(true)
  }
}

const treeColumns = [
  {
    title: '',
    width: 1,
    key: 'nonEditable',
    dataIndex: 'index',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '角色名称',
    dataIndex: 'name',
    key: 'name'
  }
]
const treeData = ref([])

const dataColumns = [
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    dataIndex: 'index',
    customRender: (obj) => {
      return (modalCurrentPage.value - 1) * modalCurrentPageSize.value + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: '用户ID',
    dataIndex: 'id',
    width: '300px'
  },
  {
    title: '用户名称',
    dataIndex: 'realName',
    width: '150px'
  },
  {
    title: '用户性别',
    dataIndex: 'sex',
    width: '100px'
  },
  {
    title: '用户账号',
    dataIndex: 'loginName',
    width: '200px'
  },
  {
    title: '用户状态',
    dataIndex: 'enabled',
    width: '100px'
  },
  {
    title: '直属部门',
    dataIndex: 'oraganizeName',
    width: '250px'
  },
  {
    title: '所在角色组',
    dataIndex: 'roles',
    width: '300px'
  },
  {
    title: '用户邮箱',
    dataIndex: 'email',
    width: '150px'
  },
  {
    title: '用户手机',
    dataIndex: 'phone',
    width: '150px'
  },
  {
    title: '用户微信',
    dataIndex: 'webChat',
    width: '150px'
  },
  {
    title: '用户钉钉',
    dataIndex: 'dingTalk',
    width: '150px'
  },
  // {
  //   title: '最后登录时间',
  //   dataIndex: 'lastLoginTime',
  //   width: '250px'
  // },
  {
    title: '用户备注',
    dataIndex: 'remark',
    width: '400px'
  }
]
const data = ref([])
const modalColumns = [
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    dataIndex: 'index',
    customRender: (obj) => {
      return (modalCurrentPage.value - 1) * modalCurrentPageSize.value + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  // {
  //   title: '用户ID',
  //   dataIndex: 'id',
  //   width: '300px'
  // },
  {
    title: '用户名称',
    dataIndex: 'realName',
    width: '150px'
  },
  {
    title: '用户性别',
    dataIndex: 'sex',
    width: '100px'
  },
  {
    title: '用户账号',
    dataIndex: 'loginName',
    width: '200px'
  },
  {
    title: '用户状态',
    dataIndex: 'enabled',
    width: '100px'
  },
  {
    title: '直属部门',
    dataIndex: 'oraganizeName',
    width: '250px'
  },
  {
    title: '所在角色组',
    dataIndex: 'roles',
    width: '300px'
  },
  {
    title: '用户邮箱',
    dataIndex: 'email',
    width: '150px'
  },
  {
    title: '用户手机',
    dataIndex: 'phone',
    width: '150px'
  },
  {
    title: '用户微信',
    dataIndex: 'webChat',
    width: '150px'
  },
  {
    title: '用户钉钉',
    dataIndex: 'dingTalk',
    width: '150px'
  },
  {
    title: '最后登录时间',
    dataIndex: 'lastLoginTime',
    width: '250px'
  },
  {
    title: '用户备注',
    dataIndex: 'remark',
    width: '400px'
  }
]
const modalData = ref([])
var currentPage = ref(1)
var currentPageSize = ref(15)
var totalCount = ref(0)
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(10)
var modalTotalCount = ref(0)
const searchTreeKey = ref('')
const onSearchTreeKey = () => {
  getRoleList()
}
/* Modal下拉选择器 */
const modalSysOrgNameSelect = ref(null)
const sysOrgNameOptions = ref([])
const modalLoginName = ref('')
const modalUserName = ref('')
function modalResetSearch() {
  modalSysOrgNameSelect.value = null
  modalLoginName.value = ''
  modalUserName.value = ''
}
function searchModalData() {
  serarchModalData()
}

const userNameKey = ref('')
const loginNameKey = ref('')
const onSearchUserName = () => {
  if (!selectedRowKey.value) {
    message.warning('请选择角色后再试')
    return
  }
  getListByRoleId()
}
const onSearchLoginName = () => {
  if (!selectedRowKey.value) {
    message.warning('请选择角色后再试')
    return
  }
  getListByRoleId()
}

function pageSizeChange(page, pageSize) {
  currentPage.value = page
  currentPageSize.value = pageSize
  getListByRoleId()
}
function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  serarchModalData()
}

const selectedRowKey = ref(null) //角色树选中Key
const customRow = (record) => {
  return {
    onClick: () => {
       if (record.isReadOnly) {
        return;
      }
      currentPage.value = 1
      currentPageSize.value = 15
      selectedRowKey.value = record.key
      //加载tableData
      getListByRoleId()
    }
  }
}
const rowSelection = computed(() => {
  return {
    selectedRowKeys: [selectedRowKey.value],
    onSelect: (record, selected, selectedRows) => {
      
      selectedRowKey.value = null
      if (selected) {
        selectedRowKey.value = record.key
        getListByRoleId()
      }
    },
    getCheckboxProps: record =>{
      return {
        disabled: record.isReadOnly
      }
    },
    columnTitle: ' '
  }
})

var tableRowSelectKeys = ref([])
const tableRowSelection = {
  selectedRowKeys: tableRowSelectKeys,
  onChange: (selectedRowKeys, selectedRows) => {
    tableRowSelectKeys.value = []
    tableRowSelectKeys.value = selectedRowKeys
  },
  onSelect: (tableRecord, tableSelected, tableSelectedRows) => {
    if (tableSelectedRows.length > 0) {
      tableRowSelectKeys.value = []
      tableSelectedRows.forEach((element) => {
        if (element) {
          tableRowSelectKeys.value.push(element.key)
        }
      })
    }
  },
  onSelectAll: (tableSelected, tableSelectedRows, tableChangeRows) => {
    tableRowSelectKeys.value = []
    if (tableSelected) {
      tableChangeRows.forEach((element) => {
        if (element) {
          tableRowSelectKeys.value.push(element.key)
        }
      })
    }
  }
}

/* 弹窗相关 */
const openModal = ref(false)

var modalSelectedRowIds = ref([]) //选中的idsUserId集合
const modalRowSelection = {
  selectedRowKeys: modalSelectedRowIds,
  onChange: (selectedRowKeys, selectedRows) => {
    modalSelectedRowIds.value = []
    modalSelectedRowIds.value = selectedRowKeys
  },
  onSelect: (modalRecord, modalSelected, modalSelectedRows) => {
    if (modalSelectedRows.length > 0) {
      modalSelectedRowIds.value = []
      modalSelectedRows.forEach((element) => {
        if (element) {
          modalSelectedRowIds.value.push(element.key)
        }
      })
    }
  },
  onSelectAll: (modalSelected, modalChangeRows) => {
    modalSelectedRowIds.value = []
    if (modalSelected) {
      modalChangeRows.forEach((element) => {
        modalSelectedRowIds.value.push(element.key)
      })
    }
  }
}
getRoleList()
getSysOrgList()
onMounted(() => {})

async function getSysOrgList() {
  await getSysOrgInfo().then((res) => {
    if (res.code == 200 && res.success) {
      sysOrgNameOptions.value = simplifyArrayToOrg(res.data)
    }
  })
}
function simplifyArrayToOrg(datas) {
  const sortedDatas = datas.sort((a, b) => a.sort - b.sort)
  return sortedDatas.map((item) => {
    //创建新对象
    const newItem = {
      key: item.id,
      label: item.name,
      id: item.id,
      value: item.id
    }
    //如果存在children,则递归处理
    if (item.oraganizeTrees && item.oraganizeTrees.length > 0) {
      newItem.children = simplifyArrayToOrg(item.oraganizeTrees)
    }
    return newItem
  })
}

async function getListByRoleId() {
  const dataObj = {
    roleid: selectedRowKey.value,
    loginName: loginNameKey.value,
    realName: userNameKey.value,
    type: 1,
    pageIndex: currentPage.value,
    pageSize: currentPageSize.value
  }
  await getUserListByRoleId(dataObj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = item.idsUserId
      })
      data.value = res.data
      totalCount.value = res.total
    }
  })
}

async function getRoleList() {
  await getRoleInfo(searchTreeKey.value).then((res) => {
    if (res.code == 200 && res.success) {
      treeData.value = simplifyArray(res.data)
    }
  })
}
function simplifyArray(datas) {
  return datas.map((item) => {
    //创建新对象
    const newItem = {
      id: item.id,
      key: item.id,
      name: item.name,
      isReadOnly: item.isReadOnly
    }
    //如果存在children,则递归处理
    if (item.children && item.children.length > 0) {
      newItem.children = simplifyArray(item.children)
    }
    return newItem
  })
}

async function openModalByAdd() {
  if (!selectedRowKey.value) {
    message.warning('请选择角色后再试')
    return
  }
  serarchModalData()
}
//查询modal数据
async function serarchModalData() {
  //获取数据
  var obj = {
    roleid: selectedRowKey.value,
    type: 2,
    oraganizeId: modalSysOrgNameSelect.value,
    loginName: modalLoginName.value,
    realName: modalUserName.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getModalTableData(obj).then((res) => {
    if (res.code == 200 && res.success) {
      openModal.value = true
      res.data.forEach((item) => {
        item.key = item.idsUserId
      })
      modalData.value = res.data
      modalTotalCount.value = res.total
    }
  })
}

async function delUserToRole() {
  if (!selectedRowKey.value) {
    message.warning('请选择角色后再试')
    return
  }
  if (!tableRowSelectKeys.value.length > 0) {
    message.warning('请选择用户后再试')
    return
  }
  await delUserToRoleByIds(selectedRowKey.value, tableRowSelectKeys.value).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户移除完成!')
      tableRowSelectKeys.value = []
      getListByRoleId()
    }
  })
  /* 操作日志 */
  const btnMsg = {
    buttonOperationId: '',
    belongPage: '从当前角色删除用户',
    buttonName: '删除用户',
    operationDescription: JSON.stringify(selectedRowKey.value, tableRowSelectKeys.value),
    operationType: '删除',
    operationPersonId: userStore.value.userid
  }
  await useInsertButtonOperationLogAsync(btnMsg)
}

async function addUserToRole(isClose) {
  await addRoleUser(selectedRowKey.value, modalSelectedRowIds.value).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户添加完成!')
      modalSelectedRowIds.value = []
      if (isClose) {
        openModal.value = false
        getListByRoleId()
      }
    }
  })
  /* 操作日志 */
  const btnMsg = {
    buttonOperationId: '',
    belongPage: '从当前角色添加用户',
    buttonName: '添加用户',
    operationDescription: JSON.stringify(selectedRowKey.value, modalSelectedRowIds.value),
    operationType: '添加',
    operationPersonId: userStore.value.userid
  }
  await useInsertButtonOperationLogAsync(btnMsg)
}
function cancelModal() {
  modalSelectedRowIds.value = []
  openModal.value = false
  getListByRoleId()
}
</script>

<style lang="scss">
.roleUserConfig {
  height: 100%;
  position: relative;

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
  .treeStyle {
    border-right: 1px solid rgb(238, 238, 238);
    padding: 5px 5px 0px 5px;
    background-color: #fff;
    position: relative;

    .ant-table-container {
      width: 300px;
    }

    .ant-table-body {
      overflow-y: auto !important;
    }

    .treeTableSummary {
      position: absolute;
      height: 40px;
      line-height: 40px;
      text-align: right;
      width: 300px;
      padding-right: 10px;
      border-top: 1px solid rgb(238, 238, 238);
      right: 0px;
      bottom: 0px;
    }
  }

  .tableStyle {
    height: 100%;
    padding: 0px 5px 0px 2px;
    .ant-table-body {
      overflow-y: auto !important;
      height: calc(100vh - 96px - 30px - 39px - 55px - 35px - 40px);
    }
    .ant-input-affix-wrapper {
      display: inline-flex;
      align-items: center;
      box-sizing: border-box;
      height: 32px;
    }
    .ant-input-affix-wrapper .ant-input {
      box-sizing: border-box;
    }
  }
}
</style>
