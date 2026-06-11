<template>
  <a-popover v-model:open="visible" trigger="click" placement="bottom" :bordered="false" arrow-point-at-center @openChange="onPopoverOpen">
    <template #content>
      <div class="ai-popover-content">
        <div class="mb-15px text-15px">AI生成字段</div>
        <ceri-input v-model:value="content" placeholder="输入你的需求描述" :maxlength="100">
          <template #suffix>
            <loading-outlined class="mr-5px" v-if="loading" />
            <i class="ym-custom ym-custom-send cursor-pointer" :class="{ 'icon-selected': content }" v-else @click="handleSubmit" />
          </template>
        </ceri-input>
      </div>
    </template>
    <a-tooltip title="AI生成字段">
      <a-button type="text" class="action-bar-btn">
        <i class="icon-ym icon-ym-ai-form" />
      </a-button>
    </a-tooltip>
  </a-popover>
</template>
<script lang="ts" setup>
  import { ref } from 'vue';
  import { LoadingOutlined } from '@ant-design/icons-vue';
  import { getAiInfo } from '@/api/onlineDev/visualDev';

  const visible = ref<boolean>(false);
  const loading = ref<boolean>(false);
  const content = ref<string>('');
  const emit = defineEmits(['confirm']);

  function onPopoverOpen() {
    content.value = '';
    loading.value = false;
  }
  function handleSubmit() {
    if (!content.value) return;
    loading.value = true;
    getAiInfo({ keyword: content.value })
      .then(res => {
        loading.value = false;
        visible.value = false;
        emit('confirm', res.data.aiModelList || []);
      })
      .catch(() => {
        loading.value = false;
      });
  }
</script>
<style lang="less" scoped>
  .ai-popover-content {
    width: 350px !important;
    padding: 6px;
  }
  .icon-selected {
    color: @primary-color!important;
  }
</style>
