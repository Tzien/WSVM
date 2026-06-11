const S4 = function () {
  return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
}
function _guid() {
  return S4() + S4() + '-' + S4() + '-' + S4() + '-' + S4() + '-' + S4() + S4() + S4()
}

function guid() {
  return S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4()
}

function buildBitGUID(length = 6) {
  return guid().substring(0, length)
}

let unique = 0
function buildShortGUID(prefix = '') {
  const time = Date.now()
  const random = Math.floor(Math.random() * 1000000000)
  unique++
  return prefix + '_' + random + unique + String(time)
}

export { _guid, guid, buildBitGUID, buildShortGUID }
