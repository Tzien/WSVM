<template>
  <div class="oj">
    <div class="org">
      <div class="title">选择组织机构信息</div>
      <div class="btn">
        <a-input-search
          v-model:value="orgInput"
          style="height: 32px; margin: 0px 8px"
          placeholder="请输入组织机构名称"
          @search="oraganize"
        />
      </div>
      <div style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>
      <div class="orgtree">
 <a-tree
        v-model:expandedKeys="expandedKeys"
        v-model:selectedKeys="selectedKeys"
        :tree-data="treeData"
      >
      </a-tree>
      </div>
     
    </div>
    <div class="job">
      <div class="title">选择岗位信息</div>
      <div class="btn" v-if="jobxian">
        <a-input-search
          v-model:value="jobInput"
          style="height: 32px; margin: 0px 8px"
          placeholder="请输入岗位名称"
          @search="job"
        />
        <a-button @click="addJob" type="primary">保存岗位</a-button>
      </div>
      <div v-if="jobxian" style="margin: 0 10px">
        <a-divider style="height: 1px; background-color: rgb(242, 242, 242)" />
      </div>
      <div class="jobtree"  v-if="jobxian">
 <a-tree
       
        v-model:checkedKeys="jobCheckedKeys"
        checkable
        :tree-data="jobTreeData"
      >
      </a-tree>
      </div>
     
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import { message } from 'ant-design-vue';
import {
  useGetOraganizeTreeAsync,
  useGetOrganizeJobAsync,
  useGetPostsAsync,
  useInsertOrganizeJobAsync
} from '@/api/BaseInfoConfig/organization';

const orgInput = ref('');
const treeData = ref([]);
const expandedKeys = ref([]);
const selectedKeys = ref([]);
const jobxian = ref(false);

// 组织机构相关
const convertTree = (data) => {
  if (!data) {
    return null;
  }
  const converted = {
    title: data.name,
    key: data.id,
    disabled: data.isReadonly,
    children: []
  };
  if (data.oraganizeTrees && data.oraganizeTrees.length) {
    data.oraganizeTrees.forEach((or) => {
      converted.children.push(convertTree(or));
    });
  }
  return converted;
};

const oraganize = async () => {
  try {
    const data = await useGetOraganizeTreeAsync(null, orgInput.value);
    if (data.code === 200 && data.success) {
      treeData.value = [];
      data.data.forEach((or) => {
        treeData.value.push(convertTree(or));
      });
    } else {
      message.error(data.message);
    }
  } catch (error) {
    message.error('获取组织机构失败：' + error.message);
  }
};
oraganize();

// 岗位相关
const convertJobTree = (data) => {
  if (!data) {
    return null;
  }
  const converted = {
    title: data.title,
    key: data.key,
    disabled: data.isReadonly,
    children: []
  };
  if (data.children && data.children.length) {
    data.children.forEach((or) => {
      converted.children.push(convertTree(or));
    });
  }
  return converted;
};

// 岗位相关
const jobInput = ref('');
const jobTreeData = ref([]);
const jobCheckedKeys = ref([]);

const job = async () => {
  try {
    const data = await useGetPostsAsync(jobInput.value);
    if (data.code === 200 && data.success) {
       jobTreeData.value = [];
      data.data.forEach((or) => {
        jobTreeData.value.push(convertTree(or));
      });
      //jobTreeData.value = data.data;
    } else {
      message.error(data.message);
    }
  } catch (error) {
    message.error('获取岗位失败：' + error.message);
  }
};
job();

// 获取组织机构已关联的岗位
const jobids = async (orgid) => {
  try {
    const data = await useGetOrganizeJobAsync(orgid);
    if (data.code === 200 && data.success) {
      jobCheckedKeys.value = data.data;
    } else {
      message.error(data.message);
    }
  } catch (error) {
    message.error('获取已关联岗位失败：' + error.message);
  }
};

// 保存岗位关联
const addJob = async () => {
  if (!selectedKeys.value || !selectedKeys.value[0]) {
    message.warning('请选择组织机构');
    return;
  }
  if (!jobCheckedKeys.value || jobCheckedKeys.value.length === 0) {
    message.warning('请选择岗位');
    return;
  }
  
  try {
    const data = await useInsertOrganizeJobAsync({
      organizeId: selectedKeys.value[0],
      jobIds: jobCheckedKeys.value
    });
    if (data.code === 200 && data.success) {
      message.success('操作成功');
      // 刷新已关联的岗位列表
      await jobids(selectedKeys.value[0]);
    } else {
      message.error(data.message);
    }
  } catch (error) {
    message.error('保存失败：' + error.message);
  }
};

// 监听选中的组织机构
watch(selectedKeys, async (newVal) => {
  if (!newVal || !newVal[0]) {
    jobxian.value = false;
    jobCheckedKeys.value = []; // 清空已选中的岗位
  } else {
    jobxian.value = true;
    await jobids(newVal[0]);
    console.log('selectedKeys', newVal);
  }
});

// 监听岗位选中状态
watch(jobCheckedKeys, (newVal) => {
  console.log('checkedKeys', newVal);
});

// 监听展开节点（可以移除空的watch）
watch(expandedKeys, () => {});
</script>

<style lang="scss" scoped>
.oj {
  display: flex;
  width: 100%;
  height: 100%;
  background-color: rgb(255, 255, 255);
  
  :where(.css-dev-only-do-not-override-1hsjdkk).ant-tree .ant-tree-node-content-wrapper,
  :where(.css-dev-only-do-not-override-1hsjdkk).ant-tree .ant-tree-checkbox + span {
    font-size: 18px;
  }
}

.org {
  width: 50%;
  height: 100%;
  border-right: 1px solid rgb(205, 205, 216);
  padding: 15px;
}

.job {
  width: 50%;
  height: 100%;
  padding: 15px;
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

.orgtree,
.jobtree {
  height: calc(100% - 120px);
  margin: 10px 0 10px 15px;
  overflow-y: auto;
}
</style>