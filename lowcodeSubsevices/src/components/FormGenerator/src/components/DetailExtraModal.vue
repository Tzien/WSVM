<template>
  <BasicModal v-bind="$attrs" width="600px" class="CERI-detail-extra-Modal" @register="registerModal" title="页签管理" @ok="handleSubmit" destroyOnClose>
    <div class="detail-extra-contain">
      <div class="detail-extra-header">
        <a-button type="link" preIcon="icon-ym icon-ym-btn-add" @click="addOrUpdateHandle()">添加</a-button>
      </div>
      <div class="detail-extra-list">
        <div class="detail-extra-item" v-for="(item, index) in detailExtraList" :key="item.id" v-if="detailExtraList.length">
          <div class="name">{{ item.fullName }}</div>
          <div class="action">
            <div class="edit-btn custom-line-icon" @click="addOrUpdateHandle(item)">
              <i class="icon-ym icon-ym-btn-edit" />
            </div>
            <div class="close-btn custom-line-icon ml-5px" @click="detailExtraList.splice(index, 1)">
              <i class="icon-ym icon-ym-btn-clearn" />
            </div>
          </div>
        </div>
        <ceri-empty class="mt-150px" v-else />
      </div>
    </div>
  </BasicModal>
  <AddDetailExtraModal @register="registerAddDetailExtraModal" @confirm="onConfirm" />
</template>
<script lang="ts" setup>
  import { reactive, toRefs } from 'vue';
  import { BasicModal, useModalInner, useModal } from '@/components/Modal';
  import AddDetailExtraModal from './AddDetailExtraModal.vue';
  import { cloneDeep } from 'lodash-es';
  import { buildUUID } from '@/utils/uuid';

  interface State {
    detailExtraList: any[];
    drawingList: any[];
  }

  const state = reactive<State>({
    detailExtraList: [],
    drawingList: [],
  });
  const { detailExtraList } = toRefs(state);
  const emit = defineEmits(['register', 'confirm']);
  const [registerModal, { closeModal }] = useModalInner(init);
  const [registerAddDetailExtraModal, { openModal: openAddDetailExtraModal }] = useModal();

  function init(data) {
    state.drawingList = cloneDeep(data.drawingList);
    state.detailExtraList = cloneDeep(data.detailExtraList || []);
  }
  function addOrUpdateHandle(data?) {
    openAddDetailExtraModal(true, { drawingList: state.drawingList, data });
  }
  function onConfirm(data) {
    if (data.id) {
      state.detailExtraList = state.detailExtraList.map(o => (o.id === data?.id ? { ...o, ...data } : o));
    } else {
      state.detailExtraList.push({ ...data, id: buildUUID() });
    }
  }
  function handleSubmit() {
    emit('confirm', state.detailExtraList);
    closeModal();
  }
</script>
<style lang="less">
  .CERI-detail-extra-Modal {
    .ant-modal-body {
      & > .scrollbar {
        padding: 20px;
        overflow: hidden;
        .detail-extra-contain {
          height: 450px;
          border-radius: 4px;
          border: 1px solid @border-color-base1;
          display: flex;
          flex-direction: column;
          .detail-extra-header {
            padding: 4px 0;
            border-bottom: 1px solid @border-color-base1;
            text-align: right;
          }
          .detail-extra-list {
            flex: 1;
            height: 100%;
            padding: 8px 0;
            overflow: auto;
            .detail-extra-item {
              display: flex;
              align-items: center;
              height: 36px;
              padding: 0 10px;
              .name {
                flex: 1;
                min-width: 0;
                overflow: hidden;
                white-space: nowrap;
                text-overflow: ellipsis;
              }
              .action {
                display: flex;
                .custom-line-icon {
                  line-height: 32px;
                  font-size: 22px;
                  padding: 0 4px;
                  color: #606266;
                  cursor: pointer;
                }
                i {
                  font-size: 18px !important;
                }
              }
            }
          }
        }
      }
    }
  }
</style>
