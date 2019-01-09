Auth2.0 客户端模式需要注意的事项，大体流程原理网上都有，这里介绍一下，为什么按照原理做了，还有问题的情况
1、资源服务器要加上验证限制[Authorize]，你想想使用就是Auth2.0就是加认证，如果资源不加[Authorize]，还使用Auth2.0干嘛
2、客户端的string clientId = "123456"; string secret = "qiuxianhu";要和认证服务器中方法ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)的        
                if (clientId == "123456" && clientSecret == "qiuxianhu")保持一致，不然会报错{"error":"invalid_client"}。想想也就明白为什么了。
3、感觉什么都配置好了，都没问题了，但是请求资源的时候居然报"Message": "已拒绝为此请求授权。"    我这里的建议是在认证服务器和资源服务器的配置文件中加上machineKey
	<system.web>		
		<machineKey validationKey="7D1ECBC712847FB1C2A31FEAA6EBC42D97265490A1309CF6434960699FD4EFD040E5B96B92E06F07024E5B1696CBD9635DAD047E3C0FE58F7B4662D87DADFA51" decryptionKey="6A9C3ED7EAC624E4FF988388627D723CB53C9542863F0B286A6482709BA7E141" validation="SHA1" decryption="AES" />
	</system.web>

	为什么要加machineKey呢？其实呢主要是负载均衡的时候会用到。有兴趣的可以了解一下machineKey的作用。我们这里提一句：machineKey会被ASP.NET识别，项目中的加密解密之类的东西会受到machineKey的影响。我们在认证服务器生成的token或许和machineKey有关系（我一点我没有去验证）。所以在资源服务器中解析token的时候也要统一，所以两边就要加上同样的machineKey。
