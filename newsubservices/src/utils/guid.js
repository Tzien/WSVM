const S4 = function () {
  return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
}
function _guid() {
  return (
    S4() +
    S4() +
    '-' +
    S4() +
    '-' +
    S4() +
    '-' +
    S4() +
    '-' +
    S4() +
    S4() +
    S4()
  )
}

function guid() {
  return S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4()
}

export { _guid, guid }
