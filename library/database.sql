# ************************************************************
# Sequel Pro SQL dump
# Version 4096
#
# http://www.sequelpro.com/
# http://code.google.com/p/sequel-pro/
#
# Host: 127.0.0.1 (MySQL 5.5.27)
# Database: gts
# Generation Time: 2014-07-22 15:58:36 +0000
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


# Dump of table BattleVideoCrawlQueue
# ------------------------------------------------------------

DROP TABLE IF EXISTS `BattleVideoCrawlQueue`;

CREATE TABLE `BattleVideoCrawlQueue` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `SerialNumber` bigint(11) unsigned DEFAULT NULL,
  `Timestamp` datetime DEFAULT NULL,
  `Complete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table BattleVideoCrawlQueue5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `BattleVideoCrawlQueue5`;

CREATE TABLE `BattleVideoCrawlQueue5` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `SerialNumber` bigint(11) unsigned DEFAULT NULL,
  `Timestamp` datetime DEFAULT NULL,
  `Complete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table BattleVideoSearchHistory
# ------------------------------------------------------------

DROP TABLE IF EXISTS `BattleVideoSearchHistory`;

CREATE TABLE `BattleVideoSearchHistory` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Metagame` int(11) DEFAULT NULL,
  `Species` int(11) DEFAULT NULL,
  `Country` int(11) DEFAULT NULL,
  `Region` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table BattleVideoSearchHistory5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `BattleVideoSearchHistory5`;

CREATE TABLE `BattleVideoSearchHistory5` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Metagame` int(11) DEFAULT NULL,
  `Species` int(11) DEFAULT NULL,
  `Country` int(11) DEFAULT NULL,
  `Region` int(11) DEFAULT NULL,
  `Special` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsCountries
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsCountries`;

CREATE TABLE `FoundationsCountries` (
  `Value` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Value`),
  KEY `Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsCountryRegions
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsCountryRegions`;

CREATE TABLE `FoundationsCountryRegions` (
  `country_id` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Value` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Name` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`country_id`,`Value`),
  KEY `Country_ID` (`country_id`),
  KEY `Country_ID_2` (`country_id`,`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsItems
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsItems`;

CREATE TABLE `FoundationsItems` (
  `Value` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Value`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsMoves
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsMoves`;

CREATE TABLE `FoundationsMoves` (
  `Value` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(30) DEFAULT NULL,
  `Type` int(11) DEFAULT NULL,
  `DamageClass` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`Value`),
  KEY `Type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsPokemon
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsPokemon`;

CREATE TABLE `FoundationsPokemon` (
  `NationalDex` int(11) unsigned NOT NULL,
  `Name` varchar(36) NOT NULL DEFAULT '',
  `Experience` int(11) NOT NULL,
  PRIMARY KEY (`NationalDex`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsPokemonForms
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsPokemonForms`;

CREATE TABLE `FoundationsPokemonForms` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `NationalDex` int(11) unsigned NOT NULL,
  `GenderRatio` tinyint(3) unsigned DEFAULT NULL,
  `FormValue` tinyint(3) unsigned NOT NULL,
  `FormName` varchar(30) DEFAULT NULL,
  `FormSuffix` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `NationalDex` (`NationalDex`),
  CONSTRAINT `foundationspokemonforms_ibfk_1` FOREIGN KEY (`NationalDex`) REFERENCES `FoundationsPokemon` (`NationalDex`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsPokemonStats2
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsPokemonStats2`;

CREATE TABLE `FoundationsPokemonStats2` (
  `form_id` int(11) unsigned NOT NULL,
  `Type1` int(11) DEFAULT NULL,
  `Type2` int(11) DEFAULT NULL,
  `BaseHP` int(11) DEFAULT NULL,
  `BaseAttack` int(11) DEFAULT NULL,
  `BaseDefense` int(11) DEFAULT NULL,
  `BaseSpeed` int(11) DEFAULT NULL,
  `BaseSpAttack` int(11) DEFAULT NULL,
  `BaseSpDefense` int(11) DEFAULT NULL,
  PRIMARY KEY (`form_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table FoundationsTypes
# ------------------------------------------------------------

DROP TABLE IF EXISTS `FoundationsTypes`;

CREATE TABLE `FoundationsTypes` (
  `Value` int(11) NOT NULL,
  `Name` varchar(30) NOT NULL DEFAULT '',
  `DamageClass` tinyint(3) unsigned DEFAULT NULL,
  PRIMARY KEY (`Value`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsHistory4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsHistory4`;

CREATE TABLE `GtsHistory4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  `Gender` tinyint(3) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `RequestedSpecies` smallint(5) unsigned NOT NULL,
  `RequestedGender` tinyint(3) unsigned NOT NULL,
  `RequestedMinLevel` tinyint(3) unsigned NOT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned NOT NULL,
  `Unknown1` tinyint(3) unsigned NOT NULL,
  `TrainerGender` tinyint(3) unsigned NOT NULL,
  `Unknown2` tinyint(3) unsigned NOT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeExchanged` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TrainerOT` smallint(5) unsigned NOT NULL,
  `TrainerCountry` tinyint(3) unsigned NOT NULL,
  `TrainerRegion` tinyint(3) unsigned NOT NULL,
  `TrainerClass` tinyint(3) unsigned NOT NULL,
  `IsExchanged` tinyint(3) unsigned NOT NULL,
  `TrainerVersion` tinyint(3) unsigned NOT NULL,
  `TrainerLanguage` tinyint(3) unsigned NOT NULL,
  `ParseVersion` int(11) unsigned DEFAULT NULL,
  `TimeWithdrawn` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsHistory5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsHistory5`;

CREATE TABLE `GtsHistory5` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `Unknown0` blob NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  `Gender` tinyint(3) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `RequestedSpecies` smallint(5) unsigned NOT NULL,
  `RequestedGender` tinyint(3) unsigned NOT NULL,
  `RequestedMinLevel` tinyint(3) unsigned NOT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned NOT NULL,
  `Unknown1` tinyint(3) unsigned NOT NULL,
  `TrainerGender` tinyint(3) unsigned NOT NULL,
  `Unknown2` tinyint(3) unsigned NOT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeExchanged` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerOT` int(11) unsigned NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TrainerCountry` tinyint(3) unsigned NOT NULL,
  `TrainerRegion` tinyint(3) unsigned NOT NULL,
  `TrainerClass` tinyint(3) unsigned NOT NULL,
  `IsExchanged` tinyint(3) unsigned NOT NULL,
  `TrainerVersion` tinyint(3) unsigned NOT NULL,
  `TrainerLanguage` tinyint(3) unsigned NOT NULL,
  `TrainerBadges` tinyint(3) unsigned NOT NULL,
  `TrainerUnityTower` tinyint(3) unsigned NOT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `TimeWithdrawn` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsPokemon4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsPokemon4`;

CREATE TABLE `GtsPokemon4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  `Gender` tinyint(3) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `RequestedSpecies` smallint(5) unsigned NOT NULL,
  `RequestedGender` tinyint(3) unsigned NOT NULL,
  `RequestedMinLevel` tinyint(3) unsigned NOT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned NOT NULL,
  `Unknown1` tinyint(3) unsigned NOT NULL,
  `TrainerGender` tinyint(3) unsigned NOT NULL,
  `Unknown2` tinyint(3) unsigned NOT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeExchanged` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TrainerOT` smallint(5) unsigned NOT NULL,
  `TrainerCountry` tinyint(3) unsigned NOT NULL,
  `TrainerRegion` tinyint(3) unsigned NOT NULL,
  `TrainerClass` tinyint(3) unsigned NOT NULL,
  `IsExchanged` tinyint(3) unsigned NOT NULL,
  `TrainerVersion` tinyint(3) unsigned NOT NULL,
  `TrainerLanguage` tinyint(3) unsigned NOT NULL,
  `ParseVersion` int(11) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsPokemon5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsPokemon5`;

CREATE TABLE `GtsPokemon5` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `Unknown0` blob NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  `Gender` tinyint(3) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `RequestedSpecies` smallint(5) unsigned NOT NULL,
  `RequestedGender` tinyint(3) unsigned NOT NULL,
  `RequestedMinLevel` tinyint(3) unsigned NOT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned NOT NULL,
  `Unknown1` tinyint(3) unsigned NOT NULL,
  `TrainerGender` tinyint(3) unsigned NOT NULL,
  `Unknown2` tinyint(3) unsigned NOT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeExchanged` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerOT` int(11) unsigned NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TrainerCountry` tinyint(3) unsigned NOT NULL,
  `TrainerRegion` tinyint(3) unsigned NOT NULL,
  `TrainerClass` tinyint(3) unsigned NOT NULL,
  `IsExchanged` tinyint(3) unsigned NOT NULL,
  `TrainerVersion` tinyint(3) unsigned NOT NULL,
  `TrainerLanguage` tinyint(3) unsigned NOT NULL,
  `TrainerBadges` tinyint(3) unsigned NOT NULL,
  `TrainerUnityTower` tinyint(3) unsigned NOT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsProfiles4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsProfiles4`;

CREATE TABLE `GtsProfiles4` (
  `pid` int(11) NOT NULL,
  `Data` blob NOT NULL,
  `Version` tinyint(3) unsigned NOT NULL,
  `Language` tinyint(3) unsigned NOT NULL,
  `Country` tinyint(3) unsigned NOT NULL,
  `Region` tinyint(3) unsigned NOT NULL,
  `OT` int(10) unsigned NOT NULL,
  `Name` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `ParseVersion` int(10) unsigned NOT NULL,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsProfiles5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsProfiles5`;

CREATE TABLE `GtsProfiles5` (
  `pid` int(11) NOT NULL,
  `Data` blob NOT NULL,
  `Version` tinyint(3) unsigned NOT NULL,
  `Language` tinyint(3) unsigned NOT NULL,
  `Country` tinyint(3) unsigned NOT NULL,
  `Region` tinyint(3) unsigned NOT NULL,
  `OT` int(11) unsigned NOT NULL,
  `Name` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `ParseVersion` int(11) unsigned NOT NULL,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalBattleVideoPokemon4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalBattleVideoPokemon4`;

CREATE TABLE `TerminalBattleVideoPokemon4` (
  `video_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`video_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalbattlevideopokemon4_ibfk_1` FOREIGN KEY (`video_id`) REFERENCES `TerminalBattleVideos4` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalBattleVideoPokemon5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalBattleVideoPokemon5`;

CREATE TABLE `TerminalBattleVideoPokemon5` (
  `video_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`video_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalbattlevideopokemon5_ibfk_1` FOREIGN KEY (`video_id`) REFERENCES `TerminalBattleVideos5` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalBattleVideos4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalBattleVideos4`;

CREATE TABLE `TerminalBattleVideos4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL,
  `SerialNumber` bigint(20) unsigned DEFAULT NULL,
  `Header` blob NOT NULL,
  `Data` blob NOT NULL,
  `md5` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TimeAdded` datetime NOT NULL,
  `ParseVersion` int(10) unsigned NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `Streak` smallint(5) unsigned DEFAULT NULL,
  `Metagame` tinyint(3) unsigned NOT NULL,
  `Country` tinyint(3) unsigned NOT NULL,
  `Region` tinyint(3) unsigned NOT NULL,
  `Views` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `SerialNumber` (`SerialNumber`),
  KEY `TimeAdded` (`TimeAdded`),
  KEY `Metagame` (`Metagame`),
  KEY `pid` (`pid`),
  KEY `Country` (`Country`,`Region`),
  KEY `md5` (`md5`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalBattleVideos5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalBattleVideos5`;

CREATE TABLE `TerminalBattleVideos5` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL,
  `SerialNumber` bigint(20) unsigned DEFAULT NULL,
  `Header` blob NOT NULL,
  `Data` blob NOT NULL,
  `md5` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TimeAdded` datetime NOT NULL,
  `ParseVersion` int(10) unsigned NOT NULL,
  `TrainerName` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `Streak` smallint(5) unsigned DEFAULT NULL,
  `Metagame` tinyint(3) unsigned NOT NULL,
  `Country` tinyint(3) unsigned NOT NULL,
  `Region` tinyint(3) unsigned NOT NULL,
  `Views` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `SerialNumber` (`SerialNumber`),
  KEY `TimeAdded` (`TimeAdded`),
  KEY `Metagame` (`Metagame`),
  KEY `pid` (`pid`),
  KEY `Country` (`Country`,`Region`),
  KEY `md5` (`md5`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalBoxes4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalBoxes4`;

CREATE TABLE `TerminalBoxes4` (
  `pid` int(11) NOT NULL,
  `SerialNumber` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `md5` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TimeAdded` datetime NOT NULL,
  `ParseVersion` int(11) unsigned NOT NULL,
  `Label` int(11) unsigned NOT NULL,
  PRIMARY KEY (`SerialNumber`),
  KEY `md5` (`md5`),
  KEY `Label` (`Label`),
  KEY `TimeAdded` (`TimeAdded`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalDressup4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalDressup4`;

CREATE TABLE `TerminalDressup4` (
  `pid` int(11) NOT NULL,
  `SerialNumber` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `md5` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TimeAdded` datetime NOT NULL,
  `ParseVersion` int(11) unsigned NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`SerialNumber`),
  KEY `md5` (`md5`),
  KEY `Species` (`Species`),
  KEY `TimeAdded` (`TimeAdded`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalMusicalPokemon5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalMusicalPokemon5`;

CREATE TABLE `TerminalMusicalPokemon5` (
  `musical_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(6) unsigned NOT NULL,
  PRIMARY KEY (`musical_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalmusicalpokemon5_ibfk_1` FOREIGN KEY (`musical_id`) REFERENCES `TerminalMusicals5` (`SerialNumber`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table TerminalMusicals5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `TerminalMusicals5`;

CREATE TABLE `TerminalMusicals5` (
  `pid` int(11) NOT NULL,
  `SerialNumber` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob NOT NULL,
  `md5` binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `TimeAdded` datetime NOT NULL,
  `ParseVersion` int(11) unsigned NOT NULL,
  PRIMARY KEY (`SerialNumber`),
  KEY `md5` (`md5`),
  KEY `TimeAdded` (`TimeAdded`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;




/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
