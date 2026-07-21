<template>
  <div class="ceri-content-wrapper">
    <div class="ceri-content-wrapper-center">
      <div class="ceri-content-wrapper-search-box" v-if="getSearchList.length">
        <BasicForm @register="registerSearchForm" :schemas="searchSchemas" @advanced-change="redoHeight" @submit="handleSearchSubmit" @reset="handleSearchReset" class="search-form" />
      </div>
      <div class="ceri-content-wrapper-content bg-white">
        <BasicTable @register="registerTable" v-bind="getTableBindValue" ref="tableRef" @columns-change="handleColumnChange">
          <template #tableTitle>
            <a-button type="primary" preIcon="icon-ym icon-ym-btn-add" @click="addHandle()">{{t('common.add2Text','新增')}}</a-button>
          </template>
          <template #toolbarAfter>
            <ViewList :menuId="searchInfo.menuId" :viewList="viewList" @itemClick="handleViewClick" @reload="initViewList" />
            <ViewSetting :menuId="searchInfo.menuId" :viewList="viewList" :currentView="currentView" @reload="initViewList" />
          </template>
          <template #bodyCell="{ column, record, index }">
            <template v-if="column.flag === 'INDEX'">
              <div class="edit-row-action">
                <span class="edit-row-index">{{ index + 1 }}</span>
                <i class="ym-custom ym-custom-arrow-expand" @click="handleRowForm(record)"></i>
              </div>
            </template>
            <template v-if="record.rowEdit">
              <template v-if="column.ceriKey === 'inputNumber'">
				<ceri-input-number v-model:value="record[column.prop]" :placeholder="column.placeholder" :min="column.min" :max="column.max" :step="column.step" :controls="column.controls" :addonBefore="column.addonBefore" :addonAfter="column.addonAfter" :precision="column.precision" :thousands="column.thousands" :disabled="column.disabled" />
              </template>
              <template v-else-if="column.ceriKey === 'sign'">
				<ceri-sign v-model:value="record[column.prop]" :disabled="column.disabled" :isInvoke="column.isInvoke" />
              </template>
			  <template v-else-if="column.ceriKey === 'signature'">
				 <ceri-signature v-model:value="record[column.prop]" :disabled="column.disabled" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'location'">
				<ceri-location v-model:value="record[column.prop]" :enableLocationScope='column.enableLocationScope' :autoLocation='column.autoLocation' :adjustmentScope='column.adjustmentScope' :enableDesktopLocation='column.enableDesktopLocation' :locationScope='column.locationScope' :disabled="column.disabled" />
              </template>
				<template v-else-if="column.ceriKey === 'rate'">
                  <ceri-rate v-model:value="record[column.prop]" :count="column.count" :allowHalf="column.allowHalf" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'slider'">
                  <ceri-slider v-model:value="record[column.prop]" :min="column.min" :max="column.max" :step="column.step" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'uploadImg'">
                  <ceri-upload-img
                    v-model:value="record[column.prop]"
                    :fileSize="column.fileSize"
                    :sizeUnit="column.sizeUnit"
                    :limit="column.limit"
                    :pathType="column.pathType"
                    :isAccount="column.isAccount"
                    :folder="column.folder"
                    :tipText="column.tipText"
                    :sortRule="column.sortRule"
                    :timeFormat="column.timeFormat"
                    :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'uploadFile'">
                  <ceri-upload-file
                    v-model:value="record[column.prop]"
                    :accept="column.accept"
                    :fileSize="column.fileSize"
                    :sizeUnit="column.sizeUnit"
                    :buttonText="column.buttonText"
                    :limit="column.limit"
                    :pathType="column.pathType"
                    :isAccount="column.isAccount"
                    :folder="column.folder"
                    :tipText="column.tipText"
                    :sortRule="column.sortRule"
                    :timeFormat="column.timeFormat"
                    :disabled="column.disabled" />
                </template>
              <template v-else-if="column.ceriKey === 'calculate'">
                <ceri-calculate v-model:value="record[column.prop]" :isStorage="column.isStorage" :precision="column.precision" :thousands="column.thousands" detailed />
              </template>
              <template v-else-if="['rate', 'slider'].includes(column.ceriKey)">
				<ceri-input-number v-model:value="record[column.prop]" placeholder="请输入" :disabled="column.disabled" />
              </template>
              <template v-else-if="column.ceriKey === 'switch'">
                <ceri-switch v-model:value="record[column.prop]" :disabled="column.disabled" />
              </template>
              <template v-else-if="column.ceriKey === 'timePicker'">
				<ceri-time-picker v-model:value="record[column.prop]" :format="column.format" :placeholder="column.placeholder" :allowClear="column.clearable" :startTime="column.startTime" :endTime="column.endTime" :disabled="column.disabled" />
              </template>
              <template v-else-if="column.ceriKey === 'datePicker'">
                <ceri-date-picker v-model:value="record[column.prop]" :type="column.type" :allowClear="column.clearable" :placeholder="column.placeholder" :startTime="column.startTime" :endTime="column.endTime" :format="column.format" :disabled="column.disabled" />
			  </template>
              <template v-else-if="column.ceriKey === 'organizeSelect'">
                <ceri-organize-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'depSelect'">
                <ceri-dep-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'roleSelect'">
                <ceri-role-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'groupSelect'">
                <ceri-group-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'posSelect'">
                <ceri-pos-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'userSelect'">
                <ceri-user-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="['all', 'custom'].includes(column.selectType) ? column.selectType : 'all'" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'usersSelect'">
                <ceri-users-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :selectType="column.selectType" :ableIds="column.ableIds" />
			  </template>
              <template v-else-if="column.ceriKey === 'areaSelect'">
                <ceri-area-select v-model:value="record[column.prop]" :level="column.level" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" />
			  </template>
              <template v-else-if="['select', 'radio', 'checkbox'].includes(column.ceriKey)">
                <ceri-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple || column.ceriKey === 'checkbox'" :allowClear="column.clearable || ['radio', 'checkbox'].includes(column.ceriKey)" :showSearch="column.filterable" :disabled="column.disabled" :options="column.options" :fieldNames="column.props" />
			  </template>
              <template v-else-if="column.ceriKey === 'cascader'">
                <ceri-cascader v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :showSearch="column.filterable" :disabled="column.disabled" :options="column.options" :fieldNames="column.props" :showAllLevels="column.showAllLevels" />
			  </template>
              <template v-else-if="column.ceriKey === 'treeSelect'">
                <ceri-tree-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :showSearch="column.filterable" :disabled="column.disabled" :options="column.options" :fieldNames="column.props" />
			  </template>
              <template v-else-if="column.ceriKey === 'relationForm'">
                <ceri-relation-form v-model:value="record[column.prop]" :placeholder="column.placeholder" :allowClear="column.clearable" :disabled="column.disabled" :modelId="column.modelId" :columnOptions="column.columnOptions" :relationField="column.relationField" :hasPage="column.hasPage" :pageSize="column.pageSize" :popupType="column.popupType" :popupTitle="column.popupTitle" :popupWidth="column.popupWidth" :queryType="column.queryType" :propsValue="column.propsValue" />
			  </template>
              <template v-else-if="column.ceriKey === 'popupSelect' || column.ceriKey === 'popupTableSelect'">
				<ceri-popup-select v-model:value="record[column.prop]" :placeholder="column.placeholder" :multiple="column.multiple" :allowClear="column.clearable" :disabled="column.disabled" :interfaceId="column.interfaceId" :columnOptions="column.columnOptions" :propsValue="column.propsValue" :relationField="column.relationField" :hasPage="column.hasPage" :pageSize="column.pageSize" :popupType="column.popupType" :popupTitle="column.popupTitle" :templateJson="column.templateJson" :popupWidth="column.popupWidth" />
              </template>
              <template v-else-if="['input', 'textarea'].includes(column.ceriKey)">
                <ceri-input v-model:value="record[column.prop]" :placeholder="column.placeholder" :allowClear="column.clearable" :disabled="column.disabled" :readonly="column.readonly" :prefixIcon="column.prefixIcon" :suffixIcon="column.suffixIcon" :addonBefore="column.addonBefore" :addonAfter="column.addonAfter" :maxlength="column.maxlength" :showPassword="column.showPassword" />
              </template>
              <template v-else-if="column.ceriKey === 'autoComplete'">
				<ceri-auto-complete v-model:value="record[column.prop]" :placeholder="column.placeholder" :allowClear="column.clearable" :disabled="column.disabled" :interfaceId="column.interfaceId" :relationField="column.relationField" :templateJson="column.templateJson" :total="column.total" />
              </template>
              <template v-else-if="systemComponentsList.includes(column.ceriKey)">
                {{ record[column.prop + '_name'] || record[column.prop] }}
              </template>
              <template v-else>
                {{ record[column.prop] }}
              </template>
            </template>
            <template v-else>
              <template v-if="column.ceriKey === 'inputNumber'">
                <ceri-input-number v-model:value="record[column.prop]" :precision="column.precision" :thousands="column.thousands" disabled detailed />
              </template>
              <template v-else-if="column.ceriKey === 'sign'">
                <ceri-sign v-model:value="record[column.prop]" detailed />
              </template>
				<template v-else-if="column.ceriKey === 'signature'">
					<ceri-signature v-model:value="record[column.prop]" detailed />
				</template>
				<template v-else-if="column.ceriKey === 'rate'">
                  <ceri-rate v-model:value="record[column.prop]" :count="column.count" :allowHalf="column.allowHalf" disabled />
                </template>
                <template v-else-if="column.ceriKey === 'slider'">
                  <ceri-slider v-model:value="record[column.prop]" :min="column.min" :max="column.max" :step="column.step" disabled />
                </template>
                <template v-else-if="column.ceriKey === 'uploadImg'">
                  <ceri-upload-img v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-else-if="column.ceriKey === 'uploadFile'">
                  <ceri-upload-file v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-else-if="column.ceriKey === 'input'">
                  <ceri-input
                    v-model:value="record[column.prop]"
                    :useMask="column.useMask"
                    :maskConfig="column.maskConfig"
                    :showOverflow="true"
                    detailed />
                </template>
              <template v-else-if="column.ceriKey === 'calculate'">
                <ceri-calculate v-model:value="record[column.prop]" :isStorage="column.isStorage" :precision="column.precision" :thousands="column.thousands" detailed />
              </template>
              <template v-else-if="column.ceriKey === 'relationForm'">
                <p class="link-text" @click="toDetail(column.modelId, record[`${column.prop}_id`])">{{ record[column.prop + '_name'] || record[column.prop] }}</p>
              </template>
              <template v-else>
                {{ record[column.prop + '_name'] || record[column.prop] }}
              </template>
            </template>
            <template v-if="column.key === 'action' && !record.top">
              <TableAction :actions="getTableActions(record, index)" />
            </template>
          </template>
        </BasicTable>
      </div>
    </div>
    <Form ref="formRef" @reload="reload" />
  </div>
</template>

<script lang="ts" setup>
	import { getList,  exportData, batchDelete, create, update } from './helper/api';
	import { getViewList } from '@/api/onlineDev/visualDev';
	import { getDictionaryDataSelector } from '@/api/systemData/dictionary';
	import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
	import { ref, reactive, onMounted, toRefs, computed, unref, nextTick, toRaw } from 'vue';
	import { useMessage } from '@/hooks/web/useMessage';
	import { useI18n } from 'vue-i18n'
	import { useUserStore } from '@/store/user';
	import { useModal } from '@/components/Modal';
	import Form from './extraForm.vue';
	import { BasicForm, useForm } from '@/components/Form';
	import { BasicTable, useTable, TableAction, ActionItem, TableActionType, SorterResult } from '@/components/Table';
  import { ExportModal, ImportModal, SuperQueryModal } from '@/components/CommonModal';
  import { downloadByUrl } from '@/utils/file/download';
  import { useRoute,useRouter } from 'vue-router';
  import { useTabs } from '@/hooks/web/useTabs';
  import { getFlowStartFormId } from '@/api/workFlow/template';
  import { FilterOutlined } from '@ant-design/icons-vue';
  import { getSearchFormSchemas } from '@/components/FormGenerator/src/helper/transform';
  import { cloneDeep } from 'lodash-es';
  import columnList from './helper/columnList';
  import searchList from './helper/searchList';
  import { dyOptionsList, systemComponentsList } from '@/components/FormGenerator/src/helper/config';
  import { thousandsFormat, getTimeUnit, getDateTimeUnit,getParamList } from '@/utils/ceri';
  import { CeriRelationForm } from '@/components/CeriOS';
  import dayjs from 'dayjs';
  import { noGroupList } from '@/components/FormGenerator/src/helper/config';
  import { useBaseStore } from '@/store/base';
  import ViewSetting from '@/views/common/dynamicModel/list/components/ViewSetting.vue';
  import ViewList from '@/views/common/dynamicModel/list/components/ViewList.vue';

  interface State {
    config: any;
    columnList: any[];
    printListOptions: any[];
    columnBtnsList: any[];
    customBtnsList: any[];
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
    viewList: any[];
    currentView: any;
  }

  const router = useRouter();
  //const { close } = useTabs();
  const route = useRoute();
  //const { hasBtnP } = usePermission();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const userStore = useUserStore();
  const userInfo = userStore.getUserInfo;
  const baseStore = useBaseStore(); 
  const [registerExportModal, { openModal: openExportModal, closeModal: closeExportModal, setModalProps: setExportModalProps }] = useModal();
  const [registerImportModal, { openModal: openImportModal }] = useModal();
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
    viewList: [],
    currentView: {},
  });
  const { childColumnList, searchSchemas, viewList, currentView  } = toRefs(state);
    const defaultSearchInfo = {
    menuId: route.meta.modelId as string,
    moduleId:'50a206bf-ec22-4f28-bfb2-66434ef9bcb0',
    superQueryJson: '',
  };
  const searchInfo:any = reactive({
    ...cloneDeep(defaultSearchInfo),
  });
  const [registerSearchForm, { updateSchema, resetFields, submit: searchFormSubmit }] = useForm({
    baseColProps: { span: 6 },
    showActionButtonGroup: true,
    showAdvancedButton: true,
    compact: true,
  });
  const [ registerTable, { reload, setLoading, getFetchParams, getSelectRows, getSelectRowKeys, redoHeight, insertTableDataRecord, updateTableDataRecord, deleteTableDataRecord, clearSelectedRowKeys }] = useTable({
    api: getList,
    immediate: false,
    clickToRowSelect: false,
	tableSetting: { setting: false },
    afterFetch: data => {
      const list = data.map(o => ({ 
        ...o, 
        rowEdit: false,
      }));
      state.cacheList = cloneDeep(list);
      return list;
    },
  });

  const getColumns = computed(() => {
    const columns: any[] = state.columns;
    return setListValue(state.currentView?.columnList, columns, 'prop');
  });
  const getSearchList = computed(() => {
    const searchSchemas = cloneDeep(state.searchSchemas).map(o => ({ ...o, show: true }));
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
      pagination: { pageSize: 20 }, //有分页
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
        width: 100,
        fixed: 'right',
        align: 'center',
        title: t('component.table.action'),
        dataIndex: 'action',
      },
    };
    if (unref(getHasBatchBtn)) {
      const rowSelection: any = { type: 'checkbox' };
      data.rowSelection = rowSelection;
    }
    return data;
  });
  function getTableActions(record, index): ActionItem[] {
    const list: any[] = [
      {
      label: t('common.editText','编辑'),
        onClick: updateHandle.bind(null, record),
      },
      {
      label: t('common.delText','删除'),
        color: 'error',
        modelConfirm: {
          onOk: handleDelete.bind(null, record.id),
        },
      },
    ];
    if (record.rowEdit) {
      let editBtnList: ActionItem[] = [
        { label: t('common.saveText'), onClick: saveForRowEdit.bind(null, record, 0) },
        { label: t('common.cancelText'), color: 'error', onClick: cancelRowEdit.bind(null, record, index) },
      ];
      return editBtnList;
    }
    return list;
  }

  function cancelRowEdit(record, index) {
    const id = !record.id || record.id === 'ceriAdd' ? '' : record.id;
    if (!id) return deleteTableDataRecord('ceriAdd');
    record.rowEdit = false;
    const item = cloneDeep(state.cacheList[index]);
    updateTableDataRecord(item.id, item);
  }
  // 行内编辑保存
  function saveForRowEdit(record, status = 0, candidateData: any = null) {
    const id = !record.id || record.id === 'ceriAdd' ? '' : record.id;
    const query = { ...record, id };
    const formMethod = query.id ? update : create;
    formMethod(query).then(res => {
      createMessage.success(res.msg);
      reload({ page: 1 });
    });
  }
  // 新增
  function addHandle() {
    buildRowRelation();
    // 不带流程新增
    let record = { 
      rowEdit: true, 
      id: 'ceriAdd',
      Name: undefined,
      Remark: undefined,
      Sort: undefined,
      Enabled: 0,
    };
    insertTableDataRecord(record, 0);
  }
  // 编辑
  function updateHandle(record) {
    buildRowRelation();
    record.rowEdit = true;
  }
  function handleDelete(id) {
	const query ={ids:[id]};
    batchDelete(query).then(res => {
        createMessage.success(res.msg);
        reload();
    });
  }
  function init() {
    state.config = {};
    searchInfo.menuId = route.meta.modelId as string;
    state.columnList = columnList;
    setLoading(true);
    getSearchSchemas();
    getColumnList();
    initViewList();
        nextTick(() => {
          unref(getSearchList).length ? searchFormSubmit() : reload({ page: 1 });
        });
    buildOptions();
  }
  function getSearchSchemas() {
    const schemas = getSearchFormSchemas(searchList);
    state.searchSchemas = schemas;
    schemas.forEach(cur => {
      const config = cur.__config__;
      if (dyOptionsList.includes(config.ceriKey)) {
        if (config.dataType === 'dictionary') {
          if (!config.dictionaryType) return;
          getDictionaryDataSelector(config.dictionaryType).then(res => {
            updateSchema([{ field: cur.field, componentProps: { options: res.data.list } }]);
          });
        }
        if (config.dataType === 'dynamic') {
          if (!config.propsUrl) return;
          const query = { paramList: getParamList(config.templateJson) || [] };
          getDataInterfaceRes(config.propsUrl, query).then(res => {
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
	state.exportList = columnList.filter(o => !noGroupList.includes(o.__config__.ceriKey)&&!o.__config__.isSubTable);
    let columns = columnList.map(o => ({
      ...o,
	  placeholder: o.placeholderI18nCode ? t(o.placeholderI18nCode, o.placeholder) : o.placeholder,
      title: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
      dataIndex: o.prop,
      align: o.align,
      fixed: o.fixed == 'none' ? false : o.fixed,
      sorter: o.sortable ? { multiple: 1 } : o.sortable,
      width: o.width || 100,
    }));
    columns = getComplexColumns(columns);
    state.columns = columns.filter(o => o.prop.indexOf('-') < 0);
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
  function handleColumnChange(data) {
    state.columnSettingList = data;
  }
  function handleSearchReset() {
    searchFormSubmit();
  }
  function handleSearchSubmit(data) {
    clearSelectedRowKeys();
    let obj = {
      ...defaultSearchInfo,
      superQueryJson: searchInfo.superQueryJson,
      ...data,
      
    };
    Object.keys(searchInfo).map(key => {
      delete searchInfo[key];
    });
    for (let [key, value] of Object.entries(obj)) {
      searchInfo[key.replaceAll('-', '_')] = value;
    }
    reload({ page: 1 });
  }
  // 行内编辑获取选项
  function buildOptions() {
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const cur = list[i];
        if (cur.children?.length) loop(cur.children);
        const config = cur.__config__;
        if (!config) continue;
        if (dyOptionsList.includes(config.ceriKey)) {
          if (config.dataType === 'dictionary') {
            if (!config.dictionaryType) return;
            baseStore.getDicDataSelector(config.dictionaryType).then(res => {
              cur.options = res;
            });
          }
          if (config.dataType === 'dynamic') {
            if (!config.propsUrl) return;
            const query = { paramList: getParamList(config.templateJson) || [] };
            getDataInterfaceRes(config.propsUrl, query).then(res => {
              cur.options = Array.isArray(res.data) ? res.data : [];
            });
          }
        }
      }
    };
    loop(state.columns);
  }
  function handleRowForm(record) {
    const data = {
      id: record.id,
      menuId: searchInfo.menuId,
      formData: record,
    };
    formRef.value?.init(data);
  }
function buildRowRelation() {
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        let cur = list[i];
        if (cur.children?.length) loop(cur.children);
        const config = cur?.__config__;
        if (!config) continue;
        if (config.ceriKey === 'datePicker') {
          if (config.startTimeRule) {
            if (config.startTimeType == 1) cur.startTime = config.startTimeValue;
            if (config.startTimeType == 3) cur.startTime = new Date().getTime();
            if (config.startTimeType == 4 || config.startTimeType == 5) {
              const type = getTimeUnit(config.startTimeTarget);
              const method = config.startTimeType == 4 ? 'subtract' : 'add';
              const startTime = dayjs()[method](config.startTimeValue, type);
              let realStartTime = startTime.startOf('day').valueOf();
              if (config.startTimeTarget == 4) realStartTime = startTime.startOf('minute').valueOf();
              if (config.startTimeTarget == 5) realStartTime = startTime.startOf('second').valueOf();
              if (config.startTimeTarget == 6) realStartTime = startTime.valueOf();
              cur.startTime = realStartTime;
            }
          }
          if (config.endTimeRule) {
            if (config.endTimeType == 1) cur.endTime = config.endTimeValue;
            if (config.endTimeType == 3) cur.endTime = new Date().getTime();
            if (config.endTimeType == 4 || config.endTimeType == 5) {
              const type = getTimeUnit(config.endTimeTarget);
              const method = config.endTimeType == 4 ? 'subtract' : 'add';
              const endTime = dayjs()[method](config.endTimeValue, type);
              let realEndTime = endTime.endOf('day').valueOf();
              if (config.endTimeTarget == 4) realEndTime = endTime.endOf('minute').valueOf();
              if (config.endTimeTarget == 5) realEndTime = endTime.endOf('second').valueOf();
              if (config.endTimeTarget == 6) realEndTime = endTime.valueOf();
              cur.endTime = realEndTime;
            }
          }
        }
        if (config.ceriKey === 'timePicker') {
          if (config.startTimeRule) {
            if (config.startTimeType == 1) cur.startTime = config.startTimeValue || null;
            if (config.startTimeType == 3) cur.startTime = dayjs().format(cur.format);
            if (config.startTimeType == 4 || config.startTimeType == 5) {
              const type = getTimeUnit(config.startTimeTarget + 3);
              const method = config.startTimeType == 4 ? 'subtract' : 'add';
              const startTime = dayjs()[method](config.startTimeValue, type).format(cur.format);
              cur.startTime = startTime;
            }
          }
          if (config.endTimeRule) {
            if (config.endTimeType == 1) cur.endTime = config.endTimeValue || null;
            if (config.endTimeType == 3) cur.endTime = dayjs().format(cur.format);
            if (config.endTimeType == 4 || config.endTimeType == 5) {
              const type = getTimeUnit(config.endTimeTarget + 3);
              const method = config.endTimeType == 4 ? 'subtract' : 'add';
              const endTime = dayjs()[method](config.endTimeValue, type).format(cur.format);
              cur.endTime = endTime;
            }
          }
        }
      }
    };
    loop(state.columns);
  }

function initViewList(currentId = '') {
    const query = {
      menuId: searchInfo.menuId,
    };
    getViewList(query).then(res => {
      const columns: any[] = state.columns;
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