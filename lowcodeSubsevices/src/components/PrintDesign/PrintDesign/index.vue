<template>
  <div :class="prefixCls">
    <div class="design-wrap">
      <div class="flex-row">
        <div class="left-side">
          <a-tabs class="average-tabs" v-model:activeKey="activeKey" :tabBarGutter="10">
            <a-tab-pane key="1" tab="组件" />
            <a-tab-pane key="2" tab="数据源" />
          </a-tabs>
          <div class="tabs-content">
            <ScrollContainer v-show="activeKey === '1'">
              <a-collapse v-model:activeKey="leftActiveKey" expandIconPosition="end" ghost>
                <a-collapse-panel v-for="item in leftDragList" :key="item.id" class="components-list" id="providerContainer">
                  <template #header>
                    <div class="components-list-title">{{ item.title }}</div>
                  </template>
                  <div class="components-item ep-draggable-item" v-for="child in item.list" :key="child.id" :tid="'providerModule.' + child.id">
                    <div class="components-body">
                      <i :class="child.icon" />
                      {{ child.label }}
                    </div>
                  </div>
                </a-collapse-panel>
                <a-collapse-panel key="4" class="components-list" v-if="seniorComponents.length">
                  <template #header>
                    <div class="components-list-title">复合组件</div>
                  </template>
                  <draggable
                    class="components-draggable"
                    v-model="seniorComponents"
                    item-key="id"
                    :sort="false"
                    draggable=".components-item"
                    @start="onDragStart"
                    @end="onDragEnd">
                    <template #item="{ element }">
                      <div class="components-item">
                        <div class="components-body">
                          <i :class="element.icon" />
                          {{ element.label }}
                        </div>
                      </div>
                    </template>
                  </draggable>
                </a-collapse-panel>
                <a-collapse-panel key="5" class="components-list" v-if="businessComponents.length">
                  <template #header>
                    <div class="components-list-title">业务套件<span class="title-tip">(点击添加)</span></div>
                  </template>
                  <div class="components-item ep-click-item" v-for="item in businessComponents" :key="item.id" @click="handleAddBusiness(item.templateJson)">
                    <div class="components-body">
                      <i :class="item.icon" />
                      {{ item.label }}
                    </div>
                  </div>
                </a-collapse-panel>
              </a-collapse>
            </ScrollContainer>
            <div class="dataSet-content" v-show="state.activeKey === '2'">
              <div class="dataSet-content-header">
                <a-button type="primary" size="small" preIcon="icon-ym icon-ym-btn-add" @click="handleAddDataSet">数据集</a-button>
                <a-button type="link" size="small" @click="handleConvertConfig" class="!p-0">数据转换</a-button>
              </div>
              <div class="dataSet-content-main">
                <BasicLeftTree
                  ref="dataSetTreeRef"
                  :showSearch="false"
                  :treeData="dataSetList"
                  :fieldNames="{ key: 'ceriId', title: 'fullName' }"
                  @select="handleTreeNodeClick"
                  :actionList="actionList" />
              </div>
            </div>
          </div>
        </div>
        <div class="center-side">
          <div class="center-side-tool-wrap">
            <div class="left-handle-tool-wrap">
              <a-space-compact>
                <a-dropdown placement="bottom">
                  <template #overlay>
                    <a-menu @click="handleAlign">
                      <a-menu-item v-for="item in verticalAlignList" :key="item?.type">
                        <a-tooltip placement="right" :title="item.title">
                          <i :class="item.preIcon" />
                        </a-tooltip>
                      </a-menu-item>
                    </a-menu>
                  </template>
                  <a-button preIcon="icon-ym icon-ym-flow-align-left" />
                </a-dropdown>
                <a-dropdown placement="bottom">
                  <template #overlay>
                    <a-menu @click="handleAlign">
                      <a-menu-item v-for="item in horizontalAlignList" :key="item?.type">
                        <a-tooltip placement="right" :title="item.title">
                          <i :class="item.preIcon" />
                        </a-tooltip>
                      </a-menu-item>
                    </a-menu>
                  </template>
                  <a-button preIcon="icon-ym icon-ym-flow-align-top" />
                </a-dropdown>
                <a-tooltip :title="item.title" v-for="item in distributeList" :key="item.type">
                  <a-button @click="setElsAlign(item.type)" :title="item.title" :preIcon="item.preIcon" />
                </a-tooltip>
              </a-space-compact>
              <a-space-compact class="size-tool-wrap">
                <a-button v-for="item in paperTypes" :key="item?.type" :type="getPresetPaperButtonType(item?.type)" @click="setPresetPaper(item)">
                  {{ item?.type }}
                </a-button>
                <a-popover overlayClassName="custom-print-size-popover" v-model:open="state.paperPopVisible" title="设置纸张宽高(mm)" trigger="click">
                  <template #content>
                    <a-input-group class="custom-size-input-group" compact>
                      <a-input-number class="custom-size-input" v-model:value="state.paperCustomWidth" min="1" step="1" :precision="0" placeholder="宽" />
                      <a-input class="custom-size-input-range" placeholder="X" disabled />
                      <a-input-number class="custom-size-input" v-model:value="state.paperCustomHeight" min="1" step="1" :precision="0" placeholder="高" />
                    </a-input-group>
                    <a-button type="primary" block @click="setCustomPaper">确定</a-button>
                  </template>
                  <a-button class="custom-size-button" :type="getCustomPaperButtonType">自定义</a-button>
                </a-popover>
                <a-button class="custom-size-button" @click="rotatePaper">旋转</a-button>
              </a-space-compact>
              <a-input-group class="scale-tool-wrap" compact>
                <a-button @click="changeScale(false)" preIcon="icon-ym icon-ym-flow-minus" />
                <a-input-number
                  class="scale-value-input"
                  disabled
                  :value="state.scaleValue"
                  :min="state.scaleMin"
                  :max="state.scaleMax"
                  :step="0.1"
                  :formatter="value => formatScaleValue(value)"
                  :parser="value => parserScaleValue(value)" />
                <a-button @click="changeScale(true)" preIcon="icon-ym icon-ym-flow-plus" />
              </a-input-group>
            </div>
            <div class="right-handle-tool-wrap">
              <a-tooltip :title="t('common.undoText')">
                <a-button type="text" class="action-bar-btn" :disabled="!getCanUndo" @click="handleUndo(updateTemp)">
                  <i class="icon-ym icon-ym-undo" />
                </a-button>
              </a-tooltip>
              <a-tooltip :title="t('common.redoText')">
                <a-button type="text" class="action-bar-btn" :disabled="!getCanRedo" @click="handleRedo(updateTemp)">
                  <i class="icon-ym icon-ym-redo" />
                </a-button>
              </a-tooltip>
              <HistoryPopover :recordList="recordList" :activeIndex="currentRecordIndex" @jump="jumpToRecord($event, updateTemp)" class="ml-10px" />
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
            </div>
          </div>
          <!-- 设计器的 容器 -->
          <div id="hiprintPrintTemplate"></div>
          <draggable tag="div" class="draggable-box w-full h-full" :animation="300" group="componentsGroup" itemKey="id" v-show="drag">
            <template #item></template>
          </draggable>
        </div>
        <div class="right-side">
          <!-- 元素参数的 容器 -->
          <div id="PrintElementOptionSetting"></div>
        </div>
      </div>
    </div>
  </div>
  <Preview @register="registerPreview" />
  <DataSetModal type="print" @register="registerDataSetModal" @confirm="onDataSetConfirm" />
  <ConvertModal @register="registerConvertModal" @confirm="updateConvertConfig" />
</template>

<script lang="ts" setup>
  import { onMounted, ref, getCurrentInstance, reactive, computed, unref, toRefs, h, watch, toRaw } from 'vue';
  import { hiprint } from 'vue-plugin-hiprint';
  import { ScrollContainer } from '@/components/Container';
  import { BasicLeftTree, TreeActionItem } from '@/components/Tree';
  import { useCopyToClipboard } from '@/hooks/web/useCopyToClipboard';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useModal } from '@/components/Modal';
  import { newHiprintPrintTemplate } from './utils/template-helper';
  import provider from './provider';
  import {
    subComponents,
    basicComponents,
    systemComponents,
    seniorComponents,
    businessComponents,
    verticalAlignList,
    horizontalAlignList,
    distributeList,
    paperTypes,
  } from './utils/define';
  import {
    paperNumberDisabled,
    paperNumberContinue,
    paperNumberFormat,
    tableTextType,
    tableBarcodeMode,
    tableQRCodeLevel,
    tableColumnH,
    imageWidth,
    imageHeight,
    formatter,
  } from './properties';
  import { useDesign } from '@/hooks/web/useDesign';
  import $ from 'jquery';
  import Preview from '../Preview.vue';
  import { DataSetModal } from '@/components/CommonModal';
  import { FormOutlined, DeleteOutlined } from '@ant-design/icons-vue';
  import { cloneDeep, omit } from 'lodash-es';
  import draggable from 'vuedraggable';
  import dayjs from 'dayjs';
  import { useGlobSetting } from '@/hooks/setting';
  import ConvertModal from './ConvertModal.vue';
  import { onKeyStroke } from '@vueuse/core';
  import { HistoryPopover } from '@/components/CommonPopover';
  import { useRedo } from '@/components/FormGenerator/src/hooks/useRedo';

  defineOptions({ name: 'printDesign' });

  interface State {
    leftDragList: any[];
    templateKey: any;
    activeKey: string;
    curPaper: any;
    paperPopVisible: boolean;
    paperCustomWidth: number;
    paperCustomHeight: number;
    scaleValue: number;
    scaleMax: number;
    scaleMin: number;
    leftActiveKey: string[];
    dataSetList: any[];
    convertConfig: any[];
    drag: boolean;
    mouseOffSet: any;
    copyTempData: any;
    isAuto: boolean;
  }

  const emit = defineEmits(['load']);
  const props = defineProps({
    conf: { type: String, default: '' },
    dataSetList: { type: Array, default: () => [] },
    convertConfig: { type: Array, default: () => [] },
    versionState: { type: Number, default: 0 },
    fullName: { type: String, default: '' },
  });
  const defaultPaper = { type: 'A4', width: 210, height: 297 };
  const defaultPanelConfig = {
    optionItems: [
      paperNumberDisabled,
      paperNumberContinue,
      paperNumberFormat,
      tableTextType,
      tableBarcodeMode,
      tableQRCodeLevel,
      tableColumnH,
      imageWidth,
      imageHeight,
      formatter,
    ],
    panel: {
      supportOptions: [
        { name: 'firstPaperFooter', hidden: true },
        { name: 'evenPaperFooter', hidden: true },
        { name: 'oddPaperFooter', hidden: true },
        { name: 'lastPaperFooter', hidden: true },
        { name: 'panelLayoutOptions', hidden: true },
      ],
    },
    text: {
      tabs: [
        { options: [] },
        { options: [] },
        { options: [] },
        {
          options: [
            { name: 'textType', hidden: true },
            { name: 'tableTextType', hidden: true },
            { name: 'barcodeMode', hidden: true },
            { name: 'barWidth', hidden: true },
            { name: 'barAutoWidth', hidden: true },
            { name: 'qrCodeLevel', hidden: true },
          ],
        },
      ],
    },
    qrcode: {
      tabs: [{ options: [{ name: 'qrcodeType', hidden: true }] }],
    },
    html: {
      tabs: [
        {
          name: '基础',
          replace: true,
          options: [
            { name: 'field', hidden: false },
            { name: 'imageWidth', hidden: false },
            { name: 'imageHeight', hidden: false },
            { name: 'coordinate', hidden: false },
            { name: 'widthHeight', hidden: false },
            { name: 'showInPage', hidden: false },
            { name: 'unShowInPage', hidden: false },
            { name: 'fixed', hidden: false },
          ],
        },
      ],
      supportOptions: [
        { name: 'imageWidth', hidden: false },
        { name: 'imageHeight', hidden: false },
      ],
    },
  };
  /**
   * 变量集合
   */
  let hiprintTemplate: any = null;
  const state = reactive<State>({
    leftDragList: [
      { id: '1', title: '辅助组件', list: subComponents },
      { id: '2', title: '基础组件', list: basicComponents },
      { id: '3', title: '系统组件', list: systemComponents },
    ],
    templateKey: undefined,
    activeKey: '1',
    curPaper: {
      type: 'A4',
      width: 210,
      height: 297,
    },
    // 自定义纸张
    paperPopVisible: false,
    paperCustomWidth: 150,
    paperCustomHeight: 150,
    // 缩放
    scaleValue: 1,
    scaleMax: 5,
    scaleMin: 0.5,
    leftActiveKey: ['1', '2', '3', '4', '5'],
    dataSetList: [],
    convertConfig: [],
    drag: false,
    mouseOffSet: { offsetX: 0, offsetY: 0 },
    copyTempData: null,
    isAuto: false,
  });
  const { leftDragList, activeKey, leftActiveKey, dataSetList, drag } = toRefs(state);
  const { prefixCls } = useDesign('print-designer');
  // ref 创建的模板json
  const templateRef = ref();
  const dataSetTreeRef = ref();
  const globSetting = useGlobSetting();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const [registerPreview, { openModal: openPreviewModal }] = useModal();
  const [registerDataSetModal, { openModal: openDataSetModal }] = useModal();
  const [registerConvertModal, { openModal: openConvertModal }] = useModal();
  const { initRedo, addRecord, handleUndo, handleRedo, getCanUndo, getCanRedo, recordList, currentRecordIndex, jumpToRecord } = useRedo();
  const actionList: TreeActionItem[] = [
    {
      render: node => {
        if (!node.children) return null;
        return h(FormOutlined, {
          class: 'mr-4px',
          title: '编辑',
          onClick: e => {
            e.stopPropagation();
            handleEditDataSet(node);
          },
        });
      },
    },
    {
      render: node => {
        if (!node.children) return null;
        return h(DeleteOutlined, {
          class: 'mr-4px',
          title: '删除',
          onClick: e => {
            e.stopPropagation();
            handleDeleteDataSet(node);
          },
        });
      },
    },
  ];

  const getCustomPaperButtonType = computed(() => {
    return state.curPaper?.type === 'other' ? 'primary' : undefined;
  });

  watch(
    () => state.paperPopVisible,
    val => {
      if (!val) return;
      state.paperCustomHeight = state.curPaper.height;
      state.paperCustomWidth = state.curPaper.width;
    },
  );

  /**
   * 旋转纸张
   */
  const rotatePaper = () => {
    hiprintTemplate.rotatePaper();
  };
  /**
   * 清空所有元素
   */
  const clearPaper = () => {
    hiprintTemplate.clear();
  };
  /**
   * 元素对齐
   */
  const handleAlign = e => {
    setElsAlign(e.key);
  };
  /**
   * 对齐方式
   */
  const setElsAlign = type => {
    hiprintTemplate.setElsAlign(type);
    addLocalRecord('移动组件');
  };
  /**
   * 获取预设尺寸按钮状态
   */
  const getPresetPaperButtonType = type => {
    return state.curPaper?.type === type ? 'primary' : undefined;
  };
  /**
   * 修改预设纸张
   */
  const setPresetPaper = item => {
    const { type, width, height } = item || {};
    const paperTypesCache = paperTypes || [];

    try {
      const matchResult = paperTypesCache?.find(option => option?.type === type);
      state.curPaper = { ...item, type: matchResult !== undefined ? matchResult?.type : 'other' };
      hiprintTemplate.setPaper(width, height);
    } catch (error) {}
  };
  /**
   * 修改自定义纸张
   */
  const setCustomPaper = () => {
    state.paperPopVisible = false;

    const { paperCustomWidth: width, paperCustomHeight: height } = state || {};
    setPresetPaper({ type: 'other', width, height });
  };
  /**
   * 设置初始化纸张
   */
  const setCurPaper = () => {
    if (!templateRef.value) return (state.curPaper = cloneDeep(defaultPaper));
    const panel = templateRef.value.panels[0];
    const { height, width } = panel;
    let curPaper: any = null;
    for (let i = 0; i < paperTypes.length; i++) {
      if (paperTypes[i].height === height && paperTypes[i].width === width) {
        curPaper = paperTypes[i];
        break;
      }
    }
    state.curPaper = curPaper ? curPaper : { type: 'other', width, height };
  };
  /**
   * 修改缩放比例
   */
  const changeScale = grow => {
    const { scaleMax, scaleMin } = state || {};

    let scaleValue = state.scaleValue || 1;

    if (grow) {
      scaleValue += 0.1;
      scaleValue = scaleValue <= scaleMax ? scaleValue : scaleMax;
    } else {
      scaleValue -= 0.1;
      scaleValue = scaleValue >= scaleMin ? scaleValue : scaleMin;
    }

    hiprintTemplate.zoom(scaleValue);
    state.scaleValue = scaleValue;
  };
  /**
   * 格式化比例值
   */
  const formatScaleValue = value => {
    return `${(value * 100)?.toFixed(0)}%`;
  };
  /**
   * 解析比例值
   */
  const parserScaleValue = value => {
    return value?.replace('%', '');
  };
  /**
   * 确认清空设计
   */
  const handleClear = () => {
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: '确定要清空所有设计吗?',
      onOk: () => {
        try {
          clearPaper();
          templateRef.value.panels[0].printElements = [];
        } catch (error) {}
      },
    });
  };

  /**
   * 选中字段
   */
  const handleTreeNodeClick = (_, node) => {
    const { id = '', children, fullName = '' } = node?.dataRef || {};
    const text = !children ? id : fullName;
    const { isSuccessRef } = useCopyToClipboard(text);
    unref(isSuccessRef) && createMessage.success('复制成功');
  };
  const handleEditDataSet = (node: any) => {
    const data: any = omit(node, ['fullName_title', 'children']);
    handleOpenDataSetModal(data);
  };
  const handleDeleteDataSet = (node: any) => {
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: '确定要删除该数据集吗?',
      onOk: () => {
        state.dataSetList = state.dataSetList.filter(o => o.ceriId !== node.ceriId);
        reBuildDesign();
      },
    });
  };
  /**
   * 暴露数据
   */
  const getData = () => {
    return new Promise(resolve => {
      const data = {
        dataSetList: state.dataSetList.map(o => omit(o, ['fullName_title', 'children'])),
        convertConfig: state.convertConfig,
        printTplPanels: JSON.stringify(hiprintTemplate.getJson() || {}),
      };
      resolve(data);
    });
  };
  /**
   * 设计预览
   */
  const handlePreview = () => {
    openPreviewModal(true, { printTplPanels: JSON.stringify(hiprintTemplate.getJson() || {}), fullName: props.fullName });
  };
  /**
   * 添加复合型组件
   */
  const handleAddComplex = templateJson => {
    if (!templateJson) return;
    if (!templateRef.value) templateRef.value = hiprintTemplate.getJson() || {};
    templateRef.value.panels[0].printElements = hiprintTemplate.getJson()?.panels[0]?.printElements?.concat(templateJson) || [];
    buildDesignModel();
  };
  // 添加业务套件
  const handleAddBusiness = templateJson => {
    function addBusiness(isClear = true) {
      if (!templateRef.value) templateRef.value = hiprintTemplate.getJson() || {};
      try {
        if (isClear) {
          state.isAuto = true;
          clearPaper();
          templateRef.value.panels[0].printElements = [];
        }
        if (!templateJson) return;
        templateRef.value.panels[0].printElements = hiprintTemplate.getJson()?.panels[0]?.printElements?.concat(templateJson) || [];
        hiprintTemplate.update(templateRef.value);
        addLocalRecord('新增套件');
      } catch (error) {}
    }
    if (!hiprintTemplate.getJson()?.panels[0]?.printElements.length) return addBusiness(false);
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: '确定后覆盖当前页面设计内容，是否继续?',
      onOk: () => {
        addBusiness();
      },
    });
  };
  /**
   * 构建左侧可拖拽元素
   */
  const buildDragElement = () => {
    const items = $('.ep-draggable-item');
    hiprint.PrintElementTypeManager.buildByHtml(items);
  };
  /**
   * 构建中间设计器
   */
  const buildDesignModel = (isInit = false) => {
    // 先清空, 避免重复构建
    $('#hiprintPrintTemplate').empty();
    $('#PrintElementOptionSetting').empty();

    const fields = convertField();
    hiprintTemplate = newHiprintPrintTemplate(state.templateKey, {
      template: toRaw(templateRef.value),
      fields,
      qtDesigner: true,
      settingContainer: '#PrintElementOptionSetting', // 元素参数容器
      onImageChooseClick: target => {
        // 创建input，模拟点击，然后 target.refresh 更新
        let input = document.createElement('input');
        input.setAttribute('type', 'file');
        input.setAttribute('accept', 'image/*');
        input.click();
        input.onchange = function () {
          const file = (this as any).files[0];
          if (!file) return;
          const isAccept = new RegExp('image/*').test(file.type);
          if (!isAccept) {
            createMessage.error(t('component.upload.uploadImg'));
            return;
          }
          if (file.size / 1024 > 500) {
            createMessage.error('操作失败，图片大小超出500K');
            return;
          }
          let reader = new FileReader();
          //通过文件流行文件转换成Base64字行串
          reader.readAsDataURL(file);
          //转换成功后
          reader.onloadend = function () {
            // 通过 target.refresh 更新图片元素
            target.refresh(reader.result);
          };
        };
        input.remove();
      },
      onDataChanged: type => {
        if (['清空', '调整大小'].includes(type) && state.isAuto) return (state.isAuto = false);
        if (type === '缩放' || (type === '框选移动' && !hiprintTemplate?.getSelectEls()?.length)) return;
        const fullName = getRealFullName(type);
        addLocalRecord(fullName);
      },
    });

    hiprintTemplate.design('#hiprintPrintTemplate');

    setCurPaper();
    if (isInit) addLocalRecord('初始化');
  };
  function getRealFullName(type) {
    // 新增、移动、删除、修改(参数调整)、大小、旋转
    if (type.indexOf('移动') > -1) return '移动组件';
    if (['元素修改', '大小', '旋转'].includes(type)) return '修改组件';
    if (['模板调整', '调整大小'].includes(type)) return '模板调整';
    return type + '组件';
  }
  // 转换hiprint需要的字段
  const convertField = () => {
    let list = [
      { text: 'systemPrintTime(打印时间)', field: 'systemInfo.printTime' },
      { text: 'systemPrinter(打印人员)', field: 'systemInfo.printer' },
    ];
    for (let i = 0; i < state.dataSetList.length; i++) {
      const e = state.dataSetList[i];
      const { children = [], fullName = '' } = e;
      const fields = children.map(o => ({ text: `${fullName}.${o?.fullName}`, field: o?.ceriId }));
      list = list.concat(fields);
    }
    return list;
  };
  // 数据转换配置
  function handleConvertConfig() {
    const fieldOptions = state.dataSetList.map(o => ({
      ...o,
      id: o.id || o.ceriId,
      disabled: true,
      children: !o.children ? [] : o.children.map(c => ({ ...c, fullName: o.fullName + '.' + c.fullName, id: o.fullName + '.' + c.id })),
    }));
    openConvertModal(true, { list: state.convertConfig, fieldOptions });
  }
  function updateConvertConfig(data) {
    state.convertConfig = data;
  }
  // 添加数据集
  function handleAddDataSet() {
    handleOpenDataSetModal();
  }
  function handleOpenDataSetModal(data = null) {
    openDataSetModal(true, { data, list: state.dataSetList });
  }
  function onDataSetConfirm(data) {
    state.dataSetList = data.map(o => ({ ...o, children: !o.children ? [] : o.children.map(c => ({ ...c, ceriId: o.fullName + '.0.' + c.id })) }));
    reBuildDesign();
  }
  function reBuildDesign() {
    templateRef.value = hiprintTemplate.getJson() || {};
    buildDesignModel();
  }
  function onDragStart(e) {
    state.mouseOffSet.offsetX = e.originalEvent.offsetX;
    state.mouseOffSet.offsetY = e.originalEvent.offsetY;
    state.drag = true;
  }
  function onDragEnd(e) {
    const item = seniorComponents[e.oldIndex];
    if (!item || !item.templateJson) return;
    const x = e.originalEvent.clientX + $('.hiprint-printTemplate').scrollLeft() - $('#hiprintPrintTemplate').offset().left - state.mouseOffSet.offsetX - 15;
    const y = e.originalEvent.clientY + $('.hiprint-printTemplate').scrollTop() - $('#hiprintPrintTemplate').offset().top - state.mouseOffSet.offsetY - 15;
    if (!(window as any).hinnn) return buildInsertedTemplate(x, y, item.templateJson);
    const firstItemX = (window as any).hinnn.px.toPt(x > 0 ? x : 0);
    const firstItemY = (window as any).hinnn.px.toPt(y > 0 ? y : 0);
    buildInsertedTemplate(firstItemX, firstItemY, item.templateJson);
    state.drag = false;
  }
  function buildInsertedTemplate(firstItemX, firstItemY, templateJson) {
    if (!templateJson) return;
    templateRef.value = hiprintTemplate.getJson() || {};
    let minY = Infinity;
    for (let i = 0; i < templateJson.length; i++) {
      const e = templateJson[i];
      if (e.options.top < minY) minY = e.options.top;
    }
    const minYList: any[] = templateJson.filter(o => o.options.top === minY);
    let minX = Infinity;
    let elementWithMinX: any = null;
    for (let i = 0; i < minYList.length; i++) {
      const e = minYList[i];
      if (e.options.left < minX) {
        minX = e.options.left;
        elementWithMinX = e;
      }
    }
    const distanceX = firstItemX - elementWithMinX?.options?.left || 0;
    const distanceY = firstItemY - elementWithMinX?.options?.top || 0;
    for (let i = 0; i < templateJson.length; i++) {
      templateJson[i].options.left += distanceX;
      templateJson[i].options.top += distanceY;
    }
    handleAddComplex(templateJson);
  }
  // 重写hinnn
  function initHinnn() {
    if (!(window as any).hinnn) return;
    (window as any).hinnn.apiUrl = globSetting.apiUrl;
    (window as any).hinnn.dateFormat = function (date, format) {
      if (!date) return '';
      if (!isNaN(date) && typeof date === 'string') date = Number(date);
      format = format.replaceAll('y', 'Y').replaceAll('d', 'D');
      return dayjs(date).format(format);
    };
  }
  function addLocalRecord(fullName: string = '') {
    const copyTempData = JSON.stringify(hiprintTemplate.getJson() || {});
    if (copyTempData != state.copyTempData) {
      state.copyTempData = copyTempData;
      addRecord(copyTempData, fullName);
    }
  }
  // 更新打印设计器模板
  function updateTemp(data) {
    templateRef.value = JSON.parse(data);
    hiprintTemplate.update(JSON.parse(data));
    $('.hiprint-printPaper').click();
    setCurPaper();
    state.isAuto = true;
    setPresetPaper(state.curPaper);
  }
  // 快捷键监听
  function handleListen() {
    // 取消绑定事件处理器
    $(document).off('keydown');

    onKeyStroke(true, (e: any) => {
      if ('INPUT' == e?.target?.tagName) return;
      // ctrl/command + z 撤销 / ctrl/command + shift + z 重做
      if ((e.ctrlKey || e.metaKey) && 90 == e.keyCode) {
        e.shiftKey ? handleRedo(updateTemp) : handleUndo(updateTemp);
        e.preventDefault();
      }
    });
  }
  function init() {
    initHinnn();
    // 初始化 provider
    hiprint.init({ providers: [provider()] });
    // 还原配置
    hiprint.setConfig();
    // 替换配置
    hiprint.setConfig(defaultPanelConfig);

    state.drag = false;
    try {
      templateRef.value = props?.conf ? JSON.parse(props.conf) : null;
    } catch (error) {
      templateRef.value = null;
    }
    state.dataSetList = (props.dataSetList as any[]).map(o => ({
      ...o,
      ceriId: o.id,
      children: !o.children ? [] : o.children.map(c => ({ ...c, ceriId: o.fullName + '.0.' + c.id })),
    }));
    state.convertConfig = props.convertConfig;

    const templateKey = getCurrentInstance()?.type?.name; // 存储模板对象的 key
    if (templateKey === undefined) return;
    state.templateKey = templateKey;
    buildDragElement();
    buildDesignModel(true);
  }

  onMounted(() => {
    initRedo();
    init();
    handleListen();
  });

  /**
   * 暴露方法
   */
  defineExpose({ getData });
</script>

<style lang="less">
  @import './style/index.less';
</style>
