<template>
  <div class="homePageStyle" style="height: calc(100vh - 96px); width: 100%; overflow-y: auto">
    <div class="userBox">
      <div class="userBoxInfo">
        <div class="welcomeTitle">
          <span>{{ userStore.realname }} {{ greeting }}! </span>
          <span
            >{{ $t('common.welcomeTitle') }} {{ drawerStore.platformName }}~
            <img loading="lazy" src="/src/assets/images/hands.png" alt="" />
          </span>
        </div>
        <div class="warnInfo">
          {{ $t('common.HomePageTitle') }}
        </div>
      </div>
    </div>
    <div style="width: 100%; margin-bottom: 50px; display: flex; justify-content: space-around">
      <div style="width: 1370px; margin-top: 10px">
        <div class="RecentlyVisited">
          <a-card :headStyle="CardHeadStyle" style="height: 100%; border: 1px solid rgb(225, 225, 231); border-radius: 5px">
            <div slot="title">
              <div><MenuOutlined /> {{ $t('home.RecentlyVisited') }}</div>
              <div class="jumpTo" @click="JumpToMenuLog()">
                {{ $t('home.ViewMore') }}>
                <!-- <ArrowRightOutlined style="Color:lightgrey !important"/> -->
              </div>
              <a-divider />
            </div>
            <div
              :key="navigationStore.language + '-' + ((routeStore.routes && routeStore.routes.length) || 0) + '-' + i18nTick"
              style="display: flex; flex-wrap: wrap"
              :class="menuRecords.length == 8 ? 'SpaceAround' : 'FlexStart'"
            >
              <div
                class="RecentlyVisitedBox"
                :style="{ backgroundColor: bgColors[index % bgColors.length] }"
                v-for="(item, index) in menuRecords"
                :key="index"
                @click="JumpToMenu(item)"
              >
                <div style="display: flex">
                  <div style="">
                    <a-avatar
                      class="VisitedBoxAvatar"
                      :style="{
                        color: iconColors1[index % iconColors1.length],
                        boxShadow: `-2px -2px 5px 1px ${getShadowColor(iconColors1[index % iconColors1.length])}`
                      }"
                      shape="square"
                      :size="45"
                    >
                      <template #icon>
                        <component v-if="iconComponents[item.menuIcon]" :is="iconComponents[item.menuIcon]" />
                      </template>
                    </a-avatar>
                  </div>
                  <div style="margin-left: 10px; display: flex; flex-direction: column; justify-content: center; align-items: flex-start">
                    <a-tooltip v-if="item.menuName" :title="$t(`message.route.XT${item.functionCode}`)">
                      <div
                        style="font-size: 16px; font-weight: bold; margin: 0; max-width: 180px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis"
                      >
                        {{ $t(`message.route.XT${item.functionCode}`) }}
                      </div>
                    </a-tooltip>
                    <div v-if="!item.menuName" style="font-size: 16px; font-weight: bold; margin: 0">未定义菜单名称</div>
                    <a-tooltip v-if="item.menuName" :title="$t(`message.route.XT${item.sysCode}`) + '——' + $t(`message.route.XT${item.functionCode}`)">
                      <div
                        style="
                          font-size: 12px;
                          color: #666;
                          width: 180px;
                          text-align: left;
                          margin-top: 5px;
                          white-space: nowrap;
                          overflow: hidden;
                          text-overflow: ellipsis;
                        "
                      >
                        {{ $t(`message.route.XT${item.sysCode}`) + '——' + $t(`message.route.XT${item.functionCode}`) }}
                      </div>
                    </a-tooltip>
                  </div>
                </div>
              </div>
            </div>
          </a-card>
        </div>
        <div class="MiddleBox" style="display: flex; justify-content: space-between">
          <a-card class="StaticBox" :headStyle="CardHeadStyle" style="width: 675px; height: 340px; border: 1px solid rgb(225, 225, 231); border-radius: 5px">
            <div slot="title">
              <div><AreaChartOutlined /> {{ $t('home.loginStatistics') }}</div>
              <div class="jumpTo" @click="JumpToUserLog()">{{ $t('home.ViewMore') }}></div>
              <a-divider />
            </div>
            <div class="chartDomStyle" ref="chartDom"></div>
          </a-card>
          <a-card :headStyle="CardHeadStyle" style="width: 675px; height: 340px; border: 1px solid rgb(225, 225, 231); border-radius: 5px">
            <div slot="title">
              <div><FileSearchOutlined /> {{ $t('home.operationLog') }}</div>
              <div class="jumpTo" @click="JumpToUserBtnLog()">{{ $t('home.ViewMore') }}></div>
              <a-divider />
            </div>
            <div style="height: 300px">
              <s-table height="240px" size="small" bordered :pagination="false" :data-source="dataSource" :columns="columns">
                <template #bodyCell="{ text, column, record }">
                  <template v-if="column.dataIndex === 'operationTime'">
                    {{ formatDateTime(text) }}
                  </template>
                </template>
              </s-table>
            </div>
          </a-card>
        </div>
        <div>
          <a-card :headStyle="CardHeadStyle" style="height: 280px; border: 1px solid rgb(225, 225, 231); border-radius: 5px">
            <div slot="title">
              <div style="display: flex; justify-content: space-between">
                <div><FieldTimeOutlined /> {{ $t('home.taskinfo') }}</div>
                <div class="jumpTo" @click="JumpToTaskInfo()">{{ $t('home.ViewMore') }}></div>
              </div>
              <a-divider />
            </div>
            <div>
              <s-table size="small" bordered :pagination="false" :data-source="taskInfoData" :columns="taskColumn">
                <template #bodyCell="{ text, column, record }">
                  <template v-if="column.dataIndex === 'startDate'">
                    {{ formatDateTime(text) }}
                  </template>
                  <template v-if="column.dataIndex === 'isSuccess'">
                    <a-tag v-if="!text" color="#f50">失败</a-tag>
                    <a-tag v-else color="#87d068">成功</a-tag>
                  </template>
                </template>
              </s-table>
            </div>
          </a-card>
        </div>
      </div>
      <div style="width: 480px; margin-top: 10px">
        <div style="margin-bottom: 20px">
          <a-card
            :headStyle="CardHeadStyle"
            style="
              height: 479px;
              background: radial-gradient(ellipse 30% 20% at top center, rgba(255, 255, 255, 0) 0%, rgba(255, 255, 255, 1) 100%);
              border: 1px solid rgb(225, 225, 231);
              border-radius: 5px;
            "
          >
            <!-- height:412px -->
            <div slot="title" style="position: static">
              <div><BellOutlined /> {{ $t('home.sysinfomsg') }}</div>
              <div class="jumpTo" @click="JumpToSysMessage()">{{ $t('home.ViewMore') }}></div>
              <a-divider />
            </div>
            <div style="display: flex; flex-direction: column; overflow-y: auto; height: 380px">
              <div style="display: flex; padding: 10px; justify-content: space-between; width: 100%" v-for="(item, index) in sysMesData" :key="index">
                <div style="display: flex; width: 60%">
                  <div style="font-size: 14px; font-weight: bold">
                    <a-tag :color="item.msgType == 1 ? ' #2db7f5' : item.msgType == 2 ? '#87d068' : '#108ee9'">
                      {{
                        item.msgType == 1
                          ? $t('home.announcement')
                          : item.msgType == 2
                            ? $t('home.wechat')
                            : item.msgType == 3
                              ? $t('home.dingtalk')
                              : $t('home.flow')
                      }}</a-tag
                    >
                  </div>
                  <div
                    style="cursor: pointer; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; text-decoration: underline"
                    @click="msgDetail(item.msgRecordId)"
                  >
                    {{ item.title }}
                  </div>
                </div>
                <div style="font-size: 14px; font-weight: 500">
                  {{ formatDateTime(item.sendTime) }}
                  <span
                    :style="{
                      color: 'red',
                      visibility: item.isRead ? 'hidden' : 'visible'
                    }"
                  >
                    ●
                  </span>
                </div>
              </div>
            </div>
          </a-card>
        </div>
        <div>
          <a-card :headStyle="CardHeadStyle" style="height: 413px; border: 1px solid rgb(225, 225, 231); border-radius: 5px">
            <div slot="title">
              <UnorderedListOutlined /> {{ $t('home.OngoingTasks') }}
              <a-divider />
            </div>
            <div style="height: 320px; display: flex; flex-wrap: wrap; padding: 10px; justify-content: flex-start">
              <a-table :columns="renwucol" :scroll="{ x: 400, y: 240 }" size="small" :pagination="false" :data-source="renwudata"> </a-table>
            </div>
          </a-card>
        </div>
      </div>
    </div>

    <div class="footCopyright">{{ footTitle }}</div>
  </div>
</template>

<script setup>
import { computed, onMounted, h, ref, onBeforeUnmount,watch  } from 'vue'
import {
  AreaChartOutlined,
  InfoCircleOutlined,
  MenuFoldOutlined,
  MenuOutlined,
  FileSearchOutlined,
  UnorderedListOutlined,
  BellOutlined,
  ArrowRightOutlined
} from '@ant-design/icons-vue'
import { notification } from 'ant-design-vue'
import { getSysMenuRecord, getuserLoginLogs, useGetButtonOperationLogAsync, getAllApi } from '@/api/sysinfo.js'
import { getLast7DaysDates, formatDateTime } from '@/utils/dateUtils.js'
import { getAllTaskLogApi } from '@/api/Msg/task.js'
import { getTaskDetailByCode } from '@/api/userLoginLogs.js'
import { useUserStore, useSignalRStore, useDrawerStore, useRouteStore, useNavigationStore } from '../store'
const userStore = useUserStore()
const signalRStore = useSignalRStore()
const drawerStore = useDrawerStore()
// 监听主应用 i18n 文案合并完成事件，用于触发“最近访问”区域强制重渲
const i18nTick = ref(0)
const onI18nUpdated = () => {
  i18nTick.value++
}
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
const footTitle = import.meta.env.VITE_SYSCOPYRIGHT
/* AntD控件编辑变量 */
const CardHeadStyle = computed(() => {
  return { fontSize: '14px', fontWeight: '400' }
})

const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) {
    return t('common.Morning')
  } else if (hour < 18) {
    return t('common.Afternoon')
  } else {
    return t('common.Evening')
  }
})

const bgColors = ['#F3F7FD', '#FFF9F3', '#EFFCFA', '#FDF9F9', '#F6F6FF']
const iconColors1 = ['#3491FA', '#FFBB00 ', '#03BBA2', '#EC4C52', '#968FDF']
function getShadowColor(color) {
  const rgb = color
    .replace('#', '')
    .match(/.{2}/g)
    ?.map((c) => parseInt(c, 16)) ?? [0, 0, 0]
  return `rgba(${rgb.map((v) => Math.max(0, v - 15)).join(',')}, 0.3)` // 更浅一点的阴影
}

/* 引入Echarts */
import * as echarts from 'echarts/core'
import { BarChart, LineChart } from 'echarts/charts'
import { TitleComponent, TooltipComponent, GridComponent, DatasetComponent, TransformComponent } from 'echarts/components'
import { LabelLayout, UniversalTransition } from 'echarts/features'
import { CanvasRenderer } from 'echarts/renderers'

// 注册必须的组件
echarts.use([
  LineChart,
  TitleComponent,
  TooltipComponent,
  GridComponent,
  DatasetComponent,
  TransformComponent,
  BarChart,
  LabelLayout,
  UniversalTransition,
  CanvasRenderer
])

//登录统计
var xAxisData = getLast7DaysDates()
const tomorrowMidnightString = (() => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  tomorrow.setHours(0, 0, 0, 0)
  return `${tomorrow.getFullYear()}/${String(tomorrow.getMonth() + 1).padStart(2, '0')}/${String(tomorrow.getDate()).padStart(2, '0')} 00:00:00`
})()
const sevenDaysAgoMidnightString = (() => {
  const sevenDaysAgo = new Date()
  sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 6)
  sevenDaysAgo.setHours(0, 0, 0, 0)
  return `${sevenDaysAgo.getFullYear()}/${String(sevenDaysAgo.getMonth() + 1).padStart(2, '0')}/${String(sevenDaysAgo.getDate()).padStart(2, '0')} 00:00:00`
})()
const chartDom = ref(null)
var myChart = null
async function getUserLoginLogsData() {
  var obj = {
    pageIndex: 1,
    pageSize: 3000,
    loginTime: sevenDaysAgoMidnightString,
    exitTime: tomorrowMidnightString,
    realName: userStore.realname
  }
  var loginCounts = {}
  var yAxisData = []
  await getuserLoginLogs(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((entry) => {
        const dateObj = new Date(entry.loginTime)
        const dateKey = `${dateObj.getFullYear()}/${dateObj.getMonth() + 1}/${dateObj.getDate()}`
        if (loginCounts[dateKey]) {
          loginCounts[dateKey] += 1
        } else {
          loginCounts[dateKey] = 1
        }
      })

      yAxisData = xAxisData.map((date) => loginCounts[date] || 0)
    }
  })

  myChart = echarts.init(chartDom.value)
  myChart.setOption({
    grid: {
      left: '5%', // 左边距
      right: '5%', // 右边距
      top: '10%', // 上边距
      bottom: '10%', // 下边距
      containLabel: true // 确保坐标轴标签不溢出容器
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: xAxisData
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        label: {
          show: true // 显示数据点标签
        },
        smooth: true,
        data: yAxisData,
        type: 'line',
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color: 'rgba(154,212,254)'
            },
            {
              offset: 1,
              color: 'rgba(138,184,249)'
            }
          ])
        }
      }
    ]
  })
  // 初始化后立即调整图表大小
  myChart.resize()

  //图标自适应
  window.addEventListener('resize', handleResize, { passive: true })
}

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize)
  window.removeEventListener('main-app-i18n-updated', onI18nUpdated)
  if (myChart) myChart.dispose()
})

// 调整图表大小
const handleResize = () => {
  if (myChart) {
    myChart.resize()
  }
}

//获取最近访问
import * as Icons from '@ant-design/icons-vue'
var menuRecords = ref([])
// 存储每个图标的组件引用
const iconComponents = ref({})
async function getSysMenuRecords() {
  await getSysMenuRecord({ userid: userStore.userid, pageIndex: 1, pageSize: 10 }).then((res) => {
    if (res.code == 200 && res.success) {
      menuRecords.value = res.data.slice(0, 8)
      console.log('menuRecords', menuRecords.value)
      res.data.forEach((item) => {
        // 根据 sysIcon 名称在 Icons 对象中查找
        if (Icons[item.menuIcon]) {
          iconComponents.value[item.menuIcon] = Icons[item.menuIcon]
        } else {
          // 如果找不到对应图标，使用默认图标（可以指定一个通用的图标名称）
          iconComponents.value[item.menuIcon] = Icons['QuestionCircleOutlined']
        }
      })
    }
  })
}

//操作日志
const columns = computed(() => [
  {
    title: t('home.Operator'),
    dataIndex: 'operationPerson',
    align: 'center'
  },
  {
    title: t('home.OperatorTime'),
    dataIndex: 'operationTime',
    align: 'center'
  },
  {
    title: t('home.OperationPage'),
    dataIndex: 'belongPage',
    align: 'center'
  },
  {
    title: t('home.TriggerButton'),
    dataIndex: 'buttonName',
    align: 'center'
  }
])
const dataSource = ref([])
function getBtnLogData() {
  dataSource.value = []
  const obj = {
    userid: userStore.userid,
    roleIds: userStore.userRoles,
    pageIndex: 1,
    pageSize: 10
  }
  useGetButtonOperationLogAsync(obj).then((res) => {
    if (res.code == 200 && res.success) {
      res.data.forEach((element) => {
        element.key = element.buttonOperationId
      })
      dataSource.value = res.data
    }
  })
}

//系统消息
var sysMesData = ref([])
async function getSysMessage() {
  var obj = { pageIndex: 1, pageSize: 20, receiveUser: userStore.userid }
  await getAllApi(obj).then((res) => {
    if (res.success && res.code == 200) {
      sysMesData.value = res.data
    }
  })
}
watch(
  () => signalRStore.needRefreshMsg,
  () => {
    getSysMessage()
  }
)
//系统信息
var taskInfoData = ref([])
const taskColumn = computed(() => [
  {
    title: t('home.taskname'),
    dataIndex: 'taskName',
    align: 'center',
    width: '250px'
  },
  {
    title: t('home.ScheduledTaskStartTime'), //home.ScheduledTaskStartTime
    dataIndex: 'startDate',
    align: 'center',
    width: '200px'
  },
  {
    title: t('home.ScheduledTaskExecutionStatus'), //home.ScheduledTaskExecutionStatus
    dataIndex: 'isSuccess',
    align: 'center',
    width: '80px'
  },
  {
    title: t('home.ScheduledCallbackMessage'), //home.ScheduledCallbackMessage
    dataIndex: 'result',
    align: 'center',
    ellipsis: true, // 自动省略，配合 CSS 使用
    width: '200px' // 建议添加宽度限制
  }
])
async function getTaskInfoData() {
  var obj = {
    page: 1,
    size: 4
  }
  await getAllTaskLogApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      taskInfoData.value = []
      res.data.forEach((item) => {
        item.key = item.taskLogId
        taskInfoData.value.push(item)
      })
    }
  })
}

// 跳转官网
function toVite() {
  window.open('https://www.vitejs.net/', '_blank')
}

/* 页面跳转 */
import { useRouter } from 'vue-router'
const router = useRouter()
const routeStore = useRouteStore()
router.addRoute(routeStore.routes)
var navigationStore = useNavigationStore()

const addTabByUnifiedEntry = (tab) => {
  if (!tab || !tab.key) {
    return
  }

  if (typeof window !== 'undefined') {
    window.dispatchEvent(
      new CustomEvent('sub-app-add-tab', {
        detail: { tab }
      })
    )
    return
  }

  if (!Array.isArray(navigationStore.tabs)) {
    return
  }

  const exists = navigationStore.tabs.some((element) => element && element.key === tab.key)
  if (!exists && typeof navigationStore.addTabs === 'function') {
    navigationStore.addTabs(tab)
  }
}

//跳转历史访问页面（区分不同子应用的相同 menuRoute）
function JumpToMenu(item) {
  const tabSysCode = item.sysCode
  const tabKey = tabSysCode ? `/${tabSysCode}${item.menuRoute}` : item.menuRoute

  addTabByUnifiedEntry({
    key: tabKey,
    title: `${item.menuName}`,
    path: `${item.menuRoute}`,
    sysCode: tabSysCode,
    i18nKey: `message.route.XT${item.functionCode}`
  })

  if (tabSysCode) {
    window.location.href = `/${tabSysCode}${item.menuRoute}`
  } else {
    router.push({ path: item.menuRoute })
  }
}
// 以下几个都是 platform 子应用下的固定页面，同样为 tabs 标记 sysCode = 'platform'
function JumpToSysMessage() {
  const sysCode = 'platform'
  const path = '/MsgConfig/MyHistory'
  const tabKey = `/${sysCode}${path}`

  addTabByUnifiedEntry({
    key: tabKey,
    title: '我的历史消息',
    path,
    sysCode,
    i18nKey: 'message.route.XTA0405'
  })
  window.location.href = `/${sysCode}${path}`
}
function msgDetail(id) {
  signalRStore.setCurrentNotification(notification)
  signalRStore.setMsgId(id)
  signalRStore.setIsShow(true)
}
//跳转用户菜单日志
function JumpToMenuLog() {
  const sysCode = 'platform'
  const path = '/SystemStatisticalAnalysis/MenuAccess'
  const tabKey = `/${sysCode}${path}`

  addTabByUnifiedEntry({
    key: tabKey,
    title: '菜单访问日志',
    path,
    sysCode,
    i18nKey: 'message.route.XTA0301'
  })
  window.location.href = `/${sysCode}${path}`
}
//跳转用户操作日志
function JumpToUserBtnLog() {
  const sysCode = 'platform'
  const path = '/SystemStatisticalAnalysis/ButtonOperationLogs'
  const tabKey = `/${sysCode}${path}`

  addTabByUnifiedEntry({
    key: tabKey,
    title: '按钮操作日志',
    path,
    sysCode,
    i18nKey: 'message.route.XTA0305'
  })
  window.location.href = `/${sysCode}${path}`
}
//跳转用户登录日志
function JumpToUserLog() {
  const sysCode = 'platform'
  const path = '/SystemStatisticalAnalysis/UserLoginLogs'
  const tabKey = `/${sysCode}${path}`

  addTabByUnifiedEntry({
    key: tabKey,
    title: '用户登录日志',
    path,
    sysCode,
    i18nKey: 'message.route.XTA0302'
  })
  window.location.href = `/${sysCode}${path}`
}
//跳转任务信息
function JumpToTaskInfo() {
  const sysCode = 'platform'
  const path = '/Task/TaskLog'
  const tabKey = `/${sysCode}${path}`

  addTabByUnifiedEntry({
    key: tabKey,
    title: '任务调度日志',
    path,
    sysCode,
    i18nKey: 'message.route.XTA0503'
  })
  window.location.href = `/${sysCode}${path}`
}

onMounted(() => {
  if (typeof window !== 'undefined') {
    window.addEventListener('main-app-i18n-updated', onI18nUpdated)
  }
  getSysMenuRecords()
  getUserLoginLogsData()
  getBtnLogData()
  getSysMessage()
  getTaskInfoData()
})
const renwudata = ref()
const renwucol = computed(() => [
  {
    title: t('home.TaskName'),
    dataIndex: 'taskName',
    align: 'center',
    width: 70,
    key: 'taskName'
  },
  {
    title: t('home.mainpersonincharge'),
    dataIndex: 'primaryPerson',
    align: 'center',
    width: 70,
    key: 'primaryPerson'
  },
  {
    title: t('home.Taskstatus'),
    dataIndex: 'taskStatus',
    align: 'center',
    width: 70,
    key: 'taskStatus'
  }
])
async function taskDetail() {
  let access_token = userStore.access_token.split('.')
  let statement = JSON.parse(decodeURIComponent(window.atob(access_token[1].replace(/-/g, '+').replace(/_/g, '/'))))
  await getTaskDetailByCode(statement.code).then((res) => {
    if (res.code == 200 && res.success) {
      renwudata.value = res.data
    }
  })
}

taskDetail()
</script>

<style lang="scss">
html,
body {
  height: 100%;
  margin: 0;
}

.homePageStyle {
  display: flex;
  flex-wrap: wrap;
  background-image: url('/src/assets/images/HomePageBG.png');

  .ant-divider-horizontal {
    margin: 10px 0px !important;
  }

  .userBox {
    font-size: 20px;
    height: 100px;
    padding: 20px 0px 0px 25px;
    display: flex;

    .userBoxInfo {
      display: flex;
      flex-direction: column;
      justify-content: center;

      .welcomeTitle {
        font-size: 20px;
        font-weight: 500;
        color: #000;
      }

      .warnInfo {
        margin-top: 10px;
        font-size: 14px;
        font-weight: 400;
        color: #86909c;
      }
    }
  }

  .RecentlyVisited {
    height: 252px;

    .SpaceAround {
      justify-content: space-around;
    }

    .FlexStart {
      justify-content: flex-start;
    }

    .RecentlyVisitedBox {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: 10px;
      width: 290px;
      margin: 5px;
      border-radius: 5px;
      transition: box-shadow 0.3s;
    }

    .RecentlyVisitedBox:hover .VisitedBoxAvatar {
      cursor: pointer;
      background-color: rgba(211, 247, 255, 0.5);
      color: #fff;
    }

    .RecentlyVisitedBox:hover {
      cursor: pointer;
    }

    .VisitedBoxAvatar {
      background-color: rgba(255, 255, 255, 0);
      color: rgb(58 125 233);
      margin-right: 10px;
      transition: background-color 1s ease;
    }
  }

  .MiddleBox {
    margin: 20px 0px;
  }

  .footCopyright {
    z-index: 99;
    position: fixed;
    bottom: 0px;
    background-color: #fff;
    border-radius: 2px;
    width: 100%;
    height: 40px;
    border: 1px solid rgb(225, 225, 231);
    text-align: center;
    line-height: 40px;
  }
}

.chartDomStyle {
  width: 100%;
  min-height: 280px;
  /* 最小高度，防止高度过小 */
  margin-bottom: 10px;
}

.jumpTo {
  position: absolute;
  top: 20px;
  right: 20px;
  height: 30px;
  text-align: center;
  line-height: 30px;
  color: #86909c;
}

.jumpTo:hover {
  cursor: pointer;
  // background-color: rgb(67, 157, 241);
  border: 1px solid lightblue;
  border-radius: 10%;
  height: 30px;
  text-align: center;
  line-height: 30px;
}
</style>
