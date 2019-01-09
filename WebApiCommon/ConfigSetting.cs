namespace WebApiCommon
{
    class ConfigSetting
    {
        /// <summary>
        /// RSA私钥加密
        /// 
        /// &lt;add key = "RSAPrivateKey" value="MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAKEuQOkx20+KA84jdAjq+OWCQFKXdDvCMtvc84GWJUzSiUnYe09q637qkrvqe+9rjDhRgyP+ET7tUkNzAl148O7qdObGfqJKxSrcnqsMctOZ6GJmd98NNa+ka+/1uiq/si0J/eZXDsRjgTTnB+jb9lxW5XFztkcgw/uSjQdV128XAgMBAAECgYBTX4WcGnoDDrUhMB2Zb1IIBfQXxNgPAUkliYk8AtqQfmcdl6lRT1E5eUUlhwlMsyj5CND5ETcCgegHk7opd6HIYanxcbEnPhF3XXGd+GmMiPksQ9De9Jnz5bRdUPeNnUMnh2yBIAZjmUvlPzKljLz/y1+2ChC2NXTfw06GJwRDIQJBANEjoBIDehnWuJvk+QeRTf+hEuu/W+xgTaHMIPMiT/ydvjv1sjW7rm8X+BmrM2i2vPH0kYtUOur5iJ3YwlxQw3MCQQDFS7HZpUm56Zo5aHHBKWA/lfV04d/KZ+koG8iQ62qZY5kayVb8BfD0Q40B3KCsOpq1oE2ibye99/YFPXTPZmTNAkAFeDsNfY3J8zWszhY9Pm2dy+akx8Jtsi8VljMeaL1SzAVXqBtbEGeFfEj+0t1rVawnX9AWpsNLte7+wdaDd5FFAkBZ9IZVntg7rPgFfsqG8M+SjlQJA8eeqLwU7n4HHV9QKqovHWfMpwTSyk3rcGXNwTay4zEig53SLtF8WCX/PkdtAkEAoBtKgoM2Qe7UZjnNYMwRs6u7AoDRc2A7NO4YFHfnL0tIoJuWQs2nDylcRnwEw9HE/05QO66xVbVlxWChsmW/Hg==" /&gt;
        /// </summary>
        internal static readonly string RSAPrivateKey = null;
        /// <summary>
        /// RSA公钥解密
        /// 
        /// &lt;add key = "RSAPublicKey" value="MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQChLkDpMdtPigPOI3QI6vjlgkBSl3Q7wjLb3POBliVM0olJ2HtPaut+6pK76nvva4w4UYMj/hE+7VJDcwJdePDu6nTmxn6iSsUq3J6rDHLTmehiZnffDTWvpGvv9boqv7ItCf3mVw7EY4E05wfo2/ZcVuVxc7ZHIMP7ko0HVddvFwIDAQAB" /&gt;
        /// </summary>
        internal static readonly string RSAPublicKey = null;
        /// <summary>
        /// XXTEA加密解密key
        /// 
        /// &lt;add key = "XXTEAKey" value="tidebuy"/&gt;
        /// </summary>
        internal static readonly string XXTEAKey = null;
        /// <summary>
        /// AES加密解密Key，Key必须十六位。固定死了。wangyunpeng。
        /// </summary>
        internal static readonly string AESKey = "bixushi16weimima";


        static ConfigSetting()
        {
            //RSAPrivateKey = ConfigHelper.GetAppSettingValue("RSAPrivateKey");//JAVA
            //RSAPrivateKey = RSACryptoJavaAndCSharp.RSAPrivateKeyJava2DotNet(RSAPrivateKey);//转.NET XML
            //RSAPublicKey = ConfigHelper.GetAppSettingValue("RSAPublicKey");//JAVA
            //RSAPublicKey = RSACryptoJavaAndCSharp.RSAPublicKeyJava2DotNet(RSAPublicKey);//转.NET XML
            //XXTEAKey = ConfigHelper.GetAppSettingValue("XXTEAKey");//JAVA
        }
    }
}
