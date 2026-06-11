<template>
  <BasicModal v-bind="$attrs" @register="registerModal"  width="600px" :minHeight="100" okText="确定" cancelText="取消" @ok="handleSubmit" :closeFunc="onClose">
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
            <a-form-item name="DictType" :labelCol="{ style: { width: '100px' } }">
              <template #label>代码管理号</template>
              <CeriInput v-model:value="dataForm.DictType" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="12" class="ant-col-item" >
            <a-form-item name="DictBaseInfoId" :labelCol="{ style: { width: '100px' } }">
              <template #label>钢种代码</template>
              <CeriInput v-model:value="dataForm.DictBaseInfoId" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="12" class="ant-col-item" >
            <a-form-item name="FieldDecimal" :labelCol="{ style: { width: '100px' } }">
              <template #label>喷号码</template>
              <CeriInput v-model:value="dataForm.FieldDecimal" placeholder='请输入' allowClear :style='{"width":"100%"}'  :showCount='false'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="BelongSystem" :labelCol="{ style: { width: '100px' } }">
              <template #label>钢种分类</template>
              <CeriSelect v-model:value="dataForm.BelongSystem" placeholder='请选择' :options="optionsObj.BelongSystemOptions" :fieldNames="optionsObj.BelongSystemProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="AllowEdit" :labelCol="{ style: { width: '100px' } }">
              <template #label>是否应用</template>
              <CeriSelect v-model:value="dataForm.AllowEdit" placeholder='请选择' :options="optionsObj.AllowEditOptions" :fieldNames="optionsObj.AllowEditProps" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>插入时间</template>
              <CeriTimePicker v-model:value="dataForm.CreateTime" placeholder='请选择' format="HH:mm:ss" allowClear :style='{"width":"100%"}'  />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateId" :labelCol="{ style: { width: '100px' } }">
              <template #label>插入人员</template>
              <CeriUserSelect v-model:value="dataForm.CreateId" placeholder='请选择' selectType="all" allowClear :style='{"width":"100%"}' @labelChange="(val) => (dataForm.CreateName = val)" />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Remark" :labelCol="{ style: { width: '100px' } }">
              <template #label>备注</template>
              <CeriTextarea v-model:value="dataForm.Remark" placeholder='请输入' allowClear :autoSize='{"minRows":4,"maxRows":4}' :style='{"width":"100%"}'  :showCount='false'  />
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
  const userInfo = userStore.getUserInfo();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const [registerModal, { openModal, setModalProps }] = useModal();
  const formRef = ref<FormInstance>();
  const selectModal = ref(null);
  const state = reactive<State>({
    dataForm: {
      id: '',
      DictType: undefined,
      DictBaseInfoId: undefined,
      FieldDecimal: undefined,
      BelongSystem: undefined,
      AllowEdit: undefined,
      CreateTime: dayjs().format('HH:mm:ss'),
      CreateId: '',
      CreateName: undefined,
      Remark: undefined,
    },
    dataRule: {
      BelongSystem: [
        {
          required: true,
          message: '请输入钢种分类',
          trigger: 'change',
        },
      ],
      AllowEdit: [
        {
          required: true,
          message: '请输入是否应用',
          trigger: 'change',
        },
      ],
    },
    optionsObj:{
      BelongSystemOptions: [{'fullName':"低合金高强度结构钢",'id':"1"},{'fullName':"优质碳素结构钢",'id':"2"},{'fullName':"其他钢种",'id':"3"}],
      BelongSystemProps: {'label':'fullName','value':'id'},
      AllowEditOptions: [{'fullName':"是",'id':"1"},{'fullName':"否",'id':"2"}],
      AllowEditProps: {'label':'fullName','value':'id'},
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

  function init(data) {
    state.submitType = 0;
    state.showContinueBtn = true;
    state.title = !data.id ? '新增' : '编辑';
    state.continueText = !data.id ? '确定并新增' : '确定并继续';
    setFormProps({ continueLoading: false });
    state.dataForm.id = data.id;
    openModal();
    state.allList = data.allList;
    state.currIndex = state.allList.length && data.id ? state.allList.findIndex((item) => item.id === data.id) : 0;
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
        DictType: undefined,
        DictBaseInfoId: undefined,
        FieldDecimal: undefined,
        BelongSystem: undefined,
        AllowEdit: undefined,
        CreateTime: dayjs().format('HH:mm:ss'),
        CreateId: userInfo?.userid ? userInfo.userid : '',
        CreateName: undefined,
        Remark: undefined,
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
      const mapped: any = { id: raw.id };
      Object.keys(raw || {}).forEach((k) => {
        if (!k) return;
        if (k === 'id') return;
        const pascalKey = k.replace(/^[a-z]/, (s) => s.toUpperCase());
        mapped[pascalKey] = raw[k];
      });
      if (mapped.CreateTime) {
        mapped._CreateTimeDate = dayjs(mapped.CreateTime).format('YYYY-MM-DD');
        mapped.CreateTime = dayjs(mapped.CreateTime).format('HH:mm:ss');
      }
      Object.assign(state.dataForm, mapped);
      changeLoading(false);
    });
  }
  async function handleSubmit() {
    try {
      const values = await getForm()?.validate();
      if (!values) return;
      setFormProps({ confirmLoading: true });
      const formMethod = state.dataForm.id ? update : create;
      const payload: any = JSON.parse(JSON.stringify(state.dataForm || {}));
      if (typeof payload.CreateTime === 'string' && /^\d{2}:\d{2}:\d{2}$/.test(payload.CreateTime)) {
        const datePart = payload._CreateTimeDate || dayjs().format('YYYY-MM-DD');
        payload.CreateTime = `${datePart}T${payload.CreateTime}`;
      }
      if (payload._CreateTimeDate) delete payload._CreateTimeDate;
      formMethod(payload)
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
    const id = state.allList[state.currIndex].id;
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
      let item = {...state.tableRows[state.currVmodel],...data[i],jnpfId:buildUUID()}
      state.dataForm[state.currVmodel].push(cloneDeep(item));
      if(state[state.currVmodel+'DefaultExpandAll']) state[state.currVmodel+'innerActiveKey'].push(item.jnpfId);
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
