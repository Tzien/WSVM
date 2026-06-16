<!-- 岗位信息配置 -->
<template>
  <div class="JobInformationConfiguration">
    <div style="height: 40px; line-height: 40px; margin: 0px 0px 0px 20px; text-align: right">
      <span>
        岗位名称：
        <a-input v-model:value="jobNameKey" placeholder="请输入岗位名称" style="width: 150px" />
      </span>
      <span style="margin-left: 10px">
        岗位编码：
        <a-input v-model:value="jobCodeKey" placeholder="请输入岗位编码" style="width: 150px" />
      </span>
      <span style="margin:0 10px">
        录入人员：
        <a-select v-model:value="inputPersonnelValue" show-search allowClear placeholder="请输入录入人员"
          style="width: 150px; text-align: left" :options="filteredOptions" :filter-option="false"
          @search="handleSearch" @change="handleChange"></a-select>
        <!--  :filter-option="filterInputPerson"   -->
      </span>
      <CustomButtonList :ParamsRoleId="inputRoleId" :ParamsFunctionName="inputFunctionName"
        :BtnFunctionNameArray="['A010301', 'A010302', 'A010303', 'A010305', 'A010307']"
        @buttonClick="handleButtonClick"></CustomButtonList>

      <!-- <a-button style="margin-left: 20px" :icon="h(SearchOutlined)" @click="getTableData(true)">查询数据</a-button>
      <a-button style="margin-left: 10px" :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      <a-button style="margin-left: 10px" type="primary" :icon="h(PlusOutlined)" @click="showModal">新增岗位</a-button>
      <a-button style="margin-left: 10px" :icon="h(ArrowsAltOutlined)" @click="expand">展开全部</a-button>
      <a-button style="margin-left: 10px" :icon="h(ShrinkOutlined)" @click="expandClose">折叠全部</a-button> -->
    </div>
    <a-table :expandIconColumnIndex="3" :indentSize="10" :columns="treeColumns" :data-source="treeData"
      :row-selection="rowSelection" :pagination="false"
      :scroll="{ y: 'calc(100vh - 96px - 40px - 55px - 35px - 35px)' }" :expandedRowKeys="expandedRowKeys"
      @expandedRowsChange="expandedRowsChange">
      <!-- 全屏幕减去 系统菜单栏  Tab栏 搜索框 下边距5px table表头 底部footer 合计行-->
      <template #expandIcon="props">
        <span v-if="props.record.children && props.record.children.length > 0">
          <div v-if="props.expanded" style="display: inline-block; margin-right: 10px" @click="
            (e) => {
              props.onExpand(props.record, e)
            }
          ">
            <CaretDownOutlined />
          </div>
          <div v-else style="display: inline-block; margin-right: 10px" @click="
            (e) => {
              props.onExpand(props.record, e)
            }
          ">
            <CaretRightOutlined />
          </div>
        </span>
        <span v-else style="margin-right: 15px"></span>
      </template>
      <template #bodyCell="{ text, column, record }">
        <template v-if="column.dataIndex === 'modifyTime' || column.dataIndex === 'createTime'">
          {{ formatDateTime(text) }}
        </template>
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="detail(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010304')">
            <CustomButtonListTable :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010304')"
              :IsOnlyIcon="true" style="display: inline; margin-right: 10px"></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010306')">
            <CustomButtonListTable :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010306')"
              :IsOnlyIcon="true" style="display: inline; margin-right: 10px"></CustomButtonListTable>
          </a>
        </template>
      </template>
    </a-table>
    <div class="totalSum">
      <a-divider style="background-color: rgb(238, 238, 238)" />
      <div class="sumtext">全部显示，共{{ totalCount }}条</div>
    </div>

    <div class="modalStyle">
      <a-drawer :title="modalState.modalTitle" size='large' :open="openModal" :body-style="{ paddingBottom: '80px' }"
        :footer-style="{ textAlign: 'right' }" @close="cancelModal">
        <a-form ref="formRef" :model="formState" :label-col="labelCol">
          <a-form-item label="父级岗位名称" name="pname">
            <a-input disabled v-model:value="formState.pname" />
          </a-form-item>
          <a-form-item label="岗位名称" name="name" :rules="[
            { required: true, message: '请输入岗位名称!' },
            { validator: validateName, trigger: 'blur' }
          ]" validateTrigger="blur">
            <a-input v-model:value="formState.name" />
          </a-form-item>
          <a-form-item label="岗位编码" name="code" :rules="[
            { required: true, message: '请输入岗位编码!' },
            { validator: validateCode, trigger: 'blur' }
          ]" validateTrigger="blur">
            <a-input v-model:value="formState.code" />
          </a-form-item>
          <a-form-item label="排序" name="sort">
            <a-input-number id="inputNumber" v-model:value="formState.sort" :min="0" :max="9999999"
              style="width: 100%" />
          </a-form-item>
          <a-form-item label="备注" name="remark">
            <a-textarea v-model:value="formState.remark" placeholder="请输入备注" :auto-size="{ minRows: 2, maxRows: 3 }" />
          </a-form-item>
          <div style="margin-top: 10px; text-align: center; width: 100%">
            <!-- 保存 -->
            <a-button type="primary" class="NewAddBtnBG" @click="save(1)" style="margin-right: 50px">
              <template #icon>
                <SaveOutlined />
              </template>
              {{ $t('message.drawer.Save') }}
            </a-button>
            <!-- 保存并关闭 -->
            <a-button type="primary" class="NewAddBtnBG" @click="save(2)">
              <template #icon>
                <SaveOutlined />
              </template>
              {{ $t('message.drawer.SaveAndClose') }}
            </a-button>
          </div>
        </a-form>
      </a-drawer>
    </div>
  </div>
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { CaretDownOutlined, SaveOutlined, CaretRightOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import { ref, reactive, onMounted, createVNode, computed, watchEffect, defineOptions, nextTick, watch } from 'vue'
import { getJobInfo, addSysJob, getJobInfoById, deleteApi, modifySysJob } from '../../api/sysJob'
import { getUserInfo, getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import { debounce } from 'lodash-es'
import { formatDateTime } from '../../utils/dateUtils'
import { useUserStore } from '../../store/user'
import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useI18n } from 'vue-i18n'
import { loadPageLocaleMessages } from '../../lang/i18n'
const i18n = useI18n({ useScope: 'global' })
// import { guid } from '@/utils/guid.js'
const { globalStore } = useGlobalState()
var userStore = ref({})
/* 页面缓存 */
defineOptions({
  name: 'A0103'
})

const labelCol = {
  style: {
    width: '100px'
  }
}
/* 动态按钮相关 */
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
const inputFunctionName = ref('A0103')
const loadCurrentPageI18n = async (locale) => {
  await loadPageLocaleMessages(i18n, locale, inputFunctionName.value)
}
const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  // 你可以在这里调用相应的方法
  if (functionName === 'getTableData(true)') {
    getTableData(true)
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
  if (functionName === 'showModal') {
    showModal()
  }
  if (functionName === 'expand') {
    expand()
  }
  if (functionName === 'expandClose(true)') {
    expandClose(true)
  }
}

//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0103',
    roleids: inputRoleId,
    btnCodes: []
  }
  await getButtonList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      allDyBtn.value = res.data.buttonDtos
    }
  })
}

const treeColumns = computed(() => {
  const _ = i18n.locale.value
  return [
    {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    customRender: (obj) => {
      return obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: i18n.t('A0103.JobNameI18n'),
    dataIndex: 'name',
    key: 'name'
  },
  {
    title: '岗位编码',
    dataIndex: 'code',
    key: 'code'
  },
  {
    title: i18n.t('A0103.JobRemarkI18n'),
    dataIndex: 'remark',
    key: 'remark'
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort'
  },
  {
    title: '录入时间',
    dataIndex: 'createTime',
    key: 'createTime'
  },
  {
    title: '录入人员',
    dataIndex: 'createName',
    key: 'createName'
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime'
  },
  {
    title: '修改人员',
    dataIndex: 'modifyName',
    key: 'modifyName'
  },
  {
    title: '操作',
    width: 160,
    dataIndex: 'operation',
    // fixed: 'right',
    align: 'center'
    }
  ]
})
var treeData = ref([])
var totalCount = ref(0)
var selectedRow = ref(null)
const rowSelection = {
  onChange: (selectedRowKeys, selectedRows) => {
    selectedRow.value = selectedRows[selectedRows.length - 1]
  },
  onSelect: (record, selected, selectedRows) => {
    if (selectedRows.length > 0) {
      selectedRow.value = selectedRows[selectedRows.length - 1]
    }
  },
  getCheckboxProps: (record) => {
      
      return {
        disabled: record.isReadonly
      }
    },
  onSelectAll: (selected, selectedRows, changeRows) => { },
  columnTitle: ' '
}

var modalState = reactive({
  isAdd: true,
  modalTitle: '新增岗位'
})
const jobNameKey = ref('')
const jobCodeKey = ref('')

/* 下拉选择器 */
const inputPersonnelValue = ref(null)
const searchQuery = ref('')
const sysUserOptions = ref([])
const filteredOptions = computed(() => {
  if (searchQuery.value) {
    return sysUserOptions.value.filter((option) => option.label.toLowerCase().includes(searchQuery.value.toLowerCase()))
  }
  return sysUserOptions.value
})

const handleSearch = debounce((value) => {
  searchQuery.value = value
}, 500)
const handleChange = (value) => {
  inputPersonnelValue.value = value
}

/* 表单校验 */
const validateName = async (rule, value) => {
  if (!value) {
    return Promise.resolve()
  }
  if (value.length > 255) {
    return Promise.reject('岗位名称太长!')
  }
  return Promise.resolve()
}
const validateCode = async (rule, value) => {
  if (!value) {
    return Promise.resolve()
  }
  if (value.length > 255) {
    return Promise.reject('岗位编码太长!')
  }
  // 正则表达式检查，例如只允许字母和数字
  if (!/^[a-zA-Z0-9]+$/.test(value)) {
    return Promise.reject('岗位编码只能包含字母和数字!')
  }
  return Promise.resolve()
}

const expandedRowKeys = ref([])
// 全部展开
const expand = () => {
  expandedRowKeys.value = []
  dg2(treeData.value)
}
function dg2(obj) {
  obj.forEach((e) => {
    if (e.children && e.children.length > 0) {
      expandedRowKeys.value.push(e.key)
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
getTableData(true)
getUserList()

onMounted(() => {
  getAllButton()
})

watch(
  () => i18n.locale.value,
  (locale) => {
    loadCurrentPageI18n(locale)
  },
  { immediate: true }
)

async function getUserList() {
  await getUserInfo().then((res) => {
    if (res.code == 200 && res.success) {
      sysUserOptions.value = []
      res.data.forEach((element) => {
        sysUserOptions.value.push({ value: element.id, label: element.realName })
      })
    }
  })
}

/* 弹窗相关 */
const openModal = ref(false)
function cancelModal() {
  safeResetFields()
  openModal.value = false
}
var formState = reactive({
  pname: '',
  pid: null,
  name: '',
  code: '',
  sort: 0,
  remark: ''
})
//获取表单dom
const formRef = ref(null)

const showModal = async () => {
  openModal.value = true
  modalState.modalTitle = '新增岗位'
  modalState.isAdd = true
  if (formRef.value) {
    safeResetFields()
  }
  await nextTick()
  if (selectedRow.value && selectedRow.value?.id != '') {
    formState.pid = selectedRow.value.id
    formState.pname = selectedRow.value.name
  }
}
function resetSearch() {
  jobNameKey.value = ''
  jobCodeKey.value = ''
  inputPersonnelValue.value = null
}

async function getTableData(isSerch) {
  var obj = {
    userid: inputPersonnelValue.value,
    name: jobNameKey.value,
    code: jobCodeKey.value
  }
  await getJobInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      treeData.value = res.data
      totalCount.value = res.total
      if (isSerch) {
        message.success(res.message)
      }
    }
  })
}

async function safeResetFields() {
  await nextTick()
  formRef.value?.resetFields()
}

function save(type) {
  formRef.value.validate().then(() => {
    // 执行保存操作
    saveDetail(type)
  })
}
async function saveDetail(type) {
  // const testParams = guid()
  // formState.guid = testParams
  if (modalState.isAdd) {
    await addSysJob(formState).then((res) => {
      if (res.code == 200 && res.success) {
        message.success('岗位添加成功!')
        if (type == 2) {
          openModal.value = false
          Object.assign(formState, {
            pname: '',
            pid: '0',
            name: '',
            code: '',
            sort: 0,
            remark: ''
          })
        }
        getTableData(false)
      } else {
        message.error(res.message)
      }
    })
  } else {
    await modifySysJob(formState).then((res) => {
      if (res.code == 200 && res.success) {
        message.success('岗位信息已更新!')
        if (type == 2) {
          openModal.value = false
          // formState = reactive({
          //   pname: '',
          //   pid: '0',
          //   name: '',
          //   code: '',
          //   sort: 0,
          //   remark: ''
          // })
        }
        getTableData(false)
      } else {
        message.error(res.message)
      }
    })
  }
  if (formRef.value) {
    safeResetFields()
  }
}

async function detail(id) {
  modalState.modalTitle = '编辑岗位'
  modalState.isAdd = false
  await getJobInfoById({ id: id }).then((res) => {
    if (res.code == 200 && res.success) {
      // formState.id = id
      // formState.pname = res.data.pname
      // formState.pid = res.data.pid
      // formState.name = res.data.name
      // formState.code = res.data.code
      // formState.sort = res.data.sort
      // formState.remark = res.data.remark
      // formState.createId = res.data.createId
      // formState.createName = res.data.createName
      // formState.createTime = res.data.createTime
      // formState.isDeleted = res.data.isDeleted

      Object.assign(formState, res.data) // 保持响应性

      openModal.value = true
    }
  })
}

//删除
function onDelete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该岗位吗？',
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
async function del(resolve, reject, id) {
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
</script>

<style lang="scss">
.JobInformationConfiguration {
  background-color: #fff;
  height: 100%;
  position: relative;

  .totalSum {
    text-align: right;
    position: absolute;
    right: 0px;
    bottom: 0px;
    width: 100%;

    .ant-divider {
      margin: 0px;
    }

    .sumtext {
      padding-right: 10px;
      height: 35px;
      line-height: 35px;
    }
  }

  .ant-table-container {
    width: 100%;
  }

  .ant-table-body {
    overflow-y: auto !important;
  }

  .modalStyle {
    ::v-deep :where(.css-dev-only-do-not-override-1hsjdkk).ant-divider-horizontal {
      margin: 0px !important;
    }
  }
}
</style>
