<template>
  <BasicModal v-bind="$attrs" width="600px" class="CERI-ai-field-Modal" @register="registerModal" title="推荐字段" @ok="handleSubmit" destroyOnClose>
    <a-table
      size="small"
      rowKey="fieldName"
      :data-source="list"
      :columns="columns"
      :pagination="false"
      :row-selection="{ columnWidth: 50, selectedRowKeys: selectedRowKeys, onChange: onSelectChange }">
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'label'">
          <ceri-input v-model:value="record.fieldTitle" placeholder="请输入" allowClear :maxlength="100" />
        </template>
        <template v-if="column.key === 'ceriKey'">
          <ceri-select
            v-model:value="record.fieldComponent"
            :fieldNames="{ label: 'label', value: 'ceriKey', options: 'options1' }"
            :options="getCeriKeyOptions"
            showSearch
            allowClear />
        </template>
      </template>
    </a-table>
  </BasicModal>
</template>
<script lang="ts" setup>
  import { nextTick, reactive, toRefs, computed } from 'vue';
  import { BasicModal, useModalInner } from '@/components/Modal';
  import { cloneDeep } from 'lodash-es';
  import { inputComponents, selectComponents, systemComponents } from '../helper/componentMap';
  import { buildAiFields } from '@/components/FormGenerator/src/helper/aiUtils';
  import { vModelIgnoreList } from '@/components/FormGenerator/src/helper/config';
  import { useMessage } from '@/hooks/web/useMessage';

  interface State {
    list: any[];
    selectedRowKeys: string[];
  }

  const state = reactive<State>({
    list: [],
    selectedRowKeys: [],
  });
  const { list, selectedRowKeys } = toRefs(state);
  const { createMessage } = useMessage();
  const emit = defineEmits(['register', 'reload']);
  const [registerModal, { closeModal }] = useModalInner(init);
  const columns = [
    { title: '字段名称', dataIndex: 'label', key: 'label', width: 200 },
    { title: '字段类型', dataIndex: 'ceriKey', key: 'ceriKey', width: 200 },
  ];
  const ceriKeyOptions = [...inputComponents, ...selectComponents, ...systemComponents];
  const ignoreList = ['table', 'relationFormAttr', 'popupAttr', ...vModelIgnoreList];

  const getCeriKeyOptions = computed(() =>
    ceriKeyOptions.filter(o => !ignoreList.includes(o.__config__.ceriKey)).map(o => ({ ...o, label: o.__config__.label, ceriKey: o.__config__.ceriKey })),
  );

  function init(data) {
    state.list = cloneDeep(data.list).filter(o => o.isMain)[0]?.fields || [];
    state.selectedRowKeys = state.list.map(o => o.fieldName);
  }
  function onSelectChange(selectedRowKeys) {
    state.selectedRowKeys = selectedRowKeys || [];
  }
  function handleSubmit() {
    const list = state.list.filter(o => state.selectedRowKeys.includes(o.fieldName));
    const fields = buildAiFields([{ fields: list, isMain: true }]);
    if (!fields.length) return createMessage.warning('请至少选择一个字段');
    emit('reload', fields);
    nextTick(() => closeModal());
  }
</script>
<style lang="css">
  .CERI-ai-field-Modal {
    .scrollbar {
      padding: 0 !important;
    }
    .ant-table-wrapper {
      min-height: 400px;
    }
  }
</style>
