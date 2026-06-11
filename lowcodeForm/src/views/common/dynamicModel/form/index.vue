<template>
  <div class="ceri-content-wrapper bg-white">
    <FormPopup @register="registerFormPopup" />
    <!-- <FlowParser @register="registerFlowParser" @reload="init()" /> -->
  </div>
</template>

<script lang="ts" setup>
  import { onMounted } from 'vue';
  import { usePopup } from '@/components/Popup';
  import FormPopup from './FormPopup.vue';
  // import FlowParser from '@/views/workFlow/components/FlowParser.vue';

  const props = defineProps(['config', 'modelId', 'isPreview']);
  const [registerFormPopup, { openPopup: openFormPopup }] = usePopup();
  // const [registerFlowParser, { openPopup: openFlowParser }] = usePopup();

  function openFlowPopup() {
    const data = {
      id: '',
      flowId: props.config.flowId,
      opType: '-1',
      hideCancelBtn: true,
      hideSaveBtn: true,
    };
    // openFlowParser(true, data);
  }
  function init() {
    if (props.config.enableFlow) return openFlowPopup();
    const data = {
      modelId: props.modelId,
      isPreview: props.isPreview,
      ...props.config,
    };
    openFormPopup(true, data);
  }

  onMounted(() => {
    init();
  });
</script>
