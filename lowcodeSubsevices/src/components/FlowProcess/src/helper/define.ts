const defaultStep = [
  { id: 0, fullName: '流程发起' },
  { id: 1, fullName: '上级审批节点' },
  { id: 2, fullName: '自选审批节点' },
];
const typeOptions = [
  { fullName: '指定成员', id: 6 },
  { fullName: '发起者本人', id: 3 },
  { fullName: '直属主管', id: 1 },
  { fullName: '部门主管', id: 2 },
  { fullName: '表单变量', id: 4 },
  { fullName: '流程环节', id: 5 },
  { fullName: '数据接口', id: 9 },
  { fullName: '候选人员', id: 7 },
];
const overTimeOptions = [
  { id: 0, fullName: '接收时间' },
  { id: 1, fullName: '发起时间' },
  { id: 2, fullName: '表单变量' },
];
const overTimeMsgOptions = [
  { id: 1, fullName: '自定义' },
  { id: 0, fullName: '关闭' },
];
const nodeOverTimeMsgOptions = [{ id: 2, fullName: '同步发起配置' }, ...overTimeMsgOptions];
const noticeOptions = [{ id: 3, fullName: '默认' }, ...overTimeMsgOptions];
const nodeNoticeOptions = [{ id: 2, fullName: '同步发起配置' }, ...noticeOptions];
const systemFieldOptions = [
  { id: '@flowId', fullName: '流程ID' },
  { id: '@taskId', fullName: '任务ID' },
  { id: '@taskNodeId', fullName: '节点ID' },
  { id: '@flowFullName', fullName: '流程名称' },
  { id: '@taskFullName', fullName: '任务标题' },
  { id: '@launchUserId', fullName: '发起用户ID' },
  { id: '@launchUserName', fullName: '发起用户名' },
  { id: '@flowOperatorUserId', fullName: '当前操作用户ID' },
  { id: '@flowOperatorUserName', fullName: '当前操作用户名' },
];
const sourceTypeOptions = [
  { id: 1, fullName: '字段' },
  { id: 2, fullName: '自定义' },
  { id: 4, fullName: '系统变量' },
  { id: 3, fullName: '为空' },
];
const simpleSourceTypeOptions = [
  { id: 1, fullName: '字段' },
  { id: 2, fullName: '自定义' },
];
const interfaceSystemOptions = [
  { id: '@formId', fullName: '@表单ID' },
  { id: '@userId', fullName: '@当前用户' },
  { id: '@userAndSubordinates', fullName: '@当前用户及下属' },
  { id: '@organizeId', fullName: '@当前组织' },
  { id: '@organizationAndSuborganization', fullName: '@当前组织及子组织' },
  { id: '@branchManageOrganize', fullName: '@当前分管组织' },
];
const templateJsonColumns = [
  { width: 50, title: '序号', align: 'center', customRender: ({ index }) => index + 1 },
  { title: '参数名称', dataIndex: 'field', key: 'field' },
  { title: '参数来源', dataIndex: 'sourceType', key: 'sourceType', width: 100 },
  { title: '参数值', dataIndex: 'relationField', key: 'relationField', width: 220 },
];
const printConditionTypeOptions = [
  { fullName: '不限制', id: 1 },
  { fullName: '节点结束', id: 2 },
  { fullName: '流程结束', id: 3 },
  { fullName: '条件设置', id: 4 },
];
const errorRuleOptions = [
  { fullName: '超级管理员', id: 1 },
  { fullName: '指定人员', id: 2 },
  { fullName: '上一节点审批人指定', id: 3 },
  { fullName: '默认审批通过', id: 4 },
  { fullName: '无法提交', id: 5 },
];
const subFlowErrorRuleOptions = [
  { id: 1, fullName: '超级管理员' },
  { id: 2, fullName: '指定人员' },
  { id: 6, fullName: '发起者本人' },
];
const formFieldTypeOptions = [
  { fullName: '用户', id: 1 },
  { fullName: '部门', id: 2 },
  { fullName: '岗位', id: 3 },
  { fullName: '角色', id: 4 },
  { fullName: '分组', id: 5 },
];
const conditionTypeOptions = [
  { id: 1, fullName: '字段' },
  { id: 3, fullName: '公式' },
];
const conditionTypeOptions1 = [
  { id: 1, fullName: '字段' },
  { id: 2, fullName: '自定义' },
  { id: 3, fullName: '系统参数' },
  { id: 4, fullName: '全局变量' },
];
const symbolOptions = [
  { id: '>=', fullName: '大于等于' },
  { id: '>', fullName: '大于' },
  { id: '==', fullName: '等于' },
  { id: '<=', fullName: '小于等于' },
  { id: '<', fullName: '小于' },
  { id: '<>', fullName: '不等于' },
  { id: 'like', fullName: '包含' },
  { id: 'notLike', fullName: '不包含' },
];
const logicOptions = [
  { id: 'and', fullName: '且' },
  { id: 'or', fullName: '或' },
];
const divideRuleOptions = [
  { fullName: '根据条件多分支流转（包容网关）', id: 'inclusion' },
  { fullName: '根据条件单分支流转（排它网关）', id: 'exclusive' },
  { fullName: '所有分支都流转（并行网关）', id: 'parallel' },
  { fullName: '由用户选择一条分支流转（选择分支）', id: 'choose' },
];
const keyMap = {
  approveFuncConfig: 'approveFuncConfig',
  rejectFuncConfig: 'rejectFuncConfig',
  backFuncConfig: 'backFuncConfig',
  recallFuncConfig: 'recallFuncConfig',
  overTimeFuncConfig: 'overTimeFuncConfig',
  noticeFuncConfig: 'noticeFuncConfig',
  initFuncConfig: 'initFuncConfig',
  endFuncConfig: 'endFuncConfig',
  flowRecallFuncConfig: 'flowRecallFuncConfig',
};
const durationList = [
  { id: 30, fullName: '30分钟' },
  { id: 60, fullName: '1小时' },
  { id: 90, fullName: '1小时30分钟' },
  { id: 120, fullName: '2小时' },
  { id: 180, fullName: '3小时' },
  { id: -1, fullName: '自定义' },
];
const urgentList = [
  { id: '1', fullName: '普通' },
  { id: '2', fullName: '重要' },
  { id: '3', fullName: '紧急' },
];
const tableList = [
  { id: 1, fullName: '此日程' },
  { id: 2, fullName: '此日程及后续' },
];
const deleteList = [
  { id: 1, fullName: '此日程' },
  { id: 2, fullName: '此日程及后续' },
  { id: 3, fullName: '所有日程' },
];
const reminderTimeList = [
  { id: -2, fullName: '不提醒' },
  { id: -1, fullName: '开始时' },
  { id: 5, fullName: '提前5分钟' },
  { id: 10, fullName: '提前10分钟' },
  { id: 15, fullName: '提前15分钟' },
  { id: 30, fullName: '提前30分钟' },
  { id: 60, fullName: '提前1小时' },
  { id: 120, fullName: '提前2小时' },
  { id: 1440, fullName: '1天前' },
  { id: 2880, fullName: '2天前' },
  { id: 10080, fullName: '1周前' },
];
const reminderTimeList_ = [
  { id: -2, fullName: '不提醒' },
  { id: 1, fullName: '当天8:00' },
  { id: 2, fullName: '当天9:00' },
  { id: 3, fullName: '当天10:00' },
  { id: 4, fullName: '1天前8:00' },
  { id: 5, fullName: '1天前9:00' },
  { id: 6, fullName: '1天前10:00' },
  { id: 7, fullName: '2天前8:00' },
  { id: 8, fullName: '2天前9:00' },
  { id: 9, fullName: '2天前10:00' },
  { id: 10, fullName: '1周前8:00' },
  { id: 11, fullName: '1周前9:00' },
  { id: 12, fullName: '1周前10:00' },
];
const remindList = [
  { id: 1, fullName: '默认' },
  { id: 2, fullName: '自定义' },
];
const repeatReminderList = [
  { id: 1, fullName: '不重复' },
  { id: 2, fullName: '每天重复' },
  { id: 3, fullName: '每周重复' },
  { id: 4, fullName: '每月重复' },
  { id: 5, fullName: '每年重复' },
];
const notSupportList = [
  'relationFormAttr',
  'popupAttr',
  'uploadFile',
  'uploadImg',
  'colorPicker',
  'editor',
  'link',
  'button',
  'text',
  'alert',
  'table',
  'sign',
  'signature',
];

export {
  defaultStep,
  typeOptions,
  overTimeOptions,
  overTimeMsgOptions,
  nodeOverTimeMsgOptions,
  noticeOptions,
  nodeNoticeOptions,
  systemFieldOptions,
  sourceTypeOptions,
  simpleSourceTypeOptions,
  interfaceSystemOptions,
  templateJsonColumns,
  printConditionTypeOptions,
  errorRuleOptions,
  subFlowErrorRuleOptions,
  formFieldTypeOptions,
  conditionTypeOptions,
  conditionTypeOptions1,
  symbolOptions,
  logicOptions,
  divideRuleOptions,
  keyMap,
  durationList,
  urgentList,
  tableList,
  deleteList,
  reminderTimeList,
  reminderTimeList_,
  remindList,
  repeatReminderList,
  notSupportList,
};
