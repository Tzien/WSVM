<template>
  <Tooltip placement="topRight">
    <template #title>
      <span>{{ t('component.table.viewSetting') }}</span>
    </template>
    <setting-outlined @click="openDrawer" />
  </Tooltip>
  <a-drawer width="340px" v-model:open="visible" class="common-container-drawer">
    <template #title>
      <a-input v-model:value="dataForm.fullName" :maxlength="6" class="w-130px" />
    </template>
    <div class="common-container-drawer-body column-setting-body">
      <a-tabs v-model:activeKey="activeKey" :tabBarGutter="11" class="average-tabs" @change="onTabChange">
        <a-tab-pane :key="0" tab="查询字段" v-if="dataForm.searchList?.length"> </a-tab-pane>
        <a-tab-pane :key="1" tab="列表字段"> </a-tab-pane>
      </a-tabs>
      <div class="flex-1 overflow-auto">
        <CheckboxGroup v-model:value="searchCheckedList" ref="searchListRef" class="check-contain" v-if="activeKey === 0">
          <div v-for="item in dataForm.searchList" :key="item.id" class="check-item">
            <DragOutlined class="table-search-drag-icon" />
            <Checkbox :value="item.id">
              <div :title="item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label">
                {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
              </div>
            </Checkbox>
          </div>
        </CheckboxGroup>
        <CheckboxGroup v-model:value="columnCheckedList" ref="columnListRef" class="check-contain" v-else>
          <div v-for="item in dataForm.columnList" :key="item.id" class="check-item">
            <DragOutlined class="table-column-drag-icon" />
            <Checkbox :value="item.id">
              <div :title="item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label">
                {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
              </div>
            </Checkbox>
          </div>
        </CheckboxGroup>
      </div>
    </div>
    <div class="h-60px leading-60px ceri-basic-drawer-footer" style="padding: 10px 12px 10px 20px">
      <a-button class="mr-10px" :loading="showAddLoading" @click="addOrUpdateHandle('')">{{ t('common.addView') }}</a-button>
      <a-button class="mr-10px" :loading="showUpdateLoading" @click="addOrUpdateHandle(dataForm.id)" type="primary" v-if="currentView.type !== 0">
        {{ t('common.updateView') }}
      </a-button>
    </div>
  </a-drawer>
</template>
<script lang="ts" setup>
import { reactive, toRefs, nextTick, unref, ref } from 'vue'
import { Tooltip, Checkbox, CheckboxGroup, Drawer as ADrawer } from 'ant-design-vue'
import { SettingOutlined, DragOutlined } from '@ant-design/icons-vue'
import { useI18n } from 'vue-i18n'
import { useMessage } from '@/hooks/web/useMessage'
import { isNullAndUnDef } from '@/utils/is'
import { cloneDeep } from 'lodash-es'
import Sortablejs from 'sortablejs'
import { createView, updateView } from '@/api/onlineDev/visualDev'
import { useUserStore } from '@/store'

interface State {
  dataForm: any
  visible: boolean
  activeKey: number
  searchCheckedList: string[]
  columnCheckedList: string[]
  showAddLoading: boolean
  showUpdateLoading: boolean
}

const state = reactive<State>({
  dataForm: {
    viewName: '',
    searchList: [],
    columnList: []
  },
  visible: false,
  activeKey: 0,
  searchCheckedList: [],
  columnCheckedList: [],
  showAddLoading: false,
  showUpdateLoading: false
})
const { dataForm, visible, activeKey, searchCheckedList, columnCheckedList, showAddLoading, showUpdateLoading } = toRefs(state)
const emit = defineEmits(['reload'])
const { t } = useI18n()
const { createMessage } = useMessage()
const searchListRef = ref(null)
const columnListRef = ref(null)
const props = defineProps({
  menuId: { type: String, default: '' },
  currentView: { type: Object, default: {} },
  viewList: { type: Array as PropType<any[]>, default: [] }
})

function openDrawer() {
  state.visible = true
  state.showUpdateLoading = false
  state.showAddLoading = false
  state.dataForm = cloneDeep(props.currentView)
  initData()
  state.activeKey = state.dataForm.searchList?.length ? 0 : 1
  initSortable()
  initCheckedList()
}
function initData() {
  const defaultData = props.viewList.filter((o) => o.type == 0)[0] || {}
  state.dataForm.searchList = mergeList(state.dataForm.searchList, defaultData.searchList)
  state.dataForm.columnList = mergeList(state.dataForm.columnList, defaultData.columnList)
}
function mergeList(array1: any = [], array2: any = []) {
  array2 = array2.map((o) => ({ ...o, show: false }))
  return [...array1.filter((o) => array2.some((i) => i.id === o.id)), ...array2.filter((o) => !array1.some((i) => i.id === o.id))]
}
function initCheckedList() {
  state.searchCheckedList = state.dataForm.searchList.filter((o) => o.show).map((o) => o.id)
  state.columnCheckedList = state.dataForm.columnList.filter((o) => o.show).map((o) => o.id)
}
function onTabChange() {
  initSortable()
}
function initSortable() {
  nextTick(() => {
    const elRef = state.activeKey == 0 ? unref(searchListRef) : unref(columnListRef)
    if (!elRef) return
    const el = (elRef as any).$el
    if (!el) return
    Sortablejs.create(unref(el), {
      animation: 500,
      delay: 400,
      delayOnTouchOnly: true,
      handle: state.activeKey == 0 ? '.table-search-drag-icon' : '.table-column-drag-icon',
      onEnd: (evt) => {
        const { oldIndex, newIndex } = evt
        if (isNullAndUnDef(oldIndex) || isNullAndUnDef(newIndex) || oldIndex === newIndex) return
        const list = state.activeKey == 0 ? state.dataForm.searchList : state.dataForm.columnList
        if (oldIndex > newIndex) {
          list.splice(newIndex, 0, list[oldIndex])
          list.splice(oldIndex + 1, 1)
        } else {
          list.splice(newIndex + 1, 0, list[oldIndex])
          list.splice(oldIndex, 1)
        }
      }
    })
  })
}
function getRealList(list, checkedList) {
  return list.map((o) => {
    const show = checkedList.findIndex((c) => c == o.id) !== -1
    return { ...o, show: show }
  })
}
function addOrUpdateHandle(id) {
  state.dataForm.searchList = getRealList(state.dataForm.searchList, state.searchCheckedList)
  state.dataForm.columnList = getRealList(state.dataForm.columnList, state.columnCheckedList)
  const methods = id ? updateView : createView
  const loading = id ? 'showUpdateLoading' : 'showAddLoading'
  const query = {
    ...state.dataForm,
    searchList: JSON.stringify(state.dataForm.searchList),
    columnList: JSON.stringify(state.dataForm.columnList),
    id,
    menuId: props.menuId,
    userId:useUserStore().userid
  }
  state[loading] = true
  methods(query)
    .then((res) => {
      if (res.code == 200 && res.success) {
        createMessage.success(res.message)
        emit('reload', state.dataForm.id)
      }else{
        createMessage.warning(res.message)
      }
      state[loading] = false
    })
    .catch((error) => {
        createMessage.warning(error.message)
      state[loading] = false
    })
}
</script>
<style lang="less">
.column-setting-body {
  display: flex;
  flex-direction: column;
  width: 100%;
  .check-contain {
    padding: 10px;
    width: 100%;
    .check-item {
      display: flex;
      align-items: center;
      min-width: 100%;
      padding: 4px 0 8px 0;
      .table-search-drag-icon,
      .table-column-drag-icon {
        margin: 0 5px;
        cursor: move;
      }
      .ant-checkbox-wrapper {
        flex: 1;
        overflow: hidden;
        display: flex;
        span:last-child {
          flex: 1;
          min-width: 0;
          div {
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
            min-width: 0;
          }
        }
      }
    }
  }
}
</style>
