<template>
  <BasicModal
    v-bind="$attrs"
    @register="registerModal"
    defaultFullscreen
    :footer="null"
    :closable="false"
    :keyboard="false"
    destroyOnClose
    class="ceri-full-modal full-modal designer-modal ceri-print-preview-modal">
    <template #title>
      <div class="ceri-full-modal-header">
        <div class="header-title">
          <img src="@/assets/images/NewLogo.png" class="header-logo" />
          <span class="header-dot"></span>
          <p class="header-txt">打印预览</p>
        </div>
        <a-space class="options" :size="10">
          <a-button type="primary" @click="handleDownload()">导出PDF</a-button>
          <a-button type="primary" @click="handlePrint()">{{ t('common.printText') }}</a-button>
          <a-button @click="closeModal()">{{ t('common.cancelText') }}</a-button>
        </a-space>
      </div>
    </template>
    <div id="previewContentDesign"></div>
  </BasicModal>
</template>

<script lang="ts" setup>
  import { ref } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { newHiprintPrintTemplate } from '@/components/PrintDesign/PrintDesign/utils/template-helper';
  import { useI18n } from '@/hooks/web/useI18n';
  import { useGlobSetting } from '@/hooks/setting';
  import { getPreviewInfo } from '@/api/system/printDev';
  import $ from 'jquery';
  import dayjs from 'dayjs';

  defineOptions({ name: 'printPreview' });

  const { t } = useI18n();
  const globSetting = useGlobSetting();
  const [registerModal, { closeModal, changeLoading }] = useModalInner(init);
  const hiprintTemplate = ref();
  const fullName = ref('');

  /**
   * 初始化
   */
  async function init(data) {
    fullName.value = data.fullName || '';
    $('#previewContentDesign').html(null);
    changeLoading(true);
    const { id, printTplPanels = '' } = data;
    try {
      const realPanels = id !== undefined ? JSON.parse((await getPreviewInfo(id))?.data?.printTemplate) : JSON.parse(printTplPanels);
      hiprintTemplate.value = newHiprintPrintTemplate('printPreview', { template: realPanels });
      initHinnn();
      $('#previewContentDesign').html(hiprintTemplate.value?.getHtml() || '模板加载失败');
    } catch (error) {
      $('#previewContentDesign').html('<div class="tpl-invalid">模板已失效，请重新设计</div>');
    } finally {
      changeLoading(false);
    }
  }
  // 重写hinnn
  function initHinnn() {
    if (!(window as any).hinnn) return;
    (window as any).hinnn.apiUrl = globSetting.apiUrl;
    (window as any).hinnn.dateFormat = function (date, format) {
      if (!date) return '';
      if (!isNaN(date) && typeof date === 'string') date = Number(date);
      format = format.replaceAll('y', 'Y').replaceAll('d', 'D');
      return dayjs(date).format(format);
    };
  }
  /**
   * 浏览器打印
   */
  function handlePrint() {
    const options = { leftOffset: 0, topOffset: 0 };
    // 调用浏览器打印
    hiprintTemplate.value?.print({}, options, {
      styleHandler: () => {
        return '<link rel="stylesheet" href="/css/print-lock.css" />';
      },
    });
  }
  // 导出PDF
  function handleDownload() {
    if (!hiprintTemplate.value) return;
    hiprintTemplate.value?.toPdf({}, `${fullName.value}_${dayjs().format('YYYYMMDDHHmmss')}`, { isDownload: true }).then(() => {});
  }
</script>

<style lang="less">
  .ceri-full-modal.ceri-print-preview-modal {
    .ant-modal-body > .scrollbar {
      padding: 0 0 10px !important;
    }

    .scrollbar__view {
      overflow-x: hidden !important;
      overflow-y: auto !important;
    }

    .hiprint-printPaper {
      background-color: #fff;
      margin: 10px auto;
    }

    .tpl-invalid {
      width: 210mm;
      height: 296mm;
      background: #fff;
      margin: 0 auto;
      text-align: center;
      font-size: 16px;
      padding-top: 20px;
      box-sizing: border-box;
    }
  }

  #hiwprint_iframe {
    border: 0 !important;
  }
</style>
