<template>
  <div class="all">
    <div class="upper">
      <a-span style="margin: 0 3px 0 15px">分厂/工序: <a-input style="margin-left: 3px; width: 200px" v-model:value="factoryinput" /></a-span>
      <a-span style="margin: 0 3px 0 15px">排版方案:<a-input style="margin-left: 3px; width: 200px" v-model:value="programinput" /></a-span>
      <a-span style="margin: 0 3px 0 15px">换班班组:<a-input style="margin-left: 3px; width: 200px" v-model:value="classinput" /></a-span>
      <a-button v-if="btnObj.query.isShow" @click="queryRecords" style="margin: 0 3px" :icon="showIcon(btnObj.query.Icon)">{{ btnObj.query.name }}</a-button>
      <a-button v-if="btnObj.resetQuery.isShow" @click="resetqueryRecords" style="margin: 0 3px" :icon="showIcon(btnObj.resetQuery.Icon)">{{
        btnObj.resetQuery.name
      }}</a-button>
    </div>
    <a-table :columns="columns" :pagination="false" :data-source="dataSource" :scroll="{ x: 2100, y: 'calc(100vh - 275px)' }" :expand-column-width="100">
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
      </a-pagination>
    </div>
  </div>
</template>
<script setup>
import { ref, h, reactive, watchEffect, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { useGetSysShiftDetailUserLogAsync } from '@/api/SysShift'
import * as Icons from '@ant-design/icons-vue'
import { getButtonList } from '../../api/commonFun'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
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

const factoryinput = ref('')
const programinput = ref('')
const classinput = ref('')
const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  queryRecords()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  queryRecords()
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
    customRender: (obj) => {
      return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
    }
  },
  {
    title: '分厂/工序',
    dataIndex: 'factoryName'
  },
  {
    title: '排版方案',
    dataIndex: 'programName'
  },
  {
    title: '原班组',
    dataIndex: 'oldClassName'
  },
  {
    title: '换班班组',
    dataIndex: 'newClassName'
  },
  {
    title: '原班次',
    dataIndex: 'oldShiftName'
  },
  {
    title: '换班班次',
    dataIndex: 'newShiftName'
  },
  {
    title: '原统计日期',
    dataIndex: 'oldTotalDate',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.split('T')[0]
      }
      return ''
    }
  },
  {
    title: '换班统计日期',
    dataIndex: 'newTotalDate',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.split('T')[0]
      }
      return ''
    }
  },
  {
    title: '原班组人员',
    dataIndex: 'oldUserName'
  },
  {
    title: '换班人员',
    dataIndex: 'newUserName'
  },
  {
    title: '操作人',
    dataIndex: 'modifyName'
  },
  {
    title: '操作时间',
    dataIndex: 'modifyTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  }
]
const dataSource = ref([])

const queryRecords = () => {
  useGetSysShiftDetailUserLogAsync({
    factoryName: factoryinput.value,
    programName: programinput.value,
    className: classinput.value,
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
  factoryinput.value = ''
  programinput.value = ''
  classinput.value = ''
  queryRecords()
}
</script>
<style scoped lang="scss">
.all {
  position: relative;
  height: 100%;
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
