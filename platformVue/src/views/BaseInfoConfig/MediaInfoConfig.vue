<template>
  <div class="MediaInfo">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>介质信息列表</p>
      </div>
      <div class="headerRight">
        <a-input style="margin-right: 5px" v-model:value="name" placeholder="请输入介质名称" />
        <CustomButtonList
          :ParamsRoleId="inputRoleId"
          :ParamsFunctionName="inputFunctionName"
          :BtnFunctionNameArray="['A010401', 'A010402', 'A010404', 'A010406', 'A010407']"
          @buttonClick="handleButtonClick"
        >
        </CustomButtonList>
      </div>
    </div>
    <a-table
      :pagination="false"
      size="large"
      :columns="columns"
      :data-source="data"
      bordered
      :scroll="{ y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
      ref="myTable"
      :expand-column-width="100"
      :expandedRowKeys="expandedRowKeys"
      @expandedRowsChange="expandedRowsChange"
      :expand-icon-column-index="2"
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
        <!-- 操作按钮 -->
        <template v-if="column.dataIndex === 'operation'">
          <a @click="showDrawer(record.mediaInfoId)" v-if="allDyBtn.find((item) => item.functionCode === 'A010408')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010408')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="detail(record.mediaInfoId)" v-if="allDyBtn.find((item) => item.functionCode === 'A010405')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010405')"
              :IsOnlyIcon="true"
              style="display: inline; margin-right: 10px"
            ></CustomButtonListTable>
          </a>
          <a @click="onDelete(record.mediaInfoId)" v-if="allDyBtn.find((item) => item.functionCode === 'A010403')">
            <CustomButtonListTable
              :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010403')"
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
      <a-form-item label="介质编码" name="code">
        <a-input :maxlength="20" v-model:value="itemFormState.code"  />
      </a-form-item>
      <a-form-item label="介质名称" name="name">
        <a-input :maxlength="20" v-model:value="itemFormState.name"  />
      </a-form-item>
      <a-form-item label="排序" name="sort">
        <a-input-number :min="0" v-model:value="itemFormState.sort"  style="width: 100%;" />
      </a-form-item>
      <a-form-item label="图标地址">
        <a-input v-model:value="itemFormState.icon"  />
      </a-form-item>
      <a-form-item label="介质折标煤系数" name="num">
        <a-input-number :min="0" v-model:value="itemFormState.num"  style="width: 100%;" />
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
</template>
<script setup>
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { ref, onMounted, h, reactive, watch, createVNode, watchEffect } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import { saveMediaInfoApi, deleteApi, getListAsyncApi, getDetailByIdAsyncApi } from '@/api/BaseInfoConfig/mediainfo'
import {
  // SearchOutlined,
  // RedoOutlined,
  // PlusOutlined,
  // CloseOutlined,
  // EditOutlined,
  // ArrowsAltOutlined,
  // ShrinkOutlined,
  CaretDownOutlined,
  CaretRightOutlined,
  ExclamationCircleOutlined
} from '@ant-design/icons-vue'

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
  name: 'A0104'
})
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0104',
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
const inputFunctionName = ref('A0104')
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
  if (functionName === 'expand') {
    expand()
  }
  if (functionName === 'expandClose') {
    expandClose()
  }
}

onMounted(() => {
  getMediaInfoList(current.value, pageSize.value, name.value)
  getAllButton()
})
const myTable = ref(null)

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getMediaInfoList(current, pageSize, name.value)
}
const pageSizeChange = (page, pageSize) => {
  getMediaInfoList(page, pageSize, name.value)
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
    title: '介质编码',
    dataIndex: 'code',
    key: 'code',
    width: 120,
    align: 'center'
  },
  {
    title: '介质名称',
    dataIndex: 'name',
    key: 'name',
    width: 120,
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
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    align: 'center'
  },
  {
    title: '图标',
    dataIndex: 'icon',
    key: 'icon',
    align: 'center'
  },
  {
    title: '介质折标煤系数',
    dataIndex: 'num',
    key: 'num',
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
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '修改人',
    dataIndex: 'createName',
    key: 'createName',
    // width: 120,
    align: 'center'
  },
  {
    title: '修改时间',
    dataIndex: 'createTime',
    key: 'createTime',
    minWidth: 180,
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    width: 180,
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
  open.value = true
  drawerTitle.value = '新增'
  itemFormState.mediaInfoId = ''
  itemFormState.pid = id
  itemFormState.code = ''
  itemFormState.name = ''
  itemFormState.remark = ''
  itemFormState.icon = ''
  itemFormState.num = ''
  itemFormState.sort = ''
}
const drawerTitle = ref('')
const itemFormState = reactive({
  mediaInfoId: '',
  pid: '',
  code: '',
  name: '',
  remark: '',
  sort: '',
  icon: '',
  num: ''
})
const validate = async (_rule, value) => {
  if (value.trim() === '') {
    return Promise.reject('这是必填项')
  } else {
    return Promise.resolve()
  }
}
const rules = {
  name: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  code: [
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
  num: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ]
}
const itemFormRef = ref()
const itemLabelCol = {
  style: {
    width: '120px'
  }
}

//退出子项抽屉
function exitItem() {
  open.value = false

  getMediaInfoList(current.value, pageSize.value, name.value)
}
//获取介质信息列表
const name = ref()
async function getMediaInfoList(page, size, name) {
  const query = {
    pageIndex: page,
    pageSize: size,
    name: name
  }
  const response = await getListAsyncApi(query)
  if (response.code === 200 && response.success) {
    data.value = response.data
    total.value = response.total
    dg(data.value)
  }
}
function dg(obj) {
  obj.forEach((e) => {
    e.key = e.mediaInfoId
    if (e.children !== null) {
      dg(e.children)
    }
  })
}
//保存介质
const saveItem = async () => {
  itemFormRef.value.validate().then(async () => {
    const response = await saveMediaInfoApi(itemFormState)
    if (response.code === 200 && response.success) {
      message.success('保存成功')
      getMediaInfoList(current.value, pageSize.value, name.value)
      open.value = false
    }
  })
}

//详情
async function detail(id) {
  open.value = true
  drawerTitle.value = '详情'
  itemFormState.mediaInfoId = ''
  itemFormState.pid = ''
  itemFormState.code = ''
  itemFormState.name = ''
  itemFormState.remark = ''
  itemFormState.sort = ''
  itemFormState.icon = ''
  itemFormState.num = ''
  const params = {
    id: id
  }
  const response = await getDetailByIdAsyncApi(params)
  if (response.code === 200 && response.success) {
    itemFormState.mediaInfoId = response.data.mediaInfoId
    itemFormState.pid = response.data.pid
    itemFormState.code = response.data.code
    itemFormState.name = response.data.name
    itemFormState.remark = response.data.remark
    itemFormState.sort = response.data.sort
    itemFormState.icon = response.data.icon
    itemFormState.num = response.data.num
  }
}

const expandedRowKeys = ref([])
// 全部展开
const expand = () => {
  dg2(data.value)
}
function dg2(obj) {
  obj.forEach((e) => {
    if (e.children !== null) {
      expandedRowKeys.value.push(e.mediaInfoId)
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
  getMediaInfoList(current.value, pageSize.value, name.value)
}
//重置查询
function resetSearch() {
  name.value = ''
  getMediaInfoList(current.value, pageSize.value, name.value)
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
      getMediaInfoList(current.value, pageSize.value, name.value)
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
.MediaInfo {
  /* 至少铺满可视区域，避免分页下方出现较大空白 */
  min-height: calc(100vh - 96px - 35px);
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
</style>
