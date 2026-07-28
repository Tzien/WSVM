<template>
  <BasicModal v-bind="$attrs" @register="registerModal" :title="title" width="600px" okText="确定" cancelText="取消" :minHeight="100" @ok="handleSubmit(0)" @continue="handleSubmit(1)" :closeFunc="onClose">
    <a-row class="dynamic-form  ">
      <a-form :colon="false" size="middle" layout="vertical" labelAlign="left" :model="dataForm" :rules="dataRule" ref="formRef">
        <a-row :gutter="15">
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Enmu" :labelCol="{ style: { width: '100px' } }">
              <template #label>单选框组</template>
              <CeriRadio v-model:value="dataForm.Enmu" :options="optionsObj.EnmuOptions" :fieldNames="optionsObj.EnmuProps" direction="horizontal" optionType="default" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Text" :labelCol="{ style: { width: '100px' } }">
              <template #label>多选框组</template>
              <CeriCheckbox v-model:value="dataForm.Text" :options="optionsObj.TextOptions" :fieldNames="optionsObj.TextProps" direction="horizontal" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Enabled" :labelCol="{ style: { width: '100px' } }">
              <template #label>下拉选择</template>
              <CeriSelect v-model:value="dataForm.Enabled" placeholder='请选择' :options="optionsObj.EnabledOptions" :fieldNames="optionsObj.EnabledProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Name" :labelCol="{ style: { width: '100px' } }">
              <template #label>级联选择</template>
              <CeriCascader v-model:value="dataForm.Name" placeholder='请选择' :options="optionsObj.NameOptions" :fieldNames="optionsObj.NameProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>日期选择</template>
              <CeriDatePicker v-model:value="dataForm.CreateTime" placeholder='请选择' format="yyyy-MM-dd" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="LastLoginTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>时间选择</template>
              <CeriTimePicker v-model:value="dataForm.LastLoginTime" placeholder='请选择' format="HH:mm:ss" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="PF" :labelCol="{ style: { width: '100px' } }">
              <template #label>评分</template>
              <CeriRate v-model:value="dataForm.PF" :count="5"  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="HK" :labelCol="{ style: { width: '100px' } }">
              <template #label>滑块</template>
              <CeriSlider v-model:value="dataForm.HK" :min="0" :max="100" :step="1.0" :style='{"width":"100%"}'  />
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
  import { CeriRelationForm } from '@/components/CeriOS';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useUserStore } from '@/store/user';
  import type { FormInstance } from 'ant-design-vue';
  import { thousandsFormat, getTimeUnit, getDateTimeUnit } from '@/utils/ceri';
  import { getDictionaryDataSelector } from '@/api/systemData/dictionary';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  import dayjs from 'dayjs';

  interface State {
    dataForm: any;
    dataRule: any;
    optionsObj: any;
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
      Enmu: undefined,
      Text: [],
      Enabled: undefined,
      Name: [],
      CreateTime: undefined,
      LastLoginTime: undefined,
      Color: undefined,
      PF: 0,
      HK: 0,
      FWB: undefined,
    },
    dataRule: {
    },
    optionsObj:{
      EnmuOptions: [{'fullName':"单选一",'id':"1"},{'fullName':"单选二",'id':"2"}],
      EnmuProps: {'label':'fullName','value':'id'},
      TextOptions: [{'fullName':"多选一",'id':"1"},{'fullName':"多选二",'id':"2"}],
      TextProps: {'label':'fullName','value':'id'},
      EnabledOptions: [{'fullName':"是",'id':"1"},{'fullName':"否",'id':"2"}],
      EnabledProps: {'label':'fullName','value':'id'},
      NameOptions: [{'id':"1",'fullName':"级联1",'children':[{"fullName":"级联1-1","id":"11"}]},{'fullName':"级联2",'id':"2",'children':[{"fullName":"级联2-1","id":"21"}]}],
      NameProps: {
  "label": "fullName",
  "value": "id",
  "children": "children"
},
    },
    title: '',
    isContinue: false,
    isEdit: false,
    allList: [],
  });
  const { title, dataForm, dataRule, optionsObj } = toRefs(state);

  defineExpose({ init });

  function init(data) {
    state.isContinue = false;
    state.title = !data.id || data.id === 'ceriAdd' ? '新增' : '编辑';
    setFormProps({ continueLoading: false });
    openModal();
    nextTick(() => {
      getForm().resetFields();
      state.dataForm = JSON.parse(JSON.stringify(data.formData));
      state.dataForm.id = !data.id || data.id === 'ceriAdd' ? '' : data.id;
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
