<template>
  <div class="all">
    <div class="role">
      <div class="title">选择角色信息</div>
      <div class="btn">
        <a-input-search v-model:value="role_Input" style="height: 32px; margin: 0px 8px" placeholder="请输入角色名称"
          @search="role_qurety" />
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>

      <div >
        <a-tree v-model:selectedKeys="selectedRoleKeys"
          :tree-data="role_treeData" checkStrictly default-expand-all showIcon :height="450">
          <template #title="{ title, key }">
            <span v-if="key === '0'" style="color: #1890ff">{{ title }}</span>
            <template v-else>{{ title }}</template>
          </template>
        </a-tree>
      </div>
    </div>
    <div class="factory">
      <div class="title">选择工厂树</div>
      <div class="btn">
        <a-button @click="saveFactory">保存工厂树权限</a-button>
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>

      <div>
        <a-tree    v-model:checkedKeys="checkedFactoryKeys"
          checkStrictly checkable :tree-data="factoryTreeData">
        </a-tree>
      </div>
    </div>
    <div class="media">
      <div class="title">选择介质</div>
      <div class="btn">
        <a-button  @click="saveMedia">保存介质权限</a-button>
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>

      <div>
        <a-tree  :autoExpandParent=true
          v-model:checkedKeys="checkedMediaKeys" checkStrictly checkable :tree-data="mediaTreeData">
        </a-tree>
      </div>
    </div>
  </div>
</template>
<script setup>
import {
  getFactoryTreeDataApi,getMediaTreeDataApi,insertSysRoleGroupAsyncApi,insertSysMediaAsyncApi
  ,selectedGroupAndMediaAsyncApi
} from '@/api/BaseInfoConfig/factoryinfo'
import { ref, watch, h, reactive } from 'vue'
import {
  useGetRolesAsync
} from '@/api/permission'
import {  message } from 'ant-design-vue';
const role_Input = ref('')
const role_qurety = () => {
  roleData(role_Input.value)
}

/* 页面缓存 */
defineOptions({
  name: 'A0202'
})
const roleData = async (roleNam) => {
  role_treeData.value = []
  const data = await useGetRolesAsync(roleNam,true)
  if (data.code === 200 && data.success) {
    structure(data.data)
  } else {
    message.error('角色查询失败!')
  }
}
const role_treeData = ref([])
//角色调用并追截数据逻辑
const structure = (data, arr) => {
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
          disabled: parent.isReadOnly,
        })
      })
    }
    role_treeData.value.push(obj)
  })
}
roleData()
const selectedRoleKeys = ref([]);
watch(selectedRoleKeys, () => {
  selectedGroupAndMediaAsync(selectedRoleKeys.value[0])
});
//查询角色已绑定工厂和介质
async function selectedGroupAndMediaAsync(roleId) {
  const query = {
    roleId: roleId
  }
  const response = await selectedGroupAndMediaAsyncApi(query);
  if (response.code === 200 && response.success) {
    checkedFactoryKeys.value=response.data.groups
    checkedMediaKeys.value=response.data.medias
  }
}


//工厂树
const factoryTreeData = ref([]);
const factoryData = async () => {
  const data = await getFactoryTreeDataApi()
  if (data.code === 200 && data.success) {
    factoryTreeData.value=data.data
  } else {
    message.error('工厂树查询失败!')
  }
}
factoryData();
//保存工厂
const checkedFactoryKeys = ref([]);
const saveFactoryDto = reactive({
  roleId: '',
  groups: []
})
const saveFactory = async()=>{
  if(selectedRoleKeys.value.length==0){
    message.warning('请选择角色。')
  }
  if(checkedFactoryKeys.value.length==0){
    message.warning('请选择工厂。')
  }
  saveFactoryDto.roleId=selectedRoleKeys.value[0]
  saveFactoryDto.groups=checkedFactoryKeys.value.checked
  const response = await insertSysRoleGroupAsyncApi(saveFactoryDto);
  if (response.code === 200 && response.success) {
    message.success('保存成功')
  }
}

//介质树
const mediaTreeData = ref([]);
const meidaData = async () => {
  const data = await getMediaTreeDataApi()
  if (data.code === 200 && data.success) {
    mediaTreeData.value=data.data
    // structure(data.data, mediaTreeData)
  } else {
    message.error('介质树查询失败!')
  }
}
meidaData();

//保存介质
const checkedMediaKeys = ref([]);
const saveMediaDto = reactive({
  roleId: '',
  medias: []
})
const saveMedia = async()=>{
  if(selectedRoleKeys.value.length==0){
    message.warning('请选择角色。')
  }
  if(checkedMediaKeys.value.length==0){
    message.warning('请选择介质。')
  }
  saveMediaDto.roleId=selectedRoleKeys.value[0]
  saveMediaDto.medias=checkedMediaKeys.value.checked
  const response = await insertSysMediaAsyncApi(saveMediaDto);
  if (response.code === 200 && response.success) {
    message.success('保存成功')
  }
}
</script>


<style scoped>
.all {
  width: 100%;
  height: 100%;
  /* border: 1px solid red; */
  display: flex;
}

.title {
  font-size: 20px;
  font-weight: 600;
  height: 35px;
  line-height: 35px;
  padding-left: 16px;
}

.btn {
  height: 20px;
  display: flex;
  margin: 13px;
}

.role {
  width: 30%;
  height: 100%;
  background-color: white;
}

.factory {
  width: 40%;
  height: 100%;
  background-color: white;
  margin-left: 10px;
  margin-right: 10px;
}

.media {
  width: 40%;
  height: 100%;
  background-color: white;
}
</style>