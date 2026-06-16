import { businessInstance } from '../../utils/request'

//组织机构树
export const getOraganizeTreeApi = (obj) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeTreeAsync', {
    params: obj
  })
}
//通过组织id获取人员信息
export const GetAllUserBySysOraganizeIdsApi = (obj) => {
  return businessInstance.get('/api/SysOraganize/GetAllUserBySysOraganizeIds', {
    params: obj
  })
}

//发送消息
export const sendMsgApi = (obj) => {
  return businessInstance.post('/api/Msg/SendMsg', obj)
}
//获取消息记录
export const getAllApi = (obj) => {
  return businessInstance.get('/api/Msg/GetAll', {
    params: obj
  })
}

//微信组织机构树(带用户)
export const useGetWeChatOraganizeUserTree = (username) => {
  return businessInstance.get('/api/WeChatMsg/GetWeChatOraganizeUserTree', {
    params: { username }
  })
}

//钉钉组织机构树(带用户)
export const useGetDingTalkOraganizeUserTree = (username) => {
  return businessInstance.get('/api/DingTalkMsg/GetDingTalkOraganizeUserTree', {
    params: { username }
  })
}

//获取用户未读消息
export const getNoReadMsgApi = (obj) => {
  return businessInstance.get('/api/Msg/GetNoReadMsg', {
    params: obj
  })
}

//阅读消息
export const readMsgApi = (obj) => {
  return businessInstance.get('/api/Msg/ReadMsg', {
    params: obj
  })
}

//获取消息详情
export const getMsgDetailApi = (obj) => {
  return businessInstance.get('/api/Msg/GetMsgDetail', {
    params: obj
  })
}
