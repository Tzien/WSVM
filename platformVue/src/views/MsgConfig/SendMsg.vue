<template>
  <div class="sendmsg">
    <a-layout style="height: 100%; width: 100%">
      <a-layout-content class="contentStyle">
        <div class="top">
          <div class="left">
            <a-form ref="formRef" :model="formState" v-bind="layout">
              <a-form-item label="消息类型">
                <a-select ref="select" v-model:value="formState.msgType">
                  <a-select-option value="1">公告信息通知</a-select-option>
                  <a-select-option value="2">微信消息通知</a-select-option>
                  <a-select-option value="3">钉钉消息通知</a-select-option>
                </a-select>
              </a-form-item>
              <a-form-item label="消息标题">
                <a-input v-model:value="formState.title" />
              </a-form-item>
              <a-form-item label="消息内容">
                <a-textarea v-model:value="formState.content" placeholder="请输入消息内容…" :rows="20" :autoSize="{ minRows: 20, maxRows: 20 }" />
              </a-form-item>
            </a-form>
          </div>
          <div class="right">
            <a-button :icon="h(PlusOutlined)" @click="addUser">添加人员</a-button>
            <a-button :icon="h(MinusOutlined)" @click="removeUser">移除人员</a-button>
            <a-table
              :pagination="false"
              size="large"
              :columns="columns"
              :data-source="data"
              :scroll="{ y: 470 }"
              bordered
              :row-selection="{
                selectedRowKeys: selectRowKeys2,
                onChange: onChange
              }"
            >
            </a-table>
          </div>
        </div>
        <div class="bottom">
          <a-button :icon="h(NotificationOutlined)" class="btn" size="large" @click="sendMsg">发送消息</a-button>
        </div>
      </a-layout-content>
    </a-layout>

    <!-- 添加用户 -->
    <a-modal v-model:open="open" title="添加部门人员" centered :keyboard="false" :maskClosable="false" width="70%" :footer="null">
      <div style="display: flex; height: 600px; border-top: 1px solid gainsboro">
        <div style="width: 20%; border-right: 1px solid gainsboro; padding-top: 10px">
          <a-p>部门机构信息</a-p>
          <a-tree @select="zuzhiSelect" :tree-data="treeData" style="width: 200px"> </a-tree>
        </div>
        <div style="width: 80%">
          <a-layout style="height: 100%; overflow: hidden">
            <a-layout>
              <a-layout-header class="header" style="background-color: white; padding: 0px; height: 10%">
                <div class="headerRight">
                  <a-button :icon="h(CheckOutlined)" @click="selectAndClose" style="margin: 10px">选择并关闭</a-button>
                  <a-p>用户名称：</a-p>
                  <a-input style="width: 200px; margin-right: 10px" v-model:value="username" placeholder="请输入用户名称" />
                  <a-p>用户账号：</a-p>
                  <a-input style="width: 200px; margin-right: 10px" v-model:value="account" placeholder="请输入用户账号" />
                  <a-button :icon="h(SearchOutlined)" @click="searchUser" style="margin-right: 10px">查询</a-button>
                  <a-button :icon="h(RedoOutlined)" @click="resetSearchUser">重置查询</a-button>
                </div>
              </a-layout-header>
              <a-layout-content class="content" style="background-color: white; padding: 0px; height: 75%">
                <div style="height: 100%; border-bottom: 1px solid gainsboro">
                  <a-table
                    :pagination="false"
                    size="large"
                    :columns="itemColumns"
                    :data-source="itemData"
                    bordered
                    :scroll="{ y: 423 }"
                    :row-selection="{
                      selectedRowKeys: selectRowKeys,
                      onSelect: onSelectChange,
                      onSelectAll: onSelectAllChange
                    }"
                  >
                    <template #bodyCell="{ text, column, record }">
                      <template v-if="column.dataIndex === 'sex'">
                        <span v-if="text === 0"> 男 </span>
                        <span v-else> 女 </span>
                      </template>
                      <template v-if="column.dataIndex === 'enabled'">
                        <span v-if="text === 1"> 正常 </span>
                        <span v-else> 冻结 </span>
                      </template>
                      <template v-if="column.dataIndex === 'weChatUserId'">
                        <span v-if="text === null"> 未绑定 </span>
                        <span v-else> 已绑定 </span>
                      </template>
                      <template v-if="column.dataIndex === 'dingTalkUserId'">
                        <span v-if="text === null"> 未绑定 </span>
                        <span v-else> 已绑定 </span>
                      </template>
                    </template>
                  </a-table>
                </div>
              </a-layout-content>
              <a-layout-footer class="foot" style="background-color: white; padding: 0px; height: 10%">
                <div style="float: right; height: 100%; display: flex; align-items: center">
                  <a-pagination
                    v-model:current="currentItem"
                    v-model:page-size="pageSizeItem"
                    :total="totalItem"
                    :show-total="(totalItem) => `总条数：${totalItem} `"
                    show-size-changer
                    @showSizeChange="onShowSizeChangeItem"
                    @change="pageSizeChangeItem"
                  />
                </div>
              </a-layout-footer>
            </a-layout>
          </a-layout>
        </div>
      </div>
    </a-modal>

    <!-- 消息弹框 -->
    <a-modal v-model:open="modal2Visible" title="消息通知" centered :footer="null" :closable="false">
      <p>标题{{ msgTitle }}</p>
      <p>{{ msgContent }}</p>
      <a-button type="primary" @click="readMsg" style="margin-right: 50px">收到</a-button>
    </a-modal>
  </div>
</template>
<script setup>
import { message } from 'ant-design-vue'
import { ref, onMounted, h, reactive, watch, watchEffect } from 'vue'
import { getOraganizeTreeApi, GetAllUserBySysOraganizeIdsApi, sendMsgApi, getNoReadMsgApi, readMsgApi } from '@/api/Msg/msg'
import { NotificationOutlined, PlusOutlined, MinusOutlined, CheckOutlined, SearchOutlined, RedoOutlined } from '@ant-design/icons-vue'
import { useUserStore, useRouteStore, useNavigationStore, useSignalRStore } from '@/store/index'

import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'

const { globalStore } = useGlobalState()
const signalRStore = useSignalRStore()
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  //   userStore = useUserStore()
  //   drawerStore = useDrawerStore()
  //   inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      signalRStore.value = globalStore.value.signalRStore
    }
  })
}
/* 页面缓存 */
defineOptions({
  name: 'A0401'
})
const formRef = ref()
const formState = reactive({
  msgType: '',
  title: '',
  content: '',
  userIds: []
})
const layout = {
  labelCol: {
    span: 2
  },
  wrapperCol: {
    span: 21
  }
}

const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    width: 100,
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '用户',
    dataIndex: 'realName',
    key: 'realName',
    align: 'center'
  }
]
const data = ref([])
const changeData = ref([])

//打开添加用户模态框
const totalItem = ref(0)
const open = ref(false)
const addUser = async () => {
  open.value = true
  // zuzhiId.value = null;
  // itemData.value = [];
  // totalItem.value = 0;
  getOraganizeTree()
}

const username = ref()
const account = ref()
const zuzhiId = ref()
async function zuzhiSelect(selectedKeys) {
  zuzhiId.value = selectedKeys[0]
  getUserList(zuzhiId.value, username.value, account.value, 1, 10)
}

async function getUserList(oraganizeId, name, account, page, size) {
  const query = {
    oraganizeId: oraganizeId,
    name: name,
    account: account,
    page: page,
    size: size
  }
  const response = await GetAllUserBySysOraganizeIdsApi(query)
  if (response.code === 200 && response.success) {
    itemData.value = response.data
    itemData.value.forEach((x) => {
      x.key = x.id
    })
    totalItem.value = response.total
  }
}

//获取部门机构树
const treeData = ref([])
async function getOraganizeTree() {
  const response = await getOraganizeTreeApi()
  if (response.code === 200 && response.success) {
    treeData.value = []
    response.data.forEach((or) => {
      treeData.value.push(convertTree(or))
    })
  }
}
function convertTree(data) {
  if (!data) {
    return null
  }
  const converted = {
    title: data.name,
    key: data.id,
    children: []
  }
  if (data.oraganizeTrees && data.oraganizeTrees.length) {
    data.oraganizeTrees.forEach((or) => {
      converted.children.push(convertTree(or))
    })
  }
  return converted
}

const itemColumns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    width: 80,
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '用户名称',
    dataIndex: 'realName',
    key: 'realName',
    align: 'center'
  },
  {
    title: '用户账号',
    dataIndex: 'account',
    key: 'account',
    align: 'center'
  },
  {
    title: '用户性别',
    dataIndex: 'sex',
    key: 'sex',
    align: 'center'
  },
  {
    title: '用户状态',
    dataIndex: 'enabled',
    key: 'enabled',
    align: 'center'
  },
  {
    title: '直属部门',
    dataIndex: 'organizeName',
    key: 'organizeName',
    align: 'center'
  },
  {
    title: '是否绑定微信',
    dataIndex: 'weChatUserId',
    key: 'weChatUserId',
    align: 'center'
  },
  {
    title: '是否绑定钉钉',
    dataIndex: 'dingTalkUserId',
    key: 'dingTalkUserId',
    align: 'center'
  }
]
const itemData = ref(null)
//查询
function searchUser() {
  // selectRows.value=[];
  // selectRowKeys.value=[];
  getUserList(zuzhiId.value, username.value, account.value, currentItem.value, pageSizeItem.value)
}
//重置
function resetSearchUser() {
  // selectRows.value=[];
  // selectRowKeys.value=[];
  username.value = ''
  account.value = ''
  getUserList(zuzhiId.value, username.value, account.value, currentItem.value, pageSizeItem.value)
}
//用户分页
const currentItem = ref(1)
const pageSizeItem = ref(10)
const onShowSizeChangeItem = (currentItem, pageSizeItem) => {
  getUserList(zuzhiId.value, username.value, account.value, currentItem, pageSizeItem)
}
const pageSizeChangeItem = (page, pageSizeItem) => {
  getUserList(zuzhiId.value, username.value, account.value, page, pageSizeItem)
}

//弹框选择人员
const selectRows = ref([])
const selectRowKeys = ref([])

const onSelectChange = (record, selected) => {
  if (selected) {
    if (!selectRowKeys.value.some((r) => r === record.key)) {
      selectRows.value.push(record)
      selectRowKeys.value.push(record.key)
    }
  } else {
    if (selectRowKeys.value.some((r) => r === record.key)) {
      selectRows.value = selectRows.value.filter((item) => item.key !== record.key)
      selectRowKeys.value.splice(selectRowKeys.value.indexOf(record.key), 1)
    }
  }
}

const onSelectAllChange = (selected, selectedRows, changeRows) => {
  if (selected) {
    changeRows.forEach((item) => {
      if (!selectRowKeys.value.some((r) => r === item.key)) {
        selectRows.value.push(item)
        selectRowKeys.value.push(item.key)
      }
    })
  } else {
    changeRows.forEach((item) => {
      if (selectRowKeys.value.some((r) => r === item.key)) {
        selectRows.value = selectRows.value.filter((ele) => ele.key !== item.key)
        selectRowKeys.value.splice(selectRowKeys.value.indexOf(item.key), 1)
      }
    })
  }
}

const selectAndClose = () => {
  open.value = false
  data.value = JSON.parse(JSON.stringify(selectRows.value))
}

//外边选择人员

const selectRowKeys2 = ref([])
const onChange = (selectedRowKeys, selectedRows) => {
  selectRowKeys2.value = selectedRowKeys
}

//移除人员
const removeUser = () => {
  selectRowKeys2.value.forEach((element) => {
    data.value = data.value.filter((ele) => ele.key !== element)
    selectRows.value = selectRows.value.filter((ele) => ele.key !== element)
    selectRowKeys.value.splice(selectRowKeys.value.indexOf(element), 1)
  })
}

const modal2Visible = ref(false)
const msgTitle = ref()
const msgContent = ref()
const num = ref(0)
//发送消息
const sendMsg = async () => {
  formState.userIds = data.value.map((x) => x.key)
  const response = await sendMsgApi(formState)
  if (response.code === 200 && response.success) {
    // if (formState.msgType == 1) {
    //   var idsUserId = response.message.split(',')
    //   num.value = 0
    //   signalRStore.connection.invoke('AddMsg', idsUserId)
    // }
    message.success('发送成功')
  }
}
</script>

<style lang="scss">
.sendmsg {
  margin: 0px;
  padding: 0px;
  height: 100%;
  width: 100%;

  .contentStyle {
    background-color: white;
    margin: 0px;
    padding: 0px;
    height: 100%;
    width: 100%;
  }

  .top {
    width: 100%;
    height: 80%;
    display: flex;
  }

  .left {
    width: 68%;
    height: 100%;
    padding: 0;
    padding-top: 20px;
    /* border: 1px solid blue; */
  }

  .right {
    width: 30%;
    height: 100%;
    padding: 0;
    padding-top: 20px;
    padding-left: 10px;
    /* border: 1px solid red; */
  }

  .right > button {
    margin-right: 10px;
    margin-bottom: 5px;
  }

  .bottom {
    width: 100%;
    height: 15%;
    padding: 0;
    /* border: 1px solid yellow; */
    text-align: center;
  }

  .ant-table-body {
    height: 470px;
    border-bottom: 1px solid gainsboro;
  }
}
</style>
