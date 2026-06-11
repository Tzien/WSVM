<template>
  <div class="condition-main" :class="{ 'condition-main-bordered': bordered, 'condition-main-compact': compact }">
    <div class="mb-10px" v-if="conditionList.length">
      <ceri-radio v-model:value="matchLogic" :options="logicOptions" optionType="button" button-style="solid" />
    </div>
    <div class="condition-item" v-for="(item, index) in conditionList" :key="index">
      <div class="condition-item-title">
        <div>条件组</div>
        <i class="icon-ym icon-ym-nav-close" @click="delGroup(index)"></i>
      </div>
      <div class="condition-item-content" :class="{ '!px-5px': compact }">
        <div class="condition-item-cap">
          以下条件全部执行：
          <ceri-radio v-model:value="item.logic" :options="logicOptions" optionType="button" button-style="solid" size="small" />
        </div>
        <a-row :gutter="compact ? 2 : 8" v-for="(child, childIndex) in item.groups" :key="index + childIndex" class="mb-10px">
          <a-col :span="6">
            <ceri-select
              v-model:value="child.field"
              :options="fieldOptions"
              showSearch
              allowClear
              :fieldNames="{ options: 'options1' }"
              @change="(val, data) => onFieldChange(val, data, child, index, childIndex)" />
          </a-col>
          <a-col :span="5">
            <ceri-select
              v-model:value="child.symbol"
              placeholder="运算符号"
              :options="getSymbolOptions(child.ceriKey)"
              :dropdownMatchSelectWidth="false"
              @change="(val, data) => onSymbolChange(val, data, child)" />
          </a-col>
          <a-col :span="4" v-if="showFieldValueType">
            <ceri-select
              v-model:value="child.fieldValueType"
              :options="isCustomFieldValueType ? getSourceTypeOptions(child) : sourceTypeOptions"
              :disabled="child.disabled"
              @change="child.fieldValue = undefined" />
          </a-col>
          <a-col :span="8" v-if="child.fieldValueType === 1">
            <ceri-select
              v-model:value="child.fieldValue"
              :options="valueFieldOptions"
              showSearch
              allowClear
              :fieldNames="{ options: 'children' }"
              :disabled="child.disabled" />
          </a-col>
          <a-col :span="showFieldValueType ? 8 : 12" v-if="child.fieldValueType == 2">
            <template v-if="child.ceriKey === 'inputNumber'">
              <ceri-number-range v-model:value="child.fieldValue" :precision="child.precision" :disabled="child.disabled" v-if="child.symbol == 'between'" />
              <ceri-input-number v-model:value="child.fieldValue" :precision="child.precision" :disabled="child.disabled" placeholder="请输入" v-else />
            </template>
            <template v-else-if="child.ceriKey === 'calculate'">
              <ceri-number-range
                v-model:value="child.fieldValue"
                :precision="child.precision || 0"
                :disabled="child.disabled"
                v-if="child.symbol == 'between'" />
              <ceri-input-number v-model:value="child.fieldValue" :precision="child.precision || 0" :disabled="child.disabled" placeholder="请输入" v-else />
            </template>
            <template v-else-if="['rate', 'slider'].includes(child.ceriKey)">
              <ceri-number-range
                v-model:value="child.fieldValue"
                :precision="child.ceriKey == 'rate' && child.allowHalf ? 1 : 0"
                :disabled="child.disabled"
                v-if="child.symbol == 'between'" />
              <ceri-input-number
                v-model:value="child.fieldValue"
                :precision="child.ceriKey == 'rate' && child.allowHalf ? 1 : 0"
                :disabled="child.disabled"
                placeholder="请输入"
                v-else />
            </template>
            <div class="pt-3px" v-else-if="child.ceriKey === 'switch'">
              <ceri-switch v-model:value="child.fieldValue" :disabled="child.disabled" />
            </div>
            <template v-else-if="child.ceriKey === 'colorPicker'">
              <ceri-color-picker v-model:value="child.fieldValue" size="small" :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'timePicker'">
              <ceri-time-range v-model:value="child.fieldValue" :format="child.format" allowClear :disabled="child.disabled" v-if="child.symbol == 'between'" />
              <ceri-time-picker v-model:value="child.fieldValue" :format="child.format" allowClear :disabled="child.disabled" v-else />
            </template>
            <template v-else-if="['datePicker', 'createTime', 'modifyTime'].includes(child.ceriKey)">
              <ceri-date-range
                v-model:value="child.fieldValue"
                :format="child.format || 'YYYY-MM-DD HH:mm:ss'"
                allowClear
                :disabled="child.disabled"
                v-if="child.symbol == 'between'" />
              <ceri-date-picker v-model:value="child.fieldValue" :format="child.format || 'YYYY-MM-DD HH:mm:ss'" allowClear :disabled="child.disabled" v-else />
            </template>
            <template v-else-if="['organizeSelect', 'currOrganize'].includes(child.ceriKey)">
              <ceri-organize-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="['depSelect'].includes(child.ceriKey)">
              <ceri-dep-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'roleSelect'">
              <ceri-role-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'groupSelect'">
              <ceri-group-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'posSelect'">
              <ceri-pos-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'currPosition'">
              <ceri-pos-select v-model:value="child.fieldValue" allowClear :multiple="child.multiple" :disabled="child.disabled" />
            </template>
            <template v-else-if="['createUser', 'modifyUser'].includes(child.ceriKey)">
              <ceri-user-select v-model:value="child.fieldValue" allowClear :multiple="child.multiple" :disabled="child.disabled" />
            </template>
            <template v-else-if="['userSelect'].includes(child.ceriKey)">
              <ceri-user-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType != 'all' && child.selectType != 'custom' ? 'all' : child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="['usersSelect'].includes(child.ceriKey)">
              <ceri-users-select
                v-model:value="child.fieldValue"
                allowClear
                :selectType="child.selectType"
                :ableIds="child.ableIds"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'areaSelect'">
              <ceri-area-select
                v-model:value="child.fieldValue"
                :level="child.level"
                allowClear
                :multiple="child.multiple"
                :disabled="child.disabled"
                :key="item.cellKey" />
            </template>
            <template v-else-if="['select', 'radio', 'checkbox'].includes(child.ceriKey)">
              <ceri-select
                v-model:value="child.fieldValue"
                showSearch
                allowClear
                :options="child.options"
                :fieldNames="child.props"
                :multiple="child.multiple || child.ceriKey === 'checkbox'"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'cascader'">
              <ceri-cascader
                v-model:value="child.fieldValue"
                :options="child.options"
                :fieldNames="child.props"
                :showAllLevels="child.showAllLevels"
                showSearch
                allowClear
                placeholder="请选择"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'treeSelect'">
              <ceri-tree-select
                v-model:value="child.fieldValue"
                :options="child.options"
                :fieldNames="child.props"
                showSearch
                allowClear
                placeholder="请选择"
                :multiple="child.multiple"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'relationForm'">
              <ceri-relation-form
                v-model:value="child.fieldValue"
                placeholder="请选择"
                :modelId="child.modelId"
                allowClear
                :columnOptions="child.columnOptions"
                :relationField="child.relationField"
                :hasPage="child.hasPage"
                :pageSize="child.pageSize"
                :popupType="child.popupType"
                :popupTitle="child.popupTitle"
                :popupWidth="child.popupWidth"
                :disabled="child.disabled"
                :queryType="child.queryType"
                :propsValue="child.propsValue" />
            </template>
            <template v-else-if="child.ceriKey === 'popupSelect' || child.ceriKey === 'popupTableSelect'">
              <ceri-popup-select
                v-model:value="child.fieldValue"
                placeholder="请选择"
                :interfaceId="child.interfaceId"
                allowClear
                :multiple="child.multiple"
                :columnOptions="child.columnOptions"
                :propsValue="child.propsValue"
                :templateJson="child.templateJson"
                :relationField="child.relationField"
                :hasPage="child.hasPage"
                :pageSize="child.pageSize"
                :popupType="child.popupType"
                :popupTitle="child.popupTitle"
                :popupWidth="child.popupWidth"
                :disabled="child.disabled" />
            </template>
            <template v-else-if="child.ceriKey === 'autoComplete'">
              <ceri-auto-complete
                v-model:value="child.fieldValue"
                placeholder="请输入"
                allowClear
                :interfaceId="child.interfaceId"
                :relationField="child.relationField"
                :templateJson="child.templateJson"
                :total="child.total"
                :disabled="child.disabled" />
            </template>
            <template v-else>
              <a-input v-model:value="child.fieldValue" placeholder="请输入" allowClear :disabled="child.disabled" />
            </template>
          </a-col>
          <a-col :span="8" v-if="child.fieldValueType === 4">
            <ceri-select v-model:value="child.fieldValue" :options="getParameterList(child.ceriKey)" allowClear />
          </a-col>
          <a-col :span="1" class="text-center">
            <i class="icon-ym icon-ym-btn-clearn" @click="delItem(index, childIndex)" />
          </a-col>
        </a-row>
        <span class="link-text inline-block" @click="addItem(index)"><i class="icon-ym icon-ym-btn-add text-14px mr-4px"></i>添加条件</span>
      </div>
    </div>
    <div class="query-noData" v-show="!conditionList.length && isSuperQuery">
      <!-- <img src="../../../../assets/images/query-noData.png" class="noData-img" /> -->
      <div class="noData-txt">
        <span>没有任何查询条件</span>
        <a-divider type="vertical"></a-divider>
        <span class="link-text" @click="addGroup">点击新增</span>
      </div>
    </div>
    <span class="link-text inline-block" @click="addGroup()" v-show="conditionList.length || !isSuperQuery">
      <i class="icon-ym icon-ym-btn-add text-14px mr-4px"></i>添加条件组
    </span>
  </div>
</template>

<script lang="ts" setup>
  import { reactive, toRefs, watch, computed } from 'vue';
  import { getDictionaryDataSelector } from '@/api/systemData/dictionary';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  import { useMessage } from '@/hooks/web/useMessage';
  import { cloneDeep } from 'lodash-es';
  import { dyOptionsList } from '@/views/WebDesign/component/StepForm/src/helper/config';
  import { CeriRelationForm } from '@/components/CeriOS';
  import { isEmpty } from '@/utils/is';
  import { getParamList } from '@/utils/ceri';
  import { useDebounceFn } from '@vueuse/core';

  interface State {
    conditionList: any;
    fieldOptions: any[];
    matchLogic: any;
  }

  const props = defineProps({
    bordered: { type: Boolean, default: false },
    isSuperQuery: { type: Boolean, default: false },
    defaultAddEmpty: { type: Boolean, default: false },
    showFieldValueType: { type: Boolean, default: false },
    valueFieldOptions: { type: Array, default: () => [] },
    compact: { type: Boolean, default: false },
    isCustomFieldValueType: { type: Boolean, default: false },
  });
  defineExpose({
    init,
    confirm,
    updateConditionList,
  });

  const { createMessage } = useMessage();
  const notSupportList = [
    'relationFormAttr',
    'popupAttr',
    'uploadFile',
    'uploadImg',
    'colorPicker',
    'editor',
    'link',
    'button',
    'text',
    'alert',
    'table',
    'sign',
    'signature',
  ];
  const emptyChildItem = {
    field: '',
    symbol: '',
    ceriKey: '',
    fieldValueType: !props.showFieldValueType || props.isCustomFieldValueType ? 2 : 1,
    fieldValue: undefined,
    fieldValueCeriKey: '',
    cellKey: +new Date(),
  };
  const emptyItem = { logic: 'and', groups: [emptyChildItem] };
  const sourceTypeOptions = [
    { id: 1, fullName: '字段' },
    { id: 2, fullName: '自定义' },
  ];
  const logicOptions = [
    { id: 'and', fullName: '且' },
    { id: 'or', fullName: '或' },
  ];
  const baseSymbolOptions = [
    { id: '==', fullName: '等于' },
    { id: '<>', fullName: '不等于' },
    { id: 'like', fullName: '包含' },
    { id: 'notLike', fullName: '不包含' },
    { id: 'null', fullName: '为空' },
    { id: 'notNull', fullName: '不为空' },
  ];
  const rangeSymbolOptions = [
    { id: '>=', fullName: '大于等于' },
    { id: '>', fullName: '大于' },
    { id: '==', fullName: '等于' },
    { id: '<=', fullName: '小于等于' },
    { id: '<', fullName: '小于' },
    { id: '<>', fullName: '不等于' },
    { id: 'between', fullName: '介于' },
    { id: 'null', fullName: '为空' },
    { id: 'notNull', fullName: '不为空' },
  ];
  const selectSymbolOptions = [
    { id: '==', fullName: '等于' },
    { id: '<>', fullName: '不等于' },
    { id: 'in', fullName: '包含任意一个' },
    { id: 'notIn', fullName: '不包含任意一个' },
    { id: 'null', fullName: '为空' },
    { id: 'notNull', fullName: '不为空' },
  ];
  const switchSymbolOptions = [
    { id: '==', fullName: '等于' },
    { id: '<>', fullName: '不等于' },
  ];
  const locationSymbolOptions = [
    { id: 'like', fullName: '包含' },
    { id: 'notLike', fullName: '不包含' },
    { id: 'null', fullName: '为空' },
    { id: 'notNull', fullName: '不为空' },
  ];
  const relationFormSymbolOptions = [...switchSymbolOptions, { id: 'null', fullName: '为空' }, { id: 'notNull', fullName: '不为空' }];
  const useRangeSymbolList = ['calculate', 'inputNumber', 'rate', 'slider', 'datePicker', 'timePicker', 'createTime', 'modifyTime'];
  const useSelectSymbolList = [
    'radio',
    'checkbox',
    'select',
    'treeSelect',
    'cascader',
    'areaSelect',
    'organizeSelect',
    'depSelect',
    'posSelect',
    'userSelect',
    'usersSelect',
    'roleSelect',
    'groupSelect',
    'createUser',
    'modifyUser',
    'currOrganize',
    'currPosition',
    'popupTableSelect',
  ];
  const dateList = ['datePicker', 'createTime', 'modifyTime'];
  const organizeList = ['organizeSelect', 'currOrganize'];
  const depList = ['depSelect'];
  const positionList = ['posSelect', 'currPosition'];
  const userList = ['userSelect', 'createUser', 'modifyUser'];
  const useSwitchSymbolList = ['switch'];
  const useRelationFormSymbolList = ['relationForm', 'popupSelect'];
  const state = reactive<State>({
    conditionList: [],
    fieldOptions: [],
    matchLogic: 'and',
  });
  const { conditionList, fieldOptions, matchLogic } = toRefs(state);
  const emit = defineEmits(['confirm']);
  const onConfirm = useDebounceFn(data => {
    emit('confirm', data);
  }, 300);

  watch(
    [() => state.conditionList, () => state.matchLogic],
    () => {
      props.compact && onConfirm({ conditionList: state.conditionList, matchLogic: state.matchLogic });
    },
    { deep: true },
  );

  function init(data) {
    updateConditionList(data);
    const fieldOptions = data.fieldOptions.filter(o => !notSupportList.includes(o.__config__.ceriKey));
    state.fieldOptions = buildOptions(fieldOptions);
    if (!state.conditionList.length && props.defaultAddEmpty) addGroup();
  }
  function updateConditionList(data) {
    state.conditionList = cloneDeep(data.conditionList || []);
    state.matchLogic = data.matchLogic || 'and';
  }
  function buildOptions(componentList) {
    componentList.forEach(cur => {
      cur.disabled = false;
      const config = cur.__config__;
      if (dyOptionsList.includes(config.ceriKey)) {
        if (config.dataType === 'dictionary' && config.dictionaryType) {
          cur.options = [];
          getDictionaryDataSelector(config.dictionaryType).then(res => {
            cur.options = res.data.list;
          });
        }
        if (config.dataType === 'dynamic' && config.propsUrl) {
          cur.options = [];
          const query = { paramList: getParamList(config.templateJson) };
          getDataInterfaceRes(config.propsUrl, query).then(res => {
            cur.options = Array.isArray(res.data) ? res.data : [];
          });
        }
      }
    });
    return componentList;
  }
  function onFieldChange(val, data, item, index, childIndex) {
    item.cellKey = +new Date();
    if (item.fieldValueType != 1) {
      item.fieldValue = undefined;
      item.fieldValueCeriKey = '';
    }
    const newItem = cloneDeep(emptyChildItem);
    for (let key of Object.keys(newItem)) {
      newItem[key] = item[key];
    }
    if (!val) {
      item.ceriKey = '';
      item.symbol = undefined;
      item.disabled = false;
      return;
    }
    item = { ...newItem, ...data };
    const config = data.__config__;
    if (item.ceriKey != config.ceriKey) item.symbol = undefined;
    item.ceriKey = data.__config__?.ceriKey || '';
    item.disabled = ['null', 'notNull'].includes(item.symbol);
    item.multiple = ['in', 'notIn'].includes(item.symbol);
    state.conditionList[index].groups[childIndex] = item;
  }
  function onSymbolChange(val, _data, item) {
    item.fieldValue = undefined;
    item.disabled = ['null', 'notNull'].includes(val);
    item.multiple = ['in', 'notIn'].includes(val);
    if (props.showFieldValueType && (['null', 'notNull'].includes(val) || (val == 'between' && dateList.includes(item.ceriKey)))) {
      item.fieldValueType = !props.showFieldValueType || props.isCustomFieldValueType ? 2 : 1;
      item.fieldValueCeriKey = '';
    }
  }
  function addItem(index) {
    state.conditionList[index].groups.push(cloneDeep(emptyChildItem));
  }
  function delItem(index, childIndex) {
    state.conditionList[index].groups.splice(childIndex, 1);
    if (!state.conditionList[index].groups.length) delGroup(index);
  }
  function addGroup() {
    state.conditionList.push(cloneDeep(emptyItem));
  }
  function delGroup(index) {
    state.conditionList.splice(index, 1);
  }
  function getSymbolOptions(ceriKey) {
    if (useSwitchSymbolList.includes(ceriKey)) return switchSymbolOptions;
    if (useRelationFormSymbolList.includes(ceriKey)) return relationFormSymbolOptions;
    if (useRangeSymbolList.includes(ceriKey)) return rangeSymbolOptions;
    if (useSelectSymbolList.includes(ceriKey)) return selectSymbolOptions;
    if (ceriKey == 'location') return locationSymbolOptions;
    return baseSymbolOptions;
  }
  function exist() {
    let isOk = true;
    for (let i = 0; i < state.conditionList.length; i++) {
      const e = state.conditionList[i];
      for (let j = 0; j < e.groups.length; j++) {
        const child = e.groups[j];
        if (!child.field) {
          createMessage.warning('条件字段不能为空');
          isOk = false;
          return;
        }
        if (!child.symbol) {
          createMessage.warning('条件符号不能为空');
          isOk = false;
          return;
        }
        if (!['null', 'notNull'].includes(child.symbol) && ((!child.fieldValue && child.fieldValue !== 0) || isEmpty(child.fieldValue))) {
          createMessage.warning('数据值不能为空');
          isOk = false;
          return;
        }
      }
    }
    return isOk;
  }
  function confirm() {
    if (!exist()) return false;
    return {
      matchLogic: state.matchLogic,
      conditionList: cloneDeep(state.conditionList),
    };
  }
  function getSourceTypeOptions({ symbol, ceriKey }) {
    const list = [...organizeList, ...depList, ...positionList, ...userList];
    const options = [{ id: 2, fullName: '自定义' }];
    return list.includes(ceriKey) || (symbol != 'between' && dateList.includes(ceriKey)) ? [...options, { id: 4, fullName: '系统变量' }] : options;
  }
  function getParameterList(ceriKey) {
    if (dateList.includes(ceriKey)) return [{ id: '@currentTime', fullName: '当前时间' }];
    if (positionList.includes(ceriKey)) return [{ id: '@positionId', fullName: '当前岗位' }];
    if (organizeList.includes(ceriKey))
      return [
        { id: '@organizeId', fullName: '当前组织' },
        { id: '@organizationAndSuborganization', fullName: '当前组织及子组织' },
        { id: '@branchManageOrganize', fullName: '当前分管组织' },
      ];
    if (depList.includes(ceriKey))
      return [
        { id: '@depId', fullName: '当前部门' },
        { id: '@depAndSubordinates', fullName: '当前部门及下级部门' },
      ];
    if (userList.includes(ceriKey))
      return [
        { id: '@userId', fullName: '当前用户' },
        { id: '@userAndSubordinates', fullName: '当前用户及下属' },
      ];
    return [];
  }
</script>
