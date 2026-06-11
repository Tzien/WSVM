import { unref } from 'vue'
import { useAppProviderContext } from '@/components/Application'
import { prefixCls as defaultPrefixCls } from '@/settings/designSetting'

export function useDesign(scope: string) {
  // 兼容未使用 AppProvider 时的场景：此时 useAppProviderContext 返回的是一个空对象
  const values: any = useAppProviderContext() || {}

  // AppProvider 注入的是 Ref<string>，否则退回到全局设计前缀（"ceri"）
  const prefix = values.prefixCls ? unref(values.prefixCls) : defaultPrefixCls

  return {
    prefixCls: `${prefix}-${scope}`,
    prefixVar: prefix
  }
}
