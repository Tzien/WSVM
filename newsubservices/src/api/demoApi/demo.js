import { businessInstance ,demoInstance } from '@/utils/request'



//获取demo用户信息详情
export const getUserInfo = (obj) => {
  return demoInstance.get('/api/v1/User/GetUser', {
    params: obj
  })
}

export const useAddUserAsync = (obj) => {
  return demoInstance.post('/api/v1/User/AddUser', obj)
}

//删除
export const deleteApi = (obj) => {
  return demoInstance.get('/api/v1/User/DeleteUser', {
    params: obj
  })
}
//修改
export const modifySysJob = (sysjob) => {
  return demoInstance.post('/api/v1/User/UpdateUser', sysjob)
}