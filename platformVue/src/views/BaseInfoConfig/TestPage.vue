<template>
  <div style="border: 1px solid red; height: calc(100% - 36px)">
    <CustomButtonList
      :ParamsRoleId="inputRoleId"
      :ParamsFunctionName="inputFunctionName"
      :BtnFunctionNameArray="['test01', 'test02', 'test03', 'test04']"
      :IsOnlyIcon="true"
      @buttonClick="handleButtonClick"
    ></CustomButtonList>
    <CustomButtonList
      :ParamsRoleId="inputRoleId"
      :ParamsFunctionName="inputFunctionName"
      :BtnFunctionNameArray="['test05']"
      @buttonClick="handleButtonClick"
      v-if="enabledBtn"
    ></CustomButtonList>

    <!-- ParamsRoleId 传入的角色Id集合  -->
    <!-- ParamsFunctionName  菜单编码 -->
    <!-- BtnFunctionNameArray 按钮编码，若为[]则查询菜单编码下全部按钮，若存在值则只查询编码值的按钮 -->
    <!-- IsOnlyIcon 是否只展示图标 -->

    <s-table :columns="columns" :scroll="{ y: 400 }" :pagination="false" :data-source="dataSource"></s-table>
  </div>
</template>

<script setup>
import { ref, onMounted, watchEffect } from 'vue'
import CustomButtonList from '@/components/commonComponents/CustomButtonList.vue'
import { useUserStore } from '@/store/user'
const inputFunctionName = ref('test')

import { useGlobalState } from '../../shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
const { globalStore } = useGlobalState()
var userStore = ref({})
var drawerStore = ref({})
var inputRoleId = []
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
  drawerStore = useDrawerStore()
  inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      drawerStore.value = globalStore.value.drawerStore
      inputRoleId = userStore.value.userRoles
    }
  })
}

const enabledBtn = ref(false)
onMounted(() => {})

function addEvent(isClose) {
  console.log(`addEvent(${isClose})`)
}
function delEvent() {
  console.log('delEvent')
}
function updateEvent() {
  console.log('updateEvent')
}
function searchEvent() {
  console.log('searchEvent')
}
function customEvent() {
  console.log('customEvent')
}

const handleButtonClick = (functionName) => {
  // 在这里处理按钮点击事件
  console.log('Button clicked:', functionName)
  // 你可以在这里调用相应的方法
  if (functionName === 'addEvent(true)') {
    addEvent(true)
  }
  if (functionName === 'delEvent') {
    delEvent()
  }
  if (functionName === 'updateEvent') {
    updateEvent()
  }
  if (functionName === 'searchEvent') {
    searchEvent()
  }
  if (functionName === 'customEvent') {
    customEvent()
  }
  if (functionName === 'searchModalData') {
    enabledBtn.value = true
  }
  if (functionName === 'customEvent') {
    customEvent()
  }
}

const dataSource = [
  {
    key: '1',
    name: '胡彦斌',
    age: 32,
    address: '西湖区湖底公园1号'
  },
  {
    key: '2',
    name: '胡彦祖',
    age: 42,
    address: '西湖区湖底公园1号'
  }
]

const columns = [
  {
    title: '姓名',
    dataIndex: 'name',
    key: 'name'
  },
  {
    title: '年龄',
    dataIndex: 'age',
    key: 'age'
  },
  {
    title: '住址',
    dataIndex: 'address',
    key: 'address'
  }
]
</script>
<style lang="less"></style>
