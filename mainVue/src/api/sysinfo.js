import { businessInstance } from '../utils/request'

export const useGetAllSysInfo = (name) => {
  return businessInstance.get('/api/SysInfo/GetAllSysInfo', {
    params: { name }
  })
}

//获取平台信息配置详情
export const getPtInfoConfigDetailApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetPtInfoConfigDetail', {
    params: obj
  })
}

//获取平台名称(无权限)
export const getPlatformNameApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetPtInfoName')
}

//添加/修改 平台信息配置
export const savePtInfoConfigApi = (obj) => {
  return businessInstance.post('/api/SysInfo/AddOrUpdatePtInfoConfig', obj)
}

//新增系统信息
export const addSysInfoApi = (obj) => {
  return businessInstance.post('/api/SysInfo/AddSysInfoAsync', obj)
}
//修改系统信息
export const updateSysInfoApi = (obj) => {
  return businessInstance.post('/api/SysInfo/UpdateSysInfoAsync', obj)
}
//删除系统信息
export const deletionSysInfoApi = (obj) => {
  return businessInstance.get('/api/SysInfo/SoftDeletionSysInfoAsync', {
    params: obj
  })
}
//获取系统信息列表
export const getSysInfoAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetSysInfoAsync', {
    params: obj
  })
}
//获取系统信息详情
export const getSysInfoByIdAsyncApi = (obj) => {
  return businessInstance.get('/api/SysInfo/GetSysInfoByIdAsync', {
    params: obj
  })
}

//上传图片
export const uploadApi = (obj) => {
  return businessInstance.post('/api/SysInfo/upload', obj)
}

//获取菜单访问日志详情
export const getSysMenuRecord = (obj) => {
  return businessInstance.get('/api/SysMenuRecord/GetRecentlyVisitedSysMenuAsync', {
    params: obj
  })
}

//获取消息记录
export const getAllApi = (obj) => {
  return businessInstance.get('/api/Msg/GetAll', {
    params: obj
  })
}

//获取用户登录日志详情
export const getuserLoginLogs = (obj) => {
  return businessInstance.get('/api/SysLoginUserLog/GetSysLoginUserLogAsync', {
    params: obj
  })
}

//获取接口列表
export const useGetButtonOperationLogAsync = (obj) => {
  return businessInstance.post('/api/ButtonOperationLog/GetButtonOperationLogAsync', obj)
}

//获取版本信息
export const getVersionInfo = (obj) => {
  return businessInstance.get('/api/SysVersionLog/GetSysVersionLogAsync', {
    params: obj
  })
}

//获取收藏信息
export const getFavInfoList = (obj) => {
  return businessInstance.post('/api/SysPermission/GetFavoriteMenuAsync', obj)
}

//添加收藏菜单/固定系统
export const AddFavInfo = (obj) => {
  return businessInstance.post('/api/SysPermission/InsertFavoriteMenuAsync', obj)
}

//删除收藏菜单/固定系统
export const DelFavInfo = (obj) => {
  return businessInstance.get('/api/SysPermission/DeleteFavoriteMenuAsync', { params: obj })
}

//获取固定系统信息
export const GetFixedInfoList = (obj) => {
  return businessInstance.get('/api/SysPermission/GetAllFixedSysinfoAsync', { params: obj })
}

//获取固定系统信息
export const GetWorkBenchFunctionList = (obj) => {
  return businessInstance.get('/api/FunctionList/GetWorkBenchFunctionList')
}

