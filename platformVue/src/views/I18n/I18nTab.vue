<template>
  <div class="all">
    <div class="left">
      <div class="head">
        <a-input-search v-model:value="treeInput" @search="user_qurety" />
        <a-button :icon="h(PlusOutlined)" style="margin-left: 5px" type="primary" @click="addData()">添加</a-button>
      </div>
      <div class="lefttree">
        <a-tree v-model:expandedKeys="expandedKeys" v-model:selectedKeys="selectedKeys" :tree-data="treeData">
          <template #title="{ title, key, data }"> {{ title }}<a-button v-if="data.children.length == 0" :size="'small'"
              :icon="h(DeleteOutlined)" @click.stop="treeDel(key)" danger
              style="height: 22px; margin-left: 5px"></a-button> </template>
        </a-tree>
      </div>
    </div>
    <div class="right">
      <div class="head"><a-button :icon="h(SaveOutlined)" style="margin-left: 5px" type="primary"
          @click="saveData()">保存</a-button></div>
      <div class="rightcontent">
        <a-form v-bind="layout" name="nest-messages">
          <a-form-item label="标签编码">
            <!-- <a-input  v-model:value="formState.code" /> -->

            <a-input v-model:value="formState.code" :style="isExit ? { border: '1px solid red' } : {}">
              <template #suffix>
                <a-tooltip v-if="isExit" title="标签编码已存在，请重新输入">
                  <ExclamationCircleOutlined style="color: red; font-size: 14px;" />
                </a-tooltip>
              </template>
            </a-input>
          </a-form-item>


          <a-form-item label="标签名称">
            <a-input v-model:value="formState.tagName" />
          </a-form-item>
          <a-form-item label="上级">
            <div>
              <a-tree-select v-model:value="formState.pid" v-model:searchValue="searchValue" show-search
                style="width: 100%" :dropdown-style="{ maxHeight: '200px', overflow: 'auto' }" placeholder="请选择上级"
                allow-clear tree-default-expand-all :tree-data="selectPData" tree-node-filter-prop="label">
              </a-tree-select>
            </div>
          </a-form-item>
        </a-form>
      </div>
    </div>
  </div>
</template>
<script setup>
import { PlusOutlined, SaveOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import { ref, reactive, h, watch, createVNode } from 'vue'
import dayjs from 'dayjs'
import { message, Modal } from 'ant-design-vue'
import { useGetTabApi, useInsertApi, useUpdate, useDelete, useGetById } from '@/api/I18nTab.js'
const layout = {
  labelCol: {
    span: 4
  },
  wrapperCol: {
    span: 16
  }
}
const treeInput = ref('')
const user_qurety = () => {
  treeDate(treeInput.value)
}
const treeData = ref([])
const selectedKeys = ref([])
const expandedKeys = ref([])



watch(selectedKeys, () => {
  if (selectedKeys.value.length > 0) {
    byid(selectedKeys.value[0])
  }

})
const treeDate = async (name) => {
  const data = await useGetTabApi('', name)
  if (data.code === 200 && data.success) {
    treeData.value = data.data
    expandedKeys.value = [data.data[0].key]
  } else {
    message.error('查询失败!')
  }
}
treeDate('')
/* Form表单 */
const resetForm = {
  code: '', pid: '', tagName: '', id: ''
}
const formState = reactive({
  code: '', pid: '', tagName: '', id: ''
})
const selectPData = ref([])
const searchValue = ref('')

const Pdata = async (id) => {
  const data = await useGetTabApi(id, '')
  if (data.code === 200 && data.success) {
    selectPData.value = []
    data.data.forEach((or) => {
      selectPData.value.push(convertTree(or))
    })
  } else {
    message.error(data.message)
  }
}
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    label: data.title,
    value: data.key,
    children: []
  }
  if (data.children && data.children.length) {
    data.children.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}
const addData = () => {
  console.log('1', selectedKeys.value);
  Object.assign(formState, resetForm)

  Pdata('')
  if (selectedKeys.value.length > 0) {
    formState.pid = selectedKeys.value[0]
  }
  console.log(formState);


}
const byid = async (id) => {
  const data = await useGetById(id)
  if (data.code === 200 && data.success) {
    Object.assign(formState, data.data)

    Pdata(id)
  } else {
    message.error(data.message)
  }
}

const saveData = () => {
  if (formState.id) {
    ModifyUser()
  } else {
    AddTag()
  }
}

const isExit = ref(false)

async function AddTag() {
  await useInsertApi(formState).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('添加成功!')
      Object.assign(formState, resetForm)
      treeDate('')
      selectedKeys.value = []
    } else {
      if (!res.success) {
        if (res.message == 'ERROR01') {
          isExit.value = true
          message.error("标签编码已存在，请重新输入.")
          return
        }
        message.error(res.message)
      }

    }
  })
}
async function ModifyUser() {
  await useUpdate(formState).then((res) => {
    if (res.code == 200 && res.success) {
      Object.assign(formState, resetForm)
      treeDate('')
      selectedKeys.value = []

      message.success('修改成功!')
    }
  })
}
async function del(resolve, reject, id) {
  await useDelete(id).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success('删除成功')
      treeDate(treeInput.value)
    }
  })
}
const treeDel = (id) => {
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
.all {
  width: 100%;
  height: 100%;
  display: flex;
}

.left {
  width: 35%;
  background-color: rgb(255, 255, 255);
  height: 100%;
}

.head {
  width: 100%;
  height: 50px;
  padding: 10px 10px;
  background-color: rgb(255, 255, 255);
  display: flex;
  align-items: center;
}

.lefttree {
  width: 100%;
  height: calc(100% - 90px);
  margin: 25px 0 0 0;
  padding: 0 8px;
  overflow-y: auto;
}

.right {
  width: 65%;
  height: 100%;
}

.rightcontent {
  width: 100%;
  height: calc(100% - 90px);
  overflow: auto;
  margin: 25px 0 0 0;
  padding: 0 8px;
}
</style>
