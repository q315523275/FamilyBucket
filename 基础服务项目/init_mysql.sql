/*
Navicat MySQL Data Transfer

Source Server         : 192.168.1.168
Source Server Version : 50710
Source Host           : 192.168.1.168:3306
Source Database       : pzsupper

Target Server Type    : MYSQL
Target Server Version : 50710
File Encoding         : 65001

Date: 2019-02-12 10:59:50
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for tb_api_resources
-- ----------------------------
DROP TABLE IF EXISTS `tb_api_resources`;
CREATE TABLE `tb_api_resources` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '资源Uri',
  `ProjectName` varchar(100) DEFAULT NULL,
  `Url` varchar(150) DEFAULT NULL,
  `Method` varchar(50) DEFAULT NULL COMMENT '请求方式(GET,POST)',
  `Controller` varchar(50) DEFAULT NULL,
  `ControllerName` varchar(100) DEFAULT NULL,
  `Message` varchar(50) DEFAULT NULL,
  `Disabled` tinyint(4) DEFAULT NULL,
  `AllowScope` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_api_resources
-- ----------------------------
INSERT INTO `tb_api_resources` VALUES ('1', 'Pinzhi.Platform.WebApi', '/Api/QueryApiList', 'GET', 'Api', 'Api资源管理', '查询资源列表', '0', '2', '2018-06-19 15:45:22', '2019-01-03 14:53:23');
INSERT INTO `tb_api_resources` VALUES ('2', 'Pinzhi.Platform.WebApi', '/Api/SetApi', 'POST', 'Api', 'Api资源控制器', '设置Api资源', '0', '2', '2018-11-21 17:05:53', '2018-11-21 18:31:07');
INSERT INTO `tb_api_resources` VALUES ('3', 'Pinzhi.Platform.WebApi', '/Config/QueryAppList', 'GET', 'Config', '配置管理控制器', '查看所有项目组', '0', '2', '2018-11-21 17:08:28', '2018-11-21 18:23:14');
INSERT INTO `tb_api_resources` VALUES ('4', 'Pinzhi.Platform.WebApi', '/Config/SetAppInfo', 'POST', 'Config', '配置管理控制器', '设置项目组信息', '0', '2', '2018-11-21 18:23:36', '2018-11-21 18:23:36');
INSERT INTO `tb_api_resources` VALUES ('5', 'Pinzhi.Platform.WebApi', '/Config/QueryAppProjectList', 'GET', 'Config', '配置管理控制器', '查询配置项目', '0', '2', '2018-11-21 18:24:13', '2018-11-21 18:24:13');
INSERT INTO `tb_api_resources` VALUES ('6', 'Pinzhi.Platform.WebApi', '/Config/SetAppProjectInfo', 'POST', 'Config', '设置配置项目', '配置管理控制器', '0', '2', '2018-11-21 18:26:01', '2018-11-21 18:26:34');
INSERT INTO `tb_api_resources` VALUES ('7', 'Pinzhi.Platform.WebApi', '/Config/QueryAppConfigList', 'GET', 'Config', '配置控制器', '查询配置信息', '0', '2', '2018-11-21 18:26:30', '2018-11-21 18:26:30');
INSERT INTO `tb_api_resources` VALUES ('8', 'Pinzhi.Platform.WebApi', '/Config/SetAppConfigInfo', 'POST', 'Config', '配置控制器', '设置配置信息', '0', '2', '2018-11-21 18:28:20', '2018-11-21 18:28:20');
INSERT INTO `tb_api_resources` VALUES ('9', 'Pinzhi.Platform.WebApi', '/Menu/QueryAllMenus', 'GET', 'Menu', '菜单控制器', '查询平台菜单', '0', '2', '2018-11-21 18:29:08', '2018-11-21 18:29:08');
INSERT INTO `tb_api_resources` VALUES ('10', 'Pinzhi.Platform.WebApi', '/Menu/SetPlatform', 'POST', 'Menu', '菜单控制器', '设置平台菜单信息', '0', '2', '2018-11-21 18:29:36', '2018-11-21 18:29:36');
INSERT INTO `tb_api_resources` VALUES ('11', 'Pinzhi.Platform.WebApi', '/Menu/QueryUserMenus', 'GET', 'Menu', '菜单控制器', '查询用户菜单', '0', '2', '2018-11-21 18:29:57', '2018-11-21 18:29:57');
INSERT INTO `tb_api_resources` VALUES ('12', 'Pinzhi.Platform.WebApi', '/Microservice/QueryServiceList', 'GET', 'Microservice', '微服务管理', '查询服务发现全部服务', '0', '2', '2018-11-21 18:30:20', '2018-11-21 18:30:20');
INSERT INTO `tb_api_resources` VALUES ('13', 'Pinzhi.Platform.WebApi', '/Microservice/SetServiceInfo', 'POST', 'Microservice', '微服务管理', '服务注册', '0', '2', '2018-11-21 18:30:42', '2018-11-21 18:30:42');
INSERT INTO `tb_api_resources` VALUES ('14', 'Pinzhi.Platform.WebApi', '/Microservice/DeleteService', 'POST', 'Microservice', '微服务管理', '服务移除', '0', '2', '2018-11-21 18:35:28', '2018-11-21 18:35:28');
INSERT INTO `tb_api_resources` VALUES ('15', 'Pinzhi.Platform.WebApi', '/Microservice/QueryApiGatewayConfiguration', 'GET', 'Microservice', '微服务管理', '查询网关配置', '0', '2', '2018-11-21 18:36:04', '2019-01-28 14:03:24');
INSERT INTO `tb_api_resources` VALUES ('16', 'Pinzhi.Platform.WebApi', '/Microservice/SetApiGatewayConfiguration', 'POST', 'Microservice', '微服务管理', '设置网关配置', '0', '2', '2018-11-21 18:36:23', '2019-01-28 14:03:33');
INSERT INTO `tb_api_resources` VALUES ('17', 'Pinzhi.Platform.WebApi', '/Platform/QueryPlatforms', 'GET', 'Platform', '平台控制器', '查询平台列表', '0', '2', '2018-11-21 18:36:47', '2018-11-21 18:36:47');
INSERT INTO `tb_api_resources` VALUES ('18', 'Pinzhi.Platform.WebApi', '/Platform/SetPlatform', 'POST', 'Platform', '平台控制器', '设置平台信息', '0', '2', '2018-11-21 18:37:04', '2018-11-21 18:37:04');
INSERT INTO `tb_api_resources` VALUES ('19', 'Pinzhi.Platform.WebApi', '/Project/QueryProject', 'GET', 'Project', '项目控制器', '查看项目列表信息', '0', '2', '2018-11-21 18:37:23', '2018-11-21 18:37:23');
INSERT INTO `tb_api_resources` VALUES ('20', 'Pinzhi.Platform.WebApi', '/Role/QueryAllRoles', 'GET', 'Role', '角色控制器', '查询所有角色', '0', '2', '2018-11-21 18:37:43', '2018-11-21 18:38:50');
INSERT INTO `tb_api_resources` VALUES ('21', 'Pinzhi.Platform.WebApi', '/Role/QueryRoles', 'GET', 'Role', '角色控制器', '查询可用角色', '0', '2', '2018-11-21 18:38:23', '2018-11-21 18:38:23');
INSERT INTO `tb_api_resources` VALUES ('22', 'Pinzhi.Platform.WebApi', '/Role/SetRole', 'POST', 'Role', '查询可用角色', '设置角色信息', '0', '2', '2018-11-21 18:39:09', '2018-11-21 18:47:29');
INSERT INTO `tb_api_resources` VALUES ('23', 'Pinzhi.Platform.WebApi', '/Role/QueryRoleInfo', 'GET', 'Role', '角色控制器', '查询角色权限信息', '0', '2', '2018-11-21 18:39:27', '2018-11-21 18:39:27');
INSERT INTO `tb_api_resources` VALUES ('24', 'Pinzhi.Platform.WebApi', '/User/QueryUsers', 'GET', 'User', '用户控制器', '查询用户列表', '0', '2', '2018-11-21 18:47:07', '2018-11-21 18:47:07');
INSERT INTO `tb_api_resources` VALUES ('25', 'Pinzhi.Platform.WebApi', '/User/SetUser', 'POST', 'User', '用户控制器', '设置用户信息', '0', '2', '2018-11-21 18:47:25', '2018-11-21 18:47:25');
INSERT INTO `tb_api_resources` VALUES ('26', 'Pinzhi.Platform.WebApi', '/Microservice/QueryApiGatewayReRouteList', 'GET', 'Microservice', '微服务管理', '查询网关路由列表', '0', '2', '2019-01-28 14:21:42', '2019-01-28 14:21:42');
INSERT INTO `tb_api_resources` VALUES ('27', 'Pinzhi.Platform.WebApi', '/Microservice/SetApiGatewayReRoute', 'POST', 'Microservice', '微服务管理', 'Microservice', '0', '2', '2019-01-28 14:22:09', '2019-01-28 14:22:09');
INSERT INTO `tb_api_resources` VALUES ('28', 'Pinzhi.Platform.WebApi', '/Microservice/SyncApiGatewayConfigurationToConsul', 'GET', 'Microservice', '微服务管理', '同步网关配置到Consul', '0', '2', '2019-01-28 14:22:38', '2019-01-28 14:22:38');
INSERT INTO `tb_api_resources` VALUES ('29', 'Pinzhi.Platform.WebApi', '/Microservice/SyncApiGatewayConfigurationToRedis', 'GET', 'Microservice', '微服务管理', '同步网关配置到Redis', '0', '2', '2019-01-28 14:22:57', '2019-01-28 14:22:57');

-- ----------------------------
-- Table structure for tb_apigateway_config
-- ----------------------------
DROP TABLE IF EXISTS `tb_apigateway_config`;
CREATE TABLE `tb_apigateway_config` (
  `GatewayId` int(11) NOT NULL AUTO_INCREMENT,
  `GatewayKey` varchar(50) DEFAULT NULL,
  `BaseUrl` varchar(255) DEFAULT NULL,
  `DownstreamScheme` varchar(50) DEFAULT NULL,
  `RequestIdKey` varchar(50) DEFAULT NULL,
  `HttpHandlerOptions` varchar(1024) DEFAULT NULL,
  `LoadBalancerOptions` varchar(1024) DEFAULT NULL,
  `QoSOptions` varchar(1024) DEFAULT NULL,
  `ServiceDiscoveryProvider` varchar(1024) DEFAULT NULL,
  `RateLimitOptions` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`GatewayId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_apigateway_config
-- ----------------------------
INSERT INTO `tb_apigateway_config` VALUES ('1', 'Bucket.ApiGateway', '', '', 'requestid', '{\"AllowAutoRedirect\":false,\"UseCookieContainer\":false,\"UseTracing\":false,\"UseProxy\":false}', '{\"Type\":\"RoundRobin\",\"Key\":\"\",\"Expiry\":0}', '{\"ExceptionsAllowedBeforeBreaking\":0,\"DurationOfBreak\":0,\"TimeoutValue\":0}', '{\"Host\":\"192.168.1.54\",\"Port\":8500,\"Type\":\"\",\"Token\":\"\",\"ConfigurationKey\":\"Pinzhi.ApiGateway\",\"PollingInterval\":0}', '{\"ClientIdHeader\":\"ClientId\",\"QuotaExceededMessage\":\"Customize Tips!\",\"RateLimitCounterPrefix\":\"ocelot\",\"DisableRateLimitHeaders\":false,\"HttpStatusCode\":429}');

-- ----------------------------
-- Table structure for tb_apigateway_reroute
-- ----------------------------
DROP TABLE IF EXISTS `tb_apigateway_reroute`;
CREATE TABLE `tb_apigateway_reroute` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `GatewayId` int(11) DEFAULT NULL,
  `UpstreamPathTemplate` varchar(100) DEFAULT NULL,
  `UpstreamHttpMethod` varchar(100) DEFAULT NULL,
  `UpstreamHost` varchar(150) DEFAULT NULL,
  `DownstreamPathTemplate` varchar(100) DEFAULT NULL,
  `DownstreamScheme` varchar(20) DEFAULT NULL,
  `DownstreamHostAndPorts` varchar(2048) DEFAULT NULL,
  `ServiceName` varchar(50) DEFAULT NULL,
  `Key` varchar(50) DEFAULT NULL,
  `RequestIdKey` varchar(50) DEFAULT NULL,
  `Priority` int(11) DEFAULT NULL,
  `Timeout` int(11) DEFAULT NULL,
  `SecurityOptions` varchar(1024) DEFAULT NULL,
  `CacheOptions` varchar(1024) DEFAULT NULL,
  `HttpHandlerOptions` varchar(1024) DEFAULT NULL,
  `AuthenticationOptions` varchar(1024) DEFAULT NULL,
  `RateLimitOptions` varchar(1024) DEFAULT NULL,
  `LoadBalancerOptions` varchar(1024) DEFAULT NULL,
  `QoSOptions` varchar(1024) DEFAULT NULL,
  `DelegatingHandlers` varchar(1024) DEFAULT NULL,
  `State` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_apigateway_reroute
-- ----------------------------
INSERT INTO `tb_apigateway_reroute` VALUES ('1', '1', '/auth/{everyting}', '[\"Post\",\"Get\"]', null, '/{everyting}', 'http', '[{\"Host\":\"192.168.1.52\",\"Port\":18082}]', null, null, null, '1', '0', '{\"IPAllowedList\":[],\"IPBlockedList\":[]}', '{\"TtlSeconds\":0,\"Region\":null}', '{\"AllowAutoRedirect\":false,\"UseCookieContainer\":false,\"UseTracing\":false,\"UseProxy\":false}', '{\"AuthenticationProviderKey\":\"\",\"AllowedScopes\":[]}', '{\"ClientWhitelist\":[],\"EnableRateLimiting\":false,\"Period\":null,\"PeriodTimespan\":0.0,\"Limit\":0}', '{\"Type\":\"RoundRobin\",\"Key\":null,\"Expiry\":0}', '{\"ExceptionsAllowedBeforeBreaking\":0,\"DurationOfBreak\":0,\"TimeoutValue\":0}', '[]', '1');
INSERT INTO `tb_apigateway_reroute` VALUES ('2', '1', '/platform/{everything}', '[\"Post\",\"Get\"]', null, '/{everything}', 'http', '[{\"Host\":\"192.168.1.53\",\"Port\":18083}]', null, null, null, '0', '0', '{\"IPAllowedList\":[],\"IPBlockedList\":[]}', '{\"TtlSeconds\":0,\"Region\":null}', '{\"AllowAutoRedirect\":false,\"UseCookieContainer\":false,\"UseTracing\":false,\"UseProxy\":true}', '{\"AuthenticationProviderKey\":\"\",\"AllowedScopes\":[]}', '{\"ClientWhitelist\":[],\"EnableRateLimiting\":false,\"Period\":null,\"PeriodTimespan\":0.0,\"Limit\":0}', '{\"Type\":\"RoundRobin\",\"Key\":null,\"Expiry\":0}', '{\"ExceptionsAllowedBeforeBreaking\":0,\"DurationOfBreak\":0,\"TimeoutValue\":0}', '[]', '1');
INSERT INTO `tb_apigateway_reroute` VALUES ('6', '1', '/tracing/{everyting}', '[\"Get\",\"Post\"]', null, '/{everyting}', 'http', '[{\"Host\":\"192.168.1.53\",\"Port\":18084}]', null, null, null, '0', '0', '{\"IPAllowedList\":[],\"IPBlockedList\":[]}', '{\"TtlSeconds\":0,\"Region\":null}', '{\"AllowAutoRedirect\":false,\"UseCookieContainer\":false,\"UseTracing\":false,\"UseProxy\":false}', '{\"AuthenticationProviderKey\":\"Bucket\",\"AllowedScopes\":[\"admin\"]}', '{\"ClientWhitelist\":[],\"EnableRateLimiting\":false,\"Period\":null,\"PeriodTimespan\":0.0,\"Limit\":0}', '{\"Type\":\"RoundRobin\",\"Key\":\"\",\"Expiry\":0}', '{\"ExceptionsAllowedBeforeBreaking\":0,\"DurationOfBreak\":0,\"TimeoutValue\":0}', '[]', '1');
INSERT INTO `tb_apigateway_reroute` VALUES ('9', '1', '/configs/{everything}', '[\"Get\",\"Post\"]', null, '/configs/{everything}', 'http', '[{\"Host\":\"192.168.1.53\",\"Port\":18081}]', null, null, null, '0', '0', '{\"IPAllowedList\":[],\"IPBlockedList\":[]}', '{\"TtlSeconds\":0,\"Region\":null}', '{\"AllowAutoRedirect\":false,\"UseCookieContainer\":false,\"UseTracing\":false,\"UseProxy\":false}', '{\"AuthenticationProviderKey\":\"\",\"AllowedScopes\":[]}', '{\"ClientWhitelist\":[],\"EnableRateLimiting\":false,\"Period\":null,\"PeriodTimespan\":0.0,\"Limit\":0}', '{\"Type\":\"RoundRobin\",\"Key\":\"\",\"Expiry\":0}', '{\"ExceptionsAllowedBeforeBreaking\":0,\"DurationOfBreak\":0,\"TimeoutValue\":0}', '[]', '1');

-- ----------------------------
-- Table structure for tb_app
-- ----------------------------
DROP TABLE IF EXISTS `tb_app`;
CREATE TABLE `tb_app` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `AppId` varchar(50) NOT NULL,
  `Secret` varchar(255) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`,`AppId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_app
-- ----------------------------
INSERT INTO `tb_app` VALUES ('1', '全家桶', 'FamilyBucket', 'hsenwkqimk4mfxt88pc9hbn6fcubcb4u', '全家桶');

-- ----------------------------
-- Table structure for tb_appconfig_dev
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_dev`;
CREATE TABLE `tb_appconfig_dev` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(100) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=60 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appconfig_dev
-- ----------------------------
INSERT INTO `tb_appconfig_dev` VALUES ('1', 'FamilyBucket', 'Public', 'RedisDefaultServer', '10.10.188.136:6379,allowadmin=true', 'Redis默认连接地址', '2019-01-23 10:19:06', '2018-05-10 11:05:36', '87', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('3', 'FamilyBucket', 'Pinzhi.Identity.WebApi', 'WxMiniApiUrl', 'https://api.weixin.qq.com', '微信小程序接口基地址', '2018-07-19 10:49:56', '2018-07-19 10:49:56', '1', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('30', 'FamilyBucket', 'Public', 'IsVerifySmsCode', '2', '是否验证短信验证码，2不验证', '2019-01-30 14:52:31', '2018-11-12 10:02:49', '91', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('33', 'FamilyBucket', 'Public', 'ApiGatewayApiUrl', 'http://10.10.188.136:5001/', '本地网关api地址', '2018-11-23 11:28:03', '2018-11-23 11:09:31', '43', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('42', 'FamilyBucket', 'Bucket.Public', 'ServiceDiscovery:Consul:HttpEndpoint', 'http://10.10.188.136:8500', '服务注册ConsulURI', '2019-01-02 09:43:45', '2019-01-02 09:43:45', '65', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('43', 'FamilyBucket', 'Bucket.Public', 'ServiceDiscovery:Consul:DnsEndpoint:Address', '10.10.188.136', '服务注册Consul_IP', '2019-01-02 09:44:41', '2019-01-02 09:44:41', '66', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('44', 'FamilyBucket', 'Bucket.Public', 'ServiceDiscovery:Consul:DnsEndpoint:Port', '8500', '服务注册Consul端口', '2019-01-02 09:45:16', '2019-01-02 09:45:16', '67', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('45', 'FamilyBucket', 'Bucket.Public', 'ErrorCodeServer:ServerUrl', 'http://122.192.33.40:18080', '错误码服务接口地址', '2019-01-02 09:47:34', '2019-01-02 09:47:34', '68', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('46', 'FamilyBucket', 'Bucket.Public', 'ErrorCodeServer:RefreshInteval', '1800', '错误码刷新时间', '2019-01-02 09:48:09', '2019-01-02 09:48:09', '69', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('47', 'FamilyBucket', 'Bucket.Public', 'EventBus:RabbitMQ:HostName', '10.10.188.136', '消息总线RabbitMq IP', '2019-01-02 09:49:07', '2019-01-02 09:49:07', '70', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('48', 'FamilyBucket', 'Bucket.Public', 'EventBus:RabbitMQ:Port', '5672', '消息总线RabbitMq 端口', '2019-01-08 17:07:04', '2019-01-02 09:50:06', '83', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('49', 'FamilyBucket', 'Bucket.Public', 'EventBus:RabbitMQ:UserName', 'guest', '消息总线RabbitMq 账号', '2019-01-08 17:07:10', '2019-01-02 09:50:57', '84', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('50', 'FamilyBucket', 'Bucket.Public', 'EventBus:RabbitMQ:Password', 'guest', '消息总线RabbitMq 密码', '2019-01-08 17:07:16', '2019-01-02 09:51:33', '85', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('51', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:Secret', 'hsenwkqimk4mfxt88pc9hbn6fcubcb4u', '认证授权密钥', '2019-01-02 09:52:26', '2019-01-02 09:52:26', '74', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('52', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:Issuer', 'poc', '认证授权Issuer', '2019-01-02 09:52:50', '2019-01-02 09:52:50', '75', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('53', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:Audience', 'axon', '认证授权', '2019-01-02 09:53:30', '2019-01-02 09:53:30', '76', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('54', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:PolicyName', 'permission', '认证授权', '2019-01-02 09:54:07', '2019-01-02 09:54:07', '77', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('55', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:DefaultScheme', 'Bearer', '认证授权', '2019-01-02 09:54:37', '2019-01-02 09:54:37', '78', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('56', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:IsHttps', 'false', '认证授权', '2019-01-02 09:55:05', '2019-01-02 09:55:05', '79', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('57', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:RequireExpirationTime', 'true', '认证授权', '2019-01-02 09:55:30', '2019-01-02 09:55:30', '80', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('58', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:MySqlConnectionString', 'characterset=utf8;server=127.0.0.1;port=3306;user id=root;password=123;persistsecurityinfo=True;database=bucket', '认证授权', '2019-01-02 10:00:22', '2019-01-02 10:00:22', '81', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('59', 'FamilyBucket', 'Bucket.Public', 'JwtAuthorize:RefreshInteval', '300', '认证授权', '2019-01-02 10:01:07', '2019-01-02 10:01:07', '82', '0');

-- ----------------------------
-- Table structure for tb_appconfig_prepro
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_prepro`;
CREATE TABLE `tb_appconfig_prepro` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(100) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_appconfig_pro
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_pro`;
CREATE TABLE `tb_appconfig_pro` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(100) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_appconfig_uat
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_uat`;
CREATE TABLE `tb_appconfig_uat` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(100) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_appnamespace
-- ----------------------------
DROP TABLE IF EXISTS `tb_appnamespace`;
CREATE TABLE `tb_appnamespace` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `AppId` varchar(50) DEFAULT NULL,
  `IsPublic` tinyint(1) DEFAULT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `CreateUid` bigint(20) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `LastUid` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appnamespace
-- ----------------------------
INSERT INTO `tb_appnamespace` VALUES ('1', 'Public', 'FamilyBucket', '1', '业务公共参数', '0', '2018-05-10 11:03:17', '1548387601696', '2019-01-03 14:25:46', '1548387601696');
INSERT INTO `tb_appnamespace` VALUES ('2', 'Bucket.Public', 'FamilyBucket', '1', '框架公共参数', '0', '2019-01-02 09:28:43', '1548387601696', '2019-01-02 09:28:54', '1548387601696');
INSERT INTO `tb_appnamespace` VALUES ('3', 'Pinzhi.Identity.WebApi', 'FamilyBucket', '0', '品值认证授权中心', '0', '2018-07-19 10:46:32', '1548387601696', '2018-07-19 10:46:32', '1548387601696');
INSERT INTO `tb_appnamespace` VALUES ('7', 'Pinzhi.BackgroundTasks', 'FamilyBucket', '0', '品值默认事件订阅', '0', '2018-09-26 16:56:09', '1548387601696', '2018-09-26 18:28:44', '1548387601696');
INSERT INTO `tb_appnamespace` VALUES ('8', 'Pinzhi.Platform.WebApi', 'FamilyBucket', '0', '系统管理平台', '0', '2018-11-20 18:06:39', '1548387601696', '2018-11-20 18:06:39', '1548387601696');

-- ----------------------------
-- Table structure for tb_events
-- ----------------------------
DROP TABLE IF EXISTS `tb_events`;
CREATE TABLE `tb_events` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Mode` varchar(100) DEFAULT NULL,
  `EventName` varchar(150) DEFAULT NULL,
  `EventKey` varchar(150) DEFAULT NULL,
  `EventCode` varchar(150) DEFAULT NULL,
  `EventValue` varchar(150) DEFAULT NULL,
  `UserKey` varchar(50) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Channel` varchar(100) DEFAULT NULL,
  `Source` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1091010230684565505 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_platform_menus
-- ----------------------------
DROP TABLE IF EXISTS `tb_platform_menus`;
CREATE TABLE `tb_platform_menus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Icon` varchar(255) DEFAULT NULL,
  `LinkUrl` varchar(255) DEFAULT NULL,
  `SortId` int(11) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `State` int(11) DEFAULT NULL,
  `PlatformId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platform_menus
-- ----------------------------
INSERT INTO `tb_platform_menus` VALUES ('1', '系统设置', 'el-icon-setting', null, '3', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('2', '平台菜单', null, '/setting/menu', '99', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('3', '用户设置', null, '/setting/user', '103', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('4', '角色设置', null, '/setting/role', '102', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('5', '平台设置', null, '/setting/platform', '98', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('6', '项目管理', null, '/setting/project', '100', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('7', '项目资源', null, '/setting/apimanage', '101', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('8', '配置中心', 'el-icon-refresh', null, '2', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('9', '项目组管理', null, '/configService/appList', '99', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('10', '项目管理', null, '/configService/projectList', '100', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('11', '配置管理', null, '/configService/configList', '101', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('12', '微服务管理', 'el-icon-upload', null, '1', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('13', '服务管理', null, '/microservice/service', '101', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('14', '网关路由', null, '/microservice/route', '100', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('15', '链路追踪', null, '/microservice/trace', '102', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('16', '访问量统计', null, '/microservice/access', '103', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('17', '链路监听', null, '/microservice/traceOnline', '104', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('18', '网关配置', null, '/microservice/gateway', '99', '12', '1', '1');

-- ----------------------------
-- Table structure for tb_platforms
-- ----------------------------
DROP TABLE IF EXISTS `tb_platforms`;
CREATE TABLE `tb_platforms` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) DEFAULT NULL,
  `Key` varchar(255) DEFAULT NULL,
  `Icon` varchar(255) DEFAULT NULL,
  `Author` varchar(255) DEFAULT NULL,
  `Developer` varchar(255) DEFAULT NULL,
  `Remark` varchar(255) DEFAULT NULL,
  `SortId` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `IsDel` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platforms
-- ----------------------------
INSERT INTO `tb_platforms` VALUES ('1', '品值综合管理平台', 'Pinzhi.Platform.Poc', null, null, null, null, '1', '2018-06-21 18:26:23', '0');
INSERT INTO `tb_platforms` VALUES ('2', '品值统一认证平台', 'Pinzhi.Platform.Identity', null, null, null, null, '2', '2018-11-27 09:13:03', '0');
INSERT INTO `tb_platforms` VALUES ('3', '品值统一消息平台', 'Pinzhi.Platform.MqMessage', null, null, null, null, '3', '2018-11-27 09:14:28', '0');
INSERT INTO `tb_platforms` VALUES ('4', '品值统一日志平台', 'Pinzhi.Platform.LogMessage', null, null, null, null, '4', '2018-11-27 09:15:32', '0');
INSERT INTO `tb_platforms` VALUES ('5', '品值统一调度平台', 'Pinzhi.Platform.Task', null, null, null, null, '5', '2018-11-27 09:21:29', '0');

-- ----------------------------
-- Table structure for tb_projects
-- ----------------------------
DROP TABLE IF EXISTS `tb_projects`;
CREATE TABLE `tb_projects` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Code` varchar(255) NOT NULL,
  `AppId` varchar(20) DEFAULT NULL,
  `Secret` varchar(255) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `RouteKey` varchar(20) DEFAULT NULL,
  `IsDeleted` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `CreateUid` bigint(20) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `LastUid` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Code`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_projects
-- ----------------------------
INSERT INTO `tb_projects` VALUES ('2', '平台管理接口服务', 'Pinzhi.Platform.WebApi', null, null, null, 'Platform', '0', '2019-01-03 10:55:33', null, '2019-01-03 14:52:45', '1548387601696');

-- ----------------------------
-- Table structure for tb_role_apis
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_apis`;
CREATE TABLE `tb_role_apis` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApiId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=255 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_apis
-- ----------------------------
INSERT INTO `tb_role_apis` VALUES ('224', '1', '1');
INSERT INTO `tb_role_apis` VALUES ('225', '2', '1');
INSERT INTO `tb_role_apis` VALUES ('226', '3', '1');
INSERT INTO `tb_role_apis` VALUES ('227', '4', '1');
INSERT INTO `tb_role_apis` VALUES ('228', '5', '1');
INSERT INTO `tb_role_apis` VALUES ('229', '6', '1');
INSERT INTO `tb_role_apis` VALUES ('230', '7', '1');
INSERT INTO `tb_role_apis` VALUES ('231', '8', '1');
INSERT INTO `tb_role_apis` VALUES ('232', '9', '1');
INSERT INTO `tb_role_apis` VALUES ('233', '10', '1');
INSERT INTO `tb_role_apis` VALUES ('234', '11', '1');
INSERT INTO `tb_role_apis` VALUES ('235', '12', '1');
INSERT INTO `tb_role_apis` VALUES ('236', '13', '1');
INSERT INTO `tb_role_apis` VALUES ('237', '14', '1');
INSERT INTO `tb_role_apis` VALUES ('238', '15', '1');
INSERT INTO `tb_role_apis` VALUES ('239', '16', '1');
INSERT INTO `tb_role_apis` VALUES ('240', '17', '1');
INSERT INTO `tb_role_apis` VALUES ('241', '18', '1');
INSERT INTO `tb_role_apis` VALUES ('242', '19', '1');
INSERT INTO `tb_role_apis` VALUES ('243', '20', '1');
INSERT INTO `tb_role_apis` VALUES ('244', '21', '1');
INSERT INTO `tb_role_apis` VALUES ('245', '22', '1');
INSERT INTO `tb_role_apis` VALUES ('246', '23', '1');
INSERT INTO `tb_role_apis` VALUES ('247', '24', '1');
INSERT INTO `tb_role_apis` VALUES ('248', '25', '1');
INSERT INTO `tb_role_apis` VALUES ('249', '26', '1');
INSERT INTO `tb_role_apis` VALUES ('250', '27', '1');
INSERT INTO `tb_role_apis` VALUES ('251', '28', '1');
INSERT INTO `tb_role_apis` VALUES ('252', '29', '1');

-- ----------------------------
-- Table structure for tb_role_menus
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_menus`;
CREATE TABLE `tb_role_menus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MenuId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=351 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_menus
-- ----------------------------
INSERT INTO `tb_role_menus` VALUES ('320', '1', '1');
INSERT INTO `tb_role_menus` VALUES ('321', '2', '1');
INSERT INTO `tb_role_menus` VALUES ('322', '3', '1');
INSERT INTO `tb_role_menus` VALUES ('323', '4', '1');
INSERT INTO `tb_role_menus` VALUES ('324', '5', '1');
INSERT INTO `tb_role_menus` VALUES ('325', '6', '1');
INSERT INTO `tb_role_menus` VALUES ('326', '7', '1');
INSERT INTO `tb_role_menus` VALUES ('327', '8', '1');
INSERT INTO `tb_role_menus` VALUES ('328', '9', '1');
INSERT INTO `tb_role_menus` VALUES ('329', '10', '1');
INSERT INTO `tb_role_menus` VALUES ('330', '11', '1');
INSERT INTO `tb_role_menus` VALUES ('331', '12', '1');
INSERT INTO `tb_role_menus` VALUES ('332', '13', '1');
INSERT INTO `tb_role_menus` VALUES ('333', '14', '1');
INSERT INTO `tb_role_menus` VALUES ('334', '15', '1');
INSERT INTO `tb_role_menus` VALUES ('335', '16', '1');
INSERT INTO `tb_role_menus` VALUES ('336', '17', '1');
INSERT INTO `tb_role_menus` VALUES ('337', '18', '1');

-- ----------------------------
-- Table structure for tb_roles
-- ----------------------------
DROP TABLE IF EXISTS `tb_roles`;
CREATE TABLE `tb_roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PlatformKey` varchar(255) DEFAULT NULL,
  `Key` varchar(255) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Remark` varchar(50) DEFAULT NULL,
  `IsSys` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  `IsDel` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Key`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_roles
-- ----------------------------
INSERT INTO `tb_roles` VALUES ('1', 'Pinzhi.Platform.Poc', 'admin', '超级管理员', '平台基础管理/超级管理员', '1', '2018-05-17 10:39:30', '2019-01-28 14:36:52', '0');

-- ----------------------------
-- Table structure for tb_thirdoauths
-- ----------------------------
DROP TABLE IF EXISTS `tb_thirdoauths`;
CREATE TABLE `tb_thirdoauths` (
  `Uid` bigint(20) NOT NULL,
  `OpenId` varchar(255) NOT NULL,
  `UnionId` varchar(255) DEFAULT NULL,
  `AuthServer` varchar(255) NOT NULL,
  `AppId` varchar(255) DEFAULT NULL,
  KEY `index_oauth` (`Uid`,`OpenId`,`UnionId`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_user_roles
-- ----------------------------
DROP TABLE IF EXISTS `tb_user_roles`;
CREATE TABLE `tb_user_roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Uid` bigint(50) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_user_roles
-- ----------------------------
INSERT INTO `tb_user_roles` VALUES ('8', '1548387601696', '1');

-- ----------------------------
-- Table structure for tb_users
-- ----------------------------
DROP TABLE IF EXISTS `tb_users`;
CREATE TABLE `tb_users` (
  `Id` bigint(30) NOT NULL,
  `UserName` varchar(30) DEFAULT NULL,
  `Password` varchar(200) DEFAULT NULL,
  `RealName` varchar(20) DEFAULT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `State` int(11) DEFAULT NULL,
  `Salt` varchar(20) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_users
-- ----------------------------
INSERT INTO `tb_users` VALUES ('1548387601696', 'admin', '816c62bc3e1dc540d8a409db908f4829f720ae0c9638b3175c47e1358c114ec8', '田亮', '15155010775', '403305950@qq.com', '1', 'akajdksjjda', '2018-05-10 17:08:33', '2019-01-28 14:37:20');

-- ----------------------------
-- Table structure for tb_api_times
-- ----------------------------
DROP TABLE IF EXISTS `tb_api_times`;
CREATE TABLE `tb_api_times` (
  `Id` varchar(50) NOT NULL,
  `OperationName` varchar(255) DEFAULT NULL,
  `Duration` bigint(20) DEFAULT NULL,
  `StartTimestamp` datetime DEFAULT NULL,
  `FinishTimestamp` datetime DEFAULT NULL,
  `ServiceName` varchar(255) DEFAULT NULL,
  `ServiceEnvironment` varchar(50) DEFAULT NULL,
  `HttpPath` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_bs_logs
-- ----------------------------
DROP TABLE IF EXISTS `tb_bs_logs`;
CREATE TABLE `tb_bs_logs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TraceId` varchar(100) DEFAULT NULL,
  `SpanId` varchar(100) DEFAULT NULL,
  `OperationName` varchar(150) DEFAULT NULL,
  `Duration` float(20,3) DEFAULT NULL,
  `StartTimestamp` datetime DEFAULT NULL,
  `FinishTimestamp` datetime DEFAULT NULL,
  `ServiceName` varchar(100) DEFAULT NULL,
  `HttpUrl` varchar(500) DEFAULT NULL,
  `HttpMethod` varchar(50) DEFAULT NULL,
  `HttpRequest` varchar(5120) DEFAULT NULL,
  `HttpResponse` varchar(5120) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tb_logs
-- ----------------------------
DROP TABLE IF EXISTS `tb_logs`;
CREATE TABLE `tb_logs` (
  `Id` bigint(20) NOT NULL,
  `ClassName` varchar(100) DEFAULT NULL,
  `ProjectName` varchar(100) DEFAULT NULL,
  `LogTag` varchar(150) DEFAULT NULL,
  `LogType` varchar(20) DEFAULT NULL,
  `LogMessage` varchar(5120) DEFAULT NULL,
  `IP` varchar(20) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;