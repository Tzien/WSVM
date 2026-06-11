<template>
  <BasicModal v-bind="$attrs" width="600px" class="CERI-add-detail-extra-Modal" @register="registerModal" :title="getTitle" @ok="handleSubmit" destroyOnClose>
    <a-form :colon="false" :labelCol="{ style: { width: '100px' } }" :model="dataForm" :rules="formRules" ref="formElRef">
      <ceri-group-title content="设置关联查询关系" :bordered="false" />
      <a-form-item label="页签名称" name="fullName">
        <ceri-input v-model:value="dataForm.fullName" placeholder="请输入" />
      </a-form-item>
      <a-form-item label="页签对象" name="targetFormId">
        <ceri-tree-select v-model:value="dataForm.targetFormId" :options="targetFormIdOptions" lastLevel @change="onTargetFormIdChange" />
      </a-form-item>
      <a-form-item label="主表字段" name="currentField">
        <ceri-select v-model:value="dataForm.currentField" :options="formFieldsOptions" :fieldNames="{ options: 'options1' }" />
      </a-form-item>
      <a-form-item label="页签对象字段" name="targetField">
        <ceri-select
          v-model:value="dataForm.targetField"
          :options="targetFieldOptions"
          :fieldNames="{ options: 'options1' }"
          @dropdownVisibleChange="visibleChange" />
      </a-form-item>
      <template v-if="dataForm.webType != 4">
        <ceri-group-title content="设置字段映射关系" :bordered="false" />
        <a-table :data-source="dataForm.formOptions" :columns="formOptionsColumns" size="small" :pagination="false">
          <template #bodyCell="{ column, record, index }">
            <template v-if="column.key === 'currentField'">
              <ceri-select
                class="!w-150px"
                v-model:value="record.currentField"
                :options="formFieldsOptions"
                :fieldNames="{ options: 'options1' }"
                allowClear
                showSearch />
            </template>
            <template v-if="column.key === 'type'">赋值给</template>
            <template v-if="column.key === 'targetField'">
              <ceri-select
                class="!w-150px"
                v-model:value="record.targetField"
                :options="targetFieldOptions"
                :fieldNames="{ options: 'options1' }"
                showSearch
                @dropdownVisibleChange="visibleChange" />
            </template>
            <template v-if="column.key === 'action'">
              <a-button class="action-btn" type="link" color="error" @click="handleDelItem(index)" size="small">删除</a-button>
            </template>
          </template>
          <template #emptyText>
            <p class="leading-60px">暂无数据</p>
          </template>
        </a-table>
        <div class="table-add-action mb-20px" @click="handleAddItem()">
          <a-button type="link" preIcon="icon-ym icon-ym-btn-add">{{ t('common.add2Text') }}</a-button>
        </div>
      </template>
    </a-form>
  </BasicModal>
</template>
<script lang="ts" setup>
  import { reactive, toRefs, ref, computed } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n';
  import { noAllowSelectList } from '../helper/config';
  import { getMenuSelectorFormTree } from '@/api/system/menu';
  import { getConfigDataByMenuId } from '@/api/onlineDev/visualDev';
  import type { FormInstance } from 'ant-design-vue';
  import { getDataInterfaceInfo } from '@/api/systemData/dataInterface';

  interface State {
    dataForm: any;
    formRules: any;
    formFieldsOptions: any[];
    targetFieldOptions: any[];
    targetFormIdOptions: any[];
  }

  const state = reactive<State>({
    dataForm: {
      fullName: '',
      targetFormId: '',
      currentField: '',
      targetField: '',
      formOptions: [],
    },
    formRules: {
      fullName: [{ required: true, message: '必填', trigger: 'change' }],
      targetFormId: [{ required: true, message: '必填', trigger: 'change' }],
      currentField: [{ required: true, message: '必填', trigger: 'change' }],
      targetField: [{ required: true, message: '必填', trigger: 'change' }],
    },
    formFieldsOptions: [],
    targetFieldOptions: [],
    targetFormIdOptions: [],
  });
  const { dataForm, formRules, formFieldsOptions, targetFieldOptions, targetFormIdOptions } = toRefs(state);
  const { t } = useI18n();
  const { createMessage } = useMessage();
  const emit = defineEmits(['register', 'confirm']);
  const formElRef = ref<FormInstance>();
  const [registerModal, { closeModal }] = useModalInner(init);
  const formOptionsColumns = [
    { width: 50, title: '序号', align: 'center', customRender: ({ index }) => index + 1 },
    { title: '主表字段', dataIndex: 'currentField', key: 'currentField', width: 160 },
    { title: '赋值给', dataIndex: 'type', key: 'type', align: 'center', width: 70 },
    { title: '页签对象字段', dataIndex: 'targetField', key: 'targetField', width: 160 },
    { title: '操作', dataIndex: 'action', key: 'action', width: 50 },
  ];
  const noAllowFormFieldsList = noAllowSelectList.filter(o => o != 'billRule');

  const getTitle = computed(() => (!state.dataForm.id ? t('common.addText') : t('common.editText')));

  function init(data) {
    resetData();
    if (data.data) state.dataForm = { ...state.dataForm, ...data.data };
    getFormFieldsOptions(data.drawingList || []);
    getTargetFormOptions();
    getFormFieldList(state.dataForm.targetFormId);
  }
  function getFormFieldsOptions(drawingList) {
    let list: any[] = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__config__ && data.__config__?.ceriKey !== 'table' && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
      if (Array.isArray(data)) data.forEach(d => loop(d, parent));
      if (data.__vModel__ && !noAllowFormFieldsList.includes(data.__config__.ceriKey)) {
        list.push({
          id: data.__vModel__,
          fullName: data.__config__.label,
        });
      }
    };
    loop(drawingList);
    state.formFieldsOptions = list;
  }
  function getTargetFormOptions() {
    getMenuSelectorFormTree().then(res => {
      state.targetFormIdOptions = res.data || [];
    });
  }
  function handleAddItem() {
    (state.dataForm.formOptions as any).push({
      currentField: '',
      targetField: '',
    });
  }
  function handleDelItem(index) {
    state.dataForm.formOptions.splice(index, 1);
  }
  function onTargetFormIdChange(id, item) {
    state.dataForm.formOptions = [];
    state.dataForm.targetField = '';
    state.targetFieldOptions = [];
    if (!id) return handleNull();
    state.dataForm.TargetFormName = item.fullName;
    getFormFieldList(id);
  }
  function getFormFieldList(menuId) {
    if (!menuId) return;
    getConfigDataByMenuId({ menuId }).then(res => {
      const { formData, webType, interfaceId } = res.data;
      state.dataForm.webType = webType;
      if (webType == 4) {
        getDataInterfaceInfo(interfaceId).then(res => {
          const data = res.data;
          if (data.hasPage !== 1) {
            const fieldJson = data.fieldJson ? JSON.parse(data.fieldJson) : [];
            state.targetFieldOptions = fieldJson.map(o => ({ id: o.defaultValue, fullName: o.field }));
          } else {
            const fieldJson = data.parameterJson ? JSON.parse(data.parameterJson) : [];
            state.targetFieldOptions = fieldJson.map(o => ({ id: o.field, fullName: o.fieldName || o.field }));
          }
        });
      } else {
        let formJson: any = {},
          fieldList: any = [];
        if (formData) formJson = JSON.parse(formData);
        fieldList = formJson.fields || [];
        state.targetFieldOptions = transformFieldList(fieldList);
      }
    });
  }
  function transformFieldList(formFieldList) {
    let list: any[] = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__vModel__) {
        const isTableChild = parent?.__config__?.ceriKey === 'table';
        if (!isTableChild && !noAllowFormFieldsList.includes(data?.__config__?.ceriKey)) {
          const item: any = {
            id: data.__vModel__,
            fullName: data.__config__.label,
          };
          list.push(item);
        }
      }
      if (Array.isArray(data)) data.forEach(d => loop(d, parent));
      if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
    };
    loop(formFieldList);
    return list;
  }
  function handleNull() {
    state.dataForm.TargetFormId = '';
    state.dataForm.TargetFormName = '';
    state.dataForm.targetFieldOptions = [];
  }
  function visibleChange(val) {
    if (val && !state.dataForm.targetFormId) createMessage.warning('请先选择页签对象');
  }
  function resetData() {
    state.formFieldsOptions = [];
    state.targetFieldOptions = [];
    state.targetFormIdOptions = [];
    state.dataForm = {
      fullName: '',
      targetFormId: '',
      currentField: '',
      targetField: '',
      formOptions: [],
    };
  }
  async function handleSubmit() {
    try {
      const values = await formElRef.value?.validate();
      if (!values) return;
      for (let i = 0; i < state.dataForm.formOptions.length; i++) {
        const e: any = state.dataForm.formOptions[i];
        if (!e.currentField) return createMessage.warning(`字段映射第${i + 1}行主表不能为空`);
        if (!e.targetField) return createMessage.warning(`字段映射第${i + 1}行页签对象字段不能为空`);
      }
      emit('confirm', { ...state.dataForm, ...values });
      closeModal();
    } catch (_) {}
  }
</script>
