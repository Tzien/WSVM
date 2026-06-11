using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.Helper.DataEncryption.Encryptions
{
    /// <summary>
    /// MD5 加密
    /// </summary>
    public static unsafe class MD5Encryption
    {
        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="text">加密文本</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <param name="is16">是否输出 16 位</param>
        /// <returns></returns>
        public static string Encrypt(string text, bool uppercase = false, bool is16 = false)
        {
            return Encrypt(Encoding.UTF8.GetBytes(text), uppercase, is16);
        }
        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="uppercase">是否输出大写加密，默认 false</param>
        /// <param name="is16">是否输出 16 位</param>
        /// <returns></returns>
        public static string Encrypt(byte[] bytes, bool uppercase = false, bool is16 = false)
        {
            var data = MD5.HashData(bytes);

            var stringBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            var md5String = stringBuilder.ToString();
            var hash = !is16 ? md5String : md5String.Substring(8, 16);
            return !uppercase ? hash : hash.ToUpper();
        }
    }
}
