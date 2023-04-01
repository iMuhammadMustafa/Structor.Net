using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Structor.Net.Core.DatabaseContext;
using Structor.Net.Core.Extentions;
using Structor.Net.Core.Setup.Tests;
using Structor.Net.Infrastructure.Entities;
using Xunit;

namespace Structor.Net.Infrastructure.Repositories;

public class Repository : IClassFixture<SqliteDbContextSetupFixture<CoreDbContext>>
{

    private CoreDbContext _coreDbContext { get; set; }
    private Repository<TestEntity, CoreDbContext> _repo { get; set; }

    public Repository(SqliteDbContextSetupFixture<CoreDbContext> sqliteDbContextSetupFixture)
    {
        _coreDbContext = sqliteDbContextSetupFixture.CreateContextForSQLiteInMemory();
        var mock = new Mock<Repository<TestEntity, CoreDbContext>>(_coreDbContext)
        {
            CallBase = true
        };
        _repo = mock.Object;
    }


    [Fact]
    public async Task Should_Insert()
    {
        TestEntity newEntity = new TestEntity();

        var res = await _repo.Insert(newEntity, true);

        Assert.NotNull(res);
        Assert.Equal(res.Guid, newEntity.Guid);
    }
    [Fact]
    public async Task Should_Update()
    {
        Guid newGuid = Guid.NewGuid();
        TestEntity newEntity = new TestEntity();
        await _repo.Insert(newEntity, true);


        newEntity.Guid = newGuid;
        var res = await _repo.Update(newEntity, true);


        Assert.True(res);
        Assert.Equal(newGuid, newEntity.Guid);
    }
    [Fact]
    public async Task Should_Delete()
    {
        TestEntity newEntity = new TestEntity();
        var createdEntity = await _repo.Insert(newEntity, true);

        var deleteRes = await _repo.Delete(createdEntity, true);
        var count = await _repo.GetCount();
        var entity = await _repo.GetById(createdEntity.Id);

        Assert.True(deleteRes);
        Assert.Equal(0, count);
        Assert.Null(entity);
    }


    [Fact]
    public async Task Should_ReturnCount()
    {
        //Arrange 
        await _repo.Insert(new TestEntity());
        await _repo.Insert(new TestEntity());
        await _repo.Insert(new TestEntity());
        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetCount();

        //Assert
        Assert.NotEqual(0, res);
        Assert.Equal(3, res);

    }

    [Fact]
    public async Task Should_ReturnAllElements()
    {
        //Arrange 
        await _repo.Insert(new TestEntity());
        await _repo.Insert(new TestEntity());
        await _repo.Insert(new TestEntity());

        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetAll().ToListAsync();

        //Assert
        Assert.NotNull(res);
        Assert.Equal(3, res.Count);
    }

    [Fact]
    public async Task Should_GetByIdd()
    {
        var entity = new TestEntity()
        {
            Id = 5
        };
        //Arrange 
        await _repo.Insert(entity, true);

        //Act 
        var res = await _repo.GetById(entity.Id);

        //Assert
        Assert.NotNull(res);
        Assert.Equal(res.Guid, entity.Guid);
        Assert.Equivalent(res, entity);
    }
    [Fact]
    public async Task Should_GetByGuid()
    {
        var entity = new TestEntity()
        {
            Id = 5
        };
        //Arrange 
        await _repo.Insert(entity, true);

        //Act 
        var res = await _repo.GetByGuid(entity.Guid);

        //Assert
        Assert.NotNull(res);
        Assert.Equal(res.Id, entity.Id);
        Assert.Equal(res.Guid, entity.Guid);
        Assert.Equivalent(res, entity);
    }


    [Fact]
    public async Task Should_GetCountWhere()
    {
        //Arrange 
        await _repo.Insert(new TestEntity()
        {
            Id = 1
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 10
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 20
        });

        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetCountWhere(entity => entity.Id > 5);

        //Assert
        Assert.NotEqual(0, res);
        Assert.Equal(2, res);
    }
    [Fact]
    public async Task Should_Paginate()
    {
        //Arrange 
        await _repo.Insert(new TestEntity()
        {
            Id = 1
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 19
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 10
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 20
        });

        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetAll().Paginate(1, 2).ToListAsync();

        //Assert
        Assert.NotEmpty(res);
        Assert.Equal(2, res.Count);
    }

    [Fact]
    public async Task Should_OrderByProperty()
    {
        //Arrange 
        await _repo.Insert(new TestEntity()
        {
            Id = 1
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 19
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 10
        });
        var result = await _repo.Insert(new TestEntity()
        {
            Id = 20
        });

        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetAll().OrderQueryable(x => x.Id, true).ToListAsync();

        //Assert
        Assert.NotEmpty(res);
        Assert.NotNull(res.FirstOrDefault());
        Assert.Equal(result.Guid, res.FirstOrDefault()!.Guid);
    }


    [Fact]
    public async Task Should_OrderedPaginated()
    {
        //Arrange 
        await _repo.Insert(new TestEntity()
        {
            Id = 1
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 19
        });
        var result = await _repo.Insert(new TestEntity()
        {
            Id = 10
        });
        await _repo.Insert(new TestEntity()
        {
            Id = 20
        });

        await _repo.SaveChanges();

        //Act 
        var res = await _repo.GetAll().OrderQueryable(x => x.Id, true).Paginate(2, 2).ToListAsync();

        //Assert
        Assert.NotEmpty(res);
        Assert.Equal(2, res.Count);
        Assert.NotNull(res.FirstOrDefault());
        Assert.Equal(result.Guid, res.FirstOrDefault()!.Guid);
    }








}
