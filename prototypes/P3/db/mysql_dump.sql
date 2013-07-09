-- phpMyAdmin SQL Dump
-- version 3.4.5
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Dec 14, 2011 at 02:35 AM
-- Server version: 5.5.16
-- PHP Version: 5.3.8

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `px3`
--

-- --------------------------------------------------------

--
-- Table structure for table `action`
--

CREATE TABLE IF NOT EXISTS `action` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `label` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=18 ;

--
-- Dumping data for table `action`
--

INSERT INTO `action` (`id`, `label`) VALUES
(1, 'AllVotingPlaces'),
(2, 'CreateUser'),
(3, 'LoadCitizen'),
(4, 'LoadUser'),
(5, 'LoadVoterCard'),
(6, 'ScanVoterCard'),
(7, 'FindCitizen'),
(8, 'FindUser'),
(9, 'FindVotingVenue'),
(10, 'SaveUser'),
(11, 'SetHasVoted'),
(12, 'SetHasVotedManually'),
(13, 'ChangeOwnPassword'),
(14, 'ChangeOthersPassword'),
(15, 'UpdateCitizens'),
(16, 'UpdateVoterCards'),
(17, 'PrintVoterCards');

-- --------------------------------------------------------

--
-- Table structure for table `permission`
--

CREATE TABLE IF NOT EXISTS `permission` (
  `action_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  PRIMARY KEY (`action_id`,`user_id`),
  KEY `Person_Action_Action1` (`action_id`),
  KEY `fk_permission_user1` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `permission`
--

INSERT INTO `permission` (`action_id`, `user_id`) VALUES
(1, 1),
(2, 1),
(3, 1),
(3, 3),
(4, 1),
(5, 1),
(6, 1),
(6, 3),
(7, 1),
(8, 1),
(9, 1),
(10, 1),
(11, 1),
(11, 3),
(12, 1),
(13, 1),
(14, 1),
(15, 1),
(16, 1),
(17, 1);

-- --------------------------------------------------------

--
-- Table structure for table `person`
--

CREATE TABLE IF NOT EXISTS `person` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) DEFAULT NULL,
  `address` text,
  `cpr` varchar(10) DEFAULT NULL,
  `eligible_to_vote` tinyint(1) NOT NULL DEFAULT '0',
  `has_voted` tinyint(1) NOT NULL DEFAULT '0',
  `place_of_birth` varchar(255) DEFAULT NULL,
  `passport_number` varchar(10) DEFAULT NULL,
  `voting_venue_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `person_voting_venue1` (`voting_venue_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=5161 ;

--
-- Dumping data for table `person`
--

INSERT INTO `person` (`id`, `name`, `address`, `cpr`, `eligible_to_vote`, `has_voted`, `place_of_birth`, `passport_number`, `voting_venue_id`) VALUES
(1, 'Jens Dahl Møllerhøj', 'Nørre Alle 75, 471', '2405901253', 1, 0, 'Gentofte Hospital', '5684895471', 7),
(2, 'Christopher Nørgård Dall', NULL, '2303871234', 0, 0, 'Samsø Sygehus', '663780088', 2),
(3, 'Mathilde Roed Birk', NULL, '1212534321', 1, 0, NULL, NULL, 1),
(4, 'Ronni Holm', NULL, '1705787897', 1, 1, 'Randers Sygehus', '376095272', 1);

-- --------------------------------------------------------

--
-- Table structure for table `quiz`
--

CREATE TABLE IF NOT EXISTS `quiz` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `question` text NOT NULL,
  `answer` text NOT NULL,
  `person_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `quiz_person1` (`person_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=32642 ;

-- --------------------------------------------------------

--
-- Table structure for table `raw_person_data`
--

CREATE TABLE IF NOT EXISTS `raw_person_data` (
  `cpr` varchar(45) NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `mother_cpr` varchar(45) DEFAULT NULL,
  `father_cpr` varchar(45) DEFAULT NULL,
  `address` text,
  `address_previous` text,
  `gender` varchar(45) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `deathdate` date DEFAULT NULL,
  `dead` tinyint(1) DEFAULT NULL,
  `birthplace` text,
  `age` int(11) DEFAULT NULL,
  `driver_id` varchar(45) DEFAULT NULL,
  `military_served` tinyint(1) DEFAULT NULL,
  `workplace` text,
  `education` text,
  `telephone` varchar(45) DEFAULT NULL,
  `passport_number` varchar(45) DEFAULT NULL,
  `city` varchar(45) DEFAULT NULL,
  `zipcode` int(11) DEFAULT NULL,
  `nationality` varchar(45) DEFAULT NULL,
  `disempowered` int(11) DEFAULT NULL,
  PRIMARY KEY (`cpr`),
  KEY `raw_data_raw_data` (`mother_cpr`),
  KEY `raw_data_raw_data1` (`father_cpr`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `raw_person_data`
--

INSERT INTO `raw_person_data` (`cpr`, `name`, `mother_cpr`, `father_cpr`, `address`, `address_previous`, `gender`, `birthday`, `deathdate`, `dead`, `birthplace`, `age`, `driver_id`, `military_served`, `workplace`, `education`, `telephone`, `passport_number`, `city`, `zipcode`, `nationality`, `disempowered`) VALUES
('0201221521', 'Bruno Kaae Hussain', '2901948180', '2609001781', 'Slotsvej 2', 'Korsgade 45', 'male', '1922-01-02', NULL, 0, 'Sygehus Syd, Næstved', 89, NULL, 0, 'Femern A/S', NULL, '18188706', '602828228', 'Blåvand', 6857, 'DNK', 0),
('0205949601', 'Paw Rohde Steen', '0807740040', '1006703241', 'Valdemar Holmers G 33', 'Gyvelvej 52', 'male', '1994-05-02', NULL, 0, 'Thisted Sygehus', 17, NULL, 0, NULL, NULL, '89564574', NULL, 'Vestbjerg', 9380, 'DNK', 0),
('0807740040', 'Andrea Brink Friis', '1004460530', '0610420011', NULL, NULL, 'female', '1974-07-08', NULL, 0, 'Gentofte Hospital', 37, NULL, 0, NULL, 'Mekatronik', NULL, '503149274', 'Vejers Strand', 6853, 'DNK', 0),
('1006703241', 'George Skytte Villadsen', '2009459290', '2304399471', 'Capellakaj 40', 'Gribskovvej 34', 'male', '1970-06-10', NULL, 0, 'Viborg-Kjellerup Sygehus', 41, NULL, 1, 'Midt- og Vestsjællands Politi', NULL, NULL, '096206057', 'Skævinge', 3320, 'DNK', 0),
('1106919221', 'Iver Nørregaard Brink', '1511640300', '1006703241', 'Orientkaj 39', 'Wiedeveltsvej 10', 'male', '1991-06-11', NULL, 0, 'Gentofte Hospital', 20, NULL, 0, NULL, NULL, '10237792', '125258391', 'Charlottenlund', 2920, 'DNK', 0),
('1212391470', 'Zehnia Kruse', '1712071610', '1106191521', 'Humlevænget 1', 'Skovshovedvej 27', 'female', NULL, NULL, 0, 'Viborg-Kjellerup Sygehus', 72, NULL, 0, 'Beierholm', NULL, '48855627', '475738453', 'Knebel', 8420, 'DNK', 0),
('1511640300', 'Dorthe Wagner', '0509469330', '2110411501', 'Kirkevej 53', 'Holmegårdsvej 58', 'female', '1964-11-15', NULL, 0, 'Regionshospitalet, Herning', 47, NULL, 0, 'Templine', 'Teknisk biomedicin', NULL, '442202007', 'Frøstrup', 7741, 'DNK', 0),
('1712071610', 'Benedicte Bisgaard Callesen', NULL, NULL, NULL, NULL, 'female', '1907-12-17', '1980-10-20', 1, 'Sygehus Syd, Næstved', 104, NULL, 0, NULL, NULL, NULL, '050337269', 'Thisted', 7700, 'DNK', 0),
('1804212540', 'Regitze Klitgaard Albrechtsen', NULL, NULL, 'Rothesgade 12', 'Danstrupvej 45', 'female', NULL, '1965-05-28', 1, 'Herlev Hospital', 90, NULL, 0, 'Tarco Entreprise a/s', NULL, '12047905', '462733701', 'Fårvang', 8882, 'DNK', 0),
('1804301611', 'Erik Winther Eriksen', NULL, '2611072591', 'Klædemålet 41', 'Gamlehave Allé 12', 'male', '1930-04-18', NULL, 0, 'Herlev Hospital', 81, '01050316', 0, NULL, 'Overfladebehandler', '45827007', '121054559', 'Høng', 4270, 'DNK', 0),
('2009459290', 'Patricia Johansen', '1804212540', '0201221521', 'Jagtvej 20', NULL, 'female', '1945-09-20', NULL, 0, 'Sygehus Fyn, Svendborg', 66, NULL, 0, 'ADEPT ApS', NULL, NULL, '942752783', 'Dronningmølle', 3120, 'DNK', 0),
('2012160361', 'Benjamin Olsen', NULL, NULL, 'Pile Alle 48', NULL, 'male', NULL, NULL, 1, 'Hjørring Sygehus', 95, NULL, 0, 'Bri•sten Gruppen', 'Medicin og teknologi', NULL, '185557728', 'Bække', 6622, 'DNK', 0),
('2303118391', 'Johannes Therkelsen', NULL, NULL, 'Heisesgade 56', 'Poul Henningsens Pl 60', 'male', '2011-03-23', NULL, 0, 'Silkeborg Sygehus', 0, NULL, 0, NULL, NULL, NULL, '061278749', 'Odense V', 5200, 'DNK', 0),
('2304399471', 'Yannick Duus Kristiansen', '3103131680', '2012160361', 'Ingeborgvej 50', 'Skjoldhøj Allé 50', 'male', '1939-04-23', NULL, 0, 'Skejby Sygehus', 72, '84734502', 1, 'FSR', NULL, NULL, '438636941', 'Nykøbing Mors', 7900, 'DNK', 0),
('2801612250', 'Malene Bjerregaard Damm', '2803310290', NULL, 'Svanevænget 26', NULL, 'female', '1961-01-28', NULL, 1, 'Sønderborg Sygehus', 50, '17423434', 0, NULL, NULL, NULL, '595987418', 'Horslunde', 4913, 'DNK', 0),
('2803310290', 'Lærke Johansson', '1712071610', '3110090081', 'A.L. Drewsens Vej 21', 'Ordrup Have 15', 'female', '1931-03-28', NULL, 0, 'Samsø Sygehus', 80, NULL, 0, 'Gadejuristen', 'VVS-uddannelsen', NULL, '094510073', 'Aabenraa', 6200, 'DNK', 0),
('2810119730', 'Stina Krarup Christophersen', '1807780970', '1106919221', 'Emdrupvej 44', NULL, 'female', '2011-10-28', NULL, 0, 'Kolding Sygehus', 0, NULL, 0, NULL, NULL, '89670094', '950198334', 'Rødkærsbro', 8840, 'DNK', 0),
('2901948180', 'Sille Rohde Olsson', NULL, NULL, 'Ragnagade 47', NULL, 'female', NULL, NULL, 1, 'Hillerød Sygehus', 117, NULL, 0, 'Cleardrive A/S', NULL, '77060687', '511668541', 'Jerslev Jylland', 9740, 'DNK', 0),
('3103131680', 'Jannie Graversen', NULL, NULL, 'Bernstorfflund Allé 55', 'Kuhlausgade 4', 'female', NULL, '1980-02-19', 1, 'Kolding Sygehus', 98, NULL, 0, 'CBS Management Consulting Club', 'Klassisk musiker', NULL, '541978973', 'Gørlev', 4281, 'DNK', 0),
('3110090081', 'Øjvind Brogaard Carlsen', NULL, NULL, 'Almestræde 48', NULL, 'male', NULL, '1981-03-27', 1, 'Regionshospitalet, Horsens', 102, '99709240', 0, 'SAS Group', 'Elektriker', '66047526', '228676134', 'Hovborg', 6682, 'DNK', 0);

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE IF NOT EXISTS `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `user_name` varchar(255) NOT NULL,
  `title` text NOT NULL,
  `person_id` int(11) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `user_salt` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `user_name_UNIQUE` (`user_name`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=4 ;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`id`, `user_name`, `title`, `person_id`, `password_hash`, `user_salt`) VALUES
(1, 'jdmo', 'Sys Tester', 1, '89D2F4EDD669E164DE3463B83F0F41F0', 'lkaFDA62lio+3'),
(2, 'slave', 'Slave', 2, '55DA30607A226C495427D0ABF43D809D', '9Ha62lio+3FDA'),
(3, 'elec', 'Election Representitive', 3, '89D2F4EDD669E164DE3463B83F0F41F0', 'lkaFDA62lio+3');

-- --------------------------------------------------------

--
-- Table structure for table `voter_card`
--

CREATE TABLE IF NOT EXISTS `voter_card` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `person_id` int(11) NOT NULL,
  `valid` tinyint(1) NOT NULL,
  `id_key` varchar(8) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `voter_card_Person1` (`person_id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=3191 ;

-- --------------------------------------------------------

--
-- Table structure for table `voting_venue`
--

CREATE TABLE IF NOT EXISTS `voting_venue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `address` text NOT NULL,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=8 ;

--
-- Dumping data for table `voting_venue`
--

INSERT INTO `voting_venue` (`id`, `address`, `name`) VALUES
(1, 'Dyssegårdsvej 34', 'Dyssegårdskolens aula'),
(2, 'Lærkevej 2', 'Gotenfods Gymnasiums gymnastiksal'),
(7, 'Bakkedals vej 23', 'Aurehøj Gymnasiums fællessal');

-- --------------------------------------------------------

--
-- Table structure for table `workplace`
--

CREATE TABLE IF NOT EXISTS `workplace` (
  `user_id` int(11) NOT NULL,
  `voting_venue_id` int(11) NOT NULL,
  PRIMARY KEY (`user_id`,`voting_venue_id`),
  KEY `user_voting_venue_voting_venue1` (`voting_venue_id`),
  KEY `user_voting_venue_user1` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `workplace`
--

INSERT INTO `workplace` (`user_id`, `voting_venue_id`) VALUES
(1, 1),
(2, 2),
(2, 7);

--
-- Constraints for dumped tables
--

--
-- Constraints for table `permission`
--
ALTER TABLE `permission`
  ADD CONSTRAINT `fk_permission_user1` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `Person_Action_Action1` FOREIGN KEY (`action_id`) REFERENCES `action` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `person`
--
ALTER TABLE `person`
  ADD CONSTRAINT `person_voting_venue1` FOREIGN KEY (`voting_venue_id`) REFERENCES `voting_venue` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `quiz`
--
ALTER TABLE `quiz`
  ADD CONSTRAINT `quiz_person1` FOREIGN KEY (`person_id`) REFERENCES `person` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `voter_card`
--
ALTER TABLE `voter_card`
  ADD CONSTRAINT `voter_card_Person1` FOREIGN KEY (`person_id`) REFERENCES `person` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `workplace`
--
ALTER TABLE `workplace`
  ADD CONSTRAINT `user_voting_venue_user1` FOREIGN KEY (`user_id`) REFERENCES `user` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `user_voting_venue_voting_venue1` FOREIGN KEY (`voting_venue_id`) REFERENCES `voting_venue` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
