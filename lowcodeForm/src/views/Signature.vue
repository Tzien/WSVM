<template>
  <div class="Signature">
    <div class="right">
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
            :BtnFunctionNameArray="['signature2']"
            :isChecked="isExpand"
            @buttonClick="handleButtonClick"
          ></CustomIconButtonList>
          <a-button @click="getTableData(true)" style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(RedoOutlined)" />
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['signature0']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
        </div>
      </div>

      <div>
        <a-divider />
      </div>
      <div class="accordion" v-show="isExpand">
        <div ref="accordionRef" class="accordion-content">
          <span style="margin-left: 20px">
            名称：
            <a-input v-model:value="name" placeholder="请输入名称" style="width: 250px; margin-right: 5px" allowClear />
          </span>

          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['signature5', 'signature1']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
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
        rowKey="signatureId"
        :row-selection="rowSelection"
        :data-source="dataSource"
        style="padding-right: 10px"
      >
        <template #bodyCell="{ text, column, record }">
          <template v-if="column.dataIndex === 'createTime'">
            {{ formatDateTime(text) }}
          </template>

          <template v-if="column.key === 'operation'">
            <a @click="detail(record.signatureId)" style="color: #2461a6; margin-right: 10px" v-if="allDyBtn.find((item) => item.functionCode === 'signature3')"
              >编辑</a
            >
            <a @click="Delete(record.signatureId)" style="color: #2461a6" v-if="allDyBtn.find((item) => item.functionCode === 'signature4')">删除</a>
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

    <!-- 抽屉 -->
    <a-drawer
      v-model:open="open"
      :title="drawerTitle"
      class="custom-class"
      root-class-name="root-class-name"
      :root-style="{ color: 'blue' }"
      style="color: red"
      size="large"
      placement="right"
    >
      <a-form ref="formRef" :model="formState" :label-col="labelCol" :rules="rules">
        <a-form-item label="名称" name="name">
          <a-input v-model:value="formState.name" />
        </a-form-item>
        <a-form-item label="编码" name="code">
          <a-input :maxlength="20" v-model:value="formState.code" />
        </a-form-item>
        <a-form-item label="授权人">
          <a-tree-select v-model:value="formState.userIdList" tree-checkable allow-clear :tree-data="filteredTreeData" />
        </a-form-item>
        <a-form-item label="签章">
          <a-upload list-type="picture-card" :file-list="fileList" :before-upload="beforeUpload" :max-count="1" @preview="handlePreview" @remove="handleRemove">
            <div v-if="fileList.length < 1">
              <PlusOutlined />
              <div style="margin-top: 8px">上传</div>
            </div>

            <!-- ✅ Antd 内置预览弹窗 -->
            <a-modal :open="previewOpen" :closable="false" :footer="null" @cancel="previewOpen = false">
              <img :src="previewImage" style="width: 100%" />
            </a-modal>
          </a-upload>
        </a-form-item>
        <div style="margin-top: 10px; text-align: center; width: 100%">
          <a-button type="primary" @click="save" style="margin-right: 50px">保存</a-button>
          <a-button @click="exit">返回</a-button>
        </div>
      </a-form>
    </a-drawer>
  </div>
</template>
<script lang="ts" setup>
import { ref, h, watchEffect, watch, reactive, onMounted, createVNode, computed, nextTick, defineOptions } from 'vue'
import { saveApi, getListApi, deleteApi, getDetailApi } from '@/api/signature'
import Form from './Form.vue'
import Preview from './Preview.vue'
import { useGetOraganizeUserTreeAsync } from '@/api/system/organization'
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
  name: 'Signature'
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
  getUsers()
  getTableData()
})

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
        const res = DeleteApi(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

async function DeleteApi(resolve, reject, id) {
  const params = {
    id: id
  }
  await deleteApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getTableData(false)
    }
  })
}

const inputFunctionName = ref('Signature')
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData()') {
    getTableData()
  }
  if (functionName === 'resetBtn()') {
    resetBtn()
  }
  if (functionName === 'toggleAccordion()') {
    toggleAccordion()
  }
  if (functionName === 'showDrawer()') {
    console.info(123)
    showDrawer()
  }
}
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'Signature',
    roleids: inputRoleId,
    btnCodes: ['signature3', 'signature4']
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
  if (userStore.access_token == '') {
    return
  }
  if (qiankunWindow.__POWERED_BY_QIANKUN__) {
    return i18n.t(navigationStore.value.tabs.find((a) => a.key == drawerStore.value.selected[0]).i18nKey)
  } else {
    return i18n.t(navigationStore.tabs.find((a) => a.key == drawerStore.selected[0]).i18nKey)
  }
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

const name = ref('')
const userIds = ref('')
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
    align: 'center'
  },
  {
    title: '编码',
    dataIndex: 'code',
    key: 'code',
    align: 'center'
  },
  {
    title: '授权人',
    dataIndex: 'userIds',
    key: 'userIds',
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
    userIds: userIds.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getListApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      dataSource.value = res.data
      modalTotalCount.value = res.total
      if (!!showMes) {
        message.success('查询成功')
      }
    } else {
      message.warning(res.message)
    }
  })
}

function resetBtn() {
  name.value = ''
  userIds.value = ''
  getTableData()
}

//新增抽屉
const searchStr = ref()
const treeData = ref([])
// 搜索过滤后的数据
const filteredTreeData = computed(() => {
  if (!searchStr.value) {
    return treeData.value
  } else {
    return filterTree(treeData.value, searchStr.value)
  }
})

function handleSearch() {
  // 输入框回车或点击搜索按钮时触发
  console.log('搜索关键字：', searchStr.value)
}

// 树过滤函数
function filterTree(data, keyword) {
  return data
    .map((node) => {
      const match = node.fullName.includes(keyword)
      if (node.children) {
        const filteredChildren = filterTree(node.children, keyword)
        if (filteredChildren.length > 0 || match) {
          return { ...node, children: filteredChildren }
        }
      }
      return match ? { ...node, children: [] } : null
    })
    .filter(Boolean)
}
async function getUsers() {
  const data = await useGetOraganizeUserTreeAsync()
  if (data.code === 200 && data.success) {
    treeData.value = convertTree(data.data, '')
  }
}
function convertTree(data, userName) {
  if (!data) return []

  // 支持数组输入
  if (Array.isArray(data)) {
    return data.map((item) => convertTree(item, userName))
  }

  const converted = {
    value: data.id,
    label: data.name,
    category: 1,
    children: []
  }
  // 添加用户节点
  if (data.userTrees?.length) {
    converted.children.push(
      ...data.userTrees.map((user) => ({
        value: user.id,
        label: user.name,
        category: 2
      }))
    )
  }

  // 递归子组织
  if (data.oraganizeUserTrees?.length) {
    converted.children.push(...data.oraganizeUserTrees.map((or) => convertTree(or, userName)))
  }

  return converted
}

const signatureId = ref('')
const open = ref(false)
//打开抽屉
const showDrawer = (id) => {
  open.value = true
  drawerTitle.value = '新增'
  formState.signatureId = ''
  formState.name = ''
  formState.code = ''
  formState.userIdList = []
  formState.img = ''
  fileList.value=[]
}
async function detail(id) {
  open.value = true
  drawerTitle.value = '详情'
  formState.signatureId = id
  formState.name = ''
  formState.code = ''
  formState.userIdList = []
  formState.img = ''
  fileList.value=[]
  const params = {
    id: id
  }
  const response = await getDetailApi(params)
  if (response.code === 200 && response.success) {
    formState.signatureId = response.data.signatureId
    formState.name = response.data.name
    formState.code = response.data.code
    formState.userIds = response.data.userIds
    formState.userIdList = response.data.userIds.split(',')
    formState.img = response.data.img
    fileList.value = [
      {
        uid: '-1',
        name: 'image.png',
        status: 'done',
        url: formState.img
      }
    ]
  }
}
const drawerTitle = ref('')
const formState = reactive({
  signatureId: '',
  name: '',
  code: '',
  userIds: '',
  userIdList: [],
  img: ''
})
const formRef = ref()
const labelCol = {
  style: {
    width: '100px'
  }
}
const rules = {
  name: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  code: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  userIds: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ]
}
const save = async () => {
  formRef.value.validate().then(async () => {
    formState.userIds = formState.userIdList.join(',')
    console.info(formState)
    const response = await saveApi(formState)
    if (response.code === 200 && response.success) {
      message.success('保存成功')
      getTableData(false)
      open.value = false
    } else if (response.code === 200 && !response.success) {
      message.warning(response.message)
    }
  })
}

const fileList = ref([])
const previewOpen = ref(false)
const previewImage = ref('')

// base64 转换
const getBase64 = (file) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.readAsDataURL(file)
    reader.onload = () => resolve(reader.result)
    reader.onerror = (error) => reject(error)
  })

// 上传前：转换为 base64，并阻止默认上传
const beforeUpload = async (file) => {
  const base64 = await getBase64(file)

  // 保存到 form
  formState.img = base64

  // Upload 回显
  fileList.value = [
    {
      uid: file.uid,
      name: file.name,
      status: 'done',
      url: base64
    }
  ]

  return false // 不上传到服务器
}

// 点击预览（默认弹窗）
const handlePreview = async (file) => {
  previewImage.value = file.url
  previewOpen.value = true
}

// 删除图片
const handleRemove = () => {
  formState.img = ''
  fileList.value = []
}

//退出子项抽屉
function exit() {
  open.value = false

  getTableData(false)
}
</script>

<style lang="scss">
.Signature {
  display: flex;
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
  }
  .right {
    width: 100%;
    height: 100%;
  }

  .avatar-uploader > .ant-upload {
    width: 128px;
    height: 128px;
  }
  .ant-upload-select-picture-card i {
    font-size: 32px;
    color: #999;
  }

  .ant-upload-select-picture-card .ant-upload-text {
    margin-top: 8px;
    color: #666;
  }
}
</style>
