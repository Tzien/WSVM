<template>
  <BasicModal v-bind="$attrs" @register="registerModal" defaultFullscreen wrap-class-name="code-preview-fullscreen-wrap"
    :footer="null" :closable="false" :keyboard="false" class="ceri-full-modal full-modal designer-modal">
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
          <a-step title="代码预览" />
        </a-steps>

        <a-space class="options" :size="10">
          <a-button @click="openDiffPreviewModal(true, { ...data })">代码对比</a-button>

          <a-button @click="closeModal()">{{ t('common.closeText') }}</a-button>
        </a-space>
      </div>
    </template>

    <div class="ceri-content-wrapper">
      <div class="ceri-content-wrapper-left" style="overflow-y: auto;">
        <BasicLeftTree :showSearch="false" ref="leftTreeRef" :fieldNames="{ title: 'fileName' }" :treeData="treeData"
          @select="handleTreeSelect" />
      </div>

      <div class="ceri-content-wrapper-center">
        <div class="ceri-content-wrapper-content bg-white">
          <MonacoEditor v-model="currentContent" :options="editorOptions" :language="editorLanguage" :key="key" />
        </div>
      </div>
    </div>
  </BasicModal>

  <DiffPreviewModal @register="registerDiffPreviewModal" />
</template>

<script lang="ts" setup>
import { codePreview } from '@/api/onlineDev/visualDev'
import { reactive, toRefs, nextTick, ref, unref, watch, onBeforeUnmount } from 'vue'
import { BasicModal, useModal, useModalInner } from '@/components/Modal'

import { useI18n } from 'vue-i18n'

import { BasicLeftTree, TreeActionType } from '@/components/Tree'

import { MonacoEditor } from '@/components/CodeEditor'

import DiffPreviewModal from './DiffPreviewModal.vue'
import { message } from 'ant-design-vue'

interface State {
  treeData: any[]
  currentId: string
  currentContent: string
  editorOptions: any
  editorLanguage: string
  key: number
  fullName: string
  data: any
}

defineEmits(['register'])
const { t } = useI18n()
const leftTreeRef = ref<Nullable<TreeActionType>>(null)
const [registerModal, { closeModal, changeLoading, getOpen }] = useModalInner(init)
const state = reactive<State>({
  treeData: [],
  currentId: '',
  currentContent: '',
  editorOptions: {
    readOnly: true,
    scrollBeyondLastLine: true,
    minimap: {
      enabled: true
    }
  },

  editorLanguage: 'html',

  key: +new Date(),

  fullName: '',

  data: {}
})

const { treeData, currentContent, editorOptions, editorLanguage, key, fullName, data } = toRefs(state)

const [registerDiffPreviewModal, { openModal: openDiffPreviewModal }] = useModal()

const lockState = ref(false)
const SCROLL_LOCK_COUNT_ATTR = 'data-code-preview-scroll-lock-count'
const PREV_BODY_OVERFLOW_ATTR = 'data-code-preview-prev-body-overflow'
const PREV_HTML_OVERFLOW_ATTR = 'data-code-preview-prev-html-overflow'

function togglePageScrollLock(locked: boolean) {
  if (typeof document === 'undefined') return

  const body = document.body
  const html = document.documentElement
  const currentCount = Number(body.getAttribute(SCROLL_LOCK_COUNT_ATTR) || 0)

  if (locked) {
    if (currentCount === 0) {
      body.setAttribute(PREV_BODY_OVERFLOW_ATTR, body.style.overflow || '')
      html.setAttribute(PREV_HTML_OVERFLOW_ATTR, html.style.overflow || '')
      body.style.overflow = 'hidden'
      html.style.overflow = 'hidden'
    }

    body.setAttribute(SCROLL_LOCK_COUNT_ATTR, String(currentCount + 1))
    return
  }

  const nextCount = Math.max(currentCount - 1, 0)
  if (nextCount === 0) {
    body.style.overflow = body.getAttribute(PREV_BODY_OVERFLOW_ATTR) || ''
    html.style.overflow = html.getAttribute(PREV_HTML_OVERFLOW_ATTR) || ''
    body.removeAttribute(PREV_BODY_OVERFLOW_ATTR)
    html.removeAttribute(PREV_HTML_OVERFLOW_ATTR)
    body.removeAttribute(SCROLL_LOCK_COUNT_ATTR)
    return
  }

  body.setAttribute(SCROLL_LOCK_COUNT_ATTR, String(nextCount))
}

watch(
  () => getOpen?.value,
  (open) => {
    if (open && !lockState.value) {
      togglePageScrollLock(true)
      lockState.value = true
      return
    }

    if (!open && lockState.value) {
      togglePageScrollLock(false)
      lockState.value = false
    }
  },
  { immediate: true }
)

onBeforeUnmount(() => {
  if (!lockState.value) return
  togglePageScrollLock(false)
  lockState.value = false
})

function init(data) {
  state.fullName = data.fullName || ''
  state.data = data
  state.treeData = []
  state.currentId = ''
  state.currentContent = ''
  state.key = +new Date()
  changeLoading(true)
  const query = {
    module: data.module || 'system',

    description: data.description,

    modulePackageName: data.modulePackageName || '',

    enableFlow: data.enableFlow
  }

  codePreview(data.id, query)
    .then((res) => {
      if (res.success && res.code == 200) {
        message.success(res.message)
        const list = res.data || []
        state.treeData = list.map((o) => ({ ...o, disabled: true }))
        const firstNode = state.treeData?.[0]?.children?.[0]
        if (!firstNode) return
        state.currentId = firstNode.id
        state.currentContent = firstNode.fileContent
        state.editorLanguage = getLanguage(firstNode)
        nextTick(() => {
          const leftTree = unref(leftTreeRef)
          leftTree?.setSelectedKeys([state.currentId])
        })
      }
    })

    .finally(() => {
      changeLoading(false)
    })
}

function handleTreeSelect(id, node) {
  state.key = +new Date()

  state.currentId = id

  state.currentContent = node.fileContent

  state.editorLanguage = getLanguage(node)
}

function getLanguage(node) {
  return ['web', 'app'].includes(node.fileType) ? 'html' : ['js', 'ts'].includes(node.folderName) ? 'js' : 'java'
}
</script>

<style lang="less">
.code-preview-fullscreen-wrap {
  overflow: hidden !important;

  overscroll-behavior: none;

  .ant-modal {
    top: 0 !important;

    margin: 0 !important;

    padding-bottom: 0 !important;

    width: 100vw !important;

    max-width: 100vw !important;
  }

  .ant-modal-header {
    padding: 0;

    .ant-modal-title {
      font-weight: normal;
    }
  }

  .ant-modal-content {
    height: 100vh;

    border-radius: 0;

    display: flex;

    flex-direction: column;
  }

  .ant-modal-body {
    flex: 1;

    min-height: 0;

    padding: 0 !important;

    overflow: hidden;

    background-color: @app-main-background;

    & > .scrollbar {
      height: 100%;

      padding: 10px;

      box-sizing: border-box;

      min-height: 0;

      overflow: hidden;

      background-color: @app-main-background;
    }

    & > .scrollbar__wrap {
      height: 100%;

      margin-bottom: 0 !important;

      overflow-x: hidden;

      & > .scrollbar_view {
        height: auto;

        min-height: 100%;

        & > div {
          height: auto;

          min-height: 100%;
        }
      }
    }
  }

  .ceri-full-modal-header {
    padding: 0 20px;

    height: 60px;

    display: flex;

    justify-content: space-between;

    align-items: center;

    .header-title {
      height: 60px;

      width: 320px;

      min-width: 0;

      display: flex;

      align-items: center;

      .header-logo {
        display: inline-block;

        vertical-align: top;

        font-size: 30px;
      }

      .header-dot {
        display: inline-block;

        position: relative;

        margin: 0 10px;

        width: 7px;

        height: 7px;

        background: rgba(181, 215, 255, 0.4);

        border-radius: 50%;

        &::after {
          content: '';

          display: block;

          position: absolute;

          width: 3px;

          height: 3px;

          background: #6a9cfa;

          border-radius: 50%;

          left: 2px;

          top: 2px;
        }
      }

      .header-txt {
        line-height: 60px;

        display: inline-block;

        margin: 0;

        font-size: 16px;

        max-width: 150px;

        white-space: nowrap;

        overflow: hidden;

        text-overflow: ellipsis;

        cursor: pointer;
      }
    }

    .options {
      width: 320px;

      justify-content: flex-end;
    }

    .header-steps {
      height: 40px;

      line-height: 40px;

      background: rgba(255, 255, 255, 0.4);

      border-radius: 20px;

      overflow: hidden;

      &.ant-steps {
        width: auto;

        &.tab-steps {
          .ant-steps-item {
            margin: 0;

            .ant-steps-item-icon {
              display: none;
            }

            .ant-steps-item-container {
              padding: 0 20px;
            }

            &::after {
              display: none;
            }
          }
        }

        .ant-steps-item {
          width: auto;

          padding-inline-start: 0;

          margin: 0 16px;

          &:first-child {
            margin-left: 0;
          }

          &:last-child {
            margin-right: 0;
          }

          &.ant-steps-item-active {
            &::before {
              display: none;
            }

            .ant-steps-item-container {
              background-color: #fff;

              border-radius: 20px;
            }
          }

          .ant-steps-item-container {
            margin-inline-start: 0;

            padding-bottom: 0;

            display: flex;

            align-items: center;
          }
        }
      }
    }
  }

  .ceri-content-wrapper {
    height: 100%;

    min-height: 0;

    width: 100%;

    display: flex;

    position: relative;

    border-radius: 8px;

    overflow: hidden;

    .ceri-content-wrapper-left {
      width: 220px;

      background-color: @component-background;

      flex-shrink: 0;

      height: 100%;

      display: flex;

      flex-direction: column;

      margin-right: 10px;

      border-radius: 8px;

      overflow: hidden;
    }

    .ceri-content-wrapper-center {
      flex: 1;

      min-width: 0;

      min-height: 0;

      height: 100%;

      overflow: hidden;

      display: flex;

      flex-direction: column;

      .ceri-content-wrapper-content {
        border-radius: 8px;

        flex: 1;

        min-height: 0;

        overflow: auto;
      }
    }
  }
}
</style>
