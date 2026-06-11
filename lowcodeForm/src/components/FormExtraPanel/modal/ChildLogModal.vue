<template>
  <BasicModal v-bind="$attrs" width="1000px" class="child-log-Modal" @register="registerModal" title="修改详情" :footer="null" destroyOnClose>
    <a-table size="small" :data-source="list" :columns="columnList" bordered :pagination="false" :customRow="customRow" :scroll="{ y: 'calc(60vh - 40px)' }">
      <template #bodyCell="{ column, record }">
        <template v-if="record.ceri_type === 0">
          <div class="min-h-20px">
            <span :class="getCellClass(column, record)" @click="showMore(column, record)" v-if="record[column.prop]">
              {{ getCellText(column, record) }}
            </span>
          </div>
        </template>
        <template v-else-if="record.ceri_type === 2">
          <div class="min-h-20px">
            <span :class="getCellClass(column, record)" @click="showMore(column, record)" v-if="record['ceri_old_' + column.prop]">
              {{ getCellText(column, record) }}
            </span>
          </div>
        </template>
        <template v-else>
          <template v-if="record[column.prop] == record['ceri_old_' + column.prop]">
            <div class="min-h-20px">
              <span :class="getCellClass(column, record)" @click="showMore(column, record)" v-if="record[column.prop]">
                {{ getCellText(column, record) }}
              </span>
            </div>
          </template>
          <template v-else>
            <template v-if="column.nameModified">
              <span class="value-cell-modify">已修改</span>
            </template>
            <template v-else-if="column.ceriKey === 'sign'">
              <span class="value-cell-more line-through" v-if="record['ceri_old_' + column.prop]" @click="handlePreview(record['ceri_old_' + column.prop])">
                旧签名
              </span>
              <div class="h-6px" v-if="record['ceri_old_' + column.prop] && record[column.prop]"></div>
              <span class="value-cell-more" v-if="record[column.prop]" @click="handlePreview(record[column.prop])">新签名</span>
            </template>
            <template v-else-if="column.ceriKey === 'signature'">
              <span
                class="value-cell-more line-through"
                v-if="record['ceri_old_' + column.prop]"
                @click="handlePreview(apiUrl + record['ceri_old_' + column.prop])">
                旧签章
              </span>
              <div class="h-6px" v-if="record['ceri_old_' + column.prop] && record[column.prop]"></div>
              <span class="value-cell-more" v-if="record[column.prop]" @click="handlePreview(apiUrl + record[column.prop])">新签章</span>
            </template>
            <template v-else>
              <span class="value-cell-old value-cell" v-if="record['ceri_old_' + column.prop]">{{ record['ceri_old_' + column.prop] }}</span>
              <div class="h-6px" v-if="record['ceri_old_' + column.prop] && record[column.prop]"></div>
              <span class="value-cell-new value-cell" v-if="record[column.prop]">{{ record[column.prop] }}</span>
            </template>
          </template>
        </template>
      </template>
    </a-table>
  </BasicModal>
</template>
<script lang="ts" setup>
  import { reactive, toRefs, ref } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { createImgPreview } from '@/components/Preview/index';
  import { useGlobSetting } from '@/hooks/setting';

  interface State {
    list: any[];
    columnList: any[];
  }

  const state = reactive<State>({
    list: [],
    columnList: [],
  });
  const { list, columnList } = toRefs(state);
  const globSetting = useGlobSetting();
  const apiUrl = ref(globSetting.apiUrl);

  const emit = defineEmits(['register', 'confirm']);
  const [registerModal] = useModalInner(init);

  function customRow(record) {
    if (record.ceri_type === 2) return { style: { background: '#fca0a0' } };
    if (record.ceri_type === 0) return { style: { background: '#aeeec2' } };
  }
  function getCellText(column, record) {
    if (column.ceriKey === 'sign') {
      return record.ceri_type === 2 ? '旧签名' : record.ceri_type === 1 ? '签名' : '新签名';
    }
    if (column.ceriKey === 'signature') {
      return record.ceri_type === 2 ? '旧签章' : record.ceri_type === 1 ? '签章' : '新签章';
    }
    if (column.nameModified) {
      return record[column.prop] == record['ceri_old_' + column.prop] ? '未修改' : '已修改';
    }
    return record.ceri_type === 2 ? record['ceri_old_' + column.prop] : record[column.prop];
  }
  function getCellClass(column, record) {
    return {
      'line-through': record.ceri_type === 2,
      'value-cell-more': column.ceriKey && ['sign', 'signature'].includes(column.ceriKey),
      'value-cell-modify': !!column.nameModified,
    };
  }
  function showMore(column, record) {
    if (!column.ceriKey || !['sign', 'signature'].includes(column.ceriKey)) return;
    const prefix = column.ceriKey === 'sign' ? '' : apiUrl.value;
    const url = record.ceri_type === 2 ? record['ceri_old_' + column.prop] : record[column.prop];
    if (!url) return;
    const imgUrl = prefix + url;
    handlePreview(imgUrl);
  }
  function handlePreview(image) {
    createImgPreview({ imageList: [image] });
  }
  function init(data) {
    state.list = data.chidData || [];
    state.columnList = data.chidField.map(o => ({ ...o, title: o.label, align: 'center', dataIndex: o.prop, width: 150 }));
  }
</script>
<style lang="less">
  .child-log-Modal {
    .ant-modal-body {
      height: 60vh;
      & > .scrollbar {
        padding: 0;
      }
    }
    .ant-table-wrapper .ant-table-tbody > tr > td.ant-table-cell-row-hover,
    .ant-table-wrapper .ant-table-tbody > tr > td.ant-table-cell {
      min-height: 39px !important;
      background: inherit !important;
    }
    .value-cell {
      display: inline-block;
      padding: 2px 10px;
      border-radius: 4px;
      word-break: break-all;
    }
    .value-cell-old {
      background: #fca0a0;
      text-decoration: line-through;
    }
    .value-cell-new {
      background-color: #aeeec2;
    }
    .value-cell-more {
      color: @primary-color;
      cursor: pointer;
    }
    .value-cell-modify {
      color: @warning-color;
    }
  }
</style>
