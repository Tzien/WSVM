<template>
  <a-form-item label="选择流程" :name="['launchFlow', 'flowId']" :rules="[{ required: true, message: '必填', trigger: 'change' }]">
    <FlowModal :value="dataForm.launchFlow.flowId" :title="dataForm.launchFlow.flowName" @change="onFlowIdChange" :allowClear="false" />
  </a-form-item>
  <a-form-item label="字段设置" style="margin-bottom: 0"></a-form-item>
  <a-table :data-source="dataForm.launchFlow?.transferList" :columns="launchColumns" size="small" :pagination="false">
    <template #bodyCell="{ column, record, index }">
      <template v-if="column.key === 'targetField'">
        <ceri-select
          class="!w-190px"
          v-model:value="record.targetField"
          :options="getTargetOptions(index)"
          showSearch
          allowClear
          :disabled="record.required"
          :fieldNames="{ options: 'options1' }" />
      </template>
      <template v-if="column.key === 'tips'">的值设为</template>
      <template v-if="column.key === 'sourceType'">
        <ceri-select v-model:value="record.sourceType" :options="sourceTypeOptions" @change="record.sourceValue = ''" />
      </template>
      <template v-if="column.key === 'sourceValue'">
        <ceri-select
          class="!w-190px"
          v-model:value="record.sourceValue"
          :options="getSourceValueOptions(record)"
          placeholder="数据源字段"
          showSearch
          allowClear
          :fieldNames="{ options: 'options1' }"
          v-if="record.sourceType === 1" />
        <a-input v-model:value="record.sourceValue" placeholder="请输入" allowClear v-if="record.sourceType === 2" />
      </template>
      <template v-if="column.key === 'action'">
        <a-button class="action-btn" type="link" color="error" @click="handleDelItem(index)" size="small" :disabled="record.required">删除</a-button>
      </template>
    </template>
    <template #emptyText>
      <p class="leading-60px">暂无数据</p>
    </template>
  </a-table>
  <div class="table-add-action mb-20px" @click="addTransferItem()">
    <a-button type="link" preIcon="icon-ym icon-ym-btn-add">{{ t('common.add2Text') }}</a-button>
  </div>
  <a-form-item>
    <template #label>发起人<BasicHelp text="多个审批人时产生多条流程实例" /></template>
    <CeriCheckboxSingle class="leading-32px" label="当前用户" v-model:value="dataForm.launchFlow.currentUser" /><br />
    <CeriCheckboxSingle class="leading-32px" label="自定义" v-model:value="dataForm.launchFlow.customUser" />
    <div class="mt-6px" v-if="dataForm.launchFlow.customUser">
      <a-button preIcon="icon-ym icon-ym-btn-add" @click="createMessage.error('请选择流程')" v-if="!dataForm.launchFlow.flowId">添加发起人</a-button>
      <template v-else>
        <initiator-user-select
          v-model:value="dataForm.launchFlow.initiator"
          :flowId="dataForm.launchFlow.flowId"
          buttonType="button"
          multiple
          v-if="dataForm.launchFlow.launchPermission == 2" />
        <ceri-users-select v-model:value="dataForm.launchFlow.initiator" buttonType="button" modalTitle="添加发起人" multiple v-else />
      </template>
    </div>
  </a-form-item>
</template>
<script lang="ts" setup>
  import { useI18n } from '@/hooks/web/useI18n';
  import { useMessage } from '@/hooks/web/useMessage';
  import { simpleSourceTypeOptions as sourceTypeOptions } from '@/components/FlowProcess/src/helper/define';
  import { dyOptionsList } from '@/components/FormGenerator/src/helper/config';
  import { omit } from 'lodash-es';
  import { getFlowFormInfo } from '@/api/workFlow/template';
  import { systemComponentsList } from '@/components/FormGenerator/src/helper/config';
  import FlowModal from '@/components/FlowProcess/src/propPanel/components/FlowModal.vue';
  import InitiatorUserSelect from '@/components/FlowProcess/src/propPanel/components/InitiatorUserSelect.vue';

  const props = defineProps({
    dataForm: { type: Object, default: () => {} },
    allFormFieldsOptions: { type: Array as PropType<any>, default: () => [] },
  });
  const { t } = useI18n();
  const { createMessage } = useMessage();
  const launchColumns = [
    { width: 50, title: '序号', align: 'center', customRender: ({ index }) => index + 1 },
    { title: '目标表单值', dataIndex: 'targetField', key: 'targetField', width: 200 },
    { title: '的值设为', dataIndex: 'tips', key: 'tips', align: 'center', width: 80 },
    { title: '类型', dataIndex: 'sourceType', key: 'sourceType', width: 100 },
    { title: '值', dataIndex: 'sourceValue', key: 'sourceValue', width: 200 },
    { title: '操作', dataIndex: 'action', key: 'action', width: 50 },
  ];

  function addTransferItem() {
    props.dataForm.launchFlow.transferList.push({ targetField: '', targetFieldLabel: '', sourceType: 1, sourceValue: '', required: false });
  }
  function handleDelItem(index) {
    props.dataForm.launchFlow.transferList.splice(index, 1);
  }
  function onFlowIdChange(id, item) {
    if (!id) return handleNull();
    props.dataForm.launchFlow.flowName = item.fullName;
    props.dataForm.launchFlow.flowId = id;
    props.dataForm.launchFlow.transferList = [];
    props.dataForm.launchFlow.initiator = [];
    props.dataForm.launchFlow.launchPermission = item.visibleType || 1;
    getFlowFormFieldList(id);
  }
  function handleNull() {
    props.dataForm.launchFlow.flowName = '';
    props.dataForm.launchFlow.flowId = '';
    props.dataForm.launchFlow.formFieldList = [];
    props.dataForm.launchFlow.transferList = [];
    props.dataForm.launchFlow.initiator = [];
    props.dataForm.launchFlow.launchPermission = 1;
  }
  function getFlowFormFieldList(flowId) {
    if (!flowId) return;
    getFlowFormInfo(flowId).then(res => {
      let { type = 1, formData } = res.data;
      let formJson: any = {},
        fieldList: any[] = [];
      if (formData) formJson = JSON.parse(formData);
      fieldList = type == 2 ? transformFormJson(formJson) : formJson.fields;
      let list: any[] = transformFieldList(fieldList);
      props.dataForm.launchFlow.formFieldList = list.map(o => ({ ...o, label: o.fullName ? o.id + '(' + o.fullName + ')' : o.id }));
      updateTransferList();
    });
  }
  function updateTransferList() {
    const formFieldList = props.dataForm.launchFlow.formFieldList;
    let list: any = [];
    for (let i = 0; i < formFieldList.length; i++) {
      if (formFieldList[i].__config__?.required) {
        list.push({ targetField: formFieldList[i].id, targetFieldLabel: formFieldList[i].fullName, sourceType: 1, sourceValue: '', required: true });
      }
    }
    props.dataForm.launchFlow.transferList = list;
  }
  function transformFormJson(list) {
    const fieldList = list.map(o => ({
      __config__: {
        label: o.filedName,
        ceriKey: o.ceriKey || '',
        required: o.required || false,
      },
      __vModel__: o.filedId,
      multiple: o.multiple || false,
    }));
    return fieldList;
  }
  function transformFieldList(formFieldList) {
    let list: any[] = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__vModel__) {
        const isTableChild = parent && parent.__config__ && parent.__config__.ceriKey === 'table';
        const item: any = {
          id: isTableChild ? parent.__vModel__ + '-' + data.__vModel__ : data.__vModel__,
          fullName: isTableChild ? parent.__config__.label + '-' + data.__config__.label : data.__config__.label,
          ...omit(data, ['__config__', 'on', 'style', 'templateJson', 'addTableConf', 'tableConf', 'disabled']),
          __vModel__: isTableChild ? parent.__vModel__ + '-' + data.__vModel__ : data.__vModel__,
          __config__: {
            label: isTableChild ? parent.__config__.label + '-' + data.__config__.label : data.__config__.label,
            ...data.__config__,
          },
        };
        const config = item.__config__;
        if (dyOptionsList.includes(config.ceriKey) && config.dataType !== 'static') delete item.options;
        list.push(item);
      }
      if (Array.isArray(data)) data.forEach(d => loop(d, parent));
      if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
    };
    loop(formFieldList);
    return list;
  }
  function getSourceValueOptions(item) {
    if (!item.targetField) return [];
    return item.targetField.includes('-') ? props.allFormFieldsOptions : props.allFormFieldsOptions.filter(o => !o?.__config__?.isSubTable);
  }
  function getTargetOptions(index: number) {
    let ignoreOptions: any[] = [];
    for (let i = 0; i < props.dataForm.launchFlow.transferList.length; i++) {
      const e = props.dataForm.launchFlow.transferList[i];
      if (e.targetField && index !== i) ignoreOptions.push(e.targetField);
    }
    const list = props.dataForm.launchFlow.formFieldList.filter(
      o => !ignoreOptions.includes(o.id) && !['table', ...systemComponentsList].includes(o?.__config__?.ceriKey),
    );
    return list;
  }
</script>
