import { createAxios } from './axios'

const usls = {
  IdentityServer: window.__APP_CONFIG__?.services?.IdentityServer,
  Business: window.__APP_CONFIG__?.services?.Business,
  demoapi: window.__APP_CONFIG__?.services?.XT0002Demoapi
}

const idsUrl = usls.IdentityServer
const ptUrl = usls.Business
const demoUrl = usls.demoapi

let idsInstance = createAxios(usls.IdentityServer)
let businessInstance = createAxios(usls.Business)
let demoInstance = createAxios(demoUrl)

export function initAxiosInstances(userStore) {
  idsInstance = createAxios(usls.IdentityServer, userStore)
  businessInstance = createAxios(usls.Business, userStore)
  demoInstance = createAxios(usls.demoapi, userStore)
}

export { idsUrl, ptUrl, demoUrl, idsInstance, businessInstance, demoInstance }
