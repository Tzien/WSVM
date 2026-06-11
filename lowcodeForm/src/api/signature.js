import { businessInstance,lowCodeFormInstance } from '../utils/request'

//保存
export const saveApi = (obj) => {
  return lowCodeFormInstance.post('/api/Signature/Save', obj)
}


//删除
export const deleteApi = (obj) => {
  return lowCodeFormInstance.get('/api/Signature/Delete', {
    params: obj
  })
}


// 详情
export const getDetailApi = (obj) => {
  return lowCodeFormInstance.get('/api/Signature/GetById', {
    params: obj
  })
}


// 列表
export const getListApi = (obj) => {
  return lowCodeFormInstance.get('/api/Signature/GetList', {
    params: obj
  })
}

//组织机构树(带用户)
export const useGetOraganizeUserTreeAsync = (username) => {
  return businessInstance.get('/api/SysOraganize/GetOraganizeUserTreeAsync', {
    params: { username }
  })
}