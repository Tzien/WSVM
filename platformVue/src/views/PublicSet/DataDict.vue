<template>
  <div class="DataDict">
    <div style="overflow-y: auto" class="leftModel">
      <a-tree @select="dictTypeSelect" :tree-data="dataDictTreeData" defaultExpandAll> </a-tree>
      <a-tree @select="sysSelect" :tree-data="sysTreeData" defaultExpandAll> </a-tree>
    </div>
    <div class="rightModel">
      <div style="height: 40px; line-height: 40px; text-align: right">
        <div class="headerLeft">
          <p>字典列表</p>
        </div>
        <div class="headerRight">
          <a-input style="width: 150px; margin-right: 5px" v-model:value="dictName" placeholder="请输入字典名称" />
          <a-button :icon="h(SearchOutlined)" @click="search">查询</a-button>
          <a-button class="NewAddBtnBG" :icon="h(PlusOutlined)" type="primary" @click="showAddDataModal">新增</a-button>
          <a-button :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
          <a-upload ref="uploadRef" :beforeUpload="importDict" :showUploadList="false" :autoUpload="false" :openFileDialogOnClick="false"   maxCount="1"
            >
            <a-button :icon="h(ImportOutlined)" @click="handleUploadClick">导入</a-button>
          </a-upload>
          <a-button :icon="h(ExportOutlined)" @click="exportDict">导出</a-button>
        </div>
      </div>

        <a-table
          :pagination="false"
          size="large"
          :columns="columns"
          :data-source="data"
          bordered
          ref="myTable"
          :scroll="{ x: 2200, y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
          :expand-column-width="100"
          :row-selection="rowSelection"
        >
          <template #bodyCell="{ text, column, record }">
            <!-- 处理状态列 -->
            

            <template v-if="column.dataIndex === 'isFieldDir'">
              <span v-if="text === true"> 是 </span>
              <span v-else> 否 </span>
            </template>
            <template v-if="column.dataIndex === 'status'">
              <span v-if="text === true"> 是 </span>
              <span v-else> 否 </span>
            </template>
            <!-- 操作按钮 -->
            <template v-if="column.dataIndex === 'operation'">
              <a v-if="data.length" @click="detail(record.dictBaseInfoId)">
                <a-button :icon="h(EditOutlined)" style="display: inline; margin-right: 10px"></a-button>
              </a>
              <a-popconfirm v-if="data.length" title="确定删除吗？" @confirm="onDelete(record.dictBaseInfoId)">
                <a-button type="primary" danger :icon="h(DeleteOutlined)" style="display: inline"></a-button>
              </a-popconfirm>
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
    <a-modal v-model:open="open" :title="modalTitle" centered :keyboard="false" :maskClosable="false" width="100%" :footer="null">
        <div class="model">
          <div class="leftModel">
            <a-form ref="formRef" :model="formState" :label-col="labelCol">
              <a-form-item label="字典名称" name="dictName">
                <a-input v-model:value="formState.dictName" style="width: 300px" />
              </a-form-item>
              <a-form-item label="字典编码" name="dictCode">
                <a-input v-model:value="formState.dictCode" style="width: 300px" />
              </a-form-item>
              <a-form-item label="字典类型">
                <a-select v-model:value="formState.dictTypeId" ref="select" :options="dictTypeList" style="width: 300px" />
              </a-form-item>
              <a-form-item label="所属系统">
                <a-select v-model:value="formState.subSysId" ref="select" :options="sysList" style="width: 300px" />
              </a-form-item>
              <a-form-item label="字典分类">
                <a-select v-model:value="formState.dictClassId" ref="select" :options="dictClassList" style="width: 300px" />
              </a-form-item>
              <a-form-item label="是否启用">
                <a-radio-group v-model:value="formState.isActive" name="isActive">
                  <a-radio :value="true">是</a-radio>
                  <a-radio :value="false">否</a-radio>
                </a-radio-group>
              </a-form-item>
              <a-form-item label="是否允许编辑" name="isAllowEdit">
                <a-radio-group v-model:value="formState.isAllowEdit" name="isActive">
                  <a-radio :value="true">是</a-radio>
                  <a-radio :value="false">否</a-radio>
                </a-radio-group>
              </a-form-item>
              <a-form-item label="是否允许删除">
                <a-radio-group v-model:value="formState.isAllowDelete" name="isActive">
                  <a-radio :value="true">是</a-radio>
                  <a-radio :value="false">否</a-radio>
                </a-radio-group>
              </a-form-item>
              <a-form-item label="备注">
                <a-input v-model:value="formState.remark" style="width: 300px" />
              </a-form-item>
              <div style="margin-top: 10px; text-align: center; width: 100%">
                <a-button type="primary" @click="save" style="margin-right: 50px">保存</a-button>
                <a-button @click="exit">返回</a-button>
              </div>
            </a-form>
          </div>
          <div class="rightModel">
            <div style="height: 40px; line-height: 40px; text-align: right">
              <div class="headerLeft">
                <p>字典详情</p>
              </div>
              <div class="headerRight">
                <a-button :icon="h(PlusOutlined)" type="primary" @click="showDrawer(null)">新增</a-button>
              </div>
            </div>

              <a-table
                :pagination="false"
                size="large"
                :columns="itemColumns"
                :data-source="itemData"
                bordered
                ref="myTable"
                :scroll="{ x: 2200, y: 'calc(100vh - 96px - 40px - 55px - 40px - 70px)' }"
                :expand-column-width="100"
              >
                <template #bodyCell="{ text, column, record }">
                  <template v-if="column.dataIndex === 'isValid'">
                    <span v-if="text === true"> 是 </span>
                    <span v-else> 否 </span>
                  </template>
                  <template v-if="column.dataIndex === 'isActive'">
                    <span v-if="text === true"> 是 </span>
                    <span v-else> 否 </span>
                  </template>
                  <template v-if="column.dataIndex === 'operation'">
                    <a v-if="itemData.length" @click="showDrawer(record.dictDetailId)">
                      <a-button type="primary" :icon="h(PlusOutlined)" style="display: inline; margin-right: 10px"></a-button>
                    </a>
                    <a v-if="itemData.length" @click="detailItem(record.dictDetailId)">
                      <a-button :icon="h(EditOutlined)" style="display: inline; margin-right: 10px"></a-button>
                    </a>
                    <a-popconfirm v-if="data.length" title="确定删除吗？" @confirm="onDeleteItem(record.dictDetailId)">
                      <a-button type="primary" danger :icon="h(DeleteOutlined)" style="display: inline"></a-button>
                    </a-popconfirm>
                  </template>
                </template>
              </a-table>
            <div class="paginationStyle">
              <a-pagination
                v-model:current="currentItem"
                v-model:page-size="pageSizeItem"
                :total="totalItem"
                :show-total="(totalItem) => `总条数：${totalItem} `"
                class="pagination"
                show-size-changer
                @showSizeChange="onShowSizeChangeItem"
                @change="pageSizeChangeItem"
              />
            </div>
          </div>
        </div>
    </a-modal>

    <!-- 抽屉 -->
    <a-drawer
      v-model:open="dictItemOpen"
      :title="itemDrawerTitle"
      class="custom-class"
      root-class-name="root-class-name"
      :root-style="{ color: 'blue' }"
      style="color: red"
      title="Basic Drawer"
      placement="right"
    size = 'large'
      @after-open-change="afterOpenChange"
    >
      <a-form ref="itemFormRef" :model="itemFormState" :label-col="itemLabelCol">
        <a-form-item label="中文简称" name="dictName">
          <a-input v-model:value="itemFormState.itemShortName"  />
        </a-form-item>
        <a-form-item label="中文名称" name="dictCode">
          <a-input v-model:value="itemFormState.itemName" />
        </a-form-item>
        <a-form-item label="英文简称">
          <a-input v-model:value="itemFormState.engItemShortName"  />
        </a-form-item>
        <a-form-item label="英文名称">
          <a-input v-model:value="itemFormState.engItemName"  />
        </a-form-item>
        <a-form-item label="编码">
          <a-input v-model:value="itemFormState.code"  />
        </a-form-item>
        <a-form-item label="排序">
          <a-input v-model:value="itemFormState.itemSort"  />
        </a-form-item>
        <a-form-item label="单元格颜色">
          <a-input v-model:value="itemFormState.cellColor"  />
        </a-form-item>
        <a-form-item label="字体颜色">
          <a-input v-model:value="itemFormState.fontColor"  />
        </a-form-item>
        <a-form-item label="列表图标">
          <a-input v-model:value="itemFormState.listIcon"  />
        </a-form-item>
        <a-form-item label="树形图标">
          <a-input v-model:value="itemFormState.treeIcon"  />
        </a-form-item>
        <a-form-item label="有效状态">
          <a-radio-group v-model:value="itemFormState.isValid">
            <a-radio :value="true">是</a-radio>
            <a-radio :value="false">否</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="启用状态">
          <a-radio-group v-model:value="itemFormState.isActive">
            <a-radio :value="true">是</a-radio>
            <a-radio :value="false">否</a-radio>
          </a-radio-group>
        </a-form-item>
        <a-form-item label="备注">
          <a-input v-model:value="itemFormState.remark"  />
        </a-form-item>
        <div style="margin-top: 10px; text-align: center; width: 100%">
          <a-button class="NewAddBtnBG" type="primary" @click="saveItem" style="margin-right: 50px">保存</a-button>
          <a-button @click="exitItem">返回</a-button>
        </div>
      </a-form>
    </a-drawer>
  </div>
</template>
<script setup>
import { ref, onMounted, h, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import {
  getDictTypetApi,
  getAllSysInfoTreeApi,
  getAllDictApi,
  saveDictApi,
  getDictInfoByIdApi,
  deleteDictApi,
  deleteDictItemApi,
  getDictItemInfoByIdApi,
  saveDictItemApi,
  getAllDictItemApi,
  exportApi,
  importApi,
  getAllDictCategoryNoPageApi
} from '@/api/PublicSet/DataDict'
import { SearchOutlined, RedoOutlined, PlusOutlined, DeleteOutlined, EditOutlined, ExportOutlined, ImportOutlined, ShrinkOutlined } from '@ant-design/icons-vue'
const dataDictTreeData = ref([
  {
    title: '字典类型',
    children: []
  }
])
const sysTreeData = ref([
  {
    title: '所属子系统',
    children: []
  }
])
const itemSpinning = ref(false)
onMounted(() => {
  getDictType()
  getAllSysInfoTree()
  getAllDict(current.value, pageSize.value, dictName.value)
  getAllDictCategoryNoPage()
})

/* 页面缓存 */
defineOptions({
  name: 'A070101'
})
const myTable = ref(null)

const dictClassList=ref([])
//获取所有字典分类
async function getAllDictCategoryNoPage() {
  dictTypeList.value = []
  const response = await getAllDictCategoryNoPageApi()
  if (response.code === 200 && response.success) {
    response.data.forEach((c) => {
      dictClassList.value.push({
        label: c.name,
        value: c.code
      })
    })
  }
}

//获取所有字典类型
async function getDictType() {
  dictTypeList.value = []
  const response = await getDictTypetApi()
  if (response.code === 200 && response.success) {
    const children = []
    response.data.forEach((c) => {
      children.push({
        title: c.dictTypeName,
        key: c.dictTypeId
      })
      dictTypeList.value.push({
        value: c.dictTypeId,
        label: c.dictTypeName
      })
    })
    dataDictTreeData.value[0].children = children
  }
}
const dictTypeId=ref();
async function dictTypeSelect(selectedKeys) {
  if (selectedKeys[0] !== '0-0') {
    dictTypeId.value=selectedKeys[0];
    getAllDict(1, 10, dictName.value )
  }
}
const sysId = ref()
async function sysSelect(selectedKeys) {
  if (selectedKeys[0] !== '0-0') {
    sysId.value = selectedKeys[0]
    getAllDict(1,10, dictName.value)
  }
}

//查询子系统名称树数据
async function getAllSysInfoTree() {
  sysList.value = []
  const response = await getAllSysInfoTreeApi()
  if (response.code === 200 && response.success) {
    const children = []
    response.data.forEach((c) => {
      children.push({
        title: c.subSysName,
        key: c.sysInfoId
      })
      sysList.value.push({
        value: c.sysInfoId,
        label: c.subSysName
      })
    })
    sysTreeData.value[0].children = children
  }
}

//获取数据字典列表
async function getAllDict(page, size, dictName) {
  const query = {
    pageIndex: page,
    pageSize: size,
    dictName: dictName,
    dictTypeId: dictTypeId.value,
    subSysId: sysId.value
  }
  const response = await getAllDictApi(query)
  if (response.code === 200 && response.success) {
    if (response.data) {
      response.data.forEach((element) => {
        element.key = element.dictBaseInfoId
      })
    }
    data.value = response.data
    total.value = response.total
  }
}

const dictId = ref()
//详情
async function detail(id) {
  formState.dictBaseInfoId = ''
  formState.dictName = ''
  formState.dictCode = ''
  formState.dictTypeId = ''
  formState.subSysId = ''
  formState.dictClassId = ''
  formState.isActive = false
  formState.isAllowEdit = false
  formState.isAllowDelete = false
  formState.remark = ''

  //子项
  itemData.value = []
  totalItem.value = 0

  itemSpinning.value = true
  modalTitle.value = '详情'
  dictId.value = id
  open.value = true
  const params = {
    dictId: id
  }
  const response = await getDictInfoByIdApi(params)
  if (response.code === 200 && response.success) {
    itemSpinning.value = false
    formState.dictBaseInfoId = id
    formState.dictName = response.data.dictName
    formState.dictCode = response.data.dictCode
    formState.dictTypeId = response.data.dictTypeId
    formState.subSysId = response.data.subSysId
    formState.dictClassId = response.data.dictClassId
    formState.isActive = response.data.isActive
    formState.isAllowEdit = response.data.isAllowEdit
    formState.isAllowDelete = response.data.isAllowDelete
    formState.remark = response.data.remark == null ? '' : response.data.remark

    //子项
    itemData.value = response.data.items
    totalItem.value = response.data.total
  }
}

//查询
const dictName = ref('')
function search() {
  getAllDict(current.value, pageSize.value, dictName.value)
}
//重置查询
function resetSearch() {
  dictName.value = ''
  getAllDict(current.value, pageSize.value, dictName.value)
}
//删除
async function onDelete(id) {
  const params = {
    dictId: id
  }
  const response = await deleteDictApi(params)
  if (response.code === 200 && response.success) {
    message.success('删除成功')
    getAllDict(current.value, pageSize.value, dictName.value)
  }
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAllDict(current, pageSize, dictName.value)
}
const pageSizeChange = (page, pageSize) => {
  getAllDict(page, pageSize, dictName.value)
}
const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    fixed: 'left',
    //width: 80,
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '字典名称',
    dataIndex: 'dictName',
    key: 'dictName',
    //width: 140,
    fixed: 'left',
    align: 'center'
  },
  {
    title: '字典编码',
    dataIndex: 'dictCode',
    key: 'dictCode',
    //width: 130,
    fixed: 'left',
    align: 'center'
  },
  {
    title: '字典类型',
    dataIndex: 'dictTypeName',
    key: 'dictTypeName',
    //width: 150,
    align: 'center'
  },
  {
    title: '所属子系统',
    dataIndex: 'subSysName',
    key: 'subSysName',
    //width: 180,
    align: 'center'
  },
  {
    title: '字典分类',
    dataIndex: 'dictClassName',
    key: 'dictClassName',
    //width: 80,
    align: 'center'
  },
  {
    title: '是否字段字典',
    dataIndex: 'isFieldDir',
    key: 'isFieldDir',
    //width: 80,
    align: 'center'
  },
  {
    title: '字段类型',
    dataIndex: 'fieldTypeName',
    key: 'fieldTypeName',
    //width: 80,
    align: 'center'
  },
  {
    title: '字段长度',
    dataIndex: 'fieldLength',
    key: 'fieldLength',
    //width: 80,
    align: 'center'
  },
  {
    title: '字段小数位',
    dataIndex: 'fieldDecimal',
    key: 'fieldDecimal',
    //width: 80,
    align: 'center'
  },
  {
    title: '字段缺省值',
    dataIndex: 'fieldDefault',
    key: 'fieldDefault',
    //width: 80,
    align: 'center'
  },
  {
    title: '启用状态',
    dataIndex: 'status',
    key: 'status',
    //width: 180,
    align: 'center'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 180,
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
    width: 180,
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

const itemColumns = [
  {
    title: '中文简称',
    dataIndex: 'itemShortName',
    key: 'itemShortName',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '中文名称',
    dataIndex: 'itemName',
    key: 'itemName',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '英文简称',
    dataIndex: 'engItemShortName',
    key: 'engItemShortName',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '英文名称',
    dataIndex: 'engItemName',
    key: 'engItemName',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '编码',
    dataIndex: 'code',
    key: 'code',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '排序',
    dataIndex: 'itemSort',
    key: 'itemSort',
    align: 'center'
  },
  {
    title: '单元格颜色',
    dataIndex: 'cellColor',
    key: 'cellColor',
    align: 'center'
  },
  {
    title: '字体颜色',
    dataIndex: 'fontColor',
    key: 'fontColor',
    align: 'center'
  },
  {
    title: '列表图标',
    dataIndex: 'listIcon',
    key: 'listIcon',
    align: 'center'
  },
  {
    title: '树形图标',
    dataIndex: 'treeIcon',
    key: 'treeIcon',
    align: 'center'
  },
  {
    title: '有效状态',
    dataIndex: 'isValid',
    key: 'isValid',
    align: 'center'
  },
  {
    title: '启用状态',
    dataIndex: 'isActive',
    key: 'isActive',
    width: 180,
    align: 'center'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 180,
    align: 'center'
  },
  {
    title: '创建人',
    dataIndex: 'createBy',
    key: 'createBy',
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createDate',
    key: 'createDate',
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
    dataIndex: 'operation',
    width: 180,
    fixed: 'right',
    align: 'center'
  }
]
const itemData = ref(null)

//模态框
const modalTitle = ref('')
const formState = reactive({
  dictBaseInfoId: '',
  dictName: '',
  dictCode: '',
  dictTypeId: '',
  subSysId: '',
  dictClassId: 0,
  isActive: false,
  isAllowEdit: false,
  isAllowDelete: false,
  remark: ''
})
const formRef = ref()
const labelCol = {
  style: {
    width: '150px'
  }
}
//新增
const open = ref(false)
const showAddDataModal = async (id) => {
  modalTitle.value = '新增'
  open.value = true
  formState.dictBaseInfoId = ''
  formState.dictName = ''
  formState.dictCode = ''
  formState.dictTypeId = ''
  formState.subSysId = ''
  formState.dictClassId = ''
  formState.isActive = false
  formState.isAllowEdit = false
  formState.isAllowDelete = false
  formState.remark = ''

  //子项
  itemData.value = []
  totalItem.value = 0
}

//字典类型下拉框

const dictTypeList = ref([])
const sysList = ref([])


//退出模态框
function exit() {
  open.value = false
  dictId.value = null
  getAllDict(current.value, pageSize.value, dictName.value)
}
//保存数据字典
const save = async () => {
  const response = await saveDictApi(formState)
  if (response.code === 200 && response.success) {
    dictId.value = response.data
    message.success('保存成功')
  }
}

//删除
async function onDeleteItem(id) {
  const params = {
    dictItemId: id
  }
  const response = await deleteDictItemApi(params)
  if (response.code === 200 && response.success) {
    message.success('删除成功')
    getAllDictItem(currentItem.value, pageSizeItem.value)
  }
}
const dictItemOpen = ref(false)

//打开抽屉
const showDrawer = (pid) => {
  console.info('父级id：', pid)
  dictItemOpen.value = true
  itemDrawerTitle.value = '新增'
  itemFormState.dictDetailId = ''
  itemFormState.pid = pid
  itemFormState.itemShortName = ''
  itemFormState.itemName = ''
  itemFormState.engItemShortName = ''
  itemFormState.engItemName = ''
  itemFormState.code = ''
  itemFormState.itemSort = ''
  itemFormState.cellColor = ''
  itemFormState.fontColor = ''
  itemFormState.listIcon = ''
  itemFormState.treeIcon = ''
  itemFormState.isValid = false
  itemFormState.isActive = false
  itemFormState.remark = ''
}
//模态框
const itemDrawerTitle = ref('')
const itemFormState = reactive({
  dictDetailId: '',
  pid: '',
  itemShortName: '',
  itemName: '',
  engItemShortName: '',
  engItemName: '',
  code: '',
  itemSort: '',
  cellColor: '',
  fontColor: '',
  listIcon: '',
  treeIcon: '',
  isValid: false,
  isActive: false,
  remark: ''
})
const itemFormRef = ref()
const itemLabelCol = {
  style: {
    width: '100px'
  }
}
//退出子项抽屉
function exitItem() {
  dictItemOpen.value = false

  getAllDictItem(currentItem.value, pageSizeItem.value)
}
//保存数据字典子项
const saveItem = async () => {
  var params = {
    dictId: dictId.value,
    items: [itemFormState]
  }
  const response = await saveDictItemApi(params)
  console.info(response)
  if (response.code === 200 && response.success) {
    message.success('保存成功')
    getAllDictItem(currentItem.value, pageSizeItem.value)
    dictItemOpen.value = false
  }
}
//获取数据字典子项列表
async function getAllDictItem(page, size) {
  itemSpinning.value = true
  const query = {
    pageIndex: page,
    pageSize: size,
    dictId: dictId.value
  }
  const response = await getAllDictItemApi(query)
  if (response.code === 200 && response.success) {
    response.data.forEach((element) => {
      element.key = element.dictDetailId
    })
    itemSpinning.value = false
    itemData.value = response.data
    totalItem.value = response.total
  }
}
//子项详情
async function detailItem(id) {
  itemDrawerTitle.value = '详情'
  dictItemOpen.value = true
  const params = {
    dictItemId: id
  }
  const response = await getDictItemInfoByIdApi(params)

  if (response.code === 200 && response.success) {
    formState.dictBaseInfoId = id
    itemFormState.dictDetailId = response.data.dictDetailId
    itemFormState.pid = response.data.pid
    itemFormState.itemShortName = response.data.itemShortName
    itemFormState.itemName = response.data.itemName
    itemFormState.engItemShortName = response.data.engItemShortName
    itemFormState.engItemName = response.data.engItemName
    itemFormState.code = response.data.code
    itemFormState.itemSort = response.data.itemSort
    itemFormState.cellColor = response.data.cellColor
    itemFormState.fontColor = response.data.fontColor
    itemFormState.listIcon = response.data.listIcon
    itemFormState.treeIcon = response.data.treeIcon
    itemFormState.isValid = response.data.isValid
    itemFormState.isActive = response.data.isActive
    itemFormState.remark = response.data.remark
  }
}

//字典子项分页
const currentItem = ref(1)
const pageSizeItem = ref(10)
const totalItem = ref(0)
const onShowSizeChangeItem = (currentItem, pageSizeItem) => {
  getAllDictItem(currentItem, pageSizeItem)
}
const pageSizeChangeItem = (page, pageSizeItem) => {
  getAllDictItem(page, pageSizeItem)
}

const rowSelection = ref({
  hideSelectAll: true,
  type: 'checkbox',
  selectedRowKeys: [],
  selectedRow: '',
  onChange: (selectedRowKeys, selectedRows) => {
    rowSelection.value.selectedRowKeys = selectedRowKeys
    dictIds.value = selectedRowKeys
  }
})
const dictIds = ref([])
//导出
const exportDict = async () => {
  if (dictIds.value.length == 0) {
    message.warning('请选择要导出的字典！')
    return
  }
  const params = {
    ids: dictIds.value.join(',')
  }
  const response = await exportApi(params)
  if (response.status === 200) {
    // 创建一个临时链接并下载文件
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'dict.txt')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  } else if (response.code === 200 && !response.success) {
    message.error(response.statusText)
  }
}

const uploadRef = ref();
// 点击按钮时先校验，再手动触发文件选择
const handleUploadClick = () => {
  if (!sysId.value) {
    message.warning('请先选择系统！');
    return;
  }
  if (uploadRef.value) {
    const input = uploadRef.value.$el.querySelector('input[type="file"]');
    if (input) input.click(); // 触发原生文件选择
  }
};



async function importDict(file) { 
  const formData = new FormData()
  formData.append('file', file)
  formData.append('sysId', sysId.value)
  const response =await importApi(formData)
  console.info('结果：',response)
  if (response.code === 200 && response.success) {
    message.success('导入成功')
    getAllDict(current.value, pageSize.value, dictName.value)
  } else if (response.code === 200 && !response.success) {
    message.error(response.message)
  }
  return false
}
</script>

<style lang="scss">
.DataDict {
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
    width: 15%;
    border-right: 1px solid gainsboro;
    background-color: white;
  }

  .rightModel {
    width: 85%;
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
  .headerRight button {
    margin: 0 5px;
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

.model {
  border-top: 1px solid gainsboro;
  height: 800px;
  display: flex;
  padding-top: 10px;

  .leftModel {
    width: 30%;
    border-right: 1px solid gainsboro;
    // border: 1px solid red;
  }

  .rightModel {
    width: 70%;

    .ant-table-container {
      width: 100%;
    }
    .ant-table-body {
      overflow-y: scroll !important;
      height: calc(100vh - 96px - 40px - 40px - 70px);
    }
    .ant-table-content {
      height: calc(100vh - 96px - 40px - 40px - 70px);
    }
    .leftModel {
      width: 15%;
      border-right: 1px solid gainsboro;
      background-color: white;
    }

    .rightModel {
      width: 85%;
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
    .headerRight button {
      margin: 0 5px;
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
}
</style>
