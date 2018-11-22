/*
Navicat MySQL Data Transfer

Source Server         : 127.0.0.1
Source Server Version : 50722
Source Host           : 127.0.0.1:3306
Source Database       : bucket

Target Server Type    : MYSQL
Target Server Version : 50722
File Encoding         : 65001

Date: 2018-11-22 14:25:44
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for tb_api_resources
-- ----------------------------
DROP TABLE IF EXISTS `tb_api_resources`;
CREATE TABLE `tb_api_resources` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '资源Uri',
  `ProjectName` varchar(255) DEFAULT NULL,
  `Url` varchar(50) DEFAULT NULL,
  `Method` varchar(50) DEFAULT NULL COMMENT '请求方式(GET,POST)',
  `Controller` varchar(50) DEFAULT NULL,
  `ControllerName` varchar(100) DEFAULT NULL,
  `Message` varchar(50) DEFAULT NULL,
  `Disabled` tinyint(4) DEFAULT NULL,
  `AllowScope` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_api_resources
-- ----------------------------
INSERT INTO `tb_api_resources` VALUES ('1', 'Pinzhi.Platform', '/Api/QueryApiList', 'GET', 'Api', 'Api资源管理', '查询资源列表', '0', '2', '2018-06-19 15:45:22', '2018-06-19 15:45:24');
INSERT INTO `tb_api_resources` VALUES ('2', 'Pinzhi.Platform', '/Api/SetApi', 'POST', 'Api', 'Api资源控制器', '设置Api资源', '0', '2', '2018-11-21 17:05:53', '2018-11-21 18:31:07');
INSERT INTO `tb_api_resources` VALUES ('3', 'Pinzhi.Platform', '/Config/QueryAppList', 'GET', 'Config', '配置管理控制器', '查看所有项目组', '0', '2', '2018-11-21 17:08:28', '2018-11-21 18:23:14');
INSERT INTO `tb_api_resources` VALUES ('4', 'Pinzhi.Platform', '/Config/SetAppInfo', 'POST', 'Config', '配置管理控制器', '设置项目组信息', '0', '2', '2018-11-21 18:23:36', '2018-11-21 18:23:36');
INSERT INTO `tb_api_resources` VALUES ('5', 'Pinzhi.Platform', '/Config/QueryAppProjectList', 'GET', 'Config', '配置管理控制器', '查询配置项目', '0', '2', '2018-11-21 18:24:13', '2018-11-21 18:24:13');
INSERT INTO `tb_api_resources` VALUES ('6', 'Pinzhi.Platform', '/Config/SetAppProjectInfo', 'POST', 'Config', '设置配置项目', '配置管理控制器', '0', '2', '2018-11-21 18:26:01', '2018-11-21 18:26:34');
INSERT INTO `tb_api_resources` VALUES ('7', 'Pinzhi.Platform', '/Config/QueryAppConfigList', 'GET', 'Config', '配置控制器', '查询配置信息', '0', '2', '2018-11-21 18:26:30', '2018-11-21 18:26:30');
INSERT INTO `tb_api_resources` VALUES ('8', 'Pinzhi.Platform', '/Config/SetAppConfigInfo', 'POST', 'Config', '配置控制器', '设置配置信息', '0', '2', '2018-11-21 18:28:20', '2018-11-21 18:28:20');
INSERT INTO `tb_api_resources` VALUES ('9', 'Pinzhi.Platform', '/Menu/QueryAllMenus', 'GET', 'Menu', '菜单控制器', '查询平台菜单', '0', '2', '2018-11-21 18:29:08', '2018-11-21 18:29:08');
INSERT INTO `tb_api_resources` VALUES ('10', 'Pinzhi.Platform', '/Menu/SetPlatform', 'POST', 'Menu', '菜单控制器', '设置平台菜单信息', '0', '2', '2018-11-21 18:29:36', '2018-11-21 18:29:36');
INSERT INTO `tb_api_resources` VALUES ('11', 'Pinzhi.Platform', '/Menu/QueryUserMenus', 'GET', 'Menu', '菜单控制器', '查询用户菜单', '0', '2', '2018-11-21 18:29:57', '2018-11-21 18:29:57');
INSERT INTO `tb_api_resources` VALUES ('12', 'Pinzhi.Platform', '/Microservice/QueryServiceList', 'GET', 'Microservice', '微服务管理', '查询服务发现全部服务', '0', '2', '2018-11-21 18:30:20', '2018-11-21 18:30:20');
INSERT INTO `tb_api_resources` VALUES ('13', 'Pinzhi.Platform', '/Microservice/SetServiceInfo', 'POST', 'Microservice', '微服务管理', '服务注册', '0', '2', '2018-11-21 18:30:42', '2018-11-21 18:30:42');
INSERT INTO `tb_api_resources` VALUES ('14', 'Pinzhi.Platform', '/Microservice/DeleteService', 'POST', 'Microservice', '微服务管理', '服务移除', '0', '2', '2018-11-21 18:35:28', '2018-11-21 18:35:28');
INSERT INTO `tb_api_resources` VALUES ('15', 'Pinzhi.Platform', '/Microservice/GetApiGatewayConfiguration', 'GET', 'Microservice', '微服务管理', '查询网关配置', '0', '2', '2018-11-21 18:36:04', '2018-11-21 18:36:04');
INSERT INTO `tb_api_resources` VALUES ('16', 'Pinzhi.Platform', '/Microservice/SetApiGatewayConfiguration', 'POST', 'Microservice', '微服务管理', '设置网关配置', '0', '2', '2018-11-21 18:36:23', '2018-11-21 18:36:23');
INSERT INTO `tb_api_resources` VALUES ('17', 'Pinzhi.Platform', '/Platform/QueryPlatforms', 'GET', 'Platform', '平台控制器', '查询平台列表', '0', '2', '2018-11-21 18:36:47', '2018-11-21 18:36:47');
INSERT INTO `tb_api_resources` VALUES ('18', 'Pinzhi.Platform', '/Platform/SetPlatform', 'POST', 'Platform', '平台控制器', '设置平台信息', '0', '2', '2018-11-21 18:37:04', '2018-11-21 18:37:04');
INSERT INTO `tb_api_resources` VALUES ('19', 'Pinzhi.Platform', '/Project/QueryProject', 'GET', 'Project', '项目控制器', '查看项目列表信息', '0', '2', '2018-11-21 18:37:23', '2018-11-21 18:37:23');
INSERT INTO `tb_api_resources` VALUES ('20', 'Pinzhi.Platform', '/Role/QueryAllRoles', 'GET', 'Role', '角色控制器', '查询所有角色', '0', '2', '2018-11-21 18:37:43', '2018-11-21 18:38:50');
INSERT INTO `tb_api_resources` VALUES ('21', 'Pinzhi.Platform', '/Role/QueryRoles', 'GET', 'Role', '角色控制器', '查询可用角色', '0', '2', '2018-11-21 18:38:23', '2018-11-21 18:38:23');
INSERT INTO `tb_api_resources` VALUES ('22', 'Pinzhi.Platform', '/Role/SetRole', 'POST', 'Role', '查询可用角色', '设置角色信息', '0', '2', '2018-11-21 18:39:09', '2018-11-21 18:47:29');
INSERT INTO `tb_api_resources` VALUES ('23', 'Pinzhi.Platform', '/Role/QueryRoleInfo', 'GET', 'Role', '角色控制器', '查询角色权限信息', '0', '2', '2018-11-21 18:39:27', '2018-11-21 18:39:27');
INSERT INTO `tb_api_resources` VALUES ('24', 'Pinzhi.Platform', '/User/QueryUsers', 'GET', 'User', '用户控制器', '查询用户列表', '0', '2', '2018-11-21 18:47:07', '2018-11-21 18:47:07');
INSERT INTO `tb_api_resources` VALUES ('25', 'Pinzhi.Platform', '/User/SetUser', 'POST', 'User', '用户控制器', '设置用户信息', '0', '2', '2018-11-21 18:47:25', '2018-11-21 18:47:25');

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
-- Records of tb_api_times
-- ----------------------------

-- ----------------------------
-- Table structure for tb_apigateway
-- ----------------------------
DROP TABLE IF EXISTS `tb_apigateway`;
CREATE TABLE `tb_apigateway` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigurationKey` varchar(255) DEFAULT NULL,
  `Configuration` text,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_apigateway
-- ----------------------------

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
INSERT INTO `tb_app` VALUES ('1', '品值综合', 'PinzhiGO', 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx', '品值综合');

-- ----------------------------
-- Table structure for tb_appconfig_dev
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_dev`;
CREATE TABLE `tb_appconfig_dev` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(50) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appconfig_dev
-- ----------------------------
INSERT INTO `tb_appconfig_dev` VALUES ('1', 'PinzhiGO', 'Public', 'RedisDefaultServer', '10.10.133.230:6379,allowadmin=true', 'Redis默认连接地址', '2018-10-30 16:32:49', '2018-05-10 11:05:36', '34', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('3', 'PinzhiGO', 'Pinzhi.Identity', 'WxMiniApiUrl', 'https://api.weixin.qq.com', '微信小程序接口基地址', '2018-07-19 10:49:56', '2018-07-19 10:49:56', '1', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('24', 'PinzhiGO', 'Pinzhi.Platform', 'ApiGatewayConfigurationKey', 'ApiGatewayConfiguration', '微服务网关配置存储Key', '2018-11-20 18:07:23', '2018-11-20 18:07:23', '78', '0');
INSERT INTO `tb_appconfig_dev` VALUES ('30', 'PinzhiGO', 'Pinzhi.Identity', 'IsVerifySmsCode', '2', '是否验证短信验证码，2不验证', '2018-11-12 10:02:49', '2018-11-12 10:02:49', '39', '0');

-- ----------------------------
-- Table structure for tb_appconfig_pro
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig_pro`;
CREATE TABLE `tb_appconfig_pro` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(50) DEFAULT NULL,
  `ConfigNamespaceName` varchar(50) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appconfig_pro
-- ----------------------------
INSERT INTO `tb_appconfig_pro` VALUES ('1', 'PinzhiGO', 'Public', 'RedisDefaultServer', '192.168.3.30:6379,allowadmin=true', 'Redis默认连接地址', '2018-07-17 13:48:39', '2018-05-10 11:05:36', '7', '0');
INSERT INTO `tb_appconfig_pro` VALUES ('3', 'PinzhiGO', 'Pinzhi.Identity', 'WxMiniApiUrl', 'https://api.weixin.qq.com', '微信小程序接口基地址', '2018-07-19 10:49:56', '2018-07-19 10:49:56', '11', '0');
INSERT INTO `tb_appconfig_pro` VALUES ('24', 'PinzhiGO', 'Pinzhi.Platform', 'ApiGatewayConfigurationKey', 'ApiGatewayConfiguration', '微服务网关配置存储Key', '2018-11-20 18:07:23', '2018-11-20 18:07:23', '78', '0');
INSERT INTO `tb_appconfig_pro` VALUES ('30', 'PinzhiGO', 'Pinzhi.Identity', 'IsVerifySmsCode', '2', '是否验证短信验证码，2不验证', '2018-11-12 10:02:49', '2018-11-12 10:02:49', '39', '0');

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
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appnamespace
-- ----------------------------
INSERT INTO `tb_appnamespace` VALUES ('1', 'Public', 'PinzhiGO', '1', '公开', '0', '2018-05-10 11:03:17', '0', '2018-06-19 10:55:55', '651421238645114');
INSERT INTO `tb_appnamespace` VALUES ('3', 'Pinzhi.Identity', 'PinzhiGO', '0', '品值认证授权中心', '0', '2018-07-19 10:46:32', '651421238645114', '2018-07-19 10:46:32', '651421238645114');
INSERT INTO `tb_appnamespace` VALUES ('8', 'Pinzhi.Platform', 'PinzhiGO', '0', '品值系统管理平台', '0', '2018-11-20 18:06:39', '651421238645114', '2018-11-20 18:06:39', '651421238645114');

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
) ENGINE=InnoDB AUTO_INCREMENT=13311 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_bs_logs
-- ----------------------------

-- ----------------------------
-- Table structure for tb_logs
-- ----------------------------
DROP TABLE IF EXISTS `tb_logs`;
CREATE TABLE `tb_logs` (
  `Id` varchar(50) NOT NULL,
  `ClassName` varchar(100) DEFAULT NULL,
  `ProjectName` varchar(100) DEFAULT NULL,
  `LogTag` varchar(150) DEFAULT NULL,
  `LogType` varchar(20) DEFAULT NULL,
  `LogMessage` varchar(5120) DEFAULT NULL,
  `IP` varchar(20) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_logs
-- ----------------------------

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
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platform_menus
-- ----------------------------
INSERT INTO `tb_platform_menus` VALUES ('1', '系统设置', 'el-icon-setting', null, '3', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('2', '平台菜单', null, '/setting/menu', '99', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('3', '用户设置', null, '/setting/user', '103', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('4', '角色设置', null, '/setting/role', '102', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('5', '平台设置', null, '/setting/platform', '98', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('6', '项目设置', null, '/setting/project', '100', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('7', '项目资源', null, '/setting/apimanage', '101', '1', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('8', '配置中心', 'el-icon-refresh', null, '2', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('9', '项目组管理', null, '/configService/appList', '99', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('10', '项目管理', null, '/configService/projectList', '100', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('11', '配置管理', null, '/configService/configList', '101', '8', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('12', '微服务管理', 'el-icon-upload', null, '1', '0', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('13', '服务管理', null, '/microservice/service', '99', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('14', '网关路由', null, '/microservice/route', '100', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('15', '链路追踪', null, '/microservice/trace', '101', '12', '1', '1');
INSERT INTO `tb_platform_menus` VALUES ('16', '访问量统计', null, '/microservice/access', '102', '12', '1', '1');

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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platforms
-- ----------------------------
INSERT INTO `tb_platforms` VALUES ('1', 'POC综合管理平台', 'PinzhiPOC', null, null, null, null, '1', '2018-06-21 18:26:23', '0');

-- ----------------------------
-- Table structure for tb_projects
-- ----------------------------
DROP TABLE IF EXISTS `tb_projects`;
CREATE TABLE `tb_projects` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) DEFAULT NULL,
  `Key` varchar(255) NOT NULL,
  `AppId` varchar(20) DEFAULT NULL,
  `Secret` varchar(255) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  `RouteKey` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Key`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_projects
-- ----------------------------
INSERT INTO `tb_projects` VALUES ('2', '平台基础管理', 'Pinzhi.Platform', null, null, null, 'Platform');

-- ----------------------------
-- Table structure for tb_role_apis
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_apis`;
CREATE TABLE `tb_role_apis` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApiId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=131 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_apis
-- ----------------------------
INSERT INTO `tb_role_apis` VALUES ('100', '1', '1');
INSERT INTO `tb_role_apis` VALUES ('101', '2', '1');
INSERT INTO `tb_role_apis` VALUES ('102', '3', '1');
INSERT INTO `tb_role_apis` VALUES ('103', '4', '1');
INSERT INTO `tb_role_apis` VALUES ('104', '5', '1');
INSERT INTO `tb_role_apis` VALUES ('105', '6', '1');
INSERT INTO `tb_role_apis` VALUES ('106', '7', '1');
INSERT INTO `tb_role_apis` VALUES ('107', '8', '1');
INSERT INTO `tb_role_apis` VALUES ('108', '9', '1');
INSERT INTO `tb_role_apis` VALUES ('109', '10', '1');
INSERT INTO `tb_role_apis` VALUES ('110', '11', '1');
INSERT INTO `tb_role_apis` VALUES ('111', '12', '1');
INSERT INTO `tb_role_apis` VALUES ('112', '13', '1');
INSERT INTO `tb_role_apis` VALUES ('113', '14', '1');
INSERT INTO `tb_role_apis` VALUES ('114', '17', '1');
INSERT INTO `tb_role_apis` VALUES ('115', '18', '1');
INSERT INTO `tb_role_apis` VALUES ('116', '19', '1');
INSERT INTO `tb_role_apis` VALUES ('117', '20', '1');
INSERT INTO `tb_role_apis` VALUES ('118', '21', '1');
INSERT INTO `tb_role_apis` VALUES ('119', '22', '1');
INSERT INTO `tb_role_apis` VALUES ('120', '23', '1');

-- ----------------------------
-- Table structure for tb_role_menus
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_menus`;
CREATE TABLE `tb_role_menus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MenuId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=227 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_menus
-- ----------------------------
INSERT INTO `tb_role_menus` VALUES ('196', '1', '1');
INSERT INTO `tb_role_menus` VALUES ('197', '2', '1');
INSERT INTO `tb_role_menus` VALUES ('198', '3', '1');
INSERT INTO `tb_role_menus` VALUES ('199', '4', '1');
INSERT INTO `tb_role_menus` VALUES ('200', '5', '1');
INSERT INTO `tb_role_menus` VALUES ('201', '6', '1');
INSERT INTO `tb_role_menus` VALUES ('202', '7', '1');
INSERT INTO `tb_role_menus` VALUES ('203', '8', '1');
INSERT INTO `tb_role_menus` VALUES ('204', '9', '1');
INSERT INTO `tb_role_menus` VALUES ('205', '10', '1');
INSERT INTO `tb_role_menus` VALUES ('206', '11', '1');
INSERT INTO `tb_role_menus` VALUES ('207', '12', '1');
INSERT INTO `tb_role_menus` VALUES ('208', '13', '1');
INSERT INTO `tb_role_menus` VALUES ('209', '14', '1');
INSERT INTO `tb_role_menus` VALUES ('210', '15', '1');
INSERT INTO `tb_role_menus` VALUES ('211', '16', '1');

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
INSERT INTO `tb_roles` VALUES ('1', 'PinzhiPOC', 'admin', '超级管理员', '平台基础管理/超级管理员', '1', '2018-05-17 10:39:30', '2018-11-22 12:30:00', '0');

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
-- Records of tb_thirdoauths
-- ----------------------------

-- ----------------------------
-- Table structure for tb_user_roles
-- ----------------------------
DROP TABLE IF EXISTS `tb_user_roles`;
CREATE TABLE `tb_user_roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Uid` bigint(50) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_user_roles
-- ----------------------------
INSERT INTO `tb_user_roles` VALUES ('5', '651421238645114', '1');

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
INSERT INTO `tb_users` VALUES ('651421238645114', 'admin', '816c62bc3e1dc540d8a409db908f4829f720ae0c9638b3175c47e1358c114ec8', '田亮', '15155010775', '403305950@qq.com', '1', 'akajdksjjda', '2018-05-10 17:08:33', '2018-07-28 13:20:40');
