/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50728
Source Host           : localhost:3306
Source Database       : litesql_test

Target Server Type    : MYSQL
Target Server Version : 50728
File Encoding         : 65001

Date: 2023-12-08 13:30:02
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Procedure structure for `query_user`
-- ----------------------------
DROP PROCEDURE IF EXISTS `query_user`;
DELIMITER ;;
CREATE DEFINER=`root`@`%` PROCEDURE `query_user`(in maxId BIGINT)
BEGIN
    SELECT * from sys_user where id<=maxId;
END
;;
DELIMITER ;
