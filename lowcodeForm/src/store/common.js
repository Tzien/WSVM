import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useCommonStore = defineStore(
  'common',
  () => {
    const dataOption = ref('')
    const dictOptions = ref([])
    const signatureImg=ref('')
    const setDataOption = (dataOption) => {
      dataOption.value = dataOption
    }
    const setDictOptions = (a) => {
      dictOptions.value = a
    }
    const setSignatureImg = (a) => {
      signatureImg.value = a
    }
    return {
      dataOption,
      dictOptions,
      signatureImg,
      setDataOption,
      setDictOptions,
      setSignatureImg
    }
  },
  {
    persist: true
  }
)