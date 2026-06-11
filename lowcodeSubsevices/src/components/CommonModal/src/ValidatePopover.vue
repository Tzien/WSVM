<template>
  <a-popover
    v-model:open="visible"
    trigger="click"
    placement="bottom"
    overlayClassName="error-contain ceri-flow-common-popover"
    :bordered="false"
    arrow-point-at-center
    v-if="getErrorList.length">
    <template #content>
      <div class="w-355px">
        <div class="error-title"><exclamation-circle-filled class="error-icon" />内容不完善，校验失败！</div>
        <div class="error-content">
          <div class="error-item" v-for="item in getErrorList">
            <div class="error-sub-item error-top">
              <div class="title" :title="item.title">{{ item.title }}</div>
              <div>
                <span>{{ item.children.length }}</span>
                项
              </div>
            </div>
            <div class="error-sub-item error-bottom" v-for="child in item.children">
              <div class="title" :title="child.errorInfo">{{ child.errorInfo }}</div>
              <div class="write" v-if="item?.id" @click="handleSelect(item.id)">去填写</div>
            </div>
          </div>
        </div>
      </div>
    </template>
    <a-button shape="round" class="error-tips-button">
      {{ getErrorList.length || 0 }}项不完善<i  style="margin-left: 5px;font-size: 10px;" class="icon-ym icon-ym-unfold" />
    </a-button>
  </a-popover>
</template>
<script lang="ts" setup>
  import { computed, ref } from 'vue';
  import { ExclamationCircleFilled } from '@ant-design/icons-vue';

  const visible = ref<boolean>(false);
  const emit = defineEmits(['select']);
  const props = defineProps({
    errorList: { type: Array as PropType<any[]>, default: () => [] },
  });

  defineExpose({ setVisible });

  const getErrorList = computed(() => convertArrayToTree(props.errorList));

  /**
   * 将errorList转成树
   * @param list  errorList
   */
  function convertArrayToTree(list) {
    let newList: any[] = [];
    list.map(item => {
      const index = newList.findIndex(o => o.id === item.id);
      if (index !== -1) {
        newList[index].children.push({ errorInfo: item.errorInfo });
      } else {
        newList.push({ title: item.title, id: item.id, children: [{ errorInfo: item.errorInfo }] });
      }
    });
    return newList;
  }
  function handleSelect(id) {
    setVisible(false);
    emit('select', id);
  }
  function setVisible(data) {
    visible.value = !!data;
  }
</script>
<style lang="less">
  .error-tips-button {
    display: flex;
    align-items: center;
    &:hover i {
      color: @primary-color;
    }
    i {
      color: @text-color-label;
    }
  }
  .error-contain {
    .ant-popover-inner {
      border-radius: 8px;
      overflow: hidden;
      .ant-popover-inner-content {
        padding: unset !important;
      }
    }
    .ant-popover-arrow::before {
      background: #fdc6c6 !important;
    }
    .error-title {
      background: linear-gradient(26deg, #fceaea 0%, #fdc6c6 100%);
      height: 46px;
      font-size: 16px;
      font-weight: bold;
      color: @text-color-base;
      display: flex;
      align-items: center;
      padding: 0 26px;
      .error-icon {
        margin-right: 4px;
        color: #e85959;
      }
    }
    .error-content {
      padding: 8px 26px;
      max-height: 300px;
      overflow: auto;
      .error-item {
        display: flex;
        flex-direction: column;
        .error-sub-item {
          display: flex;
          justify-content: space-between;
          line-height: 35px;
          .title {
            flex: 1;
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
          }
        }
        .error-top {
          .title {
            font-weight: bold;
          }
          span {
            color: @error-color;
          }
        }
        .error-bottom {
          .title {
            color: @text-color-label;
          }
          .write {
            color: @primary-color;
            flex-shrink: 0;
            margin-left: 10px;
            cursor: pointer;
          }
        }
      }
    }
  }
</style>
