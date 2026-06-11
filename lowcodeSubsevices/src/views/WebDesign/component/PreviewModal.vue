<template>
  <BasicModal
    v-bind="$attrs"
    @register="registerModal"
    defaultFullscreen
    :footer="null"
    :closable="false"
    :keyboard="false"
    class="ceri-full-modal full-modal designer-modal">
    <template #title>
      <div class="ceri-full-modal-header">
        <div class="header-title">
          <img src="@/assets/images/NewLogo.png" class="header-logo" />
          <span class="header-dot"></span>
          <a-tooltip :title="fullName">
            <p class="header-txt">{{ fullName }}</p>
          </a-tooltip>
        </div>
        <a-steps :current="1" type="navigation" size="small" class="header-steps tab-steps">
          <a-step title="代码预览" />
        </a-steps>
        <a-space class="options" :size="10">
          <a-button @click="openDiffPreviewModal(true, { ...data })">代码对比</a-button>
          <a-button @click="closeModal()">{{ t('common.closeText') }}</a-button>
        </a-space>
      </div>
    </template>
    <div class="ceri-content-wrapper">
      <div class="ceri-content-wrapper-left">
        <BasicLeftTree :showSearch="false" ref="leftTreeRef" :fieldNames="{ title: 'fileName' }" :treeData="treeData" @select="handleTreeSelect" />
      </div>
      <div class="ceri-content-wrapper-center">
        <div class="ceri-content-wrapper-content bg-white">
          <MonacoEditor v-model="currentContent" :options="editorOptions" :language="editorLanguage" :key="key" />
        </div>
      </div>
    </div>
  </BasicModal>
  <DiffPreviewModal @register="registerDiffPreviewModal" />
</template>
<script lang="ts" setup>
  // import { codePreview } from '@/api/onlineDev/visualDev';
  import { reactive, toRefs, nextTick, ref, unref } from 'vue';
  import { BasicModal, useModal, useModalInner } from '@/components/Modal';
  import { useI18n } from '@/hooks/web/useI18n';
  import { BasicLeftTree, TreeActionType } from '@/components/Tree';
  import { MonacoEditor } from '@/components/CodeEditor';
  import DiffPreviewModal from './DiffPreviewModal.vue';

  interface State {
    treeData: any[];
    currentId: string;
    currentContent: string;
    editorOptions: any;
    editorLanguage: string;
    key: number;
    fullName: string;
    data: any;
  }

  defineEmits(['register']);
  const { t } = useI18n();
  const leftTreeRef = ref<Nullable<TreeActionType>>(null);
  const [registerModal, { closeModal, changeLoading }] = useModalInner(init);
  const state = reactive<State>({
    treeData: [],
    currentId: '',
    currentContent: '',
    editorOptions: {
      readOnly: true,
      scrollBeyondLastLine: true,
      minimap: {
        enabled: true,
      },
    },
    editorLanguage: 'html',
    key: +new Date(),
    fullName: '',
    data: {},
  });
  const { treeData, currentContent, editorOptions, editorLanguage, key, fullName, data } = toRefs(state);
  const [registerDiffPreviewModal, { openModal: openDiffPreviewModal }] = useModal();

  function init(data) {
    state.fullName = data.fullName || '';
    state.data = data;
    state.treeData = [];
    state.currentId = '';
    state.currentContent = '';
    state.key = +new Date();
    changeLoading(true);
    const query = {
      module: data.module || 'system',
      description: data.description,
      modulePackageName: data.modulePackageName || '',
      enableFlow: data.enableFlow,
    };
    // codePreview(data.id, query).then(res => {
    //   state.treeData = res.data.list.map(o => ({ ...o, disabled: true }));
    //   state.currentId = state.treeData[0].children[0].id;
    //   state.currentContent = state.treeData[0].children[0].fileContent;
    //   state.editorLanguage = ['web', 'app'].includes(state.treeData[0].children[0].fileType) ? 'html' : 'java';
    //   nextTick(() => {
    //     const leftTree = unref(leftTreeRef);
    //     leftTree?.setSelectedKeys([state.currentId]);
    //     changeLoading(false);
    //   });
    // });
  }
  function handleTreeSelect(id, node) {
    state.key = +new Date();
    state.currentId = id;
    state.currentContent = node.fileContent;
    state.editorLanguage = ['web', 'app'].includes(node.fileType) ? 'html' : 'java';
  }
</script>
