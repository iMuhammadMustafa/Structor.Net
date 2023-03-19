# Structor.Net

This is a starter project for a .NET application. 

My aim is to make this modular and to follow best practices as much as possible. 

---

## Project Structure 
This projects follows clean architecture and vertical slice architecture. 

![ProjectStructure](ProjectStructure.png)

1. [Core](#core)
2. [Infrastructure](#infrastructure)
3. [Features](#features)
4. [Database](#database)

###  Core
This will be the entry point to the project bootstrapping and holding everything together. 

New Modules will: 
1. Add their services to the `CoreServicesCollection's AddModulesServices` method
2. Add their entities and domain models to the `DbContext` class as `DbSet`

CoreDbContext is instaciated with the QueryTrackingBehavior NoTracking. This greatly improves performance and it means on implementing an update method you must first reattach the entity to the context
Other Application DbContext could follow the same pattern by setting either .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) or set it up in the constructor _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking
Or keep the default behavior. 

### Infrastructure
This will contain shared services across the application.

### Features 
1. Each feature consists of multiple domains 
![Feature](Feature.png)
2. Each domain starts with an entity and the Repository and Services class for that Entity and a Controller if it needs one.
![Domain](Domain.png)
3. Each feature contains its own unit tests next to the class it tests. This might be frowned upon because the build will include the tests on production but I found a clever way to avoid this: 
    - In csproj I added the following: 
	```
	<ItemGroup Condition="'$(Configuration)' == 'Release'">
		<Compile Remove="**\*.Tests.cs" />
	</ItemGroup>
	<ItemGroup Condition="'$(Configuration)' != 'Release'">
		<PackageReference Include="coverlet.collector" Version="3.2.0">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
		<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.5.0" />
		<PackageReference Include="xunit" Version="2.4.2" />
		<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
	</ItemGroup>
	```
	The above ensures that in the Release configuration all the files named *.Tests.cs are excluded from compilation, and also that the required unit testing package references are removed.

### Database
Each Module will interact with the database using the Entity Framework DBContext either the main application context or a seperate context for that Feature
