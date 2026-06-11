import type { LockInfo, UserInfo, PermissionInfo } from '@/types/store'
import type { ProjectConfig } from '@/types/config'
import type { RouteLocationNormalized } from 'vue-router'
import { toRaw } from 'vue'
import { Memory } from './memory'

import { DEFAULT_CACHE_TIME } from '@/settings/encryptionSetting'
import { createLocalStorage, createSessionStorage } from '@/utils/cache'

import {
  TOKEN_KEY,
  USER_INFO_KEY,
  PERMISSIONS_KEY,
  LOCK_INFO_KEY,
  PROJ_CFG_KEY,
  APP_LOCAL_CACHE_KEY,
  APP_SESSION_CACHE_KEY,
  MULTIPLE_TABS_KEY
} from '@/enums/cacheEnum'

interface BasicStore {
  [LOCK_INFO_KEY]: LockInfo
  [PROJ_CFG_KEY]: ProjectConfig
  [MULTIPLE_TABS_KEY]: RouteLocationNormalized[]
}

type LocalStore = BasicStore
type SessionStore = BasicStore

type LocalKeys = keyof LocalStore
type SessionKeys = keyof SessionStore

const localMemory = new Memory(DEFAULT_CACHE_TIME)
const sessionMemory = new Memory(DEFAULT_CACHE_TIME)
const ls = createLocalStorage()
const ss = createSessionStorage()

export class Persistent {
  static getLocal<T>(key: LocalKeys) {
    return localMemory.get(key)?.value as Nullable<T>
  }

  static setLocal(key: LocalKeys, value: LocalStore[LocalKeys], immediate = false): void {
    localMemory.set(key, toRaw(value))
    immediate && ls.set(APP_LOCAL_CACHE_KEY, localMemory.getCache)
  }

  static removeLocal(key: LocalKeys, immediate = false): void {
    localMemory.remove(key)
    immediate && ls.set(APP_LOCAL_CACHE_KEY, localMemory.getCache)
  }

  static clearLocal(immediate = false): void {
    localMemory.clear()
    immediate && ls.clear()
  }

  static getSession<T>(key: SessionKeys) {
    return sessionMemory.get(key)?.value as Nullable<T>
  }

  static setSession(key: SessionKeys, value: SessionStore[SessionKeys], immediate = false): void {
    sessionMemory.set(key, toRaw(value))
    immediate && ss.set(APP_SESSION_CACHE_KEY, sessionMemory.getCache)
  }

  static removeSession(key: SessionKeys, immediate = false): void {
    sessionMemory.remove(key)
    immediate && ss.set(APP_SESSION_CACHE_KEY, sessionMemory.getCache)
  }
  static clearSession(immediate = false): void {
    sessionMemory.clear()
    immediate && ss.clear()
  }

  static clearAll(immediate = false) {
    sessionMemory.clear()
    localMemory.clear()
    if (immediate) {
      ls.clear()
      ss.clear()
    }
  }
}
