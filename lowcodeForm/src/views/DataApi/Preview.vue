<template>
  <div class="Preview">
    <div class="tool">
      <a-button @click="$emit('close')">返回</a-button>
      <p>接口预览</p>
    </div>
    <div class="content">
      <a-form style="width: 100%; padding: 10px; height: 100%" :model="formState" :label-col="labelCol" :wrapper-col="wrapperCol">
        <!-- Request URL -->
        <a-form-item label="Request URL">
          <a-input-group compact>
            <!-- <a-button disabled>{{ formState.method }}</a-button>
            <a-input disabled v-if="formState.method=='GET'" style="width: calc(100% - 143px)" v-model:value="formState.apiUrl" />
            
            <a-input disabled v-if="formState.method=='POST'" style="width: calc(100% - 152px)" v-model:value="formState.apiUrl" /> -->
            <a-input disabled style="width: calc(100% - 90px)" v-model:value="formState.apiUrl" />
            <a-button type="primary" @click="testApi">测试接口</a-button>
          </a-input-group>
        </a-form-item>

        <!-- 如果 type=1 -->
        <template v-if="formState.type === '1'">
          <template v-if="sqlApiParams">
            <div v-for="(item, index) in sqlApiParams" :key="item.key">
              <a-form-item label="Request Param">
                <a-input-group compact>
                  <a-input style="width: 10%" v-model:value="item.paramName" disabled />
                  <a-input style="width: 90%" v-model:value="item.defaultValue" allow-clear />
                </a-input-group>
              </a-form-item>
            </div>
          </template>
        </template>

        <!-- 如果 type=2 -->
        <template v-if="formState.type === '2'">
          <!-- GET -->
          <template v-if="queryParams">
            <div v-for="(item, index) in queryParams" :key="item.key">
              <a-form-item label="Request Param">
                <a-input-group compact>
                  <a-input style="width: 10%" v-model:value="item.name" disabled />
                  <a-input style="width: 90%" v-model:value="item.value" allow-clear />
                </a-input-group>
              </a-form-item>
            </div>
          </template>
        </template>

        <!-- 如果 type=3 -->
        <template v-if="formState.type === '3'">
          <template v-if="sqlApiParams">
            <div v-for="(item, index) in sqlApiParams" :key="item.key">
              <a-form-item label="Request Param">
                <a-input-group compact>
                  <a-input style="width: 10%" v-model:value="item.paramName" disabled />
                  <a-input style="width: 90%" v-model:value="item.defaultValue" allow-clear />
                </a-input-group>
              </a-form-item>
            </div>
          </template>
        </template>

        <!-- Response Body -->
        <a-form-item label="Response Body">
          <a-textarea style="height: 400px" v-model:value="formState.desc" />
        </a-form-item>
      </a-form>
    </div>
  </div>
</template>
<script setup>
import { ref, reactive, onMounted, toRaw } from 'vue'
import { getDetailApi, testApiApi } from '@/api/DataApi/DataApi'
import { message } from 'ant-design-vue'
import { lowCodeUrl } from '../../utils/request'
const formState = reactive({
  apiUrl: '',
  desc: '',
  method: '',
  type: ''
})

const queryParams = ref([])
const sqlApiParams = ref([])
const postParams = ref([])

const labelCol = {
  style: {
    width: '150px'
  }
}
const wrapperCol = {
  span: 24
}

const props = defineProps({
  dataApiId: {
    type: String,
    default: ''
  },

  dataHandler: {
    type: String,
    default: ''
  },

  dataType: {
    type: String,
    default: ''
  }
})

onMounted(() => {
  console.info('id:', props.dataApiId)
  if (props.dataApiId) {
    getDetail()
  }
})

async function getDetail() {
  const obj = { id: props.dataApiId }
  const res = await getDetailApi(obj)
  if (res.code === 200 && res.success) {
    const data = res.data
    formState.apiUrl = lowCodeUrl + '/api/DataApi/TestApi/' + props.dataApiId
    formState.desc = data.desc
    formState.method = data.method
    formState.type = data.type

    if (data.type === '2') {
      if (data.type === '2' && data.method === 'GET') {
        queryParams.value = data.queryData ? JSON.parse(data.queryData) : []
      } else if (data.method === 'POST') {
        // if (data.formData && data.formData !== '[]') {
        //   postParams.value = data.formData ? JSON.parse(data.formData) : []
        // } else if (data.wwwFormData && data.wwwFormData !== '[]') {
        //   postParams.value = data.wwwFormData ? JSON.parse(data.wwwFormData) : []
        // }
      }
    } else if (data.type === '1') {
      console.info(data.apiDataParamsJson)
      sqlApiParams.value = data.apiDataParamsJson ? JSON.parse(data.apiDataParamsJson) : []
    } else if (data.type === '3') {
      console.info(data)
      // sqlApiParams.value = data.apiDataParamsJson ? JSON.parse(data.apiDataParamsJson) : []
    }
  }
}

// 解析你这种双重 JSON 转义的数组
function parseParamData(str) {
  try {
    const once = JSON.parse(str)
    return JSON.parse(once)
  } catch (e) {
    console.error('参数解析失败:', e)
    return []
  }
}

const testApiDto = reactive({
  apiParams: [],
  sqlApiParams: []
})
const handler = new Function('return ' + props.dataHandler)()
async function testApi() {
  testApiDto.apiParams = queryParams.value.map((item) => ({
    name: item.name,
    value: item.value
  }))
  testApiDto.sqlApiParams = sqlApiParams.value.map((item) => ({
    paramName: item.paramName,
    paramType: item.paramType,
    defaultValue: item.defaultValue,
    required: item.required
  }))

  const response = await testApiApi(props.dataApiId, testApiDto)
  if (props.dataType == '3') {
    formState.desc = JSON.stringify(handler(response), null, 2)
  } else {
    if (response.code === 200) {
      if (response.success) {
        message.success('测试成功')
      } else {
        message.warning('测试失败')
      }
      response.data = handler(response.data)
      formState.desc = JSON.stringify(response, null, 2)
    } else {
      message.warning('接口异常')
    }
  }
}
</script>

<style lang="scss">
.Preview {
  width: 100%;
  height: 100%;
  // background: green;
  padding: 5px;
  z-index: 1000;

  .tool {
    width: 100%;
    height: 50px;
    // border: 1px solid blue;
    align-items: center;
    display: flex;
    p {
      margin: 0; /* 去掉 p 默认的上下外边距 */
      margin-left: 5px;
    }
  }
  .content {
    width: 100%;
    height: calc(100% - 50px);
    border-top: 1px solid gainsboro;
    // border: 1px solid green;
    overflow-y: auto;
  }
}
</style>
