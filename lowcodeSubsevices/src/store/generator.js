import { defineStore } from 'pinia'

export const useGeneratorStore = defineStore({
  id: 'app-generator',
  state: () => ({
    hasTable: false,
    subTable: [],
    allTable: [],
    formItemList: [],
    relationData: {},
    dynamicModelExtra: {},
  }),
  getters: {
    getHasTable() {
      return this.hasTable;
    },
    getAllTable() {
      return this.allTable;
    },
    getSubTable() {
      return this.subTable;
    },
    getFormItemList() {
      return this.formItemList;
    },
    getRelationData() {
      return this.relationData;
    },
    getDynamicModelExtra() {
      return this.dynamicModelExtra;
    },
  },
  actions: {
    setHasTable(hasTable = false) {
      this.hasTable = hasTable;
    },
    setAllTable(val) {
      this.allTable = val;
    },
    setSubTable(val) {
      this.subTable = val;
    },
    setFormItemList(val) {
      this.formItemList = val;
    },
    setRelationData(val) {
      this.relationData = val;
    },
    setDynamicModelExtra(val) {
      this.dynamicModelExtra = val;
    },
  },
});