<template>
  <BasicModal v-bind="$attrs" @register="registerModal" title="预览" :footer="null" :width="600" class="ceri-add-modal ceri-preview-modal">
    <a-tabs v-model:activeKey="state.previewType" size="small" v-if="state.type === 'webDesign' || state.type === 'flow'">
      <a-tab-pane :key="0" tab="设计中" />
      <a-tab-pane :key="1" tab="已发布" v-if="state.isRelease" />
    </a-tabs>
    <div style="display: flex; align-items: center; justify-content: space-between; margin: 20px 0;">
      <div style="width: 270px; height: 136px; background: #edfbfd; border-radius: 8px; display: flex; flex-direction: column; align-items: center; justify-content: center; cursor: pointer; transition: opacity 0.3s; margin-right: 20px;" 
           @click="previewPc()" @mouseenter="handleMouseEnter" @mouseleave="handleMouseLeave">
        <i class="icon-ym icon-ym-pc" style="width: 56px; height: 56px; background: #d6f0ff; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 24px; color: #1890ff; margin-bottom: 16px;"></i>
        <div style="text-align: center;">
          <p style="font-size: 16px; margin: 0; color: #333; font-weight: 500;">Web预览</p>
        </div>
      </div>
      <div style="width: 270px; height: 136px; background: #f0fff7; border-radius: 8px; display: flex; flex-direction: column; align-items: center; justify-content: center; cursor: pointer; transition: opacity 0.3s;" 
           @click="previewApp()" @mouseenter="handleMouseEnter" @mouseleave="handleMouseLeave">
        <i class="icon-ym icon-ym-mobile" style="width: 56px; height: 56px; background: #d9f7be; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 24px; color: #52c41a; margin-bottom: 16px;"></i>
        <div style="text-align: center;">
          <p style="font-size: 16px; margin: 0; color: #333; font-weight: 500;">App预览</p>
        </div>
      </div>
    </div>
    <BasicModal v-bind="$attrs" @register="registerQrModal" title="预览" :footer="null" :width="400" class="ceri-qrcode-modal">
      <ceri-qrcode :staticText="qrCodeText" :width="280" />
      <p class="tip">打开手机APP扫码预览</p>
    </BasicModal>
  </BasicModal>
</template>
<script lang="ts" setup>
  import { ref, reactive } from 'vue';
  import { useGo } from '@/hooks/web/usePage';
  import { BasicModal, useModal, useModalInner } from '@/components/Modal';
import router from '@/router';

  interface State {
    type: string;
    id: string;
    fullName: string;
    previewType: number;
    isRelease: number;
  }

  const emit = defineEmits(['register', 'previewPc']);
  const [registerModal, { closeModal }] = useModalInner(init);
  const [registerQrModal, { openModal: openQrModal }] = useModal();
  const go = useGo();
  const qrCodeText = ref('');
  const state = reactive<State>({
    type: '',
    id: '',
    fullName: '',
    previewType: 0,
    isRelease: 0,
  });

  function init(data) {
    state.type = data.type || '';
    state.id = data.id;
    state.fullName = data.fullName || '';
    state.isRelease = data.isRelease || 0;
    state.previewType = 0;
  }
  function previewPc() {
    closeModal();
    if (state.type === 'webDesign') {
      if (!state.id) return;
      go(`/previewModel?isPreview=1&id=${state.id}&previewType=${state.previewType}`);
      return;
    }
    setTimeout(() => {
      emit('previewPc', { id: state.id, previewType: state.previewType });
    }, 300);
  }
  function previewApp() {
    let data: any = {
      t: state.type === 'flow' ? 'WFP' : state.type === 'webDesign' ? 'ADP' : state.type,
      id: state.id,
    };
    if (state.type === 'report') data.fullName = state.fullName;
    if (state.type == 'webDesign' || state.type === 'flow') data.previewType = state.previewType;
    qrCodeText.value = JSON.stringify(data);
    closeModal();
    openQrModal(true);
  }
  function handleMouseEnter(e: Event) {
    (e.target as HTMLElement).style.opacity = '0.9';
  }
  function handleMouseLeave(e: Event) {
    (e.target as HTMLElement).style.opacity = '1';
  }
</script>
<style lang="less">
  .ceri-qrcode-modal {
    .ceri-qrcode {
      padding: 10px;
    }
    .tip {
      text-align: center;
      font-size: 18px;
      margin-top: 10px;
      padding-bottom: 20px;
      color: @text-color-secondary;
    }
  }
</style>
