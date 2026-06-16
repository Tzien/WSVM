import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore(
  'user',
  () => {
    const realname=ref('')
    const userid = ref('')
    const loginname = ref('')
    const access_token = ref('')
    const refresh_token = ref('')
    const expires_in = ref('')
    const expiration_timestamp = ref(0)
    const userRoles = ref(['0'])
    const setUserMessage = (
      realName,
      userId,
      loginName,
      accessToken,
      refreshToken,
      expiresIn,
      timestamp,
      userRole
    ) => {
      realname.value = realName
      userid.value = userId
      loginname.value = loginName
      access_token.value = accessToken
      expires_in.value = expiresIn
      refresh_token.value = refreshToken
      expiration_timestamp.value = timestamp
      userRoles.value = userRole
    }
    const removeUserMessage = () => {
      realname.value = ''
      userid.value = ''
      loginname.value = ''
      access_token.value = ''
      expires_in.value = ''
      refresh_token.value = ''
      expiration_timestamp.value = 0
      userRoles.value = ['0']
    }
    return {
      realname,
      loginname,
      access_token,
      expires_in,
      refresh_token,
      expiration_timestamp,
      userRoles,
      setUserMessage,
      removeUserMessage,
    }
  },
  {
    persist: true,
  }
)
