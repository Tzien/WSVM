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
          <code-diff
            :old-string="oldFileContent"
            :new-string="currentContent"
            output-format="side-by-side"
            :language="editorLanguage"
            :theme="getThemeColor"
            :context="99999"
            :key="key" />
        </div>
      </div>
    </div>
  </BasicModal>
</template>
<script lang="ts" setup>
  // import { codePreview } from '@/api/onlineDev/visualDev';
  import { reactive, toRefs, nextTick, ref, unref, computed } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { useI18n } from '@/hooks/web/useI18n';
  import { BasicLeftTree, TreeActionType } from '@/components/Tree';
  import { CodeDiff } from 'v-code-diff';
  import { useAppStore } from '@/store/app';

  interface State {
    treeData: any[];
    currentId: string;
    oldFileContent: string;
    currentContent: string;
    editorLanguage: string;
    key: number;
    fullName: string;
  }

  defineEmits(['register']);
  const { t } = useI18n();
  const leftTreeRef = ref<Nullable<TreeActionType>>(null);
  const appStore = useAppStore();
  const [registerModal, { closeModal, changeLoading }] = useModalInner(init);
  const state = reactive<State>({
    treeData: [],
    currentId: '',
    oldFileContent: '',
    currentContent: '',
    editorLanguage: 'html',
    key: +new Date(),
    fullName: '',
  });
  const { treeData, oldFileContent, currentContent, editorLanguage, key, fullName } = toRefs(state);

  const getThemeColor = computed(() => appStore.getDarkMode);

  function init(data) {
    state.fullName = data.fullName || '';
    state.key = +new Date();
    changeLoading(true);
    const query = {
      module: data.module || 'system',
      description: data.description,
      modulePackageName: data.modulePackageName || '',
      enableFlow: data.enableFlow,
      contrast: true,
    };
    // codePreview(data.id, query).then(res => {
    //   state.treeData = res.data.list.map(o => ({ ...o, disabled: true }));
    //   state.currentId = state.treeData[0].children[0].id;
    //   state.currentContent = state.treeData[0].children[0].fileContent;
    //   state.oldFileContent = state.treeData[0].children[0].oldFileContent;
    //   state.editorLanguage = getLanguage(state.treeData[0].children[0]);
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
    state.oldFileContent = node.oldFileContent;
    state.editorLanguage = getLanguage(node);
  }
  function getLanguage(node) {
    return ['web', 'app'].includes(node.fileType) ? 'html' : ['js', 'ts'].includes(node.folderName) ? 'js' : 'java';
  }
</script>
<style lang="less">
  .code-diff-view {
    margin-top: unset !important;
    margin-bottom: unset !important;
    height: 100% !important;
    border: unset !important;
    background-color: @component-background!important;
    .file-header,
    .empty-cell {
      background-color: @component-background!important;
    }
  }
</style>
