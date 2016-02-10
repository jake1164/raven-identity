===============================
CreativeColon.Raven.Identity
===============================
A RavenDB storage provider library for ASP.NET Identity

Guidelines to integrate RavenDB with ASP.NET MVC application

1. Create a new ASP.NET MVC project with the Authentication type set to Individual User Accounts

2. Remove Entity Framework packages & get RavenDB Identity by running these commands in the Package Manager Console

	Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
	Uninstall-Package EntityFramework
	Install-Package CreativeColon.Raven.Identity

3. Go ahead and delete these files

	App_Start/IdentityConfig.cs
	Models/IdentityModels.cs

4. Add these namespaces in the given files

	Namespaces
		using CreativeColon.Raven.Identity.Domain;
		using CreativeColon.Raven.Identity.Models;

	Files
		App_Start/Startup.Auth.cs
		Controllers/AccountController.cs
		Controllers/ManageController.cs
