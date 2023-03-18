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
1. Add their services to the `ServiceCollectionExtension's AddModulesServices` method
2. Add their entities and domain models to the `DbContext` class as `DbSet`

### Infrastructure
This will contain shared services across the application.

### Features 
1. Each feature consists of multiple domains 
![Feature](Feature.png)
2. Each domain starts with an entity and the Repository and Services class for that Entity and a Controller if it needs one.
![Domain](Domain.png)

### Database
Each Module will interact with the database using the Entity Framework DBContext either the main application context or a seperate context for that Feature
