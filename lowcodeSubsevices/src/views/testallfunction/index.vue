<template>
  <div class="ceri-content-wrapper demos-page">
    <div class="ceri-content-wrapper-center">
      <div class="ceri-content-wrapper-search-box" v-if="getSearchList.length">
        <BasicForm @register="registerSearchForm" :schemas="getSearchList" @advanced-change="redoHeight" @submit="handleSearchSubmit" class="search-form">
        </BasicForm>
        <div class="demos-search-actions">
          <a-button type="primary" @click="handleClickSearch()">{{ t('common.queryText', '查询') }}</a-button>
          <a-button class="ml-10" @click="handleSearchReset()">{{ t('common.resetText', '重置') }}</a-button>
        </div>
      </div>
      <div class="ceri-content-wrapper-content bg-white">
        <BasicTable @register="registerTable" v-bind="getTableBindValue" ref="tableRef" @columns-change="handleColumnChange">
          <template #tableTitle>
            <a-button type="primary" preIcon="icon-ym icon-ym-btn-add" @click="addHandle()">{{t('common.add2Text','新增')}}</a-button>
          </template>
          <template #toolbarAfter>
            <ViewList :userId="userStore.userid" :menuId="searchInfo.menuId" :viewList="viewList" @itemClick="handleViewClick" @reload="initViewList" />
            <ViewSetting :menuId="searchInfo.menuId" :viewList="viewList" :currentView="currentView" @reload="initViewList" />
          </template>
          <template #bodyCell="{ column, record }">
<template v-if="!(record.top || column.id?.includes('-'))">
            <template v-if="column.ceriKey === 'sign'">
                <ceri-sign v-model:value="record[column.dataIndex]" detailed />
            </template>
            <template v-if="column.ceriKey === 'signature'">
                <ceri-signature v-model:value="record[column.dataIndex]" detailed />
            </template>
            <template v-if="column.ceriKey === 'rate'">
                <ceri-rate v-model:value="record[column.dataIndex]" :count="column.count" :allowHalf="column.allowHalf" disabled />
            </template>
            <template v-if="column.ceriKey === 'slider'">
                <ceri-slider v-model:value="record[column.dataIndex]" :min="column.min" :max="column.max" :step="column.step" disabled />
            </template>
            <template v-if="column.ceriKey === 'uploadImg'">
                <ceri-upload-img v-model:value="record[column.dataIndex]" disabled detailed simple v-if="record[column.dataIndex]?.length" />
            </template>
            <template v-if="column.ceriKey === 'uploadFile'">
                <ceri-upload-file v-model:value="record[column.dataIndex]" disabled detailed simple v-if="record[column.dataIndex]?.length" />
            </template>
            <template v-if="column.ceriKey === 'input'">
                <ceri-input
                v-model:value="record[column.dataIndex]"
                :useMask="column.useMask"
                :maskConfig="column.maskConfig"
                :showOverflow="true"
                detailed />
            </template>
            <template v-if="column.ceriKey === 'datePicker'">
                {{ formatDateCell(record[column.dataIndex], column.format || 'YYYY-MM-DD HH:mm:ss') }}
            </template>
            <template v-if="['select', 'radio', 'checkbox'].includes(column.ceriKey)">
                {{ formatOptionCell(record[column.dataIndex], column) }}
            </template>
            <template v-if="column.ceriKey === 'inputNumber'">
              <ceri-input-number v-model:value="record[column.prop]" :precision="column.precision" :thousands="column.thousands" disabled detailed />
            </template>
</template>
            <template v-if="column.flag === 'ACTION' && !record.top">
              <TableAction :actions="getTableActions(record)" />
            </template>
          </template>
        </BasicTable>
      </div>
    </div>
    <Form ref="formRef" @reload="reload" />
  </div>
</template>

<script lang="ts" setup>
  import { getList, del, exportData, batchDelete } from './helper/api';
  import { getViewList } from '@/api/onlineDev/visualDev';
  import { getDictionaryDataSelector } from '@/api/systemData/dictionary';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  //import { getOrgByOrganizeCondition, getDepartmentSelectAsyncList } from '@/api/permission/organize';
  import { ref, reactive, onMounted, toRefs, computed, unref, nextTick, provide } from 'vue';
  import dayjs from 'dayjs'
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n'
  //import { useOrganizeStore } from '@/store/modules/organize';
  import { useUserStore } from '@/store/user';
  import { useModal } from '@/components/Modal';
  import { BasicForm, useForm } from '@/components/Form';
  import { BasicTable, useTable, TableAction, ActionItem, TableActionType, SorterResult } from '@/components/Table';
  import Form from './Form.vue';
  import { ExportModal, ImportModal, SuperQueryModal } from '@/components/CommonModal';
  import { downloadByUrl } from '@/utils/file/download';
  import { useRoute,useRouter } from 'vue-router';
  //import { useTabs } from '@/hooks/web/useTabs';
  //import { getFlowStartFormId } from '@/api/workFlow/template';
  import { FilterOutlined } from '@ant-design/icons-vue';
  import { getSearchFormSchemas } from '@/components/FormGenerator/src/helper/transform';
  import { cloneDeep } from 'lodash-es';
  import columnList from './helper/columnList';
  import searchList from './helper/searchList';
  import { dyOptionsList } from '@/components/FormGenerator/src/helper/config';
  import { thousandsFormat,getParamList, getDateFormat } from '@/utils/ceri';
  //import { usePermission } from '@/hooks/web/usePermission';
  import { noGroupList } from '@/components/FormGenerator/src/helper/config';
  import ViewSetting from '@/views/common/dynamicModel/list/components/ViewSetting.vue';
  import ViewList from '@/views/common/dynamicModel/list/components/ViewList.vue';


    const __searchTypes = (() => {
    const map: Record<string, number> = {};
    (searchList || []).forEach((it: any) => {
      const key = it?.prop;
      const type = it?.searchType;
      if (key && typeof type === 'number') map[key] = type;
    });
    return map;
  })();
  function formatDateCell(val: any, format: string) {
    if (!val && val !== 0) return '';
    const dateValue = !isNaN(Number(val)) ? Number(val) : val;
    return dayjs(dateValue).format(getDateFormat(format));
  }
  function getColumnFieldKey(key: string) {
    const matchedKey = (state.columnList || []).map((item) => item?.prop).find((item) => item && item.toLowerCase() === key.toLowerCase());
    return matchedKey || key.replace(/^[a-z]/, (s) => s.toUpperCase());
  }
  function formatOptionCell(value: any, column: any) {
    if (value === undefined || value === null || value === '') return '';
    const cellValue = normalizeOptionCellValue(value, column);
    const options = Array.isArray(column?.options) ? column.options : [];
    if (!options.length) return Array.isArray(cellValue) ? cellValue.join(',') : cellValue;
    const props = column?.props || {};
    const labelKey = props.label || 'fullName';
    const valueKey = props.value || 'id';
    const getLabel = (val: any) => {
      const item = options.find((option: any) => String(option?.[valueKey]) === String(val));
      return item?.[labelKey] ?? val;
    };
    return Array.isArray(cellValue) ? cellValue.map(getLabel).join(',') : getLabel(cellValue);
  }
  function normalizeOptionCellValue(value: any, column: any) {
    if (Array.isArray(value) || typeof value !== 'string') return value;
    const trimmed = value.trim();
    if (!trimmed) return '';
    if (trimmed.startsWith('[') && trimmed.endsWith(']')) {
      try {
        const parsed = JSON.parse(trimmed);
        return Array.isArray(parsed) ? parsed : value;
      } catch {
        return value;
      }
    }
    if (column?.ceriKey === 'checkbox' && trimmed.includes(',')) {
      return trimmed.split(',').map((item: string) => item.trim()).filter(Boolean);
    }
    return value;
  }
  function getRowId(row: any) {
    return row?.id ?? row?.id ?? row?.Id;
  }

  interface State {
    config: any;
    columnList: any[];
    printListOptions: any[];
    columnBtnsList: any[];
    customBtnsList: any[];
    treeActiveId: string;
    treeActiveNodePath: any;
    columns: any[];
    complexColumns: any[];
    childColumnList: any[];
    exportList: any[];
    cacheList: any[];
    currFlow: any;
    isCustomCopy: boolean;
    candidateType: number;
    currRow: any;
    workFlowFormData: any;
    expandObj: any;
    columnSettingList: any[];
    searchSchemas: any[];
    treeRelationObj: any;
    treeQueryJson: any;
    leftTreeActiveInfo: any;
    viewList: any[];
    currentView: any;
  }

  const router = useRouter();
  //const { close } = useTabs();
  const route = useRoute();
  //const { hasBtnP } = usePermission();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  //const organizeStore = useOrganizeStore();
  const userStore = useUserStore();
  //const userInfo = userStore.getUserInfo;

  //const [registerExportModal, { openModal: openExportModal, closeModal: closeExportModal, setModalProps: setExportModalProps }] = useModal();
  //const [registerImportModal, { openModal: openImportModal }] = useModal();
  const [registerSuperQueryModal, { openModal: openSuperQuery }] = useModal();
  const formRef = ref<any>(null);
  const tableRef = ref<Nullable<TableActionType>>(null);
  const detailRef = ref<any>(null);
  const state = reactive<State>({
    config: {},
    columnList: [],
    printListOptions: [],
    columnBtnsList: [],
    customBtnsList: [],
    treeActiveId: '',
    treeActiveNodePath: [],
    columns: [],
    complexColumns: [], // 复杂表头
    childColumnList: [],
    exportList: [],
    cacheList: [],
    currFlow: {},
    isCustomCopy: false,
    candidateType: 1,
    currRow: {},
    workFlowFormData: {},
    expandObj: {},
    columnSettingList: [],
    searchSchemas: [],
    treeRelationObj: null,
    treeQueryJson: {},
    leftTreeActiveInfo: {},
    viewList: [],
    currentView: {},
  });
  const { childColumnList, searchSchemas, viewList, currentView  } = toRefs(state);
  const defaultSearchInfo = {
    menuId:  route.path as string,
    //moduleId:'67ee5d72-247c-4e7d-b283-5216923544de',
    superQueryJson: '',
  };
  const searchInfo:any = reactive({
    ...cloneDeep(defaultSearchInfo),
  });
  const [registerSearchForm, { updateSchema, resetFields, submit: searchFormSubmit,getFieldsValue }] = useForm({
    baseColProps: { span: 6 },
    showActionButtonGroup: false,
    showAdvancedButton: false,
    compact: true,
  });
  const [registerChildTable] = useTable({
    pagination: false,
    canResize: false,
    showTableSetting: false,
  });  
  const [registerTable, { reload, setLoading, getFetchParams,getSelectRows, getSelectRowKeys, redoHeight, clearSelectedRowKeys }] = useTable({
    api: getList,
    immediate: false,
    clickToRowSelect: false,
    tableSetting: { setting: false },
    afterFetch: (data) => {
      const rows = Array.isArray(data) ? data : data?.list || [];
      const mapRow = (row: any) => {
        const mapped: any = {
          ...row,
          ...state.expandObj,
        };
        Object.keys(row || {}).forEach((key) => {
          const fieldKey = getColumnFieldKey(key);
          if (!(fieldKey in mapped)) mapped[fieldKey] = row[key];
        });
        const rowId = getRowId(row);
        if (rowId !== undefined && rowId !== null) {
          mapped.id = rowId;
          mapped.id = rowId;
          mapped.Id = rowId;
        }
        return mapped;
      };
      const list = rows.map((o) => mapRow(o));
      state.cacheList = cloneDeep(list);
      return list;
    },
  });

  provide('getLeftTreeActiveInfo', () => state.leftTreeActiveInfo);

  const getColumns = computed(() => {
    const columns: any[] = state.complexColumns;
    if (!state.currentView || !Array.isArray(state.currentView.columnList) || !state.currentView.columnList.length) {
      return columns;
    }
    return setListValue(state.currentView?.columnList, columns, 'prop');
  });
  const getSearchList = computed(() => {
    const searchSchemas = cloneDeep(state.searchSchemas).map(o => ({ ...o, show: true }));
    if (!state.currentView || !Array.isArray(state.currentView.searchList) || !state.currentView.searchList.length) {
      return searchSchemas;
    }
    return setListValue(state.currentView?.searchList, searchSchemas, 'field');
  });
  const getHasBatchBtn = computed(() => {
    let btnsList:any[] =[]
    return !!btnsList.length
  });
  const getTableBindValue = computed(() => {
    let columns = unref(getColumns);
    const defaultSortConfig = []; 
    const sortField = defaultSortConfig.map(o => (o.sort === 'desc' ? '-' : '') + o.field); 
    const data: any = {
      pagination: {
        pageSize: 20,
        pageSizeOptions: ['20', '30', '45', '60'],
        showSizeChanger: true,
        showTotal: (total) => `${t('message.drawer.TotalOf')} ${total} ${t('message.drawer.Items')}`,
        size: 'default',
      }, //有分页
        maxHeight: 585,
        striped: true,
        // 为本页表格预留一点高度，避免不同布局密度下表格刚好顶到容器底部导致分页被裁掉
        // 调小为 15，让紧凑布局下分页更贴近底部
        resizeHeightOffset: 20,
      searchInfo: unref(searchInfo),
      ellipsis: true,
      defSort: { sidx: sortField.join(',') },
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
      bordered: true,
      actionColumn: {
        width: 120,
        fixed: 'right',
        align: 'center',
        title: t('component.table.action'),
        dataIndex: 'action',
        key: 'action',
        flag: 'ACTION',
      },
    };
    if (unref(getHasBatchBtn)) {
      const rowSelection: any = { type: 'checkbox' };
      data.rowSelection = rowSelection;
    }
    return data;
  });
function getTableActions(record): ActionItem[] {
  return [
    {
      label: t('common.editText','编辑'),
      onClick: updateHandle.bind(null, record),
    },
    {
      label: t('common.delText','删除'),
      color: 'error',
      modelConfirm: {
        onOk: handleDelete.bind(null, getRowId(record)),
      },
    },
  ];
}
// 新增
function addHandle() {
  const data = {
    id: '',
    menuId: searchInfo.menuId,
    allList: state.cacheList,
  };
  formRef.value?.init(data);
}
  // 编辑
  function updateHandle(record) {
    // 不带工作流
    const data = {
      id: getRowId(record),
      menuId: searchInfo.menuId,
      allList: state.cacheList,
    };
    formRef.value?.init(data);
  }
  function handleDelete(id) {
      const query = [id];
      batchDelete(query).then(res => {
      createMessage.success(res.msg);
      clearSelectedRowKeys();
      reload();
    });
  }
  function init() {
    state.config = {};
    searchInfo.menuId =  route.path as string;

    state.columnList = columnList;
    getSearchSchemas();

    setLoading(true);
    getColumnList();
    initViewList();
        nextTick(() => {
          unref(getSearchList).length ? searchFormSubmit() : reload({ page: 1 });
        });
  }
  function getSearchSchemas() {
    const schemas = getSearchFormSchemas(searchList);
    state.searchSchemas = schemas;
    schemas.forEach((cur) => {
      const config = cur.__config__;
      if (dyOptionsList.includes(config.ceriKey)) {
        if (config.dataType === 'dictionary') {
          if (!config.dictionaryType) return;
          getDictionaryDataSelector(config.dictionaryType).then((res) => {
            updateSchema([{ field: cur.field, componentProps: { options: res.data.list } }]);
          });
        }
        if (config.dataType === 'dynamic') {
          if (!config.propsUrl) return;
          const query = { paramList: getParamList(config.templateJson) || [] };
          getDataInterfaceRes(config.propsUrl, query).then((res) => {
            const data = Array.isArray(res.data) ? res.data : [];
            updateSchema([{ field: cur.field, componentProps: { options: data } }]);
          });
        }
      }
      cur.defaultValue = cur.value;
    });
  }
  function getColumnList() {
    // 没有权限
    let  columnList = state.columnList;

  state.exportList = columnList.filter(o => !noGroupList.includes(o.__config__.ceriKey));
  let columns = columnList.map((o) => ({
    ...o,
    title: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
    dataIndex: o.prop,
    align: o.align,
    fixed: o.fixed == 'none' ? false : o.fixed,
    sorter: o.sortable ? { multiple: 1 } : o.sortable,
    width: o.width || 100,
  }));
    columns = getComplexColumns(columns);
  state.columns = columns.filter((o) => o.prop.indexOf('-') < 0);
  getChildComplexColumns(columns);
}
  function getComplexColumns(columns) {
    let complexHeaderList: any[] = [];
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
       if (!childColumns.includes(e.prop)) {
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
  function getChildComplexColumns(columnList) {
    let list: any[] = [];
    for (let i = 0; i < columnList.length; i++) {
      const e = columnList[i];
      if (!e.prop.includes('-')) {
        list.push(e);
      } else {
        let prop = e.prop.split('-')[0];
        let vModel = e.prop.split('-')[1];
        let label = e.label.split('-')[0];
        let childLabel = e.label.replace(label + '-', '');
        if (e.fullNameI18nCode && Array.isArray(e.fullNameI18nCode) && e.fullNameI18nCode[0]) label = t(e.fullNameI18nCode[0], label);
        let newItem = {
          align: 'center',
          ceriKey: 'table',
          prop,
          label,
          title: label,
          dataIndex: prop,
          children: [],
        };
        e.dataIndex = vModel;
        e.title = e.labelI18nCode ? t(e.labelI18nCode, childLabel) : childLabel;
        if (!state.expandObj.hasOwnProperty(`${prop}Expand`)) state.expandObj[`${prop}Expand`] = false;
        if (!list.some((o) => o.prop === prop)) list.push(newItem);
        for (let i = 0; i < list.length; i++) {
          if (list[i].prop === prop) {
            list[i].children.push(e);
            break;
          }
        }
      }
    }
    // 行内分组展示
    getMergeList(list);
    state.complexColumns = list;
    state.childColumnList = list.filter((o) => o.ceriKey === 'table');
    // 子表分组展示宽度取100
    for (let i = 0; i < state.childColumnList.length; i++) {
      const e = state.childColumnList[i];
      if (e.children?.length) e.children = e.children.map(o => ({ ...o, width: 100 }));
    }
  }
  function getMergeList(list) {
    list.forEach((item) => {
      if (item.ceriKey === 'table' && item.children && item.children.length) {
        item.children.forEach((child, index) => {
          if (index == 0) {
            child.customCell = () => ({
              rowspan: 1,
              colspan: item.children.length,
              class: 'child-table-box',
            });
          } else {
            child.customCell = () => ({
              rowspan: 0,
              colspan: 0,
            });
          }
        });
      }
    });
  }
  function handleColumnChange(data) {
    state.columnSettingList = data;
  }
    function handleClickSearch() {
        const data = getFieldsValue()
        handleSearchSubmit(data)
    }
    async function handleSearchReset() {
        await resetFields()
        const data = getFieldsValue()
        handleSearchSubmit(data)
    }
  function handleSearchSubmit(data) {
    clearSelectedRowKeys();
    let obj = {
      ...defaultSearchInfo,
      superQueryJson: searchInfo.superQueryJson,
      ...data,
      ...(state.treeQueryJson || {}),
      __searchTypes,
      
    };
    Object.keys(searchInfo).map(key => {
      delete searchInfo[key];
    });
    for (let [key, value] of Object.entries(obj)) {
      searchInfo[key.replaceAll('-', '_')] = value;
    }
    reload({ page: 1 });
  }
function initViewList(currentId = '') {
    const query = {
      menuId: searchInfo.menuId,
      userId: userStore.userid
    };
    getViewList(query).then(res => {
      const columns: any[] = state.complexColumns;
      const searchList: any[] = state.searchSchemas.map(o => ({ label: o.label, id: o.field, show: o.show }));
      const columnList: any[] = columns.map(o => ({ label: o.label, id: o.prop, show: true, fixed: o.fixed || 'none', labelI18nCode: o.labelI18nCode }));
      state.viewList = (res.data || []).map(o => {
        if (o.type == 0) return { ...o, searchList, columnList };
        return { ...o, searchList: o.searchList ? JSON.parse(o.searchList) : [], columnList: o.columnList ? JSON.parse(o.columnList) : [] };
      });
      if (currentId) {
        state.currentView = state.viewList.filter(o => o.id === currentId)[0] || state.viewList[0];
      } else {
        state.currentView = state.viewList.filter(o => o.status === 1)[0] || state.viewList[0];
      }
    });
  }
  function handleViewClick(item) {
    state.currentView = item;
  }
  function setListValue(data: any[] = [], defaultData: any[] = [], key) {
    let list: any[] = [];
    for (let i = 0; i < data.length; i++) {
      for (let j = 0; j < defaultData.length; j++) {
        if (data[i].show && data[i].id == defaultData[j][key]) list.push(defaultData[j]);
      }
    }
    return list;
  }
  onMounted(() => {
    init();
  });
</script>

<style lang="less" scoped>
.demos-page {
  // 让查询/重置按钮定位在搜索区域右上角，与第一行表单项同一行
  :deep(.ceri-content-wrapper-search-box) {
    position: relative;
    padding-right: 220px;
  }

  :deep(.ceri-content-wrapper-search-box .ant-form-item-label) {
    text-align: right !important;
    width: 100px !important;
    max-width: 100px !important;
    flex: 0 0 100px !important;
    box-sizing: border-box;
  }

  :deep(.ceri-content-wrapper-search-box .ant-form-item-control) {
    flex: 1 1 auto;
    max-width: calc(100% - 100px);
  }

  :deep(.ceri-content-wrapper-search-box .ant-form-item-label > label) {
    width: 100%;
    display: flex;
    justify-content: flex-end;
  }

  .demos-search-actions {
    position: absolute;
    top: 10px;
    right: 16px;
    display: flex;
    align-items: center;
    flex-wrap: nowrap;
    z-index: 2;
  }

  // 当前页面表格列居中显示（表头和表体）
  :deep(.ant-table-thead > tr > th),
  :deep(.ant-table-tbody > tr > td) {
    text-align: center;
  }

  // 调整当前页表格底部分页的视觉样式，尽量贴近 WebDesign 页的 .paginationStyle
  :deep(.ant-table-pagination.ant-pagination) {
    margin-top: 5px;
    text-align: right;
    height: 50px;
    line-height: 50px;
    font-size: 14px; // 与默认分页字号一致，避免看起来比 WebDesign 页明显小
    padding: 0 30px 0 10px;
    display: flex;
    align-items: center;
    justify-content: flex-end;
  }
   :deep(.ant-table-pagination.ant-pagination .ant-pagination-options) {
    display: flex;
    align-items: center;
  }
}
</style>