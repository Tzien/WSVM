<template>
  <div class="form-extra-panel" :class="{ 'form-extra-panel-unfold': !isAdvanced }">
    <a-tooltip placement="left" :title="isAdvanced ? t('component.form.fold') : t('component.form.unfold')" :key="tooltipKey">
      <div class="trigger-btn" @click="toggleAdvanced">
        <DoubleRightOutlined v-if="isAdvanced" />
        <DoubleLeftOutlined v-else />
      </div>
    </a-tooltip>
    <a-tabs v-model:activeKey="activeKey" :tabBarGutter="10" size="small" class="average-tabs" :class="{ 'average-tabs-single': showLog }">
      <a-tab-pane :key="1" tab="修改记录" v-if="showLog" />
    </a-tabs>
    <div class="form-extra-panel-main">
      <DataLogList v-bind="getBindValue" v-if="activeKey == 1" />
    </div>
  </div>
</template>
<script lang="ts" setup>
  import { reactive, toRefs, onMounted, computed } from 'vue';
  import { DoubleLeftOutlined, DoubleRightOutlined } from '@ant-design/icons-vue';
  import { useI18n } from 'vue-i18n';
  import DataLogList from './DataLogList.vue';

  interface State {
    isAdvanced: boolean;
    activeKey: number;
    tooltipKey: number;
  }

  const props = defineProps({
    showLog: { type: Boolean, default: false },
    modelId: { type: String, default: '' },
    formDataId: { type: [String, Number], default: '' },
  });

  const state = reactive<State>({
    isAdvanced: true,
    activeKey: 1,
    tooltipKey: +new Date(),
  });
  const { isAdvanced, activeKey, tooltipKey } = toRefs(state);
  const { t } = useI18n();

  const getBindValue = computed(() => ({ ...props }));

  function toggleAdvanced() {
    state.isAdvanced = !state.isAdvanced;
    state.tooltipKey = +new Date();
  }

  onMounted(() => {
    state.activeKey = 1;
  });
</script>
