using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Structor.Infrastructure.Extentions;
using Structor.Infrastructure.Setup.Tests;
using Xunit;

namespace Structor.Infrastructure.Repositories;

public class Repository : IClassFixture<DbContextSetupFixture<TestDbContext>>
{
    private TestRepo _repo { get; set; }
    public Repository(DbContextSetupFixture<TestDbContext> dbContextSetupFixture)
    {
        TestDbContext _testDbContext = dbContextSetupFixture.CreateContextForSQLiteInMemory();
        
        var _logger = Substitute.For<ILogger<TestRepo>>();

        _repo = new TestRepo(_testDbContext, _logger);
        // If you want to test the abstract repository itself not the implementer of it then: 
        //_repo = Substitute.ForPartsOf<Repository<TestEntity, TestDbContext>>(_testDbContext, _logger);
        //private IRepository<TestEntity> _repo { get; set; }
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
        var count = await _repo.Count();
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
        var res = await _repo.Count();

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
        var res = await _repo.CountWhere(entity => entity.Id > 5);

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

public class TestRepo : Repository<TestEntity, TestDbContext>
{
    public TestRepo(TestDbContext context, ILogger<TestRepo> logger) : base(context, logger)
    {
    }
}