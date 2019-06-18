
## 全家桶之ORM,当前基于SqlSugar进行封装,支持仓储,多库等操作

1、参数配置,可以将SqlSugar的ConnectionConfig所以参数全部在配置文件里进行配置

```csharp
  "DbConfig": [
    {
      "Name": "localhost",
      "Default": true,
      "ConnectionString": "server=10.10.141.112;port=3306;database=test;uid=root;pwd=123123;characterset=utf8;",
      "DbType": "MySql",
      "IsAutoCloseConnection": false
    },
    {
      "Name": "localhost2",
      "Default": false,
      "ConnectionString": "server=10.10.141.111;port=3306;database=test;uid=root;pwd=123123;characterset=utf8;",
      "DbType": "MySql",
      "IsAutoCloseConnection": false
    }
  ]
```
2、 注入
```csharp
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      // 添加数据ORM、数据仓储
      familyBucket.AddSqlSugarDbContext().AddSqlSugarDbRepository();
  }
```


3、仓储使用
```csharp
   // 构造函数获取
   private readonly ISqlSugarDbRepository<UserModel> _userDbRepository;
   public ValuesController(ISqlSugarDbRepository<UserModel> userDbRepository)
   {
      _userDbRepository = userDbRepository;
   }
   // 多库切换,"log"是DbConfig里的Name参数
   _userDbRepository.UseDb("log")
```
```csharp
   // action方法获取
   Get([FromServices] ISqlSugarDbRepository<UserModel> userDbRepository, int id)
   {
     userDbRepository.GetFirst(it => it.Id == id);
   }
```
```csharp
   // HttpContext方法获取
   var userDbRepository = HttpContext.RequestServices.GetRequiredService<ISqlSugarDbRepository<UserModel>>();
   // 也可以写一个Controller基类来获取
   public ISqlSugarDbRepository<TEntity> GetDbRepository<TEntity>() where TEntity : class, new()
   {
       return HttpContext.RequestServices.GetRequiredService<ISqlSugarDbRepository<TEntity>>();
   }
```
4、SqlSugarClient使用方法
```csharp
   // 构造函数获取
   // 后来想想DbContext实现对原Orm改造太多就放弃了,直接改成多Client管理
   private readonly ISqlSugarDbContextFactory _sqlSugarDbContextFactory;
   public ValuesController(ISqlSugarDbContextFactory sqlSugarDbContextFactory)
   {
      _sqlSugarDbContextFactory = sqlSugarDbContextFactory;
   }
   // "log"是DbConfig里的Name参数,通过Get实现多库切换使用
   var logDbContext = _sqlSugarDbContextFactory.Get("log");
```
```csharp
   // 直接使用Client,获取配置里Default为True的默认第一条
   private readonly BucketSqlSugarClient _dbContext;
   public ValuesController(BucketSqlSugarClient dbContext)
   {
      _dbContext = dbContext;
   }
```