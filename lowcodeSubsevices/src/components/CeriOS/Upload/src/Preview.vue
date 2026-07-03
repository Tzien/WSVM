<template>
  <a-modal
    v-model:open="visible"
    :footer="null"
    :closable="false"
    :keyboard="false"
    :maskClosable="false"
    :width="fullScreenRef ? '100%' : '80%'"
    class="common-container-modal ceri-full-modal full-modal file-preview-modal"
    :wrap-class-name="getWrapClassName">
    <template #closeIcon>
      <ModalClose :fullScreen="fullScreenRef" @cancel="handleCancel" @fullscreen="handleFullScreen" />
    </template>
    <template #title>
      <div class="ceri-full-modal-header">
        <div class="header-title">
          <p class="header-txt">{{ title }}</p>
        </div>
        <a-space class="options" :size="10">
          <a-button type="primary" @click="handleDownload()" v-if="showDownload">下载</a-button>
          <a-button @click="handleCancel()">{{ t('common.cancelText') }}</a-button>
        </a-space>
      </div>
    </template>
    <div class="basic-content bg-white" v-loading="loading" element-loading-text="文件加载中...">
      <iframe v-if="url" :key="url" width="100%" height="100%" :src="url" frameborder="0" @load="handleIframeLoad"></iframe>
    </div>
  </a-modal>
</template>
<script lang="ts" setup>
  import { reactive, toRefs } from 'vue';
  import { Modal as AModal } from 'ant-design-vue';
  import ModalClose from '@/components/Modal/src/components/ModalClose.vue';
  import { useFullScreen } from '@/components/Modal/src/hooks/useModalFullScreen';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n';
  import { getDownloadUrl, previewFile } from '@/api/basic/common';
  import { downloadByUrl } from '@/utils/file/download';
  import type { fileItem } from './props';

  interface State {
    visible: boolean;
    loading: boolean;
    title: string;
    url: string;
    file: fileItem | null;
  }
  const props = defineProps({
    showDownload: { type: Boolean, default: false },
    type: { type: String, default: 'annex' },
  });
  defineEmits(['register']);
  const { createMessage } = useMessage();
  const { t } = useI18n();
  const state = reactive<State>({
    visible: false,
    loading: false,
    title: '',
    url: '',
    file: null,
  });
  const { visible, loading, title, url } = toRefs(state);
  const { handleFullScreen, getWrapClassName, fullScreenRef, resetFullScreen } = useFullScreen();

  defineExpose({ init });

  function init(file: fileItem) {
    state.title = '文档预览 - ' + file.name;
    state.url = '';
    state.file = file;
    state.visible = true;
    state.loading = true;
    const query = {
      fileName: file.fileId,
      originalFileName: file.name,
      fileExtension: file.fileExtension,
      fileVersionId: file.fileVersionId,
      fileDownloadUrl: file.url,
    };
    previewFile(query)
      .then(res => {
        if (res.data) {
          state.url = res.data;
          return;
        }
        createMessage.warning('文件不存在');
        handleCancel();
      })
      .catch(() => {
        state.loading = false;
        handleCancel();
      });
  }
  function handleCancel() {
    state.visible = false;
    resetFullScreen();
  }
  function handleIframeLoad() {
    state.loading = false;
  }
  function handleDownload() {
    const file = state.file;
    if (!file?.fileId) return;
    getDownloadUrl(props.type, file.fileId).then(res => {
      downloadByUrl({ url: res.data.url, fileName: file.name });
    });
  }
</script>
<style lang="less">
  .file-preview-modal {
    top: 5vh;
    max-width: 100%;

    .ant-modal-body {
      height: 75vh;
      padding: 10px !important;
    }
    .header-txt {
      max-width: 80vw !important;
    }
  }

  .fullscreen-modal {
    overflow: hidden;

    .file-preview-modal {
      top: 0;
      padding-bottom: 0;

      .ant-modal-content {
        height: 100vh;
        border-radius: 0;
        display: flex;
        flex-direction: column;
      }

      .ant-modal-body {
        flex: 1;
        height: 0;
      }
    }
  }
</style>
