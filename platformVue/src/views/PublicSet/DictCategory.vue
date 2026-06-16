<template>
  <div class="DictCategory">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>字典分类</p>
      </div>
      <div class="headerRight">
        <a-button :icon="h(SearchOutlined)" @click="search">查询</a-button>
        <a-button class="NewAddBtnBG" :icon="h(PlusOutlined)" type="primary" @click="showAddDataModal">新增</a-button>
      </div>
    </div>

    <a-table
      :pagination="false"
      size="large"
      :columns="columns"
      :data-source="data"
      bordered
      ref="myTable"
      :scroll="{  y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
      :expand-column-width="100"
      :row-selection="rowSelection"
    >
      <template #bodyCell="{ text, column, record }">
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a v-if="data.length" @click="detail(record.dictCategoryId)">
            <a-button :icon="h(EditOutlined)" style="display: inline; margin-right: 10px"></a-button>
          </a>
          <a-popconfirm v-if="data.length" title="确定删除吗？" @confirm="onDelete(record.dictCategoryId)">
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

    <!-- 抽屉 -->
    <a-drawer
      v-model:open="open"
      :title="drawerTitle"
      class="custom-class"
      root-class-name="root-class-name"
      :root-style="{ color: 'blue' }"
      style="color: red"
      placement="right"
    size = 'large'
      @after-open-change="afterOpenChange"
    >
      <a-form ref="formRef" :model="formState" :label-col="labelCol">
        <a-form-item label="分类名称" name="name">
          <a-input v-model:value="formState.name"  />
        </a-form-item>
        <a-form-item label="编码" name="code">
          <a-input-number v-model:value="formState.code" :min="0" style="width: 100%" />
        </a-form-item>
        <div style="margin-top: 10px; text-align: center; width: 100%">
          <a-button class="NewAddBtnBG" type="primary" @click="save" style="margin-right: 50px">保存</a-button>
          <a-button @click="exit">返回</a-button>
        </div>
      </a-form>
    </a-drawer>
  </div>
</template>
<script setup>
import { ref, onMounted, h, reactive, watch } from 'vue'
import { message } from 'ant-design-vue'
import {
  saveCategoryApi,
  deleteCategoryApi,
  updateCategoryApi,
  getCategoryByIdApi,
  getAllDictCategorytApi,
  getAllDictCategoryNoPageApi
} from '@/api/PublicSet/DataDict'
import { SearchOutlined, RedoOutlined, PlusOutlined, DeleteOutlined, EditOutlined, ExportOutlined, ImportOutlined, ShrinkOutlined } from '@ant-design/icons-vue'

onMounted(() => {
  getAllDictCategory(current.value, pageSize.value)
})

/* 页面缓存 */
defineOptions({
  name: 'A070102'
})
const myTable = ref(null)

const data = ref(null)
//获取数据字典列表
async function getAllDictCategory(page, size) {
  const query = {
    pageIndex: page,
    pageSize: size
  }
  const response = await getAllDictCategorytApi(query)
  if (response.code === 200 && response.success) {
    if (response.data) {
      response.data.forEach((element) => {
        element.key = element.dictCategoryId
      })
    }
    data.value = response.data
    total.value = response.total
  }
}
function search() {
  getAllDictCategory(current.value, pageSize.value)
}
const formRef = ref()
const formState = reactive({
  dictCategoryId: '',
  name: '',
  code: 0
})
const drawerTitle = ref()
const dictCategoryId = ref()
//详情
async function detail(id) {
  formState.dictCategoryId = ''
  formState.name = ''
  formState.code = 0

  open.value = true
  drawerTitle.value = '详情'
  dictCategoryId.value = id
  const params = {
    id: id
  }
  console.info('id:',id)
  const response = await getCategoryByIdApi(params)
  if (response.code === 200 && response.success) {
    formState.dictCategoryId = id
    formState.name = response.data.name
    formState.code = response.data.code
  }
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAllDictCategory(current, pageSize)
}
const pageSizeChange = (page, pageSize) => {
  getAllDictCategory(page, pageSize)
}

const labelCol = {
  style: {
    width: '90px'
  }
}
const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    fixed: 'left',
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '分类名称',
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
    title: '创建人',
    dataIndex: 'createName',
    key: 'createName',
    align: 'center'
  },
  {
    title: '创建时间',
    dataIndex: 'createTime',
    key: 'createTime',
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

//新增
const open = ref(false)
const showAddDataModal = async (id) => {
  drawerTitle.value = '新增'
  open.value = true
  formState.dictCategoryId = ''
  formState.name = ''
  formState.code = 0
}

//退出模态框
function exit() {
  open.value = false
  dictCategoryId.value = null
  getAllDictCategory(current.value, pageSize.value)
}
//保存分类
const save = async () => {
  if (!formState.dictCategoryId) {
    const response = await saveCategoryApi(formState)
    if (response.code === 200 && response.success) {
      dictCategoryId.value = response.data
      message.success('保存成功')
    }else if(response.code===200 &&!response.success){
      message.warning(response.message)
    }
  } else {
    const response = await updateCategoryApi(formState)
    if (response.code === 200 && response.success) {
      dictCategoryId.value = response.data
      message.success('保存成功')
    }else if(response.code===200 &&!response.success){
      message.warning(response.message)
    }
  }
  open.value = false
  getAllDictCategory(current.value, pageSize.value)
}

//删除
async function onDelete(id) {
  const params = {
    id: id
  }
  const response = await deleteCategoryApi(params)
  if (response.code === 200 && response.success) {
    message.success('删除成功')
    getAllDictCategory(current.value, pageSize.value)
  }
}
</script>

<style lang="scss">
.DictCategory {
  height: 100%;
  position: relative;
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
</style>
