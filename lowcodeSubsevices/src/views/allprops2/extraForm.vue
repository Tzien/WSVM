<template>
  <BasicModal v-bind="$attrs" @register="registerModal" :title="title" width="600px" okText="确定" cancelText="取消" :minHeight="100" @ok="handleSubmit(0)" @continue="handleSubmit(1)" :closeFunc="onClose">
    <a-row class="dynamic-form  ">
      <a-form :colon="false" size="middle" layout="vertical" labelAlign="left" :model="dataForm" :rules="dataRule" ref="formRef">
        <a-row :gutter="15">
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="DanHang" :labelCol="{ style: { width: '100px' } }">
              <template #label>单行输入</template>
              <CeriInput v-model:value="dataForm.DanHang" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="DuoHang" :labelCol="{ style: { width: '100px' } }">
              <template #label>多行输入</template>
              <CeriTextarea v-model:value="dataForm.DuoHang" placeholder='请输入' allowClear :autoSize='{"minRows":4,"maxRows":4}' :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="ShuZi" :labelCol="{ style: { width: '100px' } }">
              <template #label>数字输入</template>
              <CeriInputNumber v-model:value="dataForm.ShuZi" placeholder='请输入' :controls=false :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="KaiGuan" :labelCol="{ style: { width: '100px' } }">
              <template #label>开关</template>
              <CeriSwitch v-model:value="dataForm.KaiGuan"  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="DanXuan" :labelCol="{ style: { width: '100px' } }">
              <template #label>单选框组</template>
              <CeriRadio v-model:value="dataForm.DanXuan" :options="optionsObj.DanXuanOptions" :fieldNames="optionsObj.DanXuanProps" direction="horizontal" optionType="default" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="DuoXuan" :labelCol="{ style: { width: '100px' } }">
              <template #label>多选框组</template>
              <CeriCheckbox v-model:value="dataForm.DuoXuan" :options="optionsObj.DuoXuanOptions" :fieldNames="optionsObj.DuoXuanProps" direction="horizontal" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="XiaLa" :labelCol="{ style: { width: '100px' } }">
              <template #label>下拉选择</template>
              <CeriSelect v-model:value="dataForm.XiaLa" placeholder='请选择' :options="optionsObj.XiaLaOptions" :fieldNames="optionsObj.XiaLaProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="JiLian" :labelCol="{ style: { width: '100px' } }">
              <template #label>级联选择</template>
              <CeriCascader v-model:value="dataForm.JiLian" placeholder='请选择' :options="optionsObj.JiLianOptions" :fieldNames="optionsObj.JiLianProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="RiQi" :labelCol="{ style: { width: '100px' } }">
              <template #label>日期选择</template>
              <CeriDatePicker v-model:value="dataForm.RiQi" placeholder='请选择' format="yyyy-MM-dd" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="ShiJian" :labelCol="{ style: { width: '100px' } }">
              <template #label>时间选择</template>
              <CeriTimePicker v-model:value="dataForm.ShiJian" placeholder='请选择' format="HH:mm:ss" allowClear :style='{"width":"100%"}'  />
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
      DanHang: undefined,
      DuoHang: undefined,
      ShuZi: undefined,
      KaiGuan: 0,
      DanXuan: undefined,
      DuoXuan: [],
      XiaLa: undefined,
      JiLian: [],
      RiQi: undefined,
      ShiJian: undefined,
      YanSe: undefined,
      PF: 0,
      HK: 0,
      FWB: undefined,
    },
    dataRule: {
    },
    optionsObj:{
      DanXuanOptions: [{'fullName':"选项一",'id':"1"},{'fullName':"选项二",'id':"2"}],
      DanXuanProps: {'label':'fullName','value':'id'},
      DuoXuanOptions: [{'fullName':"选项一",'id':"1"},{'fullName':"选项二",'id':"2"}],
      DuoXuanProps: {'label':'fullName','value':'id'},
      XiaLaOptions: [{'fullName':"选项一",'id':"1"},{'fullName':"选项二",'id':"2"}],
      XiaLaProps: {'label':'fullName','value':'id'},
      JiLianOptions: [{'id':"1",'fullName':"选项1",'children':[{"id":"2","fullName":"选项1-1"}]}],
      JiLianProps: {
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
