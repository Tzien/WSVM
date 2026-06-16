<template>
  <div class="all">
    <div class="left">
      <div class="title">部门机构管理</div>
      <div class="left_btn">
        <a-input-search v-if="btnObj.userQuery.isShow" v-model:value="user_Input" placeholder="请输入用户名称/用户账号" @search="user_qurety" />
      </div>
      <div class="left_tree">
        <a-tree v-model:expandedKeys="expandedKeys" v-model:selectedKeys="selectedKeys" :tree-data="JG" show-icon @select="handleSelect"  class="custom-tree">
          <template #title="{ title }">
            <span class="tree-node-title">{{ title }}</span>
          </template>
          <template #icon="{ category }">
            <template v-if="category === 1">
              <UngroupOutlined />
            </template>
            <template v-else-if="category === 2">
              <UserOutlined />
            </template>
          </template>
        </a-tree>
      </div>
    </div>
    <div class="right">
      <div class="right_btn">
        <a-button
          v-if="btnObj.addOraganize.isShow"
          size="middle"
          @click="oraganize_add"
          style="margin: 3px 4px 3px 8px"
          :icon="showIcon(btnObj.addOraganize.Icon)"
          type="primary"
          >{{ btnObj.addOraganize.name }}</a-button
        ><a-button v-if="btnObj.addUser.isShow" @click="user_add" style="margin: 3px 4px" :icon="showIcon(btnObj.addUser.Icon)" type="primary">{{
          btnObj.addUser.name
        }}</a-button
        ><a-button
          v-if="btnObj.saveOraganize.isShow && (state.key === '23300' || state.category === 1)"
          @click="oraganize_save"
          style="margin: 3px 4px"
          :icon="showIcon(btnObj.saveOraganize.Icon)"
          >{{ btnObj.saveOraganize.name }}</a-button
        >
        <a-button
          v-if="btnObj.saveUser.isShow && (state.key === '33300' || state.category === 2)"
          @click="user_save"
          style="margin: 3px 4px"
          :icon="showIcon(btnObj.saveUser.Icon)"
          >{{ btnObj.saveUser.name }}</a-button
        >
        <a-button
          v-if="btnObj.password.isShow && state.category === 2 && state.key != '33300'"
          style="margin: 3px 4px"
          :icon="showIcon(btnObj.password.Icon)"
          @click="user_password"
          >{{ btnObj.password.name }}</a-button
        >
        <a-button
          v-if="btnObj.delUser.isShow && state.category === 2 && state.key != '33300'"
          style="margin: 3px 4px"
          type="primary"
          danger
          :icon="showIcon(btnObj.delUser.Icon)"
          @click="user_del"
          >{{ btnObj.delUser.name }}</a-button
        >
        <a-button
          v-if="btnObj.oraganizeType.isShow"
          style="margin: 3px 4px; background-color: rgb(68, 152, 229)"
          :icon="showIcon(btnObj.oraganizeType.Icon)"
          type="primary"
          @click="oraganizetype"
          >{{ btnObj.oraganizeType.name }}</a-button
        >
      </div>
      <div class="right_content">
        <template v-if="state.category === 'empty'">
          <div></div>
        </template>
        <template v-else-if="state.category === 1">
          <SelectOrganization ref="childeRef" :organizationid="state.key"></SelectOrganization>
        </template>
        <template v-else-if="state.category === 2">
          <SelectUser ref="childeUserRef" :userid="state.key"></SelectUser>
        </template>
        <template v-else-if="state.category === 3">
          <SysOraganizeType
            :typeQuery="btnObj.typeQuery"
            :typeReset="btnObj.typeReset"
            :addType="btnObj.addType"
            :uptType="btnObj.uptType"
            :delType="btnObj.delType"
            :showicon="showIcon"
          ></SysOraganizeType>
        </template>
      </div>
    </div>
  </div>
</template>
<script setup>
import { h, ref, provide, reactive, watchEffect, defineOptions, createVNode } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useGetOraganizeUserTreeAsync } from '../../api/BaseInfoConfig/organization'
import { UserOutlined, UngroupOutlined } from '@ant-design/icons-vue'
import { useUserStore } from '../../store/index.js'
import * as Icons from '@ant-design/icons-vue'
import { getButtonList } from '../../api/commonFun'
import SelectOrganization from '@/components/BaseInfoConfig/SelectOrganization.vue'
import SelectUser from '@/components/BaseInfoConfig/SelectUser.vue'
import SysOraganizeType from '@/components/BaseInfoConfig/SysOraganizeType.vue'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'

/* 页面缓存 */
defineOptions({
  name: 'A0101'
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
  //左侧用户名称查询
  userQuery: { isShow: false, btnCode: 'A010101' },
  //添加组织信息
  addOraganize: { isShow: false, btnCode: 'A010102' },
  //保存组织信息
  saveOraganize: { isShow: false, btnCode: 'A010103' },
  //添加用户
  addUser: { isShow: false, btnCode: 'A010104' },
  //保存用户
  saveUser: { isShow: false, btnCode: 'A010105' },
  //初始化密码
  password: { isShow: false, btnCode: 'A010106' },
  //删除用户
  delUser: { isShow: false, btnCode: 'A010107' },
  //配置组织类型
  oraganizeType: { isShow: false, btnCode: 'A010108' },
  //组织类型查询
  typeQuery: { isShow: false, btnCode: 'A010109' },
  //组织类型重置查询
  typeReset: { isShow: false, btnCode: 'A010110' },
  //组织类型添加
  addType: { isShow: false, btnCode: 'A010111' },
  //组织类型修改
  uptType: { isShow: false, btnCode: 'A010112' },
  //组织类型删除
  delType: { isShow: false, btnCode: 'A010113' }
})

getButtonList({ menuCode: 'A0101', roleids: userStore.value.userRoles }).then((data) => {
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
const childeRef = ref()
const childeUserRef = ref()
const user_Input = ref('')
const user_qurety = () => {
  JG.value = []
  oraganizeuser(user_Input.value)
}

const JG = ref([])
function convertTree(data, userName) {
  if (!data) {
    return null
  }
  const converted = {
    key: data.id,
    title: data.name,
    category: 1,
    disabled: data.isReadonly,
    children: []
  }
  if (userName !== '' && userName !== null && userName !== undefined) {
    console.log(data.id)

    expandedKeys.value.push(data.id)
  }
  if (data.userTrees && data.userTrees.length) {
    data.userTrees.forEach((user) => {
      converted.children.push({
        key: user.id,
        title: user.name,
        category: 2
      })
    })
  }
  if (data.oraganizeUserTrees && data.oraganizeUserTrees.length) {
    data.oraganizeUserTrees.forEach((or) => {
      converted.children.push(convertTree(or, userName))
    })
  }
  return converted
}

const oraganizeuser = async (username) => {
  const data = await useGetOraganizeUserTreeAsync(username)
  if (data.code === 200 && data.success) {
    expandedKeys.value = []
    data.data.forEach((or) => {
      JG.value.push(convertTree(or, username))
      if (username === '' || username === null || username === undefined) {
        expandedKeys.value.push(or.id)
      }
    })
  } else {
    message.error('部门树查询失败!')
  }
}
oraganizeuser()

const expandedKeys = ref([])
const selectedKeys = ref(null)

provide('refreshTree', (operate) => {
  JG.value = []
  oraganizeuser(user_Input.value)
  if (operate === 'del') {
    selectedKeys.value = []
  }
})
const oraganize_save = () => {
  childeRef.value.onOraganizeSubmit()
}
const user_save = () => {
  childeUserRef.value.onUserSubmit()
}

const user_password = () => {
  childeUserRef.value.iconnitializePassword()
}

const user_del = () => {
  //childeUserRef.value.deleteuser()
   Modal.confirm({
    title: '删除提醒',
    icon: createVNode(Icons.ExclamationCircleOutlined),
    content: '确认删除该用户吗？',
    okText: '确认删除',
    cancelText: '取消',
    onOk: async () => {
      try {
        await childeUserRef.value.deleteuser()
      } catch (error) {
        console.error('删除失败:', error)
      }
    }
  })
}

const oraganize_add = () => {
  selectedKeys.value = []
  //为监听随便赋的值
  state.value.key = '23300'
  state.value.category = 1
}
const user_add = () => {
  selectedKeys.value = []
  state.value.key = '33300'
  state.value.category = 2
}
const oraganizetype = () => {
  selectedKeys.value = []
  state.value.key = '43300'
  state.value.category = 3
}

//切换
const state = ref({ key: '0', category: 'empty' })
const handleSelect = (selectedKeys, e) => {
  if (e.selected) {
    state.value.key = e.node.key
    state.value.category = e.node.category
  } else {
    state.value.key = '0'
    state.value.category = 'empty'
  }
}
</script>
<style scoped lang="scss">
.all {
  width: 100%;
  height: 100%;
  display: flex;
  .left {
    width: 18%;
    background-color: rgb(255, 255, 255);
    height: 100%;
    .title {
      width: 100%;
      font-size: 20px;
      font-weight: 600;
      height: 35px;
      line-height: 35px;
      padding-left: 16px;
    }
    .left_btn {
      width: 100%;
      height: 30px;
      padding: 10px 10px;
    }
    .left_tree {
      width: 100%;
      height: calc(100% - 90px);
      margin: 25px 0 0 0;
      padding: 0 8px;
      overflow-y: auto;
    }
  }
}

.right {
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  width: 82%;
  height: 100%;
}
.right_btn {
  display: flex;
  padding-top: 5px;
  padding-bottom: 5px;
  background-color: rgb(255, 255, 255);
}
.right_content {
  display: flex;
  flex-direction: row;
  width: 100%;
  height: calc(100% - 48px);
}
</style>
