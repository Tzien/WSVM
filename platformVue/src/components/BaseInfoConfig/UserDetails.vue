<template>
  <a-drawer v-model:open="childrenOpen" :width="'1200'" @close="close" title="用户信息详情">
    <a-descriptions bordered>
      <a-descriptions-item label="用户名称">{{ userData?.realName }}</a-descriptions-item>
      <a-descriptions-item label="性别">{{ userData?.sex == 0 ? '男' : '女' }}</a-descriptions-item>
      <a-descriptions-item label="身份证号">{{ userData?.idCard }}</a-descriptions-item>
      <a-descriptions-item label="是否激活">{{ userData?.enabled ? '激活' : '冻结' }}</a-descriptions-item>
      <a-descriptions-item label="邮箱">{{ userData?.email }}</a-descriptions-item>
      <a-descriptions-item label="手机号">{{ userData?.phone }}</a-descriptions-item>
      <a-descriptions-item label="账号">{{ userData?.loginName }}</a-descriptions-item>
      <a-descriptions-item label="登录别名">{{ userData?.alias }}</a-descriptions-item>
      <a-descriptions-item label="编码">{{ userData?.code }}</a-descriptions-item>
      <a-descriptions-item label="排序">{{ userData?.sort }}</a-descriptions-item>
      <a-descriptions-item label="微信">{{ userData?.webChat }}</a-descriptions-item>
      <a-descriptions-item label="钉钉">{{ userData?.dingTalk }}</a-descriptions-item>
      <a-descriptions-item label="出生日期">{{ userData?.birthdate }}</a-descriptions-item>
      <a-descriptions-item label="角色" :span="2">{{ userData?.rolesName }}</a-descriptions-item>
      <a-descriptions-item label="直属组织">{{ userData?.oraganizesName }}</a-descriptions-item>
      <a-descriptions-item label="管理组织" :span="2">{{ userData?.managingName }}</a-descriptions-item>
      <a-descriptions-item label="职位">{{ userData?.jobType }}</a-descriptions-item>
      <a-descriptions-item label="岗位" :span="2">{{ userData?.postsName }}</a-descriptions-item>
      <a-descriptions-item label="办公电话">{{ userData?.officePhone }}</a-descriptions-item>
      <a-descriptions-item label="所属组织" :span="2">{{ userData?.deptName }}</a-descriptions-item>
      <a-descriptions-item label="部门变动日期">{{ userData?.modifiedTime }}</a-descriptions-item>
      <a-descriptions-item label="正式入职日期(合同生效日期)">{{ userData?.rybeginDate }}</a-descriptions-item>
      <a-descriptions-item label="离职日期(停薪日期)">{{ userData?.ryendDate }}</a-descriptions-item>
      <a-descriptions-item label="备注" :span="3">
        {{ userData?.remark }}
      </a-descriptions-item>
    </a-descriptions>
  </a-drawer>
</template>
<script setup>
import { ref, watch } from 'vue'
import { useGetAllUserMessageAsync } from '@/api/user'
const props = defineProps({
  open: Boolean,
  userid: String
})
const childrenOpen = ref(false)
watch(
  () => props.open,
  (newOpen) => {
    childrenOpen.value = newOpen
    if (newOpen) allmsg()
  }
)
const emit = defineEmits(['closeDetails'])
const close = (e) => {
  childrenOpen.value = false
  emit('closeDetails', false)
}
//******************************************************* */
const userData = ref()
const allmsg = () => {
  useGetAllUserMessageAsync(props.userid).then((data) => {
    if (data.code === 200 && data.success) {
      userData.value = data.data
    } else {
      message.error(data.message)
    }
  })
}
</script>
