<template>
  <div :class="`${prefixCls}`">
    <div class="left-board common-board">
      <a-tabs v-model:activeKey="leftTabActiveKey" :tabBarGutter="10" class="average-tabs" v-if="!formInfo.isKit">
        <a-tab-pane key="1" tab="控件" />
        <a-tab-pane key="2" tab="模板" />
      </a-tabs>
      <ScrollContainer>
        <a-collapse v-model:activeKey="leftActiveKey" expandIconPosition="end" ghost v-show="leftTabActiveKey === '1'">
          <a-collapse-panel :header="item.title" :key="item.id" v-for="item in leftComponents" class="components-list">
            <draggable
              class="components-draggable"
              v-model="item.list"
              item-key="__config__.ceriKey"
              filter=".disabled"
              :sort="false"
              draggable=".components-item"
              :clone="cloneComponent"
              :group="{ name: 'componentsGroup', pull: 'clone', put: false }"
              @end="onLeftEnd">
              <template #item="{ element }">
                <div class="components-item" :class="{ disabled: element.__config__.dragDisabled }" @click="addComponent(element)">
                  <div class="components-body">
                    <i :class="element.__config__.tagIcon" />
                    {{ element.__config__.label }}
                    <BasicHelp text="不支持代码生成" v-if="element.__config__.ceriKey === 'calculate'" />
                  </div>
                </div>
              </template>
            </draggable>
          </a-collapse-panel>
        </a-collapse>
        <a-collapse v-model:activeKey="leftKitActiveKey" expandIconPosition="end" ghost v-show="leftTabActiveKey === '2'">
          <a-collapse-panel :header="item.fullName" :key="item.id" v-for="item in state.kitList" class="components-list">
            <draggable
              class="components-draggable"
              v-model="item.children"
              item-key="id"
              filter=".disabled"
              :sort="false"
              draggable=".components-item"
              :clone="cloneKit"
              :group="{ name: 'componentsGroup', pull: 'clone', put: false }"
              @end="onLeftKitEnd">
              <template #item="{ element }">
                <div class="components-item" @click="addKit(element)">
                  <div class="components-body">
                    <i :class="element.icon" />
                    <span>{{ element.fullName }}</span>
                  </div>
                </div>
              </template>
            </draggable>
          </a-collapse-panel>
        </a-collapse>
      </ScrollContainer>
    </div>
    <div class="center-board common-board">
      <div class="action-bar">
        <div class="action-bar-left">
          <div class="ceri-device-switch">
            <div class="ceri-device-switch-item" :class="{ 'ceri-device-switch-item--active': showType === 'pc' }" @click="state.showType = 'pc'">
              <a-tooltip title="PC">
                <i class="icon-ym icon-ym-pc" />
              </a-tooltip>
            </div>
            <div class="ceri-device-switch-item" :class="{ 'ceri-device-switch-item--active': showType === 'app' }" @click="state.showType = 'app'">
              <a-tooltip title="APP">
                <i class="icon-ym icon-ym-mobile" />
              </a-tooltip>
            </div>
          </div>
        </div>
        <div class="action-bar-right">
          <a-tooltip :title="t('common.undoText')">
            <a-button type="text" class="action-bar-btn" :disabled="!getCanUndo" @click="handleUndo(replaceDrawingList)">
              <i class="icon-ym icon-ym-undo" />
            </a-button>
          </a-tooltip>
          <a-tooltip :title="t('common.redoText')">
            <a-button type="text" class="action-bar-btn" :disabled="!getCanRedo" @click="handleRedo(replaceDrawingList)">
              <i class="icon-ym icon-ym-redo" />
            </a-button>
          </a-tooltip>
          <a-divider type="vertical" class="action-bar-divider" />
          <a-tooltip :title="t('common.cleanText')">
            <a-button type="text" class="action-bar-btn" @click="handleClear">
              <i class="icon-ym icon-ym-clean" />
            </a-button>
          </a-tooltip>
          <a-tooltip :title="t('common.previewText')">
            <a-button type="text" class="action-bar-btn" @click="handlePreview">
              <i class="icon-ym icon-ym-video-play" />
            </a-button>
          </a-tooltip>
          <AiChatPopover @confirm="handleAiConfirm" />
          <a-tooltip title="存为模板" v-if="!formInfo.isKit">
            <a-button type="text" class="action-bar-btn" @click="openKitForm">
              <i class="icon-ym icon-ym-save" />
            </a-button>
          </a-tooltip>
        </div>
      </div>
      <div class="center-board-main">
        <ScrollContainer v-show="showType === 'pc'">
          <a-row class="center-board-row" :gutter="formConf.gutter || 15">
            <a-form
              :colon="formConf.colon"
              :size="formConf.size"
              :layout="formConf.labelPosition === 'top' ? 'vertical' : 'horizontal'"
              :labelAlign="formConf.labelPosition === 'right' ? 'right' : 'left'">
              <draggable class="drawing-board" v-model="drawingList" :animation="300" group="componentsGroup" item-key="renderKey" @end="onCenterEnd">
                <template #item="{ element, index }">
                  <draggable-item
                    :key="element.renderKey"
                    :drawing-list="drawingList"
                    :element="element"
                    :index="index"
                    :active-id="activeId"
                    :form-conf="formConf"
                    @activeItem="activeFormItem"
                    @copyItem="drawingItemCopy"
                    @deleteItem="drawingItemDelete"
                    :mergeLeftColDisabled="mergeLeftColDisabled"
                    :mergeRightColDisabled="mergeRightColDisabled"
                    :mergeWholeRowDisabled="mergeWholeRowDisabled"
                    :mergeAboveRowDisabled="mergeAboveRowDisabled"
                    :mergeBelowRowDisabled="mergeBelowRowDisabled"
                    :mergeWholeColDisabled="mergeWholeColDisabled"
                    :undoMergeRowDisabled="undoMergeRowDisabled"
                    :undoMergeColDisabled="undoMergeColDisabled"
                    :deleteWholeColDisabled="deleteWholeColDisabled"
                    :deleteWholeRowDisabled="deleteWholeRowDisabled"
                    @addRow="handleTableAddRow"
                    @addCol="handleTableAddCol"
                    @handleTableSetting="handleTableSetting"
                    @handleShowMenu="handleShowMenu"
                    :put="shouldClone"
                    :end="onTableEnd" />
                </template>
              </draggable>
              <div v-show="!drawingList.length" class="empty-info">
                <img src="@/assets/images/emptyElement.png" class="empty-img" />
              </div>
            </a-form>
          </a-row>
        </ScrollContainer>
        <div class="ipad" v-show="showType === 'app'">
          <div class="outerIpad">
            <div class="ipadBody">
              <ScrollContainer>
                <a-row class="center-board-row">
                  <a-form
                    :colon="false"
                    :size="formConf.size"
                    :layout="formConf.labelPosition === 'top' ? 'vertical' : 'horizontal'"
                    :labelAlign="formConf.labelPosition === 'right' ? 'right' : 'left'">
                    <draggable class="drawing-board" v-model="drawingList" :animation="300" group="componentsGroup" item-key="renderKey" @end="onCenterEnd">
                      <template #item="{ element, index }">
                        <draggable-item
                          showType="app"
                          :key="element.renderKey"
                          :drawing-list="drawingList"
                          :element="element"
                          :index="index"
                          :active-id="activeId"
                          :form-conf="formConf"
                          @activeItem="activeFormItem"
                          @copyItem="drawingItemCopy"
                          @deleteItem="drawingItemDelete"
                          :put="debounceShouldClone"
                          :end="onTableEnd" />
                      </template>
                    </draggable>
                    <div v-show="!drawingList.length" class="empty-info app-empty-info">
                      <img src="@/assets/images/emptyElement.png" class="empty-img" />
                    </div>
                  </a-form>
                </a-row>
              </ScrollContainer>
            </div>
          </div>
        </div>
      </div>
    </div>
    <RightPanel
      ref="rightPanelRef"
      v-bind="getRightPanelBind"
      @relationChange="onRelationChange"
      @addTableComponent="handleAddTableComponent"
      @activeFormItem="activeFormItem" />
    <PreviewModal @register="registerPreviewModal" :formConf="formConf" />
    <KitForm @register="registerKitForm" @reload="getKitList" />
    <AiFieldModal @register="registerAiFieldModal" @reload="addComponentByAi" />
  </div>
</template>
<script lang="ts" setup>
  import draggable from 'vuedraggable';
  import { reactive, toRefs, nextTick, watch, onMounted, provide, computed, unref, ref } from 'vue';
  import type { GenItem } from './types/genItem';
  import { inputComponents, selectComponents, systemComponents, layoutComponents, formConf as defaultFormConf } from './helper/componentMap';
  import { noVModelList, noTableAllowList } from './helper/config';
  import { ScrollContainer } from '@/components/Container';
  import DraggableItem from './DraggableItem.vue';
  import RightPanel from './RightPanel.vue';
  import PreviewModal from './components/PreviewModal.vue';
  import KitForm from './components/KitForm.vue';
  import AiChatPopover from './components/AiChatPopover.vue';
  import AiFieldModal from './components/AiFieldModal.vue';
  import { useModal } from '@/components/Modal';
  import { useDesign } from '@/hooks/web/useDesign';
  import { cloneDeep } from 'lodash-es';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useDebounceFn } from '@vueuse/core';
  import { useGeneratorStore } from '@/store/modules/generator';
  import { useRedo } from '@/components/FormGenerator/src/hooks/useRedo';
  import { isUrl } from '@/utils/is';
  import { buildBitUUID } from '@/utils/uuid';
  import { getKitSelector } from '@/api/system/kit';

  interface State {
    leftComponents: any[];
    showType: string;
    formConf: any;
    drawingList: any[];
    copyDrawingList: string;
    activeId: any;
    activeData: any;
    activeItem: any;
    activeTableItem: any;
    showTip: boolean;
    rowIndex: number;
    colIndex: number;
    rowData: any[];
    colData: any[];
    selectCell: any;
    leftActiveKey: string[];
    leftKitActiveKey: string[];
    leftTabActiveKey: string;
    kitList: any[];
    errorList: any[];
  }
  interface ComType {
    setTabActiveKey: (data) => any;
  }

  const props = defineProps(['conf', 'formInfo', 'dbType']);
  const emit = defineEmits(['showValidatePopover']);
  defineExpose({ getData, activeFormItemById, setTabActiveKey });
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const [registerPreviewModal, { openModal: openPreviewModal }] = useModal();
  const [registerKitForm, { openModal: openKitFormModal }] = useModal();
  const [registerAiFieldModal, { openModal: openAiFieldModal }] = useModal();
  const generatorStore = useGeneratorStore();
  const { prefixCls } = useDesign('basic-generator');
  const debounceShouldClone = useDebounceFn(shouldClone, 200);
  const rightPanelRef = ref<Nullable<ComType>>(null);
  let tempActiveData;
  let tempActiveKitData;
  let kitParent;
  const state = reactive<State>({
    leftComponents: [
      { id: '1', title: '基础控件', list: inputComponents },
      { id: '2', title: '高级控件', list: selectComponents },
      { id: '3', title: '系统控件', list: systemComponents },
      { id: '4', title: '布局控件', list: layoutComponents },
    ],
    showType: 'pc',
    formConf: cloneDeep(defaultFormConf),
    drawingList: [],
    copyDrawingList: '',
    activeId: null,
    activeData: {},
    activeItem: {},
    activeTableItem: {},
    showTip: true,
    rowIndex: 0,
    colIndex: 0,
    rowData: [],
    colData: [],
    selectCell: {
      __config__: {
        rowspan: 1,
        colspan: 1,
      },
    },
    leftActiveKey: ['1', '2', '3', '4'],
    leftKitActiveKey: [],
    leftTabActiveKey: '1',
    kitList: [],
    errorList: [],
  });
  const { leftComponents, leftActiveKey, leftKitActiveKey, leftTabActiveKey, showType, drawingList, formConf, activeId } = toRefs(state);
  const { initRedo, addRecord, handleUndo, handleRedo, getCanUndo, getCanRedo } = useRedo();

  const getRightPanelBind = computed(() => {
    return {
      activeData: state.activeData,
      formConf: state.formConf,
      drawingList: state.drawingList,
      formInfo: props.formInfo,
      dbType: props.dbType,
    };
  });
  const mergeLeftColDisabled = computed(() => {
    if (!state.colData.length) return true;
    return state.colIndex <= 0 || state.colData[state.colIndex - 1].__config__.rowspan !== state.selectCell.__config__.rowspan;
  });
  const mergeRightColDisabled = computed(() => {
    if (!state.colData.length) return true;
    let rightColIndex = state.colIndex + state.selectCell.__config__.colspan;
    return (
      state.colIndex >= state.colData.length - 1 ||
      rightColIndex > state.colData.length - 1 ||
      state.colData[rightColIndex].__config__.rowspan !== state.selectCell.__config__.rowspan
    );
  });
  const mergeWholeRowDisabled = computed(() => {
    if (!state.selectCell.__config__ || !state.rowData.length) return true;
    let rowDataChildren = state.rowData[state.rowIndex].__config__.children;
    let startRowspan = rowDataChildren[0].__config__.rowspan;
    let unmatchedFlag = false;
    for (let i = 1; i < rowDataChildren.length; i++) {
      if (rowDataChildren[i].__config__.rowspan !== startRowspan) {
        unmatchedFlag = true;
        break;
      }
    }
    if (unmatchedFlag) return true;
    return state.colData.length <= 1 || state.colData.length === state.selectCell.__config__.colspan;
  });
  const mergeAboveRowDisabled = computed(() => {
    if (!state.rowData.length || state.rowIndex <= 0) return true;
    return (
      state.rowData[state.rowIndex - 1].__config__.children[state.colIndex].__config__.colspan !== state.selectCell.__config__.colspan ||
      state.rowData[state.rowIndex - 1].__config__.children[state.colIndex].__config__.merged
    );
  });
  const mergeBelowRowDisabled = computed(() => {
    if (!state.rowData.length || state.rowIndex == state.rowData.length) return true;
    let belowRowIndex = state.rowIndex + state.selectCell.__config__.rowspan;
    return (
      state.rowIndex >= state.rowData.length - 1 ||
      belowRowIndex > state.rowData.length - 1 ||
      state.rowData[belowRowIndex].__config__.children[state.colIndex].__config__.colspan !== state.selectCell.__config__.colspan ||
      state.rowData[belowRowIndex].__config__.children[state.colIndex].__config__.merged
    );
  });
  const mergeWholeColDisabled = computed(() => {
    if (!state.rowData.length) return true;
    let startColspan = state.rowData[0].__config__.children[state.colIndex].__config__.colspan;
    let unmatchedFlag = false;
    for (let i = 1; i < state.rowData.length; i++) {
      if (state.rowData[i].__config__.children[state.colIndex].__config__.colspan !== startColspan) {
        unmatchedFlag = true;
        break;
      }
    }
    if (unmatchedFlag) return true;
    return state.rowData.length <= 1 || state.rowData.length === state.selectCell.__config__.rowspan;
  });
  const undoMergeRowDisabled = computed(() => {
    return state.selectCell.__config__.merged || state.selectCell.__config__.colspan <= 1;
  });
  const undoMergeColDisabled = computed(() => {
    return state.selectCell.__config__.merged || state.selectCell.__config__.rowspan <= 1;
  });
  const deleteWholeColDisabled = computed(() => {
    if (!state.rowData.length) return true;
    if (state.rowData[0].__config__.children[0].__config__.colspan === state.rowData[0].__config__.children.length) return true;
    let startColspan = state.rowData[0].__config__.children[state.colIndex].__config__.colspan;
    let unmatchedFlag = false;
    for (let i = 1; i < state.rowData.length; i++) {
      if (state.rowData[i].__config__.children[state.colIndex].__config__.colspan !== startColspan) {
        unmatchedFlag = true;
        break;
      }
    }
    if (unmatchedFlag) return true;
    return state.selectCell.__config__.colspan === state.colData.length;
  });
  const deleteWholeRowDisabled = computed(() => {
    if (!state.rowData.length || state.rowData.length <= state.rowIndex) return true;
    if (state.rowData[0].__config__.children[0].__config__.rowspan === state.rowData.length) return true;
    //整行所有单元格行高不一致不可删除！！
    let startRowspan = state.rowData[state.rowIndex].__config__.children[0].__config__.rowspan;
    let unmatchedFlag = false;
    for (let i = 1; i < state.rowData[state.rowIndex].__config__.children.length; i++) {
      if (state.rowData[state.rowIndex].__config__.children[i].__config__.rowspan !== startRowspan) {
        unmatchedFlag = true;
        break;
      }
    }
    if (unmatchedFlag) return true;
    return state.rowData.length === 1 || state.selectCell.__config__.rowspan === state.rowData.length;
  });

  provide('getShowType', () => state.showType);
  provide('getDrawingList', () => state.drawingList);

  watch(
    () => state.drawingList,
    val => {
      if (val.length) return;
      state.activeData = {};
      state.activeId = null;
    },
    { deep: true },
  );

  // 供父组件使用 获取表单JSON
  function getData() {
    return new Promise(resolve => {
      state.errorList = [];
      // 表单验证
      if (!state.drawingList.length) setErrorList('表单内容不能为空');
      // 表单属性验证
      if (state.formConf.hasPrintBtn && !state.formConf.printId?.length) setErrorList('打印模板不能为空', 'formAttr', '表单属性');
      if (state.formConf.useBusinessKey && !state.formConf.businessKeyList?.length)
        setErrorList('业务主键的规则设置中联合字段不能为空', 'formAttr', '表单属性');
      // 控件验证
      const loop = list => {
        for (let i = 0; i < list.length; i++) {
          const e = list[i];
          const config = e.__config__;
          const formId = config.formId;
          const label = config.label;
          if (config.layout === 'colFormItem' && (!noVModelList.includes(config.ceriKey) || e.isStorage)) {
            const RegExp = /(^_([a-zA-Z0-9]_?)*$)|(^[a-zA-Z](_?[a-zA-Z0-9])*_?$)/;
            if (!e.__vModel__) setErrorList('控件字段不能为空', formId, label);
            if (e.__vModel__ && !RegExp.test(e.__vModel__)) setErrorList('控件字段不规范', formId, label);
          }
          if (config.ceriKey === 'billRule') {
            if (config.ruleType === 2) {
              if (config.ruleConfig?.type === 1) {
                const RegExp = /^[0-9]*[1-9]$/;
                if (!config.ruleConfig?.digit) setErrorList('中位数不能为空', formId, label);
                if (!config.ruleConfig?.startNumber) setErrorList('起始值不能为空', formId, label);
                if (config.ruleConfig?.startNumber && !RegExp.test(config.ruleConfig?.startNumber)) setErrorList('起始值只能输入数字且不能为0', formId, label);
              }
            } else {
              if (!config.rule) setErrorList('单据模板不能为空', formId, label);
            }
          }
          if (config.ceriKey === 'relationForm') {
            if (!e.modelId) setErrorList('关联功能不能为空', formId, label);
            if (!e.propsValue) setErrorList('存储字段不能为空', formId, label);
            if (!e.relationField) setErrorList('回显字段不能为空', formId, label);
          }
          if (config.ceriKey === 'popupSelect' || config.ceriKey === 'popupTableSelect') {
            if (!e.interfaceId) setErrorList('数据接口不能为空', formId, label);
            if (!e.propsValue) setErrorList('存储字段不能为空', formId, label);
            if (!e.relationField) setErrorList('回显字段不能为空', formId, label);
          }
          if (config.ceriKey === 'autoComplete') {
            if (!e.interfaceId) setErrorList('数据接口不能为空', formId, label);
            if (!e.relationField) setErrorList('回显字段不能为空', formId, label);
          }
          if (config.layout === 'rowFormItem' && !config.children.length) setErrorList('控件中没有组件', '', label);
          if (config.ceriKey === 'uploadFile' || config.ceriKey === 'uploadImg') {
            if (config.ceriKey === 'uploadImg' && e.showType === 'button' && !e.buttonText) setErrorList('按钮文字不能为空', formId, label);
            if (config.ceriKey === 'uploadFile' && !e.buttonText) setErrorList('按钮文字不能为空', formId, label);
            if (e.pathType === 'selfPath') {
              if (!e.sortRule || !e.sortRule.length) setErrorList('自定义路径不能为空', formId, label);
              if (e.sortRule?.includes('3')) {
                if (!e.folder) {
                  setErrorList('文件夹名不能为空', formId, label);
                } else {
                  const regex = /^[a-zA-Z0-9][a-zA-Z0-9\-_]{0,99}$/;
                  let flag = false;
                  const folderList = e.folder.split('/');
                  folder: for (let i = 0; i < folderList.length; i++) {
                    if (!folderList[i] || !regex.test(folderList[i])) {
                      flag = true;
                      break folder;
                    }
                  }
                  if (flag) {
                    setErrorList('文件夹名不正确', formId, label);
                  }
                }
              }
            }
          }
          if (config.ceriKey === 'link' || config.ceriKey === 'iframe') {
            if (e.href && !isUrl(e.href)) setErrorList('请输入正确的链接地址', formId, label);
          }
          if (config && config.children && Array.isArray(config.children)) {
            loop(config.children);
          }
        }
      };
      loop(state.drawingList);
      assembleFormData();
      resolve({ formData: state.formConf, errorList: state.errorList, target: 1 });
    });
  }
  function setErrorList(errorInfo, id = '', title = '表单设计') {
    state.errorList.push({ errorInfo, id, title });
  }
  function addComponent(item) {
    if (item.__config__.dragDisabled) return;
    const clone = cloneComponent(item);
    (state.drawingList as any[]).push(clone);
    activeFormItem(clone);
    addLocalRecord(state.drawingList);
  }
  function cloneComponent(origin) {
    const clone = cloneDeep(origin);
    const config = clone.__config__;
    config.span = state.formConf.span; // 生成代码时，会根据span做精简判断
    createIdAndKey(clone);
    tempActiveData = clone;
    return tempActiveData;
  }
  function addKit(item) {
    const kitTemp = getKitTemp(item.fields);
    //添加控件到中间的面板
    (state.drawingList as any[]).push(...kitTemp);
    activeFormItem(kitTemp[0]);
    addLocalRecord(state.drawingList);
  }
  function cloneKit(origin) {
    tempActiveKitData = getKitTemp(origin.fields);
  }
  function getKitTemp(fields) {
    let list: any = [];
    const vModelList: any = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
      if (Array.isArray(data)) return data.forEach(d => loop(d, parent));
      const uuid = buildBitUUID();
      data.__config__.formId = 'formItem' + uuid;
      if (data.__vModel__ && data.__config__.ceriKey) {
        const oldVal = data.__vModel__;
        const newVal = toggleVmodelCase(`${data.__config__.ceriKey}Field${uuid}`);
        data.__vModel__ = !generatorStore.getHasTable ? newVal : '';
        vModelList.push({ oldVal, newVal });
      }
    };
    loop(fields);
    let fieldsStr = JSON.stringify(fields);
    for (let i = 0; i < vModelList.length; i++) {
      const e = vModelList[i];
      fieldsStr = fieldsStr.replaceAll(e.oldVal, e.newVal);
    }
    list = JSON.parse(fieldsStr);
    return list;
  }
  function createIdAndKey(item, parent: Nullable<GenItem> = null) {
    const uuid = buildBitUUID();
    const config = item.__config__;
    config.formId = 'formItem' + uuid;
    config.renderKey = +new Date(); // 改变renderKey后可以实现强制更新组件
    if (config.layout === 'colFormItem') {
      if (noVModelList.indexOf(config.ceriKey) < 0 || item.isStorage) {
        item.__vModel__ = !generatorStore.getHasTable ? toggleVmodelCase(`${config.ceriKey}Field${uuid}`) : '';
      }
      if (parent?.__vModel__ && parent?.__config__.ceriKey === 'table') {
        item.__config__.parentVModel = parent.__vModel__;
      }
    } else if (config.layout === 'rowFormItem') {
      if (config.ceriKey === 'table') {
        item.__vModel__ = toggleVmodelCase(`${config.ceriKey}Field${uuid}`);
      }
      config.componentName = `${config.ceriKey}${uuid}`;
      !Array.isArray(config.children) && (config.children = []);
    }
    if (Array.isArray(config.children)) {
      config.children = config.children.map(childItem => createIdAndKey(childItem, item));
    }
    return item;
  }
  function toggleVmodelCase(str) {
    const dbType = props.dbType || '';
    if (dbType.toLowerCase() === 'Oracle'.toLowerCase() || dbType.toLowerCase() === 'DM'.toLowerCase()) {
      return str.toUpperCase();
    }
    if (dbType.toLowerCase() === 'PostgreSQL'.toLowerCase() || dbType.toLowerCase() === 'KingBaseES'.toLowerCase()) {
      return str.toLowerCase();
    }
    return str;
  }
  function activeFormItem(element) {
    state.activeData = element;
    state.activeId = element.__config__.formId;
  }
  function drawingItemCopy(item, parent, isActiveFormItem = true) {
    let clone = cloneDeep(item);
    clone = createIdAndKey(clone);
    parent.push(clone);
    isActiveFormItem && activeFormItem(clone);
    addLocalRecord(state.drawingList);
  }
  function drawingItemDelete(index, parent) {
    parent.splice(index, 1);
    nextTick(() => {
      const len = state.drawingList.length;
      if (len) activeFormItem(state.drawingList[len - 1]);
      addLocalRecord(state.drawingList);
    });
  }
  function assembleFormData() {
    state.formConf = {
      ...state.formConf,
      fields: cloneDeep(state.drawingList),
    };
  }
  function replaceDrawingList(data) {
    state.drawingList = cloneDeep(data);
    state.copyDrawingList = JSON.stringify(state.drawingList);
    let boo = false;
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const e = list[i];
        if (e.__config__.formId === state.activeId) {
          activeFormItem(e);
          boo = true;
        }
        const config = e.__config__;
        if (config && config.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(state.drawingList);
    if (!boo) {
      state.activeData = {};
      state.activeId = null;
    }
  }
  function onLeftKitEnd(obj) {
    if (!tempActiveKitData?.length) return;
    nextTick(() => {
      state.showTip = true;
      if (!kitParent) {
        (state.drawingList as any[]).splice(obj.newIndex, 0, ...tempActiveKitData);
      } else {
        kitParent.__config__.children.splice(obj.newIndex, 0, ...tempActiveKitData);
        kitParent = null;
      }
      activeFormItem(tempActiveKitData[0]);
      addLocalRecord(state.drawingList);
    });
  }
  function onLeftEnd(obj) {
    state.showTip = true;
    if (obj.from !== obj.to) {
      state.activeData = tempActiveData;
      state.activeId = tempActiveData.__config__.formId;
    }
    if (obj.to.className.indexOf('table-wrapper') > -1) {
      (state.activeItem as GenItem).__config__.isSubTable = true;
      (state.activeItem as GenItem).__config__.parentVModel = (state.activeTableItem as GenItem).__vModel__;
      if (generatorStore.getHasTable) {
        (state.activeItem as GenItem).__config__.relationTable = (state.activeTableItem as GenItem).__config__.tableName;
        (state.activeItem as GenItem).__vModel__ = '';
      }
    }
    addLocalRecord(state.drawingList);
  }
  function onCenterEnd(obj) {
    state.showTip = true;
    if (obj.from === obj.to) return addLocalRecord(state.drawingList);
    if (obj.to.className.indexOf('table-wrapper') > -1) {
      (state.activeItem as GenItem).__config__.isSubTable = true;
      (state.activeItem as GenItem).__config__.parentVModel = (state.activeTableItem as GenItem).__vModel__;
      if (generatorStore.getHasTable) {
        (state.activeItem as GenItem).__config__.relationTable = (state.activeTableItem as GenItem).__config__.tableName;
        (state.activeItem as GenItem).__vModel__ = '';
      }
    }
    addLocalRecord(state.drawingList);
  }
  function onTableEnd(obj) {
    if (obj.from == obj.to) return addLocalRecord(state.drawingList);
    if (obj.to.className.indexOf('table-wrapper') < 0) {
      if ((state.activeItem as GenItem).__config__) {
        (state.activeItem as GenItem).__config__.isSubTable = false;
        (state.activeItem as GenItem).__config__.parentVModel = '';
        if (generatorStore.getHasTable) (state.activeItem as GenItem).__vModel__ = '';
      }
    } else {
      (state.activeItem as GenItem).__config__.isSubTable = true;
      (state.activeItem as GenItem).__config__.parentVModel = (state.activeTableItem as GenItem).__vModel__;
      if (generatorStore.getHasTable) {
        (state.activeItem as GenItem).__config__.relationTable = (state.activeTableItem as GenItem).__config__.tableName;
        (state.activeItem as GenItem).__vModel__ = '';
      }
    }
    addLocalRecord(state.drawingList);
  }
  //  阻止表格中嵌套行容器
  function shouldClone(_to, _from, target, _event, conf) {
    const targetConf = target._underlying_vm_;
    if (conf.__config__.ceriKey === 'table') {
      if (!targetConf) return false;
      if (noTableAllowList.includes(targetConf.__config__.ceriKey) || state.leftTabActiveKey === '2') {
        return false;
      }
      if (targetConf.__config__.layout === 'rowFormItem') return false;
      if (generatorStore.getHasTable) {
        if (!conf.__config__.tableName) {
          if (state.showTip) {
            createMessage.warning(`子表请先关联数据表`);
            state.showTip = false;
          }
          return false;
        }
      }
      state.activeItem = targetConf;
      state.activeTableItem = conf;
    }
    if (!targetConf && state.leftTabActiveKey === '2') {
      kitParent = conf;
      return;
    }
    if (conf.__config__.ceriKey === 'tableGridTd' && targetConf.__config__.ceriKey === 'tableGrid') return false;
    return true;
  }
  // 预览表单
  function handlePreview() {
    assembleFormData();
    openPreviewModal(true, { formConf: unref(state.formConf) });
  }
  // 清空
  function handleClear() {
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: t('formGenerator.cleanComponentTip'),
      onOk: () => {
        state.drawingList = [];
        addLocalRecord(state.drawingList);
      },
    });
  }
  function addLocalRecord(val) {
    if (JSON.stringify(val) != state.copyDrawingList) {
      state.copyDrawingList = JSON.stringify(val);
      addRecord(val);
    }
  }
  function onRelationChange() {
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const config = list[i].__config__;
        if (list[i].relationField && list[i].relationField === list[i].__vModel__) {
          list[i].showField = '';
        }
        if (config?.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(state.drawingList);
  }
  function handleTableAddRow(element, insertPos, colIndex, aboveFlag) {
    const row = element.__config__.children;
    let rowIdx = !!aboveFlag ? insertPos : insertPos + 1;
    if (!aboveFlag) {
      //继续向下寻找同列第一个未被合并的单元格
      let tmpRowIdx = rowIdx;
      let rowFoundFlag = false;
      while (tmpRowIdx < row.length) {
        if (!row[tmpRowIdx].__config__.children[colIndex].__config__.merged) {
          rowIdx = tmpRowIdx;
          rowFoundFlag = true;
          break;
        } else {
          tmpRowIdx++;
        }
      }
      if (!rowFoundFlag) {
        rowIdx = row.length;
      }
    }
    let newRow = cloneDeep(row[insertPos]) || cloneDeep(row[row.length - 1]);
    newRow.__config__.children.forEach(col => {
      col.__config__.formId = 'formItem' + buildBitUUID();
      col.__config__.merged = false;
      col.__config__.colspan = 1;
      col.__config__.rowspan = 1;
      col.__config__.children = [];
    });
    newRow.__config__.formId = 'formItem' + buildBitUUID();
    newRow.__config__.ceriKey = 'tableGridTr';
    element.__config__.children.splice(rowIdx, 0, newRow);
    let colNo = 0;
    while (rowIdx < row.length - 1 && colNo < row[0].__config__.children.length) {
      const cellOfNextRow = row[rowIdx + 1].__config__.children[colNo];
      const rowMerged = cellOfNextRow.__config__.merged;
      if (!!rowMerged) {
        let unMergedCell: any = {};
        let startRowIndex = null;
        for (let i = rowIdx; i >= 0; i--) {
          //查找该行已合并的主单元格
          if (!row[i].__config__.children[colNo].__config__.merged && row[i].__config__.children[colNo].__config__.rowspan > 1) {
            startRowIndex = i;
            unMergedCell = row[i].__config__.children[colNo];
            break;
          }
        }
        //如果有符合条件的unMergedCell
        let newRowspan = unMergedCell.__config__.rowspan + 1;
        setPropsOfMergedRows(startRowIndex, unMergedCell.__config__.colspan, newRowspan, colNo);
        colNo += unMergedCell.__config__.colspan;
      } else {
        colNo += cellOfNextRow.__config__.colspan || 1;
      }
    }
  }
  function handleTableAddCol(element, insertPos) {
    const row = element.__config__.children;
    let colIdx = insertPos === undefined ? row[0].__config__.children.length : insertPos; //确定插入列位置
    row.forEach(item => {
      let newCol = {
        __config__: {
          ceriKey: 'tableGridTd',
          merged: false,
          colspan: 1,
          rowspan: 1,
          formId: 'formItem' + buildBitUUID(),
          children: [],
          backgroundColor: '',
        },
      };
      item.__config__.children.splice(colIdx, 0, newCol);
    });
  }
  function mergeTableCol(_element, type = 0) {
    let mergedColIndex = type == 1 ? state.colIndex : state.colIndex + state.colData[state.colIndex].__config__.colspan;
    let remainedColIndex = type == 1 ? state.colIndex - state.colData[state.colIndex - 1].__config__.colspan : state.colIndex;
    const colChildren = state.colData[mergedColIndex].__config__.children;
    const colChildren_ = state.colData[remainedColIndex].__config__.children;
    state.colData[remainedColIndex].__config__.children = [...colChildren_, ...cloneDeep(colChildren)];
    let newColspan = state.colData[mergedColIndex].__config__.colspan * 1 + state.colData[remainedColIndex].__config__.colspan * 1;
    setPropsOfMergedCols(remainedColIndex, newColspan, state.selectCell.__config__.rowspan);
  }
  function mergeWholeCol() {
    let childrenData = state.colData.filter(colItem => {
      return !colItem.merged && colItem.__config__.children?.length;
    });
    if (childrenData?.length) {
      childrenData.map((o, i) => {
        if (i == 0) state.colData[0].__config__.children = cloneDeep(o.__config__.children);
        if (i != 0) state.colData[0].__config__.children.push(...cloneDeep(o.__config__.children));
      });
    }
    setPropsOfMergedCols(0, state.colData.length, state.colData[state.colIndex].__config__.rowspan);
  }
  function mergeTableRow(type = 0) {
    let mergedRowIndex = type == 1 ? state.rowIndex : state.rowIndex + state.selectCell.__config__.rowspan;
    let remainedRowIndex = type == 1 ? state.rowIndex - 1 : state.rowIndex;
    let childrenData = state.rowData[mergedRowIndex].__config__.children[state.colIndex].__config__.children;
    let childrenData_ = state.rowData[remainedRowIndex].__config__.children[state.colIndex].__config__.children;
    state.rowData[remainedRowIndex].__config__.children[state.colIndex].__config__.children = [...childrenData_, ...cloneDeep(childrenData)];
    let newRowspan =
      state.rowData[mergedRowIndex].__config__.children[state.colIndex].__config__.rowspan * 1 +
      state.rowData[remainedRowIndex].__config__.children[state.colIndex].__config__.rowspan * 1;
    setPropsOfMergedRows(remainedRowIndex, state.selectCell.__config__.colspan, newRowspan);
  }
  function mergeWholeRow() {
    let childrenData: any[] = [];
    state.rowData.forEach(o => {
      let tempCell = o.__config__.children[state.colIndex];
      if (!o.__config__.merged && !!o.__config__.children && o.__config__.children.length) {
        childrenData.push(tempCell);
      }
    });
    let firstCellOfCol = state.rowData[0].__config__.children[state.colIndex];
    if (childrenData && childrenData.length) {
      childrenData.map((o, i) => {
        if (i != 0) firstCellOfCol.__config__.children.push(...cloneDeep(o.__config__.children));
      });
    }
    setPropsOfMergedRows(0, firstCellOfCol.__config__.colspan, state.rowData.length);
  }
  function undoMergeCol() {
    setPropsOfSplitCol(state.colIndex, state.selectCell.__config__.colspan, state.selectCell.__config__.rowspan);
  }
  function undoMergeRow() {
    setPropsOfSplitRow(state.colIndex, state.selectCell.__config__.colspan, state.selectCell.__config__.rowspan);
  }
  function deleteWholeCol() {
    let startColspan = state.rowData[0].__config__.children[state.colIndex].__config__.colspan;
    state.rowData.forEach(rItem => {
      rItem.__config__.children.splice(state.colIndex, startColspan);
    });
  }
  function deleteWholeRow() {
    let startRowspan = state.rowData[state.rowIndex].__config__.children[0].__config__.rowspan;
    state.rowData.splice(state.rowIndex, startRowspan);
  }
  function setPropsOfMergedCols(startColIndex, newColspan, rowspan) {
    for (let i = state.rowIndex; i < state.rowIndex + rowspan; i++) {
      for (let j = startColIndex; j < startColIndex + newColspan; j++) {
        if (i === state.rowIndex && j === startColIndex) {
          state.rowData[i].__config__.children[j].__config__.colspan = newColspan;
          continue;
        }
        state.rowData[i].__config__.children[j].__config__.merged = true;
        state.rowData[i].__config__.children[j].__config__.colspan = newColspan;
        state.rowData[i].__config__.children[j].__config__.children = [];
      }
    }
  }
  function setPropsOfMergedRows(startRowIndex, colspan, newRowspan, colIndex = 0) {
    if (!colIndex) colIndex = state.colIndex;
    for (let i = startRowIndex; i < startRowIndex + newRowspan; i++) {
      for (let j = colIndex; j < colIndex + colspan; j++) {
        if (i === startRowIndex && j === colIndex) {
          state.rowData[i].__config__.children[j].__config__.rowspan = newRowspan;
          continue;
        }
        state.rowData[i].__config__.children[j].__config__.merged = true;
        state.rowData[i].__config__.children[j].__config__.rowspan = newRowspan;
        state.rowData[i].__config__.children[j].__config__.children = [];
      }
    }
  }
  function setPropsOfSplitCol(startColIndex, colspan, rowspan) {
    for (let i = state.rowIndex; i < state.rowIndex + rowspan; i++) {
      for (let j = startColIndex; j < startColIndex + colspan; j++) {
        state.rowData[i].__config__.children[j].__config__.merged = false;
        state.rowData[i].__config__.children[j].__config__.rowspan = 1;
        state.rowData[i].__config__.children[j].__config__.colspan = 1;
      }
    }
  }
  function setPropsOfSplitRow(startColIndex, colspan, rowspan) {
    for (let i = state.rowIndex; i < state.rowIndex + rowspan; i++) {
      for (let j = startColIndex; j < startColIndex + colspan; j++) {
        state.rowData[i].__config__.children[j].__config__.merged = false;
        state.rowData[i].__config__.children[j].__config__.rowspan = 1;
        state.rowData[i].__config__.children[j].__config__.colspan = 1;
      }
    }
  }
  function handleTableSetting(e, element) {
    switch (e) {
      case '1':
        //插入左侧列
        handleTableAddCol(element, state.colIndex);
        break;
      case '2':
        //插入右侧列
        handleTableAddCol(element, state.colIndex + 1);
        break;
      case '3':
        //插入上方行
        handleTableAddRow(element, state.rowIndex, state.colIndex, true);
        break;
      case '4':
        //插入下方行
        handleTableAddRow(element, state.rowIndex, state.colIndex, false);
        break;
      case '5':
        //向左合并
        mergeTableCol(element, 1);
        break;
      case '6':
        //向右合并
        mergeTableCol(element);
        break;
      case '7':
        //合并整行
        mergeWholeCol();
        break;
      case '8':
        //向上合并
        mergeTableRow(1);
        break;
      case '9':
        //向下合并
        mergeTableRow();
        break;
      case '10':
        //合并整列
        mergeWholeRow();
        break;
      case '11':
        //撤销行合并
        undoMergeCol();
        break;
      case '12':
        //撤销列合并
        undoMergeRow();
        break;
      case '13':
        //删除整列
        deleteWholeCol();
        break;
      case '14':
        //删除整行
        deleteWholeRow();
        break;
      default:
        break;
    }
    resetData();
  }
  function resetData() {
    state.rowIndex = 0;
    state.colIndex = 0;
    state.rowData = [];
    state.colData = [];
    state.selectCell = {
      __config__: {
        rowspan: 1,
        colspan: 1,
      },
    };
  }
  function handleShowMenu(element, rowIndex, colIndex) {
    state.rowIndex = rowIndex;
    state.colIndex = colIndex;
    state.rowData = element.__config__.children;
    state.colData = state.rowData[rowIndex].__config__.children;
    state.selectCell = state.colData[colIndex];
  }
  function initData() {
    if (typeof props.conf === 'object' && props.conf !== null) {
      state.drawingList = cloneDeep(props.conf.fields);
      Object.assign(state.formConf, props.conf);
      if (state.drawingList.length) activeFormItem(state.drawingList[0]);
    } else {
      state.drawingList = [];
    }
    addLocalRecord(state.drawingList);
  }
  function handleAddTableComponent(item, parent) {
    drawingItemCopy(item, parent, false);
  }
  // 获取模板列表
  function getKitList() {
    getKitSelector().then(res => {
      const list = res.data;
      for (let i = 0; i < list.length; i++) {
        list[i].children = list[i].children.map(o => {
          const formData = o.formData ? JSON.parse(o.formData) : null;
          return { ...o, fields: !formData ? [] : formData.fields };
        });
      }
      state.kitList = list;
      state.leftKitActiveKey = list.map(o => o.id);
    });
  }
  // 另存为模板
  function openKitForm() {
    getData()
      .then((res: any) => {
        if (res.errorList?.length) {
          emit('showValidatePopover', res.errorList);
          return;
        }
        const formData = cloneDeep(defaultFormConf);
        formData.fields = (res as any)?.formData?.fields || [];
        const data = {
          formData: JSON.stringify(formData),
          fullName: props.formInfo?.fullName,
          enCode: props.formInfo?.enCode + buildBitUUID(),
          category: props.formInfo?.category,
        };
        openKitFormModal(true, data);
      })
      .catch(err => {
        err.msg && createMessage.warning(err.msg);
      });
  }
  function activeFormItemById(id) {
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const e = list[i];
        if (e.__config__.formId === id) {
          state.activeData = e;
          state.activeId = e.__config__.formId;
          break;
        }
        const config = e.__config__;
        if (config && config.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(state.drawingList);
  }
  function setTabActiveKey(data) {
    unref(rightPanelRef)?.setTabActiveKey(data);
  }
  function handleAiConfirm(list) {
    openAiFieldModal(true, { list });
  }
  function addComponentByAi(list) {
    list.forEach(o => addComponent(o));
  }

  onMounted(() => {
    state.leftActiveKey = ['1', '2', '3', '4'];
    initRedo();
    initData();
    state.leftTabActiveKey = '1';
    if (!props.formInfo?.isKit) getKitList();
  });
</script>
