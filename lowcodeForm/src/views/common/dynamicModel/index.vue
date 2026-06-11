<template>
  <component :is="currentView" :config="config" :modelId="modelId" :isPreview="isPreview" :isDataManage="isDataManage" v-if="showPage" />
</template>
<script lang="ts" setup>
import { reactive, onMounted, toRefs, markRaw } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getConfigData } from '@/api/demoApi/demo'
// import { getFlowStartFormId } from '@/api/workFlow/template';
import { useMessage } from '@/hooks/web/useMessage'
// import { useTabs } from '@/hooks/web/useTabs';
import { useBaseStore } from '@/store/base'
import Form from './form/index.vue'
import List from './list/index.vue'

interface State {
  currentView: any
  showPage: boolean
  isPreview: boolean
  isDataManage: boolean
  previewType: string
  modelId: string
  flowId: string
  enableFlow: number
  config: any
}

defineOptions({ name: 'dynamicModel' })
const { createMessage } = useMessage()
const baseStore = useBaseStore()
// const { close } = useTabs();
const state = reactive<State>({
  currentView: '',
  showPage: false,
  isPreview: false,
  isDataManage: false,
  previewType: '',
  modelId: '',
  flowId: '',
  enableFlow: 0,
  config: {}
})
const { currentView, showPage, isPreview, isDataManage, modelId, config } = toRefs(state)
const router = useRouter()

async function init() {
  const route = useRoute()
  await baseStore.getDictionaryAll();
  state.isPreview = (route.query.isPreview as unknown as boolean) || false
  state.isDataManage = (route.query.isDataManage as unknown as boolean) || false
  if (state.isPreview || state.isDataManage) {
    if (state.isPreview) {
      state.previewType = (route.query.previewType as string) || ''
    }
    getConfig(route.query.id)
    return
  }
  state.enableFlow = route.meta.type === 9 ? 1 : 0
  if (!state.enableFlow) return getConfig(route.meta.relationId)
  getModelId(route.meta.relationId)
}
function getModelId(flowId) {
  state.flowId = flowId
  // getFlowStartFormId(flowId)
  //   .then(res => {
  //     if (!res?.data || !res?.data.formId) return;
  //     getConfig(res.data.formId);
  //   })
  //   .catch(() => {
  //     close();
  //     router.replace('/404');
  //   });
}
async function getConfig(modelId) {
  if (!modelId) return
  state.modelId = modelId
  await getConfigData(state.modelId, state.previewType).then((res) => {
    if (res.code !== 200 || !res.data) {
      // close();
      router.replace('/404')
      createMessage.error(res.msg || '请求出错，请重试')
      return
    }
    state.config = res.data
    state.config.id = state.config.id || state.modelId
    if (state.enableFlow) {
      state.config.enableFlow = state.enableFlow
      state.config.flowId = state.flowId
    }
    state.currentView = res.data.webType == '1' ? markRaw(Form) : markRaw(List)
    state.showPage = true
  })
}

onMounted(() => {
  init()
})
</script>
