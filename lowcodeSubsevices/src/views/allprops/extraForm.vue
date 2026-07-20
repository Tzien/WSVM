<template>
  <BasicModal v-bind="$attrs" @register="registerModal" :title="title" width="600px" okText="确定" cancelText="取消" :minHeight="100" @ok="handleSubmit(0)" @continue="handleSubmit(1)" :closeFunc="onClose">
    <a-row class="dynamic-form  ">
      <a-form :colon="false" size="middle" layout="vertical" labelAlign="left" :model="dataForm" :rules="dataRule" ref="formRef">
        <a-row :gutter="15">
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Name" :labelCol="{ style: { width: '100px' } }">
              <template #label>单行输入</template>
              <CeriInput v-model:value="dataForm.Name" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Remark" :labelCol="{ style: { width: '100px' } }">
              <template #label>多行输入</template>
              <CeriTextarea v-model:value="dataForm.Remark" placeholder='请输入' allowClear :autoSize='{"minRows":4,"maxRows":4}' :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Sort" :labelCol="{ style: { width: '100px' } }">
              <template #label>数字输入</template>
              <CeriInputNumber v-model:value="dataForm.Sort" placeholder='请输入' :controls=false :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Enabled" :labelCol="{ style: { width: '100px' } }">
              <template #label>开关</template>
              <CeriSwitch v-model:value="dataForm.Enabled"  />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-row>
  </BasicModal>
</template>
<script lang="ts" setup>
  import { create, update, getInfo } from './helper/api';
  import { reactive, toRefs, nextTick, ref, unref, computed } from 'vue';
  import { BasicModal, useModal } from '@/components/Modal';
  import { JnpfRelationForm } from '@/components/Jnpf';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useUserStore } from '@/store/modules/user';
  import type { FormInstance } from 'ant-design-vue';
  import { thousandsFormat, getTimeUnit, getDateTimeUnit } from '@/utils/jnpf';
  import { getDictionaryDataSelector } from '@/api/systemData/dictionary';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  import dayjs from 'dayjs';

  interface State {
    dataForm: any;
    dataRule: any;
    isEdit: any;
    isContinue: boolean;
    title: string;
    allList: any[];
  }

  const emit = defineEmits(['reload']);
  const userStore = useUserStore();
  const userInfo = userStore.getUserInfo;
  const { createMessage } = useMessage();
  const { t } = useI18n();
  const [registerModal, { openModal, setModalProps }] = useModal();
  const formRef = ref<FormInstance>();
  const state = reactive<State>({
    dataForm: {
      id: '',
      Name: undefined,
      Remark: undefined,
      Sort: undefined,
      Enabled: 0,
    },
    dataRule: {
    },
    title: '',
    isContinue: false,
    isEdit: false,
    allList: [],
  });
  const { title, dataForm, dataRule } = toRefs(state);

  defineExpose({ init });

  function init(data) {
    state.isContinue = false;
    state.title = !data.id || data.id === 'jnpfAdd' ? '新增' : '编辑';
    setFormProps({ continueLoading: false });
    openModal();
    nextTick(() => {
      getForm().resetFields();
      state.dataForm = JSON.parse(JSON.stringify(data.formData));
      state.dataForm.id = !data.id || data.id === 'jnpfAdd' ? '' : data.id;
    });
  }
  function getForm() {
    const form = unref(formRef);
    if (!form) {
      throw new Error('form is null!');
    }
    return form;
  }
  async function handleSubmit() {
    try {
      const values = await getForm()?.validate();
      if (!values) return;
      setFormProps({ continueLoading: true });
      const formMethod = state.dataForm.id ? update : create;
      formMethod(state.dataForm)
        .then(res => {
          createMessage.success(res.msg);
          state.isContinue = true;
          setFormProps({ continueLoading: false });
          setFormProps({ open: false });
          emit('reload');
        })
        .catch(() => {
          setFormProps({ continueLoading: false });
        });
    } catch (_) {}
  }
  function setFormProps(data) {
    setModalProps(data);
  }
  function changeLoading(loading) {
    setModalProps({ loading });
  }
  async function onClose() {
    if (state.isContinue) emit('reload');
    return true;
  }
  function getParamList(templateJson, formData, index?) {
    for (let i = 0; i < templateJson.length; i++) {
      if (templateJson[i].relationField && templateJson[i].sourceType == 1) {
        //区分是否子表
        if (templateJson[i].relationField.includes('-')) {
          let tableVModel = templateJson[i].relationField.split('-')[0]
          let childVModel = templateJson[i].relationField.split('-')[1]
          templateJson[i].defaultValue = formData[tableVModel] && formData[tableVModel][index] && formData[tableVModel][index][childVModel] || ''
        } else {
          templateJson[i].defaultValue = formData[templateJson[i].relationField] || ''
        }
      }
    }
    return templateJson
  }
</script>
