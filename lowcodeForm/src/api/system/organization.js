import { idsInstance,businessInstance } from '@/utils/request'


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

//岗位树
export const getJobInfo = (obj) => {
  return businessInstance.get('/api/SysJob/GetSysJobAsync', {
    params: obj
  })
}


/* 获取角色下拉 */
export const getRoleInfo = (roleName) => {
  return idsInstance.get('/api/Role/GetRolesAsync', {
    params: {
      roleName
    }
  })
}

export const getLowCodeDict = () => {
  return businessInstance.get('/api/DataDict/GetLowCodeDict')
}