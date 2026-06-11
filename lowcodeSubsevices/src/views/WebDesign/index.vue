<template>
  <div class="webDesignIndex">
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
          :BtnFunctionNameArray="['ImportDemo', 'ExportDemo', 'FilterDemo']"
          :isChecked="isExpand"
          @buttonClick="handleButtonClick"
        ></CustomIconButtonList>
        <!-- <a-button style="margin: 0 5px" shape="circle" :type="''" :icon="h(DownloadOutlined)" />
        <a-button style="margin: 0 5px" shape="circle" :type="''" :icon="h(UploadOutlined)" />
        <a-button style="margin-left: 10px" shape="circle" :icon="h(FilterOutlined)" :type="isExpand ? 'primary' : ''" @click="toggleAccordion()" /> -->
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['test1']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>
        <!-- <a-button style="margin-left: 20px; background-color: #2461a6" :icon="h(PlusOutlined)" @click="openUserDrawer()" type="primary">新建</a-button> -->
      </div>
    </div>

    <div>
      <a-divider />
    </div>
    <div class="accordion" v-show="isExpand">
      <div ref="accordionRef" class="accordion-content">
        <span style="margin-left: 20px">
          关键词：
          <a-input v-model:value="UserNameKey" placeholder="请输入关键词" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <span style="margin-left: 20px">
          分类：
          <a-input v-model:value="UserNameKey" placeholder="请输入分类" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <span style="margin-left: 20px">
          类型：
          <a-input v-model:value="UserNameKey" placeholder="请输入类型" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <span style="margin-left: 20px">
          状态：
          <a-input v-model:value="UserNameKey" placeholder="请输入状态" style="width: 250px; margin-right: 5px" allowClear />
        </span>
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['test2', 'resetDemo']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>

        <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData()">查询数据</a-button>
        <a-button style="margin-left: 20px" :icon="h(PlusOutlined)" @click="openUserDrawer()" type="primary">添加用户</a-button> -->
      </div>
    </div>

    <div
      v-if="loading"
      style="
        display: flex;
        flex-direction: column;
        gap: 12px;
        padding: 16px;
        border: 1px solid #f0f0f0;
        border-radius: 4px;
        background: #fff;
        height: calc(100vh - 96px - 35px - 40px - 80px);
      "
    >
      <!-- 表头 -->
      <div style="display: flex; gap: 16px; font-weight: bold">
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
      </div>
      <!-- 表格行 -->
      <div v-for="n in 13" :key="n" style="display: flex; gap: 16px">
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
        <a-skeleton-input style="width: 500px" active />
      </div>
    </div>

    <s-table
      v-if="!loading"
      :columns="columns"
      :height="calcHeight"
      headerAlign="center"
      align="center"
      size="small"
      stripe
      :pagination="false"
      rowKey="formDesignId"
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
        <template v-if="column.dataIndex === 'status'">
          <a-tag v-if="text == 0" color="default">未发布</a-tag>
          <a-tag v-if="text == 1" color="warning">已修改</a-tag>
          <a-tag v-if="text == 2" color="success">已发布</a-tag>
        </template>
        <template v-if="column.dataIndex === 'formCategoryId'">
          <!-- <span v-if="text == 'GZ'" color="default">钢种管理</span>
          <span v-if="text == 'HJ'" color="warning">合金管理</span>
          <span v-if="text == 'Other'" color="success">其他材料</span> -->
          <span>{{ getCategoryLabel(text) }}</span>
        </template>
        <template v-if="column.key === 'operation'">
          <!-- <a @click="Edit(record)" style="color: #2461a6; margin-right: 10px" v-if="allDyBtn.find((item) => item.functionCode === 'test3')">编辑</a>
          <a @click="Delete(record.id)" style="color: #2461a6" v-if="allDyBtn.find((item) => item.functionCode === 'test4')">删除</a> -->

          <!-- <a style="color: #2461a6; margin-right: 10px" @click="Edit(record)">编辑</a>
          <a style="color: #2461a6" @click="Delete(record.id)">删除</a> -->

          <a style="color: #2461a6; margin-right: 10px" @click="Edit(record)">编辑</a>
          <a style="color: #2461a6; margin-right: 10px" @click="handlePreview(record.formDesignId)">预览表单</a>
          <a style="color: #2461a6; margin-right: 10px" @click="handleDownload(record)">代码生成</a>
          <a style="color: #2461a6" @click="Delete(record.formDesignId)">删除表单</a>
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
    <!-- v-model:isShowStepDrawer="showstep"   -->
    <StepDesign
      :isShowStepDrawer="showstep"
      @update:isShowStepDrawer="handleDrawerChange"
      :categoryOptions="formDesignCategoryOptions"
      v-if="showstep"
      :rowData="selectRow"
    ></StepDesign>
    <DownloadModal @register="registerDownloadModal" />
    <PreviewModal @register="registerPreview" />
  </div>
</template>
<script setup>
import DownloadModal from './component/DownloadModal.vue'
import { PreviewModal } from '@/components/CommonModal'
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomIconButtonList from '@/components/commonComponents/CustomIconButtonList.vue'
import StepDesign from './component/StepDesign.vue'
import { ExclamationCircleOutlined, RedoOutlined, CloseOutlined, CheckOutlined } from '@ant-design/icons-vue'
// import { SearchOutlined, PlusOutlined, DownloadOutlined, UploadOutlined, FilterOutlined } from '@ant-design/icons-vue'
import { ref, h, watchEffect, reactive, onMounted, createVNode, computed, nextTick, defineOptions, unref } from 'vue'
import { getFormDb, deleteFormDb } from '@/api/demoApi/demo.js'
import { formatDateTime } from '@/utils/dateUtils'
import { message, Modal } from 'ant-design-vue'
import { useUserStore, useDrawerStore, useNavigationStore } from '@/store/index'
import { useGlobalState } from '@/shared/useGlobalState'
import { getButtonList } from '@/api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useI18n } from 'vue-i18n'
import { useBaseStore } from '@/store/base'
import { useModal } from '@/components/Modal'
const [registerDownloadModal, { openModal: openDownloadModal }] = useModal()
const [registerPreview, { openModal: openPreviewModal }] = useModal()

const { globalStore } = useGlobalState()
const baseStore = useBaseStore()

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
    getTableData(true)
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

const loading = ref(true)

/* 计算页头标题 */
const i18n = useI18n()
const PageTitle = ref('表单设计')
// const PageTitle = computed(() => {
//   if (userStore.access_token == '') {
//     return
//   }
//   if (qiankunWindow.__POWERED_BY_QIANKUN__) {
//     return i18n.t(navigationStore.value.tabs.find((a) => a.key == drawerStore.value.selected[0]).i18nKey)
//   } else {
//     if (drawerStore.selected[0] == '/previewModel') {
//       return '功能预览'
//     }
//     return i18n.t(navigationStore.tabs.find((a) => a.key == drawerStore.selected[0]).i18nKey)
//   }
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

function handleDownload(record) {
  openDownloadModal(true, {
    tables: record.formDbs,
    id: record.formDesignId,
    hasPackage: record.hasPackage && false,
    fullName: record.name,
    webType: record.webType
  })
}

function handlePreview(id) {
  openPreviewModal(true, { type: 'webDesign', id })
}

const UserNameKey = ref('')
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(20)
var modalTotalCount = ref(0)
const dataSource = ref([])
const formDesignCategoryOptions = ref([])

function getOptions() {
  baseStore.getDictionaryData('FormDesignCategory').then((res) => {
    // @ts-ignore
    res.forEach((item) => {
      formDesignCategoryOptions.value.push({ value: item.id, label: item.fullName })
    })
  })
}

function getCategoryLabel(value) {
  return formDesignCategoryOptions.value.find((o) => o.value === value)?.label || value
}

const columns = [
  {
    title: '序号',
    key: 'no',
    dataIndex: 'no',
    fixed: 'left',
    width: 10,
    align: 'center',
    customRender: (obj) => {
      return (modalCurrentPage.value - 1) * modalCurrentPageSize.value + obj.index + 1
    }
  },
  {
    title: '名称',
    dataIndex: 'name',
    key: 'name',
    width: 100,
    align: 'center'
  },
  {
    title: '编码',
    dataIndex: 'code',
    key: 'code',
    width: 100,
    align: 'center'
  },
  {
    title: '分类',
    dataIndex: 'formCategoryId',
    key: 'formCategoryId',
    width: 100,
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    width: 100,
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    width: 100,
    align: 'center'
  },
  {
    title: '最后修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime',
    width: 100,
    align: 'center'
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort',
    width: 50,
    align: 'center'
  },
  {
    title: '状态',
    dataIndex: 'status',
    key: 'status',
    width: 60,
    align: 'center'
  },
  {
    title: '操作',
    key: 'operation',
    fixed: 'right',
    width: 320,
    align: 'center'
  }
]

const rowSelection = {
  onChange: (selectedRowKeys, selectedRows) => {
    console.log(`selectedRowKeys: ${selectedRowKeys}`, 'selectedRows: ', selectedRows)
  },
  getCheckboxProps: (record) => ({
    disabled: record.name === 'Disabled User',
    name: record.name
  })
}

function handleDrawerChange(val) {
  showstep.value = val
  if (!val) {
    getTableData(false)
  }
}

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData(true)
}

let selectRow = unref({})

function Edit(row) {
  selectRow = row
  if (selectRow.tableJson) {
    selectRow.maxStep = 2
  } else if (selectRow.formJson) {
    selectRow.maxStep = 1
  } else {
    selectRow.maxStep = 0
  }
  showstep.value = true
}

async function getTableData(showMes) {
  loading.value = showMes
  var obj = {
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getFormDb(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((element) => {})
      dataSource.value = res.data
      modalTotalCount.value = res.total
      if (!!showMes) {
        message.success(res.message)
      }
      setTimeout(() => {
        loading.value = false
      }, 2000) // 模拟异步加载
    }
  })
}

function resetBtn() {
  UserNameKey.value = ''
}

const showstep = ref(false)
function openUserDrawer() {
  selectRow = {}
  showstep.value = true
}

//删除
function Delete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该表单吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = DelForm(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

async function DelForm(resolve, reject, id) {
  await deleteFormDb(id).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData(false)
    } else {
      message.error(res.message)
    }
  })
}

onMounted(() => {
  getAllButton()
  getTableData(true)
  getOptions()
})
</script>
<style lang="scss">
.webDesignIndex {
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
