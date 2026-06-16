<template>
  <div class="Interface">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>接口信息配置</p>
      </div>
      <div class="headerRight">
        <a-input style="width: 150px; margin-right: 5px" v-model:value="name" placeholder="请输入接口名称" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A010504', 'A010503', 'A010507', 'A010506', 'A010511']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>
        <a-upload :before-upload="tableImport" :showUploadList="false" maxCount="1">
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010509']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
        </a-upload>
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A010501', 'A010505']"
          @buttonClick="handleButtonClick"
        ></CustomButtonList>
      </div>
    </div>
    <a-table
      :pagination="false"
      size="large"
      :columns="columns"
      :data-source="data"
      bordered
      ref="myTable"
      :scroll="{ x: 2000, y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
      :expand-column-width="100"
      :expandedRowKeys="expandedRowKeys"
      @expandedRowsChange="expandedRowsChange"
      :expand-icon-column-index="4"
      :row-selection="rowSelection"
    >
      <template #expandIcon="props">
        <span v-if="props.record.children && props.record.children.length > 0">
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
      <template #bodyCell="{ text, column, record }">
        <!-- 处理状态列 -->

        <template v-if="column.dataIndex === 'isActive'">
          <span v-if="text === true"> 是 </span>
          <span v-else> 否 </span>
        </template>
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="showDrawer(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010508')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010508')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="detail(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010510')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010510')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010502')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010502')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
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

  <!-- 模态框 -->
  <a-modal v-model:open="importOpen" :title="导入" centered :keyboard="false" :maskClosable="false" width="30%" :footer="null">
    <a-form ref="formRef" :model="formState" :label-col="labelCol">
      <a-form-item label="父级名称" name="name">
        <a-input disabled v-model:value="formState.name" style="width: 300px" />
      </a-form-item>
      <a-form-item label="父级编码" name="code">
        <a-input disabled v-model:value="formState.code" style="width: 300px" />
      </a-form-item>
      <a-form-item label="接口路径">
        <a-input-search v-model:value="formState.swaggerAddress" style="width: 300px" @search="onSearch" />
      </a-form-item>
      <a-form-item label="所属系统">
        <a-input disabled v-model:value="formState.sysInfoId" style="width: 300px" />
      </a-form-item>
      <a-form-item label="起始序号">
        <a-input-number v-model:value="formState.sort" style="width: 300px" />
      </a-form-item>
      <a-form-item label="控制器列表">
        <div>
          <a-checkbox v-model:checked="state.checkAll" :indeterminate="state.indeterminate" @change="onCheckAllChange"> 全选 </a-checkbox>
        </div>
        <a-checkbox-group v-model:value="formState.controller" :options="plainOptions" style="width: 300px" />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button class="NewAddBtnBG" type="primary" @click="save" style="margin-right: 50px">导入</a-button>
        <a-button @click="exit">返回</a-button>
      </div>
    </a-form>
  </a-modal>

  <!-- 抽屉 -->
  <a-drawer
    v-model:open="open"
    :title="drawerTitle"
    class="custom-class"
    root-class-name="root-class-name"
    :root-style="{ color: 'blue' }"
    style="color: red"
    size = 'large'
    placement="right"
  >
    <a-form ref="itemFormRef" :model="itemFormState" :label-col="itemLabelCol" :rules="rules">
      <a-form-item label="编码" name="code">
        <a-input v-model:value="itemFormState.code"  />
      </a-form-item>
      <a-form-item label="接口名称" name="name">
        <a-input v-model:value="itemFormState.name"  />
      </a-form-item>
      <a-form-item label="备注">
        <a-input v-model:value="itemFormState.remark"  />
      </a-form-item>
      <a-form-item label="接口地址" name="linkUrl">
        <a-input v-model:value="itemFormState.linkUrl"  />
      </a-form-item>
      <a-form-item label="区域名称">
        <a-input v-model:value="itemFormState.areaName"  />
      </a-form-item>
      <a-form-item label="所属系统" name="sysInfoId">
        <a-select  ref="select" v-model:value="itemFormState.sysInfoId" :options="options" />
      </a-form-item>
      <a-form-item label="排序" name="sort">
        <a-input-number :min="0" v-model:value="itemFormState.sort" style="width: 100%" />
      </a-form-item>
      <a-form-item label="是否激活" name="isActive">
        <a-radio-group v-model:value="itemFormState.isActive" name="isActive">
          <a-radio :value="true">是</a-radio>
          <a-radio :value="false">否</a-radio>
        </a-radio-group>
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button type="primary" @click="saveItem" style="margin-right: 50px">保存</a-button>
        <a-button @click="exitItem">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ref, onMounted, h, reactive, watch, createVNode, watchEffect } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import axios from 'axios'
import { message, Modal } from 'ant-design-vue'
import {
  getListAsyncApi,
  getAllSysInfoTreeApi,
  addInterfaceInfoApi,
  updateInterfaceInfoApi,
  getDetailByIdAsyncApi,
  deleteApi,
  getSwaggerInfoApi,
  importInterfaceApi,
  tableExportApi,
  tableImportApi
} from '@/api/interface'
import { ExclamationCircleOutlined, CaretDownOutlined, CaretRightOutlined } from '@ant-design/icons-vue'

/* 动态按钮相关 */
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
  name: 'A0105'
})
const inputFunctionName = ref('A0105')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0105',
    roleids: inputRoleId,
    btnCodes: []
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
    }
  })
}
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'tableExport') {
    tableExport()
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'importSwagger') {
    importSwagger()
  }
  if (functionName === 'expand') {
    expand()
  }
  if (functionName === 'expandClose') {
    expandClose()
  }
  if (functionName === 'search') {
    search()
  }
  if (functionName === 'showDrawer') {
    showDrawer()
  }
}

onMounted(() => {
  getAllInterface(current.value, pageSize.value, name.value)
  getAllButton()
})
const myTable = ref(null)

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAllInterface(current, pageSize, name.value)
}
const pageSizeChange = (page, pageSize) => {
  getAllInterface(page, pageSize, name.value)
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
    title: '编码',
    dataIndex: 'code',
    key: 'code',
    width: 100,
    fixed: 'left',
    align: 'center'
  },
  {
    title: '接口名称',
    dataIndex: 'name',
    key: 'name',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '操作动作',
    dataIndex: 'action',
    key: 'action',
    align: 'center'
  },
  {
    title: '接口地址',
    dataIndex: 'linkUrl',
    key: 'linkUrl',
    //width: 150,
    align: 'center'
  },
  // {
  //   title: '区域名称',
  //   dataIndex: 'areaName',
  //   key: 'areaName',
  //   //width: 180,
  //   align: 'center'
  // },
  {
    title: '所属系统',
    dataIndex: 'sysInfoID',
    key: 'sysInfoID',
    //width: 80,
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
    title: '是否激活',
    dataIndex: 'isActive',
    key: 'isActive',
    width: 100,
    align: 'center'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    //width: 130,
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    width: 120,
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
    width: 160,
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    width: 160,
    dataIndex: 'operation',
    fixed: 'right',
    align: 'center'
  }
]
const data = ref(null)

//新增抽屉
const open = ref(false)
//打开抽屉
const showDrawer = (id) => {
  console.info(id)
  getAllSysInfo()
  open.value = true
  drawerTitle.value = '新增'
  itemFormState.id = ''
  itemFormState.pid = id
  itemFormState.code = ''
  itemFormState.name = ''
  itemFormState.remark = ''
  itemFormState.linkUrl = ''
  itemFormState.areaName = ''
  itemFormState.sysInfoId = ''
  itemFormState.sort = ''
  itemFormState.isActive = ''

  interfaceId.value = ''
}
const drawerTitle = ref('')
const itemFormState = reactive({
  id: '',
  pid: '',
  code: '',
  action: '',
  name: '',
  remark: '',
  linkUrl: '',
  areaName: '',
  sysInfoId: '',
  sort: '',
  isActive: ''
})
const itemFormRef = ref()
const itemLabelCol = {
  style: {
    width: '100px'
  }
}
//查询系统名称
async function getAllSysInfo() {
  options.value = []
  const response = await getAllSysInfoTreeApi()
  if (response.code === 200 && response.success) {
    response.data.forEach((c) => {
      options.value.push({
        value: c.sysInfoId,
        label: c.subSysName
      })
    })
  }
}

const options = ref()
//退出子项抽屉
function exitItem() {
  open.value = false

  getAllInterface(current.value, pageSize.value, name.value)
}
//获取接口信息列表
const name = ref()
async function getAllInterface(page, size, name) {
  const query = {
    pageIndex: page,
    pageSize: size,
    name: name
  }
  const response = await getListAsyncApi(query)
  console.info('接口数据：',response)
  if (response.code === 200 && response.success) {
    data.value = response.data
    total.value = response.total
    dg(data.value)
  }
}
function dg(obj) {
  obj.forEach((e) => {
    e.key = e.id
    if (e.children !== null) {
      dg(e.children)
    }
  })
}
const interfaceId = ref(null)
//保存接口信息
const validate = async (_rule, value) => {
  if (value.trim() === '') {
    return Promise.reject('这是必填项')
  } else {
    return Promise.resolve()
  }
}
const rules = {
  code: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  name: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  linkUrl: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  sysInfoId: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  sort: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  isActive: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ]
}
const saveItem = async () => {
  itemFormRef.value.validate().then(async () => {
    if (!interfaceId.value || interfaceId.value == '') {
      const response = await addInterfaceInfoApi(itemFormState)
      if (response.code === 200 && response.success) {
        message.success('添加成功')
        getAllInterface(current.value, pageSize.value, name.value)
        open.value = false
      }
    } else {
      const response = await updateInterfaceInfoApi(itemFormState)
      if (response.code === 200 && response.success) {
        message.success('修改成功')
        getAllInterface(current.value, pageSize.value, name.value)
        open.value = false
      } else {
        message.warning(response.message)
      }
    }
  })
}

//详情
async function detail(id) {
  getAllSysInfo()
  open.value = true
  drawerTitle.value = '详情'
  itemFormState.id = ''
  itemFormState.pid = ''
  itemFormState.code = ''
  itemFormState.name = ''
  itemFormState.remark = ''
  itemFormState.linkUrl = ''
  itemFormState.action = ''
  itemFormState.areaName = ''
  itemFormState.sysInfoId = ''
  itemFormState.sort = ''
  itemFormState.isActive = ''

  interfaceId.value = ''
  const params = {
    id: id
  }
  const response = await getDetailByIdAsyncApi(params)
  if (response.code === 200 && response.success) {
    interfaceId.value = id
    itemFormState.id = response.data.id
    itemFormState.pid = response.data.pid
    itemFormState.code = response.data.code
    itemFormState.name = response.data.name
    itemFormState.action = response.data.action
    itemFormState.remark = response.data.remark
    itemFormState.linkUrl = response.data.linkUrl
    itemFormState.areaName = response.data.areaName
    itemFormState.sysInfoId = response.data.sysInfoID
    itemFormState.sort = response.data.sort
    itemFormState.isActive = response.data.isActive
  }
}
//模态框
const formState = reactive({
  pid: '',
  name: '',
  code: '',
  swaggerAddress: '',
  sysInfoId: '',
  sort: '',
  controller: []
})
const formRef = ref()
const labelCol = {
  style: {
    width: '150px'
  }
}
//swagger导入
const importOpen = ref(false)
const importSwagger = async () => {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要导入数据的父节点！')
    return
  }
  state.indeterminate = false
  state.checkAll = false
  importOpen.value = true
  formState.pid = rowSelection.value.selectedRow[0].id
  formState.name = rowSelection.value.selectedRow[0].name
  formState.code = rowSelection.value.selectedRow[0].code
  formState.swaggerAddress = ''
  formState.sysInfoId = rowSelection.value.selectedRow[0].sysInfoID
  formState.sort = 0
  formState.controller = []
  plainOptions.value = []
}
//表格导出
const tableExport = async () => {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要导出数据的父节点！')
    return
  }
  const params = {
    id: rowSelection.value.selectedRow[0].id
  }
  const response = await tableExportApi(params)
  // console.info(response)
  if (response.status === 200) {
    // 创建一个临时链接并下载文件
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'example.txt')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  } else if (response.code === 200 && !response.success) {
    message.error(response.statusText)
  }
}

//表格导入
async function tableImport(file) {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要导出数据的父节点！')
    return
  }
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('id', rowSelection.value.selectedRow[0].id)
    const response = await tableImportApi(formData)
    console.info('导入接口：', response)
    if (response.status === 200 && response.statusText === 'OK') {
      message.success('导入成功')
      getAllInterface(current.value, pageSize.value, name.value)
    }
    return false
  } catch (error) {
    console.info(error)
      message.error(`导入失败`)
    return false
  }
}

const onSearch = async (searchValue) => {
  plainOptions.value = []
  const params = {
    swaggerAddress: searchValue
  }
  const response = await getSwaggerInfoApi(params)
  if (response.code === 200 && response.success) {
    response.data.forEach((c) => {
      plainOptions.value.push({
        value: c.tagName,
        label: c.name
      })
    })
  } else if (response.code === 200 && !response.success) {
    message.error(response.message)
  }
}
const plainOptions = ref([])
const state = reactive({
  indeterminate: false,
  checkAll: false
})
const onCheckAllChange = (e) => {
  Object.assign(state, {
    indeterminate: false
  })
  Object.assign(formState, {
    controller: e.target.checked ? plainOptions.value.map((x) => x.value) : []
  })
}

//导入
const importDto = reactive({
  pid: '',
  sort: '',
  swaggerAddress: '',
  controllerNames: []
})
const save = async () => {
  // console.info(formState)
  importDto.pid = formState.pid
  importDto.sort = formState.sort
  importDto.swaggerAddress = formState.swaggerAddress
  importDto.controllerNames = formState.controller
  // console.info(importDto)
  const response = await importInterfaceApi(importDto)
  if (response.code === 200 && response.success) {
    message.success('导入成功')
    getAllInterface(current.value, pageSize.value, name.value)
    importOpen.value = false
  }
}
//退出模态框
function exit() {
  importOpen.value = false
}

const rowSelection = ref({
  hideSelectAll: true,
  type: 'checkbox',
  selectedRowKeys: [],
  selectedRow: '',
  onChange: (selectedRowKeys, selectedRows) => {
    rowSelection.value.selectedRowKeys = selectedRowKeys.slice(-1)
    rowSelection.value.selectedRow = selectedRows.slice(-1)
  }
})
const expandedRowKeys = ref([])
// 全部展开
const expand = () => {
  dg2(data.value)
}
function dg2(obj) {
  obj.forEach((e) => {
    if (e.children !== null) {
      expandedRowKeys.value.push(e.id)
      dg2(e.children)
    }
  })
}
// 全部折叠
const expandClose = () => {
  expandedRowKeys.value = []
}
// 表格行操作（展开折叠）
const expandedRowsChange = (expandedRows) => {
  expandedRowKeys.value = expandedRows
}
//查询
function search() {
  getAllInterface(current.value, pageSize.value, name.value)
}
//重置查询
function resetSearch() {
  name.value = ''
  getAllInterface(current.value, pageSize.value, name.value)
}
//删除
async function del(resolve, reject, id) {
  const params = {
    id: id
  }
  await deleteApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getAllInterface(current.value, pageSize.value, name.value)
    } else {
      console.info('这里要弹框')
      message.warning(res.message)
    }
  })
}
function onDelete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除吗？',
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
</script>

<style lang="scss">
.Interface {
  height: 100%;
  position: relative;

  .ant-table-container {
    width: 100%;
  }
  .ant-table-body {
    overflow-y: scroll !important;
    height: calc(100vh - 96px - 40px - 40px - 35px);
  }
  .ant-table-content {
    height: calc(100vh - 96px - 40px - 40px - 35px);
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
    margin-left: 10px;
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

// .model {
//   height: 99.3%;
//   display: flex;
// }

// .leftModel {
//   width: 20%;
//   border-right: 1px solid gainsboro;
//   background-color: white;
// }

// .rightModel {
//   width: 80%;
//   background-color: white;
// }

// .header {
//   height: 6%;
//   background-color: white !important;
//   align-items: center;
//   padding: 0px;
// }

// .headerLeft {
//   float: left;
//   height: 100%;
//   display: flex;
//   align-items: center;
// }

// .headerLeft > p {
//   font-size: 16px;
//   margin: 0;
//   padding-left: 10px;
//   font-weight: 600;
//   color: black;
// }

// .headerRight {
//   margin-top: 5px;
//   height: 35px;
//   line-height: 35px;
//   display: flex;
//   align-items: center;
//   justify-content: flex-end;
// }

// .headerRight > button {
//   margin-left: 10px;
// }

// .headerRight > input {
//   margin-left: 10px;
// }

// .content {
//   margin: 0px;
//   height: 87%;
//   padding: 0px;
//   background-color: white;
// }

// .foot {
//   height: 7%;
//   background-color: white;
//   padding: 0%;
//   align-items: center;
//   padding: 0px;
// }

// .footPagintaion {
//   float: right;
//   height: 100%;
//   display: flex;
//   align-items: center;
// }
</style>
