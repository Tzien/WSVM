import { getI18n } from './i18n';

/**
 * 统一的安全 t 函数获取方法：
 * - 在 i18n 初始化完成后返回真正的 t
 * - 在尚未初始化或异常时，优雅降级为 (key) => key，避免抛错
 */
export function getSafeT() {
  try {
    return getI18n().global.t as (key: string, ...args: any[]) => string;
  } catch (e) {
    return ((key: string) => key) as (key: string, ...args: any[]) => string;
  }
}
