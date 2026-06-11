import { createAxios } from './axios'

const usls = {
  IdentityServer: 'http://192.168.50.125:6014',
  Business: 'http://192.168.50.125:6015',
  lowcodeformapi: 'http://192.168.50.125:5235',
  lowcodedemo: 'https://localhost:63147'

  // //测试环境
  // IdentityServer: 'http://192.168.50.104:6014',
  // Business: 'http://192.168.50.104:6015',
  // lowcodeformapi: 'http://192.168.50.104:6004',
  // lowcodedemo: 'https://localhost:63147'
}

const idsUrl = usls.IdentityServer
// const ptUrl = usls.Business
// const demoUrl = usls.demoapi
// const lowCodeUrl = usls.lowcodeapi
// const lowCodeFormUrl = usls.lowcodeformapi

let idsInstance = createAxios(usls.IdentityServer)
let businessInstance = createAxios(usls.Business)
let lowCodeFormInstance = createAxios(usls.lowcodeformapi)
let lowCodeInstance = createAxios(usls.lowcodeformapi)
let lowCodeDemoInstance = createAxios(usls.lowcodedemo)

export function initAxiosInstances(userStore) {
  idsInstance = createAxios(usls.IdentityServer, userStore)
  businessInstance = createAxios(usls.Business, userStore)
  lowCodeInstance = createAxios(usls.lowcodeformapi, userStore)
  lowCodeFormInstance = createAxios(usls.lowcodeformapi, userStore)
  lowCodeDemoInstance = createAxios(usls.lowcodedemo, userStore)
}
export { idsUrl, idsInstance, businessInstance,lowCodeInstance, lowCodeFormInstance, lowCodeDemoInstance }
