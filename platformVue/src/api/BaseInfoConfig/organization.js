import { businessInstance } from '@/utils/request'

//添加组织机构
export const useAddSysOraganizeAsync = (obj) => {
  return businessInstance.post('/api/SysOraganize/AddSysOraganizeAsync', obj)
}

//组织机构树
export const useGetOraganizeTreeAsync = (id, name) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeTreeAsync', {
    params: { id: id, name: name }
  })
}

//组织机构树(带用户)
export const useGetOraganizeUserTreeAsync = (username) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeUserTreeAsync', {
    params: { username }
  })
}

//根据ID查询组织机构树
export const useGetOraganizeByIdAsync = (oraganizeid) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeByIdAsync', {
    params: { oraganizeid }
  })
}

//根据组织ID查询用户
export const useGetUsersAsyncAsync = (obj) => {
  return businessInstance.get('/api/User/GetUsersAsync', {
    params: obj
  })
}

//修改组织机构
export const useUpdateSysOraganizeAsync = (obj) => {
  return businessInstance.post('/api/SysOraganize/UpdateSysOraganizeAsync', obj)
}

//岗位
export const useGetPostsAsync = (name) => {
  return businessInstance.get('/api/SysJob/GetSysJobAsync', {
    params: { name }
  })
}


//添加组织机构岗位
export const useInsertOrganizeJobAsync = (obj) => {
  return businessInstance.post('/api/SysJob/InsertOrganizeJobAsync', obj)
}

//组织机构树
export const useGetOrganizeJobAsync = (orgid) => {
  return businessInstance.get('/api/SysJob/GetOrganizeJobAsync', {
    params: { orgId: orgid }
  })
}
