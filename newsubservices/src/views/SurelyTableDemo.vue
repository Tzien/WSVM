<template>
  <div class="SurelyTableDemo">
    <div
      style="display: flex; align-items: center; justify-content: space-between; height: 36px; line-height: 36px; margin-top: 17px">
      <a-typography-paragraph style="overflow: auto; padding-left: 20px">
        <blockquote style="color: black; border-inline-start: 5px solid rgb(37, 97, 239) !important">
          <span style="font-weight: bold; color: #111111; font-size: 16px">{{ PageTitle }}</span>
        </blockquote>
      </a-typography-paragraph>
      <div style="padding-right: 20px">
        <CustomIconButtonList :ParamsRoleId="inputRoleId" :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['ImportDemo', 'ExportDemo', 'FilterDemo']" :isChecked="isExpand"
          @buttonClick="handleButtonClick"></CustomIconButtonList>
        <!-- <a-button style="margin: 0 5px" shape="circle" :type="''" :icon="h(DownloadOutlined)" />
        <a-button style="margin: 0 5px" shape="circle" :type="''" :icon="h(UploadOutlined)" />
        <a-button style="margin-left: 10px" shape="circle" :icon="h(FilterOutlined)" :type="isExpand ? 'primary' : ''" @click="toggleAccordion()" /> -->
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
        <CustomButtonList :ParamsRoleId="inputRoleId" :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['test1']" @buttonClick="handleButtonClick"></CustomButtonList>
        <!-- <a-button style="margin-left: 20px; background-color: #2461a6" :icon="h(PlusOutlined)" @click="openUserDrawer()" type="primary">新建</a-button> -->
      </div>
    </div>

    <div>
      <a-divider />
    </div>
    <div class="accordion" v-show="isExpand">
      <div ref="accordionRef" class="accordion-content">
        <span style="margin-left: 20px">
          姓名：
          <a-input v-model:value="UserNameKey" placeholder="请输入姓名" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <CustomButtonList :ParamsRoleId="inputRoleId" :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['test2', 'resetDemo']" @buttonClick="handleButtonClick"></CustomButtonList>

        <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData()">查询数据</a-button>
        <a-button style="margin-left: 20px" :icon="h(PlusOutlined)" @click="openUserDrawer()" type="primary">添加用户</a-button> -->
      </div>
    </div>

    <s-table :columns="columns" :height="calcHeight" headerAlign="center" align="center" size="small" stripe
      :pagination="false" rowKey="id" :scroll="{ x: 2000 }" :row-selection="rowSelection" :data-source="dataSource">
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'modifyTime' && text">
          {{ formatDateTime(text) }}
        </template>
        <template v-if="column.dataIndex === 'createTime'">
          {{ formatDateTime(text) }}
        </template>
        <template v-if="column.key === 'operation'">
          <a @click="Edit(record)" style="color: #2461a6; margin-right: 10px"
            v-if="allDyBtn.find((item) => item.functionCode === 'test3')">编辑</a>
          <a @click="Delete(record.id)" style="color: #2461a6"
            v-if="allDyBtn.find((item) => item.functionCode === 'test4')">{{ $t('drawer.Save') }}</a>


          <!-- <a style="color: #2461a6; margin-right: 10px" @click="Edit(record)">编辑</a>
          <a style="color: #2461a6" @click="Delete(record.id)">删除</a> -->
        </template>
      </template>
    </s-table>
    <div class="paginationStyle" style="margin-top: 10px">
      <a-pagination v-model:current="modalCurrentPage" v-model:page-size="modalCurrentPageSize" :total="modalTotalCount"
        align="right" :pageSizeOptions="['20', '30', '45', '60']" showSizeChanger
        :show-total="(total) => `${$t('message.drawer.TotalOf')} ${total} ${$t('message.drawer.Items')}`"
        @change="modalPageSizeChange" />
    </div>
    <a-drawer v-model:open="openDrawer" :body-style="{ padding: '20px 50px' }" :footer-style="{ textAlign: 'right' }"
      :title="drawerTitle" placement="right">
      <a-form :model="formState">
        <a-form-item :label="$t('table.Name')">
          <a-input v-model:value="formState.name" />
        </a-form-item>
        <a-form-item :label="$t('table.Address')">
          <a-input v-model:value="formState.address" />
        </a-form-item>
      </a-form>
      <template #footer>
        <a-button style="width: 100px; margin-right: 10px" @click="onClose">
          <template #icon>
            <CloseOutlined />
          </template>取 消</a-button>
        <a-button type="primary" @click="handleOk()">
          <template #icon>
            <CheckOutlined />
          </template> 确 认</a-button>
      </template>
    </a-drawer>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomIconButtonList from '@/components/commonComponents/CustomIconButtonList.vue'
import { ExclamationCircleOutlined, RedoOutlined, CloseOutlined, CheckOutlined } from '@ant-design/icons-vue'
import { SearchOutlined, PlusOutlined, DownloadOutlined, UploadOutlined, FilterOutlined } from '@ant-design/icons-vue'
import { ref, h, watchEffect, reactive, onMounted, createVNode, computed, nextTick, defineOptions } from 'vue'
import { getUserInfo, useAddUserAsync, modifySysJob, deleteApi } from '@/api/demoApi/demo.js'
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
  name: 'XT0002SurelyTableDemo'
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
const inputFunctionName = ref('XT0002SurelyTableDemo')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData()') {
    getTableData()
  }
  if (functionName === 'resetBtn') {
    resetBtn()
  }
  if (functionName === 'openUserDrawer()') {
    openUserDrawer()
  }
  if (functionName === 'toggleAccordion()') {
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
    menuCode: 'XT0002SurelyTableDemo',
    roleids: inputRoleId,
    btnCodes: ['test3', 'test4']
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
    }
  })
}

/* 计算页头标题 */
const i18n = useI18n()
const PageTitle = computed(() => {
  return 'SurlyTable Demo'
})
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

const UserNameKey = ref('')
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(20)
var modalTotalCount = ref(0)
const dataSource = ref([])

const columns = computed(() => [
  {
    title: i18n.t('XT0002SurelyTableDemo.TEST'),
    dataIndex: 'name',
    key: 'name',
    align: 'center'
  },
  {
    title: i18n.t('table.Address'),
    dataIndex: 'address',
    key: 'address',
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    align: 'center'
  },
  {
    title: '修改人员',
    dataIndex: 'modifyName',
    key: 'modifyName',
    align: 'center'
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime',
    align: 'center'
  },
  {
    title: '操作',
    key: 'operation',
    fixed: 'right',
    width: 150,
    align: 'center'
  }
])

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
const openDrawer = ref(false)
function Edit(row) {
  formState.name = row.name
  formState.address = row.address
  formState.id = row.id
  openDrawer.value = true
  drawerTitle.value = '编辑用户'
}

/* Form表单 */
const formState = reactive({
  name: '',
  address: ''
})

async function getTableData(showMes) {
  var obj = {
    name: UserNameKey.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getUserInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      dataSource.value = res.data
      modalTotalCount.value = res.total
      if (!!showMes) {
        message.success(res.message)
      }
    }
  })
}

function resetBtn() {
  UserNameKey.value = ''
}

function openUserDrawer() {
  formState.id = ''
  formState.name = ''
  formState.address = ''
  openDrawer.value = true
  drawerTitle.value = '添加用户'
}

function handleOk() {
  if (formState.id) {
    ModifyUser()
  } else {
    AddUser()
  }
}

function onClose() {
  formState.id = ''
  formState.name = ''
  formState.address = ''
  openDrawer.value = false
}

async function AddUser() {
  await useAddUserAsync(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户添加成功!')
      // openDrawer.value = false
      formState.value = reactive({
        name: '',
        address: ''
      })
      getTableData(false)
    }
  })
}
async function ModifyUser() {
  await modifySysJob(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户信息已更新!')
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
  await deleteApi(params).then((res) => {
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
.SurelyTableDemo {
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
