<template>
  <BasicModal
    v-bind="$attrs"
    @register="registerModal"
    defaultFullscreen
    :footer="null"
    :closable="false"
    :keyboard="false"
    class="ceri-full-modal designer-modal"
    style="width: 100vw; height: 100vh"
  >
    <template #title>
      <div class="ceri-full-modal-header">
        <div class="header-title">
          <img src="@/assets/images/NewLogo.png" class="header-logo" />
          <span class="header-dot"></span>
          <a-tooltip :title="fullName">
            <p class="header-txt">{{ fullName }}</p>
          </a-tooltip>
        </div>
        <a-steps :current="1" type="navigation" size="small" class="header-steps tab-steps">
          <a-step title="命名规范" />
        </a-steps>
        <a-space class="options" :size="10">
          <a-button type="primary" @click="handleSubmit()" :disabled="loading" :loading="btnLoading">{{ t('common.saveText') }}</a-button>
          <a-button @click="closeModal()">{{ t('common.closeText') }}</a-button>
        </a-space>
      </div>
    </template>
    <div class="ceri-content-wrapper">
      <div class="ceri-content-wrapper-center">
        <div class="ceri-content-wrapper-content bg-white p-10px">
          <a-alert :message="tipTxt" :description="descTxt" type="warning" show-icon class="mb-10px" ref="alertElRef" />
          <a-table :data-source="list" v-bind="getTableBindValues" v-model:expandedRowKeys="expandedRowKeys" ref="tableElRef">
            <template #expandedRowRender="{ record }">
              <a-table :data-source="record.fields" v-bind="getChildTableBindValues">
                <template #bodyCell="{ column, record }">
                  <template v-if="column.key === 'field'">
                    <a-input v-model:value="record.field" disabled detailed />
                  </template>
                  <template v-if="column.key === 'aliasName'">
                    <a-input v-model:value="record.aliasName" placeholder="请输入" :maxlength="100" />
                  </template>
                  <template v-if="column.key === 'fieldName'">
                    <a-input v-model:value="record.fieldName" disabled detailed />
                  </template>
                </template>
              </a-table>
            </template>
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'table'">
                <a-input v-model:value="record.table" disabled detailed />
              </template>
              <template v-if="column.key === 'aliasName'">
                <a-input v-model:value="record.aliasName" placeholder="请输入" :maxlength="100" />
              </template>
              <template v-if="column.key === 'comment'">
                <a-input v-model:value="record.comment" disabled detailed />
              </template>
            </template>
          </a-table>
        </div>
      </div>
    </div>
  </BasicModal>
</template>
<script lang="ts" setup>
// import { getAliasInfo, saveAlias } from '@/api/onlineDev/visualDev';
import { ref, reactive, toRefs, computed, unref, nextTick } from 'vue'
import { BasicModal, useModalInner } from '@/components/Modal'
import { useI18n } from 'vue-i18n'
import { useMessage } from '@/hooks/web/useMessage'

interface State {
  id: string
  fullName: string
  list: any[]
  loading: boolean
  btnLoading: boolean
  expandedRowKeys: string[]
}

defineEmits(['register'])
const tipTxt = '规范名称命名规则：（1）只能由字母、数字、下划线组成，且不能以数字开头；（2）不能使用系统关键字或开发语言关键字'
const descTxt =
  '系统关键字包含：tenantid、id、foreignid、flowid、flowtaskid、deleteuserid、deletetime、deletemark、version、tenant_id、foreign_id、flow_id、flow_task_id、delete_user_id、delete_time、delete_mark、f_tenant_id、f_id、f_foreign_id、f_flow_id、f_flow_task_id、f_delete_user_id、f_delete_time、f_delete_mark、f_version'
const { t } = useI18n()
const { createMessage } = useMessage()
const [registerModal, { closeModal, changeLoading }] = useModalInner(init)
const alertElRef = ref<any>(null)
const tableElRef = ref<any>(null)
const columns = [
  { width: 50, title: '序号', dataIndex: 'index', key: 'index', align: 'center', customRender: ({ index }) => index + 1 },
  { title: '表名', dataIndex: 'table', key: 'table' },
  { title: '规范名称', dataIndex: 'aliasName', key: 'aliasName' },
  { title: '说明', dataIndex: 'comment', key: 'comment' }
]
const childColumns = [
  { width: 50, title: '序号', dataIndex: 'index', key: 'index', align: 'center', customRender: ({ index }) => index + 1 },
  { title: '字段', dataIndex: 'field', key: 'field' },
  { title: '规范名称', dataIndex: 'aliasName', key: 'aliasName' },
  { title: '说明', dataIndex: 'fieldName', key: 'fieldName' }
]
const state = reactive<State>({
  id: '',
  fullName: '',
  list: [],
  loading: false,
  btnLoading: false,
  expandedRowKeys: []
})
const { list, loading, btnLoading, expandedRowKeys, fullName } = toRefs(state)

const getHeaderHeight = computed(() => {
  const alertEl = alertElRef.value?.$el
  if (!alertEl) return 120
  return alertEl?.offsetHeight
})
const getScrollY = computed(() => window.innerHeight - unref(getHeaderHeight) - 150)
const getTableBindValues = computed(() => {
  return {
    columns,
    pagination: false,
    size: 'small',
    rowKey: 'table',
    scroll: { y: unref(getScrollY) }
  }
})
const getChildTableBindValues = computed(() => {
  return {
    columns: childColumns,
    pagination: false,
    size: 'small',
    rowKey: 'field'
  }
})

function init(data) {
  state.id = data.id
  state.fullName = data.fullName || ''
  // state.loading = true
  state.expandedRowKeys = []
  // changeLoading(true)

  // getAliasInfo(data.id).then(res => {
  //   state.list = res.data;
  //   state.expandedRowKeys = res.data.map(e => e.table);
  //   nextTick(() => {
  //     const tableEl = tableElRef.value?.$el;
  //     let bodyEl = tableEl.querySelector('.ant-table-body');
  //     bodyEl!.style.height = `${unref(getScrollY)}px`;
  //     changeLoading(false);
  //     state.loading = false;
  //   });
  // });
}
function handleSubmit() {
  state.btnLoading = true
  changeLoading(true)
  // saveAlias(state.id, { tableList: state.list })
  //   .then(res => {
  //     state.btnLoading = false;
  //     changeLoading(false);
  //     createMessage.success(res.msg);
  //     closeModal();
  //   })
  //   .catch(() => {
  //     state.btnLoading = false;
  //     changeLoading(false);
  //   });
}
</script>

