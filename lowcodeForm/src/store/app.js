import { defineStore,createPinia } from 'pinia';
import { APP_DARK_MODE_KEY_, PROJ_CFG_KEY } from '@/enums/cacheEnum';
import { Persistent } from '@/utils/cache/persistent';
import { darkMode } from '@/settings/designSetting';
// import { resetRouter } from '@/router';
import { deepMerge } from '@/utils';

let timeId;

export const useAppStore = defineStore({
  id: 'app',
  state: () => ({
    darkMode: undefined,
    pageLoading: false,
    projectConfig: Persistent.getLocal(PROJ_CFG_KEY),
    beforeMiniInfo: {},
  }),
  getters: {
    getPageLoading(state) {
      return state.pageLoading;
    },
    getDarkMode(state) {
      return state.darkMode || localStorage.getItem(APP_DARK_MODE_KEY_) || darkMode;
    },
    getBeforeMiniInfo(state) {
      return state.beforeMiniInfo;
    },
    getProjectConfig(state) {
      return state.projectConfig || {};
    },
    getHeaderSetting(state) {
      return state.projectConfig?.headerSetting || {};
    },
    getMenuSetting(state) {
      return state.projectConfig?.menuSetting || {};
    },
    getTransitionSetting(state) {
      return state.projectConfig?.transitionSetting || {};
    },
    getMultiTabsSetting(state) {
      return state.projectConfig?.multiTabsSetting || {};
    },
    getSysConfigInfo(state) {
      return state.projectConfig?.sysConfigInfo || {};
    },
  },
  actions: {
    setPageLoading(loading) {
      this.pageLoading = loading;
    },
    setDarkMode(mode) {
      this.darkMode = mode;
      localStorage.setItem(APP_DARK_MODE_KEY_, mode);
    },
    setBeforeMiniInfo(state) {
      this.beforeMiniInfo = state;
    },
    setProjectConfig(config) {
      this.projectConfig = deepMerge(this.projectConfig || {}, config);
      Persistent.setLocal(PROJ_CFG_KEY, this.projectConfig);
    },
    async resetAllState() {
      // resetRouter();
      Persistent.clearAll();
    },
    async setPageLoadingAction(loading) {
      if (loading) {
        clearTimeout(timeId);
        timeId = setTimeout(() => {
          this.setPageLoading(loading);
        }, 50);
      } else {
        this.setPageLoading(loading);
        clearTimeout(timeId);
      }
    },
  },
});

// 用于 setup 外部调用
export function useAppStoreWithOut() {
  return useAppStore(createPinia());
}
