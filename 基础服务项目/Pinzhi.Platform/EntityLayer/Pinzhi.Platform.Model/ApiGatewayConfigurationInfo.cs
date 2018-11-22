using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinzhi.Platform.Model
{
    [SugarTable("tb_apigateway")]
    public class ApiGatewayConfigurationInfo
    {
        /// <summary>
        /// udcid 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { set; get; }
        public string ConfigurationKey { set; get; }
        public string Configuration { set; get; }
    }
}
