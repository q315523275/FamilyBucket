using AutoMapper;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.DTO;
using Pinzhi.Platform.Model;
using SqlSugar;
using System.Threading.Tasks;
using Pinzhi.Platform.Interface;
namespace Pinzhi.Platform.Business
{
    /// <summary>
    /// 项目管理类
    /// </summary>
    public class ProjectBusiness: IProjectBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly SqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        public ProjectBusiness(SqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
        }
        /// <summary>
        /// 查看项目列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryProjectOutput> QueryProject()
        {
            var list = await _dbContext.Queryable<ProjectInfo>()
                                 .ToListAsync();
            return new QueryProjectOutput { Data = list };
        }
    }
}
