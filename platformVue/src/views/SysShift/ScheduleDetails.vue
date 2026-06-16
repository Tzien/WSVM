<template>
  <div class="all">
    <div class="upper">
      <a-span class="span">分厂/工序: </a-span
      ><a-select
        v-model:value="factorySelectId"
        show-search
        allowClear
        placeholder="请选择分厂/工序"
        style="width: 170px; margin: 5px 5px"
        :options="factorySelectData"
      ></a-select>

      <a-span class="span">班组:</a-span
      ><a-select
        v-model:value="ShiftClasstId"
        show-search
        allowClear
        placeholder="请选择班组"
        style="width: 170px; margin: 5px 5px"
        :options="ShiftClassData"
      ></a-select>
      <a-span class="span">班次:</a-span
      ><a-select
        v-model:value="ShiftSelectId"
        show-search
        allowClear
        placeholder="请选择班次"
        style="width: 170px; margin: 5px 10px"
        :options="ShiftSelectData"
        :filter-option="false"
        @search="handleSearch"
        @change="handleChange"
      ></a-select>

      <a-span class="span">统计时间:</a-span>
      <a-date-picker
        v-model:value="startShowtime"
        show-time
        placeholder="起始于"
        @change="onChangeBegin"
        style="margin: 5px 0 5px 5px; width: 180px; height: 32px"
      />
      <a-date-picker
        @change="onChangeEnd"
        v-model:value="endShowtime"
        placeholder="终止于"
        show-time
        style="margin: 5px 0 5px 5px; width: 180px; height: 32px"
      />

      <a-button v-if="btnObj.query.isShow" @click="queryDetail" style="margin: 5px 2px" :icon="showIcon(btnObj.query.Icon)">{{ btnObj.query.name }}</a-button>
      <a-button v-if="btnObj.resetQuery.isShow" @click="resetDetail" style="margin: 5px 2px" :icon="showIcon(btnObj.resetQuery.Icon)">{{
        btnObj.resetQuery.name
      }}</a-button>
      <a-button v-if="btnObj.add.isShow" @click="showModal" style="margin: 5px 2px" type="primary" :icon="showIcon(btnObj.add.Icon)">{{
        btnObj.add.name
      }}</a-button>
    </div>
    <a-table :columns="columns" :pagination="false" :data-source="dataSource" :scroll="{ x: 2000, y: 'calc(100vh - 275px)' }" :expand-column-width="100">
      <template #expandedRowRender="{ record }">
        <div style="display: inline-block; margin: 0 0 0 80px; font-weight: 700">人员姓名:</div>
        <div style="display: inline-block; margin-left: 10px">{{ record.userNames }}</div>
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
      </a-pagination>
    </div>
    <a-modal v-model:open="open" :width="300" title="请选择日期" @ok="handleOk" @cancel="handleCancel" ok-text="确认" cancel-text="取消">
      <a-divider style="height: 1px" />
      <a-date-picker @change="onChangeScheduling" v-model:value="schedulingShowtime" placeholder="排班时间" style="width: 250px; margin: 0 2px" />
    </a-modal>
  </div>
</template>
<script setup>
import { ref, reactive, h, watchEffect, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import { PlusOutlined, SaveOutlined, SearchOutlined, EditOutlined, CloseOutlined, DeleteOutlined, ExclamationCircleOutlined } from '@ant-design/icons-vue'
import { useGetFactorySelect } from '@/api/BaseInfoConfig/factoryinfo'
import { useInsertSysShiftDetailAsync, useGetSysShiftDetailAsync, getGetShiftClassSelect, getGetShiftSelect } from '@/api/SysShift'
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
  query: { isShow: false, btnCode: 'A060303' },
  //重置查询
  resetQuery: { isShow: false, btnCode: 'A060301' },
  //生成排班
  add: { isShow: false, btnCode: 'A060302' }
})

getButtonList({ menuCode: 'A0603', roleids: userStore.value.userRoles }).then((data) => {
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

const factorySelectData = ref([])
const factorySelectId = ref(null)
const ShiftClassData = ref([])
const ShiftClasstId = ref(null)
const ShiftSelectData = ref([])
const ShiftSelectId = ref(null)
const starttime = ref(null)
const startShowtime = ref(null)
const endtime = ref(null)
const endShowtime = ref(null)
const schedulingtime = ref(null)
const schedulingShowtime = ref(null)
const onChangeBegin = (value, dateString) => {
  starttime.value = dateString
}
const onChangeEnd = (value, dateString) => {
  endtime.value = dateString
}
const onChangeScheduling = (value, dateString) => {
  schedulingtime.value = dateString
}
const resetDetail = () => {
  startShowtime.value = null
  endShowtime.value = null
  factorySelectId.value = null
  ShiftClasstId.value = null
  ShiftSelectId.value = null
  starttime.value = null
  endtime.value = null
  queryDetail()
}

const handlePageChange = (newPage) => {
  pagination.value.current = newPage
  queryDetail()
}
const handlePageSizeChange = (newPageSize, currentPage) => {
  pagination.value.current = newPageSize
  pagination.value.pageSize = currentPage
  queryDetail()
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
      return (pagination.value.current - 1) * pagination.value.pageSize + obj.index + 1
    }
  },
  {
    title: '分厂/工序',
    align: 'center',
    dataIndex: 'factoryName'
  },
  {
    title: '班次方案',
    align: 'center',
    dataIndex: 'programName'
  },
  {
    title: '班组',
    align: 'center',
    dataIndex: 'className'
  },
  {
    title: '班次',
    align: 'center',
    dataIndex: 'shiftName'
  },
  {
    title: '开始时间',
    align: 'center',
    dataIndex: 'startTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '结束时间',
    align: 'center',
    dataIndex: 'endTime',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.replace('T', ' ')
      }
    }
  },
  {
    title: '统计日期',
    align: 'center',
    dataIndex: 'totalDate',
    customRender: (obj) => {
      if (obj.text !== null && obj.text !== undefined && obj.text.trim() !== '') {
        return obj.text.split('T')[0]
      }
      return ''
    }
  },
  {
    title: '操作时间',
    align: 'center',
    dataIndex: 'modifyTime'
  },
  {
    title: '操作人',
    align: 'center',
    dataIndex: 'modifyName'
  }
]
const dataSource = ref([])

useGetFactorySelect().then((data) => {
  if (data.code === 200 && data.success) {
    factorySelectData.value = data.data
  } else {
    message.error(data.message)
  }
})
getGetShiftClassSelect().then((data) => {
  if (data.code === 200 && data.success) {
    ShiftClassData.value = data.data
  } else {
    message.error(data.message)
  }
})
getGetShiftSelect().then((data) => {
  if (data.code === 200 && data.success) {
    ShiftSelectData.value = data.data
  } else {
    message.error(data.message)
  }
})
const queryDetail = () => {
  useGetSysShiftDetailAsync({
    factoryId: factorySelectId.value,
    classId: ShiftClasstId.value,
    shiftId: ShiftSelectId.value,
    startTime: starttime.value,
    endTime: endtime.value,
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
queryDetail()

const open = ref(false)
const showModal = () => {
  open.value = true
}
const handleOk = (e) => {
  if (schedulingtime.value == null) {
    message.error('请选择生成排班时间')
    return
  }
  useInsertSysShiftDetailAsync(new Date(schedulingtime.value + 'Z').toISOString()).then((data) => {
    if (data.code === 200 && data.success) {
      message.success(data.message)
      queryDetail()
    } else {
      message.error(data.message)
    }
  })
  schedulingtime.value = null
  schedulingShowtime.value = null
  open.value = false
}
const handleCancel = (e) => {
  schedulingtime.value = null
  schedulingShowtime.value = null
  open.value = false
}
</script>
<style scoped lang="scss">
.all {
  height: 100%;
  width: 100%;
  position: relative;
}

.upper {
  width: 100%;

  display: flex;
  flex-wrap: wrap;
}

.span {
  height: 30px;
  line-height: 30px;
  margin: 5px 5px;
}

.fenye {
  position: absolute;
  bottom: 5px;
  right: 5px;
  height: 33px;
}
</style>
