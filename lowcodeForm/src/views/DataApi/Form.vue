<template>
  <div class="form-fullscreen">
    <!-- 步骤导航 + 操作按钮同一行 -->
    <div class="form-header">
      <a-button type="link" @click="$emit('close')">← 返回</a-button>
      <a-steps :current="current" :items="steps" direction="horizontal" size="small" />
      <div class="form-actions">
        <a-button v-if="current > 0" @click="prev" style="margin-right: 8px"> 上一步 </a-button>
        <a-button v-if="current < steps.length - 1" type="primary" @click="next" style="margin-right: 8px"> 下一步 </a-button>
        <a-button v-if="current === steps.length - 1" type="primary" @click="done"> 完成 </a-button>
      </div>
    </div>

    <!-- 中间内容区 -->
    <div class="steps-content">
      <div v-if="steps[current]?.title === '基本信息'" class="form-wrapper">
        <a-form :model="formState" :label-col="labelCol">
          <a-form-item label="名称">
            <a-input v-model:value="formState.name" />
          </a-form-item>
          <a-form-item label="编码">
            <a-input v-model:value="formState.code" />
          </a-form-item>
          <a-form-item label="分类">
            <a-select v-model:value="formState.category" size="middle" :options="apiCategoryOptions"></a-select>
          </a-form-item>
          <a-form-item label="类型">
            <a-radio-group style="display: flex" v-model:value="formState.type">
              <a-radio value="0">静态数据</a-radio>
              <a-radio value="1">SQL操作</a-radio>
              <a-radio value="2">API操作</a-radio>
              <a-radio value="3">组态接口</a-radio>
            </a-radio-group>
          </a-form-item>
          <a-form-item label="排序">
            <a-input-number style="width: 100%" v-model:value="formState.sort" :min="0" />
          </a-form-item>
          <a-form-item label="说明">
            <a-textarea v-model:value="formState.desc" />
          </a-form-item>
        </a-form>
      </div>

      <!-- 第二步内容：数据配置 -->
      <div v-else-if="steps[current]?.title === '数据配置'" style="height: 100%">
        <!-- 静态数据 -->
        <div v-if="formState.type == '0'" style="height: 100%" class="sdata">
          <div class="sdata-left" style="height: 100%">
            <MonacoEditor class="" style="text-align: left;height: 100%;" ref="editorRef" v-model="formState.staticData" language="json" />
          </div>
          <div class="sdata-right">
            <div class="title-box">
              <span> 字段列表<BasicHelp text="设置接口返回字段" /> </span>
            </div>
            <a-table
              style="height: 627px"
              :columns="sDataResponseColumns"
              :scroll="{ y: 600 }"
              :dataSource="sDataResponseData"
              :pagination="false"
              rowKey="name"
              bordered
            >
            </a-table>
            <div class="table-footer">
              <a-button type="" style="color: blueviolet; width: 100%" @click="showsDataRequest()">新建</a-button>
            </div>
          </div>
        </div>

        <!-- sql语句 -->
        <div v-if="formState.type == '1'" style="height: 100%" class="sql">
          <div class="sql-left">
            <a-select
              style="width: 100%; text-align: left"
              v-model:value="selectedDbId"
              placeholder="选择数据库"
              allowClear
              size="middle"
              :options="dbConnStrOptions"
            ></a-select>
            <div class="db">
              <a-input-search v-model:value="searchValue" style="margin-bottom: 8px" placeholder="输入关键字" />
              <a-tree :expanded-keys="expandedKeys" :auto-expand-parent="autoExpandParent" :tree-data="filteredData" @expand="onExpand">
                <template #title="{ title }">
                  <span v-if="title.indexOf(searchValue) > -1">
                    {{ title.substring(0, title.indexOf(searchValue)) }}
                    <span style="color: #f50">{{ searchValue }}</span>
                    {{ title.substring(title.indexOf(searchValue) + searchValue.length) }}
                  </span>
                  <span v-else>{{ title }}</span>
                </template>
              </a-tree>
            </div>
          </div>
          <div class="sql-right">
            <div class="title-box">
              <span> SQL语句<BasicHelp text="支持SQL语句&存储过程语句" /> </span>
            </div>
            <MonacoEditor style="text-align: left" class="sql-editor" ref="sqlEditorRef" v-model="formState.sqlStr" language="sql" />
          </div>
          <div class="right2">
            <div class="top">
              <div class="title-box">
                <span> 接口参数<BasicHelp text="接收方式：QueryParams" /> </span>
              </div>
              <a-table
                style="height: 277px"
                :columns="sqlApiParamsColumns"
                :scroll="{ y: 221 }"
                :dataSource="sqlApiParamsData"
                :pagination="false"
                rowKey="paramName"
                bordered
              >
              </a-table>
              <div class="table-footer">
                <a-button type="" style="color: blueviolet; width: 100%" @click="showSqlApiParams()">新建</a-button>
              </div>
            </div>
            <div class="bottom">
              <div class="title-box">
                <span> 字段列表<BasicHelp text="设置接口返回字段" /> </span>
              </div>
              <a-table
                style="height: 277px"
                :columns="sqlApiResponseColumns"
                :scroll="{ y: 221 }"
                :dataSource="sqlApiResponseData"
                :pagination="false"
                rowKey="name"
                bordered
              >
              </a-table>
              <div class="table-footer">
                <a-button type="" style="color: blueviolet; width: 100%" @click="showSqlApiRequest()">新建</a-button>
              </div>
            </div>
          </div>
        </div>

        <!-- api操作 -->
        <div v-if="formState.type == '2'" class="api">
          <div class="left">
            <div class="httpType">
              <a-input v-model:value="formState.apiUrl" style="width: 100%">
                <template #addonBefore>
                  <a-select v-model:value="formState.method" style="width: 80px">
                    <a-select-option value="GET">GET</a-select-option>
                    <a-select-option value="POST">POST</a-select-option>
                  </a-select>
                </template>
              </a-input>
            </div>
            <div class="queryTitle">
              <span>请求参数</span>
            </div>
            <div class="queryTags">
              <a-tabs v-model:activeKey="activeTab">
                <a-tab-pane key="Header" tab="Header">
                  <a-table :dataSource="headerData" :pagination="false" rowKey="key" bordered>
                    <a-table-column title="参数名" key="name">
                      <template #default="{ record }">
                        <a-input v-model:value="record.name" placeholder="请输入参数名" />
                      </template>
                    </a-table-column>

                    <a-table-column title="类型" key="type">
                      <template #default="{ record }">
                        <a-select v-model:value="record.type" placeholder="选择类型" style="width: 100px">
                          <a-select-option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</a-select-option>
                        </a-select>
                      </template>
                    </a-table-column>

                    <a-table-column title="参数值" key="value">
                      <template #default="{ record }">
                        <a-input v-model:value="record.value" placeholder="请输入参数值" />
                      </template>
                    </a-table-column>

                    <a-table-column title="操作" key="action">
                      <template #default="{ index }">
                        <a-button danger type="link" @click="deleteRow(headerData, index)">删除</a-button>
                      </template>
                    </a-table-column>
                  </a-table>
                  <div class="table-footer">
                    <a-button type="" style="color: blueviolet; width: 100%" @click="addRow(headerData)">添加Header参数</a-button>
                  </div>
                </a-tab-pane>

                <a-tab-pane key="Query" tab="Query">
                  <a-table :dataSource="queryData" :pagination="false" rowKey="key" bordered>
                    <a-table-column title="参数名" key="name">
                      <template #default="{ record }">
                        <a-input v-model:value="record.name" placeholder="请输入参数名" />
                      </template>
                    </a-table-column>

                    <a-table-column title="类型" key="type">
                      <template #default="{ record }">
                        <a-select v-model:value="record.type" placeholder="选择类型" style="width: 100px">
                          <a-select-option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</a-select-option>
                        </a-select>
                      </template>
                    </a-table-column>

                    <a-table-column title="参数值" key="value">
                      <template #default="{ record }">
                        <a-input v-model:value="record.value" placeholder="请输入参数值" />
                      </template>
                    </a-table-column>

                    <a-table-column title="操作" key="action">
                      <template #default="{ index }">
                        <a-button danger type="link" @click="deleteRow(queryData, index)">删除</a-button>
                      </template>
                    </a-table-column>
                  </a-table>
                  <div class="table-footer">
                    <a-button type="" style="color: blueviolet; width: 100%" @click="addRow(queryData)">添加Query参数</a-button>
                  </div>
                </a-tab-pane>

                <a-tab-pane key="Body" tab="Body">
                  <div class="bodyType">
                    <a-radio-group v-model:value="bodyType">
                      <a-radio value="none">none</a-radio>
                      <a-radio value="form-data">form-data</a-radio>
                      <a-radio value="x-www-form-urlencoded">x-www-form-urlencoded</a-radio>
                      <a-radio value="json">json</a-radio>
                      <a-radio value="xml">xml</a-radio>
                    </a-radio-group>
                  </div>
                  <div class="bodyContent">
                    <!-- none -->
                    <div v-if="bodyType === 'none'" style="color: gray; text-align: left; padding-top: 10px; padding-left: 5px">该请求没有 Body 主体</div>

                    <!-- form-data -->
                    <div v-else-if="bodyType === 'form-data'">
                      <a-table :dataSource="formData" :pagination="false" rowKey="key" bordered>
                        <a-table-column title="参数名" key="name">
                          <template #default="{ record }">
                            <a-input v-model:value="record.name" placeholder="请输入参数名" />
                          </template>
                        </a-table-column>

                        <a-table-column title="类型" key="type">
                          <template #default="{ record }">
                            <a-select v-model:value="record.type" placeholder="选择类型" style="width: 100px">
                              <a-select-option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</a-select-option>
                            </a-select>
                          </template>
                        </a-table-column>

                        <a-table-column title="参数值" key="value">
                          <template #default="{ record }">
                            <a-input v-model:value="record.value" placeholder="请输入参数值" />
                          </template>
                        </a-table-column>

                        <a-table-column title="操作" key="action">
                          <template #default="{ index }">
                            <a-button danger type="link" @click="deleteRow(formData, index)">删除</a-button>
                          </template>
                        </a-table-column>
                      </a-table>
                      <div class="table-footer">
                        <a-button type="" style="color: blueviolet; width: 100%" @click="addRow(formData)">添加Body参数</a-button>
                      </div>
                    </div>

                    <!-- x-www-form-urlencoded -->
                    <div v-else-if="bodyType === 'x-www-form-urlencoded'">
                      <a-table :dataSource="wwwFormData" :pagination="false" rowKey="key" bordered>
                        <a-table-column title="参数名" key="name">
                          <template #default="{ record }">
                            <a-input v-model:value="record.name" placeholder="请输入参数名" />
                          </template>
                        </a-table-column>

                        <a-table-column title="类型" key="type">
                          <template #default="{ record }">
                            <a-select v-model:value="record.type" placeholder="选择类型" style="width: 100px">
                              <a-select-option v-for="type in typeOptions" :key="type" :value="type">{{ type }}</a-select-option>
                            </a-select>
                          </template>
                        </a-table-column>

                        <a-table-column title="参数值" key="value">
                          <template #default="{ record }">
                            <a-input v-model:value="record.value" placeholder="请输入参数值" />
                          </template>
                        </a-table-column>

                        <a-table-column title="操作" key="action">
                          <template #default="{ index }">
                            <a-button danger type="link" @click="deleteRow(wwwFormData, index)">删除</a-button>
                          </template>
                        </a-table-column>
                      </a-table>
                      <div class="table-footer">
                        <a-button type="" style="color: blueviolet; width: 100%" @click="addRow(wwwFormData)">添加Body参数</a-button>
                      </div>
                    </div>

                    <!-- json -->
                    <div v-else-if="bodyType === 'json'">
                      <div class="" style="height: 462px">
                        <MonacoEditor class="" style="text-align: left" ref="editorRef" v-model="formState.bodyJsonData" language="json" />
                      </div>
                    </div>

                    <!-- xml -->
                    <div v-else-if="bodyType === 'xml'">
                      <div class="" style="height: 462px">
                        <MonacoEditor class="" style="text-align: left" ref="editorRef" v-model="formState.bodyXmlData" language="json" />
                      </div>
                    </div>
                  </div>
                </a-tab-pane>
              </a-tabs>
            </div>
          </div>
          <div class="right">
            <div class="title-box">
              <span> 字段列表<BasicHelp text="设置接口返回字段" /> </span>
            </div>
            <a-table
              style="height: 627px"
              :columns="apiDataResponseColumns"
              :scroll="{ y: 600 }"
              :dataSource="apiDataResponseData"
              :pagination="false"
              rowKey="name"
              bordered
            >
            </a-table>
            <div class="table-footer">
              <a-button type="" style="color: blueviolet; width: 100%" @click="showapiDataRequest()">新建</a-button>
            </div>
          </div>
        </div>

        
        <!-- 组态接口 -->
        <div v-if="formState.type == '3'" style="height: 100%" class="sql">
          <div class="sql-left">
            <a-select
              style="width: 100%; text-align: left"
              v-model:value="selectedDbId"
              placeholder="选择数据库"
              allowClear
              size="middle"
              :options="dbConnStrOptions"
            ></a-select>
            <div class="db">
              <a-input-search v-model:value="searchValue" style="margin-bottom: 8px" placeholder="输入关键字" />
              <a-tree :expanded-keys="expandedKeys" :auto-expand-parent="autoExpandParent" :tree-data="filteredData" @expand="onExpand">
                <template #title="{ title }">
                  <span v-if="title.indexOf(searchValue) > -1">
                    {{ title.substring(0, title.indexOf(searchValue)) }}
                    <span style="color: #f50">{{ searchValue }}</span>
                    {{ title.substring(title.indexOf(searchValue) + searchValue.length) }}
                  </span>
                  <span v-else>{{ title }}</span>
                </template>
              </a-tree>
            </div>
          </div>
          <div class="sql-right">
            <div class="title-box">
              <span> SQL语句<BasicHelp text="支持SQL语句&存储过程语句" /> </span>
            </div>
            <MonacoEditor style="text-align: left" class="sql-editor" ref="sqlEditorRef" v-model="formState.sqlStr" language="sql" />
          </div>
          <div class="right2">
            <div class="top">
              <div class="title-box">
                <span> 接口参数<BasicHelp text="接收方式：QueryParams" /> </span>
              </div>
              <a-table
                style="height: 277px"
                :columns="sqlApiParamsColumns"
                :scroll="{ y: 221 }"
                :dataSource="sqlApiParamsData"
                :pagination="false"
                rowKey="paramName"
                bordered
              >
              </a-table>
              <div class="table-footer">
                <a-button type="" style="color: blueviolet; width: 100%" @click="showSqlApiParams()">新建</a-button>
              </div>
            </div>
            <div class="bottom">
              <div class="title-box">
                <span> 字段列表<BasicHelp text="设置接口返回字段" /> </span>
              </div>
              <a-table
                style="height: 277px"
                :columns="sqlApiResponseColumns"
                :scroll="{ y: 221 }"
                :dataSource="sqlApiResponseData"
                :pagination="false"
                rowKey="name"
                bordered
              >
              </a-table>
              <div class="table-footer">
                <a-button type="" style="color: blueviolet; width: 100%" @click="showSqlApiRequest()">新建</a-button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 异常验证 -->
      <div v-else-if="steps[current]?.title === '异常验证'" style="height: 100%">
        <div class="" style="height: 85%">
          <MonacoEditor class="" style="text-align: left; height: 100%" ref="editorRef" v-model="formState.exVer" language="json" />
        </div>
        <div class="jsTips">
          <p>1、支持JavaScript的脚本</p>
          <p>2、小程序不支持在线JS脚本</p>
        </div>
      </div>

      <!-- 数据处理 -->
      <div v-else-if="steps[current]?.title === '数据处理'" style="height: 100%">
        <div class="" style="height: 85%">
          <MonacoEditor class="" style="text-align: left; height: 100%" ref="editorRef" v-model="formState.dataHandler" language="json" />
        </div>
        <div class="jsTips">
          <p>1、支持JavaScript的脚本</p>
          <p>2、小程序不支持在线JS脚本</p>
        </div>
      </div>
    </div>
  </div>
  <a-modal v-model:open="sqlApiParams" title="参数配置" @ok="sqlApiParamsHandleOk">
    <a-form layout="vertical">
      <!-- 参数名称 -->
      <a-form-item label="参数名称">
        <a-input v-model:value="sqlApiParamsForm.paramName" placeholder="请输入参数名称" />
      </a-form-item>

      <!-- 参数类型 -->
      <a-form-item label="参数类型">
        <a-select v-model:value="sqlApiParamsForm.paramType" placeholder="请选择参数类型">
          <a-select-option value="string">字符串</a-select-option>
          <a-select-option value="int">数字</a-select-option>
          <a-select-option value="datetime">时间</a-select-option>
        </a-select>
      </a-form-item>

      <!-- 默认值 -->
      <a-form-item label="默认值">
        <!-- 字符串 -->
        <a-input v-if="sqlApiParamsForm.paramType === 'string'" v-model:value="sqlApiParamsForm.defaultValue" placeholder="请输入默认值" />

        <!-- 整形 -->
        <a-input-number
          v-else-if="sqlApiParamsForm.paramType === 'int'"
          v-model:value="sqlApiParamsForm.defaultValue"
          style="width: 100%"
          placeholder="请输入数字"
        />

        <!-- 时间 -->
        <a-date-picker
          v-else-if="sqlApiParamsForm.paramType === 'datetime'"
          v-model:value="sqlApiParamsForm.defaultValue"
          show-time
          style="width: 100%"
          placeholder="请选择时间"
        />
      </a-form-item>

      <!-- 必填 -->
      <a-form-item label="必填">
        <a-switch v-model:checked="sqlApiParamsForm.required" />
      </a-form-item>

      <!-- 参数说明 -->
      <a-form-item label="参数说明">
        <a-input v-model:value="sqlApiParamsForm.description" placeholder="请输入参数说明" />
      </a-form-item>
    </a-form>
  </a-modal>
  <a-modal v-model:open="sqlApiResponse" title="参数配置" @ok="sqlApiResponseHandleOk">
    <a-form layout="vertical">
      <!-- 参数名称 -->
      <a-form-item label="字段名称">
        <a-input v-model:value="sqlApiResponseForm.name" placeholder="请输入参数名称" />
      </a-form-item>
      <a-form-item label="映射字段">
        <a-input v-model:value="sqlApiResponseForm.mapName" placeholder="请输入参数名称" />
      </a-form-item>
    </a-form>
  </a-modal>
  <a-modal v-model:open="sDataResponse" title="参数配置" @ok="sDataResponseHandleOk">
    <a-form layout="vertical">
      <!-- 参数名称 -->
      <a-form-item label="字段名称">
        <a-input v-model:value="sDataResponseForm.name" placeholder="请输入参数名称" />
      </a-form-item>
      <a-form-item label="映射字段">
        <a-input v-model:value="sDataResponseForm.mapName" placeholder="请输入参数名称" />
      </a-form-item>
    </a-form>
  </a-modal>
  <a-modal v-model:open="apiDataResponse" title="参数配置" @ok="apiDataResponseHandleOk">
    <a-form layout="vertical">
      <!-- 参数名称 -->
      <a-form-item label="字段名称">
        <a-input v-model:value="apiDataResponseForm.name" placeholder="请输入参数名称" />
      </a-form-item>
      <a-form-item label="映射字段">
        <a-input v-model:value="apiDataResponseForm.mapName" placeholder="请输入参数名称" />
      </a-form-item>
    </a-form>
  </a-modal>
</template>

<script setup>
import { ref, reactive, onMounted, watch, computed, toRaw, defineEmits, h, watchEffect } from 'vue'
import { message, Button, Space, Popconfirm } from 'ant-design-vue'
import dayjs from 'dayjs'
import { QuestionCircleOutlined } from '@ant-design/icons-vue'
import { MonacoEditor } from '@/components/CodeEditor'
import { getApiCategoryDictApi, getDbConStrApi, getDbTablesApi, saveApi, getDetailApi } from '@/api/DataApi/DataApi'
import { useUserStore } from '@/store/index'
import { useGlobalState } from '@/shared/useGlobalState'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper'
const { globalStore } = useGlobalState()
var userStore = ref({})

/* 根据权限动态加载按钮 */
var inputRoleId = []
if (!qiankunWindow.__POWERED_BY_QIANKUN__) {
  userStore = useUserStore()
  inputRoleId = userStore.userRoles
} else {
  watchEffect(() => {
    if (globalStore.value) {
      userStore.value = globalStore.value.userStore
      inputRoleId = userStore.value.userRoles
    }
  })
}
onMounted(() => {
  getApiCategoryDict()
  getDbConStr()
  console.info('id：', props.dataApiId)
  if (props.dataApiId) {
    getDetail()
  }
})

const props = defineProps({
  dataApiId: ''
})
async function getDetail() {
  var obj = {
    id: props.dataApiId
  }
  await getDetailApi(obj).then((res) => {
    if (res.code == 200 && res.success) {
      // formState=res.data
      formState.dataApiId = res.data.dataApiId
      formState.name = res.data.name
      formState.code = res.data.code
      formState.category = res.data.category
      formState.type = res.data.type
      formState.sort = res.data.sort
      formState.desc = res.data.desc
      formState.staticData = res.data.staticData
      formState.sqlStr = res.data.sqlStr
      formState.dataHandler = res.data.dataHandler
      formState.exVer = res.data.exVer
      formState.apiUrl = res.data.apiUrl
      formState.method = res.data.method
      formState.bodyJsonData = res.data.bodyJsonData
      formState.bodyXmlData = res.data.bodyXmlData

      formState.bodyType = res.data.bodyType
      selectedDbId.value = res.data.dbConfigId
      headerData.splice(0, headerData.length) // 清空数组
      headerData.push(...JSON.parse(res.data.headerData || '[]'))

      queryData.splice(0, queryData.length) // 清空数组
      queryData.push(...JSON.parse(res.data.queryData || '[]'))

      formData.splice(0, formData.length) // 清空数组
      formData.push(...JSON.parse(res.data.formData || '[]'))

      wwwFormData.splice(0, wwwFormData.length) // 清空数组
      wwwFormData.push(...JSON.parse(res.data.wwwFormData || '[]'))

      sqlApiParamsData.splice(0, sqlApiParamsData.length) // 清空数组
      sqlApiParamsData.push(...JSON.parse(res.data.apiDataParamsJson || '[]'))

      sqlApiResponseData.splice(0, sqlApiResponseData.length) // 清空数组
      sqlApiResponseData.push(...JSON.parse(res.data.fieldListJson || '[]'))

      sDataResponseData.splice(0, sDataResponseData.length) // 清空数组
      sDataResponseData.push(...JSON.parse(res.data.fieldListJson || '[]'))

      
      apiDataResponseData.splice(0, apiDataResponseData.length) // 清空数组
      apiDataResponseData.push(...JSON.parse(res.data.fieldListJson || '[]'))

      console.info(formState)
    }
  })
}

//基本信息
const formState = reactive({
  dataApiId: '',
  name: '',
  code: '',
  category: '',
  type: '0',
  sort: 0,
  desc: '',
  staticData: '',
  sqlStr: '',
  dbConfigId: '',
  dataHandler: `(data) => {
  // 处理数据逻辑

  // 返回所需的数据
  return data
}`,
  exVer: `(data) => {
    // 返回true表示接口验证成功！

    // 返回false表示接口验证失败！
    return true
}`,
  apiUrl: '',
  method: 'GET',
  bodyJsonData: '',
  bodyXmlData: '',
  headerData: '',
  queryData: '',
  formData: '',
  wwwFormData: '',
  bodyType: '',
  apiDataParamsJson: '',
  fieldListJson: ''
})

const current = ref(0)

// const next = () =>
function next() {
  if (formState.type == '1' && current.value == 1) {
    if (selectedDbId.value) {
      current.value++
    } else {
      message.warn('请选择数据库')
    }
  } else {
    current.value++
  }
}
const prev = () => current.value--

const emit = defineEmits(['done'])
async function done() {
  formState.headerData = JSON.stringify(toRaw(headerData))
  formState.queryData = JSON.stringify(toRaw(queryData))
  formState.formData = JSON.stringify(toRaw(formData))
  formState.wwwFormData = JSON.stringify(toRaw(wwwFormData))
  formState.bodyType = bodyType.value
  formState.dbConfigId = selectedDbId.value
  formState.apiDataParamsJson = JSON.stringify(toRaw(sqlApiParamsData))
  if(formState.type=='0'){
    formState.fieldListJson = JSON.stringify(toRaw(sDataResponseData))
  }else if(formState.type=='1'){
    formState.fieldListJson = JSON.stringify(toRaw(sqlApiResponseData))
  }else if(formState.type=='2'){
    formState.fieldListJson = JSON.stringify(toRaw(apiDataResponseData))
  }else if(formState.type=='3'){
    formState.fieldListJson = JSON.stringify(toRaw(sqlApiResponseData))
  }
  console.info(formState)
  const response = await saveApi(formState)
  if (response.code === 200 && response.success) {
    message.success(response.message)
  } else if (response.code === 200 && !response.success) {
    message.warning(response.message)
  }
  emit('done')
}

const allSteps = [
  { title: '基本信息', content: 'First-content' },
  { title: '数据配置', content: 'Second-content' },
  { title: '异常验证', content: 'Third-content' },
  { title: '数据处理', content: 'Fourth-content' }
]

// 初始化 steps，只显示 type 为 '0' 时的步骤
const steps = ref(allSteps.slice(0, 2)) // 基本信息 + 数据配置

watch(
  () => formState.type,
  (newVal) => {
    if (newVal === '2') {
      // 显示全部步骤
      steps.value = [...allSteps]
    } else if (newVal === '1' || newVal === '3') {
      // 基本信息 + 数据配置 + 数据处理（去掉异常验证）
      steps.value = allSteps.filter((step) => step.title !== '异常验证')
    } else {
      // 仅显示基本信息 + 数据配置
      steps.value = allSteps.slice(0, 2)
    }

    // 防止 current 越界
    if (current.value >= steps.value.length) {
      current.value = 0
    }
  }
)

// const items = steps.map((item) => ({
//   key: item.title,
//   title: item.title
// }))

//接口分类下拉框
const apiCategoryOptions = ref([])
async function getApiCategoryDict() {
  var obj = {
    dictCode: 'DataApiCategory'
  }
  await getApiCategoryDictApi(obj).then((res) => {
    const children = []
    res.data.items.forEach((c) => {
      children.push({
        label: c.itemName,
        value: c.dictDetailId
      })
    })
    apiCategoryOptions.value = children
  })
}

const labelCol = {
  style: {
    width: '50px'
  }
}

//静态数据
const sDataResponse = ref(false)
const sDataResponseData = reactive([])
const editingKey3 = ref(null) // 当前正在编辑的行 paramName
// 表格列
const sDataResponseColumns = [
  {
    title: '参数名称',
    dataIndex: 'name',
    key: 'name',
    width: '90px'
  },
  {
    title: '映射字段',
    dataIndex: 'mapName',
    key: 'mapName',
    width: '90px'
  },
  {
    title: '操作',
    key: 'action',
    customRender: ({ record }) => {
      return h(
        Space,
        {},
        {
          default: () => [
            // 编辑按钮
            h(
              Button,
              {
                type: 'link',
                onClick: () => handleEdit3(record)
              },
              { default: () => '编辑' }
            ),
            // 删除按钮 + 二次确认
            h(
              Popconfirm,
              {
                title: '确定删除吗？',
                onConfirm: () => handleDelete3(record.name)
              },
              {
                default: () => h(Button, { type: 'link', danger: true }, { default: () => '删除' })
              }
            )
          ]
        }
      )
    }
  }
]

const sDataResponseForm = reactive({
  name: '',
  mapName: ''
})
// 点击编辑时
const handleEdit3 = (record) => {
  editingKey3.value = record.name // 记录正在编辑的行
  Object.assign(sDataResponseForm, record) // 把数据填充到表单
  sDataResponse.value = true // 打开弹框
}
// 删除
const handleDelete3 = (name) => {
  const index = sDataResponseData.findIndex((item) => item.name === name)
  if (index !== -1) {
    sDataResponseData.splice(index, 1)
  }
}

function showsDataRequest() {
  sDataResponse.value = true
  resetForm3()
}

// 点击确定时（参考我之前给你的逻辑）
const sDataResponseHandleOk = () => {
  if (!sDataResponseForm.name) {
    message.error('参数名称不能为空！')
    return
  }

  if (editingKey3.value) {
    // 编辑模式
    const exist = sDataResponseData.find((item) => item.name === sDataResponseForm.name && item.name !== editingKey3.value)
    if (exist) {
      message.error('参数名称已存在，不能重复！')
      return
    }

    const index = sDataResponseData.findIndex((item) => item.name === editingKey3.value)
    if (index !== -1) {
      sDataResponseData[index] = { ...sDataResponseForm }
    }
  } else {
    // 新增模式
    const exist = sDataResponseData.find((item) => item.name === sDataResponseForm.name)
    if (exist) {
      message.error('参数名称已存在！')
      return
    }
    sDataResponseData.push({ ...sDataResponseForm })
  }

  sDataResponse.value = false
  editingKey3.value = null // 记得重置
  resetForm3()
}

// 重置表单
const resetForm3 = () => {
  sDataResponseForm.name = ''
  sDataResponseForm.mapName = ''
}

//sql左边数据库

//数据库连接串
const dbConnStrOptions = ref([])
async function getDbConStr() {
  const res = await getDbConStrApi()
  dbConnStrOptions.value = res.data.map((group) => ({
    label: group.dbType,
    options: group.items.map((item) => ({
      label: item.name,
      value: item.dbConfigId
    }))
  }))
}
const selectedDbId = ref(null)

watch(selectedDbId, (newVal) => {
  if (newVal) {
    console.log('选中的数据库配置 ID 是：', newVal)
    getDbTables(newVal)
  } else {
    gData.value = []
  }
})
//获取数据库表信息
const searchValue = ref('')
const gData = ref([])
const expandedKeys = ref([])
const autoExpandParent = ref(true)

async function getDbTables(dbConfigId) {
  var obj = {
    dbConfigId: dbConfigId
  }
  const res = await getDbTablesApi(obj)
  if (res.code == 200 && res.success) {
    gData.value = res.data.map((table) => {
      const tableKey = `table-${table.name}`
      return {
        title: table.name + (table.description ? `（${table.description}）` : ''),
        key: tableKey,
        children: table.columnInfos.map((col) => {
          const colKey = `${table.name}-${col.name}`
          const pkMark = col.isPrimaryKey ? ' 🔑' : ''
          const dest = col.description ? '（' + col.description + '）' : ''
          return {
            title: `${col.name}${dest}${pkMark}`,
            key: colKey,
            isLeaf: true
          }
        })
      }
    })
  } else {
    gData.value = []
  }
}
function onExpand(keys) {
  expandedKeys.value = keys
  autoExpandParent.value = false
}
// 过滤后的树数据
const filteredData = computed(() => {
  if (!searchValue.value) {
    return gData.value
  }

  return gData.value.filter((table) => table.title.includes(searchValue.value))
})

const sqlApiParams = ref(false)
const sqlApiParamsData = reactive([])
const editingKey = ref(null) // 当前正在编辑的行 paramName
// 表格列
const sqlApiParamsColumns = [
  {
    title: '参数名称',
    dataIndex: 'paramName',
    key: 'paramName',
    width: '90px',
    customRender: ({ text, record }) => {
      return h(
        'div',
        {
          style: {
            cursor: 'pointer',
            color: '#1677ff',
            width: '100%',
            height: '100%'
          },
          onClick: () => {
            formState.sqlStr = formState.sqlStr + '{' + record.paramName + '}'
            console.log('点击整列：', record.paramName)
          }
        },
        text
      )
    }
  },
  {
    title: '参数类型',
    dataIndex: 'paramType',
    key: 'paramType',
    width: '90px'
  },
  {
    title: '操作',
    key: 'action',
    customRender: ({ record }) => {
      return h(
        Space,
        {},
        {
          default: () => [
            // 编辑按钮
            h(
              Button,
              {
                type: 'link',
                onClick: () => handleEdit(record)
              },
              { default: () => '编辑' }
            ),
            // 删除按钮 + 二次确认
            h(
              Popconfirm,
              {
                title: '确定删除吗？',
                onConfirm: () => handleDelete(record.paramName)
              },
              {
                default: () => h(Button, { type: 'link', danger: true }, { default: () => '删除' })
              }
            )
          ]
        }
      )
    }
  }
]

// 点击编辑时
const handleEdit = (record) => {
  editingKey.value = record.paramName // 记录正在编辑的行
  Object.assign(sqlApiParamsForm, record) // 把数据填充到表单
  sqlApiParams.value = true // 打开弹框
}
// 删除
const handleDelete = (paramName) => {
  const index = sqlApiParamsData.findIndex((item) => item.paramName === paramName)
  if (index !== -1) {
    sqlApiParamsData.splice(index, 1)
  }
}

// 弹窗里的表单
const sqlApiParamsForm = reactive({
  paramName: '',
  paramType: 'string',
  defaultValue: null,
  required: false,
  description: ''
})
function showSqlApiParams() {
  sqlApiParams.value = true
  resetForm()
}

// 点击确定时（参考我之前给你的逻辑）
const sqlApiParamsHandleOk = () => {
  if (!sqlApiParamsForm.paramName) {
    message.error('参数名称不能为空！')
    return
  }

  if (editingKey.value) {
    // 编辑模式
    const exist = sqlApiParamsData.find((item) => item.paramName === sqlApiParamsForm.paramName && item.paramName !== editingKey.value)
    if (exist) {
      message.error('参数名称已存在，不能重复！')
      return
    }

    const index = sqlApiParamsData.findIndex((item) => item.paramName === editingKey.value)
    if (index !== -1) {
      sqlApiParamsData[index] = { ...sqlApiParamsForm }
    }
  } else {
    // 新增模式
    const exist = sqlApiParamsData.find((item) => item.paramName === sqlApiParamsForm.paramName)
    if (exist) {
      message.error('参数名称已存在！')
      return
    }
    sqlApiParamsData.push({ ...sqlApiParamsForm })
  }

  sqlApiParams.value = false
  editingKey.value = null // 记得重置
  resetForm()
}

// 重置表单
const resetForm = () => {
  sqlApiParamsForm.paramName = ''
  sqlApiParamsForm.paramType = 'string'
  sqlApiParamsForm.defaultValue = ''
  sqlApiParamsForm.required = false
  sqlApiParamsForm.description = ''
}

const sqlApiResponse = ref(false)
const sqlApiResponseData = reactive([])
const editingKey2 = ref(null) // 当前正在编辑的行 paramName
// 表格列
const sqlApiResponseColumns = [
  {
    title: '参数名称',
    dataIndex: 'name',
    key: 'name',
    width: '90px'
  },
  {
    title: '映射字段',
    dataIndex: 'mapName',
    key: 'mapName',
    width: '90px'
  },
  {
    title: '操作',
    key: 'action',
    customRender: ({ record }) => {
      return h(
        Space,
        {},
        {
          default: () => [
            // 编辑按钮
            h(
              Button,
              {
                type: 'link',
                onClick: () => handleEdit2(record)
              },
              { default: () => '编辑' }
            ),
            // 删除按钮 + 二次确认
            h(
              Popconfirm,
              {
                title: '确定删除吗？',
                onConfirm: () => handleDelete2(record.name)
              },
              {
                default: () => h(Button, { type: 'link', danger: true }, { default: () => '删除' })
              }
            )
          ]
        }
      )
    }
  }
]

// 点击编辑时
const handleEdit2 = (record) => {
  editingKey2.value = record.name // 记录正在编辑的行
  Object.assign(sqlApiResponseForm, record) // 把数据填充到表单
  sqlApiResponse.value = true // 打开弹框
}
// 删除
const handleDelete2 = (name) => {
  const index = sqlApiResponseData.findIndex((item) => item.name === name)
  if (index !== -1) {
    sqlApiResponseData.splice(index, 1)
  }
}

// 弹窗里的表单
const sqlApiResponseForm = reactive({
  name: '',
  mapName: ''
})
function showSqlApiRequest() {
  sqlApiResponse.value = true
  resetForm2()
}

// 点击确定时（参考我之前给你的逻辑）
const sqlApiResponseHandleOk = () => {
  if (!sqlApiResponseForm.name) {
    message.error('参数名称不能为空！')
    return
  }

  if (editingKey2.value) {
    // 编辑模式
    const exist = sqlApiResponseData.find((item) => item.name === sqlApiResponseForm.name && item.name !== editingKey2.value)
    if (exist) {
      message.error('参数名称已存在，不能重复！')
      return
    }

    const index = sqlApiResponseData.findIndex((item) => item.name === editingKey2.value)
    if (index !== -1) {
      sqlApiResponseData[index] = { ...sqlApiResponseForm }
    }
  } else {
    // 新增模式
    const exist = sqlApiResponseData.find((item) => item.name === sqlApiResponseForm.name)
    if (exist) {
      message.error('参数名称已存在！')
      return
    }
    sqlApiResponseData.push({ ...sqlApiResponseForm })
  }

  sqlApiResponse.value = false
  editingKey2.value = null // 记得重置
  resetForm2()
}

// 重置表单
const resetForm2 = () => {
  sqlApiResponseForm.name = ''
  sqlApiResponseForm.mapName = ''
}

//api
// 当前选中的 tab
const activeTab = ref('Header')

// 数据源：Header 和 Query 用不同数组
const headerData = reactive([])
const queryData = reactive([])
const formData = reactive([])
const wwwFormData = reactive([])

// 下拉类型选项
const typeOptions = ['字符串', '时间', '整形', '浮点']

// 添加行方法
function addRow(targetArray) {
  targetArray.push({
    key: Date.now(), // 唯一 key
    name: '',
    type: '',
    value: ''
  })
}

// 删除行方法
function deleteRow(targetArray, index) {
  targetArray.splice(index, 1)
}
const apiDataResponse = ref(false)
const apiDataResponseData = reactive([])
const editingKey4 = ref(null) // 当前正在编辑的行 paramName
// 表格列
const apiDataResponseColumns = [
  {
    title: '参数名称',
    dataIndex: 'name',
    key: 'name',
    width: '90px'
  },
  {
    title: '映射字段',
    dataIndex: 'mapName',
    key: 'mapName',
    width: '90px'
  },
  {
    title: '操作',
    key: 'action',
    customRender: ({ record }) => {
      return h(
        Space,
        {},
        {
          default: () => [
            // 编辑按钮
            h(
              Button,
              {
                type: 'link',
                onClick: () => handleEdit4(record)
              },
              { default: () => '编辑' }
            ),
            // 删除按钮 + 二次确认
            h(
              Popconfirm,
              {
                title: '确定删除吗？',
                onConfirm: () => handleDelete4(record.name)
              },
              {
                default: () => h(Button, { type: 'link', danger: true }, { default: () => '删除' })
              }
            )
          ]
        }
      )
    }
  }
]

const apiDataResponseForm = reactive({
  name: '',
  mapName: ''
})
// 点击编辑时
const handleEdit4 = (record) => {
  editingKey4.value = record.name // 记录正在编辑的行
  Object.assign(apiDataResponseForm, record) // 把数据填充到表单
  apiDataResponse.value = true // 打开弹框
}
// 删除
const handleDelete4 = (name) => {
  const index = apiDataResponseData.findIndex((item) => item.name === name)
  if (index !== -1) {
    apiDataResponseData.splice(index, 1)
  }
}

function showapiDataRequest() {
  apiDataResponse.value = true
  resetForm4()
}

// 点击确定时（参考我之前给你的逻辑）
const apiDataResponseHandleOk = () => {
  if (!apiDataResponseForm.name) {
    message.error('参数名称不能为空！')
    return
  }

  if (editingKey4.value) {
    // 编辑模式
    const exist = apiDataResponseData.find((item) => item.name === apiDataResponseForm.name && item.name !== editingKey4.value)
    if (exist) {
      message.error('参数名称已存在，不能重复！')
      return
    }

    const index = apiDataResponseData.findIndex((item) => item.name === editingKey4.value)
    if (index !== -1) {
      apiDataResponseData[index] = { ...apiDataResponseForm }
    }
  } else {
    // 新增模式
    const exist = apiDataResponseData.find((item) => item.name === apiDataResponseForm.name)
    if (exist) {
      message.error('参数名称已存在！')
      return
    }
    apiDataResponseData.push({ ...apiDataResponseForm })
  }

  apiDataResponse.value = false
  editingKey4.value = null // 记得重置
  resetForm4()
}

// 重置表单
const resetForm4 = () => {
  apiDataResponseForm.name = ''
  apiDataResponseForm.mapName = ''
}

const bodyType = ref('none')
</script>

<style lang="scss">
.form-fullscreen {
  width: 100%;
  height: 100%;
  background: #fff;
  padding: 10px;
  z-index: 1000;
  overflow-y: auto;

  .form-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16px;
    gap: 16px;
  }

  .form-actions {
    display: flex;
    align-items: center;
  }

  .steps-content {
    flex: 1;
    border: 1px dashed #e9e9e9;
    border-radius: 6px;
    height: 93%;
    text-align: center;
    padding: 10px;
  }

  [data-theme='dark'] .steps-content {
    background-color: #2f2f2f;
    border: 1px dashed #404040;
  }
  .form-wrapper {
    display: flex;
    justify-content: center; /* 水平居中 */
    height: 100%; /* 占满父容器高度 */
  }

  .form-wrapper form {
    min-width: 300px;
    max-width: 900px;
    width: 100%;
    background: #fff;
  }

  /* 数据配置 */

  .sdata {
    display: flex;
    height: 100%;
    .title-box {
      border: 1px solid gainsboro;
      border-bottom: 0px;
      border-top-left-radius: 6px;
      border-top-right-radius: 6px;
      display: flex;
      height: 20px;
    }
    .sdata-left {
      width: 80%;
      margin-right: 10px;
    }
    .sdata-right {
      border-radius: 6px;
      width: 20%;
      border: 1px solid gainsboro;
      
      .ant-table-placeholder {
        height: 570px;
      }
      .table-footer {
        // border-top: 1px dashed gainsboro;
        // border:1px solid red;
        margin-bottom: 0px;
      }
    }
  }

  /* sql */
  .sql {
    display: flex;
    height: 100%;

    .title-box {
      border: 1px solid gainsboro;
      border-bottom: 0px;
      border-top-left-radius: 6px;
      border-top-right-radius: 6px;
      display: flex;
      height: 20px;
    }
    .sql-left {
      width: 20%;
      margin-right: 10px;

      .db {
        padding: 10px;
        border: 1px solid gainsboro;
        border-radius: 6px;
        margin-top: 10px;
        height: calc(100% - 40px);
        overflow: auto;
      }
    }
    .sql-right {
      width: 60%;
      margin-right: 10px;

      .sql-editor {
        height: calc(100% - 20px);
      }
    }
    .right2 {
      border-radius: 6px;
      width: 25%;
      .top {
        height: 49%;
        margin-bottom: 13px;
        border: 1px solid gainsboro;
      }
      .bottom {
      display: block;
        border: 1px solid gainsboro;
        height: 49%;
      }
      .ant-table-placeholder {
        height: 221px;
      }
      .table-footer {
        border-top: 1px dashed gainsboro;
        margin-bottom: 0px;
      }
    }
  }

  .jsTips {
    flex-shrink: 0;
    padding: 8px 16px;
    background-color: #e6f7ff;
    border-radius: 4px;
    border-left: 5px solid #1890ff;
    margin-top: 10px;

    p {
      text-align: left;
      line-height: 24px;
      color: #909399;
    }
  }

  .api {
    height: 100%;
    display: flex;

    .left {
      height: 100%;
      border: 1px solid gainsboro;
      border-radius: 6px;
      width: 80%;
      // margin-right: 10px;

      .httpType {
        height: 70px;
        padding: 0 15px;
        display: flex;
        align-items: center; /* 垂直居中 */
      }
      .queryTitle {
        height: 40px;
        border-top: 1px solid gainsboro;
        border-bottom: 1px solid gainsboro;
        text-align: left;
        display: flex;
        padding-left: 10px;
        align-items: center; /* 垂直居中 */
      }
      .queryTags {
        height: calc(100% - 110px);
        .ant-table {
          height: 460px; /* 外层固定高度 */
          overflow-y: auto;
        }

        .table-footer {
          /* padding: 10px 0; */
          border-top: 1px solid #f0f0f0;
          text-align: center;
          background: #fff;
        }

        .bodyType {
          display: flex;
        }
        .bodyContent {
          margin-top: 10px;
          height: 460px;
          // border: 1px solid red;

          .ant-table {
            height: 424px; /* 外层固定高度 */
            overflow-y: auto;
          }

          .table-footer {
            /* padding: 10px 0; */
            border-top: 1px solid #f0f0f0;
            text-align: center;
            background: #fff;
          }
        }
      }
    }
    .right {
      border-radius: 6px;
      width: 20%;
      border: 1px solid gainsboro;
      
      .ant-table-placeholder {
        height: 570px;
      }
      .table-footer {
        // border-top: 1px dashed gainsboro;
        // border:1px solid red;
        margin-bottom: 0px;
      }
    }
  }
}
</style>
