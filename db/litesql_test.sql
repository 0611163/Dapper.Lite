/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50728
Source Host           : localhost:3306
Source Database       : litesql_test

Target Server Type    : MYSQL
Target Server Version : 50728
File Encoding         : 65001

Date: 2022-08-04 15:59:03
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `bs_order`
-- ----------------------------
DROP TABLE IF EXISTS `bs_order`;
CREATE TABLE `bs_order` (
  `id` varchar(50) NOT NULL COMMENT '主键',
  `order_time` datetime NOT NULL COMMENT '订单时间',
  `amount` decimal(20,2) DEFAULT NULL COMMENT '订单金额',
  `order_userid` bigint(20) NOT NULL COMMENT '下单用户',
  `status` tinyint(4) NOT NULL COMMENT '订单状态(0草稿 1已下单 2已付款 3已发货 4完成)',
  `remark` varchar(255) DEFAULT NULL COMMENT '备注',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单表';

-- ----------------------------
-- Records of bs_order
-- ----------------------------
BEGIN;
INSERT INTO `bs_order` VALUES ('100001', '2022-01-19 12:04:42', '1393.50', '10', '0', '订单已修改70', '10', '2022-01-19 12:04:42', '10', '2022-08-04 15:57:45'), ('100002', '2022-01-19 12:05:23', '17268.42', '10', '0', '测试订单002', '10', '2022-01-19 12:05:23', null, null), ('100003', '2022-01-19 12:05:59', '17268.42', '10', '0', '测试订单003', '10', '2022-01-19 12:05:59', null, null), ('50d2810f286040909f958700f6b67446', '2022-08-04 15:57:45', '17268.42', '10', '0', null, '10', '2022-08-04 15:57:45', null, null), ('73b2c2d8c8d240d395c182f5991c1bd3', '2022-08-04 15:57:45', '17268.42', '10', '0', null, '10', '2022-08-04 15:57:45', null, null), ('e0f39b21fab2493696becab732a1b25d', '2022-08-04 15:57:45', '17268.42', '10', '0', null, '10', '2022-08-04 15:57:45', null, null);
COMMIT;

-- ----------------------------
-- Table structure for `bs_order_detail`
-- ----------------------------
DROP TABLE IF EXISTS `bs_order_detail`;
CREATE TABLE `bs_order_detail` (
  `id` varchar(50) NOT NULL COMMENT '主键',
  `order_id` varchar(50) NOT NULL COMMENT '订单ID',
  `goods_name` varchar(200) NOT NULL COMMENT '商品名称',
  `quantity` int(11) NOT NULL COMMENT '数量',
  `price` decimal(20,2) NOT NULL COMMENT '价格',
  `spec` varchar(100) DEFAULT NULL COMMENT '物品规格',
  `order_num` int(11) DEFAULT NULL COMMENT '排序',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单明细表';

-- ----------------------------
-- Records of bs_order_detail
-- ----------------------------
BEGIN;
INSERT INTO `bs_order_detail` VALUES ('10000201', '100002', '键盘', '11', '123.66', '个', '3', '10', '2022-01-19 12:05:23', null, null), ('10000202', '100002', '鼠标', '12', '50.68', '个', '2', '10', '2022-01-19 12:05:23', null, null), ('10000203', '100002', '电脑', '3', '5100.00', '台', '1', '10', '2022-01-19 12:05:23', null, null), ('10000301', '100003', '鼠标', '12', '50.68', '个', '2', '10', '2022-01-19 12:05:59', null, null), ('10000302', '100003', '电脑', '3', '5100.00', '台', '1', '10', '2022-01-19 12:05:59', null, null), ('10000303', '100003', '键盘', '11', '123.66', '个', '3', '10', '2022-01-19 12:05:59', null, null), ('2f5b7ad742a143c7afd33ada92a6f652', '50d2810f286040909f958700f6b67446', '电脑', '3', '5100.00', '台', '1', '10', '2022-08-04 15:57:45', null, null), ('3259eb99f44d4891adc3100cf803fab7', '73b2c2d8c8d240d395c182f5991c1bd3', '鼠标', '12', '50.68', '个', '2', '10', '2022-08-04 15:57:45', null, null), ('342faba409f043d0ba2055eb74c80853', '50d2810f286040909f958700f6b67446', '键盘', '11', '123.66', '个', '3', '10', '2022-08-04 15:57:45', null, null), ('6d5ff46bb33c4abcbe9d9ed50e04ab57', '100001', '椅子41', '20', '30.23', '把', '5', '10', '2022-08-04 15:57:45', null, null), ('86e64ae82b6f4e3b804ecbe81e6f64e2', '50d2810f286040909f958700f6b67446', '鼠标', '12', '50.68', '个', '2', '10', '2022-08-04 15:57:45', null, null), ('870228558227454586f3340ab85ad9be', 'e0f39b21fab2493696becab732a1b25d', '鼠标', '12', '50.68', '个', '2', '10', '2022-08-04 15:57:45', null, null), ('8858984ded1d458bad134c577796a902', '100001', '桌子40', '10', '78.89', '张', '4', '10', '2022-08-04 15:57:45', null, null), ('97198fc27e2146a1adc50b5e20a1fecc', '73b2c2d8c8d240d395c182f5991c1bd3', '键盘', '11', '123.66', '个', '3', '10', '2022-08-04 15:57:45', null, null), ('b56e24e4716a44f8883dbd473f67a5c2', 'e0f39b21fab2493696becab732a1b25d', '电脑', '3', '5100.00', '台', '1', '10', '2022-08-04 15:57:45', null, null), ('e07504b581dd46c7ae76e9e6f8762478', '73b2c2d8c8d240d395c182f5991c1bd3', '电脑', '3', '5100.00', '台', '1', '10', '2022-08-04 15:57:45', null, null), ('ea1e9765c05641c584a019b268d462d8', 'e0f39b21fab2493696becab732a1b25d', '键盘', '11', '123.66', '个', '3', '10', '2022-08-04 15:57:45', null, null);
COMMIT;

-- ----------------------------
-- Table structure for `sys_user`
-- ----------------------------
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `user_name` varchar(50) NOT NULL COMMENT '用户名',
  `real_name` varchar(50) DEFAULT NULL COMMENT '用户姓名',
  `password` varchar(200) NOT NULL COMMENT '用户密码',
  `remark` varchar(200) DEFAULT NULL COMMENT '备注',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1727895 DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- ----------------------------
-- Records of sys_user
-- ----------------------------
BEGIN;
INSERT INTO `sys_user` VALUES ('1', 'admin', '超级管理员', '123456', '超级管理员', '1', '2020-11-01 13:39:43', '1', '2020-11-01 13:39:47'), ('2', 'admin2020', '普通管理员', '123456', '普通管理员', '1', '2020-11-01 13:42:55', '1', '2020-11-01 13:42:58'), ('9', 'wangwu', '王五', '123456', '测试修改用户02', '1', '2022-01-19 12:10:17', '1', null), ('10', 'zhangsan', '张三', '123456', '测试修改用户6', '1', '2020-11-01 13:40:30', '1', '2022-08-04 15:57:45'), ('11', 'lisi', '李四', '123456', '测试修改用户03', '1', '2020-11-01 13:42:08', '1', '2022-01-17 10:29:55'), ('1727893', 'testUser', '测试插入用户', '123456', null, '1', '2022-08-04 15:57:45', null, null), ('1727894', 'testUser', '测试插入用户', '123456', null, '1', '2022-08-04 15:57:45', null, null);
COMMIT;

-- ----------------------------
-- Table structure for `sys_user_202208`
-- ----------------------------
DROP TABLE IF EXISTS `sys_user_202208`;
CREATE TABLE `sys_user_202208` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '主键',
  `user_name` varchar(50) NOT NULL COMMENT '用户名',
  `real_name` varchar(50) DEFAULT NULL COMMENT '用户姓名',
  `password` varchar(200) NOT NULL COMMENT '用户密码',
  `remark` varchar(200) DEFAULT NULL COMMENT '备注',
  `create_userid` varchar(50) NOT NULL COMMENT '创建者ID',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_userid` varchar(50) DEFAULT NULL COMMENT '更新者ID',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- ----------------------------
-- Records of sys_user_202208
-- ----------------------------
BEGIN;
INSERT INTO `sys_user_202208` VALUES ('1', 'admin', '超级管理员', '123456', '超级管理员', '1', '2020-11-01 13:39:43', '1', '2020-11-01 13:39:47'), ('2', 'admin2020', '普通管理员', '123456', '普通管理员', '1', '2020-11-01 13:42:55', '1', '2020-11-01 13:42:58'), ('9', 'wangwu', '王五', '123456', '测试修改用户02', '1', '2022-01-19 12:10:17', '1', '2022-08-01 16:18:15'), ('10', 'zhangsan', '张三', '123456', '测试修改分表数据51', '1', '2020-11-01 13:40:30', '1', '2022-08-04 15:57:45'), ('11', 'lisi', '李四', '123456', '测试修改用户03', '1', '2020-11-01 13:42:08', '1', '2022-01-17 10:29:55'), ('12', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-01 20:38:22', null, null), ('13', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-01 20:43:18', null, null), ('14', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-01 20:51:12', null, null), ('15', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-01 22:48:44', null, null), ('16', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-04 12:12:06', null, null), ('17', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-04 12:40:18', null, null), ('18', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-04 12:46:31', null, null), ('19', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-04 12:49:33', null, null), ('20', 'testUser', '测试插入分表数据', '123456', '测试插入分表数据', '1', '2022-08-04 13:21:02', null, null);
COMMIT;
