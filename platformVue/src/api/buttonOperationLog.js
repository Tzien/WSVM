import { businessInstance } from '@/utils/request'

//添加按钮操作日志
export const useInsertButtonOperationLogAsync = (obj) => {
  return businessInstance.post('/api/ButtonOperationLog/InsertButtonOperationLogAsync', obj)
}

//获取接口列表
export const useGetButtonOperationLogAsync = (obj) => {
  return businessInstance.post('/api/ButtonOperationLog/GetButtonOperationLogAsync', obj)
}

//获取接口列表
export const useGetInterfaceOperationLogAsync = (btnid) => {
  return businessInstance.get('/api/ButtonOperationLog/GetInterfaceOperationLogAsync', {
    params: { btnid }
  })
}