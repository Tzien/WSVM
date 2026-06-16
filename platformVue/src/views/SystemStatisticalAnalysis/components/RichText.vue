<template>
  <div>
    <a-form ref="formRef" :model="formState" :label-col="labelCol">
      <div style="display: flex; height: 34px; line-height: 34px; margin-bottom: 10px">
        <a-form-item
          label="系统名称"
          name="sysId"
          :rules="[
            { required: true, message: '请选择系统!' },
            { validator: validateSysId, trigger: 'blur' }
          ]"
          validateTrigger="blur"
        >
          <a-select v-model:value="formState.sysId" show-search allow-clear placeholder="请选择系统" style="width: 200px" :options="sysNameOptions"></a-select>
        </a-form-item>
        <a-form-item
          label="版本号"
          name="version"
          :rules="[
            { required: true, message: '请输入版本号!' },
            { validator: validateVersion, trigger: 'blur' }
          ]"
          validateTrigger="blur"
        >
          <a-input v-model:value="formState.version" placeholder="请输入版本号" />
        </a-form-item>
        <a-form-item label="消息类型" name="messageType">
          <a-select
            v-model:value="formState.messageType"
            show-search
            allow-clear
            placeholder="请选择消息类型"
            style="width: 200px"
            :options="messageOptions"
          ></a-select>
        </a-form-item>
      </div>
      <div style="display: flex">
        <div style="width: 70%">
          <div style="margin-top: 10px">
            <a-form-item
              label="更新标题"
              name="title"
              :rules="[
                { required: true, message: '请输入更新标题!' },
                { validator: validateTitle, trigger: 'blur' }
              ]"
              validateTrigger="blur"
            >
              <a-input v-model:value="formState.title" placeholder="请输入更新标题" />
            </a-form-item>
          </div>
          <a-form-item label="更新内容" name="content">
            <div style="border: 1px solid #ccc; height: 300px">
              <Toolbar style="border-bottom: 1px solid #ccc" :editor="editorRef" :defaultConfig="toolbarConfig" :mode="toolBarMode" />
              <Editor
                style="height: 175px; overflow-y: hidden"
                v-model="formState.content"
                :defaultConfig="editorConfig"
                :mode="editorMode"
                @onCreated="handleCreated"
              />
            </div>
          </a-form-item>
        </div>
        <div style="border: 1px solid #ccc; height: 355px; margin: 10px 0px 10px 10px; padding: 10px 10px 0px 10px; width: 30%">
          <div style="height: 35px; line-height: 35px; font-weight: bold">选择通知人员</div>
          <div style="padding-top: 10px; border: 1px solid rgb(238, 238, 238); height: 290px">
            <a-tree
              style="height: 290px; overflow-y: auto !important"
              checkable
              v-model:checkedKeys="formState.recUserIds"
              :tree-data="treeData"
              show-icon
              v-model:expandedKeys="treeExpandedKeys"
              @select="handleSelect"
            >
              <template #title="{ title }">
                {{ title }}
              </template>
              <template #icon="{ category }">
                <template v-if="category === 1">
                  <ApartmentOutlined />
                </template>
                <template v-else-if="category === 2">
                  <UserOutlined />
                </template>
              </template>
            </a-tree>
          </div>
        </div>
      </div>

      <a-form-item label="Jenkins容器">
        <a-textarea
          v-model:value="formState.jenkinsUrl"
          placeholder="请输入Jenkins构建容器地址（构建多个Jenkins触发器使用，分割）"
          :auto-size="{ minRows: 2, maxRows: 5 }"
        />
      </a-form-item>
      <a-form-item label="是否构建">
        <a-switch v-model:checked="formState.isBuild" />
      </a-form-item>

      <div style="margin-top: 10px; text-align: center; width: 100%">
        <!-- <a-button type="primary" @click="save" style="margin-right: 50px">保存发送并关闭</a-button> 
        <a-button type="primary" @click="cancel">取消并关闭</a-button> -->

        <a-button type="primary" @click="save" style="margin-right: 50px">
          <template #icon>
            <SaveOutlined />
          </template>
          {{ $t('message.drawer.SaveAndClose') }}
        </a-button>
        <!-- 保存并关闭 -->
        <a-button type="primary" @click="cancel">
          <template #icon>
            <CloseOutlined />
          </template>
          {{ $t('message.drawer.Cancel') }}
        </a-button>
        <!-- 取消 -->
      </div>
    </a-form>
  </div>
</template>

<script setup>
import '@wangeditor/editor/dist/css/style.css' // 引入 css
import { UserOutlined, SaveOutlined, CloseOutlined, ApartmentOutlined } from '@ant-design/icons-vue'
import { onBeforeUnmount, ref, shallowRef, onMounted, watch} from 'vue'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { message } from 'ant-design-vue'
import { addVersionInfo, getVersionInfoById, modifyApi } from '@/api/versionLog'
import { getSysinfoList, getSysOrgUserInfo } from '@/api/commonFun'

const props = defineProps({
  modalState: Object
})
const emit = defineEmits(['childEvent'])

watch(
  props.modalState,
  (newVal, oldVal) => {
    if (props.modalState.isAdd) {
      formState.value = {
        sysId: null,
        messageType: 1,
        version: '',
        title: '',
        content: '',
        jenkinsUrl: '',
        isBuild: false,
        recUserIds: []
      }
    }
  },
  { deep: true }
)

/* 表单校验 */
const formRef = ref(null)
const validateSysId = async (rule, value) => {
  if (!value) {
    return Promise.resolve()
  }
}
const validateVersion = async (rule, value) => {
  if (!value) {
    return Promise.resolve()
  }
  if (!/^\d*\.?\d+$/.test(value)) {
    return Promise.reject('请输入有效的数字（支持小数）')
  }
}
const validateTitle = async (rule, value) => {
  if (!value) {
    return Promise.resolve()
  }
  if (value.length > 255) {
    return Promise.reject('更新标题太长!')
  }
}

async function getDataById(id) {
  await getVersionInfoById({ id: id }).then((res) => {
    /* 赋值给formState */
    formState.value = {
      id: res.data.id,
      sysId: res.data.sysId,
      messageType: res.data.messageType,
      version: res.data.version,
      title: res.data.title,
      content: res.data.content,
      jenkinsUrl: res.data.jenkinsUrl,
      isBuild: res.data.isBuild,
      recUserIds: res.data.recUserIds
    }
  })
}

defineExpose({ getDataById })
/* form相关 */
var formState = ref({
  sysId: null,
  messageType: 1,
  version: '',
  title: '',
  content: '',
  jenkinsUrl: '',
  isBuild: false,
  recUserIds: []
})
const labelCol = {
  style: {
    width: '100px'
  }
}
const sysNameOptions = ref([])
const messageOptions = ref([{ value: 1, label: '系统更新提醒' }])

// 编辑器实例，必须用 shallowRef
const editorRef = shallowRef()
// 内容 HTML
const toolBarMode = ref('')
const editorMode = ref('')

const toolbarConfig = {}
const editorConfig = { placeholder: '请输入内容...' }
// 组件销毁时，也及时销毁编辑器
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})

const handleCreated = (editor) => {
  editorRef.value = editor // 记录 editor 实例，重要！
}

onMounted(() => {
  getSysList()
  getOrgUserList()
  if (props.modalState.isAdd) {
    formState.value = {
      sysId: null,
      messageType: 1,
      version: '',
      title: '',
      content: '',
      jenkinsUrl: '',
      isBuild: false,
      recUserIds: []
    }
  }
})

async function getSysList() {
  await getSysinfoList().then((res) => {
    if (res.code == 200 && res.success) {
      sysNameOptions.value = []
      res.data.forEach((element) => {
        sysNameOptions.value.push({ value: element.sysInfoId, label: element.subSysName })
      })
    }
  })
}

const treeExpandedKeys = ref([])
const getOrgUserList = async (username) => {
  await getSysOrgUserInfo(username).then((res) => {
    if (res.code === 200 && res.success) {
      res.data.forEach((or) => {
        treeExpandedKeys.value.push(or.id)
        treeData.value.push(convertTree(or))
      })
    } else {
      message.error('部门树查询失败!')
    }
  })
}
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    key: data.id,
    title: data.name,
    category: 1,
    children: []
  }
  if (data.userTrees && data.userTrees.length) {
    data.userTrees.forEach((user) => {
      converted.children.push({
        key: user.id,
        title: user.name,
        category: 2
      })
    })
  }
  if (data.oraganizeUserTrees && data.oraganizeUserTrees.length) {
    data.oraganizeUserTrees.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}
// const selectNoticeUserIds = ref([])
const handleSelect = (selectedKeys, e) => {
  //存在则删除，不存在则添加
  if (!formState.value.recUserIds.includes(e.node.key)) {
    formState.value.recUserIds.push(e.node.key)
  } else {
    //从选中当中移除
    formState.value.recUserIds = formState.value.recUserIds.filter((item) => item != e.node.key)
  }
}

/* 保存发送并关闭 */
function save() {
  formRef.value.validate().then(() => {
    // 执行保存操作
    savedetail()
  })
}
async function savedetail() {
  if (props.modalState.isAdd) {
    await addVersionInfo(formState.value).then((res) => {
      if (res.code == 200 && res.success) {
        message.success('版本更新记录添加成功!')
        formState.value = {
          sysId: null,
          messageType: 1,
          version: '',
          title: '',
          content: '',
          jenkinsUrl: '',
          isBuild: false,
          recUserIds: []
        }
        emit('childEvent', 'success')
      }
    })
  } else {
    await modifyApi(formState.value).then((res) => {
      if (res.code == 200 && res.success) {
        message.success('版本信息已更新!')
        formState.value = {
          sysId: null,
          messageType: 1,
          version: '',
          title: '',
          content: '',
          jenkinsUrl: '',
          isBuild: false,
          recUserIds: []
        }
        emit('childEvent', 'success')
      }
    })
  }
  if (formRef.value) {
    formRef.value.resetFields()
  }
}
/* 取消并关闭 */
function cancel() {
  formState.value = {
    sysId: null,
    messageType: 1,
    version: '',
    title: '',
    content: '',
    jenkinsUrl: '',
    isBuild: false,
    recUserIds: []
  }
  if (formRef.value) {
    formRef.value.resetFields()
  }
  emit('childEvent', 'cancel')
}

/* table树相关 */
const treeData = ref([])
</script>
