import type { VNode, VNodeChild, ComponentPublicInstance, FunctionalComponent, PropType as VuePropType } from 'vue'

export {}
declare global {
  type Nullable<T> = T | null
  type Recordable<T = any> = Record<string, T>
  type PropType<T> = VuePropType<T>
  type VueNode = VNodeChild | JSX.Element

  interface ChangeEvent extends Event {
    target: HTMLInputElement
  }

  namespace JSX {
    // tslint:disable no-empty-interface
    type Element = VNode
    // tslint:disable no-empty-interface
    type ElementClass = ComponentPublicInstance
    interface ElementAttributesProperty {
      $props: any
    }
    interface IntrinsicElements {
      [elem: string]: any
    }
    interface IntrinsicAttributes {
      [elem: string]: any
    }
  }

  type TimeoutHandle = ReturnType<typeof setTimeout>;
}
