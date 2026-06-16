<template>
  <div class="I18nConfigurationStyle">
    <div class="left">
      <div class="head">
        <a-input-search v-model:value="treeInput" @search="user_qurety" />
      </div>
      <div class="lefttree">
        <a-tree v-model:expandedKeys="expandedKeys" v-model:selectedKeys="selectedKeys" @select="onTreeNodeSelect"
          :tree-data="treeData"> </a-tree>
      </div>
    </div>
    <div class="right">
      <div class="head">
        <span style="margin-left: 20px"> 语言值: <a-input v-model:value="valueInput" style="width: 200px" allowClear />
        </span><span style="margin-left: 20px">国际化Key: <a-input v-model:value="keyIdInput" style="width: 200px"
            allowClear /></span>
        <span style="margin-left: 20px"><a-button type="primary"
            style="margin-left: 10px; width: 120px; background-color: rgb(36, 98, 167)" :icon="h(SearchOutlined)"
            @click="getTableData()">查询</a-button></span>
        <a-button style="margin-left: 10px; width: 100px; background-color: rgb(36, 98, 167)" :icon="h(PlusOutlined)"
          @click="openUserModal()" type="primary" v-if="tabname !== '空'" :disabled="disabledAdd">
          添加
          <a-tooltip v-if="disabledAdd" title="页面国际化文案请在相应的菜单页面下添加.">
            <QuestionCircleOutlined style="margin-left: 8px; color: #fff" />
          </a-tooltip>
        </a-button>
      </div>
      <div class="rightcontent">
        <a-table :columns="columns" :scroll="{ y: 'calc(100vh - 325px)' }" :pagination="false"
          :data-source="dataSource">
          <template #bodyCell="{ text, column, record }">
            <template v-if="column.key === 'operation'">
              <a @click="Edit(record)">
                <a-button type="primary" :icon="h(EditOutlined)" style="display: inline; margin-right: 10px"></a-button>
              </a>
              <a @click="Delete(record.id)">
                <a-button :icon="h(DeleteOutlined)" type="primary" danger
                  style="display: inline; margin-right: 10px"></a-button>
              </a>
            </template>
          </template>
        </a-table>
        <div class="paginationStyle" style="margin-top: 10px">
          <a-pagination v-model:current="modalCurrentPage" v-model:page-size="modalCurrentPageSize"
            :total="modalTotalCount" align="right" :show-total="(total) => `共 ${modalTotalCount} 条`"
            @change="modalPageSizeChange" />
        </div>
      </div>
    </div>
  </div>

  <a-drawer :footer-style="{ textAlign: 'center' }" :width="drawerwhitch" v-model:open="openModal" :title="ModalTitle"
    @ok="handleOk" ok-text="确认" cancel-text="取消" centered>
    <template #extra>
      <a-button style="margin-right: 8px" @click="configchange">{{ btnname }}</a-button>
    </template>
    <a-form :model="formState" :label-col="labelCol" :wrapper-col="wrapperCol">
      <a-form-item label="标签">
        <div>{{ tabname }}</div>
      </a-form-item>
      <a-form-item v-if="formState.id" label="语言值">
        <a-input v-model:value="formState.value" />
      </a-form-item>
      <a-form-item label="国际化Key">
        <a-input v-model:value="formState.keyId">
          <template #suffix>
            <a-tooltip title="系统页面标签下的国际化Key必须是它自身的编码,否则国际化无效">
              <QuestionCircleOutlined style="color: rgba(0,0,0,.45); cursor: help;" />
            </a-tooltip>
          </template>
        </a-input>
      </a-form-item>
      <a-form-item v-if="formState.id" label="语言编码">
        <a-select ref="select" v-model:value="formState.langCode" :options="options" />
      </a-form-item>
      <template v-else>
        <a-form-item v-for="(item, index) in formState.languageItems" :key="index" :label="index === 0 ? '语言文案:' : ' '"
          :colon="false">
          <div class="languageItemStyle">
            <a-select v-model:value="item.langCode" :options="options" placeholder="语言编码" style="width: 160px" />
            <a-input v-model:value="item.value" placeholder="语言值" style="flex: 1" />
            <a-button v-if="index === formState.languageItems.length - 1" type="primary" :icon="h(PlusOutlined)"
              @click="addLanguageItem" />
            <a-button v-if="formState.languageItems.length > 1" danger :icon="h(DeleteOutlined)"
              @click="removeLanguageItem(index)" />
          </div>
        </a-form-item>
      </template>
      <a-form-item label="排序">
        <a-input-number v-model:value="formState.sort" :min="0" :max="9999999999" />
      </a-form-item>
    </a-form>
    <template #footer><a-button type="primary" @click="handleOk" style="margin-right: 10px">保存</a-button>
      <a-button @click="exitItem">返回</a-button>
    </template>
  </a-drawer>
</template>
<script setup>
import { SearchOutlined, PlusOutlined, EditOutlined, DeleteOutlined, QuestionCircleOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import { ref, reactive, h, watch, createVNode, onMounted } from 'vue'
import dayjs from 'dayjs'
import { message, Modal } from 'ant-design-vue'
import { useGetTabApi, useInsertApi, useUpdate, useDelete, useGetById } from '@/api/I18ntab.js'
import { useGetLanguageApi } from '@/api/language.js'
import { useGetConfigApi, useInsertConfigApi, useUpdateConfig, useDeleteConfig, useGetConfigById } from '@/api/I18nconfig.js'

const options = ref([])
async function getAllLanguage() {
  options.value = []
  var obj = {
    Name: '',
    pageIndex: 1,
    pageSize: 1000
  }
  const response = await useGetLanguageApi(obj)
  if (response.code === 200 && response.success) {
    response.data.forEach((c) => {
      options.value.push({
        value: c.code,
        label: c.name
      })
    })
  }
}

const treeInput = ref('')
const user_qurety = () => {
  const keyword = String(treeInput.value || '').trim()
  const currentTreeData = keyword ? filterTreeData(fullTreeData.value, keyword) : fullTreeData.value
  setCurrentTreeData(currentTreeData, Boolean(keyword))
}
const treeData = ref([])
const fullTreeData = ref([])
const selectedKeys = ref([])
const expandedKeys = ref([])

const hasChildren = (node) => Array.isArray(node.children) && node.children.length > 0
const isSelectableNode = (node) => node.selectable !== false && (!hasChildren(node) || node.level === 1)
const setParentNodeUnselectable = (nodes = [], depth = 1) => {
  return nodes.map((node) => {
    const mappedChildren = Array.isArray(node.children) ? setParentNodeUnselectable(node.children, depth + 1) : node.children
    const hasKids = Array.isArray(mappedChildren) && mappedChildren.length > 0
    return {
      ...node,
      children: mappedChildren,
      level: depth,
      selectable: !hasKids || depth === 1,
      disabled: hasKids && depth !== 1
    }
  })
}
const findFirstLeafNode = (nodes = []) => {
  for (const node of nodes) {
    if (hasChildren(node)) {
      const leafNode = findFirstLeafNode(node.children)
      if (leafNode) {
        return leafNode
      }
    } else if (isSelectableNode(node)) {
      return node
    }
  }
  return null
}
const getNodeSearchText = (node) => {
  return [node.title, node.label, node.name, node.key, node.code, node.subCode]
    .filter((item) => item !== null && item !== undefined)
    .join('')
    .toLowerCase()
}
const filterTreeData = (nodes = [], keyword) => {
  const keywordText = String(keyword || '').toLowerCase()
  if (!keywordText) {
    return nodes
  }
  return nodes.reduce((result, node) => {
    const isMatch = getNodeSearchText(node).includes(keywordText)
    const children = Array.isArray(node.children) ? (isMatch ? node.children : filterTreeData(node.children, keywordText)) : node.children
    if (isMatch || hasChildren({ children })) {
      result.push({
        ...node,
        children
      })
    }
    return result
  }, [])
}
const getParentNodeKeys = (nodes = []) => {
  return nodes.reduce((keys, node) => {
    if (hasChildren(node)) {
      keys.push(node.key, ...getParentNodeKeys(node.children))
    }
    return keys
  }, [])
}
const setCurrentTreeData = (nodes = [], expandAll = false) => {
  treeData.value = nodes
  const firstLeafNode = findFirstLeafNode(treeData.value)
  selectedKeys.value = firstLeafNode ? [firstLeafNode.key] : []
  tabname.value = firstLeafNode ? firstLeafNode.title : '空'
  globalSubCode.value = firstLeafNode ? firstLeafNode.subCode : 0

  if (expandAll) {
    expandedKeys.value = getParentNodeKeys(treeData.value)
  } else {
    // 动态获取所有第一级节点的key
    const rootKeys = treeData.value.map(node => node.key)

    // CeriOS平台的固定key
    const ceriosKey = 'a8b962638202e26cc4178323750b4ceb'

    // 合并所有需要展开的key
    expandedKeys.value = [...rootKeys, ceriosKey]
  }

  if (!firstLeafNode) {
    dataSource.value = []
    modalTotalCount.value = 0
  }
}

const EXCLUDED_TREE_KEYS = new Set(['ab781169d02c4c859e4e41cf49292f8e'])
const excludeNodesByKey = (nodes = [], excludes = EXCLUDED_TREE_KEYS) => {
  const walk = (arr = []) =>
    (arr || [])
      .filter((node) => !excludes.has(node.key))
      .map((node) => ({
        ...node,
        children: Array.isArray(node.children) ? walk(node.children) : node.children
      }))
  return walk(nodes)
}

const treeDate = async () => {
  const data = await useGetTabApi('', '')
  if (data.code === 200 && data.success) {
    const filtered = excludeNodesByKey(data.data)
    fullTreeData.value = setParentNodeUnselectable(filtered)
    treeData.value = fullTreeData.value
    console.log(JSON.stringify(treeData.value))
    setCurrentTreeData(fullTreeData.value)
  } else {
    message.error('查询失败!')
  }
}

onMounted(async () => {
  await treeDate('')
  getTableData()
})

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
const configchange = () => {
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

const valueInput = ref('')
const keyIdInput = ref('')
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
    },
    width: 80
  },
  {
    title: '语言值',
    dataIndex: 'value',
    key: 'value',
    width: 250
  },
  {
    title: '国际化Key',
    dataIndex: 'keyId',
    key: 'keyId',
    width: 200
  },
  {
    title: '语言编码',
    dataIndex: 'langCode',
    key: 'langCode',
    width: 80
  },
  {
    title: '排序',
    dataIndex: 'sort',
    key: 'sort',
    width: 80
  },
  {
    title: '修改时间',
    dataIndex: 'modifyTime',
    key: 'modifyTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    },
    width: 120
  },
  {
    title: '操作',
    key: 'operation',
    fixed: 'right',
    width: 120
  }
]

/* Form表单 */
const resetForm = {
  id: '',
  value: '',
  keyId: '',
  langCode: '',
  languageItems: [
    {
      langCode: '',
      value: ''
    }
  ]
}
let formState
const getResetForm = () => ({
  ...resetForm,
  languageItems: resetForm.languageItems.map((item) => ({ ...item }))
})
formState = reactive(getResetForm())
const addLanguageItem = () => {
  formState.languageItems.push({
    langCode: '',
    value: ''
  })
}
const removeLanguageItem = (index) => {
  formState.languageItems.splice(index, 1)
}

function modalPageSizeChange(page, pageSize) {
  modalCurrentPage.value = page
  modalCurrentPageSize.value = pageSize
  getTableData()
}

const ModalTitle = ref('')
const openModal = ref(false)
function Edit(row) {
  getAllLanguage()
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
    subCode: globalSubCode.value,
    biao: selectedKeys.value[0],
    Value: valueInput.value,
    KeyId: keyIdInput.value,
    pageIndex: modalCurrentPage.value,
    pageSize: modalCurrentPageSize.value
  }
  if (obj.biao == "sysinfo") obj.biao = "ab781169d02c4c859e4e41cf49292f8e"
  await useGetConfigApi(obj).then((res) => {

    if (res.code == 200 && res.success) {
      dataSource.value = res.data
      modalTotalCount.value = res.total
      //message.success(res.message)
    }
  })
}

function openUserModal() {
  getAllLanguage()
  formState = reactive(getResetForm())
  openModal.value = true
  ModalTitle.value = '添加'
}

function handleOk() {
  if (formState.id) {
    if (formState.value == '' || formState.keyId == '' || formState.langCode == '') {
      message.warning('表单各项不能为空')
      return
    }
    ModifyUser(formState)
  } else {
    debugger
    const languageItems = formState.languageItems || []
    if (formState.keyId == '' || languageItems.some((item) => item.value == '' || item.langCode == '')) {
      message.warning('表单各项不能为空')
      return
    }
    const langCodes = languageItems.map((item) => item.langCode)
    if (new Set(langCodes).size !== langCodes.length) {
      message.warning('语言编码不能重复')
      return
    }
    const list = languageItems.map((item) => {
      const str = { id: '', tabId: selectedKeys.value[0], keyId: formState.keyId, value: item.value, langCode: item.langCode,sort:formState.sort || 0 }
      //自动拼前缀
      switch (selectedKeys.value[0]) {
        case "5DE034D1FD674FE7B2A3878204B8A8E3":
          str.keyId = "message.route.XT" + str.keyId
          break
        case "D94FF1A59E554616A36C6960D08D398C":
          break
        //TODO:如果是拼过来的菜单信息表，则根据菜单的Code拼接出keyId
        default:
          str.keyId = selectedKeys.value[0] + "." + str.keyId
          str.subCode = 2
      }
      return str
    })
    AddTagRelation(list)
  }
}

const exitItem = () => {
  openModal.value = false
}

async function AddTagRelation(obj) {
  const list = Array.isArray(obj) ? obj : [obj]
  const result = await Promise.all(list.map((item) => useInsertConfigApi(item)))
  const error = result.find((res) => res.code != 200 || !res.success)
  if (!error) {
    message.success('添加成功!')
    openModal.value = false
    formState = reactive(getResetForm())
    getTableData()
  } else {
    message.error(error.message)
  }
}
async function ModifyUser(obj) {
  await useUpdateConfig(obj).then((res) => {
    if (res.code == 200 && res.success) {
      message.success('信息已更新!')
      openModal.value = false
      formState = reactive(getResetForm())
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
  await useDeleteConfig(id).then((res) => {
    resolve()
    if (res.code === 200 && res.success) {
      message.success(res.message)
      getTableData()
    }
  })
}
const tabname = ref('空')
const globalSubCode = ref(0)
const disabledAdd = ref(false)
const onTreeNodeSelect = (keys, e) => {
  modalCurrentPage.value = 1
  disabledAdd.value = false
  if (e.selected && e.node.key == 'ab781169d02c4c859e4e41cf49292f8e') {
    disabledAdd.value = true
  }

  if (hasChildren(e.node) && e.node.level !== 1) {
    selectedKeys.value = keys.filter((key) => key !== e.node.key)
    tabname.value = '空'
    return
  }
  if (e.selected) {
    tabname.value = e.node.title
    globalSubCode.value = e.node.subCode
  } else {
    tabname.value = '空'
  }
}
watch(selectedKeys, () => {
  if (selectedKeys.value.length > 0) {
    getTableData()
  }
})
</script>
<style lang="scss">
.I18nConfigurationStyle {
  width: 100%;
  height: 100%;
  display: flex;

  .left {
    width: 20%;
    background-color: rgb(255, 255, 255);
    height: 100%;
  }

  .right {
    width: 80%;
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

  .rightcontent {
    width: 100%;
    height: calc(100% - 50px - 10px);
    margin-top: 10px;
    padding: 0 8px;

    .paginationStyle {
      height: 40px;
      padding-right: 10px;
      position: absolute;
      right: 0px;
      bottom: 50px;
      width: 100%;
      align-items: center;
      display: flex;
      justify-content: flex-end;
    }
  }


}

.languageItemStyle {
  display: flex;
  gap: 8px;
}
</style>
