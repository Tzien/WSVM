import { businessInstance } from '../utils/request'

//班组下拉框
export const getGetShiftClassSelect = () => {
    return businessInstance.get('/api/SysShiftClass/GetShiftClassSelectAsync')
}

//班次下拉框
export const getGetShiftSelect = () => {
    return businessInstance.get('/api/SysShift/GetShiftSelectAsync')
}

//查询排班明细
export const useGetSysShiftDetailAsync = (obj) => {
    return businessInstance.get('/api/SysShiftDetail/GetSysShiftDetailAsync', {
        params: obj
    })
}

//添加排班明细
export const useInsertSysShiftDetailAsync = (dateTime) => {
    return businessInstance.post('/api/SysShiftDetail/InsertSysShiftDetailAsync', { dateTime });
}

//查询班次
export const useGetAllSysShiftAsync = () => {
    return businessInstance.get('/api/SysShift/GetAllSysShiftAsync')
}

//查询班组信息
export const useGetShiftClassAsync = (userName) => {
    return businessInstance.get('/api/SysShiftClass/GetShiftClassAsync', {
        params: { userName }
    })
}

//根据id查询班次信息
export const useGetSysShiftByIdAsync = (id) => {
    return businessInstance.get('/api/SysShift/GetSysShiftByIdAsync', {
        params: { id } 
    })
}

//添加班次信息
export const useInsertSysShiftAsync = (obj) => {
    return businessInstance.post('/api/SysShift/InsertSysShiftAsync', obj)
}

//修改班次信息
export const useUpdateSysShiftAsync = (obj) => {
    return businessInstance.post('/api/SysShift/UpdateSysShiftAsync', obj)
}

//删除班次信息
export const useSoftDeleteSysShiftAsync = (obj) => {
    return businessInstance.post('/api/SysShift/SoftDeleteSysShiftAsync', obj)
}

//添加班组信息
export const useInsertSysShiftClassAsync = (obj,btnguid) => {
    return businessInstance.post('/api/SysShiftClass/InsertSysShiftClassAsync', obj,{
        headers: {
          'buttonoperationid': btnguid
        }
      })
}

//添加班组信息
export const useUpdateSysShiftClassAsync = (obj,btnguid) => {
    return businessInstance.post('/api/SysShiftClass/UpdateSysShiftClassAsync', obj,{
        headers: {
          'buttonoperationid': btnguid
        }
      })
}

//根据id查询班次信息
export const useGetShiftClassByIdAsync = (id,btnguid) => {
    return businessInstance.get('/api/SysShiftClass/GetShiftClassByIdAsync', {
        params: { id } ,
        headers: {
            'buttonoperationid': btnguid
          }
    })
}

//删除班组信息
export const useSoftDeleteSysShiftClassAsync = (obj,btnid) => {
    return businessInstance.post('/api/SysShiftClass/SoftDeleteSysShiftClassAsync', obj,{
        headers: {
          'buttonoperationid': btnid
        }
      })
}

//根据班组id查询用户信息
export const useGetUserByClassIdAsync = (obj) => {
    return businessInstance.post('/api/SysShiftClassUser/GetUserByClassIdAsync', obj)
}

//删除用户信息
export const useDeletSysShiftClassUserAsync = (obj) => {
    return businessInstance.post('/api/SysShiftClassUser/DeletSysShiftClassUserAsync', obj)
}

//查询查询排班信息
export const useGetSysShiftProgramAsync = (obj) => {
    return businessInstance.get('/api/SysShiftProgram/GetSysShiftProgramAsync', {
        params: obj
    })
}

//添加排版方案信息
export const useInsertSysShiftProgramAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgram/InsertSysShiftProgramAsync', obj)
}

//根据id排版方案信息
export const useGetSysShiftProgramByIdAsync = (id) => {
    return businessInstance.get('/api/SysShiftProgram/GetSysShiftProgramByIdAsync', {params : {id}})
}

//修改排版方案信息
export const useUpdateSysShiftProgramAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgram/UpdateSysShiftProgramAsync', obj)
}

//删除排版方案信息
export const useSoftSysShiftProgramAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgram/SoftSysShiftProgramAsync', obj)
}

//根据id排版方案信息
export const useGetProgramCfgByProgramIdAsync = (shiftProgramId) => {
    return businessInstance.get('/api/SysShiftProgramCfg/GetProgramCfgByProgramIdAsync', {params : { shiftProgramId }})
}

//班次下拉框
export const useGetShiftSelectAsync = () => {
    return businessInstance.get('/api/SysShift/GetShiftSelectAsync')
}

//班组下拉框
export const useGetShiftClassSelectAsync = () => {
    return businessInstance.get('/api/SysShiftClass/GetShiftClassSelectAsync')
}

//添加班次排班配置
export const useInsertSysShiftProgramCfgAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgramCfg/InsertSysShiftProgramCfgAsync', obj)
}

//根据id排版方案信息
export const useGetProgramCfgByIdAsync = (id) => {
    return businessInstance.get('/api/SysShiftProgramCfg/GetProgramCfgByIdAsync', {params : { id }})
}

//修改班次排班配置
export const useUpdateSysShiftProgramCfgAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgramCfg/UpdateSysShiftProgramCfgAsync', obj)
}

//删除排版方案信息
export const useSoftSysShiftProgramCfgAsync = (obj) => {
    return businessInstance.post('/api/SysShiftProgramCfg/SoftSysShiftProgramCfgAsync', obj)
}

//查询排版换班记录
export const useGetSysShiftDetailUserLogAsync = (obj) => {
    return businessInstance.get('/api/SysShiftDetailUserLog/GetSysShiftDetailUserLogAsync', {
        params: obj
    })
}

//添加班组用户
export const useInsertSysShiftClassUserAsync = (obj) => {
    return businessInstance.post('/api/SysShiftClassUser/InsertSysShiftClassUserAsync', obj)
}