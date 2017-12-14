CREATE DATABASE  IF NOT EXISTS `mytchatroomdb` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `mytchatroomdb`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: mytchatroomdb
-- ------------------------------------------------------
-- Server version	5.7.20-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `grades`
--

DROP TABLE IF EXISTS `grades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `grades` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `NAME` varchar(25) NOT NULL,
  `COLOR` varchar(7) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `grades_has_permissions`
--

DROP TABLE IF EXISTS `grades_has_permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `grades_has_permissions` (
  `GRADES_ID` int(11) NOT NULL,
  `PERMISSIONS_CODE` int(11) NOT NULL,
  PRIMARY KEY (`GRADES_ID`,`PERMISSIONS_CODE`),
  KEY `fk_GRADES_has_PERMISSIONS_PERMISSIONS1_idx` (`PERMISSIONS_CODE`),
  KEY `fk_GRADES_has_PERMISSIONS_GRADES1_idx` (`GRADES_ID`),
  CONSTRAINT `fk_GRADES_has_PERMISSIONS_GRADES1` FOREIGN KEY (`GRADES_ID`) REFERENCES `grades` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_GRADES_has_PERMISSIONS_PERMISSIONS1` FOREIGN KEY (`PERMISSIONS_CODE`) REFERENCES `permissions` (`CODE`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `images`
--

DROP TABLE IF EXISTS `images`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `images` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `IMAGE` blob NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `messages_for_rooms`
--

DROP TABLE IF EXISTS `messages_for_rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `messages_for_rooms` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `MESSAGE` longtext NOT NULL,
  `DATE_SEND` datetime NOT NULL,
  `USERS_ID` int(11) NOT NULL,
  `ROOMS_ID` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_MESSAGES_FOR_ROOMS_ROOMS1_idx` (`ROOMS_ID`),
  KEY `fk_MESSAGES_FOR_ROOMS_USERS1_idx` (`USERS_ID`),
  CONSTRAINT `fk_MESSAGES_FOR_ROOMS_ROOMS1` FOREIGN KEY (`ROOMS_ID`) REFERENCES `rooms` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_MESSAGES_FOR_ROOMS_USERS1` FOREIGN KEY (`USERS_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `messages_for_users`
--

DROP TABLE IF EXISTS `messages_for_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `messages_for_users` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `MESSAGE` longtext NOT NULL,
  `DATE_SEND` datetime NOT NULL,
  `READ` datetime DEFAULT NULL,
  `SENDER_ID` int(11) NOT NULL,
  `RECEVER_ID` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_MESSAGES_USERS1_idx` (`SENDER_ID`),
  KEY `fk_MESSAGES_USERS2_idx` (`RECEVER_ID`),
  CONSTRAINT `fk_MESSAGES_USERS1` FOREIGN KEY (`SENDER_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_MESSAGES_USERS2` FOREIGN KEY (`RECEVER_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permissions`
--

DROP TABLE IF EXISTS `permissions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `permissions` (
  `CODE` int(11) NOT NULL,
  `LABEL` varchar(45) NOT NULL,
  PRIMARY KEY (`CODE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rooms` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `NAME` varchar(30) NOT NULL,
  `DESCRIPTION` longtext,
  `RECRUITMENT_ON` tinyint(4) NOT NULL,
  `MEMBER_LIMIT` int(11) NOT NULL,
  `DATE_CREATION` date NOT NULL,
  `IMAGES_ID` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `images_id1_idx` (`IMAGES_ID`),
  CONSTRAINT `fk_IMAGES_ID1` FOREIGN KEY (`IMAGES_ID`) REFERENCES `images` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rooms_has_users`
--

DROP TABLE IF EXISTS `rooms_has_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rooms_has_users` (
  `ROOMS_ID` int(11) NOT NULL,
  `USERS_ID` int(11) NOT NULL,
  `GRADES_ID` int(11) DEFAULT NULL,
  `LAST_CHECK` datetime NOT NULL,
  `STATUT` enum('Créateur','Membre','En attente','Banni') NOT NULL,
  PRIMARY KEY (`ROOMS_ID`,`USERS_ID`),
  KEY `fk_ROOMS_has_USERS_USERS1_idx` (`USERS_ID`),
  KEY `fk_ROOMS_has_USERS_ROOMS1_idx` (`ROOMS_ID`),
  KEY `fk_ROOMS_HAS_USERS_GRADES1_idx` (`GRADES_ID`),
  CONSTRAINT `fk_ROOMS_HAS_USERS_GRADES1` FOREIGN KEY (`GRADES_ID`) REFERENCES `grades` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ROOMS_has_USERS_ROOMS1` FOREIGN KEY (`ROOMS_ID`) REFERENCES `rooms` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ROOMS_has_USERS_USERS1` FOREIGN KEY (`USERS_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `USERNAME` varchar(20) NOT NULL,
  `PSSW` varchar(257) NOT NULL,
  `EMAIL` varchar(45) NOT NULL,
  `PHONE` varchar(15) DEFAULT NULL,
  `DESCRIPTION` longtext,
  `HOBBIES` longtext,
  `STATUT` enum('En ligne','Absent','Ne pas déranger','Invisible','Hors-ligne') NOT NULL DEFAULT 'Hors-ligne',
  `AVATAR_ID` int(11) DEFAULT NULL,
  `BACKGROUND_ID` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `EMAIL_UNIQUE` (`EMAIL`),
  UNIQUE KEY `USERNAME_UNIQUE` (`USERNAME`),
  KEY `fk_IMAGES_ID1_idx` (`AVATAR_ID`),
  KEY `fk_BACKGROUND_ID1_idx` (`BACKGROUND_ID`),
  CONSTRAINT `fk_AVATAR_ID2` FOREIGN KEY (`AVATAR_ID`) REFERENCES `images` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_BACKGROUND_ID1` FOREIGN KEY (`BACKGROUND_ID`) REFERENCES `images` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users_has_friends`
--

DROP TABLE IF EXISTS `users_has_friends`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users_has_friends` (
  `USERS_ID` int(11) NOT NULL,
  `USERS_FRIEND_ID` int(11) NOT NULL,
  `STATUT` enum('Ami','En attente','Muet','Bloqué') NOT NULL,
  `MESSAGE` longtext,
  PRIMARY KEY (`USERS_ID`,`USERS_FRIEND_ID`),
  KEY `fk_USERS_has_USERS_USERS2_idx` (`USERS_FRIEND_ID`),
  KEY `fk_USERS_has_USERS_USERS1_idx` (`USERS_ID`),
  CONSTRAINT `fk_USERS_has_USERS_USERS1` FOREIGN KEY (`USERS_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_USERS_has_USERS_USERS2` FOREIGN KEY (`USERS_FRIEND_ID`) REFERENCES `users` (`ID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-12-14 12:47:02
