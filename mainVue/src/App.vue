<template>
  <a-layout class="MainLayout" v-if="isMainLayout">
    <!-- <Mascot />  京小诚AI体 -->
    <a-layout-header class="AppHeader" :class="drawerStore.theme == 'dark' ? 'newDarkColor' : 'newBlueColor'">
      <div class="AppHeaderContent">
        <div class="AppHeaderImg">
          <a-image :src="avatarImageURL" :preview="false" />
        </div>
        <div>
          <div style="display: flex">
            <div :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
              style="width: 170px; text-align: center; height: 64px; line-height: 64px; font-weight: 700; font-size: 18px; font-family: Source Han Sans CN">
              {{ drawerStore.platformName }}
            </div>
            <div class="LayoutDropdown">
              <a-dropdown :trigger="['click']" placement="bottom" :overlayStyle="{
                width: '100vw',
                marginTop: '10px'
              }" overlayClassName="workbench-dropdown">
                <div class="workbench-trigger">
                  <img src="/src/assets/images/Vector.png" alt="Loading..." />
                </div>
                <template #overlay>
                  <div class="workbench-content" @click="handleMenuClickWorkbench" @click.native.stop>
                    <div class="dropdown-arrow"></div>
                    <!-- 这里放置您的工作台内容 -->
                    <div class="workbench-section">
                      <div style="display: flex; height: 40px; line-height: 40px; align-items: center">
                        <div style="display: flex; align-items: center; margin-right: 10px">
                          <a-input-search v-model:value="workbenchSerchKey" placeholder="请输入要查询的菜单名称" enter-button
                            style="width: 540px" @search="showAllSysInfo(true, true)" />
                        </div>
                        <span class="userSearchRecord" v-for="item in userStore.searchKey" :key="item"
                          @click="setSearchKey(item)">{{ item }}</span>
                      </div>
                      <div class="workbench-grid">
                        <div>
                          <div class="workbench-item" style="margin-top: 20px; width: 311px; height: 330px">
                            <div style="display: flex; align-items: center">
                              <div style="height: 16px; width: 4px; background-color: #2461a6"></div>
                              <div
                                style="font-weight: 500; font-size: 16px; height: 24px; line-height: 24px; color: #005ec9; margin-left: 5px">
                                {{ $t('common.RecentlyUsed') }}
                              </div>
                            </div>
                            <div style="font-size: 14px; height: calc(100% - 30px); overflow-y: auto; margin-top: 10px">
                              <div class="BoxContent1" @click="JumpToMenu(item, item.sysCode)"
                                v-for="item in menuRecords" :key="item.menuCode">
                                <component style="margin: 0px 5px" :is="mapIcon(item.menuIcon)" />
                                {{ $t(`message.route.XT${item.menuCode}`) }}
                              </div>
                            </div>
                          </div>
                          <div class="workbench-item" style="margin-top: 20px; width: 311px; height: 330px">
                            <div style="display: flex; align-items: center">
                              <div style="height: 16px; width: 4px; background-color: #2461a6"></div>
                              <div
                                style="font-weight: 500; font-size: 16px; height: 24px; line-height: 24px; color: #005ec9; margin-left: 5px">
                                {{ $t('common.MyCollection') }}
                              </div>
                            </div>
                            <div style="font-size: 14px; height: calc(100% - 30px); overflow-y: auto; margin-top: 10px">
                              <div class="BoxContent2" v-for="favItem in favList" :key="favItem.menuId"
                                @click="JumpToMenu(favItem, favItem.sysCode)">
                                <span>
                                  <component style="margin: 0px 5px" :is="mapIcon(favItem.menuIcon)" />
                                  {{ $t(`message.route.XT${favItem.menuCode}`) }}
                                </span>
                                <span style="margin: 0px 10px" @click.native.stop="UnCollect(favItem.menuId)">
                                  <img src="/src/assets/images/icons/collectSvg.svg" alt="Loading..." />
                                </span>
                              </div>
                            </div>
                          </div>
                        </div>

                        <div class="workbench-item"
                          style="width: 250px; margin-top: 20px; margin-left: 20px; padding: 0px; border-radius: 4px 0px 0px 4px">
                          <div style="display: flex; align-items: center; padding: 16px">
                            <div style="height: 16px; width: 4px; background-color: #2461a6"></div>
                            <div
                              style="font-weight: 500; font-size: 16px; height: 24px; line-height: 24px; color: #005ec9; margin-left: 5px">
                              {{ $t('common.MenuList') }}
                            </div>
                          </div>
                          <div style="font-size: 14px; height: calc(100% - 30px - 16px - 16px); overflow-y: auto">
                            <div class="BoxContent3" v-for="menu in menuItems" :key="menu.sysCode"
                              :class="{ active: hoveredId === menu.sysCode }" @mouseenter="onHover(menu)">
                              <div
                                style="display: flex; min-height: 25px; line-height: 25px; justify-content: space-between">
                                <div>
                                  <component style="margin: 0px 5px" :is="menu.systemIcon" />
                                  <span> {{ $t(`message.route.XT${menu.sysCode}`) }}</span>
                                </div>
                                <div style="display: flex; align-items: center">
                                  <img title="固定系统" v-show="!fixedList.some((a) => a.sysCode == menu.sysCode)"
                                    src="/src/assets/images/icons/unFixed.svg" alt="Loading..."
                                    @click.native.stop="AddCollect(menu.systemId, true)" />
                                  <img title="取消固定系统" v-show="fixedList.some((a) => a.sysCode == menu.sysCode)"
                                    src="/src/assets/images/icons/fixedSvg.svg" alt="Loading..."
                                    @click.native.stop="UnCollect(menu.systemId, true)" />
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                        <div class="workbench-item"
                          style="width: 1000px; margin-top: 20px; border-radius: 0px; display: flex; flex-direction: row; flex-wrap: wrap; overflow-y: auto">
                          <template v-if="persistedSubMenu">
                            <div v-for="item in persistedSubMenu" :key="item.title" style="margin: 5px">
                              <div style="position: relative">
                                <div v-if="item.items && item.items.length > 0"
                                  style="max-height: 300px; width: 210px; text-align: left; overflow-y: scroll">
                                  <div style="
                                      display: flex;
                                      align-items: center;
                                      width: 250px;
                                      padding: 16px;
                                      position: absolute;
                                      top: 0px;
                                      left: 0px;
                                      background-color: #fff;
                                    ">
                                    <div style="height: 16px; width: 4px; background-color: #2461a6"></div>
                                    <div style="
                                        display: flex;
                                        align-items: center;
                                        min-width: 0;
                                        font-weight: 500;
                                        font-size: 16px;
                                        height: 24px;
                                        line-height: 24px;
                                        color: #005ec9;
                                        margin-left: 5px;
                                      ">
                                      <component style="margin: 0px 5px" :is="item.menuIcon" />
                                      <a-tooltip :title="$t(`message.route.XT${item.menuCode}`)">
                                        <span
                                          style="display: block; min-width: 0; flex: 1 1 auto; white-space: nowrap; overflow: hidden; text-overflow: ellipsis">
                                          {{ $t(`message.route.XT${item.menuCode}`) }}
                                        </span>
                                      </a-tooltip>
                                    </div>
                                  </div>
                                  <div style="margin-top: 58px">
                                    <div class="BoxContent3" v-for="sub in item.items" :key="sub">
                                      <div
                                        style="display: flex; min-height: 25px; line-height: 25px; justify-content: space-between"
                                        @click="JumpToMenu(sub, hoveredId)">
                                        <div
                                          style="display: flex; align-items: center; gap: 4px; min-width: 0; flex: 1 1 auto">
                                          <component style="margin: 0px 5px" :is="sub.menuIcon" />
                                          <a-tooltip :title="$t(`message.route.XT${sub.menuCode}`)">
                                            <span style="
                                                display: block;
                                                min-width: 0;
                                                flex: 1 1 auto;
                                                white-space: nowrap;
                                                overflow: hidden;
                                                text-overflow: ellipsis;
                                              ">
                                              {{ $t(`message.route.XT${sub.menuCode}`) }}
                                            </span>
                                          </a-tooltip>
                                        </div>
                                        <div style="display: flex; align-items: center">
                                          <img v-show="favList.some((a) => a.menuRoute == sub.menuRoute)"
                                            src="/src/assets/images/icons/collectSvg.svg" alt="Loading..."
                                            @click.native.stop="UnCollect(sub.menuId, false)" />
                                          <img v-show="!favList.some((a) => a.menuRoute == sub.menuRoute)"
                                            src="/src/assets/images/icons/unCollect.svg" alt="Loading..."
                                            @click.native.stop="AddCollect(sub.menuId, false)" />
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                                <div v-else>
                                  <div class="BoxContent3">
                                    <div
                                      style="display: flex; min-height: 25px; min-width: 150px; line-height: 25px; justify-content: space-between">
                                      <div
                                        style="display: flex; align-items: center; gap: 4px; min-width: 0; flex: 1 1 auto">
                                        <component style="margin: 0px 5px" :is="item.menuIcon" />
                                        <a-tooltip :title="item.menuTitle">
                                          <span
                                            style="display: block; min-width: 0; flex: 1 1 auto; white-space: nowrap; overflow: hidden; text-overflow: ellipsis">
                                            {{ item.menuTitle }}
                                          </span>
                                        </a-tooltip>
                                      </div>
                                      <div style="display: flex; align-items: center; margin-left: 10px">
                                        <img v-show="favList.some((a) => a.menuRoute == item.menuRoute)"
                                          src="/src/assets/images/icons/collectSvg.svg" alt="Loading..."
                                          @click.native.stop="UnCollect(item.menuId, false)" />
                                        <img v-show="!favList.some((a) => a.menuRoute == item.menuRoute)"
                                          src="/src/assets/images/icons/unCollect.svg" alt="Loading..."
                                          @click.native.stop="AddCollect(item.menuId, false)" />
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </template>
                        </div>

                        <div class="workbench-item"
                          style="margin-top: 20px; width: 400px; text-align: center; border-radius: 0px 4px 4px 0px">
                          <div class="BoxRightAll" v-for="item in rightBoxList" :key="item.id"
                            :style="{ backgroundImage: `url(${getWorkbenchBg(item.picName)})` }">
                            <div class="BoxRightTitle">{{ item.title }}</div>
                            <div class="BoxRightDesc">{{ item.description }}</div>
                            <div style="margin-top: 10px;">
                              <a-button type="primary" class="BoxRightBtn" shape="round" @click="checkURL(item.url)">
                                <template #icon>
                                  点击进入
                                </template>
                              </a-button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </template>
              </a-dropdown>
            </div>
            <a-menu class="LayoutMenu" :style="{ width: `calc(100vw - 220px - 210px - 38px - 280px - 50px)` }"
              v-model:selectedKeys="drawerStore.selectFirstMenuPath" mode="horizontal" @click="clickItem"
              style="background-color: rgb(0, 0, 0, 0)" :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
              :theme="drawerStore.theme == 'light' ? 'light' : 'dark'">
              <template v-if="fixedList.length > 1">
                <a-menu-item v-for="item in fixedList" :key="item.key">
                  <component style="margin-right: 5px" :is="mapIcon(item.subSysIcon)" />
                  <i v-if="item.sysCode == 'ComprehensiveHomePage'" style="margin-left: 2px; font-style: normal">{{
                    $t('common.ComprehensiveHomePage') }}</i>
                  <i v-else style="margin-left: 2px; font-style: normal">{{ $t(`message.route.XT${item.sysCode}`) }}</i>
                </a-menu-item>
              </template>
              <template v-else>
                <a-menu-item v-for="item in allSysInfo" :key="item.key">
                  <component style="margin-right: 5px" :is="mapIcon(item.subSysIcon)" />
                  <i v-if="item.sysCode == 'ComprehensiveHomePage'" style="margin-left: 2px; font-style: normal">{{
                    $t('common.ComprehensiveHomePage') }}</i>
                  <i v-else style="margin-left: 2px; font-style: normal">{{ $t(`message.route.XT${item.sysCode}`) }}</i>
                </a-menu-item>
              </template>
            </a-menu>
            <div class="IconF">
              <div class="IconS">
                <a-tooltip :title="$t('home.msgcenter')">
                  <div @click="showMsg">
                    <a-badge :count="signalRStore.msgCount" size="small">
                      <BellOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                        style="font-size: 16px" />
                    </a-badge>
                  </div>
                </a-tooltip>
              </div>
              <div class="IconS" @click="test">
                <a-tooltip :title="$t('home.flowcenter')">
                  <div>
                    <BranchesOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="font-size: 16px" />
                  </div>
                </a-tooltip>
              </div>

              <!-- <div class="IconS" @click="lowcode">
                <a-tooltip title="低代码中心">
                  <div><CodeOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''" style="font-size: 16px" /></div>
                </a-tooltip>
              </div> -->
              <!-- <div class="IconS" @click="GoToRuleEngineDesign()">
                <a-tooltip title="规则引擎设计器">
                  <div>
                    <img src="/src/assets/RuleEngine.svg" :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="width: 18px; height: 20px; margin-bottom: 5px" />
                  </div>
                </a-tooltip>
              </div>
              <div class="IconS" @click="GoToAIAddress()">
                <a-tooltip title="AI代码中心">
                  <div>
                    <img src="/src/assets/ai-icon.svg" :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="width: 18px; height: 20px; margin-bottom: 5px" />
                  </div>
                </a-tooltip>
              </div>
              <div class="IconS" @click="GoToConfigurationDesigner()">
                <a-tooltip title="组态设计器">
                  <div>
                    <img src="/src/assets/configuration.svg" :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="width: 18px; height: 20px; margin-bottom: 5px" />
                  </div>
                </a-tooltip>
              </div>
              <div class="IconS" @click="GoToDSJ()">
                <a-tooltip title="大数据平台">
                  <div>
                    <img src="/src/assets/DSJ.svg" :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="width: 18px; height: 20px; margin-bottom: 5px" />
                  </div>
                </a-tooltip>
              </div> -->
              <div class="IconS" @click="showDrawer">
                <a-tooltip title="设置">
                  <div>
                    <SettingOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="font-size: 16px" />
                  </div>
                </a-tooltip>
              </div>
              <div class="IconS" @click="logoff">
                <a-tooltip title="退出">
                  <div>
                    <PoweroffOutlined :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''"
                      style="font-size: 16px" />
                  </div>
                </a-tooltip>
              </div>
              <div>
                <a-dropdown placement="bottom" :overlayStyle="{ width: '150px', textAlign: 'center' }">
                  <div style="" @mouseover="hovered = true" @mouseleave="hovered = false" class="loginname"
                    :class="drawerStore.theme == 'dark' ? 'darkColorFont' : ''">
                    {{ userStore.realname ? userStore.realname : userStore.loginname }}
                    <CaretUpOutlined v-if="hovered" style="font-size: 16px" />
                    <CaretDownOutlined v-else style="font-size: 16px" />
                  </div>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item v-if="isBreak">
                        <a target="_blank" @click="OpenMarkDown">
                          <ReadOutlined /> &nbsp;{{ $t('common.Guidebook') }}
                        </a>
                      </a-menu-item>
                      <a-menu-item>
                        <a target="_blank">
                          <StarOutlined /> &nbsp;{{ $t('common.MyCollection') }}
                        </a>
                      </a-menu-item>
                      <a-menu-item>
                        <a target="_blank" @click="uptpas">
                          <UnlockOutlined /> &nbsp;{{ $t('drawer.ChangePassword') }}
                        </a>
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </div>
            </div>
          </div>
          <div class="TabsCss" ref="tabsContainer">
            <a-tabs :activeKey="currentTabKey" :tabBarGutter="0" type="editable-card" @tabClick="tabclick"
              @edit="onEdit" hide-add>
              <a-tab-pane key="/" :tab="$t('common.ComprehensiveHomePage')" :closable="false"></a-tab-pane>
              <template v-if="i18nReady">
                <a-tab-pane v-for="pane in navigationStore.tabs" :key="pane.key" :tab="getTabDisplayTitle(pane)"
                  :closable="true"> </a-tab-pane>
              </template>
            </a-tabs>
          </div>
        </div>
      </div>
    </a-layout-header>
    <a-config-provider :locale="currentAntdLocale">
      <div v-if="drawerStore.selectFirstMenuPath == '/'" style="height: calc(100vh - 96px)">
        <router-view />
      </div>
    </a-config-provider>
    <!-- </div> -->
  </a-layout>
  <a-layout v-else>
    <!-- 给Login用的 -->
    <router-view></router-view>
  </a-layout>

  <div v-for="item in userStore.allSysCode" :key="item" :id="`${item}-container`"></div>
  <div id="XTMarkDown-container"></div>

  <!-- 退出弹窗 -->
  <div>
    <a-modal style="height: 500px" v-model:open="exitModal" title="提示" @ok="handleOk">
      <p>确认退出吗？</p>
    </a-modal>
  </div>
  <div>
    <PublicDrawers></PublicDrawers>
  </div>

  <!-- 消息详情弹框 -->
  <MsgDetail v-if="signalRStore.isShow"></MsgDetail>

  <div>
    <a-modal v-model:open="open" title="修改密码" :footer="null">
      <a-divider style="height: 1px; background-color: rgb(240, 240, 240)" />
      <div>
        <div class="mimadiv">
          <label class="passlabel">原密码:</label>
          <a-input-password v-model:value="yuan" style="width: 200px; vertical-align: top"
            placeholder="请输入原密码"></a-input-password>
        </div>
        <div class="mimadiv">
          <label class="passlabel">新密码: </label>
          <a-input-password v-model:value="xin" style="width: 200px" placeholder="请输入新密码"> </a-input-password>
        </div>
        <div class="mimadiv">
          <label class="passlabel">确认密码: </label>
          <a-input-password v-model:value="que" style="width: 200px" placeholder="请输入确认密码"> </a-input-password>
        </div>
        <a-divider style="height: 1px; background-color: rgb(240, 240, 240)" />
        <div style="display: flex; justify-content: center">
          <a-button type="primary" @click="handleOkpass" style="width: 100px; margin-right: 5px">确认</a-button>
          <a-button @click="handlecancel" style="width: 100px; margin-left: 5px">重置</a-button>
        </div>
      </div>
    </a-modal>

    <!-- 右键菜单 -->
    <a-dropdown v-model:open="menuVisible" ref="menuRef" :trigger="[]"
      :overlayStyle="{ position: 'fixed', top: `${menuY}px`, left: `${menuX}px` }">
      <div></div>
      <template #overlay>
        <a-menu style="min-width: 100px; width: auto; max-width: 150px" @click="onContextMenuClick">
          <a-menu-item key="closeOthers">关闭其它</a-menu-item>
          <a-menu-item key="closeAll">关闭全部</a-menu-item>
        </a-menu>
      </template>
    </a-dropdown>
  </div>

  <!-- 消息列表 -->
  <a-drawer :width="500" :title="t('home.Notification')" placement="right" :open="msgOpen" @close="msgOpen = false"
    :closable="false">
    <a-tabs v-model:activeKey="activeKey" @change="tabChange">
      <a-tab-pane v-for="tab in tabs" :key="tab.key">
        <template #tab>
          <span>
            <component :is="tab.icon" style="margin-right: 0px" />
            {{ tab.title }}
          </span>
        </template>
        <div style="height: 730px; overflow-y: auto; border-bottom: 1px solid #f0f0f0">
          <a-list :data-source="tab.data" item-layout="horizontal">
            <template #renderItem="{ item }">
              <a-list-item style="display: flex; justify-content: space-between; align-items: center; padding-left: 0">
                <span style="display: flex; align-items: center; gap: 6px">
                  <SoundOutlined style="color: #faad14; font-size: 14px" />
                  <span v-if="activeKey == '1'">
                    {{ item.sendUser }} 发布了
                    <span style="color: #1677ff; cursor: pointer" @click="msgDetail(item.msgRecordId)"> 《{{ item.title
                    }}》 </span>
                  </span>
                  <span v-if="activeKey == '2'">
                    {{ item.sendUser }} 发送了
                    <span style="color: #1677ff; cursor: pointer" @click="msgDetail(item.msgRecordId)"> 《{{ item.title
                    }}》 </span>
                  </span>
                  <span v-if="activeKey == '3'">
                    {{ item.sendUser }} 发送了
                    <span style="color: #1677ff; cursor: pointer" @click="msgDetail(item.msgRecordId)"> 《{{ item.title
                    }}》 </span>
                  </span>
                  <span v-if="activeKey == '4'">
                    <span style="color: #1677ff; cursor: pointer" @click="msgDetail(item.msgRecordId)">【{{ item.content
                    }}】</span>
                  </span>
                </span>
                <span> {{ item.sendTime }}<span style="color: red">●</span> </span>
              </a-list-item>
            </template>
          </a-list>
        </div>
      </a-tab-pane>
    </a-tabs>
  </a-drawer>
</template>
<script setup>
import * as signalR from '@microsoft/signalr'
import { useGetPermissionAsync } from './api/permission'
import { ref, watch, h, onMounted, nextTick, onBeforeUnmount, getCurrentInstance, computed } from 'vue'
import { useRouter } from 'vue-router'
// import {  useRoute } from 'vue-router'
import zhCN from 'ant-design-vue/es/locale/zh_CN'
import 'dayjs/locale/zh-cn'
import { useInsertSysMenuRecord, useUpdateSysLoginUserLogAsync, useInsertSysLoginUserLogAsync } from '@/api/userLoginLogs'
import {
  ReadOutlined,
  BellOutlined,
  BranchesOutlined,
  SettingOutlined,
  PoweroffOutlined,
  CaretUpOutlined,
  CaretDownOutlined,
  StarOutlined,
  MessageOutlined,
  UnlockOutlined,
  NotificationOutlined,
  WechatOutlined,
  DingdingOutlined
} from '@ant-design/icons-vue'
import * as Icons from '@ant-design/icons-vue'
import { useUserStore, useDrawerStore, useNavigationStore, useSignalRStore, useRouteStore } from './store/index.js'

import { notification, message } from 'ant-design-vue'
import { getNoReadMsgApi, getDingSelectState, getWeChatSelectState } from './api/Msg/msg'

import { getFavInfoList, AddFavInfo, DelFavInfo, GetFixedInfoList, GetWorkBenchFunctionList, getSysMenuRecord, getPlatformNameApi, getAllApi } from '@/api/sysinfo.js'

import { usePassWordLogin, useGetSFToken, useUpdatePassword } from '@/api/user'
import { registerMicroApps, start } from 'qiankun'
import { buildPermissionQuery } from './utils/commonTools'
import { loadLocaleMessages } from './lang/i18n'
import DDMSJBg from './assets/workbenchRightBg/DDMSJ.png'
import DSJBg from './assets/workbenchRightBg/DSJ.png'
import GZYQBg from './assets/workbenchRightBg/GZYQ.png'
import LCZXBg from './assets/workbenchRightBg/LCZX.png'
import ZTSJBg from './assets/workbenchRightBg/ZTSJ.png'
const userStore = useUserStore()
const routeStore = useRouteStore()
const drawerStore = useDrawerStore()
const navigationStore = useNavigationStore()
const signalRStore = useSignalRStore()
const router = useRouter()
// const currentRoute = useRoute()
const i18nReady = ref(false)
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
import { debounce } from 'lodash-es'
import dayjs from 'dayjs'
import 'dayjs/locale/zh-cn'
import 'dayjs/locale/en'
import 'dayjs/locale/id'
import 'dayjs/locale/ru'
const i18n = useI18n({ useScope: 'global' })
const currentAntdLocale = ref(null)
const antdLocaleMap = {
  zh: () => import('ant-design-vue/es/locale/zh_CN'),
  en: () => import('ant-design-vue/es/locale/en_US'),
  id: () => import('ant-design-vue/es/locale/id_ID'),
  ru: () => import('ant-design-vue/es/locale/ru_RU')
}
const dayjsLocaleMap = {
  zh: 'zh-cn',
  en: 'en',
  id: 'id',
  ru: 'ru'
}
const debouncedLoadAntdLocale = debounce(async (language) => {
  if (!language) return
  try {
    const key = String(language).toLowerCase().split(/[-_]/)[0]
    const loader = antdLocaleMap[key]
    if (loader) {
      const module = await loader()
      currentAntdLocale.value = module.default || module
      const dayKey = dayjsLocaleMap[key]
      if (dayKey) {
        dayjs.locale(dayKey)
      }
    } else {
      const fallback = await import('ant-design-vue/es/locale/zh_CN')
      currentAntdLocale.value = fallback.default || fallback
      dayjs.locale('zh-cn')
    }
  } catch (error) {
    const fallback = await import('ant-design-vue/es/locale/zh_CN')
    currentAntdLocale.value = fallback.default || fallback
    dayjs.locale('zh-cn')
  }
}, 100)
watch(
  () => navigationStore.language,
  (lang) => {
    debouncedLoadAntdLocale(lang)
  },
  { immediate: true }
)

const buildRoleKey = (roles) => {
  const roleList = Array.isArray(roles) ? roles : Array.isArray(roles?.value) ? roles.value : []
  return [...roleList]
    .map((role) => `${role ?? ''}`)
    .sort()
    .join('|')
}

const getRoutePermissionSnapshot = () => {
  const language = navigationStore.language || ''
  const systems = routeStore.getCachedPermissionSystems(userStore.userRoles, language)

  return {
    roleKey: buildRoleKey(userStore.userRoles),
    language,
    systems: Array.isArray(systems) ? systems : []
  }
}

// 顶部 tabs 的选中状态与当前路由 path 保持一致
const currentTabKey = computed(() => router.currentRoute.value.path)

watch(navigationStore.tabs, bindContextMenus, { deep: true })

const tabsContainer = ref(null)
const menuRef = ref(null)
const menuVisible = ref(false)
const menuX = ref(0)
const menuY = ref(0)
const contextKey = ref(null)
function bindContextMenus() {
  nextTick(() => {
    const container = tabsContainer.value
    if (!container) return
    const tabs = container.querySelectorAll('.ant-tabs-tab')
    tabs.forEach((tabEl, index) => {
      tabEl.oncontextmenu = (e) => {
        e.preventDefault()
        contextKey.value = navigationStore.tabs[index - 1].key
        menuX.value = e.clientX
        menuY.value = e.clientY
        menuVisible.value = true
      }
    })
  })
}

const rightBoxList = ref([])

function getWorkBenchFunctionList() {
  GetWorkBenchFunctionList().then((res) => {
    if (res.code == 200 && res.success) {
      rightBoxList.value = res.data
      return
    }
    message.error('获取链接门户列表异常，请刷新重试')
  })
}

const workbenchBgImages = {
  DDMSJ: DDMSJBg,
  DSJ: DSJBg,
  GZYQ: GZYQBg,
  LCZX: LCZXBg,
  ZTSJ: ZTSJBg
}

function getWorkbenchBg(name) {
  const imageName = String(name ?? '')
    .trim()
    .replace(/\.png$/i, '')
    .toUpperCase()
  return workbenchBgImages[imageName] || LCZXBg
}


function base64UrlToUtf8Json_legacy(b64url) {
  let b64 = b64url.replace(/-/g, '+').replace(/_/g, '/')
  const pad = b64.length % 4
  if (pad) b64 += '='.repeat(4 - pad)

  // 关键：先 escape 再 decodeURIComponent
  const jsonStr = decodeURIComponent(escape(window.atob(b64)))
  return JSON.parse(jsonStr)
}

const handleSubAppAddTab = (event) => {
  const detail = event && event.detail
  const tab = detail && detail.tab
  if (!tab || !tab.key) {
    return
  }
  addNavigationTabIfMissing(tab)
}

const normalizePathLike = (value) => {
  const text = `${value ?? ''}`.trim()
  if (!text) return '/'
  return text.startsWith('/') ? text : `/${text}`
}

const normalizeTabInnerPath = (tab) => {
  const rawPath = normalizePathLike(tab?.path)
  const sysCode = `${tab?.sysCode ?? ''}`.trim()
  const rootPath = sysCode ? `/${sysCode}` : ''

  if (rootPath && (rawPath === rootPath || rawPath.startsWith(`${rootPath}/`))) {
    const innerPath = rawPath.slice(rootPath.length)
    return innerPath ? normalizePathLike(innerPath) : '/'
  }

  return rawPath
}

const buildTabFullPath = (tab) => {
  const innerPath = normalizeTabInnerPath(tab)
  const sysCode = `${tab?.sysCode ?? ''}`.trim()
  if (!sysCode) {
    return innerPath
  }
  return `${`/${sysCode}`}${innerPath}`
}

const normalizeTabEntity = (tab) => {
  if (!tab || typeof tab !== 'object') {
    return tab
  }
  const normalized = { ...tab }
  normalized.path = normalizeTabInnerPath(normalized)
  const fullPath = buildTabFullPath(normalized)
  normalized.key = normalizePathLike(normalized.key || fullPath)
  return normalized
}

const getTabDisplayTitle = (tab) => {
  if (!tab || typeof tab !== 'object') return ''
  const fallbackTitle = `${tab.title ?? ''}`
  const i18nKey = `${tab.i18nKey ?? ''}`.trim()
  if (!i18nKey) {
    return fallbackTitle
  }
  const translated = i18n.t(i18nKey)
  return translated && translated !== i18nKey ? translated : fallbackTitle
}

const refreshTabTitles = () => {
  if (!Array.isArray(navigationStore.tabs) || navigationStore.tabs.length === 0) {
    return
  }

  navigationStore.tabs = navigationStore.tabs.map((tab) => {
    const normalized = normalizeTabEntity(tab)
    return {
      ...normalized,
      title: getTabDisplayTitle(normalized)
    }
  })
}

const addNavigationTabIfMissing = (tab) => {
  if (!tab || typeof tab !== 'object' || !Array.isArray(navigationStore.tabs)) {
    return
  }

  const normalizedTab = normalizeTabEntity(tab)
  normalizedTab.title = getTabDisplayTitle(normalizedTab)
  const exists = navigationStore.tabs.some((item) => item && item.key === normalizedTab.key)
  if (!exists && typeof navigationStore.addTabs === 'function') {
    navigationStore.addTabs(normalizedTab)
  }
}

const normalizePersistedTabs = () => {
  if (!Array.isArray(navigationStore.tabs) || navigationStore.tabs.length === 0) {
    return
  }

  const map = new Map()
  navigationStore.tabs.forEach((tab) => {
    const normalized = normalizeTabEntity(tab)
    if (!normalized?.key) return
    normalized.title = getTabDisplayTitle(normalized)
    map.set(normalized.key, normalized)
  })

  navigationStore.tabs = Array.from(map.values())
}

// 在script setup部分添加
onMounted(async () => {
  normalizePersistedTabs()

  // 先尽早注册子应用 tab 事件监听，避免异步操作耗时期间丢失事件
  window.addEventListener('sub-app-add-tab', handleSubAppAddTab)

  // 确保国际化资源已加载完成
  await waitForI18nReady()
  i18nReady.value = true
  syncGlobalState()
  if (userStore.access_token == '') {
    return
  }
  const currentLanguage = navigationStore?.language || i18n.locale.value || 'zh'
  const routePermissionSystems = await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
  await loadLocaleMessages(i18n, currentLanguage, {
    includeCommonMessages: true,
    includeRouteMessages: true,
    routeSourceData: routePermissionSystems
  })
  if (typeof window !== 'undefined') {
    window.dispatchEvent(new CustomEvent('main-app-i18n-updated', { detail: { locale: currentLanguage } }))
  }
  refreshTabTitles()
  syncGlobalState()
  await showAllSysInfo(false, false, routePermissionSystems)
  /* 绑定右键菜单事件 */
  bindContextMenus()
  document.addEventListener('click', onClickOutside)
  getFavList()
  getSysMenuRecords()
  getFixedList()
  getWorkBenchFunctionList()
})

// 监听 token，就绪后补齐路由与路由文案，避免刷新后出现 Key
watch(
  () => userStore.access_token,
  async (token) => {
    if (!token) return
    try {
      const currentLanguage = navigationStore?.language || i18n.locale.value || 'zh'
      const routePermissionSystems = await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
      await loadLocaleMessages(i18n, currentLanguage, {
        includeCommonMessages: true,
        includeRouteMessages: true,
        routeSourceData: routePermissionSystems
      })
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('main-app-i18n-updated', { detail: { locale: currentLanguage } }))
      }
      refreshTabTitles()
      syncGlobalState()
    } catch (e) {
      console.warn('load route i18n on token ready failed:', e)
    }
  },
  { immediate: true }
)

watch(
  () => i18n.locale.value,
  () => {
    refreshTabTitles()
  }
)

const isMainLayout = computed(() => {
  const currentPath = router.currentRoute.value.path.toLowerCase()
  const isLoginRoute = currentPath === '/login'
  return IsExpiredToken(userStore) && !isLoginRoute
})

function IsExpiredToken(userStore) {
  const currentTimeMillis = new Date().getTime()
  const current = Math.floor(currentTimeMillis / 1000)
  const expiration = userStore.expiration_timestamp === undefined ? 0 : userStore.expiration_timestamp
  if (current >= expiration || !userStore.access_token) {
    return false
  }
  return true
}

onBeforeUnmount(() => {
  document.removeEventListener('click', onClickOutside)
  window.removeEventListener('sub-app-add-tab', handleSubAppAddTab)
})
const onContextMenuClick = ({ key }) => {
  switch (key) {
    case 'closeOthers':
      navigationStore.tabs = navigationStore.tabs.filter((tab) => tab.key === contextKey.value)
      const entity = navigationStore.tabs.find((obj) => obj.key === contextKey.value)
      if (entity) {
        const rootPath = entity.sysCode ? `/${entity.sysCode}` : findRootPath(routeStore.routes, entity.path)
        if (rootPath === '/') {
          drawerStore.AddSelectFirstMenuPath(['/'])
          router.push({ path: entity.path })
        } else {
          if (rootPath !== drawerStore.selectFirstMenuPath[0]) {
            router.push({ path: `${rootPath}${entity.path}` })
          } else {
            const event = new CustomEvent('main-app-trigger-route', { detail: { menu: rootPath, path: entity } })
            window.dispatchEvent(event)
            return
          }
        }
      }
      break

    case 'closeAll':
      navigationStore.removeAllTab()
      drawerStore.AddSelectFirstMenuPath(['/'])
      drawerStore.changeSelected(['/'])
      window.location.href = '/'
      break
  }

  menuVisible.value = false
}

const ecAuth = async () => {
  if (/[?&]code=/.test(window.location.href)) {
    const urlParams = new URLSearchParams(window.location.search)
    const authorizationCode = urlParams.get('code')
    if (authorizationCode) {
      const data = await useGetSFToken(authorizationCode, 'zhbg')
      const sf = JSON.parse(data.data)
      const sxarry = Object.keys(sf)
      if (sxarry.includes('access_token')) {
        await ssologin(sf.access_token)
      } else if (sxarry.includes('errcode')) {
        console.log(`errcode:${sf.errcode},msg:${sf.msg}`)
      }
    } else {
    }
  }
}

const ssologin = async (sfToken) => {
  try {
    if (!sfToken) {
      return
    }

    const tokenData = await usePassWordLogin({
      ClientId: 'znpt',
      ClientSecret: 'secret',
      IsDSF: true,
      SFToken: sfToken,
      ConfigCode: 'zhbg'
    })
    if (tokenData.code === 200 && tokenData.success) {
      let access_token = tokenData.data.access_token.split('.')
      // let statement = JSON.parse(decodeURIComponent(window.atob(access_token[1].replace(/-/g, '+').replace(/_/g, '/'))))
      let statement = base64UrlToUtf8Json_legacy(access_token[1])
      if (typeof statement.role === 'string') {
        statement.role = [statement.role]
      }
      userStore.setUserMessage(
        statement.realname,
        statement.ptid,
        tokenData.loginname,
        tokenData.data.access_token,
        tokenData.data.refresh_token,
        tokenData.data.expires_in,
        statement.exp, //秒级
        statement.role
      )
      //获取动态路由
      const routePermissionSystems = await routeStore.loadRoutes(userStore.userRoles)
      // 动态添加路由
      routeStore.routes.forEach((route) => {
        router.addRoute(route)
      })

      //重置tabs
      navigationStore.removeAllTab()

      useInsertSysLoginUserLogAsync({ CreateId: statement.ptid, LoginName: tokenData.loginname }).then((data) => {
        if (data.code === 200 && data.success) {
          userStore.setlogid(data.data)
        }
      })

      //告诉后端要登录了
      signalRStore.connection.invoke('Login', statement.sub) //发送消息
      //检查是否有未读消息
      signalRStore.setIdsUserId(statement.sub)
      signalRStore.setPtUserId(statement.ptid)
      const query = {
        userId: statement.sub
      }
      getNoReadMsgApi(query).then((res) => {
        if (res.code === 200 && res.success) {
          signalRStore.setMsgCount(res.total)
        }
      })

      message.success('登录成功!')

      //获取微应用系统信息，并写入 userStore，实际 registerMicroApps/start 统一在 main.js 中执行
      const currentLanguage = navigationStore?.language || i18n.locale.value || 'zh'
      await loadLocaleMessages(i18n, currentLanguage, {
        includeCommonMessages: true,
        includeRouteMessages: true,
        routeSourceData: routePermissionSystems
      })
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('main-app-i18n-updated', { detail: { locale: currentLanguage } }))
      }
      refreshTabTitles()
      syncGlobalState()
      let sysInfoData = Array.isArray(routePermissionSystems) ? routePermissionSystems : []
      const canReuseRouteSystems = sysInfoData.length > 0 && sysInfoData.every((item) => typeof item?.subSysAccessUrl === 'string' && item.subSysAccessUrl)

      if (!canReuseRouteSystems) {
        const obj = buildPermissionQuery({
          accurate: 0,
          roleids: userStore.userRoles,
          langCode: currentLanguage
        })
        const sysInfo = await useGetPermissionAsync(obj)
        sysInfoData = Array.isArray(sysInfo?.data) ? sysInfo.data : []
      }

      sysInfoData.forEach((item) => {
        userStore.setSysCodeUrlItem(`${item.subSysAccessUrl}_${item.sysCode}`)
        userStore.setSysCodeItem(item.sysCode)
      })

      getPlatformNameApi().then((res) => {
        if (res.code === 200 && res.success) {
          drawerStore.SetPlatformName(res.data)
        } else {
          message.warn('获取系统名称异常，请联系管理员')
        }
      })

      // 根据最新的微应用配置初始化 qiankun（只会在当前页面生命周期内执行一次）
      if (window.__init_qiankun__) {
        window.__init_qiankun__()
      }
      console.log('4.3')
      router.push({ path: '/' })
      console.log('4.4')
    } else {
      message.error(tokenData.message)
    }
  } catch (error) {
    console.log(error)
  }
}
ecAuth()

function onClickOutside(e) {
  // 如果右键菜单打开，点击了非菜单区域，就关闭
  if (!menuVisible.value) return
  const menuEl = menuRef.value?.$el // 获取 dropdown DOM
  if (menuEl && !menuEl.contains(e.target)) {
    menuVisible.value = false
  }
}

// 添加一个方法来等待国际化资源加载完成
async function waitForI18nReady() {
  // 如果使用的是动态加载的国际化资源，可以在这里添加等待逻辑
  // 例如，等待一个特定的标志或者检查特定的资源是否已加载
  return new Promise((resolve) => {
    // 这里可以添加检查逻辑，例如每100ms检查一次资源是否加载完成
    // const checkInterval = setInterval(() => {
    //   // 检查是否有特定的国际化键可用，作为资源加载完成的标志
    //   try {
    //     // const testKey = $t('message.route.platform')
    //     // if (testKey && testKey !== 'message.route.platform') {
    //     //   clearInterval(checkInterval)
    //     //   resolve()
    //     // }
    //   } catch (e) {
    //     // 如果出错，继续等待
    //   }
    // }, 100)

    // 设置一个超时，避免无限等待
    setTimeout(() => {
      // clearInterval(checkInterval)
      resolve()
    }, 100)
  })
}

// 存储每个图标的组件引用
// const iconComponents = ref({})
const hovered = ref(false)
/* 获取Logo图片 */
import avatarImageW from '@/assets/images/NewLogo.png'
import avatarImageB from '@/assets/images/NewLogoW.png'
var avatarImageURL = drawerStore.theme == 'light' ? avatarImageW : avatarImageB
/* 监听主题变化加载背景图片 */
watch(
  () => drawerStore.theme,
  () => {
    avatarImageURL = drawerStore.theme == 'light' ? avatarImageW : avatarImageB
  }
)

/* tab功能 */
function findRootPath(routes, targetPath) {
  function recursiveSearch(routeList, currentRootPath) {
    for (const route of routeList) {
      if (route.path === targetPath) {
        return currentRootPath || route.path // 找到目标路径，返回当前根节点路径
      }
      if (route.children && route.children.length > 0) {
        // 继续在子节点中搜索
        const result = recursiveSearch(route.children, currentRootPath || route.path)
        if (result) {
          return result // 如果在子节点中找到了，返回结果
        }
      }
    }
    return null // 未找到目标路径
  }

  // 从根节点开始搜索
  return recursiveSearch(routes, '') || '/'
}

const tabclick = (key) => {
  if (key == '/') {
    window.location.href = '/'
    return
  }
  const entityRaw = navigationStore.tabs.find((obj) => obj.key === key)
  const entity = normalizeTabEntity(entityRaw)
  if (entity) {
    const innerPath = normalizeTabInnerPath(entity)
    const fullPath = buildTabFullPath(entity)
    const rootPath = entity.sysCode ? `/${entity.sysCode}` : findRootPath(routeStore.routes, innerPath)
    if (rootPath === '/') {
      drawerStore.AddSelectFirstMenuPath(['/'])
      router.push({ path: innerPath })
    } else {
      if (rootPath !== drawerStore.selectFirstMenuPath[0]) {
        router.push({ path: fullPath })
      } else {
        const event = new CustomEvent('main-app-trigger-route', { detail: { menu: rootPath, path: innerPath } })
        window.dispatchEvent(event)
        return
      }
    }
  }
}

const add = () => { }
const remove = (targetKey) => {
  const tabs = navigationStore.tabs
  const length = tabs.length
  // 若点击的是最后一个tabs,则跳转综合首页
  if (length === 1) {
    navigationStore.removeTab(targetKey)
    drawerStore.AddSelectFirstMenuPath(['/'])
    drawerStore.changeSelected(['/'])
    window.location.href = '/'
  } else {
    const index = tabs.findIndex((tab) => tab.key === targetKey)

    if (index === -1) {
      navigationStore.removeTab(targetKey)
      return
    }

    const closingTab = normalizeTabEntity(tabs[index])
    const closingRootPath = closingTab.sysCode ? `/${closingTab.sysCode}` : findRootPath(routeStore.routes, normalizeTabInnerPath(closingTab))

    // 优先根据当前路由判断是否正在关闭激活中的 tab，避免依赖可能不同步的 drawerStore 选中状态
    const isClosingCurrentByRoute = router.currentRoute.value.path === targetKey

    // 兼容原有基于 drawerStore 的判定逻辑，作为补充条件
    const isClosingCurrentByDrawer =
      drawerStore.selectFirstMenuPath[0] === closingRootPath && (drawerStore.selected[0] === closingTab.path || drawerStore.selected[0] === closingTab.key)

    const isClosingCurrentTab = isClosingCurrentByRoute || isClosingCurrentByDrawer

    // last
    let checktab
    if (index === length - 1) {
      checktab = tabs[length - 2]
    } else {
      checktab = tabs[length - 1]
    }

    navigationStore.removeTab(targetKey)

    // select
    if (isClosingCurrentTab && checktab) {
      const normalizedCheckTab = normalizeTabEntity(checktab)
      const checkTabInnerPath = normalizeTabInnerPath(normalizedCheckTab)
      const checkTabFullPath = buildTabFullPath(normalizedCheckTab)
      const lastMenuPath = normalizedCheckTab.sysCode ? `/${normalizedCheckTab.sysCode}` : findRootPath(routeStore.routes, checkTabInnerPath)

      // 通过两种信号判断“是否同一子应用系统”
      // 1. 顶部一级菜单当前选中系统（旧逻辑，兼容历史行为）
      const isSameSystemByDrawer = drawerStore.selectFirstMenuPath[0] === lastMenuPath
      // 2. 当前路由 path 前缀是否匹配目标系统（避免 selectFirstMenuPath 未及时同步导致误判）
      const isSameSystemByRoute = lastMenuPath !== '/' && router.currentRoute.value.path.startsWith(lastMenuPath)

      if (isSameSystemByDrawer || isSameSystemByRoute) {
        // 同一子应用内切换 tab：仅在当前子应用内部 router.push，避免整页刷新
        drawerStore.changeSelected([normalizedCheckTab.key])
        const event = new CustomEvent('main-app-trigger-route', { detail: { menu: lastMenuPath, path: checkTabInnerPath } })
        window.dispatchEvent(event)
      } else {
        // 跨系统：通过 location.href 进行整页跳转，保持原有逻辑
        window.location.href = checkTabFullPath
      }
    }
  }
}
const onEdit = (targetKey, action) => {
  if (action === 'add') {
    add()
  } else {
    remove(targetKey)
  }
}

function handleMenuClickWorkbench(e) {
  // 阻止事件冒泡，从而不关闭Dropdown
  e.stopImmediatePropagation()
  // 可以在这里添加其他逻辑，比如根据点击的菜单项执行操作等。
}

// **菜单数据**
const menuItems = ref([])

const favList = ref([])
/* 获取收藏列表 */
function getFavList() {
  favList.value = []
  const obj = {
    userid: userStore.userid,
    roleids: userStore.userRoles
  }
  getFavInfoList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        var newFav = {
          menuTitle: item.functionName,
          menuRoute: item.route,
          sysCode: item.sysCode,
          menuId: item.id,
          menuCode: item.functionCode,
          menuIcon: item.icon
        }
        favList.value.push(newFav)
      })
      return
    }
    message.error('获取收藏列表异常，请刷新重试')
  })
}
const fixedList = ref([
  {
    key: '/',
    sysCode: 'ComprehensiveHomePage',
    subSysIcon: 'PieChartOutlined'
  }
])

const allSysInfo = ref([])
//获取固定系统列表
function getFixedList() {
  fixedList.value = [
    {
      key: '/',
      sysCode: 'ComprehensiveHomePage',
      subSysIcon: 'PieChartOutlined'
    }
  ]
  const obj = {
    userid: userStore.userid
  }
  GetFixedInfoList(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        item.key = `/${item.sysCode}`
        fixedList.value.push(item)
      })

      return
    }
    message.error('获取固定系统列表异常，请刷新重试')
  })
}

async function UnCollect(pid, isSys) {
  const obj = {
    userid: userStore.userid,
    Permissionid: pid
  }
  await DelFavInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      if (isSys) {
        getFixedList()
      } else {
        getFavList()
      }
      message.success('操作成功')
      return
    }
    message.error('操作失败，请刷新页面重试')
  })
}

async function AddCollect(pid, isSys) {
  const obj = {
    userid: userStore.userid,
    Permissionid: pid,
    isSysInfo: isSys
  }
  await AddFavInfo(obj).then((res) => {
    if (res.code == 200 && res.success) {
      if (isSys) {
        getFixedList()
      } else {
        getFavList()
      }
      message.success('操作成功')
      return
    }
    message.error('操作失败，请刷新页面重试')
  })
}

function JumpToMenu(item, sysCode) {
  const tabSysCode = sysCode || item.sysCode
  const tabKey = tabSysCode ? `/${tabSysCode}${item.menuRoute}` : item.menuRoute

  addNavigationTabIfMissing({
    key: tabKey,
    title: item.menuTitle,
    path: item.menuRoute,
    sysCode: tabSysCode,
    i18nKey: `message.route.XT${item.menuCode}`
  })

  if (tabSysCode) {
    window.location.href = `/${tabSysCode}${item.menuRoute}`
  } else {
    router.push({ path: item.menuRoute })
  }
}



/* 获取最近访问 */
const menuRecords = ref([])
async function getSysMenuRecords() {
  menuRecords.value = []
  await getSysMenuRecord({ userid: userStore.userid, pageIndex: 1, pageSize: 10 }).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((item) => {
        var newItem = {
          menuRoute: item.menuRoute,
          menuTitle: item.menuName,
          menuCode: item.functionCode,
          menuIcon: item.menuIcon,
          sysCode: item.sysCode
        }
        menuRecords.value.push(newItem)
      })
    }
  })
}

// **当前悬浮的菜单 ID**
const hoveredId = ref(null)
// **右侧显示的二级菜单**
const persistedSubMenu = ref(null)

// **鼠标悬浮时更新二级菜单**
const onHover = (item) => {
  hoveredId.value = item.sysCode
  persistedSubMenu.value = item.subMenu
}

// 监听 token 变化，当 token 改变时再次获取数据
watch(
  () => userStore.access_token,
  async () => {
    if (userStore.access_token == '') {
      return
    }

    await nextTick()
    const currentLanguage = navigationStore?.language ?? ''
    const cachedOrLoaded = await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
    await showAllSysInfo(false, false, cachedOrLoaded)
    getFavList()
    getSysMenuRecords()
    getFixedList()
  }
)

/**
 * 递归提取所有有 routeFile 的菜单项
 */
function extractLeafMenus(menus = []) {
  const result = []
  for (const menu of menus) {
    if (menu.routeFile) {
      result.push({
        menuCode: menu.functionCode,
        menuId: menu.id,
        menuTitle: menu.functionName,
        menuRoute: menu.route,
        menuIcon: mapIcon(menu.icon)
      })
    } else if (Array.isArray(menu.menuDtos) && menu.menuDtos.length > 0) {
      result.push(...extractLeafMenus(menu.menuDtos))
    }
  }

  return result
}

const isBreak = ref(false)

function mapIcon(iconName) {
  return Icons[iconName] || Icons['QuestionCircleOutlined']
}

function filterMenusByKeyword(menus = [], keyword = '') {
  return menus
    .map((menu) => {
      const nameMatched = menu.functionName?.toLowerCase().includes(keyword)
      const selfIsLeaf = menu.routeFile && (!menu.menuDtos || menu.menuDtos.length === 0)

      let children = []
      if (Array.isArray(menu.menuDtos)) {
        children = filterMenusByKeyword(menu.menuDtos, keyword)
      }

      if ((selfIsLeaf && nameMatched) || children.length > 0) {
        return {
          ...menu,
          menuDtos: children
        }
      }

      return null
    })
    .filter(Boolean)
}

async function showAllSysInfo(isSearch, isBtn, externalSysInfoList = null) {
  const currentLanguage = navigationStore?.language ?? ''
  let sourceSysInfoList = Array.isArray(externalSysInfoList) ? externalSysInfoList : null

  if (!sourceSysInfoList) {
    sourceSysInfoList = routeStore.getCachedPermissionSystems(userStore.userRoles, currentLanguage)
  }

  if (!sourceSysInfoList) {
    sourceSysInfoList = await routeStore.loadRoutes(userStore.userRoles, currentLanguage)
  }

  let displaySysInfoList = sourceSysInfoList

  allSysInfo.value = [
    {
      key: '/',
      sysCode: 'ComprehensiveHomePage',
      subSysIcon: 'PieChartOutlined'
    }
  ]
  sourceSysInfoList.forEach((item) => {
    if (item.sysCode === 'XTMarkDown') return
    allSysInfo.value.push({ key: `/${item.sysCode}`, sysCode: item.sysCode, subSysIcon: item.subSysIcon })
  })
  if (isSearch) {
    const keyword = workbenchSerchKey.value?.trim()?.toLowerCase()
    displaySysInfoList = sourceSysInfoList
      .map((sys) => {
        if (!Array.isArray(sys.menuDtos)) return null

        // 深拷贝防止污染原结构
        const filteredMenus = filterMenusByKeyword(sys.menuDtos, keyword)

        return filteredMenus.length > 0 ? { ...sys, menuDtos: filteredMenus } : null
      })
      .filter(Boolean) // 去掉 null 的系统
  }
  // 清空原有 menuItems
  menuItems.value = []

  displaySysInfoList.forEach((sys, index) => {
    //  跳过 XTMarkDown 系统
    if (sys.sysCode === 'XTMarkDown') {
      isBreak.value = true
      return
    }

    // 忽略没有菜单数据的系统
    if (!Array.isArray(sys.menuDtos)) return
    const systemNode = {
      systemId: sys.sysInfoId,
      sysCode: sys.sysCode,
      systemTitle: sys.subSysName,
      systemIcon: mapIcon(sys.subSysIcon),
      subMenu: []
    }

    const allFirstLevelMenus = sys.menuDtos

    for (const first of allFirstLevelMenus) {
      const childLeafItems = extractLeafMenus(first.menuDtos || [])

      const selfIsLeaf = first.routeFile && (!first.menuDtos || first.menuDtos.length === 0)

      const mergedItems = []

      if (selfIsLeaf) {
        mergedItems.push({
          menuId: first.id,
          menuTitle: first.functionName,
          menuRoute: first.route,
          menuIcon: mapIcon(first.icon),
          menuCode: first.functionCode
        })
      }

      mergedItems.push(...childLeafItems)

      // 如果合并后还有内容才加
      if (mergedItems.length > 0) {
        systemNode.subMenu.push({
          menuTitle: first.functionName,
          menuIcon: mapIcon(first.icon),
          items: mergedItems,
          menuId: first.id,
          menuCode: first.functionCode
        })
      }
    }
    // 如果 subMenu 是纯一级 leaf（扁平结构），可以转成 item 数组
    if (systemNode.subMenu.length === 0) {
      // fallback: 提取最底层 leaf 节点
      const flatLeaves = extractLeafMenus(allFirstLevelMenus)
      if (flatLeaves.length > 0) {
        systemNode.subMenu = flatLeaves
      }
    }

    if (systemNode.subMenu.length > 0) {
      menuItems.value.push(systemNode)
    }
  })

  if (isSearch && isBtn && workbenchSerchKey.value != '') {
    userStore.addSearchKey(workbenchSerchKey.value)
  }
}

/* 条件筛选 */
const workbenchSerchKey = ref('')
// 添加防抖搜索方法（延迟 300ms 执行）
const debouncedSearch = debounce(() => {
  showAllSysInfo(true, false)
}, 300)
// 监听条件变化，前端筛选菜单列表
watch(
  () => workbenchSerchKey.value,
  () => {
    debouncedSearch()
    hoveredId.value = null
    persistedSubMenu.value = null
  }
)
function setSearchKey(item) {
  workbenchSerchKey.value = item
}

const clickItem = (val) => {
  // useInsertSysMenuRecord({
  //   CreateId: userStore.userid,
  //   MenuId: val.item.sysInfoId,
  //   IsSysInfoId: val.item.isSysInfoId
  // })
  if (val.key == '/') {
    drawerStore.changeSelected(['/'])
    router.push(val.key)
    return
  }
  const sysCode = val.item.sysCode
  const readyMap = userStore.subAppReady || {}
  if (sysCode && !readyMap[sysCode]) {
    userStore.isLoading = true
  }

  router.push(val.key)
  // drawerStore.AddSelectFirstMenuPath(val.keyPath)
}

// 根据当前路由自动同步主应用的 drawerStore 选中状态
watch(
  () => router.currentRoute.value.path,
  (path) => {
    if (!path) return

    const lower = path.toLowerCase()
    // 登录页不处理选中状态
    if (lower === '/login') {
      return
    }

    // 综合首页：统一认为选中根路径
    if (path === '/') {
      if (drawerStore.selectFirstMenuPath[0] !== '/') {
        drawerStore.AddSelectFirstMenuPath(['/'])
      }
      if (drawerStore.selected[0] !== '/') {
        drawerStore.changeSelected(['/'])
      }
      return
    }

    // 其它路径：根据 URL 前缀推导所属系统，例如 /platform/..., /ids4/...
    const segments = path.split('/').filter(Boolean)
    const rootPath = segments.length ? `/${segments[0]}` : '/'

    if (drawerStore.selectFirstMenuPath[0] !== rootPath) {
      drawerStore.AddSelectFirstMenuPath([rootPath])
    }

    // 选中的菜单始终使用完整路由路径（包括子应用内部路径），避免停留在系统根路径
    if (drawerStore.selected[0] !== path) {
      drawerStore.changeSelected([path])
    }
  },
  { immediate: true }
)

//设置全局通信
import actions from './actions'

let isUpdatingFromGlobalState = false

// 构建用于 qiankun 全局通信的纯数据快照，避免传递 Pinia store 实例
const buildGlobalStateSnapshot = () => {
  return {
    userStore: {
      realname: userStore.realname,
      userid: userStore.userid,
      userloginlogid: userStore.userloginlogid,
      loginname: userStore.loginname,
      access_token: userStore.access_token,
      refresh_token: userStore.refresh_token,
      expires_in: userStore.expires_in,
      expiration_timestamp: userStore.expiration_timestamp,
      userRoles: userStore.userRoles,
      isLoading: userStore.isLoading,
      subAppReady: userStore.subAppReady,
      allSysCodeUrl: userStore.allSysCodeUrl,
      allSysCode: userStore.allSysCode,
      searchKey: userStore.searchKey
    },
    drawerStore: {
      open: drawerStore.open,
      theme: drawerStore.theme,
      menumode: drawerStore.menumode,
      menuwidth: drawerStore.menuwidth,
      menuwidthCopy: drawerStore.menuwidthCopy,
      menuCollapsed: drawerStore.menuCollapsed,
      selected: drawerStore.selected,
      foldmenu: drawerStore.foldmenu,
      displaybread: drawerStore.displaybread,
      selectFirstMenuPath: drawerStore.selectFirstMenuPath,
      platformName: drawerStore.platformName
    },
    navigationStore: {
      language: navigationStore.language,
      bread: navigationStore.bread,
      // tabs 在 Pinia 中是 ref，这里需要展开成普通数组，方便子应用直接通过 length 判断
      tabs: Array.isArray(navigationStore.tabs) ? navigationStore.tabs : Array.isArray(navigationStore.tabs?.value) ? navigationStore.tabs.value : []
    },
    i18nSnapshot: {
      locale: navigationStore.language,
      messages: {}
    },
    routePermissionSnapshot: getRoutePermissionSnapshot(),
    signalRStore: {
      msgCount: signalRStore.msgCount,
      idsUserId: signalRStore.idsUserId,
      ptUserId: signalRStore.ptUserId,
      durationTime: signalRStore.durationTime,
      isShow: signalRStore.isShow,
      msgId: signalRStore.msgId
    }
  }
}

// 同步当前主应用的 pinia store 到 qiankun 全局状态
const syncGlobalState = () => {
  const snapshot = buildGlobalStateSnapshot()
  /* 设置全局状态（只传递纯数据快照，避免 cloneDeep Pinia 实例导致栈溢出） */
  actions.setGlobalState(snapshot)
}

syncGlobalState()

/* 获取全局状态 */
actions.onGlobalStateChange((state, prev) => {
  //state:变更后的状态；prev:变更前的状态
  if (state && state.drawerStore) {
    const ds = state.drawerStore

    isUpdatingFromGlobalState = true
    try {
      // 子应用可能推的是各种形式（boolean / 0/1 / 字符串），统一用布尔值写回 Pinia
      if (typeof ds.menuCollapsed !== 'undefined') {
        drawerStore.menuCollapsed = !!ds.menuCollapsed
      }

      if (typeof ds.menuwidth === 'number') {
        drawerStore.menuwidth = ds.menuwidth
      }

      if (typeof ds.menuwidthCopy === 'number') {
        drawerStore.menuwidthCopy = ds.menuwidthCopy
      }

      if (Array.isArray(ds.foldmenu)) {
        drawerStore.setFoldmenu(ds.foldmenu)
      }
    } finally {
      isUpdatingFromGlobalState = false
    }
  }
})

// 监听关键字段变化，在登录成功（token、角色就绪）、语言、主题、菜单宽度/收起状态以及标签页数量变化时，主动推送最新全局状态给子应用
watch(
  () => [
    userStore.access_token,
    userStore.userRoles,
    navigationStore.language,
    Array.isArray(routeStore.routes) ? routeStore.routes.length : 0,
    drawerStore.theme,
    drawerStore.menuwidth,
    drawerStore.menuCollapsed,
    // 通过 tabs 长度来感知标签页增删，避免仅引用同一个数组实例导致 watch 不触发
    Array.isArray(navigationStore.tabs) ? navigationStore.tabs.length : Array.isArray(navigationStore.tabs?.value) ? navigationStore.tabs.value.length : 0
  ],
  (newVal, oldVal) => {
    if (isUpdatingFromGlobalState) {
      return
    }
    syncGlobalState()
  }
)

/* 展示设置抽屉 */
const showDrawer = () => {
  drawerStore.setOpen(true)
}

// const GoToRuleEngineDesign = () => {
//   window.open('http://192.168.50.128:1881/', '_blank')
// }

// const GoToAIAddress = () => {
//   window.open('http://192.168.57.218:8000/', '_blank')
// }


// const GoToConfigurationDesigner = () => {
//   window.open('http://175.27.188.92:1880/', '_blank')
// }

// const GoToDSJ = () => {
//   window.open('http://172.16.139.7:28180/index/menu?self_auth_enable=true', '_blank')
// }

function checkURL(url) {
  window.open(url, '_blank')
}

function test() {
  const redirect = '/workFlow/flowQuickLaunch'

  const url =
    window.__APP_CONFIG__?.services?.WorkFlowUrl +
    '/CeriOS_workFlow/login-bridge' +
    `?token=${encodeURIComponent(userStore.access_token)}` +
    `&redirect=${encodeURIComponent(redirect)}`

  window.open(url, '_blank')
}

/* 退出按钮相关 */
const exitModal = ref(false)
const logoff = () => {
  exitModal.value = true
}

const { proxy } = getCurrentInstance()

const handleOk = async () => {
  if (userStore.userloginlogid != '') {
    await useUpdateSysLoginUserLogAsync(userStore.userloginlogid)
  }
  //关闭通知
  if (signalRStore.currentNotification) signalRStore.currentNotification.close('aaa')

  exitModal.value = false
  localStorage.clear()
  proxy.$inactivityMonitor?.stop()
  localStorage.setItem('app', 'app')
  window.location.href = '/Login'
}

//消息弹框
const msgId = ref()
signalRStore.setConnection(
  new signalR.HubConnectionBuilder()
    .withUrl(window.__APP_CONFIG__.services.Business + '/api/chat') //这里一定要写指定的ip，否则报错，大坑搞了1天的时间
    .withAutomaticReconnect()
    .build()
)
//开启
signalRStore.connection.start()
signalRStore.connection.on('ChatInfo', (msg) => {
  getMsg(msg)
})

//消息列表
const msgData = ref([])
const dingEnable = ref(false)
const wechatEnable = ref(false)

const tabs = computed(() => {
  const arr = [
    {
      key: '1',
      title: t('home.AnnouncementsNotifications'),
      icon: NotificationOutlined,
      data: msgData.value,
    },
  ]

  if (wechatEnable.value) {
    arr.push({
      key: '2',
      title: t('home.WeComMessage'),
      icon: WechatOutlined,
      data: msgData.value,
    })
  }

  if (dingEnable.value) {
    arr.push({
      key: '3',
      title: t('home.DingTalkMessage'),
      icon: DingdingOutlined,
      data: msgData.value,
    })
  }

  arr.push({
    key: '4',
    title: t('home.ProcessNotification'),
    icon: BranchesOutlined,
    data: msgData.value,
  })

  return arr
})
const msgOpen = ref(false)
const activeKey = ref('1')
function tabChange() {
  getMsgList()
}

getDingSelectState().then((res) => {
  if (res.code === 200 && res.success) {
    dingEnable.value = !!res.data
  }
})

getWeChatSelectState().then((res) => {
  if (res.code === 200 && res.success) {
    wechatEnable.value = !!res.data
  }
})
//查询未读消息
function showMsg() {
  msgOpen.value = true
  getMsgList()
}
function getMsgList() {
  msgData.value = []
  const obj = {
    pageIndex: 1,
    pageSize: 999,
    receiveUser: userStore.userid,
    msgType: activeKey.value,
    isRead: false
  }

  getAllApi(obj).then((res) => {
    if (res.success && res.code == 200) {
      msgData.value = res.data.map((item) => ({
        sendTime: item.sendTime.replace('T', ' '),
        title: item.title,
        sendUser: item.sendUser,
        content: item.content,
        msgRecordId: item.msgRecordId
      }))
    }
  })
}
function msgDetail(id) {
  signalRStore.setCurrentNotification(notification)
  signalRStore.setMsgId(id)
  signalRStore.setIsShow(true)
}
const getMsg = async (id) => {
  const query = {
    userId: id
  }
  await getNoReadMsgApi(query).then((res) => {
    if (res.code === 200 && res.success) {
      signalRStore.setMsgCount(res.total)
      //有了列表就不右下角弹框了
      // if (res.total > 0) {
      //   msgId.value = res.data[0].msgRecordId
      //   openNotification('bottomRight', res.data[0].title, res.data[0].content)
      // }
    }
  })
}

function openNotification(placement, a, b) {
  notification.open({
    key: 'aaa',
    message: a,
    description: b,
    placement,
    duration: null,
    icon: () => h(MessageOutlined),
    onClose: () => {
      msgId.value = null
    },
    onClick: () => {
      signalRStore.setCurrentNotification(notification)
      signalRStore.setMsgId(msgId.value)
      signalRStore.setIsShow(true)
    }
  })
}

function OpenMarkDown() {
  router.push({ path: '/XTMarkDown' })
  drawerStore.AddSelectFirstMenuPath(['/XTMarkDown'])
}

const yuan = ref('')
const xin = ref('')
const que = ref('')
const open = ref(false)
const uptpas = () => {
  yuan.value = ''
  xin.value = ''
  que.value = ''
  open.value = true
}

const handleOkpass = () => {
  if (yuan.value == '' || xin.value == '' || que.value == '') {
    message.warning('请输入密码')
    return
  }
  if (xin.value != que.value) {
    message.warning('确认密码与新密码不匹配')
    return
  }
  // const str = import.meta.env.VITE_UPTPwdRule_Regular
  // const str1 = import.meta.env.VITE_UPTPwdRule_Prompt
  ///^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]{6,15}$/
  const passwordPattern = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,15}$/
  if (!passwordPattern.test(xin.value)) {
    message.warning('密码必须包含特殊字符,字母和数字,且长度为6-15位')
    return
  }
  useUpdatePassword({
    UserId: userStore.userid,
    OriginalPassword: yuan.value,
    Password: que.value
  }).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      open.value = false
      yuan.value = ''
      xin.value = ''
      que.value = ''
    } else {
      message.error(data.message)
    }
  })
}

const handlecancel = () => {
  yuan.value = ''
  xin.value = ''
  que.value = ''
}

// function parseJwt(token) {
//   const base64Url = token.split('.')[1]
//   const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
//   const jsonPayload = decodeURIComponent(
//     atob(base64)
//       .split('')
//       .map((c) => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
//       .join('')
//   )
//   return JSON.parse(jsonPayload)
// }
</script>
<style lang="scss">
.MainLayout {
  .AppHeader {
    height: 96px;
    line-height: 96px;
    padding-inline: 0px !important;
    display: flex;
    justify-content: space-between;

    // .AppHeaderContent {
    //   display: flex;
    //   // z-index: 1;
    //   // background-image: url('/src/assets/images/HeaderBG.png'), linear-gradient(to right, rgb(134, 198, 249), rgb(134, 198, 249));
    //   // background-image: url('/src/assets/images/HeaderBG.png'), linear-gradient(to right, #061451, rgb(134, 198, 249));
    //   background-image: url('/src/assets/images/HeaderBG.png');
    //   background-repeat: no-repeat;
    // }

    .AppHeaderContent {
      display: flex;
      max-width: calc(100vw - 250px);
      margin-left: 248px;
      background-image: url('/src/assets/images/headM.png');
      background-size: 100% 100%;

      .AppHeaderImg {
        width: 210px;
        margin-left: -240px;
        margin-right: 40px;
        z-index: 1;
        overflow: hidden;
      }
    }

    // .ant-layout-header {
    //   padding-inline: 0px !important;
    //   background-color: #fff;
    // }

    .ant-menu-dark .ant-menu-item {
      &:hover {
        color: #9ff3ff;
      }
    }

    // .ant-menu-dark .ant-menu-item-selected {
    //   background-color: rgb(19, 74, 222);
    // }
    .ant-menu-dark .ant-menu-item-selected {
      background-color: rgb(19, 74, 222, 0);
      color: #fff;
    }

    .ant-menu-item {
      color: rgba(255, 255, 255, 75%);
    }

    .LayoutMenu {
      height: 64px;
      line-height: 64px;
    }

    .IconF {
      width: 310px;
      height: 64px;
      line-height: 64px;
      display: flex;
      margin-right: 20px;

      .IconS {
        text-align: center;
        width: 40px;
      }

      .IconS:hover {
        background-color: rgb(230, 229, 229);
        cursor: pointer;
      }

      .loginname {
        font-size: 18px;
        font-weight: 700;
        text-align: center;
        line-height: 64px;
        margin: 0px;
        cursor: pointer;
        width: 90px;
      }

      .ant-scroll-number {
        pointer-events: none;
      }
    }

    .TabsCss {
      width: calc(100vw - 1px);
      margin-left: -250px;
      padding-left: 220px;

      .ant-tabs {
        .ant-tabs-nav {
          margin: 0px !important;

          .ant-tabs-nav-list {
            margin-top: 1px;
            margin-left: 10px;

            .ant-tabs-tab {
              padding: 0px 25px !important;
              height: 30px;
              border-radius: 0px;
              background: #fff;
            }

            .ant-tabs-tab-active {
              // background-image: url('/src/assets/images/icons/TabBG2.png');
              background-size: 100% 100%;
              /* 让背景图片铺满整个 tab */
              background-repeat: no-repeat;
              background: linear-gradient(to top, #f4f4f4, #d1e1ee);

              .ant-tabs-tab-btn {
                color: #00498a;
                font-weight: 700;
              }
            }

            .ant-tabs-tab-btn {
              color: #000;
              font-weight: 400;
            }

            .ant-tabs-tab-remove {
              margin-right: -15px !important;
            }
          }
        }

        .ant-tabs-nav-operations {
          height: 30px;
        }
      }
    }
  }

  // 整个滚动条区域
  ::-webkit-scrollbar {
    width: 5px;
    // background-color: rgb(161, 162, 162);
    background-color: #fff;
    border-radius: 5px;
  }

  // 滚动轴区域
  ::-webkit-scrollbar-thumb {
    background-color: rgb(202, 202, 202);
    border-radius: 5px;
  }

  .LayoutDropdown {
    position: relative;

    .workbench-trigger {
      margin: 0px 10px;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 4px;
      height: 64px;

      &:hover {
        color: #1890ff;
      }
    }
  }
}

.workbench-dropdown {
  .workbench-content {
    position: relative;
    width: 100vw;
    min-height: 300px;
    // background-color: #fff;
    background: linear-gradient(to right, #f8fbff, #e1edfa);
    padding: 24px;
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);

    .dropdown-arrow {
      position: absolute;
      top: -5px;
      left: 433px;
      width: 10px;
      height: 10px;
      background: rgb(147, 196, 255);
      transform: rotate(45deg);
      box-shadow: -2px -2px 5px rgba(0, 0, 0, 0.06);
      z-index: -1;
    }

    .workbench-section {
      .userSearchRecord {
        color: #0166b3;
        cursor: pointer;
        background-color: #d8eaff;
        border-radius: 5px;
        margin: 0px 5px;
        font-size: 14px;
        font-weight: 500;
        padding: 0 20px;
        height: 35px;
        line-height: 35px;

        &:hover {
          transform: scale(1.1);
          background-color: #85c6ff;
        }
      }

      .workbench-grid {
        // display: grid;
        // grid-template-columns: repeat(auto-fill, minmax(100%, 1fr));
        // gap: 100px;
        display: flex;
        min-height: 700px;
        max-height: 700px;

        .workbench-item {
          padding: 16px;
          background: #fff;
          border-radius: 4px;
          cursor: pointer;
          transition: all 0.3s;

          // &:hover {
          //   background: #e6f7ff;
          //   color: #1890ff;
          // }

          .BoxContent1 {
            color: #4e5969;
            margin: 5px 0;
            padding: 5px 5px;
            cursor: pointer;

            &:hover {
              color: #1890ff;
            }
          }

          .BoxContent2 {
            color: #4e5969;
            margin: 5px 0;
            padding: 5px 5px;
            cursor: pointer;

            display: flex;
            justify-content: space-between;

            &:hover {
              color: #1890ff;
            }
          }

          .BoxContent3 {
            color: #4e5969;
            margin: 5px 0px;
            padding: 5px 20px;
            transition: background 0.3s;
            cursor: pointer;

            &:hover {
              border-radius: 2px;
              background-color: #e5f1ff;
            }
          }

          .BoxContent3.active {
            border-radius: 2px;
            background-color: #e5f1ff;
            // text-decoration: underline;
          }

          .BoxRightAll {
            text-align: left;
            padding: 10px;
            margin-bottom: 10px;
            border-radius: 8px;
            background-repeat: no-repeat;
            background-size: cover;


            .BoxRightTitle {
              color: #082E5F;
              font-weight: bold;
              font-size: 16px;
            }

            .BoxRightDesc {
              color: #082E5FCC
            }

            .BoxRightBtn {
              background-color: #6BAEFF;
            }
            .BoxRightBtn:hover {
              background-color: #0988ff;
            }
          }
        }
      }
    }
  }

  // 整个滚动条区域
  ::-webkit-scrollbar {
    width: 5px;
    // background-color: rgb(232, 244, 255);
    background-color: #fff;
    border-radius: 5px;
  }

  // 滚动轴区域
  ::-webkit-scrollbar-thumb {
    background-color: #f2f4f7;
    border-radius: 5px;
  }
}

.mimadiv {
  padding: 8px 0;
  display: flex;
  justify-content: center;
  align-items: center;

  .passlabel {
    display: inline-block;
    width: 65px;
    text-align: right;
    margin-right: 5px;
  }
}
</style>