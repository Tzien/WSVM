import { createAxios } from './axios'

const usls = {
  // IdentityServer: `${import.meta.env.VITE_CURRENTIP}:7006`,
  // Business:  `${import.meta.env.VITE_CURRENTIP}:5001`,
  // TaskServer:  `${import.meta.env.VITE_CURRENTIP}:5002`,

  IdentityServer: window.__APP_CONFIG__?.services?.IdentityServer,
  Business: window.__APP_CONFIG__?.services?.Business,
  TaskServer: window.__APP_CONFIG__?.services?.TaskServer,
  TaskManagement: window.__APP_CONFIG__?.services?.TaskManagement
  // IdentityServer: 'http://172.16.134.46:7006',
  // Business: 'http://172.16.134.46:5001'

  // IdentityServer: 'https://localhost:7006',
  // Business: 'http://localhost:5001'

  // IdentityServer: 'http://192.168.61.14:7006',
  // Business: 'http://192.168.61.14:5001',
  // TaskServer: 'http://192.168.61.14:5002'
}

const idsUrl = usls.IdentityServer
const ptUrl = usls.Business
const taskUrl = usls.TaskServer
const tmUrl = usls.TaskManagement

const idsInstance = createAxios(usls.IdentityServer)
const businessInstance = createAxios(usls.Business)
const taskInstance = createAxios(usls.TaskServer)
const tmInstance = createAxios(usls.TaskManagement)

export { idsUrl, ptUrl, taskUrl, idsInstance, businessInstance, taskInstance,tmInstance }
