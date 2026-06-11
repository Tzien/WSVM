<template>
  <div class="common-container">
    <a-input-group compact v-if="isCheckSubFlow && innerValue">
      <a-select
        style="width: calc(100% - 90px)"
        v-model:value="innerValue"
        v-bind="getSelectBindValue"
        :options="options"
        @change="onChange"
        @click="openSelectModal">
      </a-select>
      <!-- <a-button @click="handleOpenFlowInfo">查看流程</a-button> -->
    </a-input-group>
    <a-select v-else v-model:value="innerValue" v-bind="getSelectBindValue" :options="options" @change="onChange" @click="openSelectModal"> </a-select>
    <a-modal
      v-model:open="visible"
      title="选择流程"
      :width="800"
      class="common-container-modal"
      @cancel="handleCancel"
      @ok="handleSubmit"
      :maskClosable="false">
      <template #closeIcon>
        <ModalClose :canFullscreen="false" @cancel="handleCancel" />
      </template>
      <div class="ceri-content-wrapper">
        <div class="ceri-content-wrapper-center">
          <div class="ceri-content-wrapper-content">
            <BasicTable @register="registerTable" class="ceri-sub-table"></BasicTable>
          </div>
        </div>
      </div>
    </a-modal>
    <a-modal v-model:open="flowVisible" title="流程图" width="80%" class="common-container-modal" @cancel="handleCancel" :footer="null" :maskClosable="false">
      <template #closeIcon>
        <ModalClose :canFullscreen="false" @cancel="handleCancel" />
      </template>
      <div class="ceri-content-wrapper" v-loading="loading">
        <div class="ceri-content-wrapper-center">
          <div class="ceri-content-wrapper-content">
            <!-- <FlowProcessMain
              class="p-10px"
              :flowInfo="state.flowInfo"
              :nodeList="state.nodeList"
              :isPreview="true"
              :openPreview="true"
              handleOpenFlowInfo
              :key="state.key" /> -->
          </div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script lang="ts" setup>
  import { getFlowSelector } from '@/api/workFlow/template';
  import { Form, Modal as AModal } from 'ant-design-vue';
  import { ref, watch, computed, nextTick, reactive } from 'vue';
  import ModalClose from '@/components/Modal/src/components/ModalClose.vue';
  import { BasicTable, useTable, BasicColumn } from '@/components/Table';
  import { useI18n } from 'vue-i18n';
  import { pick } from 'lodash-es';
  // import FlowProcessMain from '@/components/FlowProcess/src/Main.vue';
  // import { getFlowTaskInfo } from '@/api/workFlow/task';

  interface State {
    flowInfo: any;
    nodeList: any[];
    opType: string;
    taskId: string;
    key: number;
    isRevokeTask: boolean;
  }

  const state = reactive<State>({
    flowInfo: {},
    nodeList: [],
    opType: '-1',
    taskId: '',
    key: +new Date(),
    isRevokeTask: false,
  });

  defineOptions({ inheritAttrs: false });
  const props = defineProps({
    value: { default: '' },
    title: { type: String, default: '' },
    placeholder: { type: String, default: '请选择' },
    disabled: { type: Boolean, default: false },
    allowClear: { type: Boolean, default: true },
    size: { type: String, default: 'default' },
    isCheckSubFlow: { type: Number, default: 0 },
  });
  const emit = defineEmits(['update:value', 'change']);
  const formItemContext = Form.useInjectFormItemContext();
  const { t } = useI18n();
  const innerValue = ref(undefined);
  const visible = ref(false);
  const flowVisible = ref(false);
  const loading = ref(false);
  const options = ref<any[]>([]);
  const columns: BasicColumn[] = [
    { title: '流程名称', dataIndex: 'fullName' },
    { title: '流程编码', dataIndex: 'enCode' },
  ];
  const [registerTable, { getForm, getSelectRows, setSelectedRowKeys, getSelectRowKeys }] = useTable({
    api: getFlowSelector,
    columns,
    immediate: false,
    searchInfo: { isAuthority: 0 },
    useSearchForm: true,
    formConfig: {
      baseColProps: { span: 8 },
      schemas: [
        {
          field: 'keyword',
          label: t('common.keyword'),
          component: 'Input',
          componentProps: {
            placeholder: t('common.enterKeyword'),
            submitOnPressEnter: true,
          },
        },
      ],
    },
    tableSetting: { size: false, setting: false },
    isCanResizeParent: true,
    resizeHeightOffset: -74,
    rowSelection: { type: 'radio' },
  });

  const getSelectBindValue = computed(() => {
    return {
      ...pick(props, ['disabled', 'size', 'allowClear', 'placeholder']),
      fieldNames: { label: 'fullName', value: 'id' },
      open: false,
      showSearch: false,
      showArrow: true,
    };
  });

  watch(
    () => props.value,
    val => {
      setValue(val);
    },
    { immediate: true },
  );

  function setValue(value) {
    innerValue.value = value || undefined;
    options.value = [{ id: innerValue.value, fullName: props.title }];
  }
  function onChange() {
    options.value = [];
    emit('change', '', {});
  }
  async function openSelectModal() {
    if (props.disabled) return;
    visible.value = true;
    nextTick(() => {
      getForm().resetFields();
      setSelectedRowKeys(innerValue.value ? [innerValue.value] : []);
    });
  }
  // // 打开流程图
  // function handleOpenFlowInfo() {
  //   loading.value = true;
  //   flowVisible.value = true;
  //   let query = {
  //     flowId: innerValue.value,
  //   };
  //   getFlowTaskInfo(0, query)
  //     .then(res => {
  //       const data = res.data;
  //       loading.value = false;
  //       state.flowInfo = data.flowInfo;
  //       state.nodeList = data.nodeList;
  //       state.opType = '0';
  //       state.key = +new Date();
  //     })
  //     .catch(() => {
  //       loading.value = false;
  //     });
  // }
  function handleCancel() {
    visible.value = false;
    flowVisible.value = false;
  }
  function handleSubmit() {
    if (!getSelectRowKeys().length && !getSelectRows().length) return;
    if (!getSelectRows().length) {
      emit('update:value', innerValue.value);
      emit('change', innerValue.value, options.value[0]);
      formItemContext.onFieldChange();
      handleCancel();
      return;
    }
    const selectRow = getSelectRows()[0];
    options.value = getSelectRows();
    innerValue.value = selectRow.id;
    emit('update:value', selectRow.id);
    emit('change', selectRow.id, selectRow);
    formItemContext.onFieldChange();
    handleCancel();
  }
</script>
