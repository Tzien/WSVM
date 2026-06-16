import axios from 'axios'
import route from '../router/index'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore } from '../store/user'

export function createAxios(url, userStore) {
  const service = axios.create({
    baseURL: url,
    timeout: 10 * 1000
  })
  service.interceptors.request.use(
    (config) => {
      // 携带token
      if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
        userStore = useUserStore()
      }
      const currentTimeMillis = new Date().getTime()
      const current = Math.floor(currentTimeMillis / 1000)
      const expire = userStore.expiration_timestamp
      if (userStore.access_token == '' && current >= expire) {
        // userStore.removeUserMessage()
        //window.location.href = loginUrl
      } else {
        config.headers.Authorization = 'Bearer ' + userStore.access_token
      }
      return config
    },
    (error) => {
      Promise.reject(error)
    }
  )
  service.interceptors.response.use(
    (response) => {
      if (response.config.responseType === 'blob') {
        return response
      } else {
        if (response.data.code === 200) {
          return response.data
        }
      }
      //控件。error（res.data.message||'服务异常'）
      return Promise.reject(response.data)
    },
    (error) => {
      if (error.response.status === 401) {
        localStorage.clear()
        route.push('/login')
      }
      //控件。error（err.response.data.message||'服务异常'）
      return Promise.reject('axios', error)
    }
  )

  return service
}
