<template>
  <div class="subPage">
    <div class="header">
      <a-input allowClear class="filterBox" v-model:value="projectNameValue" placeholder="项目名称" />
      <a-select
        allowClear
        class="filterBox"
        v-model:value="categoryIdValue"
        show-search
        placeholder="产品大类"
        :options="categoryOptions"
        :filter-option="categoryFilterOptions"
      ></a-select>
      <a-select
        allowClear
        class="filterBox"
        v-model:value="projectTypeValue"
        show-search
        placeholder="项目类型"
        :options="projectTypeOptions"
        :filter-option="projectTypeFilterOptions"
      ></a-select>
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="项目所在省份" :options="options"></a-select>
      <a-input allowClear class="filterBox" v-model:value="nameObj" placeholder="业主名称" />
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="集团名称" :options="options"></a-select>
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="产品细分" :options="options"></a-select>
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="营销区域" :options="options"></a-select>
      <a-input allowClear class="filterBox" v-model:value="nameObj" placeholder="营销经理" />
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="来源" :options="options"></a-select>
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="重点项目" :options="options"></a-select>
      <a-select allowClear class="filterBox" v-model:value="value" show-search placeholder="项目状态" :options="options"></a-select>
      <a-button allowClear class="filterBtn" :icon="h(SearchOutlined)" type="primary" @click="getDataList(true)">查询</a-button>
      <a-button allowClear class="filterBtn" :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      <a-button allowClear class="filterBtn" style="background-color: rgb(113, 204, 85); color: white" :icon="h(PlusOutlined)" @click="OpenAddModal"
        >新建</a-button
      >
      <a-button allowClear class="filterBtn" style="background-color: rgb(113, 204, 85); color: white" :icon="h(UploadOutlined)" @click="ExportData"
        >导出</a-button
      >
      <a-button allowClear class="filterBtn" type="primary" :icon="h(DownloadOutlined)" @click="OpenImport">导入</a-button>
    </div>
    <div class="content">
      <div class="accordion" ref="panelRef">
        <div ref="accordionRef" class="accordion-content">
          <div>
            <div style="margin-top: 10px">
              <a-input v-model:value="value" style="width: 250px" placeholder="模糊检索" />
              <a-button type="primary" :icon="h(SearchOutlined)" style="margin-left: 10px">搜索</a-button>
            </div>
          </div>
          <div>
            <div style="margin: 20px 10px">
              <span>营销区域：</span>
              <span
                style="margin: 0px 5px"
                :class="{ active: selectedValue === item.value }"
                class="area-span"
                v-for="item in areaOption"
                :key="item.value"
                @click="addCheckBtnCss(item.value)"
                >{{ item.label }}</span
              >
            </div>
            <div style="margin: 20px 10px">
              <span>产品大类：</span>
              <span
                style="margin: 0px 5px"
                :class="{ active: selectedValue === item.value }"
                class="area-span"
                v-for="item in categoryOptions"
                :key="item.value"
                @click="addCheckBtnCss(item.value)"
                >{{ item.label }}</span
              >
            </div>
            <div style="margin: 20px 10px">
              <span>项目类型：</span>
              <span
                style="margin: 0px 5px"
                :class="{ active: selectedValue === item.value }"
                class="area-span"
                v-for="item in projectTypeOptions"
                :key="item.value"
                @click="addCheckBtnCss(item.value)"
                >{{ item.label }}</span
              >
            </div>
          </div>
        </div>
        <div style="text-align: right; margin-right: 20px; margin-bottom: 10px">
          <a href="javascript:void(0);" @click="toggleAccordion">点击<span v-show="!isExpand">展开</span><span v-show="isExpand">折叠</span>面板</a>
        </div>
      </div>
      <s-table :columns="columns" :height="calcHeight" :data-source="dataSource" :pagination="false">
        <!-- (100vh - 64px - 36px - 90px - 35px - 50px) -->
        <template #bodyCell="{ text, column, record }">
          <template v-if="column.dataIndex === 'categoryId'">
            <span v-if="text === 1"> 电力工程 </span>
            <span v-if="text === 2"> 智能化工程 </span>
          </template>
          <template v-if="column.key === 'operation'">
            <a @click="openEdit(record)" style="margin-right: 20px">编辑</a>
            <a @click="onDelete(record.id)">删除</a>
          </template>
        </template>
      </s-table>
      <a-pagination
        style="height: 50px; text-align: right; line-height: 50px"
        v-model:current="currentPageIndex"
        v-model:page-size="currentPageSize"
        :total="total"
        :show-total="(total) => `Total ${total} items`"
      />
    </div>

    <!-- 新增数据弹窗 -->
    <a-modal v-model:open="openModal" title="新增项目" @ok="handleOk" cancelText="取消" okText="保存" :maskClosable="false">
      <a-form style="margin-right: 50px" ref="formRef" :model="formState" :rules="rules" :label-col="labelCol" :wrapper-col="wrapperCol">
        <a-form-item ref="name" label="项目名称" name="name">
          <a-input v-model:value="formState.name" />
        </a-form-item>
        <a-form-item ref="number" label="项目编号" name="number">
          <a-input v-model:value="formState.number" />
        </a-form-item>
        <a-form-item label="产品大类" name="categoryId">
          <a-select v-model:value="formState.categoryId" placeholder="请选择产品大类">
            <a-select-option :value="1">电力工程</a-select-option>
            <a-select-option :value="2">智能化工程</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="项目类型" name="projectType">
          <a-select v-model:value="formState.projectType" placeholder="请选择项目类型">
            <a-select-option value="E">E</a-select-option>
            <a-select-option value="P">P</a-select-option>
            <a-select-option value="P(S)">P(S)</a-select-option>
            <a-select-option value="EP">EP</a-select-option>
            <a-select-option value="PC">PC</a-select-option>
            <a-select-option value="EPC">EPC</a-select-option>
          </a-select>
        </a-form-item>
        <!-- <a-form-item :wrapper-col="{ span: 14, offset: 4 }">
          <a-button type="primary" @click="onSubmit">Create</a-button>
          <a-button style="margin-left: 10px" @click="resetForm">Reset</a-button>
        </a-form-item> -->
      </a-form>
    </a-modal>
    <a-modal v-model:open="importModal" title="数据导入" @ok="ImportData" cancelText="取消" okText="导入" :maskClosable="false">
      <a-upload-dragger
        style="margin-bottom: 15px"
        v-model:fileList="fileList"
        name="file"
        :multiple="false"
        accept=".xls,.xlsx"
        :before-upload="beforeUploadFile"
      >
        <p class="ant-upload-drag-icon">
          <inbox-outlined></inbox-outlined>
        </p>
        <p class="ant-upload-text">选择或拖拽上传文件</p>
        <!-- <p class="ant-upload-hint">Support for a single or bulk upload. Strictly prohibit from uploading company data or other band files</p> -->
      </a-upload-dragger>
      <a href="/ExcelModel/ExcelModel.xlsx" download="ExcelModel.xlsx"> 下载导入模板 </a>
    </a-modal>
  </div>
</template>
<script setup>
import { ref, onMounted, h, reactive, watch, createVNode, computed, onBeforeUnmount, nextTick } from 'vue'
import { SearchOutlined, RedoOutlined, UploadOutlined, DownloadOutlined, PlusOutlined, ExclamationCircleOutlined, InboxOutlined } from '@ant-design/icons-vue'
import { Modal, message, Upload } from 'ant-design-vue'
// import { getprojectTrackingList, saveProjectTracking, deleteProjectTrackingById, uploadFile } from '@/api/demoApi/demo'
import { guid } from '@/utils/guid.js'
import * as XLSX from 'xlsx' //导出

const panelHeight = ref(80)
const panelRef = ref(null)
const calcHeight = computed(() => {
  return `calc(100vh - ${panelHeight.value}px - 64px - 36px - 90px - 35px - 50px)`
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

const columns = ref([
  {
    title: '序号',
    width: 80,
    key: 'nonEditable',
    dataIndex: 'index',
    customRender: (obj) => {
      return (currentPageIndex.value - 1) * currentPageSize.value + obj.index + 1
    },
    fixed: 'left',
    align: 'center'
  },
  {
    title: '项目名称',
    dataIndex: 'projectName',
    key: 'projectName',
    align: 'center',
    width: 150
  },
  {
    title: '营销项目编号',
    dataIndex: 'marketingProjectNumber',
    align: 'center',
    width: 150
  },
  {
    title: '产品大类',
    dataIndex: 'categoryId',
    align: 'center',
    width: 150
  },
  {
    title: '产品细分',
    dataIndex: 'detailedClassification',
    align: 'center',
    width: 200
  },
  {
    title: '项目类型',
    dataIndex: 'projectType',
    align: 'center',
    width: 150
  },
  {
    title: '预估合同额（万元）',
    dataIndex: 'contractAmount',
    align: 'center',
    width: 200
  },
  {
    title: '乙方名称',
    dataIndex: 'nameOfPartyB',
    align: 'center',
    width: 150
  },
  {
    title: '项目所在省份',
    dataIndex: 'province',
    align: 'center',
    width: 150
  },
  {
    title: '业主名称',
    dataIndex: 'ownerName',
    align: 'center',
    width: 150
  },
  {
    title: '集团名称',
    dataIndex: 'groupName',
    align: 'center',
    width: 150
  },
  {
    title: '操作',
    align: 'center',
    key: 'operation',
    dataIndex: 'operation',
    width: 150,
    fixed: 'right'
  }
])

const currentPageIndex = ref(1)
const currentPageSize = ref(10)
const total = ref(0)
const dataSource = ref([])

//太多了，定义一个不做多举例
const value = ref(null)
const nameObj = ref('')
const options = ref([])

const projectNameValue = ref('')
const categoryIdValue = ref(null)
const categoryOptions = ref([
  {
    value: 0,
    label: '全部'
  },
  {
    value: 1,
    label: '电力工程'
  },
  {
    value: 2,
    label: '智能化工程'
  }
])
const projectTypeValue = ref(null)
const projectTypeOptions = ref([
  {
    value: 0,
    label: '全部'
  },
  {
    value: 'E',
    label: 'E'
  },
  {
    value: 'P',
    label: 'P'
  },
  {
    value: 'P(S)',
    label: 'P(S)'
  },
  {
    value: 'EP',
    label: 'EP'
  },
  {
    value: 'PC',
    label: 'PC'
  },
  {
    value: 'EPC',
    label: 'EPC'
  }
])
const fileList = ref([])
const categoryFilterOptions = (input, option) => {
  return option.label.toLowerCase().indexOf(input.toLowerCase()) >= 0
}
const projectTypeFilterOptions = (input, option) => {
  return option.label.toLowerCase().indexOf(input.toLowerCase()) >= 0
}

getDataList(true)
/* 获取数据源 */
async function getDataList(isShowMess) {
  const obj = {
    projectName: projectNameValue.value,
    categoryId: categoryIdValue.value,
    projectType: projectTypeValue.value,
    pageIndex: currentPageIndex.value,
    pageSize: currentPageSize.value
  }
  await getprojectTrackingList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = item.id
        if (item.children == null || item.children.length == 0) {
          delete item.children
        }
      })
      dataSource.value = res.data
      total.value = res.total
      if (isShowMess) {
        message.success('查询成功')
      }
    }
  })
}
/* 重置查询 */
function resetSearch() {
  // 重置项目名称
  projectNameValue.value = ''
  // 重置产品大类
  categoryIdValue.value = null
  // 重置项目类型
  projectTypeValue.value = null
}

const openModal = ref(false)
const importModal = ref(false)

const formState = reactive({
  name: '',
  number: '',
  categoryId: null,
  projectType: null
})
const labelCol = {
  style: {
    width: '150px'
  }
}
const wrapperCol = {
  span: 14
}
function resetForm() {
  formState.name = ''
  formState.number = ''
  formState.categoryId = null
  formState.projectType = null
}

const rules = {
  name: [
    {
      required: true,
      message: '请输入项目名称',
      trigger: 'change'
    },
    {
      min: 3,
      max: 5,
      message: '项目名称长度在3到5个字符',
      trigger: 'blur'
    }
  ],
  categoryId: [
    {
      required: true,
      message: '请选择产品大类',
      trigger: 'change'
    }
  ],
  projectType: [
    {
      required: true,
      message: '请选择项目类型',
      trigger: 'change'
    }
  ]
}
//获取表单dom
const formRef = ref(null)
function OpenAddModal() {
  resetForm()
  openModal.value = true
}

function handleOk() {
  const obj = {
    ProjectName: formState.name,
    MarketingProjectNumber: formState.number,
    CategoryId: formState.categoryId,
    ProjectType: formState.projectType
  }
  //判断是新增还是修改
  if (formState.id) {
    obj.Id = formState.id
  } else {
    obj.Id = guid()
  }
  formRef.value.validate().then(async () => {
    // 执行保存操作
    await saveProjectTracking(obj).then((res) => {
      if (res.code == 200 && res.success) {
        getDataList(false)
        openModal.value = false
        message.success('保存成功')
      }
    })
  })
}

function openEdit(row) {
  openModal.value = true
  formState.id = row.id
  formState.name = row.projectName
  formState.number = row.marketingProjectNumber
  formState.categoryId = row.categoryId
  formState.projectType = row.projectType
}

async function onDelete(id) {
  Modal.confirm({
    title: 'Confirm',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确定删除该条数据吗？',
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

//删除
async function del(resolve, reject, id) {
  await deleteProjectTrackingById({ id: id }).then((res) => {
    resolve()
    if (res.code == 200 && res.success) {
      message.success('删除成功')
      getDataList(false)
    }
  })
}

/* 导出数据 */
function ExportData() {
  const tableData = tranceData(columns.value, dataSource.value)
  // 创建工作簿
  const wb = XLSX.utils.book_new()
  // 将数据数组转化为工作表
  const ws = XLSX.utils.aoa_to_sheet(tableData)
  // 设置列宽
  ws['!cols'] = [
    { wpx: 120 },
    { wpx: 100 },
    { wpx: 110 },
    { wpx: 110 },
    { wpx: 150 },
    { wpx: 150 },
    { wpx: 200 },
    { wpx: 150 },
    { wpx: 150 },
    { wpx: 150 },
    { wpx: 150 },
    { wpx: 150 },
    { wpx: 150 }
  ]
  // 将 工作表 添加到 workbook
  XLSX.utils.book_append_sheet(wb, ws, 'Sheet1')
  // 将 workbook 写入文件
  XLSX.writeFile(wb, 'tablename.xlsx')
}

function tranceData(columns, tableList) {
  const obj = columns.reduce((acc, cur) => {
    if (!acc.titles && !acc.keys) {
      acc.titles = []
      acc.keys = []
    }
    acc.titles.push(cur.title)
    acc.keys.push(cur.dataIndex)
    return acc
  }, {})
  const tableBody = tableList.map((item, i) => {
    item.index = i + 1
    item.categoryId = item.categoryId == 1 ? '电力工程' : '智能化工程'
    return obj.keys.map((key, index) => item[key])
  })
  return [obj.titles, ...tableBody]
}

function OpenImport() {
  importModal.value = true
}

/* 校验文件类型 */
function beforeUploadFile(file) {
  const isExcel = file.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' || file.type === 'application/vnd.ms-excel'
  if (!isExcel) {
    message.error('只能上传 Excel 文件（.xls 或 .xlsx）！')
    return Upload.LIST_IGNORE
  }
  fileList.value = [file] // 只允许存储一个文件
  return false // 阻止自动上传
}

async function ImportData() {
  if (fileList.value.length === 0) {
    message.warning('请先选择文件！')
    return
  }

  const formData = new FormData()
  formData.append('file', fileList.value[0].originFileObj)

  try {
    const res = await uploadFile(formData)
    if (res.success) {
      message.success('文件上传成功！')
      fileList.value = [] // 清空文件列表
      importModal.value = false
      getDataList(false)
    } else {
      message.error(res.message)
    }
  } catch (error) {
    message.error('上传错误：' + error.message)
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

const areaOption = ref([
  {
    value: 0,
    label: '全部'
  },
  {
    value: 1,
    label: '东部'
  },
  {
    value: 2,
    label: '西部'
  },
  {
    value: 3,
    label: '南部'
  },
  {
    value: 4,
    label: '北部'
  },
  {
    value: 5,
    label: '中部'
  }
])

const selectedValue = ref(null)
function addCheckBtnCss(value) {
  selectedValue.value = value
}
</script>
<style lang="scss">
.subPage {
  height: 100%;

  .header {
    height: 90px;
    padding-left: 10px;
    padding-bottom: 10px;
    //防止页面布局塌陷
    display: flex;
    flex-wrap: wrap;
    flex-direction: row;

    .filterBox {
      width: 200px;
      height: 30px;
      margin-right: 10px;
      margin-top: 10px;
    }

    .filterBtn {
      width: 150px;
      margin-right: 10px;
      margin-top: 10px;
    }
  }

  .content {
    /* 折叠面板 */
    .accordion {
      background-color: #fff;
      border: 1px solid #ddd;
      border-radius: 5px;
      overflow: hidden;
      margin-bottom: 10px;
      margin: 0 10px;

      .accordion-content {
        max-height: 50px;
        overflow: hidden;
        transition: max-height 0.3s ease-in-out, padding 0.1s ease-in-out;
        background: #fff;
        padding: 0 10px;

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
  }
}
</style>
