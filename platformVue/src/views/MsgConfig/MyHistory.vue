<template>
  <div class="MyHistory">
    <div style="height: 40px; line-height: 40px; text-align: right">
      <div class="headerLeft">
        <p>我的历史消息</p>
      </div>
      <div class="headerRight">
        <a-p>消息内容：</a-p>
        <a-input style="width: 150px;" v-model:value="content" placeholder="消息内容" />
        <a-p>消息类型：</a-p>
        <a-select ref="select" v-model:value="msgType" style="width: 150px;margin-right: 5px;">
          <a-select-option value="1">公告信息通知</a-select-option>
          <a-select-option value="2">微信消息通知</a-select-option>
          <a-select-option value="3">钉钉消息通知</a-select-option>
        </a-select>
        <a-p>是否已读：</a-p>
        <a-select ref="select" v-model:value="isRead" style="width: 150px;margin-right: 5px;">
          <a-select-option value="true">已读取</a-select-option>
          <a-select-option value="false">未读取</a-select-option>
        </a-select>
        <a-button :icon="h(SearchOutlined)" @click="search">查询</a-button>
        <a-button :icon="h(RedoOutlined)" @click="resetSearch">重置查询</a-button>
      </div>
    </div>
    <a-table
          :pagination="false"
          size="large"
          :columns="columns"
          :data-source="data"
          bordered
          ref="myTable"
      :scroll="{ y: 'calc(100vh - 96px - 40px - 55px - 40px - 35px)' }"
          :expand-column-width="100"
        >
          <template #bodyCell="{ text, column, record }">
            <template v-if="column.dataIndex === 'content'">
              <div class="ellipsis">{{ record.content }}</div>
            </template>
            <!-- 操作按钮 -->
            <template v-if="column.dataIndex === 'operation'">
              <a @click="detail(record.msgRecordId)">
                <a-button :icon="h(EditOutlined)" style="display: inline"></a-button>
              </a>
            </template>
            <!-- 处理状态列 -->
            <template v-if="column.dataIndex === 'msgType'">
              <span v-if="text === 1"> 公告信息通知 </span>
              <span v-else-if="text === 2"> 微信消息通知 </span>
              <span v-else-if="text === 3"> 钉钉消息通知 </span>
            </template>
            <template v-if="column.dataIndex === 'isRead'">
              <span v-if="text === true"> 已读 </span>
              <span v-else> 未读 </span>
            </template>
          </template>
        </a-table>
    <div class="paginationStyle">
      <a-pagination
          v-model:current="current"
          v-model:page-size="pageSize"
          :total="total"
          :show-total="(total) => `总条数：${total} `"
          class="pagination"
          show-size-changer
          @showSizeChange="onShowSizeChange"
          @change="pageSizeChange"
        />
    </div>
  </div>

  
  <!-- 模态框 -->

  <MsgDetail v-if="signalRStore.isShow" :id="msgId" @read-success="handleReadSuccess"></MsgDetail>
</template>
<script setup>
import { useSignalRStore } from '@/store/index'
import { ref, onMounted, h, reactive, watch,watchEffect } from 'vue'
import { message } from 'ant-design-vue'
import { getAllApi, getMsgDetailApi } from '@/api/Msg/msg'
import { SearchOutlined, RedoOutlined, PlusOutlined, DeleteOutlined, EditOutlined, TeamOutlined, UploadOutlined, ShrinkOutlined } from '@ant-design/icons-vue'
import MsgDetail from '@/components/MsgDetail.vue'

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
  name: 'A0405'
})
onMounted(() => {
  getAllList(current.value, pageSize.value, content.value, msgType.value, isRead.value)
})
const myTable = ref(null)

const handleReadSuccess = () => {
  console.info('hjhhhhhhhhh')
  getAllList(
    current.value,
    pageSize.value,
    content.value,
    msgType.value,
    isRead.value
  )
}


const content = ref()
const msgType = ref()
const isRead = ref()
async function getAllList(page, size, content, msgType, isRead) {
  const query = {
    pageIndex: page,
    pageSize: size,
    content: content,
    msgType: msgType,
    isRead: isRead,
    receiveUser: signalRStore.ptUserId
  }
  const response = await getAllApi(query)
  if (response.code === 200 && response.success) {
    data.value = response.data
    total.value = response.total
  }
}

//分页
const current = ref(1)
const pageSize = ref(10)
const total = ref(0)
const onShowSizeChange = (current, pageSize) => {
  getAllList(current, pageSize, content.value, msgType.value, isRead.value)
}
const pageSizeChange = (page, pageSize) => {
  getAllList(page, pageSize, content.value, msgType.value, isRead.value)
}
const columns = [
  {
    title: '序号',
    dataIndex: 'no',
    key: 'no',
    fixed: 'left',
    width: 80,
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '消息内容',
    dataIndex: 'content',
    key: 'content',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '消息标题',
    dataIndex: 'title',
    key: 'title',
    fixed: 'left',
    align: 'center'
  },
  {
    title: '消息类型',
    dataIndex: 'msgType',
    key: 'msgType',
    //width: 130,
    align: 'center'
  },
  {
    title: '消息接收人',
    dataIndex: 'receiveUser',
    key: 'receiveUser',
    //width: 150,
    align: 'center'
  },
  {
    title: '发送人',
    dataIndex: 'sendUser',
    key: 'sendUser',
    //width: 80,
    align: 'center'
  },
  {
    title: '消息发送时间',
    dataIndex: 'sendTime',
    key: 'sendTime',
    //width: 180,
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '是否已读',
    dataIndex: 'isRead',
    key: 'isRead',
    //width: 80,
    align: 'center'
  },
  {
    title: '消息读取时间',
    dataIndex: 'msgReadTime',
    key: 'msgReadTime',
    //width: 80,
    align: 'center',
    customRender: (obj) => {
      if (obj.text !== null) {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '操作',
    dataIndex: 'operation',
    width: 180,
    fixed: 'right',
    align: 'center'
  }
]
const data = ref(null)

//查询
function search() {
  console.info(signalRStore.ptUserId)
  getAllList(current.value, pageSize.value, content.value, msgType.value, isRead.value)
}
//重置查询
function resetSearch() {
  content.value = ''
  msgType.value = ''
  isRead.value = ''
  getAllList(current.value, pageSize.value, content.value, msgType.value, isRead.value)
}

//详情
async function detail(id) {
  signalRStore.setMsgId(id)
  signalRStore.setIsShow(true)
}
</script>

<style lang="scss">
.MyHistory {
  height: 100%;
  position: relative;

  .ant-table-container {
    width: 100%;
  }
  .ant-table-body {
    overflow-y: scroll !important;
    height: calc(100vh - 96px - 40px - 40px - 35px);
  }
  .ant-table-content {
    height: calc(100vh - 96px - 40px - 40px - 35px);
  }
  .headerLeft {
    float: left;
    height: 100%;
    display: flex;
    align-items: center;
  }

  .headerLeft > p {
    font-size: 16px;
    margin: 0;
    padding-left: 10px;
    font-weight: 600;
    color: black;
  }

  .headerRight {
    float: right;
    height: 100%;
    display: flex;
    align-items: center;
  }

  .headerRight > input {
    margin-right: 5px;
  }
  .headerRight > button {
    margin:0 5px;
  }

  .paginationStyle {
    height: 40px;
    padding-right: 10px;
    position: absolute;
    right: 0px;
    bottom: 0px;
    width: 100%;
    align-items: center;
    display: flex;
    justify-content: flex-end;
  }

  .ellipsis {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
</style>
