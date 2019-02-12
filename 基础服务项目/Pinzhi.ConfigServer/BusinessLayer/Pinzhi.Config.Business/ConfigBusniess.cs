using Pinzhi.Config.DTO;
using Pinzhi.Config.Model;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bucket.Exceptions;
using Bucket.Utility;
using Pinzhi.Config.Interface;
using System;
using Bucket.DbContext;

namespace Pinzhi.Config.Business
{
    public class ConfigBusniess : IConfigBusniess
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly BucketSqlSugarClient _dbContext;
        public ConfigBusniess(BucketSqlSugarClient dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 查询项目配置信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryConfigOutput> QueryConfig(QueryConfigInput input)
        {
            // 验证
            if (input.AppId.IsEmpty())
                throw new BucketException("config_001", "AppId不能为空");
            if (input.Sign.IsEmpty())
                throw new BucketException("config_002", "签名不能为空");
            if (input.NamespaceName.IsEmpty())
                throw new BucketException("config_005", "NamespaceName不能为空");
            // 返回结果
            var result = new QueryConfigOutput() { KV = new Dictionary<string, string>(), Version = input.Version };
            // 项目与签名验证
            var project = await _dbContext.Queryable<AppInfo>().Where(it => it.AppId == input.AppId).FirstAsync();
            if (project == null)
                throw new BucketException("config_003", "项目不存在");
            var signstr = $"appId={project.AppId}&appSecret={project.Secret}&namespaceName={input.NamespaceName}";
            var sign = Bucket.Utility.Helpers.Encrypt.SHA256(signstr);
            if (sign.ToLower() != input.Sign)
                throw new BucketException("config_004", "签名错误");
            // 环境库
            Enum.TryParse<Env>(input.Env.SafeString().ToLower(), out var env);
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
                    throw new BucketException("config_06", "环境不存在");
            }
            // 配置查询
            var namespaceList = await _dbContext.Queryable<AppNamespaceInfo>()
                                                .Where(it => it.AppId == project.AppId && it.IsDeleted == false && it.IsPublic == true)
                                                .Select(it => new { it.Name })
                                                .ToListAsync();
            var namespaceKeyList = namespaceList.Select(it => it.Name).ToList();
            namespaceKeyList.Add(input.NamespaceName);
            if (namespaceKeyList.Count > 0)
            {
                var config = await _dbContext.Queryable<AppConfigInfo>().AS(tableName)
                                             .Where(it => it.ConfigAppId == project.AppId && it.IsDeleted == false && namespaceKeyList.Contains(it.ConfigNamespaceName) && it.Version > input.Version)
                                             .ToListAsync();
                if (config.Count > 0)
                {
                    result.Version = config.Max(it => it.Version);
                    config.ForEach(p =>
                    {
                        result.KV.Add(p.ConfigKey, p.ConfigValue);
                    });
                }
            }
            result.AppName = project.Name;
            return result;
        }
    }
}
