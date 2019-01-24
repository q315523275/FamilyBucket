using AutoMapper;
using Bucket.Exceptions;
using Bucket.Core;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Interface;
using Pinzhi.Platform.Model;
using SqlSugar;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bucket.Listener.Abstractions;
using Pinzhi.Platform.Dto.Config;
using Bucket.DbContext;

namespace Pinzhi.Platform.Business
{
    public class ConfigBusiness : IConfigBusiness
    {
        private readonly BucketSqlSugarClient _dbContext;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        private readonly IPublishCommand _publishCommand;
        public ConfigBusiness(BucketSqlSugarClient dbContext,
            IMapper mapper,
            IJsonHelper jsonHelper,
            IUser user,
            IPublishCommand publishCommand)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _jsonHelper = jsonHelper;
            _user = user;
            _publishCommand = publishCommand;
        }

        public async Task<QueryAppConfigListOutput> QueryAppConfigList(QueryAppConfigListInput input)
        {
            var totalNumber = 0;
            // 环境库
            Enum.TryParse<Env>(input.Environment, out var env);
            var tableName = string.Empty;
            switch (env)
            {
                case Env.dev:
                    tableName = $"tb_appconfig_{Env.dev.ToString()}";
                    break;
                case Env.pro:
                    tableName = $"tb_appconfig_{Env.pro.ToString()}";
                    break;
                case Env.prepro:
                    tableName = $"tb_appconfig_{Env.prepro.ToString()}";
                    break;
                case Env.uat:
                    tableName = $"tb_appconfig_{Env.uat.ToString()}";
                    break;
                default:
                    throw new BucketException("plm_001", "环境不存在");
            }
            // 执行
            var result = await _dbContext.Queryable<AppConfigInfo>().AS(tableName)
                                .WhereIF(!string.IsNullOrWhiteSpace(input.AppId), it => it.ConfigAppId == input.AppId)
                                .WhereIF(!string.IsNullOrWhiteSpace(input.NameSpace), it => it.ConfigNamespaceName == input.NameSpace)
                                .ToPageListAsync(input.PageIndex, input.PageSize, totalNumber);
            return new QueryAppConfigListOutput { Data = result.Key, CurrentPage = input.PageIndex, Total = result.Value };
        }

        public async Task<QueryAppListOutput> QueryAppList()
        {
            var list = await _dbContext.Queryable<AppInfo>()
                                 .ToListAsync();
            return new QueryAppListOutput { Data = list };
        }

        public async Task<QueryAppProjectListOutput> QueryAppProjectList(QueryAppProjectListInput input)
        {
            var totalNumber = 0;
            var result = await _dbContext.Queryable<AppNamespaceInfo>()
                                .WhereIF(!string.IsNullOrWhiteSpace(input.AppId), it => it.AppId == input.AppId)
                                .WhereIF(input.IsPublic == 1, it => it.IsPublic == true)
                                .WhereIF(input.IsPublic == 0, it => it.IsPublic == false)
                                .ToPageListAsync(input.PageIndex, input.PageSize, totalNumber);
            return new QueryAppProjectListOutput { Data = result.Key, CurrentPage = input.PageIndex, Total = totalNumber };
        }

        public async Task<SetAppConfigInfoOutput> SetAppConfigInfo(SetAppConfigInfoInput input)
        {
            // 环境库
            Enum.TryParse<Env>(input.Environment, out var env);
            var tableName = "tb_appconfig_test";
            switch (env)
            {
                case Env.dev:
                    tableName = $"tb_appconfig_{Env.dev.ToString()}";
                    break;
                case Env.pro:
                    tableName = $"tb_appconfig_{Env.pro.ToString()}";
                    break;
                case Env.prepro:
                    tableName = $"tb_appconfig_{Env.prepro.ToString()}";
                    break;
                case Env.uat:
                    tableName = $"tb_appconfig_{Env.uat.ToString()}";
                    break;
                default:
                    throw new BucketException("plm_001", "环境不存在");
            }
            // 编辑或新增
            var model = _mapper.Map<AppConfigInfo>(input);
            if (model.Id > 0)
            {
                // 基础字段不容许更新
                model.LastTime = DateTime.Now;
                model.Version = _dbContext.Queryable<AppConfigInfo>().AS(tableName).Max(it => it.Version) + 1;
                await _dbContext.Updateable(model).AS(tableName)
                                .IgnoreColumns(it => new { it.CreateTime })
                                .ExecuteCommandAsync();
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.LastTime = DateTime.Now;
                model.Version = _dbContext.Queryable<AppConfigInfo>().AS(tableName).Max(it => it.Version) + 1;
                await _dbContext.Insertable(model).AS(tableName)
                                .ExecuteCommandAsync();
            }
            return new SetAppConfigInfoOutput { };
        }

        public async Task<SetAppInfoOutput> SetAppInfo(SetAppInfoInput input)
        {
            var model = _mapper.Map<SetAppInfoInput, AppInfo>(input);
            if (model.Id > 0)
            {
                // 基础字段不容许更新
                await _dbContext.Updateable(model)
                                .ExecuteCommandAsync();
            }
            else
            {
                await _dbContext.Insertable(model)
                                .ExecuteCommandAsync();
            }
            return new SetAppInfoOutput { };
        }

        public async Task<SetAppProjectInfoOutput> SetAppProjectInfo(SetAppProjectInfoInput input)
        {
            var model = _mapper.Map<SetAppProjectInfoInput, AppNamespaceInfo>(input);
            if (model.Id > 0)
            {
                // 基础字段不容许更新
                model.LastTime = DateTime.Now;
                model.LastUid = Convert.ToInt64(_user.Id);
                await _dbContext.Updateable(model)
                                .IgnoreColumns(it => new { it.CreateUid, it.CreateTime })
                                .ExecuteCommandAsync();
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.CreateUid = Convert.ToInt64(_user.Id);
                model.LastTime = DateTime.Now;
                model.LastUid = model.CreateUid;
                await _dbContext.Insertable(model)
                                .ExecuteCommandAsync();
            }
            return new SetAppProjectInfoOutput { };
        }

        public async Task<PublishCommandOutput> PublishCommand(PublishCommandInput input)
        {
            await _publishCommand.PublishCommandMessage(input.ProjectName, new Bucket.Values.NetworkCommand { CommandText = input.CommandText, NotifyComponent = input.CommandType });
            return new PublishCommandOutput { };
        }
    }
}
