<template>
  <a-divider>单据配置</a-divider>
  <a-form-item class="flex justify-center">
    <ceri-radio v-model:value="activeData.__config__.ruleType" :options="ruleTypeOptions" optionType="button" buttonStyle="solid" @change="onRuleTypeChange" />
  </a-form-item>
  <a-form-item label="单据模板" v-if="activeData.__config__.ruleType !== 2">
    <BillRuleModal :value="activeData.__config__.rule" :title="activeData.__config__.ruleName" @change="onRuleChange" />
  </a-form-item>
  <template v-if="activeData.__config__.ruleType === 2">
    <a-form-item>
      <template #label>前缀设置<BasicHelp text="前缀长度不超过100字符" /></template>
      <a-table
        :data-source="activeData.__config__.ruleConfig.prefixList"
        :columns="prefixColumns"
        size="small"
        :pagination="false"
        rowKey="id"
        class="prefix-table">
        <template #bodyCell="{ column, record, index }">
          <template v-if="column.key === 'drag'">
            <i class="drag-handler icon-ym icon-ym-darg" title="点击拖动" />
          </template>
          <template v-if="column.key === 'sourceType'">
            <ceri-select
              v-model:value="record.sourceType"
              placeholder="请选择"
              :options="sourceTypeOptions"
              class="!w-84px"
              @change="onSourceTypeChange(record)" />
          </template>
          <template v-if="column.key === 'relationField'">
            <ceri-select
              v-model:value="record.relationField"
              placeholder="请选择"
              :options="formFieldsOptions"
              allowClear
              showSearch
              class="!w-104px"
              v-if="record.sourceType === 1" />
            <a-input v-model:value="record.relationField" placeholder="请输入" allowClear class="!w-104px" v-else />
          </template>
          <template v-if="column.key === 'action'">
            <a-button class="action-btn" type="link" color="error" @click="handleDelItem(index, 'prefixList')" size="small">删除</a-button>
          </template>
        </template>
        <template #emptyText>
          <p class="leading-30px">暂无数据</p>
        </template>
      </a-table>
      <div class="table-add-action" @click="handleAddItem('prefixList')">
        <a-button type="link" preIcon="icon-ym icon-ym-btn-add">新增</a-button>
      </div>
    </a-form-item>
    <a-form-item label="方式设置">
      <ceri-radio
        v-model:value="activeData.__config__.ruleConfig.type"
        :options="typeOptions"
        optionType="button"
        buttonStyle="solid"
        @change="onRuleTypeChange" />
    </a-form-item>
    <template v-if="activeData.__config__.ruleConfig.type === 1">
      <a-form-item label="格式">
        <ceri-select v-model:value="activeData.__config__.ruleConfig.dateFormat" :options="dateFormatOptions" />
      </a-form-item>
      <a-form-item label="位数">
        <ceri-input-number
          v-model:value="activeData.__config__.ruleConfig.digit"
          placeholder="请输入"
          :min="1"
          :max="9"
          :precision="0"
          @change="handleChange" />
      </a-form-item>
      <a-form-item>
        <template #label>起始值<BasicHelp text="不允许输入0或特殊字符" /></template>
        <ceri-input v-model:value="activeData.__config__.ruleConfig.startNumber" placeholder="请输入" @change="handleChange" />
      </a-form-item>
    </template>
    <template v-if="activeData.__config__.ruleConfig.type === 2">
      <a-form-item label="位数">
        <ceri-select v-model:value="activeData.__config__.ruleConfig.randomDigit" :options="randomDigitOptions" />
      </a-form-item>
      <a-form-item label="类型">
        <ceri-select v-model:value="activeData.__config__.ruleConfig.randomType" :options="randomTypeOptions" />
      </a-form-item>
    </template>
    <a-form-item>
      <template #label>后缀设置<BasicHelp text="后缀长度不超过100字符" /></template>
      <a-table
        :data-source="activeData.__config__.ruleConfig.suffixList"
        :columns="prefixColumns"
        size="small"
        :pagination="false"
        rowKey="id"
        class="suffix-table">
        <template #bodyCell="{ column, record, index }">
          <template v-if="column.key === 'drag'">
            <i class="drag-handler icon-ym icon-ym-darg" title="点击拖动" />
          </template>
          <template v-if="column.key === 'sourceType'">
            <ceri-select
              v-model:value="record.sourceType"
              placeholder="请选择"
              :options="sourceTypeOptions"
              class="!w-84px"
              @change="onSourceTypeChange(record)" />
          </template>
          <template v-if="column.key === 'relationField'">
            <ceri-select
              v-model:value="record.relationField"
              placeholder="请选择"
              :options="formFieldsOptions"
              allowClear
              showSearch
              class="!w-104px"
              v-if="record.sourceType === 1" />
            <a-input v-model:value="record.relationField" placeholder="请输入" allowClear class="!w-104px" v-else />
          </template>
          <template v-if="column.key === 'action'">
            <a-button class="action-btn" type="link" color="error" @click="handleDelItem(index, 'suffixList')" size="small">删除</a-button>
          </template>
        </template>
        <template #emptyText>
          <p class="leading-30px">暂无数据</p>
        </template>
      </a-table>
      <div class="table-add-action" @click="handleAddItem('suffixList')">
        <a-button type="link" preIcon="icon-ym icon-ym-btn-add">新增</a-button>
      </div>
    </a-form-item>
  </template>
</template>
<script lang="ts" setup>
  import { computed, nextTick } from 'vue';
  import { BillRuleModal } from '@/components/CommonModal';
  import { cloneDeep } from 'lodash-es';
  import { buildBitUUID } from '@/utils/uuid';
  import Sortablejs from 'sortablejs';

  defineOptions({ inheritAttrs: false });
  const props = defineProps(['activeData', 'drawingList']);

  const defaultConfig = {
    prefixList: [],
    type: 1,
    dateFormat: 'YYYY',
    digit: null,
    startNumber: '',
    randomDigit: 16,
    randomType: 1,
    suffixList: [],
  };

  const ruleTypeOptions = [
    { id: 1, fullName: '单据模板' },
    { id: 2, fullName: '单据规则' },
  ];
  const typeOptions = [
    { id: 1, fullName: '时间格式' },
    { id: 2, fullName: '随机数编号' },
    { id: 3, fullName: 'UUID' },
  ];
  const sourceTypeOptions = [
    { id: 2, fullName: '自定义' },
    { id: 1, fullName: '字段' },
  ];
  const dateFormatOptions = [
    { fullName: 'YYYY', id: 'YYYY' },
    { fullName: 'YYYYMM', id: 'YYYYMM' },
    { fullName: 'YYYYMMDD', id: 'YYYYMMDD' },
    { fullName: 'YYYYMMDDHH', id: 'YYYYMMDDHH' },
    { fullName: 'YYYYMMDDHHmm', id: 'YYYYMMDDHHmm' },
    { fullName: 'YYYYMMDDHHmmss', id: 'YYYYMMDDHHmmss' },
    { fullName: 'YYYYMMDDHHmmssSSS', id: 'YYYYMMDDHHmmssSSS' },
    { fullName: 'no', id: 'no' },
  ];
  const randomDigitOptions = [
    { fullName: '16', id: 16 },
    { fullName: '32', id: 32 },
  ];
  const randomTypeOptions = [
    { fullName: '纯数字', id: 1 },
    { fullName: '字母+数字', id: 2 },
  ];
  const prefixColumns = [
    { title: '拖动', dataIndex: 'drag', key: 'drag', align: 'center', width: 50 },
    { title: '类型', dataIndex: 'sourceType', key: 'sourceType', width: 100 },
    { title: '值', dataIndex: 'relationField', key: 'relationField', width: 120 },
    { title: '操作', dataIndex: 'action', key: 'action', align: 'center', width: 50 },
  ];
  const allowList = ['input', 'inputNumber'];

  const formFieldsOptions = computed(() => {
    let list: any[] = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
      if (Array.isArray(data)) data.forEach(d => loop(d, parent));
      if (data.__config__ && data.__config__.ceriKey) {
        if (allowList.includes(data.__config__.ceriKey) && data.__vModel__ && isSameSource(data)) {
          const isTableChild = parent && parent.__config__ && parent.__config__.ceriKey === 'table';
          const item = {
            id: isTableChild ? parent.__vModel__ + '-' + data.__vModel__ : data.__vModel__,
            fullName: isTableChild ? parent.__config__.label + '-' + data.__config__.label : data.__config__.label,
          };
          list.push(item);
        }
      }
    };
    loop(props.drawingList);
    return list;
  });

  function isSameSource(data) {
    const isSubTable = props.activeData.__config__.isSubTable;
    if (isSubTable) return data.__config__.isSubTable && props.activeData.__config__.parentVModel === data.__config__.parentVModel;
    return !data.__config__.isSubTable;
  }
  function onRuleTypeChange(val) {
    if (!props.activeData.__config__.ruleConfig || JSON.stringify(props.activeData.__config__.ruleConfig) === '{}') {
      props.activeData.__config__.ruleConfig = cloneDeep(defaultConfig);
    }
    if (val !== 2) return;
    nextTick(() => {
      initPrefixSort();
      initSuffixSort();
    });
  }
  function onRuleChange(val, row) {
    if (!val) {
      props.activeData.__config__.rule = '';
      props.activeData.__config__.ruleName = '';
      return;
    }
    if (props.activeData.__config__.rule === val) return;
    props.activeData.__config__.rule = val;
    props.activeData.__config__.ruleName = row.fullName;
  }
  function handleAddItem(key) {
    const item = { id: buildBitUUID(), sourceType: 2, relationField: '' };
    props.activeData.__config__.ruleConfig[key].push(item);
  }
  function handleDelItem(index, key) {
    props.activeData.__config__.ruleConfig[key].splice(index, 1);
  }
  function onSourceTypeChange(record) {
    record.relationField = '';
  }
  function initPrefixSort() {
    const table: any = document.querySelector(`.prefix-table .ant-table-tbody`);
    Sortablejs.create(table, {
      handle: '.drag-handler',
      animation: 150,
      easing: 'cubic-bezier(1, 0, 0, 1)',
      onStart: () => {},
      onEnd: ({ newIndex, oldIndex }: any) => {
        const currRow = props.activeData.__config__.ruleConfig.prefixList.splice(oldIndex, 1)[0];
        props.activeData.__config__.ruleConfig.prefixList.splice(newIndex, 0, currRow);
      },
    });
  }
  function initSuffixSort() {
    const table: any = document.querySelector(`.suffix-table .ant-table-tbody`);
    Sortablejs.create(table, {
      handle: '.drag-handler',
      animation: 150,
      easing: 'cubic-bezier(1, 0, 0, 1)',
      onStart: () => {},
      onEnd: ({ newIndex, oldIndex }: any) => {
        const currRow = props.activeData.__config__.ruleConfig.suffixList.splice(oldIndex, 1)[0];
        props.activeData.__config__.ruleConfig.suffixList.splice(newIndex, 0, currRow);
      },
    });
  }
  function handleChange() {
    // 流水位数
    let digitVal = props.activeData.__config__.ruleConfig.digit || 0;
    if (digitVal) digitVal = Array(digitVal).join('0') + 0;
    // 流水起始
    const startNumber = props.activeData.__config__.ruleConfig.startNumber || '';
    let startNumberVal = '';
    if (startNumber != '') {
      startNumberVal = digitVal + startNumber;
      digitVal = startNumberVal.substring(startNumberVal.length - digitVal.length, startNumberVal.length);
      props.activeData.__config__.ruleConfig.startNumber = digitVal;
    }
  }
</script>
