<template>
  <div class="SurelyTableDemo">
    <div style="display: flex; align-items: center; justify-content: space-between; height: 36px; line-height: 36px; margin-top: 17px">
      <a-typography-paragraph style="overflow: auto; padding-left: 20px">
        <blockquote style="color: black; border-inline-start: 5px solid rgb(37, 97, 239) !important">
          <span style="font-weight: bold; color: #111111; font-size: 16px">纪检监督台账管理</span>
        </blockquote>
      </a-typography-paragraph>
      <div style="padding-right: 20px">
        <a-button shape="circle" :type="''" :icon="h(DownloadOutlined)" />
        <a-button style="margin-left: 10px" shape="circle" :type="''" :icon="h(UploadOutlined)" />
        <a-button style="margin-left: 10px" shape="circle" :icon="h(FilterOutlined)" :type="isExpand ? 'primary' : ''" @click="toggleAccordion" />
        <a-button style="margin-left: 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
        <a-button style="margin-left: 20px" :icon="h(PlusOutlined)" @click="openUserModal()" type="primary">新建</a-button>
      </div>
    </div>
    <div>
      <a-divider />
    </div>
    <div class="accordion" ref="panelRef" v-if="isExpand">
      <div ref="accordionRef" class="accordion-content">
        <span style="margin-left: 20px">
          姓名：
          <a-input v-model:value="UserNameKey" placeholder="请输入姓名" style="width: 250px" allowClear />
        </span>
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['test1', 'test2']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>

        <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData()">查询数据</a-button>
        <a-button style="margin-left: 20px" :icon="h(PlusOutlined)" @click="openUserModal()" type="primary">添加用户</a-button>
      </div>
    </div>

    <s-table :columns="columns" :height="calcHeight" :pagination="false" rowKey="id" :data-source="dataSource">
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'modifyTime' && text">
          {{ formatDateTime(text) }}
        </template>
        <template v-if="column.dataIndex === 'createTime'">
          {{ formatDateTime(text) }}
        </template>
        <template v-if="column.key === 'operation'">
          <a @click="Edit(record)" v-if="allDyBtn.find((item) => item.functionCode === 'test3')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'test3')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="Delete(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'test4')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'test4')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>

          <!-- <a @click="Edit(record)" style="margin-right: 10px">编辑</a>
        <a @click="Delete(record.id)">删除</a> -->
        </template>
      </template>
    </s-table>
    <div class="paginationStyle" style="margin-top: 10px">
      <a-pagination
        v-model:current="modalCurrentPage"
        v-model:page-size="modalCurrentPageSize"
        :total="modalTotalCount"
        align="right"
        :pageSizeOptions="['15', '30', '45', '60']"
        showSizeChanger
        :show-total="(total) => `共 ${modalTotalCount} 条`"
        @change="modalPageSizeChange"
      />
    </div>
    <a-modal v-model:open="openModal" :title="ModalTitle" @ok="handleOk" ok-text="确认" cancel-text="取消" centered>
      <a-form :model="formState" :label-col="labelCol" :wrapper-col="wrapperCol">
        <a-form-item label="姓名">
          <a-input v-model:value="formState.name" />
        </a-form-item>
        <a-form-item label="住址">
          <a-input v-model:value="formState.address" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { SearchOutlined, PlusOutlined, ExclamationCircleOutlined, DownloadOutlined, UploadOutlined, FilterOutlined, RedoOutlined } from '@ant-design/icons-vue'
// import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue'
import { ref, h, watchEffect, reactive, onMounted, createVNode, computed, nextTick, onBeforeUnmount } from 'vue'
import { getUserInfo, useAddUserAsync, modifySysJob, deleteApi } from '@/api/demoApi/demo.js'
import { formatDateTime } from '@/utils/dateUtils'
import { message, Modal } from 'ant-design-vue'
import { useUserStore } from '@/store/user'
import { useGlobalState } from '@/shared/useGlobalState'
import { getButtonList } from '@/api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
const { globalStore } = useGlobalState()
var userStore = ref({})

/* 根据权限动态加载按钮 */
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
const inputFunctionName = ref('XT0002SurelyTableDemo')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData()') {
    getTableData()
  }
  if (functionName === 'openUserModal()') {
    openUserModal()
  }
}
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'XT0002SurelyTableDemo',
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

//筛选按钮
const panelHeight = ref(80)
const panelRef = ref(null)
const calcHeight = computed(() => {
  return `calc(100vh - 96px - 53px - 21px - ${panelHeight.value}px - 50px - 35px)`
})

// 监听窗口变化，动态调整面板高度
const updatePanelHeight = async () => {
  await nextTick()
  if (panelRef.value) {
    panelRef.value.addEventListener(
      'transitionend',
      () => {
        panelHeight.value = panelRef.value.clientHeight
      },
      { once: true }
    ) // `once: true` 确保事件只触发一次
  }
}
//监听窗口大小变化
onMounted(() => {
  window.addEventListener('resize', updatePanelHeight)
  updatePanelHeight() // 初始计算
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', updatePanelHeight)
})

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
var modalCurrentPageSize = ref(15)
var modalTotalCount = ref(0)
const dataSource = ref([])

const columns = [
  {
    title: '姓名',
    dataIndex: 'name',
    key: 'name'
  },
  {
    title: '住址',
    dataIndex: 'address',
    key: 'address'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '修改人员',
    dataIndex: 'modifyName',
    key: 'modifyName'
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime'
  },
  {
    title: '操作',
    key: 'operation',
    fixed: 'right',
    width: 150
  }
]

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData()
}

const ModalTitle = ref('')
const openModal = ref(false)
function Edit(row) {
  formState.name = row.name
  formState.address = row.address
  formState.id = row.id
  openModal.value = true
  ModalTitle.value = '编辑用户'
}

/* Form表单 */
const formState = reactive({
  name: '',
  address: ''
})
const labelCol = {
  style: {
    width: '120px'
  }
}
const wrapperCol = {
  span: 14
}

async function getTableData() {
  var obj = {
    name: UserNameKey.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getUserInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      dataSource.value = res.data
      modalTotalCount.value = res.total
      message.success(res.message)
    }
  })
}

function openUserModal() {
  formState.id = ''
  formState.name = ''
  formState.address = ''
  openModal.value = true
  ModalTitle.value = '添加用户'
}

function handleOk() {
  if (formState.id) {
    ModifyUser()
  } else {
    AddUser()
  }
}

async function AddUser() {
  await useAddUserAsync(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户添加成功!')
      openModal.value = false
      formState.value = reactive({
        name: '',
        address: ''
      })
      getTableData()
    }
  })
}
async function ModifyUser() {
  await modifySysJob(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('用户信息已更新!')
      openModal.value = false
      formState.value = reactive({
        name: '',
        address: ''
      })
      getTableData()
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
      getTableData()
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
