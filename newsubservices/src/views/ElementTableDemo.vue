<template>
  <div class="ElementTableDemo">
    <div style="height: 50px; line-height: 50px">
      <el-input v-model="UserNameKey" style="width: 240px; margin: 10px 20px" placeholder="请输入姓名" clearable />
      <el-button plain @click="getTableData">查询数据</el-button>
      <el-button type="primary" plain @click="OpenDialog">添加用户</el-button>
    </div>

    <el-table ref="tableRef" row-key="date"  :data="tableData" height="calc(100vh - 96px - 35px - 50px - 50px)">
      <el-table-column align="center" prop="name" label="姓名" width="280" />
      <el-table-column align="center" prop="address" label="地址" min-width="200" />
      <el-table-column align="center" prop="createName" label="创建人" min-width="200" />
      <el-table-column align="center" prop="createTime" label="创建时间" min-width="200">
        <template #default="scope">
          {{ formatDateTime(scope.row.createTime) }}
        </template>
      </el-table-column>
      <el-table-column align="center" prop="modifyName" label="修改人员" min-width="200" />
      <el-table-column align="center" prop="modifyTime" label="修改时间" min-width="200">
        <template #default="scope">
          <span v-if="scope.row.modifyTime">
            {{ formatDateTime(scope.row.modifyTime) }}
          </span>
          <span v-else>
            {{ scope.row.modifyTime }}
          </span>
        </template>
      </el-table-column>

      <el-table-column align="center" fixed="right" label="操作" min-width="120">
        <template #default="scope">
          <el-button link type="primary" size="small" @click="Edit(scope.row)">编辑</el-button>
          <el-button link type="primary" size="small" @click="Delete(scope.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination
      class="paginationStyle"
      v-model:current-page="modalCurrentPage"
      v-model:page-size="modalCurrentPageSize"
      :page-sizes="[15, 20, 30, 50, 100]"
      layout="sizes, prev, pager, next"
      :total="modalTotalCount"
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
    />

    <el-dialog v-model="dialogFormVisible" :title="dialogTitle" width="500">
      <el-form :model="form">
        <el-form-item label="姓名" :label-width="formLabelWidth">
          <el-input v-model="form.name" autocomplete="off" style="width: 280px" />
        </el-form-item>
        <el-form-item label="地址" :label-width="formLabelWidth">
          <el-input v-model="form.address" autocomplete="off" style="width: 280px" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="dialogFormVisible = false">取消</el-button>
          <el-button type="primary" @click="handleOk"> 确认 </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, reactive } from 'vue'
import { getUserInfo, useAddUserAsync, modifySysJob, deleteApi } from '@/api/demoApi/demo.js'
import { formatDateTime } from '@/utils/dateUtils'

interface User {
  date: string
  name: string
  address: string
  tag: string
}

const tableRef = ref<TableInstance>()
const UserNameKey: string = ref('')
const resetDateFilter = () => {
  tableRef.value!.clearFilter(['date'])
}
const clearFilter = () => {
  tableRef.value!.clearFilter()
}
const formatter = (row: User, column: TableColumnCtx<User>) => {
  return row.address
}
const filterTag = (value: string, row: User) => {
  return row.tag === value
}

const tableData = ref<User[]>()
var modalCurrentPage = ref(1)
var modalCurrentPageSize = ref(15)
var modalTotalCount = ref(0)

/* 获取用户列表 */

async function getTableData() {
  var obj = {
    name: UserNameKey.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  await getUserInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      tableData.value = res.data
      modalTotalCount.value = res.total
    }
    ElMessage({
      type: 'success',
      message: res.message
    })
  })
}

const handleSizeChange = (val: number) => {
  modalCurrentPageSize.value = val
  getTableData()
}
const handleCurrentChange = (val: number) => {
  modalCurrentPage.value = val
  getTableData()
}

/* Form部分 */
const dialogFormVisible = ref(false)
const form = reactive({
  name: '',
  address: ''
})
const formLabelWidth = '120px'
const dialogTitle = ref('')

function OpenDialog() {
  form.id = ''
  form.name = ''
  form.address = ''
  dialogTitle.value = '添加用户信息'
  dialogFormVisible.value = true
}
function Edit(row) {
  form.name = row.name
  form.address = row.address
  form.id = row.id
  dialogFormVisible.value = true
  dialogTitle.value = '编辑用户'
}

function handleOk() {
  if (form.id) {
    ModifyUser()
  } else {
    AddUser()
  }
}

async function AddUser() {
  await useAddUserAsync(form).then((res) => {
    if (res.code == 200 && res.success) {
      dialogFormVisible.value = false
      form.value = reactive({
        name: '',
        address: ''
      })
      getTableData()
    }
  })
}
async function ModifyUser() {
  await modifySysJob(form).then((res) => {
    if (res.code == 200 && res.success) {
      dialogFormVisible.value = false
      form.value = reactive({
        name: '',
        address: ''
      })
      getTableData()
    }
  })
}

function Delete(id) {
  ElMessageBox.confirm('确认删除该用户信息吗?', 'Warning', {
    confirmButtonText: '确认',
    cancelButtonText: '取消',
    type: 'warning'
  })
    .then(() => {
      DelUser(id)
    })
    .catch(() => {
      ElMessage({
        type: 'info',
        message: '取消删除'
      })
    })
}
async function DelUser(id) {
  const params = {
    id: id
  }
  await deleteApi(params).then((res) => {
    if (res.code === 200 && res.success) {
      ElMessage({
        type: 'success',
        message: res.message
      })
      getTableData()
    }
  })
}

onMounted(() => {
  getTableData()
})
</script>
<style lang="scss">
.ElementTableDemo {
  height: 100%;
  position: relative;

  .paginationStyle {
    height: 50px;
    line-height: 50px;
    position: absolute;
    padding-right: 30px;
    right: 0px;
    bottom: 0px;
  }
}
</style>
