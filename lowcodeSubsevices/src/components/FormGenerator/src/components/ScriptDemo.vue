<template>
  <a-popover overlayClassName="script-demo-popover" @openChange="onOpenChange">
    <template #title>
      <div class="script-demo-popover-title">参数介绍及使用示例</div>
    </template>
    <template #content>
      <div class="script-demo-popover-main" :style="{ height: editorHeight }">
        <MonacoEditor :options="editorOptions" v-model="currentContent" />
      </div>
    </template>
    <span class="link-text">参数介绍及使用示例</span>
  </a-popover>
</template>

<script lang="ts" setup>
  import { reactive, toRefs } from 'vue';
  import { MonacoEditor } from '@/components/CodeEditor';

  interface State {
    currentContent: string;
    editorOptions: any;
    editorHeight: string;
  }

  const props = defineProps({
    type: {
      type: String,
      default: 'formField',
    },
  });
  const state = reactive<State>({
    currentContent: '',
    editorOptions: {
      readOnly: true,
      lineNumbers: 'off',
      minimap: { enabled: false },
    },
    editorHeight: '500px',
  });

  const { currentContent, editorOptions, editorHeight } = toRefs(state);
  const commonContent = `\n// onlineUtils--在线js工具类\n// 获取用户信息\nconst userInfo = onlineUtils.getUserInfo()\n// 获取设备信息\nconst deviceInfo = onlineUtils.getDeviceInfo()\n// 请求接口(url,method='get',data,headers)\nonlineUtils.request('url', 'get', { param: '1' }, { header: '1' })\n// 路由跳转(url,type) (type仅移动端支持)\nonlineUtils.route('url')\n// 消息提示(message,type='info',duration=3000)\nonlineUtils.toast('message', 'info', 3000) \n`;
  const formContent = `// formData--表单数据\nconsole.log(formData)\n// setFormData--设置表单某个组件数据(prop,value)\nsetFormData('key', 'value')\n// setShowOrHide--设置显示或隐藏(prop,value)\nsetShowOrHide('key', true)\n// setRequired--设置必填项(prop,value)\nsetRequired('key', true)\n// setDisabled--设置禁用项(prop,value)\nsetDisabled('key', true)`;
  const formFieldContent = `// data--当前组件的选中数据\nconsole.log(data)\n// rowIndex--当前行下标(仅在子表中可用)\nconsole.log(rowIndex)\n` + formContent;
  const btnEventContent = `// data--列表当前行数据/当前选中数据\nconsole.log(data)\n// index--列表当前行下标(仅按钮在列表中支持)\nconsole.log(index)\n// refresh--刷新列表\nrefresh()`;
  const childTableBtnContent = `// data--当前数据/当前选中数据\nconsole.log(data)`;
  const afterOnloadContent = `// data--列表行数据\nconsole.log(data)\n// tableRef--表格DOM元素\nconsole.log(tableRef)`;
  const rowStyleContent = `// row--列表行数据\n// rowIndex--列表行下标\n({ row, rowIndex }) => {\r\n   return {\r\n      background: rowIndex%2==0 ? 'red' : 'blue'\r\n   }\r\n}`;
  const cellStyleContent = `// row--列表行数据\n// column--列表列数据\n// rowIndex--列表行下标\n// columnIndex--列表列下标\n({ row, column, rowIndex, columnIndex }) => {\r\n    return {\r\n        color: rowIndex%2 == 0 ? 'blue' : 'red'\r\n    }\r\n}`;
  const btnEnableContent = `// row--列表行数据\n// rowIndex--列表行下标\n({ row, rowIndex, onlineUtils}) => {\r\n   return row.status==1\r\n}`;
  const exampleContent = `\n/**------------代码示例------------*/`;
  const interfaceExampleContent = `\n/*  1.js请求接口代码示例vue3  */\n var dataParam = {}; \n dataParam.tenantId = ''  \n dataParam.origin = 'preview';  \n var paramList = [];\n var codingParam = {  \n  id: '',  \n  field: 'bianma', \n  defaultValue: formData.bianma, \n  fieldName: '编号',  \n dataType: 'varchar',   \n required: 0,  \n }; \n paramList.push(codingParam);\n dataParam.paramList = paramList; \n onlineUtils.request('/api/system/DataInterface/484378895726936069/Actions/Preview', 'post', dataParam).then(res => { \n  if (res.data && res.data.length >= 1) { \n   console.log(res.data); \n   onlineUtils.toast('编号' + formData.bianma + '已经存在请重新输入', 'error', 2000); \n  } else {\n   console.log('编码' + formData.bianma + '不存在', res.data); \n   onlineUtils.toast('编号' + formData.bianma + '可正常输入', 'success', 2000); \n }\n });\n`;
  const crossDomainContent = `\n/*  2.js里面请求第三方接口，以避免跨域  */\n fetch('https://api.vvhan.com/api/hotlist/toutiao', { \n  method: 'get',\n }).then(response => response.json())\n  .then(data => console.log(data))\n.catch(error => console.error('Error:', error)); \n`;
  const getTonkenContent = `\n/*  3.如何获取TOKEN值  */ \n localStorage.getItem("token")\n`;
  const parameterTransmissionContent = `\n/*  4.如何实现页签间参数传值  */ \n localStorage.setItem('demo', hash); \n localStorage.getItem('demo');\n`;
  const subTableAssignDataContent = `\n/*  5.主表控件change事件写JS，给子表赋多行数据  */ \n console.log(data.id) \n \n let list = [ \n  {  \n   F_GoodsCode: '公司财务人员',  \n }, \n  { \n   F_GoodsCode: '开发部主管',  \n },   \n {  \n   F_GoodsCode: '角色测试',\n  }, \n ]\n \n console.log(list); \n setFormData('tableField102', list);\n`;
  const dateComponentContent = `\n/*  6.日期组件怎么定义默认值是当天  */ \n var a = new Date().toLocaleDateString(); //获取当前日期\n var nowdate = (new Date(a) / 1000) * 1000; //把当前日期变成时间戳\n console.log(nowdate);\n setFormData('f_orderdate', nowdate); //设置某组件日期\n setFormData('f_salesmanname', 'run');\n setFormData('f_ordercode', 'run123'); \n`;
  const stateContent = `\n/*  7.在onLoad事件中判断操作状态是：新增还是修改状态  */ \n  var a = formData.id;\n  if (a == null || a == '') { \n  console.log('新增状态'); \n  } else {\n   console.log('编辑状态'); \n  } \n`;
  const hourCountContent = `\n/*  8.计算开始日期和结束日期之间的小时数  */ \n  console.log(data); \n // var a = data \n //setFormData('comInputField103', a)\n var d = data; \n var time = formData.dateField101;\n console.log(time);\n if (time != null) {\n  if (d == null) {\n    setFormData('comInputField103', null);\n } else { \n    if (d >= time) { \n    var rangeDateNum = (d - time) / (1000 * 3600); \n   var formattedAmount = Math.round(rangeDateNum * 100) / 100;\n\n   console.log(formattedAmount);\n    console.log(rangeDateNum); \n   setFormData('f_leave_hour', formattedAmount); \n        } \n      } \n    }  \n`;
  const workflowBtnContent = `// flowInfo--流程信息\nconsole.log(flowInfo)\n// formData--表单数据\nconsole.log(formData)\n// taskInfo--任务节点信息\nconsole.log(taskInfo)\n`;

  function onOpenChange(val) {
    if (!val) return;
    let content =
      commonContent +
      exampleContent +
      interfaceExampleContent +
      crossDomainContent +
      getTonkenContent +
      parameterTransmissionContent +
      subTableAssignDataContent +
      dateComponentContent +
      stateContent +
      hourCountContent;
    if (props.type === 'form') {
      state.currentContent = formContent + content;
      state.editorHeight = '420px';
      return;
    }
    if (props.type === 'btnEvent') {
      state.currentContent = btnEventContent + content;
      state.editorHeight = '350px';
      return;
    }
    if (props.type === 'childTableBtn') {
      state.currentContent = childTableBtnContent + content;
      state.editorHeight = '270px';
      return;
    }
    if (props.type === 'afterOnload') {
      state.currentContent = afterOnloadContent + content;
      state.editorHeight = '310px';
      return;
    }
    if (props.type === 'rowStyle') {
      state.currentContent = rowStyleContent;
      state.editorHeight = '140px';
      return;
    }
    if (props.type === 'cellStyle') {
      state.currentContent = cellStyleContent;
      state.editorHeight = '180px';
      return;
    }
    if (props.type === 'btnEnableRule') {
      state.currentContent = btnEnableContent + content;
      state.editorHeight = '330px';
      return;
    }
    if (props.type === 'workflow') {
      state.currentContent = workflowBtnContent + content;
      state.editorHeight = '420px';
      return;
    }
    state.currentContent = formFieldContent + content;
  }
</script>

<style lang="less">
  .script-demo-popover {
    .script-demo-popover-title {
      padding: 6px 0;
      font-size: 16px;
    }
    .script-demo-popover-main {
      width: 430px;
    }
  }
</style>
