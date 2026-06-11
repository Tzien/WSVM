<template>
  <BasicPopup
    v-bind="$attrs"
    @register="registerPopup"
    showOkBtn
    :okText="getOkText"
    :cancelText="getCancelText"
    destroyOnClose
    @ok="handleSubmit"
    :closeFunc="onClose"
    class="full-popup">
    <template #title>
      <a-space :size="10">
        <div class="text-16px font-medium">{{ title }}</div>
        <a-space-compact size="small" block v-if="getShowMoreBtn">
          <a-tooltip :title="t('common.prevRecord')">
            <a-button size="small" :loading="state.prevBtnLoading" :disabled="getPrevDisabled" @click="handlePrev">
              <i class="icon-ym icon-ym-caret-left text-10px"></i>
            </a-button>
          </a-tooltip>
          <a-tooltip :title="t('common.nextRecord')">
            <a-button size="small" :loading="state.nextBtnLoading" :disabled="getNextDisabled" @click="handleNext">
              <i class="icon-ym icon-ym-caret-right text-10px"></i>
            </a-button>
          </a-tooltip>
        </a-space-compact>
      </a-space>
    </template>
    <template #insertToolbar>
      <CeriCheckboxSingle v-model:value="submitType" :label="continueText" v-if="showContinueBtn" />
    </template>
    <div class="ceri-common-form-wrapper">
      <div class="ceri-common-form-wrapper__main p-10px" :style="{ margin: '0 auto', width: formConf.fullScreenWidth || '100%' }">
        <Parser ref="parserRef" :formConf="formConf" @submit="submitForm" :key="key" v-if="!loading" />
      </div>
      <FormExtraPanel v-bind="getFormExtraBind" v-if="state.dataForm.id && formConf.dataLog && !loading" :key="key" />
    </div>
  </BasicPopup>
  <BasicModal
    v-bind="$attrs"
    @register="registerModal"
    :width="formConf.generalWidth"
    :minHeight="100"
    :okText="getOkText"
    :cancelText="getCancelText"
    @ok="handleSubmit"
    :closeFunc="onClose">
    <template #title>
      <a-space :size="10">
        <div class="text-16px font-medium">{{ title }}</div>
        <a-space-compact size="small" block v-if="getShowMoreBtn">
          <a-tooltip :title="t('common.prevRecord')">
            <a-button size="small" :loading="state.prevBtnLoading" :disabled="getPrevDisabled" @click="handlePrev">
              <i class="icon-ym icon-ym-caret-left text-10px"></i>
            </a-button>
          </a-tooltip>
          <a-tooltip :title="t('common.nextRecord')">
            <a-button size="small" :loading="state.nextBtnLoading" :disabled="getNextDisabled" @click="handleNext">
              <i class="icon-ym icon-ym-caret-right text-10px"></i>
            </a-button>
          </a-tooltip>
        </a-space-compact>
      </a-space>
    </template>
    <template #insertFooter>
      <div class="float-left mt-5px" v-if="showContinueBtn">
        <CeriCheckboxSingle v-model:value="submitType" :label="continueText" />
      </div>
    </template>
    <Parser ref="parserRef" :formConf="formConf" @submit="submitForm" :key="key" v-if="!loading" />
  </BasicModal>
  <BasicDrawer
    v-bind="$attrs"
    @register="registerDrawer"
    :width="formConf.drawerWidth"
    showFooter
    :okText="getOkText"
    :cancelText="getCancelText"
    @ok="handleSubmit"
    :closeFunc="onClose">
    <template #title>
      <div class="flex justify-between items-center">
        <div class="text-16px font-medium">{{ title }}</div>
        <a-space-compact size="small" v-if="getShowMoreBtn">
          <a-tooltip :title="t('common.prevRecord')">
            <a-button size="small" :loading="state.prevBtnLoading" :disabled="getPrevDisabled" @click="handlePrev">
              <i class="icon-ym icon-ym-caret-left text-10px"></i>
            </a-button>
          </a-tooltip>
          <a-tooltip :title="t('common.nextRecord')">
            <a-button size="small" :loading="state.nextBtnLoading" :disabled="getNextDisabled" @click="handleNext">
              <i class="icon-ym icon-ym-caret-right text-10px"></i>
            </a-button>
          </a-tooltip>
        </a-space-compact>
      </div>
    </template>
    <template #insertFooter>
      <div class="float-left mt-5px" v-if="showContinueBtn">
        <CeriCheckboxSingle v-model:value="submitType" :label="continueText" />
      </div>
    </template>
    <div class="p-10px">
      <Parser ref="parserRef" :formConf="formConf" @submit="submitForm" :key="key" v-if="!loading" />
    </div>
  </BasicDrawer>
</template>
<script lang="ts" setup>
  import { createModel, updateModel, getModelInfo } from '@/api/onlineDev/visualDev';
  import { reactive, toRefs, nextTick, ref, unref, computed, inject } from 'vue';
  import { createAsyncComponent } from '@/utils/factory/createAsyncComponent';
  import { BasicPopup, usePopup } from '@/components/Popup';
  import { BasicModal, useModal } from '@/components/Modal';
  import { BasicDrawer, useDrawer } from '@/components/Drawer';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n';
  import { useUserStore } from '@/store/user';
  import { useGeneratorStore } from '@/store/generator';
  import { cloneDeep } from 'lodash-es';
  import dayjs from 'dayjs';
  import { getDateTimeUnit } from '@/utils/ceri';
  import FormExtraPanel from '@/components/FormExtraPanel/index.vue';

  interface State {
    formConf: any;
    defaultFormConf: any;
    formData: any;
    config: any;
    loading: boolean;
    prevBtnLoading: boolean;
    nextBtnLoading: boolean;
    key: number;
    dataForm: any;
    formOperates: any[];
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
  const generatorStore = useGeneratorStore();
  const { createMessage } = useMessage();
  const { t } = useI18n();
  const [registerPopup, { openPopup, setPopupProps }] = usePopup();
  const [registerModal, { openModal, setModalProps }] = useModal();
  const [registerDrawer, { openDrawer, setDrawerProps }] = useDrawer();
  const parserRef = ref<any>(null);
  const state = reactive<State>({
    formConf: {},
    defaultFormConf: {},
    formData: {},
    config: {},
    loading: true,
    prevBtnLoading: false,
    nextBtnLoading: false,
    key: +new Date(),
    dataForm: {
      id: '',
      data: '',
    },
    formOperates: [],
    title: '',
    continueText: '',
    allList: [],
    currIndex: 0,
    isContinue: false,
    submitType: 0,
    showContinueBtn: true,
  });
  const { title, formConf, key, loading, continueText, showContinueBtn, submitType } = toRefs(state);
  const Parser = createAsyncComponent(() => import('@/views/WebDesign/component/StepForm/src/components/Parser.vue'));

  const getPrevDisabled = computed(() => state.currIndex === 0);
  const getNextDisabled = computed(() => state.currIndex === state.allList.length - 1);
  const getShowMoreBtn = computed(() => state.config.id && state.config.showMoreBtn && state.formConf.hasConfirmAndAddBtn);
  const getOkText = computed(() => {
    const text = state.formConf.confirmButtonTextI18nCode
      ? t(state.formConf.confirmButtonTextI18nCode, state.formConf.confirmButtonText)
      : state.formConf.confirmButtonText;
    return text || t('common.okText');
  });
  const getCancelText = computed(() => {
    const text = state.formConf.cancelButtonTextI18nCode
      ? t(state.formConf.cancelButtonTextI18nCode, state.formConf.cancelButtonText)
      : state.formConf.cancelButtonText;
    return text || t('common.cancelText');
  });
  const getFormExtraBind = computed(() => ({ showLog: state.formConf.dataLog, modelId: state.config.modelId, formDataId: state.config.id }));

  defineExpose({ init });

  function fillFormData(form, data, isAdd) {
    const userInfo = userStore.getUserInfo;
    const currDate = new Date();
    const loop = (list, parent?) => {
      for (let i = 0; i < list.length; i++) {
        let item = list[i];
        if (item.__vModel__) {
          let val = data.hasOwnProperty(item.__vModel__) ? data[item.__vModel__] : item.__config__.defaultValue;
          if (!item.__config__.isSubTable) item.__config__.defaultValue = val;
          if (isAdd || item.__config__.isSubTable) {
            if (item.__config__.defaultCurrent) {
              if (item.__config__.ceriKey === 'datePicker') {
                item.__config__.defaultValue = dayjs(currDate).startOf(getDateTimeUnit(item.format)).valueOf();
              }
              if (item.__config__.ceriKey === 'timePicker') {
                item.__config__.defaultValue = dayjs(currDate).format(item.format || 'HH:mm:ss');
              }
              if (item.__config__.ceriKey === 'organizeSelect' && userInfo.organizeIdList?.length) {
                item.__config__.defaultValue = item.multiple ? [userInfo.organizeIdList] : userInfo.organizeIdList;
              }
              if (item.__config__.ceriKey === 'depSelect' && userInfo.departmentId) {
                item.__config__.defaultValue = item.multiple ? [userInfo.departmentId] : userInfo.departmentId;
              }
              if (item.__config__.ceriKey === 'userSelect' && userInfo.userId) {
                item.__config__.defaultValue = item.multiple ? [userInfo.userId] : userInfo.userId;
              }
              if (item.__config__.ceriKey === 'usersSelect' && userInfo.userId) {
                item.__config__.defaultValue = item.multiple ? [userInfo.userId + '--user'] : userInfo.userId + '--user';
              }
              if (item.__config__.ceriKey === 'posSelect' && userInfo.positionIds?.length) {
                item.__config__.defaultValue = item.multiple ? userInfo.positionIds.map(o => o.id) : userInfo.positionIds[0].id;
              }
              if (item.__config__.ceriKey === 'roleSelect' && userInfo.roleIds?.length) {
                item.__config__.defaultValue = item.multiple ? userInfo.roleIds : userInfo.roleIds[0];
              }
              if (item.__config__.ceriKey === 'groupSelect' && userInfo.groupIds?.length) {
                item.__config__.defaultValue = item.multiple ? userInfo.groupIds : userInfo.groupIds[0];
              }
              if (item.__config__.ceriKey === 'sign' && userInfo.signImg) {
                item.__config__.defaultValue = userInfo.signImg;
              }
            }
          }
          if (isAdd && !item.__config__.isSubTable && data.hasOwnProperty(item.__vModel__)) item.__config__.defaultValue = data[item.__vModel__];
          if (!state.config.isPreview && !state.config.isDataManage && state.config.useFormPermission) {
            let id = item.__config__.isSubTable ? parent.__vModel__ + '-' + item.__vModel__ : item.__vModel__;
            let noShow = true;
            if (state.formOperates && state.formOperates.length) {
              noShow = !state.formOperates.some(o => o.enCode === id);
            }
            noShow = item.__config__.noShow ? item.__config__.noShow : noShow;
            item.__config__.noShow = noShow;
          }
        }
        if (item.__config__ && item.__config__.children && Array.isArray(item.__config__.children)) {
          loop(item.__config__.children, item);
        }
      }
    };
    loop(form.fields);
    form.formData = data;
  }
  function init(data) {
    state.loading = true;
    state.submitType = 0;
    state.isContinue = false;
    state.prevBtnLoading = false;
    state.nextBtnLoading = false;
    state.title = !data.id || data.id === 'ceriAdd' ? t('common.add2Text') : t('common.editText');
    state.continueText = !data.id ? t('common.continueAndAddText') : t('common.continueText');
    state.config = data;
    state.defaultFormConf = cloneDeep(data.formConf);
    state.formConf = cloneDeep(state.defaultFormConf);
    state.showContinueBtn = !data.formData && state.formConf.hasConfirmAndAddBtn;
    state.dataForm.id = !data.id || data.id === 'ceriAdd' ? '' : data.id;
    getFormOperates();
    openForm();
    state.allList = data.allList;
    state.currIndex = state.allList.length && data.id ? state.allList.findIndex(item => item.id === data.id) : 0;
    nextTick(() => {
      if (!data.formData) return setTimeout(initData, 0);
      // 行内编辑
      setTimeout(() => {
        state.formData = { ...data.formData, id: state.dataForm.id };
        setFormValue();
      }, 0);
    });
  }
  function initData() {
    changeLoading(true);
    state.loading = true;
    if (state.config.id) {
      const extra = { modelId: state.config.modelId, id: state.config.id, type: 2 };
      generatorStore.setDynamicModelExtra(extra);
      getInfo(state.config.id);
    } else {
      generatorStore.setDynamicModelExtra({});
      state.formData = {};
      setFormValue(true);
    }
  }
  function getInfo(id) {
    getModelInfo(state.config.modelId, id, state.config.menuId).then(res => {
      state.dataForm = res.data || {};
      if (!state.dataForm.data) return;
      state.formData = { ...JSON.parse(state.dataForm.data), id: state.dataForm.id };
      setFormValue();
    });
  }
  function setFormValue(isAdd = false) {
    if (isAdd && getLeftTreeActiveInfo) state.formData = { ...(getLeftTreeActiveInfo() || {}) };
    state.formConf = cloneDeep(state.defaultFormConf);
    fillFormData(state.formConf, state.formData, isAdd);
    nextTick(() => {
      state.key = +new Date();
      state.loading = false;
      state.prevBtnLoading = false;
      state.nextBtnLoading = false;
      changeLoading(false);
    });
  }
  function getFormOperates() {
    if (state.config.isPreview || state.config.isDataManage || !state.config.useFormPermission) return;
    const permissionList = userStore.getPermissionList;
    const modelId = state.config.menuId;
    const list = permissionList.filter(o => o.modelId === modelId);
    state.formOperates = list[0] && list[0].form ? list[0].form : [];
  }
  function submitForm(data, callback) {
    if (!data) return;
    setFormProps({ confirmLoading: true });
    const formData = { ...state.formData, ...data };
    state.dataForm.data = JSON.stringify(formData);
    const formMethod = state.dataForm.id ? updateModel : createModel;
    formMethod(state.config.modelId, { ...state.dataForm, menuId: state.config.menuId })
      .then(res => {
        createMessage.success(res.msg);
        if (callback && typeof callback === 'function') callback();
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
  }
  function handleReset() {
    getParser().handleReset();
  }
  function handleSubmit() {
    if (state.config.isPreview) return createMessage.warning('功能预览不支持数据保存');
    getParser().handleSubmit();
  }
  function handlePrev() {
    state.currIndex--;
    // state.prevBtnLoading = true;
    handleGetNewInfo();
  }
  function handleNext() {
    state.currIndex++;
    // state.nextBtnLoading = true;
    handleGetNewInfo();
  }
  function handleGetNewInfo() {
    changeLoading(true);
    state.loading = true;
    handleReset();
    state.config.id = state.allList[state.currIndex].id;
    getInfo(state.config.id);
  }
  function getParser() {
    const parser = unref(parserRef);
    if (!parser) throw new Error('parser is null!');
    return parser;
  }
  function openForm() {
    if (state.formConf.popupType === 'fullScreen') return openPopup();
    if (state.formConf.popupType === 'drawer') return openDrawer();
    openModal();
  }
  function setFormProps(data) {
    if (state.formConf.popupType === 'fullScreen') return setPopupProps(data);
    if (state.formConf.popupType === 'drawer') return setDrawerProps(data);
    setModalProps(data);
  }
  function changeLoading(loading) {
    setFormProps({ loading });
  }
  async function onClose() {
    if (state.isContinue) emit('reload');
    return true;
  }
</script>
