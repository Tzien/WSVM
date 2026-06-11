<template>
  <div class="DataApi">
    <div class="top">
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
            :BtnFunctionNameArray="['DataApi2']"
            :isChecked="isExpand"
            @buttonClick="handleButtonClick"
          ></CustomIconButtonList>
          <a-button @click="getTableData(true)" style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['DataApi0']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
        </div>
      </div>
    </div>
    <div>
      <a-divider />
    </div>
    <div class="bottom">
      <div class="left" v-show="!showForm && !showPerview">
        <a-tree :tree-data="apiCategoryTreeData" @select="onSelect" defaultExpandAll />
      </div>
      <div class="right" v-show="!showForm && !showPerview">
        <div class="accordion" v-show="isExpand">
          <div ref="accordionRef" class="accordion-content">
            <span style="margin-left: 20px">
              名称：
              <a-input v-model:value="name" placeholder="请输入名称" style="width: 250px; margin-right: 5px" allowClear />
            </span>
            <span style="margin-left: 20px">
              类型：
              <a-select
                v-model:value="type"
                allowClear
                placeholder="请选择类型"
                style="width: 250px; margin-right: 5px"
                :options="dataApiTypeOptions"
              ></a-select>
            </span>
            <CustomButtonList
              :ParamsRoleId="inputRoleId"
              :ParamsFunctionName="inputFunctionName"
              :BtnFunctionNameArray="['DataApi5', 'DataApi1']"
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
          rowKey="dataApiId"
          :row-selection="rowSelection"
          :data-source="dataSource"
          style="padding-right: 10px"
        >
          <template #bodyCell="{ text, column, record }">
            <template v-if="column.dataIndex === 'createTime'">
              {{ formatDateTime(text) }}
            </template>

            <template v-if="column.dataIndex === 'type'">
              <span v-if="text === '0'"> 静态数据 </span>
              <span v-else-if="text === '1'"> SQL操作 </span>
              <span v-else-if="text === '2'"> API操作 </span>
              <span v-else-if="text === '3'"> 组态接口 </span>
            </template>

            <template v-if="column.key === 'operation'">
              <a
                @click="showFormVue(record.dataApiId)"
                style="color: #2461a6; margin-right: 10px"
                v-if="allDyBtn.find((item) => item.functionCode === 'DataApi3')"
                >编辑</a
              >
              <a
                @click="perview(record.dataApiId, record.dataHandler,record.type)"
                style="color: rgb(243,208,123); margin-right: 10px"
                v-if="allDyBtn.find((item) => item.functionCode === 'DataApi6')"
                >预览</a
              >
              <a @click="Delete(record.dataApiId)"  style="color: rgb(255, 77, 79)" v-if="allDyBtn.find((item) => item.functionCode === 'DataApi4')">删除</a>
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
      </div>
    </div>

    <!-- 全屏显示 Form.vue -->
    <!-- <Form v-if="showForm" :dataApiId="dataApiId" @close="showForm = false" @done="onFormDone" />

    <Preview :dataApiId="dataApiId" :dataHandler="dataHandler" @close="showPerview = false" v-if="showPerview" /> -->
    <div v-if="showForm" class="full-page">
      <Form :dataApiId="dataApiId" @close="showForm = false" @done="onFormDone" />
    </div>

    <div v-if="showPerview" class="full-page">
      <Preview :dataApiId="dataApiId" :dataHandler="dataHandler" :dataType="dataType" @close="showPerview = false" />
    </div>
  </div>
</template>
<script lang="ts" setup>
import { ref, h, watchEffect, watch, reactive, onMounted, createVNode, computed, nextTick, defineOptions } from 'vue'
import { getApiCategoryDictApi, getListApi, deleteApi } from '@/api/DataApi/DataApi'
import Form from './Form.vue'
import Preview from './Preview.vue'
import { formatDateTime } from '@/utils/dateUtils'
import { ExclamationCircleOutlined, RedoOutlined, CloseOutlined, CheckOutlined } from '@ant-design/icons-vue'
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
  name: 'DataApi'
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
onMounted(() => {
  getAllButton()
  getApiCategoryDict()
  getTableData()
})
//接口分类树
const apiCategoryTreeData = ref([
  {
    title: '接口分类',
    children: []
  }
])
async function getApiCategoryDict() {
  var obj = {
    dictCode: 'DataApiCategory'
  }
  await getApiCategoryDictApi(obj).then((res) => {
    const children = []
    res.data.items.forEach((c) => {
      children.push({
        title: c.itemName,
        key: c.dictDetailId
      })
    })
    apiCategoryTreeData.value[0].children = children
  })
}

//新建/编辑页面
const showForm = ref(false)

const dataApiId = ref('')
const dataHandler = ref('')
const dataType=ref('')
function showFormVue(id) {
  showPerview.value = false
  showForm.value = true
  dataApiId.value = id
}


function onFormDone() {
  showForm.value = false
  getTableData(false)
  console.log('Form 组件已完成并关闭')
}

//删除
function Delete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = DeleteDataApi(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

async function DeleteDataApi(resolve, reject, id) {
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

const inputFunctionName = ref('DataApi')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData()') {
    getTableData()
  }
  if (functionName === 'resetBtn()') {
    resetBtn()
  }
  if (functionName === 'showFormVue()') {
    showFormVue()
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
    menuCode: 'DataApi',
    roleids: inputRoleId,
    btnCodes: ['DataApi3', 'DataApi4', 'DataApi6']
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
    }
  })
}

/* 计算页头标题 */
const i18n = useI18n()
const PageTitle = ref('数据接口')
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

const name = ref('')
const type = ref('')
const category = ref('')
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(10)
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
    title: '名称',
    dataIndex: 'name',
    key: 'name',
    minWidth: 200,
    align: 'center'
  },
  {
    title: '编码',
    dataIndex: 'code',
    key: 'code',
    width: 150,
    align: 'center'
  },
  {
    title: '接口分类',
    dataIndex: 'categoryName',
    key: 'categoryName',
    width: 150,
    align: 'center'
  },
  {
    title: '类型',
    dataIndex: 'type',
    key: 'type',
    width: 200,
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

async function getTableData(showMes) {
  var obj = {
    name: name.value,
    type: type.value,
    category: category.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getListApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = item.dataApiId
      })
      dataSource.value = res.data
      modalTotalCount.value = res.total

      if (!!showMes) {
        message.success(res.message)
      }
    } else {
      message.warning(res.message)
    }
  })
}

const dataApiTypeOptions = ref([
  { value: '0', label: '静态数据' },
  { value: '1', label: 'SQL操作' },
  { value: '2', label: 'API操作' },
  { value: '3', label: '组态接口' }
])

function onSelect(selectedKeys, info) {
  if (info.node.key != '0-0') {
    category.value = info.node.key
    console.log('节点 key:', info.node.key)
  } else {
    category.value = ''
  }
  getTableData()
}

function resetBtn() {
  name.value = ''
  type.value = null
  getTableData()
}

const showPerview = ref(false)
function perview(id, dataHandler2,type) {
  showForm.value = false
  showPerview.value = true
  dataHandler.value = dataHandler2
  dataApiId.value = id
  dataType.value=type
}
</script>

<style lang="scss">
.DataApi {
  background-color: #fff;
  height: 100%;
  overflow: auto;
  position: relative;

.full-page {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: #fff;
  z-index: 999;
}
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
      transition:
        max-height 0.3s ease-in-out,
        padding 0.1s ease-in-out;
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
  .top {
    width: 100%;
    // border: 1px solid black;
  }
  .bottom {
    height: calc(100% - 80px);
    display: flex;
    // border: 1px solid red;
    .left {
      width: 15%;
      overflow-y: auto;
      margin-right: 10px;
      padding-top: 15px;
      padding-left: 5px;
      border-right: 1px solid gainsboro;
    }
    .right {
      width: 85%;
      height: 100%;

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
  }
}
</style>
