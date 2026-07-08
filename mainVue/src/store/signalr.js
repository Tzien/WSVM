import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useSignalRStore = defineStore(
  'signalr',
  () => {
    // SignalR 连接（不要持久化）
    const connection = ref(null)

    // 业务状态（可以持久化）
    const msgCount = ref(0)
    const idsUserId = ref(null)
    const ptUserId = ref(null)
    const durationTime = ref(null)
    const isShow = ref(false)
    const msgId = ref(null)

    // ⚠️ UI 实例：一定不要持久化
    const currentNotification = ref(null)

    /* setters */
    const setConnection = (conn) => {
      connection.value = conn
    }

    const setMsgCount = (count) => {
      msgCount.value = count
    }

    const setIdsUserId = (id) => {
      idsUserId.value = id
    }

    const setPtUserId = (id) => {
      ptUserId.value = id
    }

    const setDurationTime = (val) => {
      durationTime.value = val
    }

    const setIsShow = (val) => {
      isShow.value = val
    }

    const setMsgId = (val) => {
      msgId.value = val
    }

    const setCurrentNotification = (val) => {
      currentNotification.value = val
    }

    // ✅ 安全关闭通知（推荐用这个）
    const closeCurrentNotification = () => {
      if (currentNotification.value && typeof currentNotification.value.close === 'function') {
        currentNotification.value.close()
      }
      currentNotification.value = null
    }
    const needRefreshMsg = ref(false)
    const triggerRefreshMsg = () => {
      needRefreshMsg.value = !needRefreshMsg.value
    }
    return {
      // state
      connection,
      msgCount,
      idsUserId,
      ptUserId,
      durationTime,
      isShow,
      msgId,
      currentNotification,
      needRefreshMsg,
      // actions
      triggerRefreshMsg,
      setConnection,
      setMsgCount,
      setIdsUserId,
      setPtUserId,
      setDurationTime,
      setIsShow,
      setMsgId,
      setCurrentNotification,
      closeCurrentNotification
    }
  },
  {
    // ✅ 只持久化「纯数据」
    persist: {
      paths: ['msgCount', 'idsUserId', 'ptUserId', 'durationTime', 'isShow', 'msgId']
    }
  }
)
