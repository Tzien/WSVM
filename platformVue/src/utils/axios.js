import axios from 'axios'
import route from '../router/index'
import { idsUrl } from './request'

export function createAxios(url, userStore) {
  const service = axios.create({
    baseURL: url,
    timeout: 10 * 6000
  })
  service.interceptors.request.use(
    (config) => {
      // if (config.method == 'post' && config.data.guid) {
      //   config.headers.BtnGuid = config.data.guid
      //   delete config.data.guid
      // }
      //发请求前做的一些处理，数据转化，配置请求头，设置token,设置loading等，根据需求去添加
      //数据转化,也可以使用qs转换
      //config.data = JSON.stringify(config.data)
      //配置请求头
      // config.headers = {
      //   'Content-Type': 'application/json' //配置请求头
      // }
      // 携带token
      const currentTimeMillis = new Date().getTime()
      const current = Math.floor(currentTimeMillis / 6000)
      const expire = userStore.expiration_timestamp
      //config.baseURL != idsUrl &&
      if (!config.url.startsWith('/api/Token/')) {
        if (!userStore.access_token && current >= expire) {
          // 处理令牌过期或无效的情况
          // userStore.removeUserMessage()
          //window.location.href = loginUrl
          // localStorage.clear()
        } else {
          config.headers.Authorization = 'Bearer ' + userStore.access_token
        }
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
      } else{
        if (response.status === 200) {
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
      return Promise.reject( error)
    }
  )

  return service
}
