/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50722
Source Host           : 127.0.0.1:3306
Source Database       : bucket

Target Server Type    : MYSQL
Target Server Version : 50722
File Encoding         : 65001

Date: 2018-06-25 17:00:12
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
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_api_resources
-- ----------------------------
INSERT INTO `tb_api_resources` VALUES ('1', 'Pinzhi.Platform', '/Api/QueryApiList', 'GET', 'Api', 'Api资源管理', '查询资源列表', '0', '2', '2018-06-19 15:45:22', '2018-06-19 15:45:24');

-- ----------------------------
-- Table structure for tb_app
-- ----------------------------
DROP TABLE IF EXISTS `tb_app`;
CREATE TABLE `tb_app` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `AppId` varchar(255) NOT NULL,
  `Secret` varchar(255) DEFAULT NULL,
  `Remark` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`Id`,`AppId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_app
-- ----------------------------
INSERT INTO `tb_app` VALUES ('1', '品值综合', 'PinzhiGO', 'R9QaIZTc4WYcPaKFneKu6zKo4F34Vz5R', '品值综合');

-- ----------------------------
-- Table structure for tb_appconfig
-- ----------------------------
DROP TABLE IF EXISTS `tb_appconfig`;
CREATE TABLE `tb_appconfig` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ConfigAppId` varchar(20) DEFAULT NULL,
  `ConfigNamespaceName` varchar(20) DEFAULT NULL,
  `ConfigKey` varchar(100) NOT NULL,
  `ConfigValue` varchar(1024) DEFAULT NULL,
  `Remark` varchar(1024) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `Version` bigint(20) NOT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appconfig
-- ----------------------------
INSERT INTO `tb_appconfig` VALUES ('1', 'PinzhiGO', 'Public', 'RedisDefaultServer', '127.0.0.1:6379,allowadmin=true', 'Redis默认连接地址', '2018-06-14 18:20:07', '2018-05-10 11:05:36', '4', '0');

-- ----------------------------
-- Table structure for tb_appnamespace
-- ----------------------------
DROP TABLE IF EXISTS `tb_appnamespace`;
CREATE TABLE `tb_appnamespace` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL,
  `AppId` varchar(20) DEFAULT NULL,
  `IsPublic` tinyint(1) DEFAULT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `IsDeleted` tinyint(1) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `CreateUid` bigint(20) DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `LastUid` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_appnamespace
-- ----------------------------
INSERT INTO `tb_appnamespace` VALUES ('1', 'Public', 'PinzhiGO', '1', '公开', '0', '2018-05-10 11:03:17', '0', '2018-06-19 10:55:55', '651421238645114');

-- ----------------------------
-- Table structure for tb_logs
-- ----------------------------
DROP TABLE IF EXISTS `tb_logs`;
CREATE TABLE `tb_logs` (
  `Id` varchar(50) NOT NULL,
  `ClassName` varchar(100) DEFAULT NULL,
  `ProjectName` varchar(20) DEFAULT NULL,
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
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;

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
INSERT INTO `tb_platforms` VALUES ('1', 'POC综合管理平台', 'poc', null, null, null, null, '1', '2018-06-21 18:26:23', '0');

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
INSERT INTO `tb_projects` VALUES ('1', 'A+B开放平台', 'abopen', null, null, null, 'ABOpen');
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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_apis
-- ----------------------------
INSERT INTO `tb_role_apis` VALUES ('6', '1', '1');

-- ----------------------------
-- Table structure for tb_role_menus
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_menus`;
CREATE TABLE `tb_role_menus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MenuId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_menus
-- ----------------------------
INSERT INTO `tb_role_menus` VALUES ('87', '1', '1');
INSERT INTO `tb_role_menus` VALUES ('88', '2', '1');
INSERT INTO `tb_role_menus` VALUES ('89', '3', '1');
INSERT INTO `tb_role_menus` VALUES ('90', '4', '1');
INSERT INTO `tb_role_menus` VALUES ('91', '5', '1');
INSERT INTO `tb_role_menus` VALUES ('92', '6', '1');
INSERT INTO `tb_role_menus` VALUES ('93', '7', '1');
INSERT INTO `tb_role_menus` VALUES ('94', '8', '1');
INSERT INTO `tb_role_menus` VALUES ('95', '9', '1');
INSERT INTO `tb_role_menus` VALUES ('96', '10', '1');
INSERT INTO `tb_role_menus` VALUES ('97', '11', '1');
INSERT INTO `tb_role_menus` VALUES ('98', '12', '1');
INSERT INTO `tb_role_menus` VALUES ('99', '13', '1');
INSERT INTO `tb_role_menus` VALUES ('100', '14', '1');
INSERT INTO `tb_role_menus` VALUES ('101', '15', '1');

-- ----------------------------
-- Table structure for tb_roles
-- ----------------------------
DROP TABLE IF EXISTS `tb_roles`;
CREATE TABLE `tb_roles` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ProjectName` varchar(255) DEFAULT NULL,
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
INSERT INTO `tb_roles` VALUES ('1', 'Pinzhi.Platform', 'admin', '超级管理员', '平台基础管理/超级管理员', '1', '2018-05-17 10:39:30', '2018-06-22 09:55:57', '0');

-- ----------------------------
-- Table structure for tb_routes
-- ----------------------------
DROP TABLE IF EXISTS `tb_routes`;
CREATE TABLE `tb_routes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PlatformId` int(11) DEFAULT NULL,
  `RouteContent` text,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_routes
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_user_roles
-- ----------------------------
INSERT INTO `tb_user_roles` VALUES ('4', '651421238645114', '1');

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
INSERT INTO `tb_users` VALUES ('651421238645114', 'admin', 'de87818be47d9548c82a6d94fa9bbd9b0e53a2aeb0ddfae947f28c254e81d69f', '田亮', '15155010775', '403305950@qq.com', '1', '7g97kt6d', '2018-05-10 17:08:33', '2018-06-22 09:29:26');
