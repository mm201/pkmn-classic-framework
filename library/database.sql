# ************************************************************
# Sequel Pro SQL dump
# Version 4541
#
# http://www.sequelpro.com/
# https://github.com/sequelpro/sequelpro
#
# Host: hoenn.local (MySQL 5.5.5-10.3.10-MariaDB)
# Database: gts
# Generation Time: 2021-02-01 07:35:50 +0000
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



# Dump of table GtsBattleSubway5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleSubway5`;

CREATE TABLE `GtsBattleSubway5` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL,
  `Name` binary(16) DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `PhraseLeader` binary(8) DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `PhraseChallenged` binary(8) DEFAULT NULL,
  `PhraseWon` binary(8) DEFAULT NULL,
  `PhraseLost` binary(8) DEFAULT NULL,
  `Unknown3` smallint(5) unsigned DEFAULT NULL,
  `Unknown4` binary(5) DEFAULT NULL,
  `Unknown5` bigint(20) DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `Rank` tinyint(3) unsigned DEFAULT NULL,
  `RoomNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `BattlesWon` tinyint(3) unsigned DEFAULT NULL,
  `Position` int(10) unsigned DEFAULT NULL,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `RoomNum` (`RoomNum`,`Rank`,`Position`),
  KEY `pid` (`pid`,`RoomNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsBattleSubwayLeaders5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleSubwayLeaders5`;

CREATE TABLE `GtsBattleSubwayLeaders5` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL DEFAULT 0,
  `Name` binary(16) DEFAULT NULL,
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `PhraseLeader` binary(8) DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `Rank` tinyint(3) unsigned DEFAULT NULL,
  `RoomNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `RoomNum` (`RoomNum`,`Rank`,`TimeAdded`),
  KEY `pid` (`pid`,`RoomNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsBattleSubwayPokemon5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleSubwayPokemon5`;

CREATE TABLE `GtsBattleSubwayPokemon5` (
  `party_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `HeldItem` smallint(5) unsigned DEFAULT NULL,
  `Move1` smallint(5) unsigned DEFAULT NULL,
  `Move2` smallint(5) unsigned DEFAULT NULL,
  `Move3` smallint(5) unsigned DEFAULT NULL,
  `Move4` smallint(5) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `Personality` int(10) unsigned DEFAULT NULL,
  `IVs` int(10) unsigned DEFAULT NULL,
  `EVs` binary(6) DEFAULT NULL,
  `Unknown1` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Ability` tinyint(3) unsigned DEFAULT NULL,
  `Happiness` tinyint(3) unsigned DEFAULT NULL,
  `Nickname` binary(22) DEFAULT NULL,
  `Unknown2` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`party_id`,`Slot`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsBattleTower4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleTower4`;

CREATE TABLE `GtsBattleTower4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL,
  `Name` binary(16) DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `PhraseLeader` binary(8) DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `PhraseChallenged` binary(8) DEFAULT NULL,
  `PhraseWon` binary(8) DEFAULT NULL,
  `PhraseLost` binary(8) DEFAULT NULL,
  `Unknown3` smallint(5) unsigned DEFAULT NULL,
  `Unknown5` bigint(20) DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `Rank` tinyint(3) unsigned DEFAULT NULL,
  `RoomNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `BattlesWon` tinyint(3) unsigned DEFAULT NULL,
  `Position` int(10) unsigned DEFAULT NULL,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `RoomNum` (`RoomNum`,`Rank`,`Position`),
  KEY `pid` (`pid`,`RoomNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsBattleTowerLeaders4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleTowerLeaders4`;

CREATE TABLE `GtsBattleTowerLeaders4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `pid` int(11) NOT NULL DEFAULT 0,
  `Name` binary(16) DEFAULT NULL,
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `PhraseLeader` binary(8) DEFAULT NULL,
  `Gender` tinyint(3) unsigned DEFAULT NULL,
  `Unknown2` tinyint(3) unsigned DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `Rank` tinyint(3) unsigned DEFAULT NULL,
  `RoomNum` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `TimeAdded` datetime DEFAULT NULL,
  `TimeUpdated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `RoomNum` (`RoomNum`,`Rank`,`TimeAdded`),
  KEY `pid` (`pid`,`RoomNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsBattleTowerPokemon4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsBattleTowerPokemon4`;

CREATE TABLE `GtsBattleTowerPokemon4` (
  `party_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `HeldItem` smallint(5) unsigned DEFAULT NULL,
  `Move1` smallint(5) unsigned DEFAULT NULL,
  `Move2` smallint(5) unsigned DEFAULT NULL,
  `Move3` smallint(5) unsigned DEFAULT NULL,
  `Move4` smallint(5) unsigned DEFAULT NULL,
  `TrainerID` int(10) unsigned DEFAULT NULL,
  `Personality` int(10) unsigned DEFAULT NULL,
  `IVs` int(10) unsigned DEFAULT NULL,
  `EVs` binary(6) DEFAULT NULL,
  `Unknown1` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Ability` tinyint(3) unsigned DEFAULT NULL,
  `Happiness` tinyint(3) unsigned DEFAULT NULL,
  `Nickname` binary(22) DEFAULT NULL,
  PRIMARY KEY (`party_id`,`Slot`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsHistory4
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsHistory4`;

CREATE TABLE `GtsHistory4` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `trade_id` bigint(20) unsigned DEFAULT NULL,
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
  `partner_pid` int(11) DEFAULT NULL,
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
  `trade_id` bigint(20) unsigned DEFAULT NULL,
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
  `partner_pid` int(11) DEFAULT NULL,
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
  `Data` blob DEFAULT NULL,
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `OT` int(10) unsigned DEFAULT NULL,
  `Name` binary(16) DEFAULT NULL,
  `MacAddress` binary(6) DEFAULT NULL,
  `Email` varchar(64) DEFAULT NULL,
  `HasNotifications` bit(1) DEFAULT NULL,
  `ClientSecret` smallint(6) DEFAULT NULL,
  `MailSecret` smallint(6) DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `TimeAdded` datetime NOT NULL,
  `TimeUpdated` datetime NOT NULL,
  `TimeLastSearch` datetime DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table GtsProfiles5
# ------------------------------------------------------------

DROP TABLE IF EXISTS `GtsProfiles5`;

CREATE TABLE `GtsProfiles5` (
  `pid` int(11) NOT NULL,
  `Data` blob DEFAULT NULL,
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `OT` int(10) unsigned DEFAULT NULL,
  `Name` binary(16) DEFAULT NULL,
  `MacAddress` binary(6) DEFAULT NULL,
  `Email` varchar(64) DEFAULT NULL,
  `HasNotifications` bit(1) DEFAULT NULL,
  `ClientSecret` smallint(6) DEFAULT NULL,
  `MailSecret` smallint(6) DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `TimeAdded` datetime NOT NULL,
  `TimeUpdated` datetime NOT NULL,
  `TimeLastSearch` datetime DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_plaza_profiles
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_plaza_profiles`;

CREATE TABLE `pkmncf_plaza_profiles` (
  `pid` int(11) NOT NULL,
  `DataPrefix` blob DEFAULT NULL,
  `Data` blob DEFAULT NULL,
  `Version` tinyint(3) unsigned DEFAULT NULL,
  `Language` tinyint(3) unsigned DEFAULT NULL,
  `Country` tinyint(3) unsigned DEFAULT NULL,
  `Region` tinyint(3) unsigned DEFAULT NULL,
  `OT` int(10) unsigned DEFAULT NULL,
  `Name` binary(16) DEFAULT NULL,
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `TimeAdded` datetime NOT NULL,
  `TimeUpdated` datetime NOT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_abilities
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_abilities`;

CREATE TABLE `pkmncf_pokedex_abilities` (
  `Value` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Value`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_countries
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_countries`;

CREATE TABLE `pkmncf_pokedex_countries` (
  `id` int(10) unsigned NOT NULL DEFAULT 0,
  `Value4` tinyint(3) unsigned DEFAULT 0,
  `Value5` tinyint(3) unsigned DEFAULT NULL,
  `iso-3166-1` char(2) DEFAULT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Name` (`Name_JA`),
  KEY `Value4` (`Value4`),
  KEY `Value5` (`Value5`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_country_regions
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_country_regions`;

CREATE TABLE `pkmncf_pokedex_country_regions` (
  `id` int(10) unsigned NOT NULL DEFAULT 0,
  `country_id` int(10) unsigned NOT NULL DEFAULT 0,
  `Value4` tinyint(3) unsigned NOT NULL DEFAULT 0,
  `Value5` tinyint(3) unsigned DEFAULT NULL,
  `iso-3166-2` varchar(4) DEFAULT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `country_id` (`country_id`),
  KEY `country_id_2` (`country_id`,`Value4`),
  KEY `country_id_3` (`country_id`,`Value5`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_encounters_random
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_encounters_random`;

CREATE TABLE `pkmncf_pokedex_encounters_random` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `room_id` int(10) unsigned NOT NULL,
  `Version` int(10) unsigned NOT NULL,
  `Method` int(10) unsigned NOT NULL,
  `EncounterSlot` tinyint(3) unsigned NOT NULL,
  `form_id` int(10) unsigned NOT NULL,
  `MinLevel` tinyint(3) unsigned NOT NULL,
  `MaxLevel` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_items
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_items`;

CREATE TABLE `pkmncf_pokedex_items` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Value3` int(10) unsigned DEFAULT NULL,
  `Value4` int(10) unsigned DEFAULT NULL,
  `Value5` int(10) unsigned DEFAULT NULL,
  `Value6` int(10) unsigned DEFAULT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `Price` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Value3` (`Value3`),
  KEY `Value4` (`Value4`),
  KEY `Value5` (`Value5`),
  KEY `Value6` (`Value6`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_locations
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_locations`;

CREATE TABLE `pkmncf_pokedex_locations` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `region_id` int(11) unsigned NOT NULL,
  `Name_JA` varchar(30) DEFAULT NULL,
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `Value3` int(11) unsigned DEFAULT NULL,
  `Value_Colo` int(11) unsigned DEFAULT NULL,
  `Value_XD` int(11) unsigned DEFAULT NULL,
  `Value4` int(11) unsigned DEFAULT NULL,
  `Value5` int(11) unsigned DEFAULT NULL,
  `Value6` int(11) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_moves
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_moves`;

CREATE TABLE `pkmncf_pokedex_moves` (
  `Value` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `type_id` int(10) unsigned DEFAULT NULL,
  `DamageClass` tinyint(3) unsigned NOT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `Damage` int(11) DEFAULT NULL,
  `PP` int(11) DEFAULT NULL,
  `Accuracy` int(11) DEFAULT NULL,
  `Priority` int(11) DEFAULT NULL,
  `Target` int(11) DEFAULT NULL,
  PRIMARY KEY (`Value`),
  KEY `Type` (`type_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_pokemon
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_pokemon`;

CREATE TABLE `pkmncf_pokedex_pokemon` (
  `NationalDex` int(10) unsigned NOT NULL,
  `family_id` int(10) unsigned NOT NULL,
  `Name_JA` varchar(36) DEFAULT '',
  `Name_EN` varchar(36) DEFAULT NULL,
  `Name_FR` varchar(36) DEFAULT NULL,
  `Name_IT` varchar(36) DEFAULT NULL,
  `Name_DE` varchar(36) DEFAULT NULL,
  `Name_ES` varchar(36) DEFAULT NULL,
  `Name_KO` varchar(36) DEFAULT NULL,
  `GrowthRate` int(10) unsigned NOT NULL,
  `GenderRatio` tinyint(3) unsigned NOT NULL,
  `EggGroup1` tinyint(3) unsigned NOT NULL,
  `EggGroup2` tinyint(3) unsigned NOT NULL,
  `EggSteps` int(10) unsigned NOT NULL,
  `GenderVariations` bit(1) DEFAULT NULL,
  PRIMARY KEY (`NationalDex`),
  KEY `family_id` (`family_id`),
  KEY `EggGroup1` (`EggGroup1`),
  KEY `EggGroup2` (`EggGroup2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_pokemon_families
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_pokemon_families`;

CREATE TABLE `pkmncf_pokedex_pokemon_families` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `BasicMale` int(10) unsigned NOT NULL,
  `BasicFemale` int(10) unsigned NOT NULL,
  `BabyMale` int(10) unsigned DEFAULT NULL,
  `BabyFemale` int(10) unsigned DEFAULT NULL,
  `Incense` int(10) unsigned DEFAULT NULL,
  `GenderRatio` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_pokemon_form_stats
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_pokemon_form_stats`;

CREATE TABLE `pkmncf_pokedex_pokemon_form_stats` (
  `form_id` int(10) unsigned NOT NULL,
  `MinGeneration` int(11) unsigned NOT NULL,
  `Type1` int(10) unsigned DEFAULT NULL,
  `Type2` int(10) unsigned DEFAULT NULL,
  `BaseHP` int(11) DEFAULT NULL,
  `BaseAttack` int(11) DEFAULT NULL,
  `BaseDefense` int(11) DEFAULT NULL,
  `BaseSpeed` int(11) DEFAULT NULL,
  `BaseSpAttack` int(11) DEFAULT NULL,
  `BaseSpDefense` int(11) DEFAULT NULL,
  `RewardHP` tinyint(3) unsigned DEFAULT NULL,
  `RewardAttack` tinyint(3) unsigned DEFAULT NULL,
  `RewardDefense` tinyint(3) unsigned DEFAULT NULL,
  `RewardSpeed` tinyint(3) unsigned DEFAULT NULL,
  `RewardSpAttack` tinyint(3) unsigned DEFAULT NULL,
  `RewardSpDefense` tinyint(3) unsigned DEFAULT NULL,
  KEY `form_id` (`form_id`,`MinGeneration`),
  KEY `Type1` (`Type1`),
  KEY `Type2` (`Type2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_pokemon_forms
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_pokemon_forms`;

CREATE TABLE `pkmncf_pokedex_pokemon_forms` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `NationalDex` int(11) unsigned NOT NULL,
  `FormValue` tinyint(3) unsigned NOT NULL,
  `Name_JA` varchar(30) DEFAULT NULL,
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `FormSuffix` varchar(30) DEFAULT NULL,
  `Height` int(10) unsigned DEFAULT NULL,
  `Weight` int(10) unsigned DEFAULT NULL,
  `Experience` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `NationalDex` (`NationalDex`),
  KEY `FormValue` (`FormValue`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_regions
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_regions`;

CREATE TABLE `pkmncf_pokedex_regions` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Name_JA` varchar(30) DEFAULT NULL,
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_ribbons
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_ribbons`;

CREATE TABLE `pkmncf_pokedex_ribbons` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Position3` int(10) unsigned DEFAULT NULL,
  `Position4` int(10) unsigned DEFAULT NULL,
  `Position5` int(10) unsigned DEFAULT NULL,
  `Position6` int(10) unsigned DEFAULT NULL,
  `Value3` int(10) unsigned DEFAULT NULL,
  `Value4` int(10) unsigned DEFAULT NULL,
  `Value5` int(10) unsigned DEFAULT NULL,
  `Value6` int(10) unsigned DEFAULT NULL,
  `Name_JA` varchar(30) DEFAULT NULL,
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `Description_JA` varchar(300) DEFAULT NULL,
  `Description_EN` varchar(300) DEFAULT NULL,
  `Description_FR` varchar(300) DEFAULT NULL,
  `Description_IT` varchar(300) DEFAULT NULL,
  `Description_DE` varchar(300) DEFAULT NULL,
  `Description_ES` varchar(300) DEFAULT NULL,
  `Description_KO` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_rooms
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_rooms`;

CREATE TABLE `pkmncf_pokedex_rooms` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `location_id` int(11) unsigned NOT NULL,
  `Comment` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_pokedex_types
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_pokedex_types`;

CREATE TABLE `pkmncf_pokedex_types` (
  `id` int(10) unsigned NOT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `DamageClass` tinyint(3) unsigned DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



# Dump of table pkmncf_web_news
# ------------------------------------------------------------

DROP TABLE IF EXISTS `pkmncf_web_news`;

CREATE TABLE `pkmncf_web_news` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
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
  `Views` int(10) unsigned NOT NULL DEFAULT 0,
  `Saves` int(10) unsigned NOT NULL DEFAULT 0,
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
  `Views` int(10) unsigned NOT NULL DEFAULT 0,
  `Saves` int(10) unsigned NOT NULL DEFAULT 0,
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
