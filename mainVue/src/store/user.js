import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore(
  'user',
  () => {
    const realname = ref('')
    const userid = ref('')
    const userloginlogid = ref('')
    const loginname = ref('')
    const access_token = ref('')
    const refresh_token = ref('')
    const expires_in = ref('')
    const expiration_timestamp = ref(0)
    const userRoles = ref(['0'])
    const isLoading = ref(false)
    // 记录各子应用（按 sysCode）是否已完成首次加载/挂载
    const subAppReady = ref({})
    const allSysCodeUrl = ref([])
    const allSysCode = ref([])
    const searchKey = ref([])
    const setUserMessage = (realName, userId, loginName, accessToken, refreshToken, expiresIn, timestamp, userRole) => {
      realname.value = realName
      userid.value = userId
      loginname.value = loginName
      access_token.value = accessToken
      expires_in.value = expiresIn
      refresh_token.value = refreshToken
      expiration_timestamp.value = timestamp
      userRoles.value = userRole
    }
    const setlogid = (userLoginLogId) => {
      userloginlogid.value = userLoginLogId
    }
    const removeUserMessage = () => {
      realname.value = ''
      userid.value = ''
      userloginlogid.value = ''
      loginname.value = ''
      access_token.value = ''
      expires_in.value = ''
      refresh_token.value = ''
      expiration_timestamp.value = 0
      userRoles.value = ['0']
      subAppReady.value = {}
      allSysCodeUrl.value = []
      allSysCode.value = []
      searchKey.value = []
    }
    const setSysCodeUrlItem = (sysCodeUrl) => {
      if (!allSysCodeUrl.value.includes(sysCodeUrl)) {
        allSysCodeUrl.value.push(sysCodeUrl)
      }
    }
    const setSysCodeItem = (sysCode) => {
      if (!allSysCode.value.includes(sysCode)) {
        allSysCode.value.push(sysCode)
      }
    }
    const addSearchKey = (key) => {
      if (!searchKey.value.some((a) => a == key)) {
        if (searchKey.value.length >= 10) {
          searchKey.value.shift()
        }
        searchKey.value.push(key)
      }
    }
    const setSubAppReady = (sysCode, ready) => {
      if (!sysCode) return
      subAppReady.value = {
        ...subAppReady.value,
        [sysCode]: !!ready
      }
    }
    return {
      realname,
      userid,
      userloginlogid,
      loginname,
      access_token,
      expires_in,
      refresh_token,
      expiration_timestamp,
      userRoles,
      setUserMessage,
      setlogid,
      removeUserMessage,
      isLoading,
      subAppReady,
      allSysCode,
      allSysCodeUrl,
      setSysCodeUrlItem,
      setSysCodeItem,
      searchKey,
      addSearchKey,
      setSubAppReady
    }
  },
  {
    persist: true
  }
)
