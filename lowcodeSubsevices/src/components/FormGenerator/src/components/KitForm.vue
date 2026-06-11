<template>
  <BasicModal v-bind="$attrs" @register="registerModal" title="另存为模板" @ok="handleSubmit">
    <BasicForm @register="registerForm" />
  </BasicModal>
</template>
<script lang="ts" setup>
  import { create } from '@/api/system/kit';
  import { reactive } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { BasicForm, useForm } from '@/components/Form';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useBaseStore } from '@/store/modules/base';
  import formValidate from '@/utils/formValidate';

  interface State {
    dataForm: any;
  }

  const emit = defineEmits(['register', 'reload']);
  const state = reactive<State>({
    dataForm: {},
  });
  const [registerForm, { setFieldsValue, resetFields, validate, updateSchema }] = useForm({
    schemas: [
      {
        field: 'fullName',
        label: '模板名称',
        component: 'Input',
        componentProps: { placeholder: '请输入', maxlength: 100 },
        rules: [{ required: true, trigger: 'blur', message: '必填' }],
      },
      {
        field: 'enCode',
        label: '模板编码',
        component: 'Input',
        componentProps: { placeholder: '请输入', maxlength: 50 },
        rules: [
          { required: true, trigger: 'blur', message: '必填' },
          { validator: formValidate('enCode'), trigger: 'blur' },
        ],
      },
      {
        field: 'category',
        label: '模板分类',
        component: 'Select',
        componentProps: { placeholder: '请选择', showSearch: true },
        rules: [{ required: true, trigger: 'change', message: '必填' }],
      },
      {
        field: 'icon',
        label: '模板图标',
        component: 'IconPicker',
        rules: [{ required: true, trigger: 'change', message: '必填' }],
      },
      {
        field: 'sortCode',
        label: '模板排序',
        defaultValue: 0,
        component: 'InputNumber',
        componentProps: { min: 0, max: 999999 },
      },
      {
        field: 'enabledMark',
        label: '模板状态',
        component: 'Switch',
        defaultValue: 1,
      },
      {
        field: 'description',
        label: '模板说明',
        component: 'Textarea',
        componentProps: { placeholder: '请输入' },
      },
    ],
  });
  const [registerModal, { closeModal, changeOkLoading }] = useModalInner(init);
  const { createMessage } = useMessage();
  const baseStore = useBaseStore();

  function init(data = {}) {
    getOptions();
    resetFields();
    state.dataForm = data;
    setFieldsValue(data);
  }
  async function getOptions() {
    const typeRes = await baseStore.getDictionaryData('businessType');
    updateSchema({ field: 'category', componentProps: { options: typeRes } });
  }
  async function handleSubmit() {
    const values = await validate();
    if (!values) return;
    changeOkLoading(true);
    const query = { ...state.dataForm, ...values };
    create(query)
      .then(res => {
        createMessage.success(res.msg);
        changeOkLoading(false);
        closeModal();
        emit('reload');
      })
      .catch(() => {
        changeOkLoading(false);
      });
  }
</script>
