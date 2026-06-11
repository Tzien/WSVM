<template>
  <BasicModal v-bind="$attrs" @register="registerModal" title="过滤规则配置" :width="700" @ok="handleSubmit" destroyOnClose class="ceri-condition-modal">
    <ConditionMain ref="conditionMainRef" defaultAddEmpty showFieldValueType isCustomFieldValueType />
  </BasicModal>
</template>
<script lang="ts" setup>
  import { ref, nextTick } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import ConditionMain from './ConditionMain.vue';

  const emit = defineEmits(['register', 'confirm']);
  const [registerModal, { closeModal, changeLoading }] = useModalInner(init);
  const conditionMainRef = ref();

  function init(data) {
    changeLoading(true);
    nextTick(() => {
      conditionMainRef.value?.init(data);
      changeLoading(false);
    });
  }
  function handleSubmit() {
    const values = conditionMainRef.value?.confirm();
    if (!values) return;
    emit('confirm', values);
    closeModal();
  }
</script>
