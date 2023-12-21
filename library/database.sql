-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.5.16-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.1.0.6537
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping structure for table gts.BattleVideoCrawlQueue
CREATE TABLE IF NOT EXISTS `BattleVideoCrawlQueue` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `SerialNumber` bigint(11) unsigned DEFAULT NULL,
  `Timestamp` datetime DEFAULT NULL,
  `Complete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.BattleVideoCrawlQueue5
CREATE TABLE IF NOT EXISTS `BattleVideoCrawlQueue5` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `SerialNumber` bigint(11) unsigned DEFAULT NULL,
  `Timestamp` datetime DEFAULT NULL,
  `Complete` bit(1) DEFAULT b'0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.BattleVideoSearchHistory
CREATE TABLE IF NOT EXISTS `BattleVideoSearchHistory` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Metagame` int(11) DEFAULT NULL,
  `Species` int(11) DEFAULT NULL,
  `Country` int(11) DEFAULT NULL,
  `Region` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.BattleVideoSearchHistory5
CREATE TABLE IF NOT EXISTS `BattleVideoSearchHistory5` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `Metagame` int(11) DEFAULT NULL,
  `Species` int(11) DEFAULT NULL,
  `Country` int(11) DEFAULT NULL,
  `Region` int(11) DEFAULT NULL,
  `Special` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleSubway5
CREATE TABLE IF NOT EXISTS `GtsBattleSubway5` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleSubwayLeaders5
CREATE TABLE IF NOT EXISTS `GtsBattleSubwayLeaders5` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleSubwayPokemon5
CREATE TABLE IF NOT EXISTS `GtsBattleSubwayPokemon5` (
  `party_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `Form` smallint(5) unsigned DEFAULT NULL,
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleTower4
CREATE TABLE IF NOT EXISTS `GtsBattleTower4` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleTowerLeaders4
CREATE TABLE IF NOT EXISTS `GtsBattleTowerLeaders4` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsBattleTowerPokemon4
CREATE TABLE IF NOT EXISTS `GtsBattleTowerPokemon4` (
  `party_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned DEFAULT NULL,
  `Form` smallint(5) unsigned DEFAULT NULL,
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsHistory4
CREATE TABLE IF NOT EXISTS `GtsHistory4` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsHistory5
CREATE TABLE IF NOT EXISTS `GtsHistory5` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsPokemon4
CREATE TABLE IF NOT EXISTS `GtsPokemon4` (
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
  `ParseVersion` int(10) unsigned DEFAULT NULL,
  `LockedBy` int(11) DEFAULT NULL,
  `LockedUntil` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsPokemon5
CREATE TABLE IF NOT EXISTS `GtsPokemon5` (
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
  `LockedBy` int(11) DEFAULT NULL,
  `LockedUntil` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `pid` (`pid`),
  KEY `Species` (`Species`),
  KEY `Gender` (`Gender`),
  KEY `Level` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsProfiles4
CREATE TABLE IF NOT EXISTS `GtsProfiles4` (
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
  `IpAddress` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.GtsProfiles5
CREATE TABLE IF NOT EXISTS `GtsProfiles5` (
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
  `IpAddress` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_gamestats_bans_ip
CREATE TABLE IF NOT EXISTS `pkmncf_gamestats_bans_ip` (
  `IpAddress` varchar(64) NOT NULL,
  `Level` int(11) NOT NULL,
  `Reason` text DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`IpAddress`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_gamestats_bans_ipv4_range
CREATE TABLE IF NOT EXISTS `pkmncf_gamestats_bans_ipv4_range` (
  `IpAddressMin` int(10) unsigned NOT NULL,
  `IpAddressMax` int(10) unsigned NOT NULL,
  `Level` int(11) NOT NULL,
  `Reason` text DEFAULT NULL,
  `Expires` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_gamestats_bans_mac
CREATE TABLE IF NOT EXISTS `pkmncf_gamestats_bans_mac` (
  `MacAddress` binary(6) NOT NULL,
  `Level` int(11) NOT NULL,
  `Reason` text DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`MacAddress`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_gamestats_bans_pid
CREATE TABLE IF NOT EXISTS `pkmncf_gamestats_bans_pid` (
  `pid` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Reason` text DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_gamestats_bans_savefile
CREATE TABLE IF NOT EXISTS `pkmncf_gamestats_bans_savefile` (
  `Version` tinyint(3) unsigned NOT NULL,
  `Language` tinyint(3) unsigned NOT NULL,
  `OT` int(10) unsigned NOT NULL,
  `Name` binary(16) NOT NULL,
  `Level` int(11) NOT NULL,
  `Reason` text DEFAULT NULL,
  `Expires` datetime DEFAULT NULL,
  PRIMARY KEY (`Version`,`Language`,`OT`,`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_plaza_profiles
CREATE TABLE IF NOT EXISTS `pkmncf_plaza_profiles` (
  `pid` int(11) NOT NULL,
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_abilities
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_abilities` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_countries
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_countries` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_country_regions
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_country_regions` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_encounters_random
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_encounters_random` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_items
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_items` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Value3` int(10) unsigned DEFAULT NULL,
  `Value4` int(10) unsigned DEFAULT NULL,
  `Value5` int(10) unsigned DEFAULT NULL,
  `Value6` int(10) unsigned DEFAULT NULL,
  `PokeballValue` int(10) unsigned DEFAULT NULL,
  `Name_JA` varchar(30) DEFAULT '',
  `Name_EN` varchar(30) DEFAULT NULL,
  `Name_FR` varchar(30) DEFAULT NULL,
  `Name_IT` varchar(30) DEFAULT NULL,
  `Name_DE` varchar(30) DEFAULT NULL,
  `Name_ES` varchar(30) DEFAULT NULL,
  `Name_KO` varchar(30) DEFAULT NULL,
  `Price` int(10) unsigned DEFAULT NULL,
  `HoldGeneration` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Value3` (`Value3`),
  KEY `Value4` (`Value4`),
  KEY `Value5` (`Value5`),
  KEY `Value6` (`Value6`),
  KEY `ValueBall` (`PokeballValue`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_locations
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_locations` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_moves
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_moves` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_pokemon
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_pokemon` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_pokemon_families
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_pokemon_families` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `BasicMale` int(10) unsigned NOT NULL,
  `BasicFemale` int(10) unsigned NOT NULL,
  `BabyMale` int(10) unsigned DEFAULT NULL,
  `BabyFemale` int(10) unsigned DEFAULT NULL,
  `Incense` int(10) unsigned DEFAULT NULL,
  `GenderRatio` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_pokemon_forms
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_pokemon_forms` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_pokemon_form_abilities
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_pokemon_form_abilities` (
  `form_id` int(10) unsigned NOT NULL,
  `MinGeneration` int(10) unsigned NOT NULL,
  `Ability1` int(10) unsigned DEFAULT NULL,
  `Ability2` int(10) unsigned DEFAULT NULL,
  `HiddenAbility1` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`form_id`,`MinGeneration`),
  KEY `Ability1` (`Ability1`),
  KEY `Ability2` (`Ability2`),
  KEY `HiddenAbility1` (`HiddenAbility1`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_pokemon_form_stats
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_pokemon_form_stats` (
  `form_id` int(10) unsigned NOT NULL,
  `MinGeneration` int(10) unsigned NOT NULL,
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
  PRIMARY KEY (`form_id`,`MinGeneration`),
  KEY `form_id` (`form_id`,`MinGeneration`),
  KEY `Type1` (`Type1`),
  KEY `Type2` (`Type2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_regions
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_regions` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_ribbons
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_ribbons` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_rooms
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_rooms` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `location_id` int(11) unsigned NOT NULL,
  `Comment` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_pokedex_types
CREATE TABLE IF NOT EXISTS `pkmncf_pokedex_types` (
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

-- Data exporting was unselected.

-- Dumping structure for procedure gts.pkmncf_terminal_proc_create_leaderboards_for_record
DELIMITER //
CREATE PROCEDURE `pkmncf_terminal_proc_create_leaderboards_for_record`(
	IN `_report_id` INT,
	IN `_record_type` INT
)
    SQL SECURITY INVOKER
BEGIN

	SELECT @start_date := StartDate
		FROM pkmncf_terminal_trainer_rankings_reports WHERE report_id = _report_id;

	INSERT INTO pkmncf_terminal_trainer_rankings_leaderboards_class
		SELECT _report_id AS report_id, TrainerClass, _record_type AS RecordType, SUM(Score) AS Score 
		FROM pkmncf_terminal_trainer_rankings_records
		INNER JOIN pkmncf_terminal_trainer_rankings_teams 
			ON pkmncf_terminal_trainer_rankings_records.pid = pkmncf_terminal_trainer_rankings_teams.pid
		WHERE pkmncf_terminal_trainer_rankings_records.LastUpdated >= @start_date
			AND RecordType = _record_type
		GROUP BY TrainerClass;

	INSERT INTO pkmncf_terminal_trainer_rankings_leaderboards_month
		SELECT _report_id AS report_id, BirthMonth, _record_type AS RecordType, SUM(Score) AS Score 
		FROM pkmncf_terminal_trainer_rankings_records
		INNER JOIN pkmncf_terminal_trainer_rankings_teams 
			ON pkmncf_terminal_trainer_rankings_records.pid = pkmncf_terminal_trainer_rankings_teams.pid
		WHERE pkmncf_terminal_trainer_rankings_records.LastUpdated >= @start_date
			AND RecordType = _record_type
		GROUP BY BirthMonth;
		
	INSERT INTO pkmncf_terminal_trainer_rankings_leaderboards_pokemon
		SELECT _report_id AS report_id, FavouritePokemon, _record_type AS RecordType, SUM(Score) AS Score 
		FROM pkmncf_terminal_trainer_rankings_records
		INNER JOIN pkmncf_terminal_trainer_rankings_teams 
			ON pkmncf_terminal_trainer_rankings_records.pid = pkmncf_terminal_trainer_rankings_teams.pid
		WHERE pkmncf_terminal_trainer_rankings_records.LastUpdated >= @start_date
			AND RecordType = _record_type
		GROUP BY FavouritePokemon;

END//
DELIMITER ;

-- Dumping structure for procedure gts.pkmncf_terminal_proc_create_leaderboards_for_report
DELIMITER //
CREATE PROCEDURE `pkmncf_terminal_proc_create_leaderboards_for_report`(
	IN `_report_id` INT
)
    SQL SECURITY INVOKER
BEGIN
	SELECT @record_type_1 := RecordType1, @record_type_2 := RecordType2, @record_type_3 := RecordType3
	FROM pkmncf_terminal_trainer_rankings_reports WHERE report_id = _report_id;
	
	DELETE FROM pkmncf_terminal_trainer_rankings_leaderboards_class WHERE report_id = _report_id;
	DELETE FROM pkmncf_terminal_trainer_rankings_leaderboards_month WHERE report_id = _report_id;
	DELETE FROM pkmncf_terminal_trainer_rankings_leaderboards_pokemon WHERE report_id = _report_id;
	
	CALL pkmncf_terminal_proc_create_leaderboards_for_record(_report_id, @record_type_1);
	CALL pkmncf_terminal_proc_create_leaderboards_for_record(_report_id, @record_type_2);
	CALL pkmncf_terminal_proc_create_leaderboards_for_record(_report_id, @record_type_3);
END//
DELIMITER ;

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_leaderboards_class
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_leaderboards_class` (
  `report_id` int(11) NOT NULL,
  `TrainerClass` int(11) NOT NULL,
  `RecordType` int(11) NOT NULL,
  `Score` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`report_id`,`TrainerClass`,`RecordType`) USING BTREE,
  KEY `leaderboard_id` (`report_id`) USING BTREE,
  CONSTRAINT `FK_pkmncf_terminal_trainer_rankings_byclass4_reports` FOREIGN KEY (`report_id`) REFERENCES `pkmncf_terminal_trainer_rankings_reports` (`report_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_leaderboards_month
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_leaderboards_month` (
  `report_id` int(11) NOT NULL,
  `Month` int(11) NOT NULL,
  `RecordType` int(11) NOT NULL,
  `Score` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`report_id`,`Month`,`RecordType`) USING BTREE,
  KEY `leaderboard_id` (`report_id`) USING BTREE,
  CONSTRAINT `FK_pkmncf_terminal_trainer_rankings_bymonth4_reports` FOREIGN KEY (`report_id`) REFERENCES `pkmncf_terminal_trainer_rankings_reports` (`report_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_leaderboards_pokemon
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_leaderboards_pokemon` (
  `report_id` int(11) NOT NULL,
  `pokemon_id` int(11) NOT NULL,
  `RecordType` int(11) NOT NULL,
  `Score` bigint(20) NOT NULL,
  PRIMARY KEY (`report_id`,`pokemon_id`,`RecordType`) USING BTREE,
  KEY `leaderboard_id` (`report_id`) USING BTREE,
  CONSTRAINT `FK_pkmncf_terminal_trainer_rankings_bypokemon4_reports` FOREIGN KEY (`report_id`) REFERENCES `pkmncf_terminal_trainer_rankings_reports` (`report_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_records
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_records` (
  `pid` int(11) NOT NULL,
  `RecordType` int(11) NOT NULL,
  `Score` bigint(20) NOT NULL DEFAULT 0,
  `LastUpdated` datetime NOT NULL,
  PRIMARY KEY (`pid`,`RecordType`),
  KEY `LastUpdated` (`LastUpdated`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_reports
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_reports` (
  `report_id` int(11) NOT NULL AUTO_INCREMENT,
  `StartDate` datetime NOT NULL,
  `EndDate` datetime NOT NULL,
  `RecordType1` int(11) NOT NULL,
  `RecordType2` int(11) NOT NULL,
  `RecordType3` int(11) NOT NULL,
  PRIMARY KEY (`report_id`) USING BTREE,
  KEY `StartDate` (`StartDate`),
  KEY `EndDate` (`EndDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_terminal_trainer_rankings_teams
CREATE TABLE IF NOT EXISTS `pkmncf_terminal_trainer_rankings_teams` (
  `pid` int(11) NOT NULL,
  `TrainerClass` int(11) NOT NULL,
  `BirthMonth` int(11) NOT NULL,
  `FavouritePokemon` int(11) NOT NULL,
  `Unknown1` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Unknown2` smallint(5) unsigned NOT NULL DEFAULT 0,
  `Unknown3` smallint(5) unsigned NOT NULL DEFAULT 0,
  `LastUpdated` datetime NOT NULL,
  PRIMARY KEY (`pid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.pkmncf_web_news
CREATE TABLE IF NOT EXISTS `pkmncf_web_news` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalBattleVideoPokemon4
CREATE TABLE IF NOT EXISTS `TerminalBattleVideoPokemon4` (
  `video_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`video_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalbattlevideopokemon4_ibfk_1` FOREIGN KEY (`video_id`) REFERENCES `TerminalBattleVideos4` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalBattleVideoPokemon5
CREATE TABLE IF NOT EXISTS `TerminalBattleVideoPokemon5` (
  `video_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(5) unsigned NOT NULL,
  PRIMARY KEY (`video_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalbattlevideopokemon5_ibfk_1` FOREIGN KEY (`video_id`) REFERENCES `TerminalBattleVideos5` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalBattleVideos4
CREATE TABLE IF NOT EXISTS `TerminalBattleVideos4` (
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
  `Hype` double DEFAULT 0,
  `HypeTimestamp` datetime DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`),
  KEY `SerialNumber` (`SerialNumber`),
  KEY `TimeAdded` (`TimeAdded`),
  KEY `Metagame` (`Metagame`),
  KEY `pid` (`pid`),
  KEY `Country` (`Country`,`Region`),
  KEY `md5` (`md5`),
  KEY `Hype` (`Hype`),
  KEY `HypeTimestamp` (`HypeTimestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalBattleVideos5
CREATE TABLE IF NOT EXISTS `TerminalBattleVideos5` (
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
  `Hype` double DEFAULT 0,
  `HypeTimestamp` datetime DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`id`),
  KEY `SerialNumber` (`SerialNumber`),
  KEY `TimeAdded` (`TimeAdded`),
  KEY `Metagame` (`Metagame`),
  KEY `pid` (`pid`),
  KEY `Country` (`Country`,`Region`),
  KEY `md5` (`md5`),
  KEY `Hype` (`Hype`),
  KEY `HypeTimestamp` (`HypeTimestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalBoxes4
CREATE TABLE IF NOT EXISTS `TerminalBoxes4` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalDressup4
CREATE TABLE IF NOT EXISTS `TerminalDressup4` (
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

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalMusicalPokemon5
CREATE TABLE IF NOT EXISTS `TerminalMusicalPokemon5` (
  `musical_id` bigint(20) unsigned NOT NULL,
  `Slot` tinyint(3) unsigned NOT NULL,
  `Species` smallint(6) unsigned NOT NULL,
  PRIMARY KEY (`musical_id`,`Slot`),
  KEY `Species` (`Species`),
  CONSTRAINT `terminalmusicalpokemon5_ibfk_1` FOREIGN KEY (`musical_id`) REFERENCES `TerminalMusicals5` (`SerialNumber`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.

-- Dumping structure for table gts.TerminalMusicals5
CREATE TABLE IF NOT EXISTS `TerminalMusicals5` (
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

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
