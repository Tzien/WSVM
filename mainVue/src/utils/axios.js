import axios from 'axios'
import route from '../router/router'
import { idsUrl } from './request'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore } from '@/store/user'
import { message } from 'ant-design-vue'

export function createAxios(url, userStore) {
  const service = axios.create({
    baseURL: url,
    timeout: 10 * 1000
  })
  service.interceptors.request.use(
    (config) => {
      //发请求前做的一些处理，数据转化，配置请求头，设置token,设置loading等，根据需求去添加
      //数据转化,也可以使用qs转换
      //config.data = JSON.stringify(config.data)
      //配置请求头
      // config.headers = {
      //   'Content-Type': 'application/json' //配置请求头
      // }
      // 携带token
      if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
        userStore = useUserStore()
      }
      if (!userStore) {
        console.log('axios')
        route.push('/Login')
      }
      const currentTimeMillis = new Date().getTime()
      const current = Math.floor(currentTimeMillis / 1000)
      const expire = userStore.expiration_timestamp
      if (config.baseURL != idsUrl && !config.url.startsWith('/api/Token/')) {
        if (userStore.access_token == '' && current >= expire) {
          // userStore.removeUserMessage()
          // window.location.href = '/Login'
          // localStorage.clear()
        } else {
          config.headers.Authorization = 'Bearer ' + userStore.access_token
        }
      }
      return config
    },
    (error) => {
      return Promise.reject(error)
    }
  )
  service.interceptors.response.use(
    (response) => {
      if (response.config.responseType === 'blob') {
        return response
      } else if (response.data.code === 200) {
        return response.data
      }
      //控件。error（res.data.message||'服务异常'）
      return Promise.reject(response.data)
    },
    (error) => {
      const status = error?.response?.status

      if (status === 401) {
        localStorage.clear()
        console.log('401')
        window.location.href = '/Login'
      }
      if (status === 403) {
        message.warning('暂无该功能权限')
        return { code: status }
      }
      //控件。error（err.response.data.message||'服务异常'）
      return Promise.reject(error.response?.data || error)
    }
  )
  return service
}
