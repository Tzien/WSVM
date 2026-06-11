// import { defHttp } from '@/utils/http/axios';
import { lowCodeInstance,lowCodeFormInstance } from '@/utils/request'


enum Api {
  Prefix = '/api/system/DictionaryData',
  TypePrefix = '/api/system/DictionaryType'
}

// 获取数据字典列表(分类+内容)
export function getDictionaryAll() {
  return lowCodeFormInstance.get(`/api/FormDb/AllDict`)
}

// 获取字典分类下拉框列表
export function getDictionaryTypeSelector(id = '0') {
  return lowCodeInstance.get(Api.TypePrefix + `/Selector/${id || '0'}`);
}

// 获取字典数据下拉框列表
export function getDictionaryDataSelector(typeId) {
  return lowCodeInstance.get(Api.Prefix + `/${typeId}/Data/Selector`);
}