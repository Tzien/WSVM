<template>
  <BasicPopup v-bind="$attrs" @register="registerPopup" :title="title" destroyOnClose :closeFunc="onClose" class="full-popup">
    <template #insertToolbar>
      <a-button
        class="ml-10px"
        v-for="item in state.customBtns"
        :key="item.value"
        type="primary"
        :preIcon="item.actionConfig?.btnIcon"
        v-if="!loading"
        @click="customBtnsHandle(item)">
        {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
      </a-button>
      <a-button class="ml-10px" type="primary" @click="handlePrint" v-if="formConf.hasPrintBtn && formConf.printId">{{ getPrintText }}</a-button>
    </template>
    <div class="ceri-common-form-wrapper">
      <div class="ceri-common-form-wrapper__main" :style="{ margin: '0 auto', width: formConf.fullScreenWidth || '100%' }">
        <template v-if="!loading && extraList.length && !hideExtra">
          <a-tabs v-model:activeKey="extraActiveKey" class="ceri-content-wrapper-tabs" destroyInactiveTabPane @change="onTabChange">
            <a-tab-pane v-for="(item, index) in extraList" :key="index" :tab="item.fullName"></a-tab-pane>
          </a-tabs>
          <div class="ceri-content-detail-extra">
            <Parser
              class="p-10px !pt-0px"
              ref="parserRef"
              :formConf="formConf"
              :formData="formData"
              @toDetail="toDetail"
              :key="key"
              v-if="extraActiveKey == 0" />
            <div class="h-full" v-loading="extraLoading" v-else>
              <ExtraList
                ref="extraListRef"
                :config="extraList[extraActiveKey]"
                :detailFormData="formData"
                :key="extraKey"
                @openDetail="handleOpenDetail"
                @openForm="handleOpenForm"
                v-if="extraList[extraActiveKey]?.extraConfig && !extraLoading" />
              <ceri-empty class="extra-empty" v-if="!extraList[extraActiveKey]?.extraConfig && !extraLoading" />
            </div>
          </div>
        </template>
        <Parser
          class="p-10px"
          ref="parserRef"
          :formConf="formConf"
          :formData="formData"
          @toDetail="toDetail"
          :key="key"
          v-if="!loading && (!extraList.length || hideExtra)" />
      </div>
      <FormExtraPanel v-bind="getFormExtraBind" v-if="state.dataForm.id && formConf.dataLog" />
    </div>
  </BasicPopup>
  <BasicModal v-bind="$attrs" @register="registerModal" :title="title" :width="formConf.generalWidth" :minHeight="100" :showOkBtn="false" :closeFunc="onClose">
    <template #insertFooter>
      <a-button
        class="ml-10px"
        v-for="item in state.customBtns"
        :key="item.value"
        type="primary"
        :preIcon="item.actionConfig?.btnIcon"
        v-if="!loading"
        @click="customBtnsHandle(item)">
        {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
      </a-button>
      <a-button class="ml-10px" type="primary" @click="handlePrint" v-if="formConf.hasPrintBtn && formConf.printId">{{ getPrintText }}</a-button>
    </template>
    <Parser ref="parserRef" :formConf="formConf" :formData="formData" @toDetail="toDetail" :key="key" v-if="!loading" />
  </BasicModal>
  <BasicDrawer v-bind="$attrs" @register="registerDrawer" :title="title" :width="formConf.drawerWidth" showFooter :showOkBtn="false" :closeFunc="onClose">
    <template #insertFooter>
      <a-button
        class="ml-10px"
        v-for="item in state.customBtns"
        :key="item.value"
        type="primary"
        :preIcon="item.actionConfig?.btnIcon"
        v-if="!loading"
        @click="customBtnsHandle(item)">
        {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
      </a-button>
      <a-button class="ml-10px" type="primary" @click="handlePrint" v-if="formConf.hasPrintBtn && formConf.printId">{{ getPrintText }}</a-button>
    </template>
    <div class="p-10px">
      <Parser ref="parserRef" :formConf="formConf" :formData="formData" @toDetail="toDetail" :key="key" v-if="!loading" />
    </div>
  </BasicDrawer>
  <Detail v-if="detailVisible" ref="detailRef" @close="state.detailVisible = false" />
  <Form v-if="formVisible" ref="formRef" @reload="reloadTable" />
  <PrintSelect @register="registerPrintSelect" @change="handleShowBrowse" />
  <PrintBrowse @register="registerPrintBrowse" />
  <CustomForm ref="customFormRef" />
</template>
<script lang="ts" setup>
  import { getDataChange, getConfigData, getConfigDataByMenuId, getModelInfo, launchFlow } from '@/api/onlineDev/visualDev';
  import { getDataInterfaceRes } from '@/api/systemData/dataInterface';
  import { reactive, toRefs, nextTick, ref, computed } from 'vue';
  import { BasicPopup, usePopup } from '@/components/Popup';
  import { BasicModal, useModal } from '@/components/Modal';
  import { BasicDrawer, useDrawer } from '@/components/Drawer';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useUserStore } from '@/store/user';
  import { useGeneratorStore } from '@/store/generator';
  import Parser from './Parser.vue';
  import Form from '../Form.vue';
  import PrintSelect from '@/components/PrintDesign/printSelect/index.vue';
  import PrintBrowse from '@/components/PrintDesign/printBrowse/index.vue';
  import { cloneDeep } from 'lodash-es';
  import { getScriptFunc, onlineUtils, getParamList, getLaunchFlowParamList } from '@/utils/ceri';
  import CustomForm from '../CustomForm.vue';
  import FormExtraPanel from '@/components/FormExtraPanel/index.vue';
  import { createAsyncComponent } from '@/utils/factory/createAsyncComponent';

  interface State {
    formConf: any;
    formData: any;
    config: any;
    loading: boolean;
    key: number;
    dataForm: any;
    formOperates: any[];
    title: string;
    detailVisible: boolean;
    formVisible: boolean;
    customBtns: any[];
    extraList: any[];
    extraActiveKey: number;
    extraConfig: any;
    extraLoading: boolean;
    extraKey: number;
    hideExtra: boolean;
  }

  defineOptions({ name: 'Detail' });
  const ExtraList = createAsyncComponent(() => import('./ExtraList.vue'));
  const emit = defineEmits(['close']);
  const userStore = useUserStore();
  const generatorStore = useGeneratorStore();
  const { createMessage, createConfirm } = useMessage();
  const { t } = useI18n();
  const [registerPopup, { openPopup, closePopup, setPopupProps }] = usePopup();
  const [registerModal, { openModal, closeModal, setModalProps }] = useModal();
  const [registerDrawer, { openDrawer, closeDrawer, setDrawerProps }] = useDrawer();
  const [registerPrintSelect, { openModal: openPrintSelect }] = useModal();
  const [registerPrintBrowse, { openModal: openPrintBrowse }] = useModal();
  const parserRef = ref<any>(null);
  const detailRef = ref<any>(null);
  const customFormRef = ref<any>(null);
  const formRef = ref<any>(null);
  const extraListRef = ref<any>(null);
  const state = reactive<State>({
    formConf: {},
    formData: {},
    config: {},
    loading: false,
    key: +new Date(),
    dataForm: {
      id: '',
      data: '',
    },
    formOperates: [],
    title: t('common.detailText'),
    detailVisible: false,
    formVisible: false,
    customBtns: [],
    extraList: [],
    extraActiveKey: 0,
    extraConfig: {},
    extraLoading: false,
    extraKey: 0,
    hideExtra: false,
  });
  const { title, formConf, formData, key, loading, detailVisible, formVisible, extraList, extraActiveKey, extraLoading, extraKey, hideExtra } = toRefs(state);

  const getPrintText = computed(() => {
    const text = state.formConf.printButtonTextI18nCode
      ? t(state.formConf.printButtonTextI18nCode, state.formConf.printButtonText)
      : state.formConf.printButtonText;
    return text || t('common.printText');
  });
  const getFormExtraBind = computed(() => ({ showLog: state.formConf.dataLog, modelId: state.config.modelId, formDataId: state.config.id }));

  defineExpose({ init });

  function fillFormData(form, data) {
    const loop = (list, parent?) => {
      for (let i = 0; i < list.length; i++) {
        let item = list[i];
        if (item.__vModel__) {
          if (item.__config__.ceriKey === 'relationForm' || item.__config__.ceriKey === 'popupSelect') {
            item.__config__.defaultValue = data[item.__vModel__ + '_id'];
            item.name = data[item.__vModel__] || '';
          } else {
            const val = data.hasOwnProperty(item.__vModel__) ? data[item.__vModel__] : item.__config__.defaultValue;
            item.__config__.defaultValue = val;
          }
          if (!state.config.isDataManage && state.config.useFormPermission) {
            let id = item.__config__.isSubTable ? parent.__vModel__ + '-' + item.__vModel__ : item.__vModel__;
            let noShow = true;
            if (state.formOperates && state.formOperates.length) {
              noShow = !state.formOperates.some(o => o.enCode === id);
            }
            noShow = item.__config__.noShow ? item.__config__.noShow : noShow;
            item.__config__.noShow = noShow;
          }
        } else {
          if (['relationFormAttr', 'popupAttr'].includes(item.__config__.ceriKey)) {
            item.__config__.defaultValue = data[item.relationField.split('_ceriTable_')[0] + '_' + item.showField];
          }
        }
        if (item.__config__ && item.__config__.children && Array.isArray(item.__config__.children)) {
          loop(item.__config__.children, item);
        }
      }
    };
    loop(form.fields);
  }
  function init(data) {
    state.loading = true;
    state.config = data;
    state.formConf = cloneDeep(data.formConf);
    state.customBtns = (state.formConf.customBtns || []).reverse();
    state.dataForm.id = data.id;
    state.extraActiveKey = 0;
    state.extraConfig = {};
    state.hideExtra = data.hideExtra || false;
    getFormOperates();
    getExtraList();
    openForm();
    nextTick(() => {
      setTimeout(initData, 0);
    });
  }
  function initData() {
    changeLoading(true);
    state.loading = true;
    if (state.config.id) {
      const extra = { modelId: state.config.modelId, id: state.config.id, type: 2 };
      generatorStore.setDynamicModelExtra(extra);
      getInfo(state.config.id, state.config.propsValue);
    } else {
      closeForm();
    }
  }
  function getInfo(id, propsValue) {
    let query: any = {
      id: id,
      menuId: state.config.menuId,
    };
    if (propsValue) query = { ...query, propsValue };
    getDataChange(state.config.modelId, query).then(res => {
      state.dataForm = res.data || {};
      if (!state.dataForm.data) return;
      state.formData = JSON.parse(state.dataForm.data);
      fillFormData(state.formConf, state.formData);
      initRelationForm(state.formConf.fields);
      nextTick(() => {
        state.loading = false;
        state.key = +new Date();
        changeLoading(false);
      });
    });
  }
  function initRelationForm(componentList) {
    componentList.forEach(cur => {
      const config = cur.__config__;
      if (config.ceriKey == 'relationFormAttr' || config.ceriKey == 'popupAttr') {
        const relationKey = cur.relationField.split('_ceriTable_')[0];
        componentList.forEach(item => {
          const noVisibility = Array.isArray(item.__config__.visibility) && !item.__config__.visibility.includes('pc');
          if (relationKey == item.__vModel__ && (noVisibility || !!item.__config__.noShow) && !cur.__vModel__) {
            cur.__config__.noShow = true;
          }
        });
      }
      if (cur.__config__.children && cur.__config__.children.length) initRelationForm(cur.__config__.children);
    });
  }
  function getFormOperates() {
    if (state.config.isPreview || state.config.isDataManage || !state.config.useFormPermission) return;
    const permissionList = userStore.getPermissionList;
    const modelId = state.config.menuId;
    const list = permissionList.filter(o => o.modelId === modelId);
    state.formOperates = list[0] && list[0].form ? list[0].form : [];
  }
  function toDetail(item) {
    if (!item.__config__.defaultValue) return;
    getConfigData(item.modelId).then(res => {
      if (!res.data) return;
      if (!res.data.formData) return;
      const formConf = JSON.parse(res.data.formData);
      formConf.popupType = state.formData.popupType;
      formConf.customBtns = [];
      formConf.hasPrintBtn = false;
      const data = {
        id: item.__config__.defaultValue,
        formConf,
        modelId: item.modelId,
        propsValue: item.propsValue,
      };
      state.detailVisible = true;
      nextTick(() => {
        detailRef.value?.init(data);
      });
    });
  }
  function handlePrint() {
    if (state.config.isPreview) return createMessage.warning('功能预览不支持打印');
    if (!state.formConf.printId?.length) return createMessage.error('未配置打印模板');
    if (state.formConf.printId?.length === 1) return handleShowBrowse(state.formConf.printId[0]);
    openPrintSelect(true, state.formConf.printId);
  }
  function handleShowBrowse(id) {
    openPrintBrowse(true, { id, formInfo: [{ formId: state.dataForm.id }] });
  }
  function openForm() {
    if (state.formConf.popupType === 'fullScreen') return openPopup();
    if (state.formConf.popupType === 'drawer') return openDrawer();
    openModal();
  }
  function closeForm() {
    if (state.formConf.popupType === 'fullScreen') return closePopup();
    if (state.formConf.popupType === 'drawer') return closeDrawer();
    closeModal();
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
    emit('close');
    return true;
  }
  // 自定义按钮点击事件
  function customBtnsHandle(item) {
    if (item.actionConfig.btnType == 1) handlePopup(item.actionConfig);
    if (item.actionConfig.btnType == 2) handleScriptFunc(item.actionConfig);
    if (item.actionConfig.btnType == 3) handleInterface(item.actionConfig);
    if (item.actionConfig.btnType == 4) handleLaunchFlow(item);
  }
  function handlePopup(item) {
    const data = {
      ...item,
      recordModelId: state.config.modelId,
      record: state.formData,
    };
    customFormRef.value?.init(data);
  }
  function handleScriptFunc(item) {
    const parameter = { data: state.formData, onlineUtils };
    const func: any = getScriptFunc(item.func);
    if (!func) return;
    func(parameter);
  }
  function handleInterface(item) {
    const handlerData = () => {
      getModelInfo(state.config.modelId, state.config.id).then(res => {
        const dataForm = res.data || {};
        if (!dataForm.data) return;
        const data = { ...JSON.parse(dataForm.data), id: state.config.id };
        handlerInterface(data);
      });
    };
    const handlerInterface = data => {
      const query = { paramList: getParamList(item.templateJson, { ...data, id: state.config.id }) || [] };
      getDataInterfaceRes(item.interfaceId, query).then(res => {
        createMessage.success(res.msg);
      });
    };
    if (!item.useConfirm) return handlerData();
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: item.confirmTitle || '确认执行此操作?',
      onOk: () => {
        handlerData();
      },
    });
  }
  function handleLaunchFlow(item) {
    const launchFlowCfg = cloneDeep(item.actionConfig.launchFlow);
    const query = {
      template: launchFlowCfg.flowId,
      btnCode: item.value,
      currentUser: launchFlowCfg.currentUser,
      customUser: launchFlowCfg.customUser,
      initiator: launchFlowCfg.initiator,
      dataList: [getLaunchFlowParamList(launchFlowCfg.transferList, state.formData)],
    };
    launchFlow(state.config.modelId, query).then(res => {
      createMessage.success(res.msg);
    });
  }
  function getExtraList() {
    state.extraList = state.formConf.detailExtraList?.length ? [{ fullName: state.config.title, id: 0 }, ...state.formConf.detailExtraList] : [];
  }
  function onTabChange(index) {
    if (state.extraList[index]?.extraConfig) return (state.extraKey = +new Date());
    state.extraLoading = true;
    setTimeout(() => {
      getConfig(state.extraList[index]?.targetFormId, index);
    }, 200);
  }
  function getConfig(menuId, index) {
    if (!menuId) return (state.extraLoading = false);
    getConfigDataByMenuId({ menuId, systemId: userStore.getUserInfo?.systemId })
      .then(res => {
        if (res.code !== 200 || !res.data) {
          state.extraList[index].extraConfig = '';
          state.extraLoading = false;
          state.extraKey = +new Date();
          return;
        }
        state.extraList[index].extraConfig = res.data;
        state.extraLoading = false;
        state.extraKey = +new Date();
      })
      .catch(() => {
        state.extraLoading = false;
        state.extraKey = +new Date();
      });
  }
  function handleOpenDetail(data) {
    state.detailVisible = true;
    nextTick(() => {
      detailRef.value?.init(data);
    });
  }
  function handleOpenForm(data) {
    state.formVisible = true;
    nextTick(() => {
      formRef.value?.init(data);
    });
  }
  function reloadTable() {
    extraListRef.value?.reload();
  }
</script>
