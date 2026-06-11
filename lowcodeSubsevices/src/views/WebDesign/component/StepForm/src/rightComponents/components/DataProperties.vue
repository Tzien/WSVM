<template>
  <a-divider>数据选项</a-divider>
  <a-form-item class="flex justify-center">
    <ceri-radio v-model:value="activeData.__config__.dataType" :options="dataTypeOptions" optionType="button" buttonStyle="solid" @change="onDataTypeChange" />
  </a-form-item>
  <div class="options-list" v-if="activeData.__config__.dataType === 'static'">
    <template v-if="['treeSelect', 'cascader'].includes(activeData.__config__.ceriKey)">
      <BasicTree ref="treeRef" :treeData="activeData.options" :actionList="actionList" />
      <div class="add-btn">
        <a-button type="link" preIcon="icon-ym icon-ym-btn-add" @click="addTreeItem" class="!px-0">添加父级</a-button>
        <a-divider type="vertical"></a-divider>
        <a-button type="link" @click="openTreeModal(true, { options: activeData.options })" class="!px-0">批量编辑</a-button>
      </div>
    </template>
    <template v-else>
      <draggable v-model="activeData.options" :animation="300" group="selectItem" handle=".option-drag" itemKey="uuid">
        <template #item="{ element, index }">
          <div class="select-item">
            <div class="select-line-icon option-drag">
              <i class="icon-ym icon-ym-darg" />
            </div>
            <a-input v-model:value="element.fullName" placeholder="选项名" />
            <a-input v-model:value="element.id" placeholder="选项值" />
            <div class="close-btn select-line-icon" @click="activeData.options.splice(index, 1)">
              <i class="icon-ym icon-ym-btn-clearn" />
            </div>
          </div>
        </template>
      </draggable>
      <div class="add-btn">
        <a-button type="link" preIcon="icon-ym icon-ym-btn-add" @click="addSelectItem" class="!px-0">添加选项</a-button>
        <a-divider type="vertical"></a-divider>
        <a-button type="link" @click="openModal(true, { options: activeData.options })" class="!px-0">批量编辑</a-button>
      </div>
    </template>
  </div>
  <div v-if="activeData.__config__.dataType === 'dictionary'">
    <a-form-item label="数据字典">
      <!-- <ceri-tree-select :options="dictOptions" v-model:value="activeData.__config__.dictionaryType" lastLevel allowClear @change="onDictionaryTypeChange" /> -->
      <a-tree-select :tree-data="dictOptions" v-model:value="activeData.__config__.dictionaryType" allowClear 
       @change="dictHandleChange"/>
    </a-form-item>
    <a-form-item label="存储字段">
      <ceri-select v-model:value="activeData.props.value" placeholder="请选择" :options="valueOptions" />
    </a-form-item>
  </div>
  <div v-if="activeData.__config__.dataType === 'dynamic'">
    <a-form-item>
      <template #label>数据接口<BasicHelp text="支持通过数据接口的参数实现控件之间的联动。" /></template>
      <interface-modal :value="activeData.__config__.propsUrl" :title="activeData.__config__.propsName" popupTitle="数据接口" @change="onPropsUrlChange" />
    </a-form-item>
    <a-form-item label="存储字段">
      <a-auto-complete
        v-model:value="activeData.props.value"
        placeholder="请输入"
        :options="options"
        @focus="onFocus(activeData.props.value)"
        @search="debounceOnSearch(activeData.props.value)"
      />
    </a-form-item>
    <a-form-item label="回显字段">
      <a-auto-complete
        v-model:value="activeData.props.label"
        placeholder="请输入"
        :options="options"
        @focus="onFocus(activeData.props.label)"
        @search="debounceOnSearch(activeData.props.label)"
      />
    </a-form-item>
    <a-form-item label="子级字段" v-if="['treeSelect', 'cascader'].includes(activeData.__config__.ceriKey)">
      <a-input v-model:value="activeData.props.children" placeholder="请输入" @change="onPropsChildrenChange" />
    </a-form-item>
    <a-form-item label="是否缓存">
      <a-switch v-model:checked="activeData.__config__.useCache" />
    </a-form-item>
    <a-form-item label="参数设置" v-if="activeData.__config__.templateJson?.length">
      <select-interface-btn
        :templateJson="activeData.__config__.templateJson"
        :fieldOptions="formFieldsOptions"
        :type="3"
        @fieldChange="onRelationFieldChange"
      />
    </a-form-item>
  </div>
  <a-divider></a-divider>
  <BatchOperate @register="registerBatchOperate" @confirm="onBatchOperateConfirm" />
  <TreeNodeModal ref="treeNodeRef" @confirm="onTreeNodeConfirm" />
  <TreeBatchOperate @register="registerTreeBatchOperate" @confirm="onBatchOperateConfirm" />
</template>
<script lang="ts" setup>
import { getLowCodeDict } from '@/api/system/organization'
import draggable from 'vuedraggable'
import { onMounted, h, ref, unref,watchEffect } from 'vue'
import { useDynamic } from '../../hooks/useDynamic'
import { useField } from '../../hooks/useField'
import { useModal } from '@/components/Modal'
import BatchOperate from './BatchOperate.vue'
import TreeBatchOperate from './TreeBatchOperate.vue'
import TreeNodeModal from './TreeNodeModal.vue'
import { InterfaceModal } from '@/components/CommonModal'
import { BasicTree, TreeActionItem, TreeActionType } from '@/components/Tree'
import { PlusOutlined, FormOutlined, DeleteOutlined } from '@ant-design/icons-vue'
import { SelectInterfaceBtn } from '@/components/Interface'
import { useCommonStore } from '@/store/index.js'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useGlobalState } from '@/shared/useGlobalState'

const emit = defineEmits(['propsChildrenChange'])
const props = defineProps(['activeData', 'dicOptions'])
const treeRef = ref<Nullable<TreeActionType>>(null)
const treeNodeRef = ref(null)
const currentNode = ref<any>(null)
const { options, debounceOnSearch, onFocus, initFieldData } = useField(props.activeData)
const {
  onDataTypeChange,
  onDictionaryTypeChange,
  onPropsUrlChange,
  dataTypeOptions,
  valueOptions,
  addSelectItem,
  formFieldsOptions,
  onRelationFieldChange,
  onBatchOperateConfirm
} = useDynamic(props.activeData, initFieldData)

var isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__
var commonStore = ref({})
const { globalStore } = useGlobalState()
if (!isQiankun) {
  commonStore = useCommonStore()
} else {
  watchEffect(() => {
    if (globalStore.value) {
      commonStore.value = globalStore.value.commonStore
    }
  })
}

const [registerBatchOperate, { openModal }] = useModal()
const [registerTreeBatchOperate, { openModal: openTreeModal }] = useModal()
const actionList: TreeActionItem[] = [
  {
    render: (node) => {
      return h(PlusOutlined, {
        class: 'ml-4px',
        title: '添加',
        onClick: () => {
          handleAdd(node)
        }
      })
    }
  },
  {
    render: (node) => {
      return h(FormOutlined, {
        class: 'ml-4px',
        title: '编辑',
        onClick: () => {
          handleEdit(node)
        }
      })
    }
  },
  {
    render: (node) => {
      return h(DeleteOutlined, {
        class: 'ml-4px',
        title: '删除',
        onClick: () => {
          handleDelete(node)
        }
      })
    }
  }
]

function getTree() {
  const tree = unref(treeRef)
  if (!tree) {
    throw new Error('tree is null!')
  }
  return tree
}
function handleAdd(node: any) {
  const data = getTree().getSelectedNode(node.id) as any
  if (!Reflect.has(data, 'children')) data.children = []
  currentNode.value = data.children
  ;(unref(treeNodeRef) as any).openModal()
}
function handleEdit(node: any) {
  const data = getTree().getSelectedNode(node.id) as any
  currentNode.value = data
  ;(unref(treeNodeRef) as any).openModal(data)
}
function handleDelete(node: any) {
  props.activeData.__config__.defaultValue = []
  getTree().deleteNodeByKey(node.id)
}
function addTreeItem() {
  ;(unref(treeNodeRef) as any).openModal()
  currentNode.value = props.activeData.options
}
function onTreeNodeConfirm(data, isEdit) {
  if (!isEdit) return currentNode.value.push(data)
  getTree().updateNodeByKey(currentNode.value.id, data)
}
function onPropsChildrenChange() {
  emit('propsChildrenChange')
}

onMounted(async () => {
  await getList()
})
const dictOptions = ref([])
const dictData=ref()
async function getList() {
  const data = await getLowCodeDict()
  if (data.code === 200 && data.success) {
    dictData.value=data.data
    commonStore.setDictOptions(data.data)
    dictOptions.value = convertTreeData(data.data)
  }
}
function convertTreeData(data) {
  return data.map((item) => ({
    label: item.categoryName,
    value: item.categoryName,  // 第一级分类值
    disabled: true,

    children: item.lowCodeDictInfos.map(dict => ({
      label: dict.dictName,    // 第二级：字典名称
      value: dict.dictId,      // 第二级：字典编码
      // 第二级没有字典子项，所以无需进一步递归
    })),
  }));
}
function dictHandleChange(value, label, extra) {
    // value 是选中的 value
    // label 是选中的 label
    // extra 是额外的信息，包含节点的数据

    // const selectedItem = findItemByDictId(dictData.value, value);
    
    // console.log('选中的字典项:', selectedItem);
    // console.info(111,commonStore.dictOptions)
    // commonStore.setDictOptions(selectedItem)
    // console.info(222,commonStore.dictOptions)
  }
function  findItemByDictId(data, dictId) {
    for (let category of data) {
      for (let dict of category.lowCodeDictInfos) {
        if (dict.dictId === dictId) {
          return dict;  // 找到匹配的字典项
        }
      }
    }
    return null;  // 如果没有找到
  }
</script>
