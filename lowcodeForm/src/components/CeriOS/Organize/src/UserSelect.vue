<template>
  <div :class="[$attrs.class, 'select-tag-list']" v-if="buttonType === 'button'">
    <a-button preIcon="icon-ym icon-ym-btn-add" @click="openSelectModal">{{ modalTitle }}</a-button>
    <div class="tags">
      <a-tag class="!mt-10px" :closable="!disabled" v-for="(item, i) in options" :key="item.id" @close="onTagClose(i)">{{ item.fullName }}</a-tag>
    </div>
  </div>
  <a-select v-bind="getSelectBindValue" v-model:value="innerValue" :options="options" @change="onChange" @click="openSelectModal" v-else />
  <a-modal v-model:open="visible" title="选择用户" :width="800" class="transfer-modal" @ok="handleSubmit" centered :maskClosable="false" :keyboard="false">
    <template #closeIcon>
      <ModalClose :canFullscreen="false" @cancel="handleCancel" />
    </template>
    <div class="transfer__body">
      <div class="transfer-pane left-pane">
        <div class="transfer-pane__tool">
          <a-input-search :placeholder="t('common.enterKeyword')" allowClear v-model:value="searchStr" @search="handleSearch" />
        </div>
        <div class="transfer-pane__body transfer-pane__body-tab" style="overflow-y: auto">
          <a-tree
    :tree-data="filteredTreeData"
    
    @select="handleNodeSelect"
  />

          <!-- <a-tabs v-model:activeKey="activeKey" :tabBarGutter="10" size="small" class="pane-tabs" v-if="hasSys">
            <a-tab-pane key="1" tab="全部数据"></a-tab-pane>
            <a-tab-pane key="system" tab="动态参数"></a-tab-pane>
          </a-tabs>
          <BasicTree class="tree-main" ref="treeRef" v-bind="getTreeBindValue" v-if="!isAsync" />
          <ScrollContainer  v-else ref="infiniteBody">
            <div v-for="item in treeData" :key="item.id" class="selected-item" @click="handleNodeClick(item)">
              <span :title="item.fullName">{{ item.fullName }}</span>
            </div>
            <ceri-empty v-if="!treeData.length" />
          </ScrollContainer> -->
        </div>
      </div>
      <div class="transfer-pane right-pane">
        <div class="transfer-pane__tool">
          <span>{{ t('component.ceri.common.selected') }}({{ selectedData.length || 0 }})</span>
          <span class="remove-all-btn" @click="removeAll">清空列表</span>
        </div>
        <div class="transfer-pane__body">
          <ScrollContainer>
            <div v-for="(item, i) in selectedData" :key="i" class="selected-item">
              <span :title="item.fullName">{{ item.fullName }}</span>
              <delete-outlined class="delete-btn" @click="removeData(i)" />
            </div>
            <ceri-empty v-if="!selectedData.length" />
          </ScrollContainer>
        </div>
      </div>
    </div>
  </a-modal>
</template>

<script lang="ts" setup>
// import { getDepartmentSelectAsyncList, getOrgByOrganizeCondition, getSelectedList } from '@/api/permission/organize';
import { Form, Modal as AModal } from 'ant-design-vue'
import { DeleteOutlined } from '@ant-design/icons-vue'
import { onMounted, computed, ref, unref, watch, nextTick, reactive } from 'vue'
import { BasicTree, TreeActionType } from '@/components/Tree'
import { ScrollContainer, ScrollActionType } from '@/components/Container'
import { depSelectProps } from './props'
import ModalClose from '@/components/Modal/src/components/ModalClose.vue'
import { useI18n } from 'vue-i18n'
import { useAttrs } from '@/hooks/core/useAttrs'
import { cloneDeep, pick } from 'lodash-es'

import { useGetOraganizeUserTreeAsync } from '@/api/system/organization'

defineOptions({ name: 'CeriDepSelect', inheritAttrs: false })
const props = defineProps(depSelectProps)
const emit = defineEmits(['update:value', 'change', 'labelChange'])
const attrs: any = useAttrs({ excludeDefaultKeys: false })
const { t } = useI18n()
const visible = ref(false)
const treeRef = ref<Nullable<TreeActionType>>(null)
const innerValue = ref([])
const nodeId = ref('0')
const treeKey = ref(+new Date())
const pagination = reactive({
  keyword: '',
  currentPage: 1,
  pageSize: 20
})
const finish = ref<boolean>(false)
const isAsync = ref<boolean>(false)
const activeKey = ref('')
const infiniteBody = ref<Nullable<ScrollActionType>>(null)
// const treeData = ref<any[]>([])
const options = ref([])
const selectedData = ref([])
const formItemContext = Form.useInjectFormItemContext()
const systemFieldList = ref<any[]>([
  { id: '@currentOrg', fullName: '当前组织' },
  { id: '@currentOrgAndSubOrg', fullName: '当前组织及子组织' },
  { id: '@currentGradeOrg', fullName: '当前分管组织' }
])

const getSelectBindValue = computed(() => ({
  ...pick(props, ['placeholder', 'disabled', 'size', 'allowClear']),
  fieldNames: { label: 'fullName', value: 'id' },
  open: false,
  mode: props.multiple ? 'multiple' : '',
  showSearch: false,
  showArrow: true,
  class: unref(attrs).class ? 'w-full ' + unref(attrs).class : 'w-full'
}))
const getTreeBindValue = computed(() => {
  const data: any = {
    treeData: treeData.value,
    onSelect: handleSelect,
    defaultExpandAll: props.selectType !== 'all' || activeKey.value !== '1',
    key: treeKey.value
  }
  // if (props.selectType === 'all' && activeKey.value === '1') data.loadData = onLoadData;
  return data
})

watch(
  () => props.value,
  () => {
    setValue()
  },
  { immediate: true }
)
// watch(
//   () => activeKey.value,
//   (val) => {
//     if (!val) return
//     if (val === 'system') return (treeData.value = cloneDeep(systemFieldList.value))
//     if (props.selectType === 'all') {
//       // handleSearch('');
//     } else {
//       initData()
//     }
//   }
// )
watch(
  () => visible.value,
  (val) => {
    if (!val) activeKey.value = ''
  }
)

function setValue() {
  if (!props.value || !props.value.length) return setNullValue()
  let ids = props.multiple ? (props.value as any[]) : [props.value]
  if (!Array.isArray(ids)) return
  const selectSysData: any[] = getSelectSysData(ids)
  ids = ids.filter((o) => o.indexOf('@') < 0)
  if (!ids.length) return setOptions(selectSysData)
  // getSelectedList(ids).then(res => {
  //   if (!props.value || !props.value.length) return setNullValue();
  //   setOptions([...(res.data.list || []), ...selectSysData]);
  // });
}
function setOptions(data) {
  if (!props.value || !props.value.length) return setNullValue()
  const selectedList: any[] = data
  const innerIds = selectedList.map((o) => o.id)
  innerValue.value = props.multiple ? innerIds : innerIds[0]
  options.value = cloneDeep(selectedList)
  selectedData.value = cloneDeep(selectedList)
  const labels = selectedData.value.map((o) => o.fullName).join()
  emit('labelChange', labels)
}
function setNullValue() {
  innerValue.value = props.multiple ? [] : undefined
  options.value = []
  selectedData.value = []
  emit('labelChange', '')
}
function getSelectSysData(ids) {
  let list: any[] = []
  for (let i = 0; i < unref(systemFieldList).length; i++) {
    inner: for (let j = 0; j < ids.length; j++) {
      if (unref(systemFieldList)[i].id === ids[j]) {
        list.push({ ...unref(systemFieldList)[i] })
        break inner
      }
    }
  }
  return list
}
function onChange(_val, option) {
  selectedData.value = option ?? []
  handleSubmit()
}
function onTagClose(i) {
  selectedData.value.splice(i, 1)
  handleSubmit()
}
function openSelectModal() {
  if (props.disabled) return
  visible.value = true
  isAsync.value = false
  pagination.keyword = ''
  pagination.currentPage = 1
  nodeId.value = '0'
  finish.value = false
  activeKey.value = '1'
  // treeData.value = []
  setValue()
  nextTick(() => {
    bindScroll()
  })
}
function handleCancel() {
  visible.value = false
}
// function handleSearch(value) {
//   pagination.keyword = value || '';
//   if (props.selectType !== 'all' || activeKey.value !== '1') return getTree().setSearchValue(value);
//   treeKey.value = +new Date();
//   nodeId.value = '0';
//   treeData.value = [];
//   pagination.currentPage = 1;
//   isAsync.value = !!pagination.keyword;
//   finish.value = false;
//   if (isAsync.value) {
//     nextTick(() => {
//       bindScroll();
//     });
//   }
//   initData();
// }
function handleSelect(keys) {
  if (!keys.length) return
  const data: any = getTree().getSelectedNode(keys[0])
  handleNodeClick(data)
}
function handleNodeClick(data) {
  if (data?.disabled || data?.type === 'company') return
  const boo = selectedData.value.some((o) => o.id === data.id)
  if (boo) return
  const item = cloneDeep(data)
  if (props.selectType === 'custom') item.fullName = item.lastFullName
  props.multiple ? selectedData.value.push(item) : (selectedData.value = [item])
}
function removeAll() {
  selectedData.value = []
  innerValue.value = []
}
function removeData(index: number) {
  selectedData.value.splice(index, 1)
}
function getTree() {
  const tree = unref(treeRef)
  if (!tree) {
    throw new Error('tree is null!')
  }
  return tree
}
function handleSubmit() {
  const ids = unref(selectedData).map((o) => o.id)
  options.value = unref(selectedData)
  innerValue.value = props.multiple ? ids : ids[0]
  if (props.multiple) {
    emit('update:value', ids)
    emit('change', ids, unref(options))
  } else {
    emit('update:value', ids[0] || '')
    emit('change', ids[0] || '', unref(options)[0])
  }
  formItemContext.onFieldChange()
  handleCancel()
  console.info(selectedData.value)
}
function bindScroll() {
  const bodyRef = infiniteBody.value
  const vBody = bodyRef?.getScrollWrap()
  vBody?.addEventListener('scroll', () => {
    if (vBody.scrollHeight - vBody.clientHeight - vBody.scrollTop <= 200 && !finish.value) {
      pagination.currentPage += 1
      getList()
    }
  })
}

onMounted(async () => {
  await getList()
})
// 点击树节点
function handleNodeSelect(selectedKeys, info) {
  // 获取原始节点数据
  const nodeData = info.node?.dataRef || info.node
  // 如果节点已存在，不重复添加
  if (!selectedData.value.find(item => item.id === nodeData.id)) {
    selectedData.value.push({ id: nodeData.id, fullName: nodeData.fullName })
    innerValue.value.push(nodeData.id)   
  }
}

const searchStr = ref()
const treeData = ref([])
// 搜索过滤后的数据
const filteredTreeData = computed(() => {
  if (!searchStr.value) {
    return treeData.value
  } else {
    return filterTree(treeData.value, searchStr.value)
  }
})

function handleSearch() {
  // 输入框回车或点击搜索按钮时触发
  console.log('搜索关键字：', searchStr.value)
}

// 树过滤函数
function filterTree(data, keyword) {
  return data
    .map((node) => {
      const match = node.fullName.includes(keyword)
      if (node.children) {
        const filteredChildren = filterTree(node.children, keyword)
        if (filteredChildren.length > 0 || match) {
          return { ...node, children: filteredChildren }
        }
      }
      return match ? { ...node, children: [] } : null
    })
    .filter(Boolean)
}

async function getList() {
  // if (pagination.keyword) nodeId.value = '0'
  const data = await useGetOraganizeUserTreeAsync()
  console.info(data.data)
  if (data.code === 200 && data.success) {
    treeData.value = convertTree(data.data,'')
    console.info(111,treeData.value)
  }
}

function convertTree(data, userName) {
  if (!data) return []

  // 支持数组输入
  if (Array.isArray(data)) {
    return data.map(item => convertTree(item, userName))
  }

  const converted = {
     id:data.id,
    key: data.id,
    title: data.name,
        fullName:data.name,
    category: 1,
    children: []
  }

  

  // 添加用户节点
  if (data.userTrees?.length) {
    converted.children.push(
      ...data.userTrees.map(user => ({
     id:user.id,
        key: user.id,
        title: user.name,
        fullName:user.name,
        category: 2
      }))
    )
  }

  // 递归子组织
  if (data.oraganizeUserTrees?.length) {
    converted.children.push(
      ...data.oraganizeUserTrees.map(or => convertTree(or, userName))
    )
  }

  return converted
}


async function initData() {
  if (props.selectType === 'all') {
    if (activeKey.value === 'system') {
      treeData.value = cloneDeep(systemFieldList.value)
    } else {
      getList()
    }
  } else {
    if (!props.ableIds?.length) return (treeData.value = [])
    const query = { departIds: props.ableIds }
    // getOrgByOrganizeCondition(query).then(res => {
    //   treeData.value = res.data.list;
    // });
  }
}
</script>
