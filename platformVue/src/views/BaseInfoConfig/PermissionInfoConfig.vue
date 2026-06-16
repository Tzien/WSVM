<template>
  <div class="PermissionInfoConfig">
    <div class="leftModel">
      <a-tree style="margin-top: 10px" @select="sysSelect" :tree-data="sysTreeData" defaultExpandAll> </a-tree>
    </div>
    <div class="rightModel">
      <div style="height: 40px; line-height: 40px; text-align: right">
        <div class="headerLeft">
          <p>权限信息配置</p>
        </div>
        <div class="headerRight">
          <a-input style="width: 150px; margin-right: 5px" v-model:value="routeName" placeholder="请输入菜单名称" />
          <CustomButtonList
            :ParamsRoleId="inputRoleId"
            :ParamsFunctionName="inputFunctionName"
            :BtnFunctionNameArray="['A010901', 'A010908', 'A010907', 'A010904', 'A010911', 'A010905', 'A010903']"
            @buttonClick="handleButtonClick"
          >
          </CustomButtonList>
          <a-upload :before-upload="tableImport" :showUploadList="false" maxCount="1">
            <CustomButtonList
              :ParamsRoleId="inputRoleId"
              :ParamsFunctionName="inputFunctionName"
              :BtnFunctionNameArray="['A010902']"
              @buttonClick="handleButtonClick"
            >
            </CustomButtonList>
          </a-upload>
        </div>
      </div>
      <a-spin :spinning="spinning">
        <a-table
          id="qwe"
          :pagination="false"
          size="large"
          :columns="columns"
          :data-source="data"
          bordered
          ref="myTable"
          :scroll="{ x: 3000, y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
          :expand-column-width="100"
          @expandedRowsChange="expandedRowsChange"
          :expand-icon-column-index="4"
          :expandedRowKeys="expandedRowKeys"
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
            <template v-if="column.dataIndex === 'function'">
              <span v-if="text === 1"> 菜单 </span>
              <span v-else-if="text === 2"> 按钮 </span>
              <span v-else-if="text === 3"> 接口 </span>
            </template>

            <template v-if="column.dataIndex === 'menuType'">
              <span v-if="text === 1"> 内部系统 </span>
              <span v-else-if="text === 2"> 外部系统 </span>
            </template>
            <template v-if="column.dataIndex === 'isActive'">
              <span v-if="text === true"> 是 </span>
              <span v-else> 否 </span>
            </template>
            <template v-if="column.dataIndex === 'isCache'">
              <span v-if="text === true"> 是 </span>
              <span v-else> 否 </span>
            </template>
            <!-- 操作按钮 -->
            <template v-if="column.dataIndex === 'operation'">
              <a @click="showDrawer(record.id)" v-if="record.function !== 3 && allDyBtn.find((item) => item.functionCode === 'A010909')">
                <CustomButtonListTable
                  :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010909')"
                  :IsOnlyIcon="true"
                  style="display: inline; margin-right: 10px"
                ></CustomButtonListTable>
              </a>
              <a @click="detail(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010906')">
                <CustomButtonListTable
                  :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010906')"
                  :IsOnlyIcon="true"
                  style="display: inline; margin-right: 10px"
                ></CustomButtonListTable>
              </a>
              <a @click="onDelete(record.id)" v-if="allDyBtn.find((item) => item.functionCode === 'A010910')">
                <CustomButtonListTable
                  :ObjItem="allDyBtn.find((item) => item.functionCode === 'A010910')"
                  :IsOnlyIcon="true"
                  style="display: inline; margin-right: 10px"
                ></CustomButtonListTable>
              </a>
            </template>
          </template>
        </a-table>
      </a-spin>

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

  <!-- 用户权限查看模态框 -->
  <a-modal v-model:open="open" title="用户权限查看" centered :keyboard="false" :maskClosable="false" width="70%" :footer="null">
    <div class="model">
      <div class="leftModel">
        <div style="margin-bottom: 10px">
          <a-input-search v-model:value="roleName" placeholder="输入角色" style="width: 90%" @search="roleSearch" />
        </div>
        <a-tree @select="roleSelect" :tree-data="role_treeData"> </a-tree>
      </div>
      <div class="rightModel">
        <div style="height: 40px; line-height: 40px; text-align: right">
          <a-input style="width: 150px" v-model:value="userName" placeholder="请输入用户姓名" />
          <a-button style="margin: 0 10px" :icon="h(SearchOutlined)" @click="searchUser">查询</a-button>
          <a-button :icon="h(RedoOutlined)" @click="resetSearchUser">重置查询</a-button>
        </div>
        <a-table
          :pagination="false"
          size="large"
          :columns="itemColumns"
          :data-source="itemData"
          bordered
          ref="myTable"
          :scroll="{ x: 1800, y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
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

        <!-- <a-layout style="height: 100%; overflow: hidden">
          <a-layout>
            <a-layout-header class="header">
              <div class="headerRight"></div>
            </a-layout-header>
            <a-layout-content class="content">
              <div ref="myDiv" style="height: 100%">
                
              </div>
            </a-layout-content>
            <a-layout-footer class="foot">
              <div class="footPagintaion">
                
              </div>
            </a-layout-footer>
          </a-layout>
        </a-layout> -->
      </div>
    </div>
  </a-modal>

  <!-- 抽屉 -->
  <a-drawer
    v-model:open="permissionOpen"
    :title="itemDrawerTitle"
    class="custom-class"
    root-class-name="root-class-name"
    :root-style="{ color: 'blue' }"
    style="color: red"
    title="Basic Drawer"
    placement="right"
    size="large"
    @after-open-change="afterOpenChange"
  >
    <a-form ref="itemFormRef" :model="itemFormState" :label-col="itemLabelCol" :rules="rules">
      <a-form-item label="编码" name="functionCode">
        <a-input v-model:value="itemFormState.functionCode" />
      </a-form-item>
      <a-form-item label="菜单名称" name="functionName">
        <a-input v-model:value="itemFormState.functionName" />
      </a-form-item>
      <!-- <a-form-item label="英文名称">
        <a-input v-model:value="itemFormState.englishName" />
      </a-form-item> -->

      <a-form-item label="功能类型" name="function">
        <a-select v-model:value="itemFormState.function" :size="size" ref="select" :options="functionOptions"></a-select>
      </a-form-item>
      <a-form-item v-if="itemFormState.function == 1" label="路由" name="route">
        <a-input v-model:value="itemFormState.route" />
      </a-form-item>
      <a-form-item v-if="itemFormState.function == 1" label="路由文件">
        <a-input v-model:value="itemFormState.routeFile" />
      </a-form-item>

      <a-form-item label="接口" name="interfaceId" :rules="itemFormState.function == 3 ? [{ required: true, message: '请选择接口' }] : []">
        <a-tree-select
          v-model:value="itemFormState.interfaceId"
          show-search
          :dropdown-style="{ maxHeight: '400px', overflow: 'auto' }"
          allow-clear
          :tree-data="interfaceOptions"
          tree-node-filter-prop="label"
          :treeDefaultExpandAll="false"
        />
      </a-form-item>
      <a-form-item label="备注">
        <a-input v-model:value="itemFormState.remark" />
      </a-form-item>
      <a-form-item label="图标">
        <!-- <a-input v-model:value="itemFormState.icon" style="width: 200px" /> -->
        <div style="display: flex">
          <a-button style="width: 100%" @click="openModal">选择Icon</a-button>
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
      <!-- <a-form-item label="按钮ID">
        <a-input v-model:value="itemFormState.buttonId" style="width: 200px" />
      </a-form-item>
      <a-form-item label="按钮方法名">
        <a-input v-model:value="itemFormState.buttonFunName" style="width: 200px" />
      </a-form-item> -->
      <a-form-item v-if="itemFormState.function == 2" label="按钮操作事件">
        <a-input v-model:value="itemFormState.buttonActionName" />
      </a-form-item>
      <a-form-item label="排序" name="sort">
        <a-input-number v-model:value="itemFormState.sort" :min="0" style="width: 100%" />
      </a-form-item>
      <a-form-item label="菜单类型" name="menuType">
        <a-select v-model:value="itemFormState.menuType" :size="size" ref="select" :options="menuTypeOptions"></a-select>
      </a-form-item>
      <a-form-item label="是否激活">
        <a-radio-group v-model:value="itemFormState.isActive">
          <a-radio :value="true">是</a-radio>
          <a-radio :value="false">否</a-radio>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="是否启用缓存">
        <a-radio-group v-model:value="itemFormState.isCache">
          <a-radio :value="true">是</a-radio>
          <a-radio :value="false">否</a-radio>
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
import { ref, onMounted, h, reactive, watch, createVNode, watchEffect, computed } from 'vue'
import CustomButtonListTable from '@/components/commonComponents/CustomButtonListTable.vue'
import { getButtonList } from '../../api/commonFun'
import { message, Modal } from 'ant-design-vue'
import * as Icons from '@ant-design/icons-vue'
import {
  getAllSysInfoTreeApi,
  getSysPermissionListApi,
  getSysInterfaceBySysIdApi,
  savePermissionApi,
  deleteApi,
  getPermissionDetailApi,
  tableExportApi,
  tableImportApi,
  getUserListApi
} from '@/api/permissionConfig'
import { useGetRolesAsync } from '@/api/permission'
import { SearchOutlined, RedoOutlined, ExclamationCircleOutlined, CaretDownOutlined, CaretRightOutlined } from '@ant-design/icons-vue'
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
  name: 'A0109'
})
const inputFunctionName = ref('A0109')
//获取页面动态按钮列表（处理table列表内按钮，降低查询频次）
const allDyBtn = ref([])
async function getAllButton() {
  const obj = {
    menuCode: 'A0109',
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
  // if (functionName === 'tableImport') {
  //   tableImport()
  // }
  if (functionName === 'tableExport') {
    tableExport()
  }
  if (functionName === 'expand') {
    expand()
  }
  if (functionName === 'expandClose') {
    expandClose()
  }
  if (functionName === 'userPermisseionShow') {
    userPermisseionShow()
  }
  if (functionName === 'search') {
    search()
  }
  if (functionName === 'showDrawer') {
    showDrawer()
  }
  if (functionName === 'resetSearch') {
    resetSearch()
  }
}

const sysTreeData = ref([
  {
    title: '所属系统',
    children: []
  }
])
const spinning = ref(false)
const itemSpinning = ref(false)
onMounted(() => {
  getAllSysInfoTree()
  getAllButton()
})

const myDiv = ref(null)
const myTable = ref(null)

const sysId = ref()
async function sysSelect(selectedKeys) {
  if (selectedKeys[0] !== '0-0') {
    sysId.value = selectedKeys[0]
    getSysPermissionList(routeName.value, 1, pageSize.value, sysId.value)
  }
}

//查询系统名称树数据
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

//表格导出
const tableExport = async () => {
  console.info(!sysId.value)
  if (!sysId.value) {
    message.warning('请选择要导出权限的系统！')
    return
  }
  const params = {
    id: sysId.value
  }
  const response = await tableExportApi(params)
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
  if (!sysId.value) {
    message.warning('请选择要导入权限的系统！')
    return
  }
  try {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('id', sysId.value)
    const response = await tableImportApi(formData)
    console.info('导入接口：', response)
    if (response.status === 200 && response.statusText === 'OK') {
      message.success('导入成功')
      getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
    }
    return false
  } catch (error) {
    console.info(error)
    message.error(`导入失败`)
    return false
  }
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
//获取权限列表
async function getSysPermissionList(name, page, size, sysId) {
  const query = {
    name: name,
    pageIndex: page,
    pageSize: size,
    sysId: sysId
  }
  const response = await getSysPermissionListApi(query)
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

const dictId = ref()
//详情
async function detail(id) {
  permissionOpen.value = true
  getSysInterfaceBySysId(sysId.value)
  itemDrawerTitle.value = '详情'
  itemFormState.id = ''
  itemFormState.pid = ''
  itemFormState.functionCode = ''
  itemFormState.functionName = ''
  itemFormState.englishName = ''
  itemFormState.route = ''
  itemFormState.routeFile = ''
  itemFormState.interfaceId = ''
  itemFormState.remark = ''
  itemFormState.icon = ''
  selectedIcon.value = ''
  itemFormState.function = ''
  itemFormState.buttonId = ''
  itemFormState.buttonFunName = ''
  itemFormState.buttonActionName = ''
  itemFormState.sort = 0
  itemFormState.menuType = ''
  itemFormState.isActive = false
  itemFormState.isCache = false
  itemFormState.sysInfoID = sysId.value

  const params = {
    id: id
  }
  const response = await getPermissionDetailApi(params)
  if (response.code === 200 && response.success) {
    itemFormState.id = response.data.id
    itemFormState.pid = response.data.pid
    itemFormState.functionCode = response.data.functionCode
    itemFormState.functionName = response.data.functionName
    itemFormState.englishName = response.data.englishName
    itemFormState.route = response.data.route
    itemFormState.routeFile = response.data.routeFile
    itemFormState.interfaceId = response.data.interfaceId
    itemFormState.remark = response.data.remark
    itemFormState.icon = response.data.icon
    selectedIcon.value = response.data.icon
    itemFormState.function = response.data.function
    itemFormState.buttonId = response.data.buttonId
    itemFormState.buttonFunName = response.data.buttonFunName
    itemFormState.buttonActionName = response.data.buttonActionName
    itemFormState.sort = response.data.sort
    itemFormState.menuType = response.data.menuType
    itemFormState.isActive = response.data.isActive
    itemFormState.isCache = response.data.isCache
    itemFormState.sysInfoID = response.data.sysInfoID
  }
}

//查询
const routeName = ref('')
function search() {
  getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
}
//重置查询
function resetSearch() {
  routeName.value = ''
  getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
}
//删除
// async function del(id) {
//   const params = {
//     id: id
//   }
//   console.info(params)
//   const response = await deleteApi(params);
//   if (response.code === 200 && response.success) {
//     message.success('删除成功')
//     getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
//   }
// }
//删除
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
async function del(resolve, reject, id) {
  const params = {
    id: id
  }
  await deleteApi(params).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
    } else {
      message.warning(res.message)
    }
  })
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getSysPermissionList(routeName.value, current, pageSize, sysId.value)
}
const pageSizeChange = (page, pageSize) => {
  getSysPermissionList(routeName.value, page, pageSize, sysId.value)
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
    dataIndex: 'functionCode',
    key: 'functionCode',
    //width: 140,
    fixed: 'left',
    align: 'center'
  },
  {
    title: '菜单名称',
    dataIndex: 'functionName',
    key: 'functionName',
    width: 250,
    fixed: 'left',
    align: 'center'
  },
  // {
  //   title: '英文名称',
  //   dataIndex: 'englishName',
  //   key: 'englishName',
  //   width: 250,
  //   align: 'center'
  // },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort',
    width: 80,
    align: 'center'
  },
  {
    title: '路由',
    dataIndex: 'route',
    key: 'route',
    width: 200,
    align: 'center'
  },
  {
    title: '路由文件',
    dataIndex: 'routeFile',
    key: 'routeFile',
    width: 200,
    align: 'center'
  },
  {
    title: '接口名称',
    dataIndex: 'interfaceName',
    key: 'interfaceName',
    //width: 180,
    align: 'center'
  },
  {
    title: '接口地址',
    dataIndex: 'interfaceAddress',
    key: 'interfaceAddress',
    //width: 80,
    align: 'center'
  },
  {
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    //width: 80,
    align: 'center'
  },
  {
    title: '图标',
    dataIndex: 'icon',
    key: 'icon',
    //width: 80,
    align: 'center'
  },
  {
    title: '功能类型',
    dataIndex: 'function',
    key: 'function',
    //width: 80,
    align: 'center'
  },
  {
    title: '按钮ID',
    dataIndex: 'buttonId',
    key: 'buttonId',
    //width: 80,
    align: 'center'
  },
  {
    title: '按钮方法名',
    dataIndex: 'buttonFunName',
    key: 'buttonFunName',
    //width: 80,
    align: 'center'
  },
  {
    title: '按钮操作事件',
    dataIndex: 'buttonActionName',
    key: 'buttonActionName',
    //width: 180,
    align: 'center'
  },
  {
    title: '是否激活',
    dataIndex: 'isActive',
    key: 'isActive',
    //width: 180,
    align: 'center'
  },
  {
    title: '是否启用缓存',
    dataIndex: 'isCache',
    key: 'isCache',
    //width: 180,
    align: 'center'
  },
  {
    title: '菜单类型',
    dataIndex: 'menuType',
    key: 'menuType',
    width: 160,
    align: 'center'
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
    title: '用户名称',
    dataIndex: 'realName',
    key: 'realName',
    fixed: 'left',
    width: 100,
    align: 'center'
  },
  {
    title: '用户性别',
    dataIndex: 'sex',
    key: 'sex',
    width: 100,
    align: 'center'
  },
  {
    title: '用户账号',
    dataIndex: 'code',
    key: 'code',
    width: 100,
    align: 'center'
  },
  {
    title: '用户状态',
    dataIndex: 'enabled',
    width: 100,
    key: 'enabled',
    align: 'center'
  },
  {
    title: '所属部门',
    dataIndex: 'code',
    key: 'code',
    width: 100,
    align: 'center'
  },
  {
    title: '所在角色组',
    dataIndex: 'code',
    key: 'code',
    width: 100,
    align: 'center'
  },
  {
    title: '邮箱',
    dataIndex: 'email',
    key: 'email',
    width: 100,
    align: 'center'
  },
  {
    title: '手机',
    dataIndex: 'phone',
    key: 'phone',
    width: 100,
    align: 'center'
  },
  {
    title: '微信',
    dataIndex: 'webChat',
    key: 'webChat',
    width: 100,
    align: 'center'
  },
  {
    title: '钉钉',
    dataIndex: 'dingTalk',
    key: 'dingTalk',
    width: 100,
    align: 'center'
  },
  {
    title: '最后登录时间',
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
    title: '备注',
    dataIndex: 'remark',
    key: 'remark',
    width: 180,
    align: 'center'
  }
]
const itemData = ref(null)
//查询用户列表
const userName = ref()

async function getUserList(name, page, size, roleId, permissionId) {
  const query = {
    name: name,
    pageIndex: page,
    pageSize: size,
    roleId: roleId,
    permissionId: permissionId
  }
  const response = await getUserListApi(query)
  if (response.code === 200 && response.success) {
    itemData.value = response.data
    totalItem.value = response.total
  }
}
//查询用户
function searchUser() {
  getUserList(userName.value, currentItem.value, pageSizeItem.value, roleId.value, permissionId.value)
}
//重置查询
function resetSearchUser() {
  userName.value = ''

  getUserList(userName.value, currentItem.value, pageSizeItem.value, roleId.value, permissionId.value)
}

//打开用户权限列表模态框
const open = ref(false)
const permissionId = ref()
const userPermisseionShow = async (id) => {
  if (rowSelection.value.selectedRowKeys.length === 0) {
    message.warning('请选择要查看的权限！')
    return
  }
  permissionId.value = rowSelection.value.selectedRowKeys[0]

  open.value = true
  itemData.value = []
  totalItem.value = 0
  roleData()
}
const roleId = ref()
async function roleSelect(selectedKeys) {
  roleId.value = selectedKeys[0]

  getUserList(userName.value, currentItem.value, pageSizeItem.value, roleId.value, permissionId.value)
}

//查角色
function roleSearch() {
  roleData(roleName.value)
}
//角色调用并追截数据逻辑
const roleName = ref()
const roleData = async (roleNam) => {
  const data = await useGetRolesAsync(roleNam)
  if (data.code === 200 && data.success) {
    roleStructure(data.data)
  } else {
    message.error('角色查询失败!')
  }
}
const role_treeData = ref([])
const roleStructure = (data) => {
  role_treeData.value = []
  data.forEach((parent) => {
    const obj = {
      title: parent.name,
      key: parent.id,
      children: []
    }
    if (parent.children && parent.children.length > 0) {
      parent.children.forEach((cil) => {
        obj.children.push({
          title: cil.name,
          key: cil.id
        })
      })
    }
    role_treeData.value.push(obj)
  })
}

const sysList = ref([])
//退出模态框
function exit() {
  open.value = false
  roleId.value = null
  permissionId.value = null
}

const permissionOpen = ref(false)

const interfaceOptions = ref([])
//获取接口下拉框数据
async function getSysInterfaceBySysId(sysIdV) {
  const query = {
    sysId: null
  }
  const response = await getSysInterfaceBySysIdApi(query)
  console.info(response)
  if (response.code === 200 && response.success) {
    dgInterface(response.data)
    interfaceOptions.value = response.data
  }
}
function dgInterface(obj) {
  obj.forEach((e) => {
    e.label = e.name
    e.value = e.id
    if (e.children !== null) {
      dgInterface(e.children)
    }
  })
}
//打开抽屉
const showDrawer = async (id) => {
  if (!sysId.value) {
    message.warning('请先选择左侧系统')
    return
  }
  getSysInterfaceBySysId(sysId.value)
  permissionOpen.value = true
  itemDrawerTitle.value = '新增'
  itemFormState.id = ''
  itemFormState.pid = id
  itemFormState.functionCode = ''
  itemFormState.functionName = ''
  itemFormState.englishName = ''
  itemFormState.route = ''
  itemFormState.routeFile = ''
  itemFormState.interfaceId = ''
  itemFormState.remark = ''
  itemFormState.icon = ''
  selectedIcon.value = ''
  itemFormState.function = ''
  itemFormState.buttonId = ''
  itemFormState.buttonFunName = ''
  itemFormState.buttonActionName = ''
  itemFormState.sort = 0
  itemFormState.menuType = ''
  itemFormState.isActive = false
  itemFormState.isCache = false
  itemFormState.sysInfoID = sysId.value
}
const functionOptions = ref([
  {
    value: 1,
    label: '菜单'
  },
  {
    value: 2,
    label: '按钮'
  },
  {
    value: 3,
    label: '接口'
  }
])
const menuTypeOptions = ref([
  {
    value: 1,
    label: '内部系统'
  },
  {
    value: 2,
    label: '外部系统'
  }
])
const itemDrawerTitle = ref('')
const itemFormState = reactive({
  id: '',
  functionCode: '',
  functionName: '',
  englishName: '',
  route: '',
  routeFile: '',
  interfaceId: '',
  remark: '',
  icon: '',
  function: '',
  buttonId: '',
  buttonFunName: '',
  buttonActionName: '',
  sort: 0,
  menuType: '',
  isActive: false,
  isCache: false,
  sysInfoID: ''
})
const itemFormRef = ref()
const itemLabelCol = {
  style: {
    width: '100px'
  }
}
//退出子项抽屉
function exitItem() {
  permissionOpen.value = false
}

const validate = async (_rule, value) => {
  if (value.trim() === '') {
    return Promise.reject('这是必填项')
  } else {
    return Promise.resolve()
  }
}
const rules = {
  functionCode: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  functionName: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  interfaceId: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ],
  route: [
    {
      required: true,
      validator: validate,
      trigger: 'change'
    }
  ],
  menuType: [
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
  function: [
    {
      required: true,
      message: '这是必填项',
      trigger: 'change'
    }
  ]
}
//保存权限
const saveItem = async () => {
  itemFormRef.value.validate().then(async () => {
    if (itemFormState.function == 1) {
      if (itemFormState.route.substring(0, 1) != '/') {
        message.warning('路由必须以“/”开头')
        return
      }
    }
    const response = await savePermissionApi(itemFormState)
    if (response.code === 200 && response.success) {
      message.success('保存成功')
      getSysPermissionList(routeName.value, current.value, pageSize.value, sysId.value)
      permissionOpen.value = false
    } else if (response.code === 200 && !response.success) {
      message.warning(response.message)
    }
  })
}

//用户分页
const currentItem = ref(1)
const pageSizeItem = ref(10)
const totalItem = ref(0)
const onShowSizeChangeItem = (currentItem, pageSizeItem) => {
  getUserList(userName.value, currentItem.value, pageSizeItem.value, roleId.value, permissionId.value)
}
const pageSizeChangeItem = (page, pageSizeItem) => {
  getUserList(userName.value, page.value, pageSizeItem.value, roleId.value, permissionId.value)
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
  itemFormState.icon = iconName
}
</script>

<style lang="scss">
.PermissionInfoConfig {
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

  position: relative;
  display: flex;
  padding-top: 10px;

  .leftModel {
    width: 20%;
    border-right: 1px solid gainsboro;
    padding: 0;
  }

  .rightModel {
    width: 80%;
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
