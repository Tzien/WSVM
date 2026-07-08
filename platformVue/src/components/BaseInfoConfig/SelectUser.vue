<template>
  <div class="right_user">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-row>
        <a-col :span="12"
          ><a-form-item label="登录账号" v-bind="validateInfos.loginName"> <a-input :disabled="isOneself" v-model:value="modelRef.loginName" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="姓名" v-bind="validateInfos.realName"> <a-input :disabled="isOneself" v-model:value="modelRef.realName" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12">
          <a-form-item label="登录别名" v-bind="validateInfos.alias"> <a-input :disabled="isOneself" v-model:value="modelRef.alias" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="编码" v-bind="validateInfos.code"> <a-input :disabled="isOneself" v-model:value="modelRef.code" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="身份证号" v-bind="validateInfos.idCard"> <a-input v-model:value="modelRef.idCard" /> </a-form-item
        ></a-col>
        <a-col :span="12">
          <a-form-item label="性别" v-bind="validateInfos.sex">
            <div>
              <a-radio-group v-model:value="modelRef.sex">
                <a-radio :value="0">男</a-radio>
                <a-radio :value="1">女</a-radio>
              </a-radio-group>
            </div>
          </a-form-item></a-col
        >
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="手机" v-bind="validateInfos.phone"> <a-input v-model:value="modelRef.phone" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="钉钉" v-bind="validateInfos.dingTalk"> <a-input v-model:value="modelRef.dingTalk" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12">
          <a-form-item label="微信号" v-bind="validateInfos.webChat"> <a-input v-model:value="modelRef.webChat" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="邮箱" v-bind="validateInfos.email"> <a-input v-model:value="modelRef.email" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="是否激活" v-bind="validateInfos.enabled">
            <div>
              <a-radio-group :disabled="isOneself" v-model:value="modelRef.enabled">
                <a-radio :value="true">正常</a-radio>
                <a-radio :value="false">冻结</a-radio>
              </a-radio-group>
            </div>
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="备注" v-bind="validateInfos.remark"> <a-input v-model:value="modelRef.remark" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="绑定微信" v-bind="validateInfos.wechatuserinput">
            <a-input :readonly="true" @click="user_wechat" v-model:value="treeTableObj.wechatuserinput" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="绑定钉钉" v-bind="validateInfos.dingtalkuserinput">
            <a-input :readonly="true" @click="user_dingtalk" v-model:value="treeTableObj.dingtalkuserinput" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="所属部门" v-bind="validateInfos.depts">
            <a-input :readonly="true" @click="user_depts" v-model:value="treeTableObj.deptinput" /> </a-form-item
        ></a-col>
        <!--  -->
        <a-col :span="12"
          ><a-form-item label="直属部门" v-bind="validateInfos.organization">
            <a-input :readonly="true" @click="user_oraganize" v-model:value="treeTableObj.organizationsinput" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <!--  -->
        <a-col :span="12"
          ><a-form-item label="所在角色" v-bind="validateInfos.roles">
            <a-input :readonly="true" @click="user_role" v-model:value="treeTableObj.rolesinput" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="管理部门" v-bind="validateInfos.managingOrganizations">
            <a-input :readonly="true" @click="user_managing" v-model:value="treeTableObj.managinginput" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="所在岗位" v-bind="validateInfos.posts">
            <a-input :readonly="true" @click="user_posts" v-model:value="treeTableObj.postsinput" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="排序" v-bind="validateInfos.sort"> <a-input v-model:value="modelRef.sort" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="管理岗位" v-bind="validateInfos.managingJobs">
            <a-input :readonly="true" @click="user_managingJobs" v-model:value="treeTableObj.managingJobsInput" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="管理角色" v-bind="validateInfos.managingRoles">
            <a-input :readonly="true" @click="user_managingRole" v-model:value="treeTableObj.managingRolesInput" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12"
          ><a-form-item label="所属上级" v-bind="validateInfos.superior">
            <a-input :readonly="true" @click="user_superior" v-model:value="treeTableObj.superiorInput" /> </a-form-item
        ></a-col>
        <a-col :span="12"
          ><a-form-item label="管理下级" v-bind="validateInfos.subordinate">
            <a-input :readonly="true" @click="user_subordinate" v-model:value="treeTableObj.subordinateInput" /> </a-form-item
        ></a-col>
      </a-row>
      <a-row>
        <a-col :span="12">
          <div>
            <a-form-item label="上传头像">
              <a-upload
                name="avatar"
                list-type="picture-card"
                class="avatar-uploader"
                :show-upload-list="true"
                v-model:file-list="LconList"
                :on-remove="handleRemove"
                :before-upload="beforeUpload"
              >
                <div>
                  <PlusOutlined />
                  <div class="ant-upload-text">上传</div>
                </div>
              </a-upload>
            </a-form-item>
          </div>
        </a-col>
        <a-col :span="12">
          <div>
            <a-form-item label="上传签名">
              <a-upload
                name="avatar"
                list-type="picture-card"
                class="avatar-uploader"
                :show-upload-list="true"
                :on-remove="handleSignRemove"
                v-model:file-list="SignList"
                :before-upload="beforeSignUpload"
              >
                <div>
                  <PlusOutlined />
                  <div class="ant-upload-text">上传</div>
                </div>
              </a-upload></a-form-item
            >
          </div>
        </a-col>
      </a-row>
    </a-form>
  </div>
  <div>
    <a-drawer class="drawer" size="large" :title="treeTableObj.title" :closable="false" :open="open" @close="onClose">
      <a-space align="center" style="margin-bottom: 16px">
        <!-- <template v-if="treeTableObj.roleisSelectAll">
          <a-button @click="rolecheckall(true)">全选</a-button>
          <a-button @click="rolecheckall(false)">取消全选</a-button></template
        > -->
        <a-input v-model:value="query" placeholder="请输入查询条件" />
        <a-button @click="condition(false)">查询</a-button>
        <a-button @click="condition(true)">重置查询</a-button>
      </a-space>
      <a-table :columns="columns" :data-source="treeTableData" :row-selection="oraganizerowSelection" :pagination="false" />
      <div class="fenye" v-if="drawerpage">
        <a-pagination
          v-model:current="pagination.current"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          :show-total="() => `共 ${pagination.total} 条数据`"
          :show-size-changer="pagination.showSizeChanger"
          :show-quick-jumper="pagination.showQuickJumper"
          :page-size-options="pagination.pageSizeOptions"
          @showSizeChange="handlePageSizeChange"
          @change="handlePageChange"
        >
        </a-pagination>
      </div>
    </a-drawer>
  </div>
</template>
<script setup>
import { ref, reactive, watch, onMounted, inject, watchEffect } from 'vue'
import { Form } from 'ant-design-vue'
import { message } from 'ant-design-vue'
import { ptUrl } from '@/utils/request'
import { useUserStore } from '../../store/index.js'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
import { PlusOutlined } from '@ant-design/icons-vue'
import { useGetPostsAsync, useGetOraganizeTreeAsync } from '@/api/BaseInfoConfig/organization'
import { useGetWeChatOraganizeUserTree, useGetDingTalkOraganizeUserTree } from '@/api/Msg/msg'
import { useGetRolesAsync } from '@/api/permission'
import {
  useAddUserAsync,
  useGetUsersByIdAsync,
  useUpdateUserAsync,
  useInitializePassword,
  useSoftDeletionUserAsync,
  useGetSuperiorAndSubordinateAsync,
  useGetUserGradeAsync,
  useInsertUserGradeAsync
} from '@/api/user'

const isOneself = ref(false)
var userStore = ref()
const { globalStore } = useGlobalState()
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
    }
  })
}

const useForm = Form.useForm
const labelCol = {
  style: {
    width: '100px'
  }
}
const wrapperCol = {
  span: 14
}
const modelRef = ref({
  id: '',
  idsUserId: '',
  loginName: '',
  realName: '',
  alias: '',
  code: '',
  idCard: '',
  sex: 0,
  phone: '',
  dingTalk: '',
  webChat: '',
  email: '',
  enabled: true,
  remark: '',
  weChatUserId: '', //微信用户id
  weChatUserName: '', //微信用户名
  dingTalkUserId: '', //钉钉用户id
  dingTalkUserName: '', //钉钉用户名
  posts: [],
  organization: '',
  roles: [],
  managingOrganizations: [],
  depts: [],
  managingJobs: [],
  managingRoles: [],
  superior: [], //上级人员
  subordinate: [], //下级人员
  sort: 999,
  iconUrl: '',
  signUrl: '',
  iconImage: '',
  signatureImage: ''
})
onMounted(async () => {
  if (props.userid !== '33300') {
    userbyidData(props.userid)
  } else {
    refreshForm()
    LconList.value = []
    SignList.value = []
  }
})
const refreshTree = inject('refreshTree')
const submitFormWithImages = () => {
  const formData = new FormData()
  const formarry = Object.keys(modelRef.value)
  formarry.forEach((element) => {
    if (element == 'iconImage' || element == 'signatureImage') {
      if (modelRef.value[element] instanceof FormData) {
        formData.append(element, modelRef.value[element].get(element))
      }
    } else if (Array.isArray(modelRef.value[element])) {
      modelRef.value[element].forEach((item) => {
        formData.append(`${element}`, item)
      })
    } else {
      formData.append(element, modelRef.value[element] == null ? '' : modelRef.value[element])
    }
  })
  return formData
}
const saveUser = async (userobj) => {
  const formDataWithImages = submitFormWithImages(userobj)
  let data
  if (userobj.id === '') {
    data = await useAddUserAsync(formDataWithImages)
    if (data.code === 200 && data.success) {
      message.success(data.message)
      refreshTree()
      refreshForm()
    } else {
      message.error(data.message)
    }
  } else {
    data = await useUpdateUserAsync(formDataWithImages)
    if (data.code === 200 && data.success) {
      message.success(data.message)
      refreshTree()
      refreshForm()
      userbyidData(props.userid)
    } else {
      message.error(data.message)
    }
  }
}
const iconnitializePassword = async () => {
  const idsuserId = modelRef.value.idsUserId
  if (idsuserId === '' || idsuserId === null) {
    message.warning('用户id为空')
  }
  const data = await useInitializePassword(idsuserId)
  if (data.code === 200 && data.success) {
    message.success(data.message)
  } else {
    message.error(data.message)
  }
}

const removeImage = () => {
  LconList.value = []
  SignList.value = []
}

const deleteuser = async () => {
  const userId = modelRef.value.id
  if (userId === '' || userId === null) {
    return message.warning('用户id为空')
  }

  try {
    const data = await useSoftDeletionUserAsync(userId)
    if (data.code === 200 && data.success) {
      message.success(data.message)
      refreshForm()
      refreshTree('del')
      return Promise.resolve(data)
    } else {
      message.error(data.message)
      return Promise.reject(data.message)
    }
  } catch (error) {
    message.error('删除失败')
    return Promise.reject(error)
  }
}

function validatorString(rule, value, wordnum, idRequired) {
  if (!value && idRequired) {
    return Promise.reject(new Error('输入为空或包含空格'))
  } else if (!value && !idRequired) {
    return Promise.resolve()
  } else if (/\s/.test(value)) {
    return Promise.reject(new Error('输入不能包含空格'))
  } else if (value.length > wordnum) {
    return Promise.reject(new Error(`不能超过 ${wordnum} 个字符`))
  } else {
    return Promise.resolve()
  }
}

const rulesRef = reactive({
  loginName: [
    {
      required: true,
      validator: (rule, value) => {
        if (!value || /\s/.test(value)) {
          return Promise.reject(new Error('输入为空或包含空格'))
        } else if (!/^[a-zA-Z0-9\u4e00-\u9fa5-]+$/.test(value)) {
          return Promise.reject(new Error('输入不能包含除 - 的特殊字符'))
        } else if (value.length > 30) {
          return Promise.reject(new Error(`不能超过30个字符`))
        } else {
          return Promise.resolve()
        }
      }
    }
  ],
  realName: [
    {
      required: true,
      validator: (rule, value) => {
        if (!value || /\s/.test(value)) {
          return Promise.reject(new Error('输入为空或包含空格'))
        } else if (!/^[a-zA-Z0-9\u4e00-\u9fa5()\-]+$/.test(value)) {
          return Promise.reject(new Error('输入不能包含除（）- 的特殊字符'))
        } else if (value.length > 30) {
          return Promise.reject(new Error(`不能超过30个字符`))
        } else {
          return Promise.resolve()
        }
      }
    }
  ],
  alias: [
    {
      validator: (rule, value) => {
        return validatorString(rule, value, 30, false)
      }
    }
  ],
  code: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value, 30, true)
      }
    }
  ],
  idCard: [
    {
      validator: (rule, value) => {
        if (!value) return Promise.resolve()
        if (!/(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/.test(value)) {
          return Promise.reject(new Error('身份证输入有误'))
        } else {
          return Promise.resolve()
        }
      }
    }
  ],
  phone: [
    {
      trigger: 'change',
      validator: (rule, value) => {
        if (!value) return Promise.resolve()
        if (!/^1\d{10}$/.test(value)) {
          return Promise.reject(new Error('手机号输入有误'))
        } else {
          return Promise.resolve()
        }
      }
    }
  ],
  organization: [
    {
      required: true,
      message: '请选择所所在部门'
    }
  ],
  email: [
    {
      validator: (rule, value) => {
        if (!value) {
          return Promise.resolve()
        } else if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(value)) {
          return Promise.reject(new Error('邮箱输入有误'))
        } else {
          return Promise.resolve()
        }
      }
    }
  ],
  sort: [
    {
      validator: (rule, value) => {
        if (value === '' || /^[0-9]+$/.test(value)) {
          if (value === '' || (value >= 0 && value <= 9999999)) {
            return Promise.resolve()
          } else {
            return Promise.reject(new Error('排序号必须在 0 到 9999999 之间'))
          }
        } else {
          return Promise.reject(new Error('排序号必须为自然数'))
        }
      }
    }
  ]
})

const { validate, validateInfos, resetFields } = useForm(
  modelRef,
  rulesRef
  // {
  //   onValidate: (...args) => {} //{console.log(...args)}
  // }
)
const onUserSubmit = () => {
  validate()
    .then(() => {
      saveUser(modelRef.value)
    })
    .catch(() => {
      //console.log('error', err)
    })
}
defineExpose({ onUserSubmit, iconnitializePassword, deleteuser, removeImage })
//****************************************************** */
const handleRemove = () => {
  LconList.value = []
  modelRef.value.iconUrl = null
  return true
}
const handleSignRemove = () => {
  SignList.value = []
  modelRef.value.signUrl = null
  return true
}
const LconList = ref([])

const beforeUpload = async (file) => {
  LconList.value = []
  modelRef.value.iconUrl = null

  if (LconList.value.length === 1) {
    message.warning('只能上传一张图片!')
    return false
  }
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  if (!isJpgOrPng) {
    message.error('您只能上传JPG文件!')
    return false
  }
  LconList.value.push(file)
  const formData = new FormData()
  formData.append('iconImage', file)
  modelRef.value.iconImage = formData
  return false
}
//****************************************************** */
const SignList = ref([])
const beforeSignUpload = async (file) => {
  SignList.value = []
  modelRef.value.signUrl = null

  if (SignList.value.length === 1) {
    SignList.value = []
    message.error('只能上传一张图片!')
    return false
  }
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  if (!isJpgOrPng) {
    message.error('您只能上传JPG文件!')
    return false
  }
  SignList.value.push(file)
  const formData = new FormData()
  formData.append('signatureImage', file)
  modelRef.value.signatureImage = formData
  return false
}
//**********************TreeTable************************* */
const oraganizerowSelection = ref()
const open = ref(false)
const drawerpage = ref(false)
const treeTableObj = ref({
  //roleselectedRowKeys: [],
  title: '',
  wechatuserinput: '未绑定', //微信用户勾选
  dingtalkuserinput: '未绑定', //钉钉ID勾选
  postsinput: '',
  organizationsinput: '',
  rolesinput: '',
  managinginput: '',
  deptinput: '',
  managingJobsInput: '',
  managingRolesInput: '',
  roleisSelectAll: false,
  superiorInput: '',
  subordinateInput: ''
})
//动态a-table
// const selectedRowKeysObj = reactive({
//   roleSelectedRowKeys: []
// })
const wechatSelectedRowKeys = ref([])
const dingtalkSelectedRowKeys = ref([])
const postsSelectedRowKeys = ref([])
const oraganizeSelectedRowKeys = ref([])
const roleSelectedRowKeys = ref([])
const managingSelectedRowKey = ref([])
const deptSelectedRowKey = ref([])
const managingJobSelectedRowKey = ref([])
const managingRoleSelectedRowKey = ref([])
const superiorSelectedRowKey = ref([])
const subordinateSelectedRowKey = ref([])
//选择所属部门数据，作为直属部门选择数据
const DirectlyUnderDept = ref([])
//需要改岗位现在没接口
const user_posts = () => {
  drawerpage.value = false
  treeTableObj.value.title = '所在岗位'
  oraganizerowSelection.value = {
    selectedRowKeys: postsSelectedRowKeys,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.posts = []
      treeTableObj.value.postsinput = ''
      let postsarry = []
      selectedRows.forEach((item) => {
        postsarry.push(item.name)
      })
      treeTableObj.value.postsinput = postsarry.join(',')
      modelRef.value.posts = selectedRowKeys
      postsSelectedRowKeys.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadonly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '1'
  columns.value = [
    {
      title: '岗位名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  posts()
}
const user_managingJobs = () => {
  drawerpage.value = false
  treeTableObj.value.title = '管理岗位'
  oraganizerowSelection.value = {
    selectedRowKeys: managingJobSelectedRowKey,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.managingJobs = []
      treeTableObj.value.managingJobsInput = ''
      let postsarry = []
      selectedRows.forEach((item) => {
        postsarry.push(item.name)
      })
      treeTableObj.value.managingJobsInput = postsarry.join(',')
      modelRef.value.managingJobs = selectedRowKeys
      managingJobSelectedRowKey.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadonly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '1'
  columns.value = [
    {
      title: '岗位名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  posts()
}
const user_oraganize = () => {
  if (DirectlyUnderDept.value.length === 0) {
    message.warning('请先勾选所属部门！')
    return
  }
  drawerpage.value = false
  treeTableData.value = DirectlyUnderDept.value
  treeTableObj.value.title = '直属部门'
  oraganizerowSelection.value = {
    selectedRowKeys: oraganizeSelectedRowKeys,
    type: 'radio',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.organization = ''
      treeTableObj.value.organizationsinput = ''

      treeTableObj.value.organizationsinput = selectedRows[0].name
      modelRef.value.organization = selectedRowKeys[0]
      oraganizeSelectedRowKeys.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadonly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '2'
  columns.value = [
    {
      title: '直属部门名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  //oraganize()
}
const user_role = () => {
  drawerpage.value = false
  treeTableObj.value.title = '所在角色'
  oraganizerowSelection.value = {
    //columnTitle: ' ',
    selectedRowKeys: roleSelectedRowKeys,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.roles = []
      treeTableObj.value.rolesinput = ''
      let rolesarry = []
      selectedRows.forEach((item) => {
        rolesarry.push(item.name)
      })
      treeTableObj.value.rolesinput = rolesarry.join(',')
      roleSelectedRowKeys.value = selectedRowKeys
      modelRef.value.roles = roleSelectedRowKeys.value
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadOnly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '3'
  columns.value = [
    {
      title: '角色名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  roleData()
}
const user_managingRole = () => {
  drawerpage.value = false
  treeTableObj.value.title = '管理角色'
  oraganizerowSelection.value = {
    //columnTitle: ' ',
    selectedRowKeys: managingRoleSelectedRowKey,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.managingRoles = []
      treeTableObj.value.managingRolesInput = ''
      let rolesarry = []
      selectedRows.forEach((item) => {
        rolesarry.push(item.name)
      })
      treeTableObj.value.managingRolesInput = rolesarry.join(',')
      managingRoleSelectedRowKey.value = selectedRowKeys
      modelRef.value.managingRoles = managingRoleSelectedRowKey.value
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadOnly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '3'
  columns.value = [
    {
      title: '角色名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  roleData()
}
const user_managing = () => {
  drawerpage.value = false
  treeTableObj.value.title = '管理部门'
  oraganizerowSelection.value = {
    selectedRowKeys: managingSelectedRowKey,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.managingOrganizations = []
      treeTableObj.value.managinginput = ''
      let managingarry = []
      selectedRows.forEach((item) => {
        managingarry.push(item.name)
      })
      treeTableObj.value.managinginput = managingarry.join(',')
      modelRef.value.managingOrganizations = selectedRowKeys
      managingSelectedRowKey.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadonly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '4'
  columns.value = [
    {
      title: '部门名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  oraganize()
}
const user_depts = () => {
  drawerpage.value = false
  treeTableObj.value.title = '所属部门'
  oraganizerowSelection.value = {
    selectedRowKeys: deptSelectedRowKey,
    type: 'checkbox',
    onChange: (selectedRowKeys, selectedRows) => {
      DirectlyUnderDept.value = []
      modelRef.value.depts = []
      treeTableObj.value.deptinput = ''
      let deptarry = []
      selectedRows.forEach((item) => {
        deptarry.push(item.name)
        DirectlyUnderDept.value.push({ key: item.key, name: item.name })
      })
      treeTableObj.value.deptinput = deptarry.join(',')
      modelRef.value.depts = selectedRowKeys

      deptSelectedRowKey.value = selectedRowKeys

      //联动取消直属
      if (modelRef.value.organization != '' && !selectedRowKeys.includes(modelRef.value.organization)) {
        oraganizeSelectedRowKeys.value = []
        modelRef.value.organization = ''
        treeTableObj.value.organizationsinput = ''
      }
    },
    getCheckboxProps: (record) => {
      if (isOneself.value) {
        return {
          disabled: true
        }
      }
      return {
        disabled: record.isReadonly
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '7'
  columns.value = [
    {
      title: '部门名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  oraganize()
}
//微信用户
const user_wechat = () => {
  drawerpage.value = false
  treeTableObj.value.title = '微信用户'
  oraganizerowSelection.value = {
    selectedRowKeys: wechatSelectedRowKeys,
    type: 'checkbox',
    columnTitle: ' ',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.weChatUserId = selectedRowKeys.slice(-1)
      wechatSelectedRowKeys.value = selectedRowKeys.slice(-1)
      if (selectedRowKeys.slice(-1).length > 0) {
        treeTableObj.value.wechatuserinput = selectedRows.slice(-1)[0].name
        modelRef.value.weChatUserName = selectedRows.slice(-1)[0].name
      } else {
        treeTableObj.value.wechatuserinput = ''
        modelRef.value.weChatUserName = ''
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '5'
  columns.value = [
    {
      title: '微信用户名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  wechatuser()
}
//钉钉用户
const user_dingtalk = () => {
  drawerpage.value = false
  treeTableObj.value.title = '钉钉用户'
  oraganizerowSelection.value = {
    selectedRowKeys: dingtalkSelectedRowKeys,
    type: 'checkbox',
    columnTitle: ' ',
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.dingTalkUserId = selectedRowKeys.slice(-1)
      dingtalkSelectedRowKeys.value = selectedRowKeys.slice(-1)
      if (selectedRowKeys.slice(-1).length > 0) {
        treeTableObj.value.dingtalkuserinput = selectedRows.slice(-1)[0].name
        modelRef.value.dingTalkUserName = selectedRows.slice(-1)[0].name
      } else {
        treeTableObj.value.dingtalkuserinput = ''
        modelRef.value.dingTalkUserName = ''
      }
    }
  }
  query.value = ''
  drawer_tabletype.value = '6'
  columns.value = [
    {
      title: '钉钉用户名称',
      dataIndex: 'name',
      key: 'name',
      width: '100%'
    }
  ]
  open.value = true
  dingtalkuser()
}

//*******************所属上级分页************************ */
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  getSuperiorAndSubordinateData('')
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  getSuperiorAndSubordinateData('')
}

const pagination = ref({
  position: ['bottomRight'],
  current: 1, // 当前页数
  pageSize: 10, // 每页条数
  total: 0, // 数据总数
  showSizeChanger: true, // 是否可以改变每页条数
  showQuickJumper: true, // 是否可以快速跳转至某页
  onChange: handlePageChange, // 页码改变的回调
  onShowSizeChange: handlePageSizeChange, // 每页条数改变的回调
  showTotal: (total) => `共 ${total} 条数据`,
  pageSizeOptions: ['10', '20', '30', '40', '50']
})
const superiorAndSubordinatecolumns = () => {
  columns.value = [
    {
      title: '序号',
      width: '10%',
      key: 'nonEditable',
      align: 'center',
      customRender: (obj) => {
        return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
      }
    },
    {
      title: '用户名称',
      dataIndex: 'realName',
      align: 'center',
      key: 'realName',
      width: '20%'
    },
    {
      title: '性别',
      dataIndex: 'sex',
      align: 'center',
      key: 'sex',
      width: '10%',
      customRender: (obj) => {
        return obj.text == 0 ? '男' : '女'
      }
    },
    {
      title: '用户状态',
      dataIndex: 'enabled',
      align: 'center',
      key: 'enabled',
      width: '15%',
      customRender: (obj) => {
        return obj.text == true ? '正常' : '冻结'
      }
    },
    {
      title: '用户编码',
      dataIndex: 'code',
      align: 'center',
      key: 'code',
      width: '20%'
    },
    {
      title: '直属部门',
      dataIndex: 'oraganizeName',
      align: 'center',
      key: 'oraganizeName',
      width: '30%'
    }
  ]
}
//*******************所属上级抽屉************************ */
const user_superior = () => {
  drawerpage.value = true
  treeTableObj.value.title = '所属上级'
  getSuperiorAndSubordinateData('')
  //表头
  superiorAndSubordinatecolumns()
  oraganizerowSelection.value = {
    selectedRowKeys: superiorSelectedRowKey,
    type: 'checkbox',
    preserveSelectedRowKeys: true,
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.superior = []
      treeTableObj.value.superiorInput = ''
      let superiorarry = []
      selectedRows.forEach((item) => {
        superiorarry.push(item.realName)
      })
      treeTableObj.value.superiorInput = superiorarry.join(',')
      modelRef.value.superior = selectedRowKeys
      superiorSelectedRowKey.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
       if (!isOneself.value) {
        return {
          disabled: false
        }
      }
      return {
        disabled: record.isReadyOnly
      }
    }
  }
  drawer_tabletype.value = '8'
  open.value = true
}

const user_subordinate = () => {
  drawerpage.value = true
  treeTableObj.value.title = '管理下级'
  getSuperiorAndSubordinateData('')
  superiorAndSubordinatecolumns()
  oraganizerowSelection.value = {
    selectedRowKeys: subordinateSelectedRowKey,
    type: 'checkbox',
    preserveSelectedRowKeys: true,
    onChange: (selectedRowKeys, selectedRows) => {
      modelRef.value.subordinate = []
      treeTableObj.value.subordinateInput = ''
      let subordinatearry = []
      selectedRows.forEach((item) => {
        subordinatearry.push(item.realName)
      })
      treeTableObj.value.subordinateInput = subordinatearry.join(',')
      modelRef.value.subordinate = selectedRowKeys
      subordinateSelectedRowKey.value = selectedRowKeys
    },
    getCheckboxProps: (record) => {
       if (!isOneself.value) {
        return {
          disabled: false
        }
      }
      return {
        disabled: record.isReadyOnly
      }
    }
  }
  drawer_tabletype.value = '8'
  open.value = true
}

const onClose = () => {
  pagination.value.current = 1
  pagination.value.pageSize = 10
  pagination.value.total = 0
  treeTableData.value = []
  treeTableObj.value.roleisSelectAll = false
  open.value = false
}
//************角色全选以及取消*************** */
const rolecheckall = (st) => {
  let rolesarry = []
  if (treeTableObj.value.title == '所在角色') {
    treeTableObj.value.rolesinput = ''

    roleSelectedRowKeys.value = []
    modelRef.value.roles = []
    if (st) {
      treeTableData.value.forEach((item) => {
        if (item.pid == null || item.pid == '') {
          roleSelectedRowKeys.value.push(item.key)
          rolesarry.push(item.name)
        }
      })
      modelRef.value.roles = roleSelectedRowKeys.value
    }
    treeTableObj.value.rolesinput = rolesarry.join(',')
  }
  if (treeTableObj.value.title == '管理角色') {
    treeTableObj.value.managingRolesInput = ''

    managingRoleSelectedRowKey.value = []
    modelRef.value.managingRoles = []
    if (st) {
      treeTableData.value.forEach((item) => {
        if (item.pid == null || item.pid == '') {
          managingRoleSelectedRowKey.value.push(item.key)
          rolesarry.push(item.name)
        }
      })
      modelRef.value.managingRoles = managingRoleSelectedRowKey.value
    }
    treeTableObj.value.managingRolesInput = rolesarry.join(',')
  }
}

const drawer_tabletype = ref('')
const columns = ref([])
const treeTableData = ref([])
const query = ref('')
const condition = (resetting) => {
  if (resetting) {
    query.value = ''
  }
  if (drawer_tabletype.value === '1') {
    posts(query.value)
  } else if (drawer_tabletype.value === '2') {
    if (query.value != '') {
      treeTableData.value = DirectlyUnderDept.value.filter((f) => f.name.includes(query.value))
    } else {
      treeTableData.value = DirectlyUnderDept.value
    }
  } else if (drawer_tabletype.value === '3') {
    roleData(query.value)
  } else if (drawer_tabletype.value === '4') {
    oraganize('', query.value)
  } else if (drawer_tabletype.value === '5') {
    //微信用户
    wechatuser(query.value)
  } else if (drawer_tabletype.value === '6') {
    //钉钉用户
    dingtalkuser(query.value)
  } else if (drawer_tabletype.value === '7') {
    //所属部门
    oraganize('', query.value)
  } else if (drawer_tabletype.value === '8') {
    getSuperiorAndSubordinateData(query.value)
  }
}
//************微信部门逻辑**************** */
function convertWeChatOrDingTalkTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    key: data.id,
    name: data.name,
    category: 1,
    children: []
  }
  if (data.users && data.users.length) {
    data.users.forEach((user) => {
      converted.children.push({
        key: user.userid,
        name: user.name,
        category: 2
      })
    })
  }
  if (data.children && data.children.length) {
    data.children.forEach((or) => {
      converted.children.push(convertWeChatOrDingTalkTree(or))
    })
  }
  return converted
}
const wechatuser = async (name) => {
  const data = await useGetWeChatOraganizeUserTree(name)
  if (data.code === 200 && data.success) {
    treeTableData.value = []
    data.data.forEach((or) => {
      treeTableData.value.push(convertWeChatOrDingTalkTree(or))
    })
  } else {
    message.error(data.message)
  }
}
//************钉钉部门逻辑**************** */
const dingtalkuser = async (name) => {
  const data = await useGetDingTalkOraganizeUserTree(name)
  if (data.code === 200 && data.success) {
    treeTableData.value = []
    data.data.forEach((or) => {
      treeTableData.value.push(convertWeChatOrDingTalkTree(or))
    })
  } else {
    message.error(data.message)
  }
}
//************所属岗位逻辑**************** */
const posts = async (name) => {
  const data = await useGetPostsAsync(name)
  if (data.code === 200 && data.success) {
    treeTableData.value = []
    treeTableData.value = data.data
  } else {
    message.error('岗位查询失败!')
  }
}
//************所属;管理部门逻辑**************** */
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    name: data.name,
    key: data.id,
    isReadonly: data.isReadonly,
    children: []
  }
  if (data.oraganizeTrees && data.oraganizeTrees.length) {
    data.oraganizeTrees.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}
const oraganize = async (id, name) => {
  const data = await useGetOraganizeTreeAsync(id, name)
  if (data.code === 200 && data.success) {
    treeTableData.value = []
    data.data.forEach((or) => {
      treeTableData.value.push(convertTree(or))
    })
  } else {
    message.error('部门树查询失败!')
  }
}
//************所属角色逻辑**************** */
const roleStructure = (data) => {
  data.forEach((parent) => {
    const obj = {
      name: parent.name,
      key: parent.id,
      children: [],
      isReadOnly: parent.isReadOnly,
      pid: null
    }
    if (parent.children && parent.children.length > 0) {
      parent.children.forEach((cil) => {
        obj.children.push({
          name: cil.name,
          key: cil.id,
          pid: cil.parentId,
          isReadOnly: cil.isReadOnly
        })
      })
    }
    treeTableData.value.push(obj)
  })
}
const roleData = async (roleName) => {
  const data = await useGetRolesAsync(roleName)
  if (data.code === 200 && data.success) {
    treeTableObj.value.roleisSelectAll = true
    treeTableData.value = []
    roleStructure(data.data)
  } else {
    message.error('角色查询失败!')
  }
}
//************所属上级;管理下级逻辑**************** */
const getSuperiorAndSubordinateData = async (userName) => {
  const org = treeTableObj.value.title == '所属上级' ? modelRef.value.organization : ''
  const parameter = {
    oraganizeId: org,
    managingOrgIds: modelRef.value.managingOrganizations,
    userId: props.userid,
    userName: userName,
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }
  const data = await useGetSuperiorAndSubordinateAsync(parameter)
  if (data.code === 200 && data.success) {
    treeTableData.value = data.data
    pagination.value.total = data.total
  }
}
//*********************************************** */
const refreshForm = () => {
  resetFields()
  treeTableObj.value = {
    title: '',
    wechatuserinput: '', //微信用户勾选
    dingtalkuserinput: '', //钉钉用户勾选
    postsinput: '',
    organizationsinput: '',
    rolesinput: '',
    managinginput: '',
    deptinput: '',
    managingJobsInput: '',
    managingRolesInput: '',
    superiorInput: '',
    subordinateInput: '',
    roleisSelectAll: false
  }
  DirectlyUnderDept.value = []
  postsSelectedRowKeys.value = []
  oraganizeSelectedRowKeys.value = []
  roleSelectedRowKeys.value = []
  managingSelectedRowKey.value = []
  deptSelectedRowKey.value = []
  managingJobSelectedRowKey.value = []
  managingRoleSelectedRowKey.value = []
  superiorSelectedRowKey.value = []
  subordinateSelectedRowKey.value = []
  removeImage()
}
//************反选逻辑**************** */
const props = defineProps({
  userid: String
})
const userbyidData = (userid) => {
  useGetUsersByIdAsync(userid).then((data) => {
    if (data.code === 200 && data.success) {
      isOneself.value = userStore.value.userid == props.userid
      modelRef.value = data.data.userFormDto
      treeTableObj.value.wechatuserinput = data.data.userFormDto.weChatUserName
      treeTableObj.value.dingtalkuserinput = data.data.userFormDto.dingTalkUserName
      treeTableObj.value.postsinput = data.data.displayName.postsName
      treeTableObj.value.organizationsinput = data.data.displayName.oraganizesName
      treeTableObj.value.rolesinput = data.data.displayName.rolesName
      treeTableObj.value.managinginput = data.data.displayName.managingName
      treeTableObj.value.deptinput = data.data.displayName.deptName
      treeTableObj.value.managingJobsInput = data.data.displayName.managingJobsName
      treeTableObj.value.managingRolesInput = data.data.displayName.managingRolesName
      treeTableObj.value.superiorInput = data.data.displayName.superiorName
      treeTableObj.value.subordinateInput = data.data.displayName.subordinateName
      DirectlyUnderDept.value = data.data.displayName.dptData
      wechatSelectedRowKeys.value[0] = modelRef.value.weChatUserId
      dingtalkSelectedRowKeys.value[0] = modelRef.value.dingTalkUserId
      postsSelectedRowKeys.value = modelRef.value.posts
      managingRoleSelectedRowKey.value = modelRef.value.managingRoles
      managingJobSelectedRowKey.value = modelRef.value.managingJobs
      oraganizeSelectedRowKeys.value[0] = modelRef.value.organization
      roleSelectedRowKeys.value = modelRef.value.roles
      managingSelectedRowKey.value = modelRef.value.managingOrganizations
      deptSelectedRowKey.value = modelRef.value.depts
      superiorSelectedRowKey.value = modelRef.value.superior
      subordinateSelectedRowKey.value = modelRef.value.subordinate
      LconList.value = []
      SignList.value = []
      if (data.data.userFormDto.iconUrl != null && data.data.userFormDto.iconUrl != '') {
        LconList.value = [{ url: ptUrl + '/' + data.data.userFormDto.iconUrl }]
      }
      if (data.data.userFormDto.signUrl != null && data.data.userFormDto.signUrl != '') {
        SignList.value = [{ url: ptUrl + '/' + data.data.userFormDto.signUrl }]
      }
    } else {
      message.error(data.message)
    }
  })
}

watch(
  () => props.userid,
  (newValue) => {
    if (newValue === '33300') {
      refreshForm()
    } else {
      userbyidData(newValue)
    }
  }
)
</script>
<style scoped lang="scss">
.right_user {
  padding-top: 10px;
  font-size: 20px;
  width: 100%;
  height: 100%;
  overflow: auto;
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

.avatar-uploader img {
  max-width: 100px;
  /* 设置图片的最大宽度 */
  max-height: 100px;
  /* 设置图片的最大高度 */
  pointer-events: none;
  /* 禁用鼠标事件 */
}

.drawer {
  position: relative;
}
.fenye {
  position: absolute;
  bottom: 10px;
  right: 10px;
}
</style>
