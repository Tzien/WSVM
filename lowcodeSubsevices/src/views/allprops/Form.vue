<template>
  <BasicModal v-bind="$attrs" @register="registerModal"  width="800px" :minHeight="100" okText="确定" cancelText="取消" @ok="handleSubmit" :closeFunc="onClose">
    <template #title>
	    <a-space :size="10">
            <div class="text-16px font-medium">{{ title }}</div>
			    <a-space-compact size="small" block v-if="dataForm.id">
				    <a-tooltip :title="t('common.prevRecord')">
					    <a-button size="small" :disabled="getPrevDisabled" @click="handlePrev">
						    <i class="icon-ym icon-ym-caret-left text-10px"></i>
					    </a-button>
				    </a-tooltip>
				    <a-tooltip :title="t('common.nextRecord')">
					    <a-button size="small" :disabled="getNextDisabled" @click="handleNext">
						    <i class="icon-ym icon-ym-caret-right text-10px"></i>
					    </a-button>
				    </a-tooltip>
			    </a-space-compact>
	    </a-space>
    </template>
	    <template #insertToolbar>
           <div class="float-left mt-5px">
			   <CeriCheckboxSingle v-model:value="submitType" :label="continueText" />
           </div>
        </template>
    <a-row class="dynamic-form ">
      <a-form :colon="false" layout="horizontal" labelAlign="right"  :labelCol="{ style: { width: '100px' } }" :model="dataForm" :rules="dataRule" ref="formRef">
        <a-row :gutter="15">
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Name" :labelCol="{ style: { width: '100px' } }">
              <template #label>姓名</template>
              <CeriInput v-model:value="dataForm.Name" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Sort" :labelCol="{ style: { width: '100px' } }">
              <template #label>排序</template>
              <CeriInputNumber v-model:value="dataForm.Sort" placeholder='请输入' :controls=false :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Enabled" :labelCol="{ style: { width: '100px' } }">
              <template #label>是否启用</template>
              <CeriSwitch v-model:value="dataForm.Enabled"  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Remark" :labelCol="{ style: { width: '100px' } }">
              <template #label>备注</template>
              <CeriTextarea v-model:value="dataForm.Remark" placeholder='请输入' allowClear :autoSize='{"minRows":4,"maxRows":4}' :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="IsDeleted" :labelCol="{ style: { width: '100px' } }">
              <template #label>是否删除</template>
              <CeriRadio v-model:value="dataForm.IsDeleted" :options="optionsObj.IsDeletedOptions" :fieldNames="optionsObj.IsDeletedProps" direction="horizontal" optionType="default" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Text" :labelCol="{ style: { width: '100px' } }">
              <template #label>距离（米）</template>
              <CeriCheckbox v-model:value="dataForm.Text" :options="optionsObj.TextOptions" :fieldNames="optionsObj.TextProps" direction="horizontal" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Enmu" :labelCol="{ style: { width: '100px' } }">
              <template #label>运动类型</template>
              <CeriSelect v-model:value="dataForm.Enmu" placeholder='请选择' :options="optionsObj.EnmuOptions" :fieldNames="optionsObj.EnmuProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>创建时间</template>
              <CeriDatePicker v-model:value="dataForm.CreateTime" placeholder='请选择' format="yyyy-MM-dd" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="LastLoginTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>最后登录时间</template>
              <CeriTimePicker v-model:value="dataForm.LastLoginTime" placeholder='请选择' format="HH:mm:ss" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Color" :labelCol="{ style: { width: '100px' } }">
              <template #label>颜色RGB</template>
              <CeriColorPicker v-model:value="dataForm.Color" color-format="rgb"  />
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
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="FWB" :labelCol="{ style: { width: '100px' } }">
              <template #label>富文本</template>
              <CeriEditor v-model:value="dataForm.FWB" :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-row>
  </BasicModal>
    <SelectModal :config="state.currTableConf" :formData="state.dataForm" ref="selectModal" @select="addForSelect"/>
</template>
<script lang="ts" setup>
  import { create, update, getInfo } from './helper/api';
  import { reactive, toRefs, nextTick, ref, unref, computed, toRaw, inject } from 'vue';
  import { BasicModal, useModal } from '@/components/Modal';
  import SelectModal from '@/components/CommonModal/src/SelectModal.vue';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n'
  import { useUserStore } from '@/store/user';
  import type { FormInstance } from 'ant-design-vue';
  import { thousandsFormat } from '@/utils/ceri';
  import { getTimeUnit, getDateTimeUnit } from '@/utils/ceri';
  import dayjs from 'dayjs';
  import { cloneDeep } from 'lodash-es';
  import { buildUUID } from '@/utils/uuid';
  import { CaretRightOutlined } from '@ant-design/icons-vue';
  import { systemComponentsList } from '@/components/FormGenerator/src/helper/config';
  import { formatDate } from '@/utils/dateUtils';

  interface State {
    dataForm: any;
    dataRule: any;
    optionsObj: any;
	currVmodel:any;
	currTableConf:any;
	addTableConf: any;
	tableRows: any;
    title: string;
    continueText: string;
    allList: any[];
    currIndex: number;
    isContinue: boolean;
    submitType: number;
    showContinueBtn: boolean;
  }

  const emit = defineEmits(['reload']);
  const getLeftTreeActiveInfo: (() => any) | null = inject('getLeftTreeActiveInfo', null);
  const userStore = useUserStore();
  const userInfo = userStore.getUserInfo;
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const [registerModal, { openModal, setModalProps }] = useModal();
  const formRef = ref<FormInstance>();
  const selectModal = ref(null);
  const state = reactive<State>({
    dataForm: {
      id: '',
      Name: undefined,
      Sort: undefined,
      Enabled: 1,
      Remark: undefined,
      IsDeleted: '0',
      Text: [],
      Enmu: undefined,
      CreateTime: dayjs(new Date()).startOf(getDateTimeUnit('yyyy-MM-dd')).valueOf(),
      LastLoginTime: dayjs().format('HH:mm:ss'),
      Color: 'rgb(0, 0, 0)',
      PF: 0,
      HK: 0,
      FWB: undefined,
    },
    dataRule: {
    },
    optionsObj:{
      IsDeletedOptions: [{'fullName':"是",'id':"1"},{'fullName':"否",'id':"0"}],
      IsDeletedProps: {'label':'fullName','value':'id'},
      TextOptions: [{'fullName':"超过50",'id':"1"},{'fullName':"小于200",'id':"2"}],
      TextProps: {'label':'fullName','value':'id'},
      EnmuOptions: [{'fullName':"走路",'id':"1"},{'fullName':"骑车",'id':"2"}],
      EnmuProps: {'label':'fullName','value':'id'},
    },
    currVmodel: '',
    currTableConf:{},
    tableRows: {
},
    addTableConf:{
    },
    title: '',
    continueText: '',
    allList: [],
    currIndex: 0,
    isContinue: false,
    submitType: 0,
    showContinueBtn: true,
  });
  const { title, continueText, showContinueBtn, dataForm,submitType,dataRule, optionsObj } = toRefs(state);

  const getPrevDisabled = computed(() => state.currIndex === 0);
  const getNextDisabled = computed(() => state.currIndex === state.allList.length - 1);

  defineExpose({ init });

  function getRowId(row: any) {
    return row?.id ?? row?.Id;
  }

  function getFormFieldKey(key: string) {
    const matchedKey = Object.keys(state.dataForm || {}).find((item) => item.toLowerCase() === key.toLowerCase());
    return matchedKey || key.replace(/^[a-z]/, (s) => s.toUpperCase());
  }

  function normalizeOptionValue(key: string, value: any) {
    const options = state.optionsObj?.[`${key}Options`];
    if (!Array.isArray(options) || value === undefined || value === null) return value;
    const props = state.optionsObj?.[`${key}Props`] || {};
    const valueKey = props.value || 'id';
    const normalize = (val: any) => {
      const item = options.find((option: any) => String(option?.[valueKey]) === String(val));
      return item ? item[valueKey] : val;
    };
    return Array.isArray(value) ? value.map(normalize) : normalize(value);
  }

  function init(data) {
    const id = data.id ?? data.Id;
    state.submitType = 0;
    state.showContinueBtn = true;
    state.title = !id ? '新增' : '编辑';
    state.continueText = !id ? '确定并新增' : '确定并继续';
    setFormProps({ continueLoading: false });
    state.dataForm.id = id;
    openModal();
    state.allList = data.allList;
    state.currIndex = state.allList.length && id ? state.allList.findIndex((item) => getRowId(item) === id) : 0;
    if (state.currIndex < 0) state.currIndex = 0;
    nextTick(() => {
      getForm().resetFields();
      setTimeout(initData, 0);
    });
  }
  function initData() {
    changeLoading(true);
    if (state.dataForm.id) {
      getData(state.dataForm.id);
    } else {
      // 设置默认值
      state.dataForm={
        Name: undefined,
        Sort: undefined,
        Enabled: 1,
        Remark: undefined,
        IsDeleted: '0',
        Text: [],
        Enmu: undefined,
        CreateTime: dayjs(new Date()).startOf(getDateTimeUnit('yyyy-MM-dd')).valueOf(),
        LastLoginTime: dayjs().format('HH:mm:ss'),
        Color: 'rgb(0, 0, 0)',
        PF: 0,
        HK: 0,
        FWB: undefined,
      };
      if (getLeftTreeActiveInfo) state.dataForm = {...state.dataForm, ...(getLeftTreeActiveInfo() || {}) };
      changeLoading(false);
    }
  }
  function getForm() {
    const form = unref(formRef);
    if (!form) {
      throw new Error('form is null!');
    }
    return form;
  }
function getData(id) {
  getInfo(id).then((res) => {
    const raw = res?.data || {};
    const mapped: any = { id: raw.id ?? raw.Id ?? id };
    if (mapped.Id === undefined) {
      mapped.Id = raw.Id ?? raw.id;
    }
    Object.keys(raw || {}).forEach((k) => {
      if (!k) return;
      if (k === 'id') return;
      const fieldKey = getFormFieldKey(k);
      mapped[fieldKey] = normalizeOptionValue(fieldKey, raw[k]);
    });
    if (mapped.CreateTime) mapped.CreateTime = dayjs(mapped.CreateTime).valueOf();
    Object.assign(state.dataForm, mapped);

    changeLoading(false);
  });
}
  function buildSubmitData() {
    const data = cloneDeep(state.dataForm);
    if (data.CreateTime || data.CreateTime === 0) data.CreateTime = formatDate(!isNaN(Number(data.CreateTime)) ? Number(data.CreateTime) : data.CreateTime);
    if (!data.id) delete data.id;
    return data;
  }
  async function handleSubmit() {
    try {
      const values = await getForm()?.validate();
      if (!values) return;
      setFormProps({ confirmLoading: true });
      const formMethod = state.dataForm.id ? update : create;
      formMethod(buildSubmitData())
        .then((res) => {
          createMessage.success(res.msg);
          setFormProps({ confirmLoading: false });
          if (state.submitType == 1) {
            initData();
            state.isContinue = true;
          } else {
            setFormProps({ open: false });
            emit('reload');
          }
        })
        .catch(() => {
          setFormProps({ confirmLoading: false });
        });
    } catch (_) {}
  }
  function handlePrev() {
    state.currIndex--;
    handleGetNewInfo();
  }
  function handleNext() {
    state.currIndex++;
    handleGetNewInfo();
  }
  function handleGetNewInfo() {
    changeLoading(true);
    getForm().resetFields();
    const id = getRowId(state.allList[state.currIndex]);
    getData(id);
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
  function openSelectDialog(key,value) {
    state.currTableConf = state.addTableConf[key+'List'+value]
    state.currVmodel = key
    nextTick(() => {
      (selectModal.value as any)?.openSelectModal();
    })
  }
  function addForSelect(data) {
    for (let i = 0; i < data.length; i++) {
      let item = {...state.tableRows[state.currVmodel],...data[i],ceriId:buildUUID()}
      state.dataForm[state.currVmodel].push(cloneDeep(item));
      if(state[state.currVmodel+'DefaultExpandAll']) state[state.currVmodel+'innerActiveKey'].push(item.ceriId);
    }
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
