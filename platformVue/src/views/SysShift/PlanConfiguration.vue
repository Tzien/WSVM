<template>
  <div class="all">
    <div class="msg">
      <div class="title">排班方案信息</div>
      <div class="configbtn">
        <a-button v-if="btnObj.addProgram.isShow" @click="addmsg()" style="margin: 2px" type="primary" :icon="showIcon(btnObj.addProgram.Icon)">{{
          btnObj.addProgram.name
        }}</a-button>
        <a-button v-if="btnObj.delProgram.isShow" @click="delmsg()" style="margin: 2px" type="primary" danger :icon="showIcon(btnObj.delProgram.Icon)">{{
          btnObj.delProgram.name
        }}</a-button>
      </div>
      <a-table
        :customRow="msgCustomRow"
        :scroll="{ x: 500, y: 'calc(100vh - 330px)' }"
        :row-selection="msgrowSelection"
        :columns="msgcolumns"
        :data-source="msgData"
        :pagination="false"
      >
        <template #bodyCell="{ column, text, record }">
          <template v-if="column.dataIndex === 'operation'">
            <a-button
              v-if="btnObj.uptProgram.isShow"
              :icon="showIcon(btnObj.uptProgram.Icon)"
              style="display: inline"
              @click.stop="editmsg(record.key)"
            ></a-button>
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
    <div class="config">
      <div class="title">排班方案配置</div>
      <div class="configbtn" v-if="msgrow.msgid != ''">
        <a-button v-if="btnObj.addCfg.isShow" @click="addconfig" style="margin: 2px" type="primary" :icon="showIcon(btnObj.addCfg.Icon)">{{
          btnObj.addCfg.name
        }}</a-button>
        <a-button v-if="btnObj.delCfg.isShow" @click="delconfig" style="margin: 2px" type="primary" danger :icon="showIcon(btnObj.delCfg.Icon)">{{
          btnObj.delCfg.name
        }}</a-button>
      </div>
      <a-table
        :row-selection="configrowSelection"
        :scroll="{ x: 1900, y: 'calc(100vh - 230px)' }"
        :columns="configcolumns"
        :data-source="configData"
        :pagination="false"
      >
        <template #bodyCell="{ column, text, record }">
          <template v-if="column.dataIndex === 'operation'">
            <a-button v-if="btnObj.uptCfg.isShow" :icon="showIcon(btnObj.uptCfg.Icon)" style="display: inline" @click="editconfig(record.key)"></a-button>
          </template>
        </template>
      </a-table>
    </div>
  </div>
  <a-drawer :title="msgtitle" size="large" :closable="false" :open="msgopen" @close="msgClose">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-item label="分厂/工序" v-bind="msgForm.validateInfos.groupId">
        <div>
          <a-select v-model:value="msgModel.groupId" show-search placeholder="请选择" style="width: 100%" :options="msgoptions"></a-select>
        </div>
      </a-form-item>
      <a-form-item label="信息名称" v-bind="msgForm.validateInfos.moduleName">
        <a-input v-model:value="msgModel.moduleName" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="备注" v-bind="msgForm.validateInfos.remark">
        <a-input v-model:value="msgModel.remark" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="排序" v-bind="msgForm.validateInfos.sort">
        <a-input v-model:value="msgModel.sort" ref="select" style="width: 100%" />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button type="primary" @click="saveMsg" style="margin-right: 50px">保存</a-button>
        <a-button @click="configClose">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
  <a-drawer :title="configtitle" size="large" :closable="false" :open="configopen" @close="configClose">
    <a-form :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-item label="分厂/工序">
        <div>
          {{ msgrow.factoryName }}
        </div>
      </a-form-item>
      <a-form-item label="班次方案">
        <div>
          {{ msgrow.moduleName }}
        </div>
      </a-form-item>
      <a-form-item label="班次" v-bind="configForm.validateInfos.shiftId">
        <div>
          <a-select v-model:value="configModel.shiftId" show-search placeholder="请选择" style="width: 100%" :options="shiftoptions"></a-select>
        </div>
      </a-form-item>
      <a-form-item label="班组" v-bind="configForm.validateInfos.classId">
        <div>
          <a-select v-model:value="configModel.classId" show-search placeholder="请选择" style="width: 100%" :options="classoptions"></a-select>
        </div>
      </a-form-item>
      <a-form-item label="开始时间" v-bind="configForm.validateInfos.startTime">
        <a-date-picker v-model:value="startShowtime" show-time placeholder="开始时间" style="width: 100%" @change="startChange" />
      </a-form-item>
      <a-form-item label="结束时间" v-bind="configForm.validateInfos.endTime">
        <a-date-picker v-model:value="endShowtime" show-time placeholder="结束时间" style="width: 100%" @change="configChange" />
      </a-form-item>
      <a-form-item label="统计日期" v-bind="configForm.validateInfos.totalDate">
        <a-date-picker v-model:value="totalShowDate" placeholder="统计日期" style="width: 100%" @change="totalChange" />
      </a-form-item>
      <a-form-item label="备注" v-bind="configForm.validateInfos.remark">
        <a-input v-model:value="configModel.remark" ref="select" style="width: 100%" />
      </a-form-item>
      <a-form-item label="排序" v-bind="configForm.validateInfos.sort">
        <a-input v-model:value="configModel.sort" ref="select" style="width: 100%" />
      </a-form-item>
      <div style="margin-top: 10px; text-align: center; width: 100%">
        <a-button type="primary" @click="saveConfig" style="margin-right: 50px">保存</a-button>
        <a-button @click="configClose">返回</a-button>
      </div>
    </a-form>
  </a-drawer>
</template>
<script setup>
import { reactive, ref, h, watch, watchEffect, onMounted } from 'vue'
import { Form } from 'ant-design-vue'
import { message } from 'ant-design-vue'
import { useGetFactorySelect } from '@/api/BaseInfoConfig/factoryinfo'
import {
  useGetSysShiftProgramAsync,
  useSoftSysShiftProgramAsync,
  useInsertSysShiftProgramAsync,
  useUpdateSysShiftProgramAsync,
  useGetSysShiftProgramByIdAsync,
  useGetProgramCfgByProgramIdAsync,
  useGetShiftSelectAsync,
  useGetShiftClassSelectAsync,
  useInsertSysShiftProgramCfgAsync,
  useUpdateSysShiftProgramCfgAsync,
  useGetProgramCfgByIdAsync,
  useSoftSysShiftProgramCfgAsync
} from '@/api/SysShift'
import { useGetOraganizeTreeAsync } from '@/api/BaseInfoConfig/organization'
import { TreeSelect } from 'ant-design-vue'
import dayjs from 'dayjs'
import { PlusOutlined, SaveOutlined, SearchOutlined, EditOutlined, CloseOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import * as Icons from '@ant-design/icons-vue'
import { getButtonList } from '../../api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
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

const btnObj = reactive({
  //新增排班方案
  addProgram: { isShow: false, btnCode: 'A060201' },
  //删除选中方案
  delProgram: { isShow: false, btnCode: 'A060203' },
  //修改排班方案
  uptProgram: { isShow: false, btnCode: 'A060202' },
  //新增方案配置
  addCfg: { isShow: false, btnCode: 'A060206' },
  //删除选中配置
  delCfg: { isShow: false, btnCode: 'A060204' },
  //修改方案配置
  uptCfg: { isShow: false, btnCode: 'A060205' }
})

getButtonList({ menuCode: 'A0602', roleids: userStore.value.userRoles }).then((data) => {
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

const useForm = Form.useForm
//{ validate, validateInfos, resetFields }
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
//************************************排版方案信息********************************************* */
const msgcolumns = [
  {
    title: '序号',
    dataIndex: 'key',
    width: 100,
    align: 'center',
    fixed: 'left',
    customRender: (obj) => {
      return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
    }
  },
  {
    title: '分厂/工序',
    align: 'center',
    dataIndex: 'factoryName'
  },
  {
    title: '方案名称',
    align: 'center',
    watch: 200,
    dataIndex: 'moduleName'
  },
  {
    title: '备注',
    align: 'center',
    dataIndex: 'remark'
  },
  {
    title: '排序',
    align: 'center',
    dataIndex: 'sort'
  },
  {
    title: '操作',
    width: 90,
    align: 'center',
    fixed: 'right',
    dataIndex: 'operation'
  }
]
const msgModel = ref({
  id: '',
  moduleName: '',
  remark: '',
  sort: 1,
  groupId: ''
})
const msgRules = ref({
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
const msgForm = useForm(msgModel, msgRules)

const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  querymsg()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  querymsg()
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
const msgData = ref([])
const querymsg = () => {
  useGetSysShiftProgramAsync({ moduleName: '', pageIndex: pagination.value.current, pageSize: pagination.value.pageSize }).then((data) => {
    if (data.code === 200 && data.success) {
      msgData.value = data.data
      pagination.value.total = data.total
    } else {
      message.error(data.message)
    }
  })
}
querymsg()
const msgopen = ref(false)
const msgtitle = ref('')
const msgClose = () => {
  msgopen.value = false
}
const addmsg = () => {
  msgForm.resetFields()
  msgtitle.value = '添加排班方案信息'
  msgopen.value = true
  factorySelect()
}
const editmsg = (id) => {
  msgForm.resetFields()
  msgtitle.value = '修改班组信息'
  msgopen.value = true
  useGetSysShiftProgramByIdAsync(id).then((data) => {
    if (data.code === 200 && data.success) {
      msgModel.value = data.data
    } else {
      message.error(data.message)
    }
  })
  factorySelect()
}

const msgkey = ref([])
const msgrowSelection = ref({
  selectedRowKeys: msgkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    msgkey.value = selectedRowKeys
  }
})
const delmsg = () => {
  if (msgkey.value.length === 0) {
    message.warning('未勾选数据')
    return
  }
  useSoftSysShiftProgramAsync(msgkey.value).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      msgkey.value = []
      querymsg()
    } else {
      message.error(data.message)
    }
  })
}
const saveMsg = () => {
  const isAdding = msgtitle.value === '添加排班方案信息'
  const apiCall = isAdding ? useInsertSysShiftProgramAsync : useUpdateSysShiftProgramAsync
  msgForm
    .validate()
    .then(() => {
      apiCall(msgModel.value).then((data) => {
        if (data.code === 200 && data.success) {
          message.success(data.message)
          msgopen.value = false
          querymsg()
          msgForm.resetFields()
        } else {
          message.error(data.message)
        }
      })
    })
    .catch(() => {
      //console.log('error', err)
    })
}

const msgoptions = ref([])
const factorySelect = () => {
  useGetFactorySelect().then((data) => {
    if (data.code === 200 && data.success) {
      msgoptions.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
//************************************配置********************************************* */
const msgrow = reactive({
  msgid: '',
  factoryName: '',
  moduleName: ''
})
const msgCustomRow = (record, index) => {
  return {
    onClick: (e) => {
      if (record.key === msgrow.msgid) {
        msgrow.msgid = ''
        msgrow.factoryName = ''
        msgrow.moduleName = ''
        configData.value = []
      } else {
        msgrow.msgid = record.key
        msgrow.factoryName = record.factoryName
        msgrow.moduleName = record.moduleName
        configqurety()
      }
    }
  }
}
const configqurety = () => {
  useGetProgramCfgByProgramIdAsync(msgrow.msgid).then((data) => {
    if (data.code === 200 && data.success) {
      configData.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
const configcolumns = [
  {
    title: '序号',
    dataIndex: 'key',
    width: 100,
    align: 'center',
    fixed: 'left',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '分厂/工序',
    dataIndex: 'factoryName',
    align: 'center',
    fixed: 'left'
  },
  {
    title: '班次方案',
    align: 'center',
    dataIndex: 'shiftProgramName'
  },
  {
    title: '班组',
    align: 'center',
    dataIndex: 'shiftName'
  },
  {
    title: '班次',
    align: 'center',
    dataIndex: 'className'
  },
  {
    title: '开始时间',
    align: 'center',
    dataIndex: 'startTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '结束时间',
    align: 'center',
    dataIndex: 'endTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '统计日期',
    align: 'center',
    dataIndex: 'totalDate',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.split('T')[0]
      }
      return ''
    }
  },
  {
    title: '排序',
    align: 'center',
    dataIndex: 'sort'
  },
  {
    title: '备注',
    align: 'center',
    dataIndex: 'remark'
  },
  {
    title: '操作',
    width: 90,
    fixed: 'right',
    dataIndex: 'operation'
  }
]

const shiftSelect = () => {
  useGetShiftSelectAsync().then((data) => {
    if (data.code === 200 && data.success) {
      shiftoptions.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
const classSelect = () => {
  useGetShiftClassSelectAsync().then((data) => {
    if (data.code === 200 && data.success) {
      classoptions.value = data.data
    } else {
      message.error(data.message)
    }
  })
}

const startShowtime = ref(null)
const endShowtime = ref(null)
const totalShowDate = ref(null)
watch(
  () => startShowtime.value,
  (newValue) => {
    if (newValue === null) {
      configModel.value.startTime == ''
    }
  }
)
watch(
  () => endShowtime.value,
  (newValue) => {
    if (newValue === null) {
      configModel.value.endTime == ''
    }
  }
)
watch(
  () => totalShowDate.value,
  (newValue) => {
    if (newValue === null) {
      configModel.value.totalDate == ''
    }
  }
)

const startChange = (value, dateString) => {
  configModel.value.startTime = new Date(dateString + 'Z').toISOString()
}
const configChange = (value, dateString) => {
  configModel.value.endTime = new Date(dateString + 'Z').toISOString()
}
const totalChange = (value, dateString) => {
  configModel.value.totalDate = new Date(dateString + 'Z').toISOString()
}

const configData = ref([])
const configkey = ref([])
const configrowSelection = ref({
  selectedRowKeys: configkey,
  type: 'checkbox',
  onChange: (selectedRowKeys, selectedRows) => {
    configkey.value = selectedRowKeys
  }
})

const configopen = ref(false)
const configtitle = ref('')
const configClose = () => {
  configopen.value = false
  configresetFields()
}
const addconfig = () => {
  configopen.value = true
  configtitle.value = '添加排班方案配置'
  shiftSelect()
  classSelect()
}

const editconfig = (id) => {
  configForm.resetFields()
  configtitle.value = '修改排班方案配置'
  configopen.value = true
  shiftSelect()
  classSelect()
  useGetProgramCfgByIdAsync(id).then((data) => {
    if (data.code === 200 && data.success) {
      configModel.value = data.data
      startShowtime.value = dayjs(configModel.value.startTime)
      endShowtime.value = dayjs(configModel.value.endTime)
      totalShowDate.value = dayjs(configModel.value.totalDate)
    } else {
      message.error(data.message)
    }
  })
}

const saveConfig = () => {
  configModel.value.shiftProgramId = msgrow.msgid
  const isAdding = configtitle.value === '添加排班方案配置'
  const apiCall = isAdding ? useInsertSysShiftProgramCfgAsync : useUpdateSysShiftProgramCfgAsync
  configForm
    .validate()
    .then(() => {
      apiCall(configModel.value).then((data) => {
        if (data.code === 200 && data.success) {
          message.success(data.message)
          configopen.value = false
          configqurety()
          configresetFields()
        } else {
          message.error(data.message)
        }
      })
    })
    .catch(() => {
      //console.log('error', err)
    })
}

const delconfig = () => {
  if (configkey.value.length === 0) {
    message.warning('请先勾选数据！')
    return
  }
  useSoftSysShiftProgramCfgAsync(configkey.value).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      configqurety()
    } else {
      message.error(data.message)
    }
  })
}

const shiftoptions = ref([])
const classoptions = ref([])

const configModel = ref({
  id: '',
  startTime: '',
  endTime: '',
  totalDate: '',
  remark: '',
  sort: '',
  shiftProgramId: '',
  shiftId: '',
  classId: ''
})
const configRules = ref({
  shiftId: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择班次'))
        }
        return Promise.resolve()
      }
    }
  ],
  classId: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择班组'))
        }
        return Promise.resolve()
      }
    }
  ],
  startTime: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择开始时间'))
        }
        if (configModel.value.endTime != '' && value >= configModel.value.endTime) {
          return Promise.reject(new Error('开始时间不能大于结束时间'))
        }
        return Promise.resolve()
      }
    }
  ],
  endTime: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择结束时间'))
        }
        if (configModel.value.startTime != '' && value <= configModel.value.startTime) {
          return Promise.reject(new Error('结束时间不能小于开始时间'))
        }
        return Promise.resolve()
      }
    }
  ],
  totalDate: [
    {
      required: true,
      validator: (rule, value) => {
        if (value === '') {
          return Promise.reject(new Error('请选择统计日期'))
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
const configForm = useForm(configModel, configRules)

const configresetFields = () => {
  configForm.resetFields()
  startShowtime.value = null
  endShowtime.value = null
  totalShowDate.value = null
}
</script>
<style scoped>
.all {
  position: relative;
  height: 100%;
  width: 100%;
}

.title {
  width: 100%;
  font-size: 20px;
  font-weight: 600;
  height: 35px;
  line-height: 35px;
  padding-left: 16px;
}

.configbtn {
  height: 35px;
}

.msg {
  height: calc((100vh - 135px));
  width: 45%;
  top: 0;
  left: 0;
  position: absolute;
}

.config {
  height: calc((100vh - 135px));
  width: 55%;
  top: 0;
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
