using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCommon
{
    /// <summary>
    /// AES加密解密-APP专用
    /// 
    /// 密码必须是16位，否则会报错哈
    /// </summary>
    public class AESCryptoHelper
    {
        /// <summary>
        /// APP默认加密Key（这里需要和APP加密KEY保持一致，来源于：TLZ.WebAPI.ConfigSetting.AESKey）
        /// </summary>
        public static string Default_AESKey = "bixushi16weimima";

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密码必须是16位，否则会报错哈</param>
        /// <returns>密文</returns>
        public static string Encrypt(string plainText, string key)
        {
            string result = null;
            if (string.IsNullOrEmpty(plainText))
            {
                return result;
            }
            byte[] plainTextArray = Encoding.UTF8.GetBytes(plainText);
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor())
                {
                    byte[] resultArray = cryptoTransform.TransformFinalBlock(plainTextArray, 0, plainTextArray.Length);
                    result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                    Array.Clear(resultArray, 0, resultArray.Length);
                    resultArray = null;
                }
            }
            Array.Clear(plainTextArray, 0, plainTextArray.Length);
            plainTextArray = null;
            return result;
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="encryptText">密文</param>
        /// <param name="key">密码必须是16位，否则会报错哈</param>
        /// <returns>明文</returns>
        public static string Decrypt(string encryptText, string key)
        {
            string result = null;
            if (string.IsNullOrEmpty(encryptText))
            {
                return result;
            }
            byte[] encryptTextArray = Convert.FromBase64String(encryptText);
            using (RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor())
                {
                    byte[] resultArray = cryptoTransform.TransformFinalBlock(encryptTextArray, 0, encryptTextArray.Length);
                    result = Encoding.UTF8.GetString(resultArray);
                    Array.Clear(resultArray, 0, resultArray.Length);
                    resultArray = null;
                }
            }
            Array.Clear(encryptTextArray, 0, encryptTextArray.Length);
            encryptTextArray = null;
            return result;
        }

        /// <summary>
        /// AES 加密(URL传参数加密)
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key"></param>
        /// <returns>加密之后URL编码</returns>
        public static string UrlEncrypt(string plainText, string key)
        {
            string encryptText = Encrypt(plainText, key);
            return HttpUtilityHelper.UrlEncode(encryptText);
        }

        /// <summary>
        /// AES 解密(URL传参数解密)
        /// </summary>
        /// <param name="encodeEncryptText">URL编码之后的密文</param>
        /// <param name="key"></param>
        /// <returns>明文</returns>
        public static string UrlDecrypt(string encodeEncryptText, string key)
        {
            if (!string.IsNullOrEmpty(encodeEncryptText))
            {
                string encodeText = HttpUtilityHelper.UrlDecode(encodeEncryptText);
                return Decrypt(encodeText, key);
            }
            return "";
        }
    }
}
