<template>
    <div class="SurelyTableDem">
    <div style="display: flex; align-items: center; justify-content: space-between; height: 36px; line-height: 36px; margin-top: 17px">
      <a-typography-paragraph style="overflow: auto; padding-left: 20px">
        <blockquote style="color: black; border-inline-start: 5px solid rgb(37, 97, 239) !important">
          <span style="font-weight: bold; color: #111111; font-size: 16px">国际化菜单配置</span>
        </blockquote>
      </a-typography-paragraph>
      <div style="padding-right: 20px">
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(UploadOutlined)" />
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :type="''" :icon="h(DownloadOutlined)" />
        <a-button style="margin: 0px 20px 0px 10px" shape="circle" :icon="h(FilterOutlined)" :type="isExpand ? 'primary' : ''" @click="toggleAccordion()" />
        <a-button style="margin-left: 10px; width: 100px; background-color: rgb(36, 98, 167)" :icon="h(PlusOutlined)" @click="openUserModal()" type="primary">添加</a-button>
      </div>
    </div>

    <div>
      <a-divider />
    </div>
    <div class="accordion" v-show="isExpand">
      <div ref="accordionRef" class="accordion-content">
        <div> <span style="margin-left: 20px"> 语言名称:  <a-input v-model:value="nameInput" style="width: 200px" allowClear /></span> <span style="margin-left: 20px"><a-button type="primary" style="margin-left: 10px; width: 120px; background-color: rgb(36, 98, 167)" :icon="h(SearchOutlined)" @click="getTableData()">查询</a-button></span> </div>
      </div>
    </div>
     
  <a-table 
  :columns="columns" 
  :scroll="{ x: 1500 }" 
  :height="calcHeight" 
  :pagination="false" 
  rowKey="code" 
  :data-source="dataSource" stripe>
    <template #bodyCell="{ text, column, record }">
      <template v-if="column.key === 'operation'">
        <a @click="Edit(record)">
          <a-button type="primary" :icon="h(EditOutlined)" style="display: inline; margin-right: 10px"></a-button>
        </a>
        <a @click="Delete(record.code)">
          <a-button :icon="h(DeleteOutlined)"  type="primary" danger style="display: inline; margin-right: 10px"></a-button>
        </a>
      </template>
    </template>
  </a-table>
    <div class="paginationStyle" style="margin-top: 10px">
    <a-pagination
      v-model:current="modalCurrentPage"
      v-model:page-size="modalCurrentPageSize"
      :total="modalTotalCount"
      align="right"
      :show-total="(total) => `共 ${modalTotalCount} 条`"
      @change="modalPageSizeChange"
    />
  </div>
  </div>

  <a-drawer  :footer-style="{ textAlign: 'center' }" :width="drawerwhitch" v-model:open="openModal" :title="ModalTitle" @ok="handleOk" ok-text="确认" cancel-text="取消" centered>
    <template #extra>
      <a-button :icon="h(Icons[icon])" style="margin-right: 8px" @click="change">{{ btnname }}</a-button>
    </template>
    <a-form :model="formState" :label-col="labelCol" :wrapper-col="wrapperCol">
        <a-form-item label="语言编码">
        <a-input v-model:value="formState.code" />
      </a-form-item>
      <a-form-item label="是否默认语言">
        <div>
          <a-radio-group v-model:value="formState.isDefault">
            <a-radio :value="true">是</a-radio>
            <a-radio :value="false">否</a-radio>
          </a-radio-group>
        </div> </a-form-item
      ><a-form-item label="语言名称">
        <a-input v-model:value="formState.name" />
      </a-form-item><a-form-item label="排序">
        <a-input v-model:value="formState.sort" />
      </a-form-item>
    </a-form>
    <template #footer
      ><a-button type="primary" @click="handleOk" style="margin-right: 10px">保存</a-button>
      <a-button @click="exitItem">返回</a-button>
    </template>
  </a-drawer>
</template>
<script setup>
import { SearchOutlined, PlusOutlined, DeleteOutlined, EditOutlined, DownloadOutlined, UploadOutlined, FilterOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import { ref, h, watchEffect, reactive, createVNode, computed, nextTick } from 'vue'
import { useGetLanguageApi, useInsertApi, useUpdate, useDelete } from '@/api/language.js'
import { message, Modal } from 'ant-design-vue'
import dayjs from 'dayjs'
import * as Icons from '@ant-design/icons-vue'

//筛选按钮
const panelHeight = ref(0)
const calcHeight = computed(() => {
  return `calc(100vh - 96px - 53px - 21px - ${panelHeight.value}px - 50px - 35px)`
})
// 监听窗口变化，动态调整面板高度
const updatePanelHeight = async () => {
  if (isExpand.value) {
    panelHeight.value = 42
  } else {
    panelHeight.value = 0
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

const btnname = ref('展开')
const icon = ref('ArrowsAltOutlined')
const drawerwhitch = ref('700')
const change = () => {
  if (btnname.value == '展开') {
    btnname.value = '还原'
    icon.value = 'ShrinkOutlined'
    drawerwhitch.value = '100%'
  } else {
    btnname.value = '展开'
    icon.value = 'ArrowsAltOutlined'
    drawerwhitch.value = '700'
  }
}

const nameInput = ref("");
const dataSource = ref([])
const modalCurrentPage = ref(1)
const modalCurrentPageSize = ref(10)
const modalTotalCount = ref(0)

const columns = [
      {
    title: '序号',
    dataIndex: 'key',
    align: 'center',
    customRender: (obj) => {
      return (modalCurrentPage.value - 1) * modalCurrentPageSize.value + obj.index + 1
    }
  },
  {
    title: '是否默认语言',
    dataIndex: 'isDefault',
     align: 'center',
    key: 'isDefault',
    customRender: (obj) => {
        if(obj.text == true){
            return '是'
        }else{
           return '否'
        }
      
    }
  },{
    title: '语言名称',
     align: 'center',
    dataIndex: 'name',
    key: 'name'
  },{
    title: '排序',
     align: 'center',
    dataIndex: 'sort',
    key: 'sort'
  },
  {
    title: '操作',
    key: 'operation',
     align: 'center',
    fixed: 'right',
    width: 140
  }
]

/* Form表单 */
const resetForm = {
 isDefault: true,isDeleted: false,name: '',code: '',sort: 0 
}
let formState

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData()
}

const ModalTitle = ref('')
const openModal = ref(false)
function Edit(row) {
  formState = reactive({ ...row })
  
  openModal.value = true
  ModalTitle.value = '编辑'
}

const labelCol = {
  style: {
    width: '120px'
  }
}
const wrapperCol = {
  span: 14
}

async function getTableData() {
  var obj = {
    Name:nameInput.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await useGetLanguageApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      dataSource.value = res.data
      modalTotalCount.value = res.total
      //message.success(res.message)
    }
  })
}

function openUserModal() {
  formState = reactive({ ...resetForm })
  
  openModal.value = true
  ModalTitle.value = '添加'
}

function handleOk() {
    if (formState.code == '' ) {
        message.warning('语言编码不能为空')
        return;
    }
    if (formState.name == '') {
        message.warning('语言名称不能为空')
        return;
    }
  if (ModalTitle.value == '添加') {
     AddUser()
   
  } else {
    ModifyUser()
  }
}

const exitItem = () => {
  openModal.value = false
}

async function AddUser() {
  await useInsertApi(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('添加成功!')
      openModal.value = false
      formState = reactive({ ...resetForm })
      getTableData()
    }
  })
}
async function ModifyUser() {
  await useUpdate(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('信息已更新!')
      openModal.value = false
      formState = reactive({ ...resetForm })
      getTableData()
    }
  })
}

//删除
function Delete(id) {
  Modal.confirm({
    title: '删除提醒',
    icon: createVNode(ExclamationCircleOutlined),
    content: '确认删除该项吗？',
    okText: '确认',
    cancelText: '取消',
    onOk() {
      return new Promise((resolve, reject) => {
        const res = DelUser(resolve, reject, id)
        return res
      }).catch(() => message.error('删除异常!'))
    }
  })
}

async function DelUser(resolve, reject, id) {
  await useDelete(id).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData()
    }
  })
}

getTableData()
</script>
<style lang="scss">
.SurelyTableDem {
  background-color: #fff;
  height: 100%;
  overflow: auto;
  position: relative;

  .ant-typography {
    margin-bottom: 0px;
  }

  .ant-divider-horizontal {
    margin: 10px 0px;
  }

  .accordion {
    overflow: hidden;

    .accordion-content {
      max-height: 50px;
      overflow: hidden;
      transition: max-height 0.3s ease-in-out, padding 0.1s ease-in-out;
      background: #fff;
      padding: 0 10px;
      margin-bottom: 10px;

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

  .paginationStyle {
    height: 50px;
    line-height: 50px;
    text-align: right;
    position: absolute;
    padding-top: 10px;
    padding-right: 30px;
    right: 0px;
    bottom: 0px;
    width: 100%;
  }
}
</style>

