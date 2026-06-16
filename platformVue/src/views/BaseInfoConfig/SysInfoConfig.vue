<template>
  <div class="SysInfoConfig">
    <div class="leftModel">
      <div class="configTitle">
        <p>平台信息配置</p>
      </div>
      <a-form  :rules="ptRules" ref="formRef" :model="formState" :label-col="labelCol">
        <a-form-item label="平台名称" name="name">
          <a-input v-model:value="formState.name" style="width: 200px" />
        </a-form-item>
        <a-form-item label="IP地址" name="ipAddress">
          <a-input v-model:value="formState.ipAddress" style="width: 200px" />
        </a-form-item>
        <a-form-item label="平台版本号">
          <a-input v-model:value="formState.version" style="width: 200px" />
        </a-form-item>
        <a-form-item label="系统备注">
          <a-input v-model:value="formState.desc" style="width: 200px" />
        </a-form-item>
        <a-form-item label="平台备注">
          <a-input v-model:value="formState.remark" style="width: 200px" />
        </a-form-item>
        <a-form-item label="平台logo">
          <a-upload :before-upload="beforeUpload" :showUploadList="false" maxCount="1">
            <a-button :icon="h(UploadOutlined)"></a-button>
          </a-upload>
        </a-form-item>
        <div style="margin-top: 10px; text-align: center; width: 100%">
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010606']"
            @buttonClick="handleButtonClick"
          ></CustomButtonList>
          <!-- <a-button type="primary" @click="saveBaseInfoConfig" style="margin-right: 50px">保存</a-button> -->
        </div>
      </a-form>
    </div>
    <div class="rightModel">
      <div style="height: 40px; line-height: 40px; text-align: right">
        <div class="headerLeft">
          <p>系统服务信息</p>
        </div>
        <div class="headerRight">
          <a-input style="width: 150px; margin-right: 5px" v-model:value="sysName" placeholder="请输入系统名称" />
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010605', 'A010604', 'A010602']"
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
        :scroll="{ x: 3000, y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
        :expand-column-width="100"
      >
        <template #bodyCell="{ text, column, record }">
          <!-- 处理状态列 -->

          <!-- 操作按钮 -->
          <template v-if="column.dataIndex === 'operation'">
            <a @click="detail(record.sysInfoId)" v-if="allDyBtn.find((item) => item.functionCode === 'A010601')">
              <CustomButtonListTable
                :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010601')"
                :IsOnlyIcon="true"
                style="display: inline; margin-right: 10px"
              ></CustomButtonListTable>
            </a>
            <a @click="onDelete(record.sysInfoId)" v-if="allDyBtn.find((item) => item.functionCode === 'A010603')">
              <CustomButtonListTable
                :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010603')"
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
  </div>

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
      <a-form-item label="系统名称" name="subSysName">
        <a-input v-model:value="itemFormState.subSysName"  />
      </a-form-item>
      <!-- <a-form-item label="系统英文名称">
        <a-input v-model:value="itemFormState.subSysEnglishName"  />
      </a-form-item> -->
      <a-form-item label="系统图标地址">
        <!-- <a-input v-model:value="itemFormState.subSysIcon" style="width: 200px" /> -->
        <div style="display: flex">
          <a-button style="width: 100%;" @click="openModal">选择Icon</a-button>
          <component
            v-if="selectedIcon"
            :is="selectedIconComponent"
            style="
              font-size: 30px;

              margin-left: 5px;
              color: #1890ff;
            "
          />
        </div>
      </a-form-item>
      <a-form-item label="系统访问地址" name="subSysAccessUrl">
        <a-input v-model:value="itemFormState.subSysAccessUrl"  />
      </a-form-item>
      <a-form-item label="系统默认首页" >
        <a-input v-model:value="itemFormState.sysDefaultIndex"  />
      </a-form-item>
      <a-form-item label="系统描述">
        <a-input v-model:value="itemFormState.describe"  />
      </a-form-item>
      <a-form-item label="系统备注">
        <a-input v-model:value="itemFormState.subSysRemark"  />
      </a-form-item>
      <a-form-item label="系统排序" name="sort">
        <a-input-number v-model:value="itemFormState.sort" :min="1" style="width: 100%" />
      </a-form-item>
      <a-form-item label="系统归属">
        <a-input v-model:value="itemFormState.belonge" />
      </a-form-item>
      <a-form-item label="ids4客户端ID">
        <a-input v-model:value="itemFormState.clientTableID"  />
      </a-form-item>
      <a-form-item label="IP地址">
        <a-input v-model:value="itemFormState.ipAddress"  />
      </a-form-item>
      <a-form-item label="版本号">
        <a-input v-model:value="itemFormState.version"  />
      </a-form-item>
      <a-form-item label="系统编码" name="sysCode">
        <a-input v-model:value="itemFormState.sysCode"  />
      </a-form-item>
      <a-form-item label="父级系统" name="sysInfoId">
        <a-select  ref="select" v-model:value="itemFormState.pid" :options="options" />
      </a-form-item>
      <a-form-item label="系统类型" name="type">
        <a-radio-group v-model:value="itemFormState.type" name="type">
          <a-radio :value=0>PC端</a-radio>
          <a-radio :value=1>移动端</a-radio>
        </a-radio-group>
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button class="NewAddBtnBG" type="primary" @click="saveItem" style="margin-right: 50px">保存</a-button>
        <a-button @click="exitItem">返回</a-button>
      </div>
    </a-form>
  </a-drawer>

  <!-- 图标选择的弹框 -->
  <a-modal :open="visible" @update:open="handleModalVisibleChange" title="选择一个图标" :footer="null" style="width: 800px">
    <!-- 图标选择区域 -->
    <div style="max-height: 500px; overflow-y: auto">
      <a-row style="width: 100%">
        <a-col
          v-for="icon in filteredIconList"
          :key="icon.name"
          :span="6"
          @click="selectIcon(icon.name)"
          style="text-align: center; cursor: pointer; margin-bottom: 10px"
        >
          <component :is="icon.component" style="font-size: 24px; color: #1890ff" />
          <div>{{ icon.name }}</div>
        </a-col>
      </a-row>
    </div>
  </a-modal>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ref, onMounted, h, reactive, createVNode, watchEffect, computed } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import * as Icons from '@ant-design/icons-vue'
import {
  getPtInfoConfigDetailApi,
  savePtInfoConfigApi,
  addSysInfoApi,
  updateSysInfoApi,
  deletionSysInfoApi,
  getSysInfoAsyncApi,
  getSysInfoByIdAsyncApi,
  uploadApi,
  getAllSysInfoTreeApi
} from '@/api/sysinfo'
import { ExclamationCircleOutlined, UploadOutlined } from '@ant-design/icons-vue'

import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
/* 动态按钮相关 */
import { useUserStore } from '@/store/user'
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
  name: 'A0106'
})
const inputFunctionName = ref('A0106')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0106',
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
  if (functionName === 'search') {
    search()
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'showDrawer') {
    showDrawer()
  }
  if (functionName === 'saveBaseInfoConfig') {
    saveBaseInfoConfig()
  }
}
onMounted(() => {
  getAllSysInfo(current.value, pageSize.value, sysName.value)
  getPtInfoConfigDetail()
  getAllButton()
})
const myTable = ref(null)

//获取平台信息配置详情
async function getPtInfoConfigDetail() {
  ;(formState.baseInfoConfigId = ''),
    (formState.name = ''),
    (formState.ipAddress = ''),
    (formState.version = ''),
    (formState.desc = ''),
    (formState.remark = ''),
    (formState.logo = '')
  const response = await getPtInfoConfigDetailApi()
  if (response.code === 200 && response.success) {
    if (response.data.baseInfoConfigId !== null) {
      ;(formState.baseInfoConfigId = response.data.baseInfoConfigId),
        (formState.name = response.data.name),
        (formState.ipAddress = response.data.ipAddress),
        (formState.version = response.data.version),
        (formState.desc = response.data.desc),
        (formState.remark = response.data.remark),
        (formState.logo = response.data.logo)
    }
  }
}
//保存平台信息配置
const saveBaseInfoConfig = async () => {
  formState.logo = logoUrl.value
  const response = await savePtInfoConfigApi(formState)
  if (response.code === 200 && response.success) {
    message.success('保存成功')

    getPtInfoConfigDetail()
  }
}
const formState = reactive({
  baseInfoConfigId: '',
  name: '',
  sort: '',
  ipAddress: '',
  version: '',
  desc: '',
  remark: '',
  logo: ''
})
const formRef = ref()
const labelCol = {
  style: {
    width: '120px'
  }
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAllSysInfo(current, pageSize, sysName.value)
}
const pageSizeChange = (page, pageSize) => {
  getAllSysInfo(page, pageSize, sysName.value)
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
    title: '系统名称',
    dataIndex: 'subSysName',
    key: 'subSysName',
    fixed: 'left',
    align: 'center'
  },
  // {
  //   title: '系统英文名称',
  //   dataIndex: 'subSysEnglishName',
  //   key: 'subSysEnglishName',
  //   fixed: 'left',
  //   align: 'center'
  // },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort',
    //width: 130,
    align: 'center'
  },
  {
    title: '系统图标地址',
    dataIndex: 'subSysIcon',
    key: 'subSysIcon',
    //width: 130,
    align: 'center'
  },
  {
    title: '系统访问地址',
    dataIndex: 'subSysAccessUrl',
    key: 'subSysAccessUrl',
    //width: 150,
    align: 'center'
  },
  {
    title: '系统默认首页',
    dataIndex: 'sysDefaultIndex',
    key: 'sysDefaultIndex',
    //width: 180,
    align: 'center'
  },
  {
    title: '系统描述',
    dataIndex: 'describe',
    key: 'describe',
    //width: 80,
    align: 'center'
  },
  {
    title: '系统备注',
    dataIndex: 'subSysRemark',
    key: 'subSysRemark',
    //width: 80,
    align: 'center'
  },
  {
    title: '系统归属',
    dataIndex: 'belonge',
    key: 'belonge',
    //width: 80,
    align: 'center'
  },
  {
    title: 'ids4客户端表ID',
    dataIndex: 'clientTableID',
    key: 'clientTableID',
    //width: 80,
    align: 'center'
  },
  {
    title: 'IP地址',
    dataIndex: 'ipAddress',
    key: 'ipAddress',
    //width: 80,
    align: 'center'
  },
  {
    title: '版本号',
    dataIndex: 'version',
    key: 'version',
    //width: 80,
    align: 'center'
  },
  {
    title: '系统编码',
    dataIndex: 'sysCode',
    key: 'sysCode',
    //width: 80,
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    //width: 180,
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


//查询系统名称
const options = ref()
async function getAllSysName() {
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


//新增抽屉
const open = ref(false)
//打开抽屉
const showDrawer = () => {
  open.value = true
  sysInfoId.value = ''
  getAllSysName()
  drawerTitle.value = '新增'
  itemFormState.sysInfoId = ''
  itemFormState.subSysName = ''
  itemFormState.subSysEnglishName = ''
  itemFormState.subSysIcon = ''
  itemFormState.subSysAccessUrl = ''
  itemFormState.sysDefaultIndex = ''
  itemFormState.describe = ''
  itemFormState.sort = ''
  itemFormState.subSysRemark = ''
  itemFormState.belonge = ''
  itemFormState.clientTableID = ''
  itemFormState.ipAddress = ''
  itemFormState.version = ''
  itemFormState.sysCode = ''
  itemFormState.pid = ''
  itemFormState.type = 0
}
const drawerTitle = ref('')
const itemFormState = reactive({
  sysInfoId: '',
  subSysName: '',
  subSysEnglishName: '',
  subSysIcon: '',
  subSysAccessUrl: '',
  sysDefaultIndex: '',
  describe: '',
  sort: '',
  subSysRemark: '',
  belonge: '',
  clientTableID: '',
  ipAddress: '',
  version: '',
  sysCode: '',
  pid:'',
  type:0
})
const itemFormRef = ref()
const itemLabelCol = {
  style: {
    width: '100px'
  }
}
//退出子项抽屉
function exitItem() {
  open.value = false
  sysInfoId.value = ''

  getAllSysInfo(current.value, pageSize.value, sysName.value)
}
//获取系统信息列表
const sysName = ref()
async function getAllSysInfo(page, size, sysName) {
  const query = {
    pageIndex: page,
    pageSize: size,
    name: sysName
  }
  const response = await getSysInfoAsyncApi(query)
  console.info(response)
  if (response.code === 200 && response.success) {
    data.value = response.data
    total.value = response.total
  }
}
const validate = async (_rule, value) => {
  if (value.trim() === '') {
    return Promise.reject('这是必填项')
  } else {
    return Promise.resolve()
  }
}
//平台配置规则校验

const ptRules = {
  name: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  ipAddress: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ]
}
const rules = {
  subSysName: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  subSysAccessUrl: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  sysDefaultIndex: [
    {
      required: true,
      validator: validate,
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
  sysCode: [
  {
    required: true,
    validator: (rule, value, callback) => {
      if (!value) {
        callback(new Error('这是必填项'))
      } else if (value.includes('_')) {
        callback(new Error('不能包含下划线(_)'))
      } else {
        callback()
      }
    },
    trigger: 'change'
  }
]
}
const sysInfoId = ref()
//保存系统信息
const saveItem = async () => {
  itemFormRef.value.validate().then(async () => {
    if (!sysInfoId.value) {
      const response = await addSysInfoApi(itemFormState)
      if (response.code === 200 && response.success) {
        message.success('添加成功')
        getAllSysInfo(current.value, pageSize.value, sysName.value)
        open.value = false
      }
    } else {
      const response = await updateSysInfoApi(itemFormState)
      if (response.code === 200 && response.success) {
        message.success('修改成功')
        getAllSysInfo(current.value, pageSize.value, sysName.value)
        open.value = false
      }
    }
  })
}

//详情
async function detail(id) {
  open.value = true
  getAllSysName()
  drawerTitle.value = '详情'
  itemFormState.sysInfoId = ''
  itemFormState.subSysName = ''
  itemFormState.subSysEnglishName = ''
  itemFormState.subSysIcon = ''
  itemFormState.subSysAccessUrl = ''
  itemFormState.sysDefaultIndex = ''
  itemFormState.describe = ''
  itemFormState.sort = ''
  itemFormState.subSysRemark = ''
  itemFormState.belonge = ''
  itemFormState.clientTableID = ''
  itemFormState.ipAddress = ''
  itemFormState.version = ''
  itemFormState.sysCode = ''
  itemFormState.pid = ''
  itemFormState.type = 0
  const params = {
    id: id
  }
  const response = await getSysInfoByIdAsyncApi(params)
  if (response.code === 200 && response.success) {
    sysInfoId.value = id
    itemFormState.sysInfoId = response.data.sysInfoId
    itemFormState.subSysName = response.data.subSysName
    itemFormState.subSysEnglishName = response.data.subSysEnglishName
    itemFormState.subSysIcon = response.data.subSysIcon
    selectedIcon.value = response.data.subSysIcon
    itemFormState.subSysAccessUrl = response.data.subSysAccessUrl
    itemFormState.sysDefaultIndex = response.data.sysDefaultIndex
    itemFormState.describe = response.data.describe
    itemFormState.subSysRemark = response.data.subSysRemark
    itemFormState.sort = response.data.sort
    itemFormState.belonge = response.data.belonge
    itemFormState.clientTableID = response.data.clientTableID
    itemFormState.ipAddress = response.data.ipAddress
    itemFormState.version = response.data.version
    itemFormState.sysCode = response.data.sysCode
    itemFormState.pid = response.data.pid
    itemFormState.type = response.data.type
  }
}

//查询
function search() {
  getAllSysInfo(current.value, pageSize.value, sysName.value)
}
//重置查询
function resetSearch() {
  sysName.value = ''
  getAllSysInfo(current.value, pageSize.value, sysName.value)
}
//删除
async function del(resolve, reject, id) {
  const params = {
    id: id
  }
  await deletionSysInfoApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getAllSysInfo(current.value, pageSize.value, sysName.value)
    }else if(res.code === 200 && !res.success){
      
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

const logoUrl = ref()
async function beforeUpload(file) {
  const formData = new FormData()
  formData.append('file', file)
  console.info(formData)
  const response = await uploadApi(formData)
  logoUrl.value = response.message
  return false
}

//icon下拉框

// 弹框和选中的图标
const visible = ref(false)
const selectedIcon = ref(null)

// 获取所有图标并生成图标数组
const iconList = computed(() =>
  Object.keys(Icons).map((iconName) => ({
    name: iconName,
    component: Icons[iconName]
  }))
)
// 过滤掉 name 首字母为小写的图标
const filteredIconList = computed(
  () => iconList.value.filter((icon) => /^[A-Z]/.test(icon.name)) // 检查 name 的首字母是否是大写
)
// 动态加载选中的图标组件
const selectedIconComponent = computed(() => {
  return Icons[selectedIcon.value] || null
})

// 打开弹窗
const openModal = () => {
  visible.value = true
}

// 关闭弹窗
const handleModalVisibleChange = (newVisible) => {
  visible.value = newVisible
}

// 选择图标并关闭弹框
function selectIcon(iconName) {
  selectedIcon.value = iconName
  visible.value = false
  itemFormState.subSysIcon = iconName
}
</script>

<style lang="scss">
.SysInfoConfig {
  height: 100%;
  position: relative;
  display: flex;

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
  .leftModel {
    width: 20%;
    border-right: 1px solid gainsboro;
    background-color: white;
  }

  .rightModel {
    width: 80%;
    background-color: white;
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
    margin-top: 5px;
    height: 35px;
    line-height: 35px;
    display: flex;
    align-items: center;
    justify-content: flex-end;
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

  .configTitle {
    height: 30px;
    display: flex;
    align-items: center;
    margin-bottom: 20px;
  }

  .configTitle > p {
    font-size: 16px;
    margin: 0;
    padding-left: 10px;
    font-weight: 600;
    color: black;
  }
}

// .header {
//   height: 6%;
//   background-color: white !important;
//   align-items: center;
//   padding: 0px;
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
