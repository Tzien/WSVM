import { createAxios } from './axios'
const usls = {
  IdentityServer: 'http://172.16.134.4:7006',
  Business: 'http://172.16.134.4:5001',
  // Business: 'http://localhost:5001',
  Task: 'http://172.16.134.4:5002'
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
