<template>
  <div class="SystemSettingDataBaseConfig">
    <div style="display: flex; align-items: center; justify-content: space-between; height: 36px; line-height: 36px; margin-top: 17px">
      <a-typography-paragraph style="overflow: auto; padding-left: 20px">
        <blockquote style="color: black; border-inline-start: 5px solid rgb(37, 97, 239) !important">
          <span style="font-weight: bold; color: #111111; font-size: 16px">{{ PageTitle }}</span>
        </blockquote>
      </a-typography-paragraph>
      <div style="padding-right: 20px">
        <CustomIconButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['LowCodeDatabaseConfigFilter']"
          :isChecked="isExpand"
          @buttonClick="handleButtonClick"
        ></CustomIconButtonList>
        <!-- <a-button style="margin: 0 5px" shape="" :type="''" :icon="h(DownloadOutlined)" />
        <a-button style="margin: 0 5px" shape="circle" :type="''" :icon="h(UploadOutlined)" />
        <a-button style="margin-left: 10px" shape="circle" :icon="h(FilterOutlined)" :type="isExpand ? 'primary' : ''" @click="toggleAccordion()" /> -->
        <!-- <a-button @click="getTableData()" style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" /> -->

        <a-button @click="getTableData()" style="margin: 0px 20px 0px 10px" shape="circle" :icon="h(RedoOutlined)"> </a-button>

        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['LowCodeDatabaseConfigAdd']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>
        <!-- <a-button style="margin-left: 20px; background-color: #2461a6" @click="openUserDrawer()" type="primary">新建</a-button> -->
      </div>
    </div>

    <div>
      <a-divider />
    </div>
    <div class="accordion" v-show="isExpand">
      <div ref="accordionRef" class="accordion-content">
        <span style="margin-left: 20px">
          关键词：
          <a-input v-model:value="nameKey" placeholder="请输入关键词" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <span style="margin-left: 20px">
          连接驱动：
          <a-select
            v-model:value="dbTypeKey"
            allowClear
            placeholder="请选择连接驱动"
            style="width: 250px; margin-right: 5px"
            :options="DbTypeOptions"
          ></a-select>
        </span>
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['LowCodeDatabaseConfigSearch', 'LowCodeDatabaseConfigReset']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>

        <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData()">查询数据</a-button>
        <a-button style="margin-left: 20px" :icon="h(PlusOutlined)" @click="openUserDrawer()" type="primary">添加用户</a-button> -->
      </div>
    </div>

    <s-table
      :columns="columns"
      :height="calcHeight"
      headerAlign="center"
      align="center"
      size="small"
      stripe
      :pagination="false"
      rowKey="dbConfigId"
      :row-selection="rowSelection"
      :data-source="dataSource"
    >
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'modifyTime' && text">
          {{ formatDateTime(text) }}
        </template>
        <template v-if="column.dataIndex === 'createTime'">
          {{ formatDateTime(text) }}
        </template>

        <template v-if="column.key === 'operation'">
          <a @click="Edit(record)" style="color: #2461a6; margin-right: 10px" v-if="allDyBtn.find((item) => item.functionCode === 'LowCodeDatabaseConfigEdit')">编辑</a>
          <a @click="Delete(record.id)"  style="color: rgb(255, 77, 79)" v-if="allDyBtn.find((item) => item.functionCode === 'LowCodeDatabaseConfigDelete')">删除</a>

          <!-- <a style="color: #2461a6; margin-right: 10px" @click="Edit(record)">编辑</a>
          <a style="color: #2461a6" @click="Delete(record.dbConfigId)">删除</a> -->
        </template>
      </template>
    </s-table>
    <div class="paginationStyle" style="margin-top: 10px">
      <a-pagination
        v-model:current="modalCurrentPage"
        v-model:page-size="modalCurrentPageSize"
        :total="modalTotalCount"
        align="right"
        :pageSizeOptions="['20', '30', '45', '60']"
        showSizeChanger
        :show-total="(total) => `${$t('message.drawer.TotalOf')} ${total} ${$t('message.drawer.Items')}`"
        @change="modalPageSizeChange"
      />
    </div>
    <a-drawer width="35%" v-model:open="openDrawer" :body-style="{ padding: '20px 50px' }" :title="drawerTitle" placement="right">
      <a-form ref="ruleForm" :model="formState" :label-col="labelCol">
        <a-form-item label="连接驱动" name="dbtype" :rules="[{ required: true, message: '请选择连接驱动!' }]">
          <a-select v-model:value="formState.dbtype" allowClear placeholder="请选择" :options="DbTypeOptions"></a-select>
        </a-form-item>
        <a-form-item label="连接名称" name="name" :rules="[{ required: true, message: '请输入连接名称!' }]">
          <a-input v-model:value="formState.name" />
        </a-form-item>
        <a-form-item label="主机地址" name="addr" :rules="[{ required: true, message: '请输入主机地址!' }]">
          <a-input v-model:value="formState.addr" />
        </a-form-item>
        <a-form-item label="端口" name="port" :rules="[{ required: true, message: '请输入端口!' }]">
          <a-input v-model:value="formState.port" />
        </a-form-item>
        <a-form-item label="用户" name="uname" :rules="[{ required: true, message: '请输入用户!' }]">
          <a-input v-model:value="formState.uname" />
        </a-form-item>
        <a-form-item label="密码" name="pwd" :rules="[{ required: true, message: '请输入密码!' }]">
          <a-input-password v-model:value="formState.pwd" />
        </a-form-item>
        <a-form-item
          v-if="formState.dbtype == 'MySql' || formState.dbtype == 'SqlServer' || formState.dbtype == 'PostgreSQL' || formState.dbtype == 'dm'"
          label="库名"
          name="dbname"
          :rules="[{ required: true, message: '请输入库名!' }]"
        >
          <!-- <a-input-search v-if="formState.dbtype == 0" v-model:value="formState.dbname" placeholder="请输入库名" @search="onSearch">
            <template #enterButton>
              <a-button type="primary" :loading="true">连接测试</a-button>
            </template>
          </a-input-search> -->
          <a-input v-model:value="formState.dbname" placeholder="请输入库名"> </a-input>
        </a-form-item>
        <!-- <a-form-item label="模式" name="pattern" v-if="formState.dbtype != 0">
          <a-input-search disabled v-model:value="formState.pattern" @search="onSearch">
            <template #enterButton>
              <a-button type="primary" :loading="true">连接测试</a-button>
            </template>
          </a-input-search>
        </a-form-item> -->

        <a-form-item label="更多" name="ismore" v-if="formState.dbtype == 'Oracle'">
          <a-switch v-model:checked="formState.ismore" />
        </a-form-item>

        <div v-if="formState.ismore">
          <a-form-item label="连接方式" name="connectmethod">
            <a-select v-model:value="formState.connectmethod" allowClear placeholder="请选择" :options="ConnectMethodOptions"></a-select>
          </a-form-item>
          <a-form-item label="角色" name="role">
            <a-select v-model:value="formState.role" allowClear placeholder="请选择" :options="RoleOptions"></a-select>
          </a-form-item>
          <a-form-item label="服务名" name="servicename">
            <a-input v-model:value="formState.servicename" />
          </a-form-item>
        </div>

        <a-form-item label="排序" name="sort">
          <a-input-number style="width: 100%" v-model:value="formState.sort" />
        </a-form-item>
      </a-form>
      <template #footer>
        <div style="display: flex; justify-content: space-between">
          <div style="margin-left: 20px">
            <a-button type="primary" :loading="isLoading" @click="TestDBConfig">连接测试</a-button>
          </div>
          <div style="margin-right: 20px">
            <a-button style="width: 100px; margin-right: 20px" @click="onClose">
              <template #icon> <CloseOutlined /> </template>取 消</a-button
            >
            <a-button type="primary" @click="handleOk()">
              <template #icon> <CheckOutlined /> </template> 确 认</a-button
            >
          </div>
        </div>
      </template>
    </a-drawer>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomIconButtonList from '@/components/commonComponents/CustomIconButtonList.vue'
import { ExclamationCircleOutlined, RedoOutlined, CloseOutlined, CheckOutlined } from '@ant-design/icons-vue'
import { ref, h, watchEffect, watch, reactive, onMounted, createVNode, computed, nextTick, defineOptions } from 'vue'
import { getConfigDBList, addConfigDBAsync, modifyConfigDB, deleteConfigDB, testDB } from '@/api/demoApi/configDB.js'
import { formatDateTime } from '@/utils/dateUtils'
import { message, Modal } from 'ant-design-vue'
import { useUserStore, useDrawerStore, useNavigationStore } from '@/store/index'
import { useGlobalState } from '@/shared/useGlobalState'
import { getButtonList } from '@/api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useI18n } from 'vue-i18n'
const { globalStore } = useGlobalState()
var userStore = ref({})
var drawerStore = ref({})
var navigationStore = ref({})

/* 页面缓存 */
defineOptions({
  name: 'LowCodeDatabaseConfig'
})

/* 根据权限动态加载按钮 */
var inputRoleId = []
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
  drawerStore = useDrawerStore()
  navigationStore = useNavigationStore()
  inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      drawerStore.value = globalStore.value.drawerStore
      navigationStore.value = globalStore.value.navigationStore
      inputRoleId = userStore.value.userRoles
    }
  })
}
const inputFunctionName = ref('LowCodeDatabaseConfig')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData') {
    getTableData()
  }
  if (functionName === 'resetBtn') {
    resetBtn()
  }
  if (functionName === 'openUserDrawer') {
    openUserDrawer()
  }
  if (functionName === 'toggleAccordion') {
    toggleAccordion()
  }
  if (functionName === 'ImportDemoFun') {
    message.success('成功调用导入接口')
  }
  if (functionName === 'ExportDemoFun') {
    message.success('成功调用导出接口')
  }
}
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'LowCodeDatabaseConfig',
    roleids: inputRoleId,
    btnCodes: ['LowCodeDatabaseConfigEdit', 'LowCodeDatabaseConfigDelete']
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
    }
  })
}

/* 计算页头标题 */
const i18n = useI18n()
const PageTitle = ref('数据连接')
// const PageTitle = computed(() => {
//   const isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__

//   // 统一获取真实的 store 引用
//   const realUserStore = isQiankun ? userStore.value : userStore
//   if (!realUserStore || !realUserStore.access_token) {
//     return ''
//   }

//   const realNavStore = isQiankun ? navigationStore.value : navigationStore
//   let tabs = realNavStore?.tabs
//   // 兼容 tabs 可能是 ref([]) 的情况
//   if (!Array.isArray(tabs) && tabs && Array.isArray(tabs.value)) {
//     tabs = tabs.value
//   }
//   if (!Array.isArray(tabs) || tabs.length === 0) {
//     return ''
//   }

//   const realDrawerStore = isQiankun ? drawerStore.value : drawerStore
//   const rawSelected = realDrawerStore?.selected
//   const selected = Array.isArray(rawSelected) ? rawSelected[0] : rawSelected
//   if (!selected) {
//     return ''
//   }

//   const sysCode = import.meta.env.VITE_APP_APPNAME
//   const keys = isQiankun && sysCode ? [`/${sysCode}${selected}`, selected] : [selected]

//   const tab = keys.reduce((found, key) => {
//     if (found) return found
//     return tabs.find((t) => t && t.key === key)
//   }, null)

//   if (!tab || typeof tab !== 'object') {
//     return ''
//   }

//   if (tab.i18nKey) {
//     return i18n.t(tab.i18nKey)
//   }
//   return tab.title || ''
// })
//筛选按钮
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

const nameKey = ref('')
const dbTypeKey = ref(null)
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(20)
var modalTotalCount = ref(0)
const dataSource = ref([])

const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    fixed: 'left',
    width: 50,
    align: 'center',

    customRender: (obj) => {
      return (modalCurrentPage.value - 1) * modalCurrentPageSize.value + obj.index + 1
    }
  },
  {
    title: '连接名称',
    dataIndex: 'name',
    key: 'name',
    minWidth: 200,
    align: 'center'
  },
  {
    title: '连接驱动',
    dataIndex: 'dbType',
    key: 'dbType',
    width: 150,
    align: 'center'
  },
  {
    title: '主机地址',
    dataIndex: 'addr',
    key: 'addr',
    width: 200,
    align: 'center'
  },
  {
    title: '端口',
    dataIndex: 'port',
    key: 'port',
    width: 100,
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    width: 180,
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    width: 180,
    align: 'center'
  },
  {
    title: '最后修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime',
    width: 180,
    align: 'center'
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort',
    width: 80,
    align: 'center'
  },
  {
    title: '操作',
    key: 'operation',
    fixed: 'right',
    width: 150,
    align: 'center'
  }
]

const rowSelection = {
  onChange: (selectedRowKeys, selectedRows) => {
    console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows)
  },
  getCheckboxProps: (record) => ({
    disabled: record.name === 'Disabled User', // Column configuration not to be checked
    name: record.name
  })
}

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData()
}

const drawerTitle = ref('')
function Edit(row) {
  mode.value = 'edit'
  initialized.value = false // 屏蔽首次赋值
  Object.assign(formState, {
    dbConfigId: row.dbConfigId,
    dbtype: row.dbType,
    name: row.name,
    addr: row.addr,
    port: row.port,
    uname: row.uName,
    pwd: row.pwd,
    dbname: row.dbName,
    ismore: row.isMore,
    connectmethod: row.connectmethod,
    role: row.role,
    servicename: row.serviceName,
    sort: row.sort
  })
  openDrawer.value = true
  drawerTitle.value = '编辑数据连接'
  // 等赋值结束后允许 watch
  nextTick(() => {
    initialized.value = true
  })
}

const isLoading = ref(false)

async function TestDBConfig() {
  isLoading.value = true
  await testDB(formState).then((res) => {
    if (res.code == 200 && res.data) {
      message.success('测试成功')
    } else {
      message.error('连接失败，请检查参数重试')
    }
    isLoading.value = false
  })
}

function onClose() {
  ruleForm.value.resetFields()
  openDrawer.value = false
  isLoading.value = false
}

/* Form表单 */

const labelCol = {
  span: 4
}
const DbTypeOptions = ref([
  { value: 'MySql', label: 'MySql' },
  { value: 'SqlServer', label: 'SqlServer' },
  // { value: 2, label: 'Sqlite' },
  { value: 'Oracle', label: 'Oracle' },
  { value: 'PostgreSQL', label: 'PostgreSQL' },
  { value: 'dm', label: '达梦' }
])
const ConnectMethodOptions = ref([
  {
    value: 'SERVICE',
    label: 'SERVICE'
  },
  {
    value: 'SID',
    label: 'SID'
  },
  {
    value: 'SCHEMA',
    label: 'SCHEMA'
  },
  {
    value: 'TNS',
    label: 'TNS'
  }
])
const RoleOptions = ref([
  {
    value: 'Default',
    label: 'Default'
  },
  {
    value: 'SYSDBA',
    label: 'SYSDBA'
  },
  {
    value: 'SYSOPER',
    label: 'SYSOPER'
  }
])
const ruleForm = ref(null)
const formState = reactive({
  dbConfigId: null,
  dbtype: null,
  name: '',
  addr: '',
  port: '',
  uname: '',
  pwd: '',
  dbname: '',
  // pattern: '',
  ismore: false,
  connectmethod: null,
  role: null,
  servicename: '',
  sort: 0
})

const mode = ref('add') // 当前弹窗模式
const initialized = ref(false) // 是否初始化完成（仅对 edit 有效）

/* 监听：改变数据库连接驱动修改模式默认值 */
watch(
  () => formState.dbtype,
  () => {
    if (mode.value === 'edit' && !initialized.value) {
      return
    }
    formState.ismore = false
    // formState.pattern = ''
    formState.role = ''
    formState.port = ''
    formState.servicename = ''
    formState.connectmethod = ''
    switch (formState.dbtype) {
      case 'SqlServer':
        // formState.pattern = 'dbo'
        break
      case 'Oracle':
        // formState.pattern = '与用户同名'
        formState.port = '1521'
        break
      case 'PostgreSQL':
        // formState.pattern = 'public'
        break
    }
  }
)

async function getTableData(showMes) {
  var obj = {
    name: nameKey.value,
    dbtype: dbTypeKey.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getConfigDBList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      // res.data.forEach((item) => {
      //   item.key = item.dbConfigId
      // })
      dataSource.value = res.data
      modalTotalCount.value = res.total
      if (!!showMes) {
        message.success(res.message)
      }
    }
  })
}

function resetBtn() {
  nameKey.value = ''
  dbTypeKey.value = null
}

const openDrawer = ref(false)
function openUserDrawer() {
  mode.value = 'add'
  formState.dbConfigId = ''
  formState.dbtype = ''
  formState.name = ''
  formState.addr = ''
  openDrawer.value = true
  drawerTitle.value = '新建数据连接'
}

function handleOk() {
  if (formState.dbConfigId) {
    ModifyDB()
  } else {
    Add()
  }
  isLoading.value = false
}

async function Add() {
  await addConfigDBAsync(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户添加成功!')
      ruleForm.value.resetFields()
      getTableData(false)
      openDrawer.value = false
    }
  })
}
async function ModifyDB() {
  await modifyConfigDB(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('数据信息已更新!')
      openDrawer.value = false
      formState.value = reactive({
        name: '',
        address: ''
      })
      getTableData(false)
    }
  })
}

//删除
function Delete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该岗位吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = DelUser(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

async function DelUser(resolve, reject, id) {
  const params = {
    id: id
  }
  await deleteConfigDB(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData(false)
    }
  })
}

onMounted(() => {
  getAllButton()
  getTableData()
})
</script>
<style lang="scss">
.SystemSettingDataBaseConfig {
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

  .accordion {
    overflow: hidden;

    .accordion-content {
      max-height: 50px;
      overflow: hidden;
      transition: max-height 0.3s ease-in-out, padding 0.1s ease-in-out;
      background: #fff;
      padding: 0 10px;
      margin-bottom: 10px;

      .area-span {
        margin: 0px 5px;
        padding: 5px 10px;
        cursor: pointer;
        border-radius: 5px;
        transition: background 0.3s;
      }

      .area-span.active {
        background: #007bff;
        color: white;
      }
    }

    /* 展开面板 */
    .open {
      max-height: 250px;
    }
  }

  .paginationStyle {
    height: 50px;
    line-height: 50px;
    text-align: right;
    position: absolute;
    padding-top: 10px;
    padding-right: 30px;
    right: 0px;
    bottom: 0px;
    width: 100%;
  }
}
</style>
