import axios from 'axios'
import route from '../router/index'
import { idsUrl } from './request'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useUserStore } from '@/store/index'
import { message } from 'ant-design-vue'

export function createAxios(url, userStore) {
  const service = axios.create({
    baseURL: url,
    timeout: 60 * 1000
  })
  service.interceptors.request.use(
    (config) => {
      if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
        userStore = useUserStore()
      }
      if (!userStore) {
        route.push('/Login')
      }
      const currentTimeMillis = new Date().getTime()
      const current = Math.floor(currentTimeMillis / 1000)
      const expire = userStore.expiration_timestamp
      if (config.baseURL != idsUrl && !config.url.startsWith('/api/Token/')) {
        if (userStore.access_token == '' && current >= expire) {
          // userStore.removeUserMessage()
          // localStorage.clear()
          // window.location.href = '/Login'
        } else {
          config.headers.Authorization = 'Bearer ' + userStore.access_token
        }
      }
      config.headers['Content-Type'] = 'application/json'
      return config
    },
    (error) => {
      Promise.reject(error)
    }
  )
  service.interceptors.response.use(
    (response) => {
      const resData = response.data
      if (response.config.responseType === 'blob') {
        return response
      }

      if (resData && typeof resData === 'object') {
        // 标准接口：包含 code 字段且为 200
        if (resData.code === 200) {
          return resData
        }

        // 低代码列表接口：顶层直接返回 { list, pagination, ... }
        // 此时没有 code 字段，但已经拿到 list 对象，也视为成功
        if (Object.prototype.hasOwnProperty.call(resData, 'list')) {
          return {
            code: 200,
            msg: '',
            data: resData
          }
        }
      }

      // 其它情况仍按失败处理
      //控件。error（res.data.message||'服务异常'）
      return Promise.reject(resData)
    },
    (error) => {
      if (error.message == 'timeout of 10000ms exceeded') {
        message.warning('参数异常或连接超时,请检查参数或者刷新页面重试')
      }
      if (error.response.status === 401) {
        // localStorage.clear()
        // window.location.href = '/Login'
      }
      if (error.response.status === 403) {
        message.warning('暂无该功能权限')
        return { code: error.response.status }
      }
      //控件。error（err.response.data.message||'服务异常'）
      return Promise.reject(error)
    }
  )
  return service
}
