<template>
  <div class="all">
    <div class="upper">
      <span style="margin: 0 3px 0 15px">所属页面:<a-input style="margin-left: 3px; width: 200px" v-model:value="belongPageinput" /></span>
      <span style="margin: 0 3px 0 15px">按钮名称:<a-input style="margin-left: 3px; width: 200px" v-model:value="btnnameinput" /></span>
      <a-button v-if="btnObj.query.isShow" @click="queryRecords" style="margin: 0 3px" :icon="showIcon(btnObj.query.Icon)">{{ btnObj.query.name }}</a-button>
      <a-button v-if="btnObj.resetQuery.isShow" @click="resetqueryRecords" style="margin: 0 3px" :icon="showIcon(btnObj.resetQuery.Icon)">{{
        btnObj.resetQuery.name
      }}</a-button>
    </div>
    <a-table :columns="columns" :pagination="false" :data-source="dataSource" :scroll="{ x: 1000, y: 'calc(100vh - 275px)' }" :expand-column-width="100">
      <template #bodyCell="{ column, text, record }">
        <template v-if="column.dataIndex === 'operation'">
          <a-button :icon="h(InsertRowBelowOutlined)" style="display: inline" @click="interfaceDetails(record.buttonOperationId)"></a-button>
        </template>
      </template>
    </a-table>
    <div class="fenye">
      <a-pagination
        v-model:current="pagination.current"
        v-model:page-size="pagination.pageSize"
        :total="pagination.total"
        :show-total="() => `共 ${pagination.total} 条数据`"
        :show-size-changer="pagination.showSizeChanger"
        :show-quick-jumper="pagination.showQuickJumper"
        :page-size-options="pagination.pageSizeOptions"
        @showSizeChange="handlePageSizeChange"
        @change="handlePageChange"
      >
        <template #buildOptionText="props">
          <span>{{ props.value }}条/页</span>
        </template>
      </a-pagination>
    </div>
  </div>
  <a-modal v-model:open="open" title="接口调用信息" :keyboard="false" :maskClosable="false" width="80%" :footer="null">
    <a-divider></a-divider>

    <a-table :columns="itemColumns" :pagination="false" :data-source="itemData" :scroll="{ x: 1000, y: 'calc(50vh)' }" :expand-column-width="100">
      <template #bodyCell="{ column, text, record }">
        <template v-if="column.dataIndex === 'operation'">
          <a-button :icon="h(InsertRowBelowOutlined)" style="display: inline" @click="interfaceDetails(record.buttonOperationId)"></a-button>
        </template>
      </template>
    </a-table>
  </a-modal>
</template>
<script setup>
import { ref, h, reactive, watchEffect, onMounted, defineOptions } from 'vue'
import { message } from 'ant-design-vue'
import { useGetButtonOperationLogAsync, useGetInterfaceOperationLogAsync } from '@/api/buttonOperationLog'
import * as Icons from '@ant-design/icons-vue'
import { getButtonList } from '../../api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
import { InsertRowBelowOutlined } from '@ant-design/icons-vue'
/* 页面缓存 */
defineOptions({
  name: 'A0305'
})
onMounted(() => {
  document.querySelectorAll('.ant-table-body').forEach((element) => {
    element.style.height = element.style.maxHeight
  })
})
var userStore = ref()
const { globalStore } = useGlobalState()
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
    }
  })
}
const showIcon = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}

const btnObj = reactive({
  //查询
  query: { isShow: false, btnCode: 'A060402' },
  //重置查询
  resetQuery: { isShow: false, btnCode: 'A060401' }
})

getButtonList({ menuCode: 'A0604', roleids: userStore.value.userRoles }).then((data) => {
  if (data.code === 200 && data.success) {
    for (const key in btnObj) {
      if (Object.hasOwnProperty.call(btnObj, key)) {
        const s = data.data.buttonDtos.find((p) => p.functionCode == btnObj[key].btnCode)
        if (s) {
          btnObj[key].isShow = true
          btnObj[key].Icon = s.icon
          btnObj[key].name = s.functionName
        }
      }
    }
  } else {
    message.error(data.message)
  }
})

const belongPageinput = ref('')
const btnnameinput = ref('')
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
}
const pagination = ref({
  position: ['bottomRight'],
  current: 1, // 当前页数
  pageSize: 10, // 每页条数
  total: 0, // 数据总数
  showSizeChanger: true, // 是否可以改变每页条数
  showQuickJumper: true, // 是否可以快速跳转至某页
  onChange: handlePageChange, // 页码改变的回调
  onShowSizeChange: handlePageSizeChange, // 每页条数改变的回调
  showTotal: (total) => `共 ${total} 条数据`,
  pageSizeOptions: ['10', '20', '30', '40', '50']
})

const columns = [
  {
    title: '序号',
    dataIndex: 'key',
    align: 'center',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '所属页面',
    align: 'center',
    dataIndex: 'belongPage'
  },
  {
    title: '按钮名称',
    align: 'center',
    dataIndex: 'buttonName'
  },
  {
    title: '按钮类型',
    align: 'center',
    dataIndex: 'operationType'
  },
  // {
  //   title: '按钮操作描述',
  //   align: 'center',
  //   dataIndex: 'operationDescription'
  // },
  {
    title: '操作人',
    align: 'center',
    dataIndex: 'operationPerson'
  },
  {
    title: '操作时间',
    align: 'center',
    dataIndex: 'operationTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  // {
  //   title: '操作',
  //   align: 'center',
  //   dataIndex: 'operation'
  // }
]
const dataSource = ref([])

const queryRecords = () => {
  if (btnnameinput.value.trim().length > 20) {
    message.warn('按钮名称不能超过20个字符')
    return
  }
  useGetButtonOperationLogAsync({
    userid:userStore.value.userid,
    roleIds:userStore.value.userRoles,
    belongPage: belongPageinput.value,
    btnName: btnnameinput.value.trim(),
    pageIndex: pagination.value.current,
    pageSize: pagination.value.pageSize
  }).then((data) => {
    if (data.code === 200 && data.success) {
      dataSource.value = data.data
      pagination.value.total = data.total
    } else {
      message.error(data.message)
    }
  })
}
queryRecords()
const resetqueryRecords = () => {
  belongPageinput.value = ''
  btnnameinput.value = ''
  queryRecords()
}
const open = ref(false)
const itemData = ref(null)
function interfaceDetails(btnid) {
  open.value = true
  useGetInterfaceOperationLogAsync(btnid).then((data) => {
    if (data.code === 200 && data.success) {
      itemData.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
const itemColumns = [
  {
    title: '序号',
    dataIndex: 'key',
    align: 'center',
    width: 80,
    fixed: 'left',
    customRender: (obj) => {
      return obj.index + 1
    }
  },
  {
    title: '系统名称',
    dataIndex: 'sysName',
    key: 'sysName',
    align: 'center',
    width: 150
  },
  {
    title: 'URL',
    dataIndex: 'linkUrl',
    key: 'linkUrl',
    align: 'center'
  },
  {
    title: '请求类型',
    dataIndex: 'action',
    key: 'action',
    align: 'center',
    width: 100
  },
  {
    title: 'http状态码',
    dataIndex: 'httpStatusCode',
    key: 'httpStatusCode',
    align: 'center',
    width: 150
  },
  {
    title: '耗时(ms)',
    dataIndex: 'timeConsum',
    key: 'timeConsum',
    align: 'center',
    width: 100
  },
  {
    title: '异常信息',
    dataIndex: 'exceptionMsg',
    key: 'exceptionMsg',
    align: 'center',
    width: 150
  }
]
</script>
<style scoped lang="scss">
.all {
  position: relative;
  height: calc(100vh - 135px);
  width: 100%;
}

.upper {
  width: 100%;
  height: 40px;
  line-height: 40px;
}

.fenye {
  position: absolute;
  bottom: 5px;
  right: 5px;
  height: 33px;
}
</style>
