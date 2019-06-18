using Bucket.Authorize.Abstractions;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.Authorize.MySql
{
    public class MySqlPermissionRepository : IPermissionRepository
    {
        private readonly string _dbConnectionString;
        private readonly string _projectName;

        public MySqlPermissionRepository(string dbConnectionString, string projectName)
        {
            _dbConnectionString = dbConnectionString;
            _projectName = projectName;
        }
        public List<Permission> Permissions { get; private set; } = new List<Permission>();
        public async Task Get()
        {
            using (var dbContext = new MySqlConnection(_dbConnectionString))
            {
                // 平台下所有需要认证Scope的接口
                var apiList = await dbContext.QueryAsync<ApiInfo>(@"SELECT api.Url,api.Method,roleapi.RoleId
                                                                    FROM tb_api_resources AS api 
                                                                    LEFT JOIN tb_role_apis AS roleapi ON api.Id = roleapi.ApiId
                                                                    WHERE AllowScope = 2 AND ProjectName = @ProjectName", new { ProjectName = _projectName });
                // 所有角色
                var roleList = await dbContext.QueryAsync<RoleInfo>(@"SELECT Id, `Key` from tb_roles WHERE IsDel=0", new { ProjectName = _projectName });
                if (apiList.Any())
                {
                    var permission = new List<Permission>();
                    var apiUrlList = apiList.GroupBy(it => it.Url).Select(it => it.FirstOrDefault()).ToList();
                    apiUrlList.ForEach(api =>
                    {
                        var apiMethodList = apiList.Where(it => it.Url == api.Url).GroupBy(it => it.Method).Select(it => it.FirstOrDefault()).ToList();
                        apiMethodList.ForEach(method =>
                        {
                            var apiInfo = apiList.Where(it => it.Url == api.Url && it.Method == method.Method).FirstOrDefault();
                            var roleids = apiList.Where(it => it.Url == api.Url && it.Method == method.Method).Select(it => it.RoleId).ToArray();
                            var scopes = roleList.Where(it => roleids.Contains(it.Id)).Select(it => it.Key).ToList();
                            permission.Add(new Permission
                            {
                                Path = apiInfo.Url,
                                Method = apiInfo.Method,
                                Scope = scopes
                            });
                        });
                    });
                    if (permission.Count > 0)
                        Permissions = permission;
                }
            }
        }
    }
}
