/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50728
Source Host           : localhost:3306
Source Database       : litesql_test

Target Server Type    : MYSQL
Target Server Version : 50728
File Encoding         : 65001

Date: 2022-10-26 12:00:15
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `values_test`
-- ----------------------------
DROP TABLE IF EXISTS `values_test`;
CREATE TABLE `values_test` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `bytes_value` blob COMMENT '字节数组测试',
  `byte_value` tinyint(4) DEFAULT NULL COMMENT 'Byte测试',
  `guid_value` varchar(50) DEFAULT NULL COMMENT 'Guid测试',
  `char_value` char(1) DEFAULT NULL,
  `chars_valiue` char(20) DEFAULT NULL,
  `bool_value` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Records of values_test
-- ----------------------------
BEGIN;
INSERT INTO `values_test` VALUES ('1', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', '181d4961-2b35-4a18-96a7-9334740160ba', 'A', 'ABC', '0'), ('2', null, null, null, null, null, null), ('3', null, null, null, null, '', null), ('4', null, null, null, null, '', null), ('5', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', 'd2a29c17-715d-4c87-afdc-a7db2ae10a2b', 'A', 'ABC', '1'), ('6', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', 'ea22e74b-8935-4170-b174-56b4aae065bc', 'A', 'ABC', '0'), ('7', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', 'e417d0ad-0199-405d-90c8-1450f4f8b54b', 'A', 'ABC', '1'), ('8', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', '0d10e8d8-0268-4442-90b2-23c6b533461c', 'A', 'ABC', '0'), ('9', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', 'fe90711d-c903-499c-a3e9-41e69929c183', 'A', 'ABC', '1'), ('10', 0xE5AD97E6AEB5E7B1BBE59E8BE6B58BE8AF95, '123', 'dd86a554-877c-431a-8d7f-9454e67c206b', 'A', 'ABC', '0');
COMMIT;
