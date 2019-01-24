using AutoMapper;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.Dto.Project;
using Pinzhi.Platform.Model;
using SqlSugar;
using System.Threading.Tasks;
using Pinzhi.Platform.Interface;
using Bucket.Listener.Abstractions;
using System;
using Bucket.Utility;
using Bucket.DbContext;

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
        private readonly BucketSqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        private readonly IPublishCommand _publishCommand;
        public ProjectBusiness(BucketSqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user,
            IPublishCommand publishCommand)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
            _publishCommand = publishCommand;
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
        /// <summary>
        /// 设置项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetProjectOutput> SetProject(SetProjectInput input)
        {
            var model = _mapper.Map<SetProjectInput, ProjectInfo>(input);
            if (model.Id > 0)
            {
                // 基础字段不容许更新
                model.LastTime = DateTime.Now;
                model.LastUid = _user.Id.ToLong();
                await _dbContext.Updateable(model)
                                .IgnoreColumns(it => new { it.CreateUid, it.CreateTime })
                                .ExecuteCommandAsync();
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.CreateUid = _user.Id.ToLong();
                model.LastTime = DateTime.Now;
                model.LastUid = model.CreateUid;
                await _dbContext.Insertable(model)
                                .ExecuteCommandAsync();
            }
            return new SetProjectOutput { };
        }
        /// <summary>
        /// 推送项目信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PublishCommandOutput> PublishCommand(PublishCommandInput input)
        {
            await _publishCommand.PublishCommandMessage(input.ProjectName, new Bucket.Values.NetworkCommand { CommandText = input.CommandText, NotifyComponent = input.CommandType });
            return new PublishCommandOutput { };
        }
    }
}
