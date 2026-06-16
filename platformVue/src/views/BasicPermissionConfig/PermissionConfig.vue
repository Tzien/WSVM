<template>
  <div class="pre">
    <div class="role">
      <div class="title">选择角色信息</div>
      <div class="btn">
        <a-input-search
          v-if="btnObj.roleQuery.isShow"
          v-model:value="role_Input"
          style="height: 32px; margin: 0px 8px"
          placeholder="请输入角色名称"
          @search="role_qurety"
        />
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>

      <div class="role_a">
        <a-tree
          v-model:expandedKeys="role_expandedKeys"
          v-model:selectedKeys="role_selectedKeys"
          :tree-data="role_treeData"
          checkStrictly
          default-expand-all
          showIcon
          @select="role_handleSelect"
        >
          <template #title="{ key, title, children }">
            <div @dblclick="dbclick('role', key, children)">
              {{ title }}
            </div>
          </template>
        </a-tree>
      </div>
    </div>
    <div class="sys">
      <div class="title">选择系统服务</div>
      <div class="btn">
        <a-input-search
          v-if="btnObj.sysQuery.isShow"
          v-model:value="sys_Input"
          style="height: 32px; margin: 0px 8px"
          placeholder="请输入系统服务名称"
          @search="sys_qurety"
        />
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>
      <!-- 树的复选框不能只读 v-model:checkedKeys="sys_checkedKeys" checkable-->
      <div class="sys_a">
        <a-tree v-model:expandedKeys="sys_expandedKeys" v-model:selectedKeys="sys_selectedKeys" :tree-data="sys_treeData" @select="sys_handleSelect">
          <template #title="{ key, title, children }">
            <div @dblclick="dbclick('role', key, children)" v-if="sys_checkedKeys.includes(key)" style="color: #1890ff; font-weight: 900">
              {{ title }}
            </div>
            <template v-else
              ><div @dblclick="dbclick('sys', key, children)">
                {{ title }}
              </div></template
            >
          </template>
        </a-tree>
      </div>
    </div>
    <div class="morbori">
      <div class="title">选择权限信息</div>
      <div class="btn">
        <template v-if="!selectAllState">
          <a-button v-if="isVisible" @click="selectAll" style="width: 100px; margin: 0 2px">全选</a-button>
        </template>
        <template v-else>
          <a-button v-if="isVisible" @click="deselectAll" style="width: 100px; margin: 0 2px">取消勾选</a-button>
        </template>
        <a-button
          v-if="isVisible && btnObj.isShowInterface.isShow"
          @click="interfaceClick"
          style="width: 110px; margin: 0 2px"

          :icon="showIcon(btnObj.isShowInterface.Icon)"
        >
          {{ showInterface ? '隐藏接口' : '展示接口' }}</a-button
        >
        <!-- :loading="isLoading" -->
        <a-button
          v-if="isVisible && btnObj.savePermission.isShow"
          :icon="showIcon(btnObj.savePermission.Icon)"
          type="primary"
          :disabled="isDisabled"
          style="width: 110px; margin: 0 2px"
          @click="editRolePermission()"
          >保存权限</a-button
        >
        <div class="tishi" v-if="isVisible && btnObj.savePermission.isShow">(用户权限变更后，需要重新登录！)</div>
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>
      <div class="morbori_a">
        <div class="morbori_tree">
          <a-tree
            v-model:expandedKeys="morbori_expandedKeys"
            v-model:checkedKeys="morbori_checkedKeys"
            checkable
            checkStrictly
            :tree-data="morbori_treeData"
            show-icon
            autoExpandParent:true
            @select="morbori_handleSelect"
            @check="handlecheck"
          >
            <template #title="{ key, title, func, children }">
              <div @dblclick="dbclick('morbori', key, children)" v-if="func === 1" style="color: #1890ff"><FileMarkdownOutlined /> {{ title }}</div>
              <div @dblclick="dbclick('morbori', key, children)" v-else-if="func === 2" style="color: rgb(154, 214, 124)">
                <FormatPainterOutlined /> {{ title }}
              </div>
              <template v-else
                ><div><ApiOutlined /> {{ title }}</div></template
              >
            </template>
          </a-tree>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, h, reactive, watchEffect } from 'vue'
import { PlusOutlined, SearchOutlined, FileMarkdownOutlined, FormatPainterOutlined, ApiOutlined } from '@ant-design/icons-vue'
import { useUserStore } from '@/store/index.js'
import { message } from 'ant-design-vue'
import { useGetRolesAsync, GetPermissionBySysInfoIDAsync, useEditRolePermissionAsync, userGetSysInfoIdByRoleIdAsync } from '@/api/permission'
import { getButtonList } from '@/api/commonFun'
import { useGetAllSysInfo } from '@/api/sysinfo'
import * as Icons from '@ant-design/icons-vue'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
import { useGlobalState } from '../../shared/useGlobalState'
import { useInsertButtonOperationLogAsync } from '@/api/buttonOperationLog.js'

var userStore = ref({})
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
let timer
const showIcon = (iconName) => {
  return Icons[iconName] ? h(Icons[iconName]) : null
}
const btnObj = reactive({
  //角色查询
  roleQuery: { isShow: false, btnCode: 'A020101' },
  //系统查询
  sysQuery: { isShow: false, btnCode: 'A020102' },
  //是否显示接口
  isShowInterface: { isShow: false, btnCode: 'A020103' },
  //保存权限
  savePermission: { isShow: false, btnCode: 'A020104' }
})

function DynamicButtons(menuCode, roleids, btnObj) {
  getButtonList({ menuCode: menuCode, roleids: roleids }).then((data) => {
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
}

DynamicButtons('A0201', userStore.value.userRoles, btnObj)
const role_Input = ref('')
const sys_Input = ref('')
const role_qurety = () => {
  role_treeData.value = []
  sys_treeData.value = []
  morbori_treeData.value = []
  role_expandedKeys.value = []
  role_selectedKeys.value = []
  sys_selectedKeys.value = []
  sys_checkedKeys.value = []
  morbori_checkedKeys.value = []
  roleData(role_Input.value)
}
function selectAllRecursive(checkpre, obj) {
  for (const entity in obj) {
    checkpre.push(obj[entity].key)
    if (obj[entity].children.length > 0) {
      selectAllRecursive(checkpre, obj[entity].children)
    }
  }
}
const selectAll = () => {
  let checkpre = []
  selectAllRecursive(checkpre, morbori_treeData.value)
  morbori_checkedKeys.value = checkpre
  selectAllState.value = true
  checkpermissionlinkage()
}
const deselectAll = () => {
  morbori_checkedKeys.value = []
  selectAllState.value = false
  checkpermissionlinkage()
}
const selectAllState = ref(false)
// const isLoading = ref(false)
//保存按钮是否禁用
const isDisabled = ref(false)
const isVisible = ref(false)
//原权限
let originalPermission = []
const showInterface = ref(true)

const sys_qurety = () => {
  sys_treeData.value = []
  morbori_treeData.value = []
  morbori_checkedKeys.value = []
  sysData(sys_Input.value)
}
//TreeData
const role_treeData = ref([])
const sys_treeData = ref([])
const morbori_treeData = ref([])
const roleStructure = (data) => {
  data.forEach((parent) => {
    const obj = {
      title: parent.name,
      key: parent.id,
      disabled: parent.isReadOnly,
      children: []
    }
    if (parent.children && parent.children.length > 0) {
      parent.children.forEach((cil) => {
        obj.children.push({
          title: cil.name,
          key: cil.id,
          disabled: cil.isReadOnly,
        })
      })
    }
    role_treeData.value.push(obj)
  })
}
const roleData = async (roleName) => {
  const data = await useGetRolesAsync(roleName, true)
  if (data.code === 200 && data.success) {
    roleStructure(data.data)
    sysData()
  } else {
    message.error('角色查询失败!')
  }
}
roleData()
//角色id系统查询
const rolesysData = async (roleid) => {
  const data = await userGetSysInfoIdByRoleIdAsync(roleid)
  if (data.code === 200 && data.success) {
    if (data.data.length == 0) {
      message.warning(data.message)
    }
    data.data.forEach((item) => {
      sys_checkedKeys.value.push(item)
    })
  } else {
    message.error(data.message)
  }
}
//Permission
function convertMenu(menu, readOnly) {
  if (!menu) {
    return null
  }
  const converted = {
    title: menu.functionName,
    key: menu.id,
    func: menu.function,
    disabled: readOnly,
    children: []
  }
  if (menu.menuDtos && menu.menuDtos.length) {
    for (let i = 0; i < menu.menuDtos.length; i++) {
      const child = convertMenu(menu.menuDtos[i], readOnly)
      converted.children.push(child)
    }
  }
  if (menu.buttonDtos && menu.buttonDtos.length) {
    for (let i = 0; i < menu.buttonDtos.length; i++) {
      const child = convertMenu(menu.buttonDtos[i], readOnly)
      converted.children.push(child)
    }
  }
  if (menu.interfaceDtos && menu.interfaceDtos.length) {
    for (let i = 0; i < menu.interfaceDtos.length; i++) {
      const child = convertMenu(menu.interfaceDtos[i], readOnly)
      converted.children.push(child)
    }
  }
  return converted
}
const permissionStructure = (data, isReadOnly) => {
  if (data.length > 0) {
    isVisible.value = true
  }
  data.forEach((element) => {
    morbori_treeData.value.push(convertMenu(element, isReadOnly))
  })
}
const permissionData = async (roleId, sysInfoID, showInterface) => {
  const data = await GetPermissionBySysInfoIDAsync(roleId, sysInfoID, showInterface)
  if (data.code === 200 && data.success) {
    if (data.data.count == 0) {
      message.warning(data.message)
      isVisible.value = false
    }
    morbori_treeData.value = []

    permissionStructure(data.data.menuDtos, data.data.isReadOnly)

    morbori_checkedKeys.value = data.data.selects
    originalPermission = data.data.selects
    if (data.data.selects.length > 0) {
      selectAllState.value = true
    }
    isDisabled.value = true
  } else {
    sys_treeData.value = []
    message.error('权限查询失败!')
  }
}
//SysInfoData
const sysData = async (sysName) => {
  const data = await useGetAllSysInfo(sysName)
  if (data.code === 200 && data.success) {
    sys_treeData.value = data.data
    let onesys = []
    data.data.forEach((element) => {
      onesys.push(element.key)
    })
    sys_expandedKeys.value = onesys
  } else {
    message.error('系统信息查询失败!')
  }
}
const editRolePermission = async () => {
  let checked
  if (morbori_checkedKeys.value.checked == undefined) {
    checked = morbori_checkedKeys.value
  } else {
    checked = morbori_checkedKeys.value.checked
  }

  const submitData = {
    roleid: role_selectedKeys.value[0],
    sysinfoid: sys_selectedKeys.value[0],
    permissions: checked,
    showInterface: showInterface.value ? 1 : 2
  }

  const btnMsg = { buttonOperationId: '',
   belongPage: '权限配置管理', 
   buttonName: '保存权限',
   operationDescription: JSON.stringify(submitData),
   operationType:'保存',
   operationPersonId:userStore.value.userid
  }
  
  const data = await useEditRolePermissionAsync(submitData)
  if (data.code === 200 && data.success) {
    message.success(data.message)
    // isLoading.value = false
    isDisabled.value = true
    await permissionData(role_selectedKeys.value[0], sys_selectedKeys.value[0], showInterface.value)
  } else {
    message.error('权限变更失败!')
    // isLoading.value = false
  }
  await useInsertButtonOperationLogAsync(btnMsg)
}

//role
const role_expandedKeys = ref([])
const role_selectedKeys = ref([])
let role_selectednewKeys
const role_handleSelect = async () => {
  clearTimeout(timer)
  if (role_selectedKeys.value[0]) {
    role_selectednewKeys = role_selectedKeys.value[0]
    timer = setTimeout(async () => {
      roleSelected(role_selectedKeys.value[0])
    }, 300)
  }
}

async function roleSelected(id) {
  selectAllState.value = false
  morbori_treeData.value = []
  sys_checkedKeys.value = []
  sys_selectedKeys.value = []
  role_selectedKeys.value = [id]
  await rolesysData(id)
}

//sys
const sys_expandedKeys = ref([])
const sys_selectedKeys = ref([])
const sys_checkedKeys = ref([])
const sys_handleSelect = () => {
  clearTimeout(timer)
  timer = setTimeout(async () => {
    isVisible.value = false
    morbori_treeData.value = []
    morbori_checkedKeys.value = []
    if (!role_selectedKeys.value[0]) {
      message.success('请勾选角色!')
      sys_selectedKeys.value = []
    } else if (sys_selectedKeys.value[0]) {
      sys_checkedKeys.value = []
      await rolesysData(role_selectedKeys.value[0])
      await permissionData(role_selectedKeys.value[0], sys_selectedKeys.value[0], showInterface.value)
    }
  }, 300)
}
//morbori_treeData
const arraysCompare = (arr1, arr2) => {
  if (arr1.length !== arr2.length) {
    return false
  }
  const sortedArr1 = arr1.slice().sort()
  const sortedArr2 = arr2.slice().sort()
  for (let i = 0; i < sortedArr1.length; i++) {
    if (sortedArr1[i] !== sortedArr2[i]) {
      return false
    }
  }
  return true
}
const morbori_expandedKeys = ref([])
const morbori_checkedKeys = ref([])
const handlecheck = (checkedKeyArry, e) => {
  //linkage(e, 'checked')
  checkchange(e)
  checkpermissionlinkage()
}

function mbiKeyAppend(preprocessData, currentData, keys, linkageType) {
  preprocessData.forEach((element) => {
    if (linkageType === 'children' || (linkageType !== 'children' && currentData.func === 1)) {
      keys.push(element.key)
    }
    if (element.children.length > 0 && linkageType === 'children') {
      mbiKeyAppend(element.children, currentData, keys, linkageType)
    }
  })
}

function upperLevelCancel(checkedArry, data) {
  if (data.parent) {
    const checkedKeys = new Set(checkedArry)
    const peerArr = data.parent.children.map((item) => item.key)
    if (!peerArr.some((item) => checkedKeys.has(item))) {
      checkedArry = checkedArry.filter((p) => p !== data.parent.key)
    }
    checkedArry = upperLevelCancel(checkedArry, data.parent)
  }
  return checkedArry
}

function checkchange(e) {
  let checkedarry = !morbori_checkedKeys.value.checked ? [...morbori_checkedKeys.value] : [...morbori_checkedKeys.value.checked]
  let childrenKeys = []
  mbiKeyAppend(e.node.children, e.node.dataRef, childrenKeys, 'children')
  const checkedKeys = new Set(checkedarry)
  childrenKeys.forEach((item) => {
    if (e.checked && !checkedKeys.has(item)) {
      checkedarry.push(item)
    } else if (!e.checked && checkedKeys.has(item)) {
      checkedarry = checkedarry.filter((p) => p !== item)
    }
  })
  let parentkeys = []
  if (e.node?.parent?.nodes) {
    mbiKeyAppend(e.node.parent.nodes, e.node.dataRef, parentkeys, 'parent')
  }
  if (e.checked) {
    parentkeys.forEach((item) => {
      if (!checkedKeys.has(item)) checkedarry.push(item)
    })
  } else {
    checkedarry = upperLevelCancel(checkedarry, e.node)
  }
  morbori_checkedKeys.value = checkedarry
}

const morbori_handleSelect = (selectedKeys, e) => {
  clearTimeout(timer)
  timer = setTimeout(() => {
    linkage(e, 'selected')
    checkpermissionlinkage()
  }, 300)
}

const linkage = (e, eventName) => {
  let checkedarry = !morbori_checkedKeys.value.checked ? [...morbori_checkedKeys.value] : [...morbori_checkedKeys.value.checked]
  const ikey = e.node.children.filter((item) => item.func === 3).map((item) => item.key)
  const checkedKeys = new Set(checkedarry)
  if (!checkedKeys.has(e.node.key)) {
    if (eventName === 'checked') {
      deleteInterface(ikey, checkedarry)
    } else if (eventName === 'selected') {
      checkedarry.push(e.node.key)
      addInterface(ikey, checkedKeys, checkedarry)
    }
  } else {
    if (eventName === 'checked') {
      addInterface(ikey, checkedKeys, checkedKeys)
    } else if (eventName === 'selected') {
      checkedarry = checkedarry.filter((item) => item !== e.node.key)
      deleteInterface(ikey, checkedarry)
    }
  }
}

function addInterface(key, checkedKeys, checkedarry) {
  key.forEach((item) => {
    if (!checkedKeys.has(item)) {
      checkedarry.push(item)
    }
  })
  morbori_checkedKeys.value = checkedarry
}

function deleteInterface(ikey, checkedarry) {
  morbori_checkedKeys.value = checkedarry.filter((item) => !ikey.includes(item))
}

function checkpermissionlinkage() {
  if (role_selectedKeys.value[0] != undefined && sys_selectedKeys.value[0] != undefined) {
    const filtersys = sys_checkedKeys.value.filter((p) => p == sys_selectedKeys.value[0])
    //加上checkStrictly(取消半选)，morbori_checkedKeys 发生了变动
    const checkedKeys = morbori_checkedKeys.value.checked ?? morbori_checkedKeys.value
    const price = checkedKeys ? checkedKeys.length > 0 : false
    isDisabled.value = arraysCompare(originalPermission, checkedKeys) ? true : false
    //****************系统联动勾选逻辑********************** */
    if (price) {
      if (filtersys.length == 0) {
        sys_checkedKeys.value.push(sys_selectedKeys.value[0])
      }
    } else {
      selectAllState.value = false
      if (filtersys.length > 0) {
        sys_checkedKeys.value = sys_checkedKeys.value.filter((key) => key != sys_selectedKeys.value[0])
      }
    }
  }
}

const interfaceClick = async () => {
  showInterface.value = !showInterface.value
  await permissionData(role_selectedKeys.value[0], sys_selectedKeys.value[0], showInterface.value)
}

const dbclick = (type, key, children) => {
  clearTimeout(timer)
  if (children.length === 0) {
    return
  }
  let expandKeys
  if (type === 'role') {
    expandKeys = role_expandedKeys
    roleSelected(role_selectednewKeys)
  } else if (type === 'sys') {
    expandKeys = sys_expandedKeys
  } else if (type === 'morbori') {
    expandKeys = morbori_expandedKeys
  }

  if (expandKeys.value.some((p) => p === key)) {
    expandKeys.value = expandKeys.value.filter((f) => f !== key)
  } else {
    expandKeys.value.push(key)
  }
}
</script>
<style lang="scss" scoped>
.pre {
  display: flex;
  width: 100%;
  height: 100%;
  justify-content: space-between;
  :where(.css-dev-only-do-not-override-1hsjdkk).ant-tree .ant-tree-node-content-wrapper,
  :where(.css-dev-only-do-not-override-1hsjdkk).ant-tree .ant-tree-checkbox + span {
    font-size: 18px;
  }
}

.role {
  background-color: rgb(255, 255, 255);
  width: 25%;
  height: calc(100% - 10px);
  margin: 5px;
}

.sys {
  background-color: rgb(255, 255, 255);
  width: 25%;
  height: calc(100% - 10px);
  margin: 5px;
}

.morbori {
  background-color: rgb(255, 255, 255);
  width: 50%;
  height: calc(100% - 10px);
  margin: 5px;
}

.pre > .role > .title,
.pre > .sys > .title,
.pre > .morbori > .title {
  font-size: 20px;
  font-weight: 600;
  height: 35px;
  line-height: 35px;
  padding-left: 16px;
}

.role_a,
.sys_a,
.morbori_a {
  height: calc(100% - 120px);
  margin: 10px 0 10px 15px;
  overflow-y: auto;
}

.btn {
  height: 30px;
  display: flex;
  margin: 10px;
}

.tishi{
  font-size: 14px;
  color: rgb(228, 65, 65);
  margin-left: auto;
  align-content: center;
}
.morbori_tree {
  height: 100%;
}
</style>
