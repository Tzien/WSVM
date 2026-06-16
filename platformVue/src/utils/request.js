import { createAxios } from './axios'
const usls = {
  // IdentityServer: `${import.meta.env.VITE_CURRENTIP}:7006`,
  // Business:  `${import.meta.env.VITE_CURRENTIP}:5001`,

  IdentityServer: window.__APP_CONFIG__?.services?.IdentityServer,
  Business: window.__APP_CONFIG__?.services?.Business,
  Task: window.__APP_CONFIG__?.services?.TaskServer

  // Task: 'http://192.168.57.139:5002'

  // IdentityServer: 'http://172.16.134.46:7006',
  // Business: 'http://172.16.134.46:5001',

  // IdentityServer: 'http://192.168.61.14:7006',
  
}

const idsUrl = usls.IdentityServer
const ptUrl = usls.Business
const taskUrl = usls.Task

let idsInstance
let businessInstance
let taskInstance

export function initAxiosInstances(userStore) {
  idsInstance = createAxios(usls.IdentityServer, userStore)
  businessInstance = createAxios(usls.Business, userStore)
  taskInstance = createAxios(usls.Task, userStore)
}

export { idsUrl, ptUrl, taskUrl, idsInstance, businessInstance, taskInstance }
