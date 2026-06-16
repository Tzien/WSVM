<template>
  <div class="all">
    <div class="shift">
      <div class="header">
        <div class="title">班次信息</div>
        <!-- <div class="sysshiftbtn">
          <a-button v-btnlog="str" style="margin-right: 2px" @click="addshift">新增班次</a-button>
          <a-button v-btnlog="strw" style="margin-right: 2px" @click="delshift">删除选中班次</a-button>
        </div> -->
      </div>
      <a-table :scroll="{ y: 'calc(40vh - 150px)' }" :row-selection="shiftrowSelection" :columns="shiftcolumns" :data-source="shiftData" :pagination="false">
        <template #bodyCell="{ column, text, record }">
          <template v-if="['enabled'].includes(column.dataIndex)">
            <template v-if="text == true"> 激活 </template>
            <template v-else> 冻结 </template>
          </template>
          <template v-else-if="column.dataIndex === 'operation'">
            <a-button :icon="h(EditOutlined)" style="display: inline" @click="editshift(record.id)"></a-button>
          </template>
        </template>
      </a-table>
    </div>
    <div class="class">
      <div class="header">
        <div class="title">班组信息</div>
        <div class="sysshiftbtn">
          <a-button
            v-btnlog="addSysShift"
            v-if="btnObj.addSysShift.isShow"
            style="margin-right: 2px"
            @click="addclass()"
            type="primary"
            :icon="showIcon(btnObj.addSysShift.Icon)"
            >{{ btnObj.addSysShift.name }}</a-button
          >
          <a-button
            v-btnlog="delSysShift"
            v-if="btnObj.delSysShift.isShow"
            style="margin-right: 2px"
            @click="delclass()"
            type="primary"
            danger
            :icon="showIcon(btnObj.delSysShift.Icon)"
            >{{ btnObj.delSysShift.name }}</a-button
          >
          <a-input-search
            v-if="btnObj.querySysShift.isShow"
            style="width: 200px; margin-right: 2px"
            v-model:value="clsbyuser"
            placeholder="请输入用户名称"
            @search="quretyclsbyuser"
          />
        </div>
      </div>

      <a-table
        :customRow="classCustomRow"
        :scroll="{ x: '50vh', y: 'calc(60vh - 180px)' }"
        :row-selection="classrowSelection"
        :columns="classcolumns"
        :data-source="classData"
        :pagination="false"
      >
        <template #bodyCell="{ column, text, record }">
          <template v-if="column.dataIndex === 'operation'">
            <a-button v-btnlog="uptSysShift" :icon="h(EditOutlined)" style="display: inline" @click.stop="editclass(record.id)"></a-button>
          </template>
        </template>
      </a-table>
    </div>
    <div class="user">
      <div class="header">
        <div class="title">班组人员信息</div>
        <div class="sysshiftbtn" v-if="classid != ''">
          <a-button v-if="btnObj.addUser.isShow" style="margin-right: 2px" @click="adduser" type="primary" :icon="showIcon(btnObj.addUser.Icon)">{{
            btnObj.addUser.name
          }}</a-button>
          <a-button v-if="btnObj.delUser.isShow" style="margin-right: 2px" @click="deluser" type="primary" danger :icon="showIcon(btnObj.delUser.Icon)">{{
            btnObj.delUser.name
          }}</a-button>
          <a-input-search
            v-if="btnObj.queryUser.isShow"
            style="width: 200px; margin-right: 2px"
            v-model:value="userInput"
            placeholder="请输入用户名称"
            @search="userqurety"
          />
        </div>
      </div>
      <a-table
        :row-selection="userrowSelection"
        :scroll="{ x: 1600, y: 'calc(60vh - 220px)' }"
        :columns="clsusercolumns"
        :data-source="clsuserData"
        :pagination="false"
      >
        <template #bodyCell="{ column, text, record }">
          <template v-if="column.dataIndex === 'sex'">
            <div v-if="text === 0">男</div>
            <div v-else-if="text === 1">女</div>
          </template>
          <template v-else-if="['enabled'].includes(column.dataIndex)">
            <template v-if="text == true"> 激活 </template>
            <template v-else> 冻结 </template>
          </template>
        </template>
      </a-table>
      <div class="fenye">
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
    </div>
  </div>
  <a-drawer :title="drawertitle" size="large" :closable="false" :open="open" @close="onClose">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-item label="是否激活" v-bind="shiftForm.validateInfos.enabled">
        <div>
          <a-radio-group v-model:value="shiftModel.enabled">
            <a-radio :value="true">正常</a-radio>
            <a-radio :value="false">冻结</a-radio>
          </a-radio-group>
        </div>
      </a-form-item>
      <a-form-item label="班次名称" v-bind="shiftForm.validateInfos.moduleName">
        <a-input v-model:value="shiftModel.moduleName" ref="select" style="width: 200px" />
      </a-form-item>
      <a-form-item label="备注" v-bind="shiftForm.validateInfos.remark">
        <a-input v-model:value="shiftModel.remark" ref="select" style="width: 200px" />
      </a-form-item>
      <a-form-item label="编码" v-bind="shiftForm.validateInfos.code">
        <a-input v-model:value="shiftModel.code" ref="select" style="width: 200px" />
      </a-form-item>
      <a-form-item label="接口编码" v-bind="shiftForm.validateInfos.interfaceCode">
        <a-input v-model:value="shiftModel.interfaceCode" ref="select" style="width: 200px" />
      </a-form-item>
      <a-form-item label="排序" v-bind="shiftForm.validateInfos.sort">
        <a-input v-model:value="shiftModel.sort" ref="select" style="width: 200px" />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button type="primary" @click="saveShift" style="margin-right: 50px">保存</a-button>
        <a-button @click="onClose">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
  <a-drawer :title="classtitle" size="large" :closable="false" :open="classopen" @close="classClose">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-item label="分厂/工序" v-bind="classForm.validateInfos.groupId">
        <div style="width: 100%">
          <a-select v-model:value="classModel.groupId" show-search placeholder="请选择" style="width: 100%" :options="classoptions"></a-select>
        </div>
      </a-form-item>
      <a-form-item label="班组名称" v-bind="classForm.validateInfos.moduleName">
        <a-input v-model:value="classModel.moduleName" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="班组编码" v-bind="classForm.validateInfos.code">
        <a-input v-model:value="classModel.code" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="班组接口编码" v-bind="classForm.validateInfos.interfaceCode">
        <a-input v-model:value="classModel.interfaceCode" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="备注" v-bind="classForm.validateInfos.remark">
        <a-input v-model:value="classModel.remark" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="排序" v-bind="classForm.validateInfos.sort">
        <a-input v-model:value="classModel.sort" ref="select" style="width: 100%" />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button v-btnlog="saveSysShift" type="primary" @click="saveClass" style="margin-right: 50px">保存</a-button>
        <a-button @click="classClose">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
  <a-drawer :title="usertitle" :width="'1250'" :closable="false" :open="useropen" @close="userClose">
    <div class="sysshiftbtn">
      <a-tree-select
        v-model:value="selectedDpt"
        :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
        style="width: 380px; height: 35px; margin: 0 1px"
        :tree-data="treeData"
        tree-checkable
        allow-clear
        :show-checked-strategy="SHOW_PARENT"
        placeholder="请选择直属部门"
        tree-node-filter-prop="label"
        :maxTagCount="3"
      />
      <a-input style="width: 200px" v-model:value="nameInput" placeholder="请输入用户名称" />
      <a-button @click="queryuser" style="height: 35px; margin: 0 1px">查询</a-button>
      <a-button @click="resetqueryuser" style="height: 35px; margin: 0 1px">重置查询</a-button>
      <a-button @click="addnotuser" style="height: 35px; margin: 0 1px">添加用户</a-button>
    </div>
    <a-divider></a-divider>
    <a-table
      :row-selection="notuserrowSelection"
      :scroll="{ x: 1600, y: 'calc(100vh - 160px)' }"
      :columns="clsusercolumns"
      :data-source="alluserdata"
      :pagination="false"
    >
    </a-table>
    <div class="fenye">
      <a-pagination
        v-model:current="userpagination.current"
        v-model:page-size="userpagination.pageSize"
        :total="userpagination.total"
        :show-total="() => `共 ${userpagination.total} 条数据`"
        :show-size-changer="userpagination.showSizeChanger"
        :show-quick-jumper="userpagination.showQuickJumper"
        :page-size-options="userpagination.pageSizeOptions"
        @showSizeChange="userhandlePageChange"
        @change="userhandlePageSizeChange"
      >
        <template #buildOptionText="props">
          <span>{{ props.value }}条/页</span>
        </template>
      </a-pagination>
    </div>
  </a-drawer>
</template>
<script setup>
import { reactive, ref, h, watch, watchEffect, onMounted } from 'vue'
import { Form } from 'ant-design-vue'
import { message } from 'ant-design-vue'
import { useGetFactorySelect } from '@/api/BaseInfoConfig/factoryinfo'
import {
  useSoftDeleteSysShiftClassAsync,
  useGetShiftClassByIdAsync,
  useGetSysShiftByIdAsync,
  useUpdateSysShiftAsync,
  useGetAllSysShiftAsync,
  useGetShiftClassAsync,
  useInsertSysShiftAsync,
  useSoftDeleteSysShiftAsync,
  useInsertSysShiftClassAsync,
  useUpdateSysShiftClassAsync,
  useGetUserByClassIdAsync,
  useDeletSysShiftClassUserAsync,
  useInsertSysShiftClassUserAsync
} from '@/api/SysShift'
import { useGetOraganizeTreeAsync } from '@/api/BaseInfoConfig/organization'
import { TreeSelect } from 'ant-design-vue'
import { getButtonList } from '../../api/commonFun'
import { useUserStore } from '../../store/index.js'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
import * as Icons from '@ant-design/icons-vue'
import { guid } from '@/utils/guid'
import { PlusOutlined, SaveOutlined, SearchOutlined, EditOutlined, CloseOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
onMounted(() => {
  document.querySelectorAll('.ant-table-body').forEach((element) => {
    element.style.height = element.style.maxHeight
  })
})

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
const showIcon = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}

const addSysShift = { buttonOperationId: '', belongPage: '班次班组信息', buttonName: '' }
const uptSysShift = { buttonOperationId: '', belongPage: '班次班组信息', buttonName: '行内修改' }
const delSysShift = { buttonOperationId: '', belongPage: '班次班组信息', buttonName: '' }
const saveSysShift = { buttonOperationId: '', belongPage: '班次班组信息', buttonName: '保存' }

const btnObj = reactive({
  //新增班组
  addSysShift: { isShow: false, btnCode: 'A060107' },
  //删除选中班组
  delSysShift: { isShow: false, btnCode: 'A060106' },
  //根据用户查询班组
  querySysShift: { isShow: false, btnCode: 'A060104' },
  //修改
  uptSysShift: { isShow: false, btnCode: 'A060101' },
  //添加用户到当前班组
  addUser: { isShow: false, btnCode: 'A060105' },
  //从当前班组移除选中用户
  delUser: { isShow: false, btnCode: 'A060103' },
  //根据用户名称查询用户
  queryUser: { isShow: false, btnCode: 'A060102' }
})

getButtonList({ menuCode: 'A0601', roleids: userStore.value.userRoles }).then((data) => {
  if (data.code === 200 && data.success) {
    for (const key in btnObj) {
      if (Object.hasOwnProperty.call(btnObj, key)) {
        const s = data.data.buttonDtos.find((p) => p.functionCode == btnObj[key].btnCode)
        if (s) {
          btnObj[key].isShow = true
          btnObj[key].Icon = s.icon
          btnObj[key].name = s.functionName
        }
      }
    }
    addSysShift.buttonName = btnObj.addSysShift.name
    delSysShift.buttonName = btnObj.delSysShift.name
  } else {
    message.error(data.message)
  }
})

const labelCol = {
  style: {
    width: '100px'
  }
}

const wrapperCol = {
  span: 18
}
const open = ref(false)
const drawertitle = ref('')
const onClose = () => {
  open.value = false
}
const shiftcolumns = ref([
  {
    title: '序号',
    dataIndex: 'id',
    align: 'center',
    width: 80,
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '班次名称',
    align: 'center',
    dataIndex: 'moduleName'
  },
  {
    title: '激活状态',
    align: 'center',
    dataIndex: 'enabled'
  },
  {
    title: '班次编码',
    align: 'center',
    dataIndex: 'code'
  },
  {
    title: '班次接口编码',
    align: 'center',
    dataIndex: 'interfaceCode'
  },
  {
    title: '排序',
    align: 'center',
    dataIndex: 'sort'
  },
  {
    title: '创建人',
    align: 'center',
    dataIndex: 'createName'
  },
  {
    title: '创建时间',
    align: 'center',
    dataIndex: 'createTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    align: 'center',
    dataIndex: 'operation'
  }
])
const shiftData = ref([])
const sysShift = () => {
  useGetAllSysShiftAsync().then((data) => {
    if (data.code === 200 && data.success) {
      data.data.forEach((element) => {
        element.key = element.id
      })
      shiftData.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
sysShift()
//添加
const addshift = () => {
  shiftForm.resetFields()
  drawertitle.value = '添加班次信息'
  open.value = true
}

const editshift = (id) => {
  drawertitle.value = '修改班次信息'
  open.value = true
  useGetSysShiftByIdAsync(id).then((data) => {
    if (data.code === 200 && data.success) {
      shiftModel.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
const shiftkey = ref([])
const shiftrowSelection = ref({
  selectedRowKeys: shiftkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    shiftkey.value = selectedRowKeys
  }
})
const delshift = () => {
  if (shiftkey.value.length === 0) {
    message.warning('未勾选数据')
    return
  }
  useSoftDeleteSysShiftAsync(shiftkey.value).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      shiftkey.value = []
      sysShift()
    } else {
      message.error(data.message)
    }
  })
}

const saveShift = () => {
  const isAdding = drawertitle.value === '添加班次信息'
  const apiCall = isAdding ? useInsertSysShiftAsync : useUpdateSysShiftAsync
  shiftForm
    .validate()
    .then(() => {
      apiCall(shiftModel.value).then((data) => {
        if (data.code === 200 && data.success) {
          message.success(data.message)
          open.value = false
          sysShift()
          shiftForm.resetFields()
        } else {
          message.error(data.message)
        }
      })
    })
    .catch(() => {
      //console.log('error', err)
    })
}

const shiftModel = ref({
  id: '',
  enabled: true,
  moduleName: '',
  remark: '',
  code: '',
  interfaceCode: '',
  sort: 1
})
const shiftRules = ref({
  moduleName: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  remark: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  code: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  sort: [
    {
      required: true,
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
const useForm = Form.useForm
//{ validate, validateInfos, resetFields }
const shiftForm = useForm(shiftModel, shiftRules)
function validatorString(rule, value) {
  if (!value) {
    return Promise.reject(new Error('输入为空或包含空格'))
  } else if (/\s/.test(value)) {
    return Promise.reject(new Error('输入不能包含空格'))
  } else if (value.length > 10) {
    return Promise.reject(new Error('不能超过10个字符'))
  } else if (!/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test(value)) {
    // 字母数字 /^[a-zA-Z0-9]+$/
    return Promise.reject(new Error('输入不能包含特殊字符'))
  } else {
    return Promise.resolve()
  }
}
//************************************班组********************************************* */
const classcolumns = [
  {
    title: '序号',
    dataIndex: 'id',
    width: 80,
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '分厂/工序',
    dataIndex: 'factory',
    width: 120
  },
  {
    title: '班组名称',
    dataIndex: 'moduleName',
    width: 120
  },
  {
    title: '班组编码',
    width: 120,
    dataIndex: 'code'
  },
  {
    title: '班组接口编码',
    width: 120,
    dataIndex: 'interfaceCode'
  },
  {
    title: '操作',
    dataIndex: 'operation',
    fixed: 'right',
    width: 80
  }
]
const classModel = ref({
  id: '',
  moduleName: '',
  remark: '',
  code: '',
  interfaceCode: '',
  sort: 1,
  groupId: ''
})
const classRules = ref({
  groupId: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择工厂'))
        }
        return Promise.resolve()
      }
    }
  ],
  moduleName: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  remark: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  code: [
    {
      required: true,
      validator: (rule, value) => {
        return validatorString(rule, value)
      }
    }
  ],
  interfaceCode: [
    {
      validator: (rule, value) => {
        if (value.length > 10) {
          return Promise.reject(new Error('不能超过10个字符'))
        }
        return Promise.resolve()
      }
    }
  ],
  sort: [
    {
      required: true,
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
const classForm = useForm(classModel, classRules)
const clsbyuser = ref('')
const quretyclsbyuser = () => {
  querycls()
}
const classData = ref([])
const querycls = () => {
  useGetShiftClassAsync(clsbyuser.value).then((data) => {
    if (data.code === 200 && data.success) {
      classData.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
querycls()
const classopen = ref(false)
const classtitle = ref('')
const classClose = () => {
  classopen.value = false
}
const addclass = () => {
  let btnguid = guid()
  addSysShift.buttonOperationId = btnguid
  classForm.resetFields()
  classtitle.value = '添加班组信息'
  classopen.value = true
  factorySelect(btnguid)
}
const editclass = (id) => {
  let btnguid = guid()
  uptSysShift.buttonOperationId = btnguid
  classForm.resetFields()
  classtitle.value = '修改班组信息'
  classopen.value = true
  useGetShiftClassByIdAsync(id, btnguid).then((data) => {
    if (data.code === 200 && data.success) {
      classModel.value = data.data
    } else {
      message.error(data.message)
    }
  })
  factorySelect(btnguid)
}
const classkey = ref([])
const classrowSelection = ref({
  selectedRowKeys: classkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    classkey.value = selectedRowKeys
  }
})
const delclass = () => {
  let btnguid = guid()
  delSysShift.buttonOperationId = btnguid
  if (classkey.value.length === 0) {
    message.warning('未勾选数据')
    return
  }
  useSoftDeleteSysShiftClassAsync(classkey.value, btnguid).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      classkey.value = []
      querycls()
    } else {
      message.error(data.message)
    }
  })
}
const saveClass = () => {
  let btnguid = guid()
  saveSysShift.buttonOperationId = btnguid
  const isAdding = classtitle.value === '添加班组信息'
  const apiCall = isAdding ? useInsertSysShiftClassAsync : useUpdateSysShiftClassAsync
  classForm
    .validate()
    .then(() => {
      apiCall(classModel.value, btnguid).then((data) => {
        if (data.code === 200 && data.success) {
          message.success(data.message)
          classopen.value = false
          querycls()
          classForm.resetFields()
        } else {
          message.error(data.message)
        }
      })
    })
    .catch(() => {
      //console.log('error', err)
    })
}
const classoptions = ref([])
const factorySelect = (btnid) => {
  useGetFactorySelect(btnid).then((data) => {
    if (data.code === 200 && data.success) {
      classoptions.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
//************************************用户********************************************* */
const classid = ref('')
const classCustomRow = (record, index) => {
  return {
    onClick: (e) => {
      if (record.id === classid.value) {
        classid.value = ''
        clsuserData.value = []
        pagination.value.total = 0
      } else {
        classid.value = record.id
        userqurety()
      }
    }
  }
}
const userInput = ref()
const userqurety = () => {
  useGetUserByClassIdAsync({
    isClassUser: true,
    classId: classid.value,
    oraganizeid: null,
    userName: userInput.value,
    loginName: '',
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }).then((data) => {
    if (data.code === 200 && data.success) {
      clsuserData.value = data.data
      pagination.value.total = data.total
    } else {
      message.error(data.message)
    }
  })
}
const clsusercolumns = [
  {
    title: '序号',
    dataIndex: 'id',
    width: 100,
    align: 'center',
    fixed: 'left',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '用户名称',
    dataIndex: 'realName',
    align: 'center',
    fixed: 'left'
  },
  {
    title: '用户性别',
    align: 'center',
    dataIndex: 'sex'
  },
  {
    title: '用户账号',
    align: 'center',
    dataIndex: 'loginName'
  },
  {
    title: '用户状态',
    align: 'center',
    dataIndex: 'enabled'
  },
  {
    title: '直属部门',
    align: 'center',
    dataIndex: 'organizationName'
  },
  {
    title: '用户邮箱',
    align: 'center',
    dataIndex: 'email'
  },
  {
    title: '用户手机',
    align: 'center',
    dataIndex: 'phone'
  },
  {
    title: '用户微信',
    align: 'center',
    dataIndex: 'webChat'
  },
  {
    title: '备注',
    align: 'center',
    dataIndex: 'remark'
  }
]
const clsuserData = ref([])
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  userqurety()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  userqurety()
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

const userkey = ref([])
const userrowSelection = ref({
  selectedRowKeys: userkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    userkey.value = selectedRowKeys
  }
})

const notuserkey = ref([])
const notuserrowSelection = ref({
  selectedRowKeys: notuserkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    notuserkey.value = selectedRowKeys
  }
})
const addnotuser = () => {
  if (notuserkey.value.length === 0) {
    message.warn('请勾选人员')
    return
  }
  useInsertSysShiftClassUserAsync({ classId: classid.value, userIds: notuserkey.value }).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      useropen.value = false
      notuserkey.value = []
      userqurety()
    } else {
      message.error(data.message)
    }
  })
}

const useropen = ref(false)
const usertitle = ref('')
const userClose = () => {
  useropen.value = false
}
const adduser = () => {
  useropen.value = true
  usertitle.value = '选择用户'
  queryuser()
  useGetOraganizeTreeAsync().then((data) => {
    if (data.code === 200 && data.success) {
      treeData.value = []
      data.data.forEach((or) => {
        treeData.value.push(convertTree(or))
      })
    } else {
      message.error(data.message)
    }
  })
}
const deluser = () => {
  if (userkey.value.length === 0) {
    message.warning('请先勾选数据！')
    return
  }
  useDeletSysShiftClassUserAsync({ classid: classid.value, userId: userkey.value }).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      userqurety()
      userkey.value = []
    } else {
      message.error(data.message)
    }
  })
}

//**************************************** */
const alluserdata = ref([])
const SHOW_PARENT = TreeSelect.SHOW_ALL
const treeData = ref([])
const selectedDpt = ref([])
const nameInput = ref('')
const queryuser = () => {
  useGetUserByClassIdAsync({
    isClassUser: false,
    classId: classid.value,
    organizeId: selectedDpt.value,
    userName: nameInput.value,
    pageIndex: userpagination.value.current,
    pageSize: userpagination.value.pageSize
  }).then((data) => {
    if (data.code === 200 && data.success) {
      alluserdata.value = data.data
      userpagination.value.total = data.total
    } else {
      message.error(data.message)
    }
  })
}
const resetqueryuser = () => {
  selectedDpt.value = []
  nameInput.value = ''
  userpagination.value.current = 1
  userpagination.value.pageSize = 10
  queryuser()
}
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    label: data.name,
    value: data.id,
    children: []
  }
  if (data.oraganizeTrees && data.oraganizeTrees.length) {
    data.oraganizeTrees.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}
const userhandlePageChange = (newPage) => {
  userpagination.value.current = newPage
  queryuser()
}
const userhandlePageSizeChange = (newPageSize, currentPage) => {
  userpagination.value.current = newPageSize
  userpagination.value.pageSize = currentPage
  queryuser()
}
const userpagination = ref({
  position: ['bottomRight'],
  current: 1, // 当前页数
  pageSize: 10, // 每页条数
  total: 0, // 数据总数
  showSizeChanger: true, // 是否可以改变每页条数
  showQuickJumper: true, // 是否可以快速跳转至某页
  onChange: userhandlePageChange, // 页码改变的回调
  onShowSizeChange: userhandlePageSizeChange, // 每页条数改变的回调
  showTotal: (total) => `共 ${total} 条数据`,
  pageSizeOptions: ['10', '20', '30', '40', '50']
})
</script>
<style scoped>
.all {
  position: relative;
  height: 100%;
  width: 100%;
}

.ant-table-body {
  height: 200px;
}

.header {
  display: flex;
}

.title {
  width: 100%;
  font-size: 20px;
  font-weight: 600;
  height: 35px;
  line-height: 35px;
  padding-left: 16px;
}

.sysshiftbtn {
  display: flex;
  height: 35px;
  margin: 3px;
}

.shift {
  height: calc((100vh - 135px) * 0.4);
  width: 100%;
  position: absolute;
}

.class {
  height: calc((100vh - 135px) * 0.6);
  width: 50%;
  top: calc((100vh - 135px) * 0.4);
  left: 0;
  position: absolute;
}

.user {
  height: calc((100vh - 135px) * 0.6);
  width: 50%;
  top: calc((100vh - 135px) * 0.4);
  right: 0;
  position: absolute;
}

.fenye {
  position: absolute;
  bottom: 5px;
  right: 5px;
  height: 33px;
}
</style>
