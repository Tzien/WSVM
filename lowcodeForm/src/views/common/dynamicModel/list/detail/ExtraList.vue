<!-- <template>
  <div class="ceri-content-wrapper">
    <div class="ceri-content-wrapper-center">
      <div class="ceri-content-wrapper-search-box" v-if="getSearchList?.length">
        <BasicForm
          @register="registerSearchForm"
          :schemas="getSearchList"
          @advanced-change="redoHeight"
          @submit="handleSearchSubmit"
          @reset="handleSearchReset"
          class="search-form">
        </BasicForm>
      </div>
      <div class="ceri-content-wrapper-content bg-white">
        <BasicTable @register="registerTable" v-bind="getTableBindValue" ref="tableRef" @columns-change="handleColumnChange">
          <template #tableTitle>
            <a-button
              v-for="item in state.headerBtnsList"
              :key="item.value"
              :type="item.value === 'add' ? 'primary' : 'link'"
              :preIcon="item.icon"
              @click="headBtnsHandle(item.value)">
              {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
            </a-button>
          </template>
          <template #expandedRowRender="{ record }" v-if="[1, 2].includes(columnData.type) && getChildTableStyle === 2 && childColumnList.length">
            <a-tabs size="small">
              <a-tab-pane :key="cIndex" :tab="child.label" :label="child.label" v-for="(child, cIndex) in childColumnList">
                <BasicTable @register="registerChildTable" :ellipsis="!!columnData.showOverflow" :data-source="record[child.prop]" :columns="child.children">
                  <template #bodyCell="{ column = {}, record: childRecord }">
                    <template v-if="column.ceriKey === 'relationForm'">
                      <p class="link-text" @click="toDetail(column.modelId, childRecord[`${column.dataIndex}_id`], childRecord[column.propsValue])">
                        {{ childRecord[column.dataIndex] }}
                      </p>
                    </template>
                    <template v-if="column.ceriKey === 'inputNumber'">
                      <ceri-input-number
                        v-model:value="childRecord[column.dataIndex]"
                        :precision="column.precision"
                        :thousands="column.thousands"
                        disabled
                        detailed />
                    </template>
                    <template v-if="column.ceriKey === 'calculate'">
                      <ceri-calculate
                        v-model:value="childRecord[column.dataIndex]"
                        :isStorage="column.isStorage"
                        :precision="column.precision"
                        :thousands="column.thousands"
                        :roundType="column.roundType"
                        detailed />
                    </template>
                    <template v-if="column.ceriKey === 'sign'">
                      <ceri-sign v-model:value="childRecord[column.dataIndex]" detailed />
                    </template>
                    <template v-if="column.ceriKey === 'signature'">
                      <ceri-signature v-model:value="childRecord[column.dataIndex]" detailed />
                    </template>
                    <template v-if="column.ceriKey === 'rate'">
                      <ceri-rate v-model:value="childRecord[column.dataIndex]" :count="column.count" :allowHalf="column.allowHalf" disabled />
                    </template>
                    <template v-if="column.ceriKey === 'slider'">
                      <ceri-slider v-model:value="childRecord[column.dataIndex]" :min="column.min" :max="column.max" :step="column.step" disabled />
                    </template>
                    <template v-if="column.ceriKey === 'uploadImg'">
                      <ceri-upload-img v-model:value="childRecord[column.dataIndex]" disabled detailed simple v-if="childRecord[column.dataIndex]?.length" />
                    </template>
                    <template v-if="column.ceriKey === 'uploadFile'">
                      <ceri-upload-file v-model:value="childRecord[column.dataIndex]" disabled detailed simple v-if="childRecord[column.dataIndex]?.length" />
                    </template>
                    <template v-if="column.ceriKey === 'input'">
                      <ceri-input
                        v-model:value="childRecord[column.dataIndex]"
                        :useMask="column.useMask"
                        :maskConfig="column.maskConfig"
                        :showOverflow="columnData.showOverflow"
                        detailed />
                    </template>
                  </template>
                </BasicTable>
              </a-tab-pane>
            </a-tabs>
          </template>
          <template #bodyCell="{ column = {}, record, index }">
            <template v-if="column.flag === 'INDEX'">
              <span>{{ index + 1 }}</span>
            </template>
            <template v-for="(item, index) in childColumnList" v-if="getChildTableStyle !== 2 && childColumnList.length">
              <template v-if="column.id?.includes('-') && item.children && item.children[0] && column.id === item.children[0]?.id">
                <ChildTableColumn
                  :data="record[item.prop]"
                  :head="item.children"
                  @toggleExpand="toggleExpand(record, `${item.prop}Expand`)"
                  @toDetail="toDetail"
                  :expand="record[`${item.prop}Expand`]"
                  :showOverflow="columnData.showOverflow"
                  :key="index" />
              </template>
            </template>
            <template v-if="!(record.top || column.id?.includes('-'))">
              <template v-if="column.ceriKey === 'relationForm'">
                <p class="link-text" @click="toDetail(column.modelId, record[`${column.prop}_id`], column.propsValue)">{{ record[column.prop] }}</p>
              </template>
              <template v-if="column.ceriKey === 'inputNumber'">
                <ceri-input-number v-model:value="record[column.prop]" :precision="column.precision" :thousands="column.thousands" disabled detailed />
              </template>
              <template v-if="column.ceriKey === 'calculate'">
                <ceri-calculate
                  v-model:value="record[column.prop]"
                  :isStorage="column.isStorage"
                  :precision="column.precision"
                  :thousands="column.thousands"
                  :roundType="column.roundType"
                  detailed />
              </template>
              <template v-if="column.ceriKey === 'sign'">
                <ceri-sign v-model:value="record[column.prop]" detailed />
              </template>
              <template v-if="column.ceriKey === 'signature'">
                <ceri-signature v-model:value="record[column.prop]" detailed />
              </template>
              <template v-if="column.ceriKey === 'rate'">
                <ceri-rate v-model:value="record[column.prop]" :count="column.count" :allowHalf="column.allowHalf" disabled />
              </template>
              <template v-if="column.ceriKey === 'slider'">
                <ceri-slider v-model:value="record[column.prop]" :min="column.min" :max="column.max" :step="column.step" disabled />
              </template>
              <template v-if="column.ceriKey === 'uploadImg'">
                <ceri-upload-img v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
              </template>
              <template v-if="column.ceriKey === 'uploadFile'">
                <ceri-upload-file v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
              </template>
              <template v-if="column.ceriKey === 'input'">
                <ceri-input
                  v-model:value="record[column.prop]"
                  :useMask="column.useMask"
                  :maskConfig="column.maskConfig"
                  :showOverflow="columnData.showOverflow"
                  detailed />
              </template>
            </template>
            <template v-if="column.key === 'action' && (!record.top || columnData.type == 5)">
              <TableAction :actions="getTableActions(record, index)" />
            </template>
          </template>
          <template #summary v-if="columnData.showSummary && [1, 2, 4].includes(columnData.type)">
            <a-table-summary fixed>
              <a-table-summary-row>
                <a-table-summary-cell :index="0">{{ t('component.table.summary') }}</a-table-summary-cell>
                <a-table-summary-cell v-for="(item, index) in getColumnSum" :key="index" :index="index + 1" :align="getSummaryCellAlign(index)">
                  {{ item }}
                </a-table-summary-cell>
                <a-table-summary-cell :index="getColumnSum.length + 1"></a-table-summary-cell>
              </a-table-summary-row>
            </a-table-summary>
          </template>
        </BasicTable>
      </div>
    </div>
    <Form ref="formRef" @reload="reload" />
    <Detail ref="detailRef" />
  </div>
</template>
<script lang="ts" setup>
  import { getModelList, batchDelete, getConfigData } from '@/api/onlineDev/visualDev';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  import { ref, reactive, onMounted, toRefs, computed, unref, nextTick } from 'vue';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useUserStore } from '@/store/user';
  import { useBaseStore } from '@/store/base';
  import { BasicForm, useForm } from '@/components/Form';
  import { BasicTable, useTable, TableAction, ActionItem, TableActionType, SorterResult } from '@/components/Table';
  import Form from '../Form.vue';
  import Detail from '../detail/index.vue';
  import ChildTableColumn from '../ChildTableColumn.vue';
  import { getScriptFunc, onlineUtils, thousandsFormat } from '@/utils/ceri';
  import { getSearchFormSchemas } from '@/components/FormGenerator/src/helper/transform';
  import { dyOptionsList } from '@/components/FormGenerator/src/helper/config';
  import { cloneDeep } from 'lodash-es';
  import { usePermission } from '@/hooks/web/usePermission';

  interface State {
    config: any;
    columnData: any;
    formConf: any;
    headerBtnsList: any[];
    columnBtnsList: any[];
    columnOptions: any[];
    columns: any[];
    childColumnList: any[];
    cacheList: any[];
    columnSettingList: any[];
    searchSchemas: any[];
    customRow: any;
    tabActiveKey: any;
    tabList: any[];
  }

  const props = defineProps(['config', 'detailFormData', 'isPreview', 'isDataManage']);
  const emit = defineEmits(['openDetail', 'openForm']);
  const { hasBtnP } = usePermission();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const userStore = useUserStore();
  const baseStore = useBaseStore();
  const formRef = ref<any>(null);
  const tableRef = ref<Nullable<TableActionType>>(null);
  const detailRef = ref<any>(null);
  const searchInfo = reactive({
    modelId: '',
    menuId: '',
    queryJson: '',
    superQueryJson: '',
    extraQueryJson: '',
  });
  const state = reactive<State>({
    config: {},
    columnData: {},
    formConf: {},
    headerBtnsList: [],
    columnBtnsList: [],
    columnOptions: [],
    columns: [],
    childColumnList: [],
    cacheList: [],
    columnSettingList: [],
    searchSchemas: [],
    customRow: null,
    tabActiveKey: '',
    tabList: [],
  });
  const { columnData, childColumnList } = toRefs(state);
  const [registerSearchForm, { updateSchema, submit: searchFormSubmit }] = useForm({
    baseColProps: { span: 6 },
    showActionButtonGroup: true,
    showAdvancedButton: true,
    compact: true,
  });
  const [registerTable, { reload, setLoading, clearSelectedRowKeys, redoHeight }] = useTable({
    api: getModelList,
    immediate: false,
    clickToRowSelect: false,
    tableSetting: { setting: false, redo: !props.isPreview },
    afterFetch: data => {
      state.cacheList = cloneDeep(data);
      return data;
    },
  });
  const [registerChildTable] = useTable({
    pagination: false,
    canResize: false,
    showTableSetting: false,
  });
  defineExpose({ reload });

  const getPagination = computed(() => {
    if ([3, 5].includes(state.columnData.type) || !state.columnData.hasPage) return false;
    return { pageSize: state.columnData.pageSize };
  });
  const getChildTableStyle = computed(() => (state.columnData.type == 3 || state.columnData.type == 5 ? 1 : state.columnData.childTableStyle));
  const getColumns = computed(() => state.columns);
  const getSearchList = computed(() => {
    return cloneDeep(state.searchSchemas).map(o => ({ ...o, show: true }));
  });
  const getRowKey = computed(() => (state.config.webType == 4 && state.columnData.viewKey ? state.columnData.viewKey : 'id'));
  const getTableBindValue = computed(() => {
    let columns = unref(getColumns);
    const defaultSortConfig = (state.columnData.defaultSortConfig || []).map(o => (o.sort === 'desc' ? '-' : '') + o.field);
    const data: any = {
      pagination: unref(getPagination),
      searchInfo: unref(searchInfo),
      defSort: { sidx: defaultSortConfig.join(',') },
      sortFn: (sortInfo: SorterResult | SorterResult[]) => {
        if (Array.isArray(sortInfo)) {
          const sortList = sortInfo.map(o => (o.order === 'descend' ? '-' : '') + o.field);
          return { sidx: sortList.join(',') };
        } else {
          const { field, order } = sortInfo;
          if (field && order) {
            // 排序字段
            return { sidx: (order === 'descend' ? '-' : '') + field };
          } else {
            return {};
          }
        }
      },
      columns,
      clearSelectOnPageChange: true,
      bordered: (unref(getChildTableStyle) != 2 && !!state.childColumnList?.length) || !!state.columnData.complexHeaderList?.length,
      rowKey: unref(getRowKey),
    };
    if (state.columnBtnsList.length) {
      let columnBtnsLen = state.columnBtnsList.length;
      data.actionColumn = {
        width: columnBtnsLen * 50,
        title: t('component.table.action'),
        dataIndex: 'action',
        fixed: 'right',
      };
    }
    if (state.customRow) data.customRow = state.customRow;
    return data;
  });
  const getSummaryColumn = computed(() => {
    let defaultColumns = unref(getColumns);
    // 处理列固定
    if (state.columnSettingList?.length) {
      for (let i = 0; i < defaultColumns.length; i++) {
        inner: for (let j = 0; j < state.columnSettingList.length; j++) {
          if (defaultColumns[i].dataIndex === state.columnSettingList[j].dataIndex) {
            defaultColumns[i].fixed = state.columnSettingList[j].fixed;
            defaultColumns[i].visible = state.columnSettingList[j].visible;
            break inner;
          }
        }
      }
      defaultColumns = defaultColumns.filter(o => o.visible);
    }
    let columns: any[] = [];
    for (let i = 0; i < defaultColumns.length; i++) {
      const e = defaultColumns[i];
      if (e.ceriKey === 'table' || e.ceriKey === 'complexHeader') {
        if (e.children?.length) columns.push(...e.children);
      } else {
        columns.push(e);
      }
      if (e.fixed && e.children?.length) {
        for (let j = 0; j < e.children.length; j++) {
          e.children[j].fixed = e.fixed;
        }
      }
    }
    const leftFixedList = columns.filter(o => o.fixed === 'left');
    const rightFixedList = columns.filter(o => o.fixed === 'right');
    const noFixedList = columns.filter(o => o.fixed !== 'left' && o.fixed !== 'right');
    return [...leftFixedList, ...noFixedList, ...rightFixedList];
  });
  // 列表合计
  const getColumnSum = computed(() => {
    const sums: any[] = [];
    const isSummary = key => state.columnData.summaryField.includes(key);
    const useThousands = key => unref(getSummaryColumn).some(o => o.__vModel__ === key && o.thousands);
    unref(getSummaryColumn).forEach((column, index) => {
      let sumVal = state.cacheList.reduce((sum, d) => sum + getCmpValOfRow(d, column.prop), 0);
      if (!isSummary(column.prop)) sumVal = '';
      sumVal = Number.isNaN(sumVal) ? '' : sumVal;
      const realVal = sumVal && !Number.isInteger(sumVal) ? Number(sumVal).toFixed(2) : sumVal;
      sums[index] = useThousands(column.prop) ? thousandsFormat(realVal) : realVal;
    });
    if ([1, 2].includes(state.columnData.type) && unref(getChildTableStyle) === 2 && state.childColumnList.length) sums.unshift('');
    return sums;
  });
  function getCmpValOfRow(row, key) {
    const isSummary = key => state.columnData.summaryField.includes(key);
    if (!state.columnData.summaryField.length || !isSummary(key)) return 0;
    const target = row[key];
    if (!target) return 0;
    const data = isNaN(target) ? 0 : Number(target);
    return data;
  }
  function getSummaryCellAlign(index) {
    if (!unref(getSummaryColumn).length) return;
    return unref(getSummaryColumn)[index]?.align || 'left';
  }
  function getTableActions(record, index): ActionItem[] {
    const list = state.columnBtnsList.map(o => {
      const item: ActionItem = {
        label: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
        onClick: columnBtnsHandle.bind(null, o.value, record),
      };
      if (o.value === 'remove') item.color = 'error';
      if (state.config.enableFlow) {
        if (o.value === 'edit') item.disabled = ![0, 8, 9].includes(record.flowState);
        if (o.value === 'remove') item.disabled = ![0, 9].includes(record.flowState);
        if (o.value === 'detail') item.disabled = !record.flowState;
      } else {
        if (o?.event?.enableFunc) {
          const parameter = { row: record, rowIndex: index, onlineUtils };
          const func: any = getScriptFunc(o.event.enableFunc);
          item.disabled = (func && !func(parameter)) || false;
        }
      }
      return item;
    });
    return list;
  }
  function addHandle() {
    const data = {
      id: '',
      formConf: transferExtraList(state.formConf),
      modelId: searchInfo.modelId,
      isPreview: props.isPreview,
      useFormPermission: state.columnData.useFormPermission,
      showMoreBtn: ![3, 5].includes(state.columnData.type),
      menuId: searchInfo.menuId,
      allList: state.cacheList,
    };
    emit('openForm', data);
  }
  // 顶部按钮点击事件
  function headBtnsHandle(key) {
    if (key === 'add') return addHandle();
  }
  // 行按钮点击事件
  function columnBtnsHandle(key, record) {
    if (key === 'edit') return updateHandle(record);
    if (key === 'detail') return goDetail(record);
    if (key == 'remove') handleDelete(record.id);
  }
  // 编辑
  function updateHandle(record) {
    const data = {
      id: record.id,
      formConf: state.formConf,
      modelId: searchInfo.modelId,
      isPreview: props.isPreview,
      useFormPermission: state.columnData.useFormPermission,
      showMoreBtn: ![3, 5].includes(state.columnData.type),
      menuId: searchInfo.menuId,
      allList: state.cacheList,
    };
    emit('openForm', data);
  }
  // 查看详情
  function goDetail(record) {
    const data = {
      id: record.id,
      formConf: state.formConf,
      modelId: searchInfo.modelId,
      menuId: searchInfo.menuId,
      useFormPermission: state.columnData.useFormPermission,
      title: state.config.fullName,
      hideExtra: true,
    };
    emit('openDetail', data);
  }
  function handleDelete(id) {
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: t('common.delTip'),
      onOk: () => {
        const query = { ids: [id], flowId: state.config.flowId || '' };
        batchDelete(searchInfo.modelId, query).then(res => {
          createMessage.success(res.msg);
          reload();
        });
      },
    });
  }
  function init() {
    state.config = {
      modelId: searchInfo.modelId,
      isPreview: props.isPreview,
      ...props.config.extraConfig,
    };
    searchInfo.modelId = state.config.id;
    searchInfo.menuId = props.config.targetFormId;
    const obj = {};
    obj[props.config.targetField] = props.detailFormData[props.config.currentField + '_ceriId'];
    searchInfo.extraQueryJson = JSON.stringify(obj) === '{}' ? '' : JSON.stringify(obj);
    if (!state.config.columnData || (state.config.webType != '4' && !state.config.formData)) return;
    state.columnData = JSON.parse(state.config.columnData);
    state.formConf = state.config.formData ? JSON.parse(state.config.formData) : {};
    const columnBtnsList = state.columnData.columnBtnsList || [];
    getHeaderBtnsList(state.columnData.btnsList.filter(o => o.value === 'add') || []);
    getColumnBtnsList(columnBtnsList);
    state.columnOptions = state.columnData.columnOptions || [];
    if (!unref(getPagination)) (searchInfo as any).pageSize = 1000000;
    setLoading(true);
    getSearchSchemas();
    getColumnList();
    if (props.isPreview) return setLoading(false);
    nextTick(() => {
      unref(getSearchList)?.length ? searchFormSubmit() : reload({ page: 1 });
    });
  }
  function getHeaderBtnsList(btnsList) {
    btnsList = btnsList.filter(o => o.show || !Reflect.has(o, 'show'));
    if (props.isPreview || props.isDataManage || !state.columnData.useBtnPermission) return (state.headerBtnsList = btnsList);
    // 过滤权限
    let btns: any[] = [];
    for (let i = 0; i < btnsList.length; i++) {
      if (hasBtnP('btn_' + btnsList[i].value, true, searchInfo.menuId)) btns.push(btnsList[i]);
    }
    state.headerBtnsList = btns;
  }
  function getColumnBtnsList(columnBtnsList) {
    columnBtnsList = columnBtnsList.filter(o => o.show || !Reflect.has(o, 'show'));
    let btns: any[] = [];
    if (props.isPreview || props.isDataManage || !state.columnData.useBtnPermission) {
      btns = columnBtnsList;
    } else {
      // 过滤权限
      const permissionList = userStore.getPermissionList;
      const list = permissionList.filter(o => o.modelId === searchInfo.menuId);
      const perBtnList = list[0] && list[0].button ? list[0].button : [];
      for (let i = 0; i < columnBtnsList.length; i++) {
        inner: for (let j = 0; j < perBtnList.length; j++) {
          if ('btn_' + columnBtnsList[i].value === perBtnList[j].enCode) {
            btns.push(columnBtnsList[i]);
            break inner;
          }
        }
      }
    }
    state.columnBtnsList = btns;
  }
  function getSearchSchemas() {
    const schemas = getSearchFormSchemas(state.columnData.searchList);
    schemas.forEach(cur => {
      const config = cur.__config__;
      if (dyOptionsList.includes(config.ceriKey)) {
        if (config.dataType === 'dictionary' && config.dictionaryType) {
          baseStore.getDicDataSelector(config.dictionaryType).then(res => {
            updateSchema([{ field: cur.field, componentProps: { options: res } }]);
          });
        }
        if (config.dataType === 'dynamic' && config.propsUrl) {
          const query = { paramList: config.templateJson || [] };
          getDataInterfaceRes(config.propsUrl, query).then(res => {
            const data = Array.isArray(res.data) ? res.data : [];
            updateSchema([{ field: cur.field, componentProps: { options: data } }]);
          });
        }
      }
      if ((Array.isArray(cur.value) && cur.value.length) || cur.value || cur.value === 0 || cur.value === false) cur.defaultValue = cur.value;
    });
    state.searchSchemas = schemas;
  }
  // 获取列
  function getColumnList() {
    let columnList: any[] = [];
    if (props.isPreview || props.isDataManage || !state.columnData.useColumnPermission) {
      columnList = state.columnData.columnList;
    } else {
      // 过滤权限
      const permissionList = userStore.getPermissionList;
      const list = permissionList.filter(o => o.modelId === searchInfo.menuId);
      const perColumnList = list[0] && list[0].column ? list[0].column : [];
      for (let i = 0; i < state.columnData.columnList.length; i++) {
        inner: for (let j = 0; j < perColumnList.length; j++) {
          if (state.columnData.columnList[i].prop === perColumnList[j].enCode) {
            columnList.push(state.columnData.columnList[i]);
            break inner;
          }
        }
      }
    }
    let columns = columnList.map(o => ({
      ...o,
      placeholder: state.columnData.type == 4 && o.placeholderI18nCode ? t(o.placeholderI18nCode, o.placeholder) : o.placeholder,
      title: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
      dataIndex: o.prop,
      align: o.align,
      fixed: o.fixed == 'none' ? false : o.fixed,
      sorter: o.sortable ? { multiple: 1 } : o.sortable,
      width: o.width || 100,
    }));
    if (state.columnData.type !== 3 && state.columnData.type !== 5) columns = getComplexColumns(columns);
    state.columns = columns.filter(o => o.prop.indexOf('-') < 0);
  }
  function getComplexColumns(columns) {
    let complexHeaderList: any[] = state.columnData.complexHeaderList || [];
    if (!complexHeaderList.length) return columns;
    let childColumns: any[] = [];
    let firstChildColumns: string[] = [];
    for (let i = 0; i < complexHeaderList.length; i++) {
      const e = complexHeaderList[i];
      e.label = e.fullName;
      e.labelI18nCode = e.fullNameI18nCode;
      e.title = e.fullNameI18nCode ? t(e.fullNameI18nCode, e.fullName) : e.fullName;
      e.align = e.align;
      e.dataIndex = e.id;
      e.prop = e.id;
      e.children = [];
      e.ceriKey = 'complexHeader';
      if (e.childColumns?.length) {
        childColumns.push(...e.childColumns);
        for (let k = 0; k < e.childColumns.length; k++) {
          const item = e.childColumns[k];
          for (let j = 0; j < columns.length; j++) {
            const o = columns[j];
            if (o.prop == item && o.fixed !== 'left' && o.fixed !== 'right') e.children.push({ ...o });
          }
        }
      }
      if (e.children.length) firstChildColumns.push(e.children[0].prop);
    }
    complexHeaderList = complexHeaderList.filter(o => o.children.length);
    let list: any[] = [];
    for (let i = 0; i < columns.length; i++) {
      const e = columns[i];
      if (!childColumns.includes(e.prop) || e.fixed === 'left' || e.fixed === 'right') {
        list.push(e);
      } else {
        if (firstChildColumns.includes(e.prop)) {
          const item = complexHeaderList.find(o => o.childColumns.includes(e.prop));
          list.push(item);
        }
      }
    }
    return list;
  }
  function toggleExpand(row, field) {
    row[field] = !row[field];
  }
  // 关联表单查看详情
  function toDetail(modelId, id, propsValue) {
    if (!id) return;
    getConfigData(modelId).then(res => {
      if (!res.data || !res.data.formData) return;
      const formConf = JSON.parse(res.data.formData);
      formConf.popupType = 'general';
      formConf.customBtns = [];
      formConf.hasPrintBtn = false;
      const data = { id, formConf, modelId, propsValue };
      detailRef.value?.init(data);
    });
  }
  function handleColumnChange(data) {
    state.columnSettingList = data;
  }
  function handleSearchSubmit(data) {
    if (props.isPreview) return;
    clearSelectedRowKeys();
    let obj = {};
    for (let [key, value] of Object.entries(data)) {
      if (value || value === 0 || value === false) {
        if (Array.isArray(value)) {
          if (value.length) obj[key] = value;
        } else {
          obj[key] = value;
        }
      }
    }
    searchInfo.queryJson = JSON.stringify(obj) === '{}' ? '' : JSON.stringify(obj);
    reload({ page: 1 });
  }
  function handleSearchReset() {
    searchFormSubmit();
  }
  function transferExtraList(formConf) {
    if (!props.config?.formOptions?.length) return formConf;
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        let item = list[i];
        if (item?.__config__?.children && Array.isArray(item.__config__.children)) {
          loop(item.__config__.children);
        }
        if (item.__vModel__) {
          for (let j = 0; j < props.config?.formOptions.length; j++) {
            const element = props.config?.formOptions[j];
            if (element.targetField == item.__vModel__) {
              item.__config__.defaultValue = props.detailFormData[element.currentField + '_ceriId'];
              item.__config__.defaultCurrent = false;
            }
          }
        }
      }
    };
    loop(formConf.fields);
    return formConf;
  }

  onMounted(() => init());
</script> -->

<template></template>