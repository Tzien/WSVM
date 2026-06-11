/**
 * 将用户输入的连续单个数字合并为一个数
 * @param {Array} expressions - 记录计算表达式的数组
 * @returns {Array} 新的数组
 */
export const mergeNumberOfExps = expressions => {
  const res: any = [];
  const isNumChar = n => /^[\d|\.]$/.test(n);
  for (let i = 0; i < expressions.length; i++) {
    if (i > 0 && isNumChar(expressions[i - 1]) && isNumChar(expressions[i])) {
      res[res.length - 1] += expressions[i];
      continue;
    }
    res.push(expressions[i]);
  }
  return res;
};

export const validExp = (expressions, mergeNum = true) => {
  const temp = mergeNum ? mergeNumberOfExps(expressions) : expressions;
  const arr = temp.filter(t => !'()'.includes(t));
  // 去括号后 length应该为奇数  并且第一个字符和最后一个字符应该为数字而非计算符号
  if (temp.length % 2 === 0 || arr.length % 2 === 0 || Number.isNaN(+arr[0]) || Number.isNaN(+arr[arr.length - 1])) {
    return false;
  }
  for (let i = 0; i < arr.length - 1; i += 2) {
    if (typeof +arr[i] !== 'number' || !Number.isNaN(+arr[i + 1])) return false;
  }
  return true;
};