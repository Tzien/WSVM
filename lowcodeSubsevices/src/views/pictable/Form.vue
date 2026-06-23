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
            <a-form-item name="PicURL" :labelCol="{ style: { width: '100px' } }">
              <template #label>图片上传</template>
              <CeriUploadImg v-model:value="dataForm.PicURL" pathType="defaultPath" sizeUnit="MB" :fileSize="10"  :sortRule='[]' timeFormat='YYYY' />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="FileURL" :labelCol="{ style: { width: '100px' } }">
              <template #label>文件上传</template>
              <CeriUploadFile v-model:value="dataForm.FileURL" buttonText='点击上传' pathType="defaultPath" :fileSize="10" sizeUnit="MB" :limit="9"  :sortRule='[]' timeFormat='YYYY' />
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
      PicURL: [],
      FileURL: [],
    },
    dataRule: {
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
  const { title, continueText, showContinueBtn, dataForm,submitType,dataRule } = toRefs(state);

  const getPrevDisabled = computed(() => state.currIndex === 0);
  const getNextDisabled = computed(() => state.currIndex === state.allList.length - 1);

  defineExpose({ init });

  function getRowId(row: any) {
    return row?.id ?? row?.id ?? row?.Id;
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
    const id = data.id ?? data.Id ?? data.id;
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
        PicURL: [],
        FileURL: [],
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
    Object.assign(state.dataForm, mapped);

    changeLoading(false);
  });
}
  function buildSubmitData() {
    const data = cloneDeep(state.dataForm);
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
