<template>
  <a-row :gutter="15" class="extra-relation-info" v-if="getExtraList.length">
    <template v-for="item in getExtraList">
      <a-col :span="24" class="extra-relation-info-cell-table" v-if="item.ceriKey === 'table'">
        <div class="extra-cell-label my-6px">{{ item.label }}</div>
        <a-table :data-source="data[item.value] || []" :columns="item.children" size="small" :pagination="false" :scroll="{ y: '300px' }" class="flex-1">
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex !== 'index'">{{ record[column.dataIndex] }}</template>
          </template>
          <template #emptyText>
            <p class="leading-30px">暂无数据</p>
          </template>
        </a-table>
      </a-col>
      <a-col :span="12" class="extra-relation-info-cell" v-else>
        <div class="extra-cell-label">{{ item.label }}</div>
        <div class="extra-cell-value">{{ data[item.value] || '' }}</div>
      </a-col>
    </template>
  </a-row>
</template>

<script lang="ts" setup>
  import { computed } from 'vue';

  defineOptions({ name: 'ExtraRelationInfo', inheritAttrs: false });
  const props = defineProps({
    data: { type: Object, default: () => ({}) },
    extraOptions: { type: Object, default: () => [] },
  });

  const indexColumn = { width: 50, title: '序号', dataIndex: 'index', key: 'index', align: 'center', fixed: 'left', customRender: ({ index }) => index + 1 };

  const getExtraList = computed(() => {
    const extraOptions = props.extraOptions.filter(o => !!o.value);
    let list: any[] = [];
    for (let i = 0; i < extraOptions.length; i++) {
      const e = extraOptions[i];
      if (!e.value.includes('-')) {
        list.push(e);
      } else {
        let value = e.value.split('-')[0];
        let childValue = e.value.split('-')[1];
        let label = e.label.split('-')[0];
        let childLabel = e.label.replace(label + '-', '');
        let newItem = {
          ceriKey: 'table',
          value,
          label,
          title: label,
          dataIndex: value,
          children: [indexColumn],
        };
        e.dataIndex = childValue;
        e.title = childLabel;
        e.width = 200;
        if (!list.some(o => o.value === value)) list.push(newItem);
        for (let i = 0; i < list.length; i++) {
          if (list[i].value === value) {
            list[i].children.push(e);
            break;
          }
        }
      }
    }
    return list;
  });
</script>
<style lang="less" scoped>
  .extra-relation-info {
    .extra-relation-info-cell {
      display: flex;
      align-items: center;
      justify-content: center;
      line-height: 32px;
      font-size: 14px;
      width: 100%;
      overflow: hidden;
      margin-top: 10px;
    }
    .extra-cell-label {
      flex-shrink: 0;
      font-size: 14px;
    }
    .extra-cell-value {
      flex: 1;
      min-width: 20px;
      margin-left: 10px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }
  }
</style>
