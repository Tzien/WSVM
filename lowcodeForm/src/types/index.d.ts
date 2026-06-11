declare type ElRef<T extends HTMLElement = HTMLDivElement> = Nullable<T>;

declare interface Fn<T = any, R = T> {
  (...arg: T[]): R;
}

declare interface ComponentElRef<T extends HTMLElement = HTMLDivElement> {
  $el: T;
}

declare type ComponentRef<T extends HTMLElement = HTMLDivElement> = ComponentElRef<T> | null;
declare type RefType<T> = T | null;
declare type EmitType = (event: string, ...args: any[]) => void;

declare type LabelValueOptions = {
  label: string;
  value: any;
  [key: string]: string | number | boolean;
}[];

declare type TargetContext = '_self' | '_blank';
