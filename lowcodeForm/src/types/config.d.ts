
export interface GlobConfig {
  // Site title
  title: string;
  // Service interface url
  apiUrl: string;
  // Report Service interface url
  reportApiUrl: string;
  // Upload url
  uploadUrl?: string;
  //  Service interface url prefix
  urlPrefix?: string;
  // Project abbreviation
  shortName: string;
  webSocketUrl: string;
  cipherKey: string;
  aMapJsKey: string;
  aMapWebKey: string;
  aMapSecurityJsCode: string;
  filePreviewServer: string;
  dataVUrl: string;
  reportServer: string;
  report: string;
}

export type LocaleType = 'zh_CN' | 'zh_TW' | 'en_US' | 'ru' | 'ja' | 'ko';

export interface GlobEnvConfig {
  // Service interface url
  // Report Service interface url
  // Service interface url prefix
  // VITE_GLOB_API_URL_PREFIX?: string;
  // Project abbreviation
  // VITE_GLOB_APP_SHORT_NAME: string;
  // VITE_GLOB_WEBSOCKET_URL: string;
}

export interface ProjectConfig {
  // 权限相关信息的存储位置
  // permissionCacheType: CacheTypeEnum;
  // 是否显示配置按钮
  showSettingButton: boolean;
  // 是否显示主题切换按钮
  showDarkModeToggle: boolean;
  // 配置按钮的显示位置
  // settingButtonPosition: SettingButtonPositionEnum;
  // 会话超时处理
  // sessionTimeoutProcessing: SessionTimeoutProcessingEnum;
  // 网站灰色模式，为可能的哀悼日期开放
  grayMode: boolean;
  // 是否打开弱色模式
  colorWeak: boolean;
  // 主题颜色
  themeColor: string;
  // 系统背景
  sysBg: string;

  // 主界面全屏显示，不显示菜单，顶部
  fullContent: boolean;
  // 内容宽度
  // contentMode: ContentEnum;
  // 是否显示徽标
  showLogo: boolean;
  // 是否显示全局页脚
  showFooter: boolean;
  // menuType: MenuTypeEnum;
  // headerSetting: HeaderSetting;
  // menuSetting
  // menuSetting: MenuSetting;
  // 多选项卡设置
  // multiTabsSetting: MultiTabsSetting;
  // 动画配置
  // transitionSetting: TransitionSetting;
  // pageLayout 是否启用keep-alive
  openKeepAlive: boolean;
  //锁屏时间
  lockTime: number;
  // Show breadcrumbs
  showBreadCrumb: boolean;
  // Show breadcrumb icon
  showBreadCrumbIcon: boolean;
  // Use error-handler-plugin 使用错误处理程序插件
  useErrorHandle: boolean;
  // Whether to open back to top 是否打开返回顶部
  useOpenBackTop: boolean;
  // Is it possible to embed iframe pages 是否可以嵌入iframe页面
  canEmbedIFramePage: boolean;
  // Whether to delete unclosed messages and notify when switching the interface 是否删除未关闭的消息，并在切换界面时通知
  closeMessageOnSwitch: boolean;
  // Whether to cancel the http request that has been sent but not responded when switching the interface. 切换接口时是否取消已发送但未响应的http请求。
  removeAllHttpPending: boolean;
  // sysConfigInfo: SysConfigInfo;
  globalBorderRadius: number;
}


export interface LocaleSetting {
  showPicker: boolean;
  // Current language
  locale: LocaleType;
  // default language
  fallback: LocaleType;
  // available Locales
  availableLocales: LocaleType[];
}