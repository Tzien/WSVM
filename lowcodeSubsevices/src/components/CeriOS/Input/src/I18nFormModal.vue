<template>
  <div class="common-container">
    <a-modal
      v-model:open="visible"
      :title="t('common.addText')"
      :width="600"
      class="common-container-modal  form-modal"
      :maskClosable="false">
      <template #closeIcon>
        <ModalClose :canFullscreen="false" @cancel="handleCancel" />
      </template>
      <template #footer>
        <a-button @click="handleCancel()">{{ t('common.cancelText') }}</a-button>
        <a-button type="primary" v-loading="loading" @click="handleSubmit()">{{ t('common.okText') }}</a-button>
      </template>
      <a-form :colon="false" :labelCol="{ style: { width: '80px' } }" :model="dataForm" :rules="formRules" ref="formElRef">
        <a-form-item label="翻译标记" name="enCode">
          <ceri-input v-model:value="dataForm.enCode" placeholder="请输入" />
        </a-form-item>
        <a-form-item :label="item.fullName" :name="item.enCode" v-for="item in categoryList">
          <ceri-input v-model:value="dataForm[item.enCode]" placeholder="请输入" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>
<script lang="ts" setup>
  import { create } from '../../../../api/system/baseLang';
  import { Modal as AModal } from 'ant-design-vue';
  import { reactive, ref, toRefs } from 'vue';
  import type { FormInstance } from 'ant-design-vue';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n';
  import { useBaseStore } from '@/store/base';
  import ModalClose from '@/components/Modal/src/components/ModalClose.vue';

  interface State {
    dataForm: any;
    formRules: any;
    categoryList: any[];
    type: Number;
  }

  defineOptions({ name: 'I18nFormModal', inheritAttrs: false });
  defineExpose({ openModal });

  const defaultForm = {
    id: '',
    type: 0,
    enCode: '',
    map: {},
  };
  const baseStore = useBaseStore();
  const emit = defineEmits(['register', 'reload']);
  const formElRef = ref<FormInstance>();
  const visible = ref(false);
  const loading = ref(false);
  const { createMessage } = useMessage();
  const { t } = useI18n();
  const state = reactive<State>({
    dataForm: {
      id: '',
      type: 0,
      enCode: '',
      map: {},
    },
    formRules: {
      enCode: [
        { required: true, message: '必填', trigger: 'change' },
        { pattern: /^[a-zA-Z][a-zA-Z0-9._-]*$/, message: '只能输入字母、数字、点、横线和下划线，且以字母开头', trigger: 'change' },
      ],
    },
    categoryList: [],
    type: 0,
  });
  const { dataForm, formRules, categoryList } = toRefs(state);

  async function openModal() {
    visible.value = true;
    loading.value = false
    state.dataForm = { ...defaultForm };
    formElRef.value?.resetFields();
    state.categoryList = categoryList.value = (await baseStore.getDictionaryData('Language')) as any[];
  }
  function handleCancel() {
    visible.value = false;
    emit('reload');
  }
  async function handleSubmit() {
    loading.value = true
    try {
      const values = await formElRef.value?.validate();
      if (!values) return;
      if (!state.dataForm.map) state.dataForm.map = {};
      for (let i = 0; i < state.categoryList.length; i++) {
        state.dataForm.map[state.categoryList[i].enCode] = state.dataForm[state.categoryList[i].enCode] || '';
      }
      create(state.dataForm)
        .then(res => {
          createMessage.success(res.data.message);
          loading.value = false
          visible.value = false;
          emit('reload',true);
        })
        .catch(() => {
          loading.value = false
        });
    } catch (_) {
      loading.value = false
    }
  }
</script>
<style lang="less">
  .form-modal {
    top: 270px !important;

    .ant-modal-body {
      padding: 30px !important;
      height: 27vh !important;
    }
  }
</style>
