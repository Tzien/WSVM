<template>
  <Tooltip placement="top">
    <template #title>
      <span>{{ t('component.table.viewList') }}</span>
    </template>
    <Popover v-model:open="visible" placement="bottomRight" trigger="click" overlayClassName="ceri-view-list-popover">
      <template #content>
        <div class="content">
          <div v-for="item in viewList" :key="item.id" class="item" @click="handleClick(item)">
            <span class="item-name">{{ item.fullName }}</span>
            <div class="item-delete">
              <i class="icon-ym icon-ym-app-delete" @click.stop="handleDel(item.id)" v-if="item.type !== 0" />
            </div>
            <div class="item-default">
              <PushpinOutlined :class="{ 'item-default-icon': item.status == 0 }" @click.stop="handleDefault(item)" />
            </div>
          </div>
        </div>
      </template>
      <menu-outlined />
    </Popover>
  </Tooltip>
</template>
<script lang="ts" setup>
  import type { PropType } from 'vue';
  import { reactive, toRefs } from 'vue';
  import { Tooltip, Popover } from 'ant-design-vue';
  import { MenuOutlined, PushpinOutlined } from '@ant-design/icons-vue';
  import { useI18n } from 'vue-i18n';
  import { useMessage } from '@/hooks/web/useMessage';
  import { delView, setDefaultView } from '@/api/onlineDev/visualDev';

  interface State {
    visible: boolean;
  }

  const state = reactive<State>({
    visible: false,
  });
  const { visible } = toRefs(state);
  const emit = defineEmits(['itemClick', 'reload']);
  const { t } = useI18n();
  const { createMessage, createConfirm } = useMessage();
  const props = defineProps({
    userId: { type: String },
    menuId: { type: String },
    viewList: { type: Array as PropType<any[]>, default: () => [] },
  });

  function handleClick(item) {
    emit('itemClick', item);
  }
  function handleDel(id) {
    state.visible = false;
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: '确定要删除此视图，是否继续?',
      onOk: () => {
        delView(id, props.menuId).then(res => {
          if (res.code == 200 && res.success) {
            createMessage.success(res.message);
          }else{
            createMessage.error(res.message)
          }
          emit('reload');
        });
      },
    });
  }
  function handleDefault(item) {
    if (item.status === 1) return;
    state.visible = false;
    createConfirm({
      iconType: 'warning',
      title: t('common.tipTitle'),
      content: '设置此视图为默认视图，是否继续？',
      onOk: () => {
        setDefaultView(item.id, props.menuId, props.userId).then(res => {
          createMessage.success(res.message);
          emit('reload');
        });
      },
    });
  }
</script>
<style lang="less">
  .ceri-view-list-popover {
    .ant-popover-inner-content {
      padding: 0;
      .content {
        width: 230px;
        max-height: 250px;
        overflow: auto;
        .item {
          display: flex;
          align-items: center;
          margin: 4px 6px;
          height: 36px;
          padding: 0 8px 0 18px;
          border-radius: 6px;
          cursor: pointer;
          &:hover {
            background-color: @selected-hover-bg;
            .item-delete i {
              display: block;
            }
          }
          .item-name {
            flex: 1;
          }
          .item-delete {
            width: 14px;
            i {
              display: none;
              font-size: 14px;
            }
          }
          .item-default {
            margin-left: 12px;
            i {
              font-size: 14px;
            }

            .item-default-icon {
              color: @primary-color;
            }
          }
        }
      }
    }
  }
</style>
