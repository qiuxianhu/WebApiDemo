本项目设计的知识点：
1、跨域。我们使用的是自定义的跨域。也可以使用微软提供的Cors
2、认证。我使用的是Auth2.0客户端模式。网上有很多关于Auth2.0的教程。如果你按照教程配置好了还没有起作用。你需要注意的是：
	（1）资源服务器和认证服务器的配置文件里面加上machineKey，其实machineKey主要为了负载均衡使用的。我们这里提一句machineKey能够被ASP.NET的识别，会影响项目中的加密解密。所以认证服务器生成token的时候会受到machineKey影响，所以资源服务器也要加上一样的machineKey。如果我们不加上machineKey，那么不同的服务器默认的machineKey就不一样了。
	<system.web>
   
		<machineKey validationKey="7D1ECBC712847FB1C2A31FEAA6EBC42D97265490A1309CF6434960699FD4EFD040E5B96B92E06F07024E5B1696CBD9635DAD047E3C0FE58F7B4662D87DADFA51" decryptionKey="6A9C3ED7EAC624E4FF988388627D723CB53C9542863F0B286A6482709BA7E141" validation="SHA1" decryption="AES" />
	</system.web>
	（2）资源服务器中加入了自定义的Startup.cs启动类，但是程序并没有走这个程序，其中一个原因是缺少Microsoft.Owin.Host.SystemWeb.dll

3、webapi传参类型和接收参数的方法。我们这里使用ModelBinder
		