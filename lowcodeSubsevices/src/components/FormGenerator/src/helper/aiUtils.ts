import { inputComponents, selectComponents, systemComponents, formConf as defaultFormConf } from '@/components/FormGenerator/src/helper/componentMap';
import { buildBitUUID } from '@/utils/uuid';
import { cloneDeep } from 'lodash-es';
import { dyOptionsList } from '@/components/FormGenerator/src/helper/config';

export function buildAiFields(aiModelList) {
  // 可支持组件
  const componentList = cloneDeep([...inputComponents, ...selectComponents, ...systemComponents]);
  // 处理组件字段
  const processField = (child, tableName?, isSubTable = false) => {
    const component = componentList.find(o => o.__config__.ceriKey === child.fieldComponent) || componentList[0];
    let field: any = {
      ...component,
      __config__: {
        ...component.__config__,
        label: child.fieldTitle,
        formId: `formItem${buildBitUUID()}`,
        renderKey: +new Date(),
        isSubTable,
      },
      __vModel__: child.fieldName,
    };
    if (isSubTable) field.__config__.parentVModel = tableName;
    if (dyOptionsList.includes(component.__config__.ceriKey)) field.options = child.fieldOptions || [];
    return field;
  };
  // 处理组件
  const handleItemModel = item => {
    if (item.isMain) {
      return item.fields.map(child => processField(child));
    } else {
      const tableComponent: any = componentList.find(o => o.__config__.ceriKey === 'table');
      const children = item.fields.map(child => processField(child, item.tableName, true));
      return {
        ...tableComponent,
        __config__: {
          ...tableComponent.__config__,
          label: item.tableTitle,
          children,
        },
        __vModel__: `tableField${buildBitUUID()}`,
      };
    }
  };
  return aiModelList.map(handleItemModel).flat() || [];
}
export function buildAiFormData(aiModelList) {
  // 表单默认数据
  const defaultDataForm = {
    id: '',
    fullName: '',
    enCode: '',
    type: 1,
    webType: 2,
    dbLinkId: '0',
    sortCode: 0,
    state: 1,
    category: '',
    description: '',
    interfaceId: '',
    interfaceName: '',
    interfaceParam: '',
    columnData: null,
    appColumnData: null,
    tables: '[]',
  };
  const formData = cloneDeep(defaultFormConf);
  formData.fields = buildAiFields(aiModelList) || [];
  return { ...defaultDataForm, formData: JSON.stringify(formData) };
}
