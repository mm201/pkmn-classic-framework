# ************************************************************
# Sequel Pro SQL dump
# Version 4096
#
# http://www.sequelpro.com/
# http://code.google.com/p/sequel-pro/
#
# Host: 127.0.0.1 (MySQL 5.5.27)
# Database: gts
# Generation Time: 2014-05-05 01:14:25 +0000
# ************************************************************


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


# Dump of table GtsPokemon4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsPokemon4`;

CREATE TABLE `GtsPokemon4` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Level` tinyint(3) unsigned DEFAULT NULL,
  `RequestedSpecies` smallint(5) unsigned DEFAULT NULL,
  `RequestedGender` tinyint(3) unsigned DEFAULT NULL,
  `RequestedMinLevel` tinyint(3) unsigned DEFAULT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned DEFAULT NULL,
  `Unknown1` tinyint(3) unsigned DEFAULT NULL,
  `TrainerGender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeWithdrawn` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerName` blob,
  `TrainerOT` smallint(5) unsigned DEFAULT NULL,
  `TrainerCountry` tinyint(3) unsigned DEFAULT NULL,
  `TrainerRegion` tinyint(3) unsigned DEFAULT NULL,
  `TrainerClass` tinyint(3) unsigned DEFAULT NULL,
  `IsExchanged` tinyint(3) unsigned DEFAULT NULL,
  `TrainerVersion` tinyint(3) unsigned DEFAULT NULL,
  `TrainerLanguage` tinyint(3) unsigned DEFAULT NULL,
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
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Data` blob,
  `Unknown0` blob,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Level` tinyint(3) unsigned DEFAULT NULL,
  `RequestedSpecies` smallint(5) unsigned DEFAULT NULL,
  `RequestedGender` tinyint(3) unsigned DEFAULT NULL,
  `RequestedMinLevel` tinyint(3) unsigned DEFAULT NULL,
  `RequestedMaxLevel` tinyint(3) unsigned DEFAULT NULL,
  `Unknown1` tinyint(3) unsigned DEFAULT NULL,
  `TrainerGender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `TimeDeposited` datetime DEFAULT NULL,
  `TimeWithdrawn` datetime DEFAULT NULL,
  `pid` int(11) NOT NULL,
  `TrainerOT` int(11) unsigned DEFAULT NULL,
  `TrainerName` blob,
  `TrainerCountry` tinyint(3) unsigned DEFAULT NULL,
  `TrainerRegion` tinyint(3) unsigned DEFAULT NULL,
  `TrainerClass` tinyint(3) unsigned DEFAULT NULL,
  `IsExchanged` tinyint(3) unsigned DEFAULT NULL,
  `TrainerVersion` tinyint(3) unsigned DEFAULT NULL,
  `TrainerLanguage` tinyint(3) unsigned DEFAULT NULL,
  `TrainerBadges` tinyint(3) unsigned DEFAULT NULL,
  `TrainerUnityTower` tinyint(3) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;




/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
