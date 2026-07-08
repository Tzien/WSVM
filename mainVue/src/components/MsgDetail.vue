<template>
  <!-- 消息详情弹框 -->
  <a-modal v-model:open="open" :title="modalTitle" centered :keyboard="false" width="30%" :footer="null" @cancel="cancel">
    <a-form ref="formRef" :model="formState" :label-col="labelCol" style="height: 500px">
      <!-- <a-form-item label="消息类型">
                <a-input disabled v-model:value="formState.msgType" style="width: 300px" />
            </a-form-item> -->
      <a-form-item :label="$t('home.Title')" name="code">
        <a-input disabled v-model:value="formState.title" style="width: 300px" />
      </a-form-item>
      <a-form-item :label="$t('home.Content')">
        <a-textarea disabled v-model:value="formState.content" style="width: 300px" :autosize="{ minRows: 12, maxRows: 15 }" />
      </a-form-item>
      <a-form-item :label="$t('home.Sender')">
        <a-input disabled v-model:value="formState.sendUser" style="width: 300px" />
      </a-form-item>
      <a-form-item :label="$t('home.SentTime')">
        <a-input disabled v-model:value="formState.sendTime" style="width: 300px" />
      </a-form-item>
      <!-- <a-form-item label="是否已读">
                <a-input disabled v-model:value="formState.isRead" style="width: 300px" />
            </a-form-item>
            <a-form-item label="读取时间">
                <a-input disabled v-model:value="formState.msgReadTime" style="width: 300px" />
            </a-form-item> -->
    </a-form>
    <div v-show="formState.isRead == t('home.Unread')" style="text-align: center">
      <a-button @click="readMsg">{{ $t('home.Received') }}</a-button>
    </div>
  </a-modal>
</template>

<script setup>
import { useSignalRStore } from '@/store/index.js'
import { ref, reactive, onMounted, watch, onBeforeUnmount, watchEffect, computed } from 'vue'
import { readMsgApi, getMsgDetailApi, getNoReadMsgApi } from '@/api/Msg/msg'

import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const signalRStore = useSignalRStore()

const open = ref(true)
onMounted(() => {
  detail()
})
const modalTitle = computed(() => {
  return t('home.MessageDetails') + '-' + formState.isRead
})
//消息详情弹框
const formState = reactive({
  content: '',
  title: '',
  msgType: '',
  sendTime: '',
  sendUser: '',
  isRead: '',
  msgReadTime: ''
})
const formRef = ref()
const labelCol = {
  style: {
    width: '150px'
  }
}
async function detail() {
  formState.content = ''
  formState.title = ''
  formState.msgType = ''
  formState.sendTime = ''
  formState.sendUser = ''
  formState.isRead = ''
  formState.msgReadTime = ''
  const params = {
    msgId: signalRStore.msgId
  }
  const response = await getMsgDetailApi(params)
  if (response.code === 200 && response.success) {
    formState.content = response.data.content
    formState.title = response.data.title
    if (response.data.msgType == '1') formState.msgType = '公告信息通知'
    else if (response.data.msgType == '2') formState.msgType = '微信消息通知'
    else if (response.data.msgType == '3') formState.msgType = '钉钉消息通知'
    formState.sendTime = response.data.sendTime.replace('T', ' ')
    formState.sendUser = response.data.sendUser
    formState.isRead = response.data.isRead ? t('home.Read') : t('home.Unread')
    if (response.data.msgReadTime !== null) formState.msgReadTime = response.data.msgReadTime.replace('T', ' ')
  }
}

const cancel = async () => {
  open.value = true
  signalRStore.setIsShow(false)
}

//阅读消息
const readMsg = async () => {
  signalRStore.currentNotification.close('aaa')
  // signalRStore.setIsShow(false)
  const query = {
    msgId: signalRStore.msgId
  }
  await readMsgApi(query).then((res) => {
    if (res.code === 200 && res.success) {
      getMsg(signalRStore.idsUserId) 
      // 通知页面刷新
      signalRStore.triggerRefreshMsg()
      signalRStore.setIsShow(false)
    }
  })
  signalRStore.setMsgId(null)
}
const getMsg = async (id) => {
  const query = {
    userId: id
  }
  await getNoReadMsgApi(query).then((res) => {
    if (res.code === 200 && res.success) {
      signalRStore.setMsgCount(res.total)
    }
  })
}
</script>
