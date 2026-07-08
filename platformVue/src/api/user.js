import { businessInstance, idsInstance } from '../utils/request'

export const usePassWordLogin = (obj) => {
  return idsInstance.post('/api/Token/PassWordLogin', obj)
}

export const useAddUserAsync = (obj) => {
  return businessInstance.post('/api/User/AddUserAsync', obj)
}

export const useGetUsersByIdAsync = (userid) => {
  return businessInstance.get('/api/User/GetUsersByIdAsync', {
    params: { userid }
  })
}

export const useUpdateUserAsync = (obj) => {
  return businessInstance.post('/api/User/UpdateUserAsync', obj)
}

export const useUpload = (obj) => {
  return businessInstance.post('/api/User/Upload', obj)
}

export const useInitializePassword = (idsUserId) => {
  return businessInstance.get('/api/User/InitializePassword', {
    params: { idsUserId }
  })
}

export const useSoftDeletionUserAsync = (userId) => {
  return businessInstance.get('/api/User/SoftDeletionUserAsync', {
    params: { userId }
  })
}

export const useGetAllUserMessageAsync = (userId) => {
  return businessInstance.get('/api/User/GetAllUserMessageAsync', {
    params: { userId:userId,userCode:null }
  })
}

export const useGetSuperiorAndSubordinateAsync = (obj) => {
  return businessInstance.post('/api/User/GetSuperiorAndSubordinateAsync', obj)
}

export const useGetUserGradeAsync = (userId,grade) => {
  return businessInstance.get('/api/User/GetUserGradeAsync', {
    params: { userId:userId,grade:grade }
  })
}

export const useInsertUserGradeAsync = (obj) => {
  return businessInstance.post('/api/User/InsertUserGradeAsync', obj)
}
