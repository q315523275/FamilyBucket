/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50722
Source Host           : 127.0.0.1:3306
Source Database       : bucket

Target Server Type    : MYSQL
Target Server Version : 50722
File Encoding         : 65001

Date: 2018-05-15 10:45:54
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
  `Action` varchar(50) DEFAULT NULL COMMENT '执行方法',
  `ActionName` varchar(500) DEFAULT NULL,
  `Controller` varchar(50) DEFAULT NULL,
  `ControllerName` varchar(100) DEFAULT NULL,
  `Message` varchar(50) DEFAULT NULL,
  `Disabled` tinyint(4) DEFAULT NULL,
  `IsAnonymous` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_api_resources
-- ----------------------------

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
INSERT INTO `tb_appconfig` VALUES ('1', 'PinzhiGO', 'Public', 'test', '配置值', '备注', '2018-05-10 11:05:34', '2018-05-10 11:05:36', '1', '0');

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
INSERT INTO `tb_appnamespace` VALUES ('1', 'Public', 'PinzhiGO', '1', '公开', '0', '2018-05-10 11:03:17', '0', '2018-05-10 11:03:20', '0');

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platform_menus
-- ----------------------------

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_platforms
-- ----------------------------

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_projects
-- ----------------------------

-- ----------------------------
-- Table structure for tb_role_apis
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_apis`;
CREATE TABLE `tb_role_apis` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApiId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_apis
-- ----------------------------

-- ----------------------------
-- Table structure for tb_role_menus
-- ----------------------------
DROP TABLE IF EXISTS `tb_role_menus`;
CREATE TABLE `tb_role_menus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MenuId` int(11) DEFAULT NULL,
  `RoleId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_role_menus
-- ----------------------------

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
  `CheckApi` tinyint(4) DEFAULT NULL,
  `IsCommon` tinyint(4) DEFAULT NULL,
  `IsSys` tinyint(4) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  `IsDel` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`Id`,`Key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_roles
-- ----------------------------

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_user_roles
-- ----------------------------

-- ----------------------------
-- Table structure for tb_users
-- ----------------------------
DROP TABLE IF EXISTS `tb_users`;
CREATE TABLE `tb_users` (
  `Id` bigint(30) NOT NULL,
  `UserName` varchar(30) DEFAULT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `RealName` varchar(20) DEFAULT NULL,
  `Mobile` varchar(20) DEFAULT NULL,
  `State` int(11) DEFAULT NULL,
  `Salt` varchar(20) DEFAULT NULL,
  `ProvinceId` varchar(50) DEFAULT NULL,
  `Province` varchar(255) DEFAULT NULL,
  `CityId` varchar(50) DEFAULT NULL,
  `City` varchar(255) DEFAULT NULL,
  `DistrictId` varchar(50) DEFAULT NULL,
  `District` varchar(255) DEFAULT NULL,
  `Channel` int(11) DEFAULT NULL,
  `Store` int(11) DEFAULT NULL,
  `StoreName` varchar(50) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `UpdateTime` datetime DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tb_users
-- ----------------------------
INSERT INTO `tb_users` VALUES ('651421238645114', '15155010775', null, '田亮', '15155010775', '1', 'lahvikbelxzkh', null, null, null, null, null, null, null, null, null, '2018-05-10 17:08:33', '2018-05-10 17:08:38', '403305950@qq.com');
