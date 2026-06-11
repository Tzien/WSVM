import { InjectionKey, Ref } from 'vue';
import { createContext, useContext } from '@/hooks/core/useContext';

export interface AppProviderContextProps {
  prefixCls: Ref<string>;
  isMobile: Ref<boolean>;
}

const key: InjectionKey<AppProviderContextProps> = Symbol();

export function useAppProviderContext() {
  return useContext<AppProviderContextProps>(key);
}


export function createAppProviderContext(context: AppProviderContextProps) {
  return createContext<AppProviderContextProps>(context, key);
}