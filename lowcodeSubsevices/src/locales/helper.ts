

import type { LocaleType } from '@/types/config';
export const loadLocalePool: LocaleType[] = [];

export function setHtmlPageLang(locale: LocaleType) {
  document.querySelector('html')?.setAttribute('lang', locale);
}

export function setLoadLocalePool(cb: (loadLocalePool: LocaleType[]) => void) {
  cb(loadLocalePool);
}

export function flattenObject(obj, prefix = '') {
  return Object.keys(obj).reduce((acc, key) => {
    const fullPath = prefix ? `${prefix}.${key}` : key; // 构建新的路径
    if (typeof obj[key] === 'object' && obj[key] !== null && !Array.isArray(obj[key])) {
      // 如果当前属性是对象，且不是数组，继续递归
      Object.assign(acc, flattenObject(obj[key], fullPath));
    } else {
      // 否则，直接设置路径和值
      acc[fullPath] = obj[key];
    }
    return acc;
  }, {});
}