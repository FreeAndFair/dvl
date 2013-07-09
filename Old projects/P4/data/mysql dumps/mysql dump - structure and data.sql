-- phpMyAdmin SQL Dump
-- version 3.4.5
-- http://www.phpmyadmin.net
--
-- Vært: localhost
-- Genereringstid: 13. 12 2011 kl. 14:06:29
-- Serverversion: 5.5.16
-- PHP-version: 5.3.8

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `dirtylion`
--
CREATE DATABASE `dirtylion` DEFAULT CHARACTER SET utf8 COLLATE utf8_danish_ci;
USE `dirtylion`;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `log`
--

CREATE TABLE IF NOT EXISTS `log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `client` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `person_id` int(11) NOT NULL,
  `action` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `polling_table` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `timestamp` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_danish_ci AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `logtestsuite`
--

CREATE TABLE IF NOT EXISTS `logtestsuite` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `client` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `person_id` int(11) NOT NULL,
  `action` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `polling_table` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `timestamp` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_danish_ci AUTO_INCREMENT=3 ;

--
-- Data dump for tabellen `logtestsuite`
--

INSERT INTO `logtestsuite` (`id`, `client`, `person_id`, `action`, `polling_table`, `timestamp`) VALUES
(2, 'Client 8', 2, 'register', '8', 1323338921);

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `person`
--

CREATE TABLE IF NOT EXISTS `person` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `firstname` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `lastname` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `cpr` varchar(10) COLLATE utf8_danish_ci NOT NULL,
  `voter_id` int(11) NOT NULL,
  `polling_venue` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `polling_table` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_danish_ci AUTO_INCREMENT=101 ;

--
-- Data dump for tabellen `person`
--

INSERT INTO `person` (`id`, `firstname`, `lastname`, `cpr`, `voter_id`, `polling_venue`, `polling_table`) VALUES
(1, 'Adam', 'Johansen', '0104371945', 1323627889, 'Lundehusskolen', '12'),
(2, 'Adam', 'Sørensen', '0707371945', 1323627844, 'Lundehusskolen', '17'),
(3, 'Albert', 'Rasmussen', '0811341883', 1323627872, 'Lundehusskolen', '4'),
(4, 'Alberte', 'Christiansen', '2901251944', 1323627853, 'Lundehusskolen', '4'),
(5, 'Alberte', 'Jørgensen', '2302421876', 1323627823, 'Lundehusskolen', '2'),
(6, 'Alfred', 'Poulsen', '0807501985', 1323627851, 'Lundehusskolen', '17'),
(7, 'Alma', 'Poulsen', '1209301892', 1323627824, 'Lundehusskolen', '19'),
(8, 'Amalie', 'Poulsen', '0502271882', 1323627905, 'Lundehusskolen', '10'),
(9, 'Amalie', 'Sørensen', '2502362056', 1323627828, 'Lundehusskolen', '14'),
(10, 'Andreas', 'Olsen', '0304591927', 1323627865, 'Lundehusskolen', '15'),
(11, 'Anna', 'Pedersen', '1109632034', 1323627833, 'Lundehusskolen', '3'),
(12, 'Asta', 'Knudsen', '1808401952', 1323627859, 'Lundehusskolen', '19'),
(13, 'Asta', 'Poulsen', '1605272026', 1323627903, 'Lundehusskolen', '5'),
(14, 'Astrid', 'Jørgensen', '2703451924', 1323627890, 'Lundehusskolen', '4'),
(15, 'Astrid', 'Nielsen', '0609312018', 1323627812, 'Lundehusskolen', '12'),
(16, 'Benjamin', 'Larsen', '0905341965', 1323627875, 'Lundehusskolen', '13'),
(17, 'Benjamin', 'Larsen', '2404351897', 1323627882, 'Lundehusskolen', '1'),
(18, 'Benjamin', 'Rasmussen', '1509131909', 1323627867, 'Lundehusskolen', '18'),
(19, 'Bertram', 'Johansen', '0401451999', 1323627879, 'Lundehusskolen', '14'),
(20, 'Bertram', 'Poulsen', '1004751991', 1323627897, 'Lundehusskolen', '10'),
(21, 'Carl', 'Johansen', '1505332013', 1323627873, 'Lundehusskolen', '12'),
(22, 'Carl', 'Larsen', '0706292043', 1323627814, 'Lundehusskolen', '6'),
(23, 'Caroline', 'Rasmussen', '2802531996', 1323627878, 'Lundehusskolen', '2'),
(24, 'Caroline', 'Sørensen', '1809721926', 1323627836, 'Lundehusskolen', '14'),
(25, 'Cecilie', 'Larsen', '2301841916', 1323627895, 'Lundehusskolen', '19'),
(26, 'Christian', 'Sørensen', '2801351895', 1323627822, 'Lundehusskolen', '6'),
(27, 'Clara', 'Olsen', '1607892020', 1323627866, 'Lundehusskolen', '15'),
(28, 'Emily', 'Poulsen', '2902441984', 1323627883, 'Lundehusskolen', '16'),
(29, 'Emma', 'Madsen', '0309832044', 1323627849, 'Lundehusskolen', '13'),
(30, 'Felix', 'Andersen', '1101451877', 1323627863, 'Lundehusskolen', '4'),
(31, 'Felix', 'Christensen', '1701671939', 1323627825, 'Lundehusskolen', '6'),
(32, 'Felix', 'Hansen', '0302751907', 1323627843, 'Lundehusskolen', '8'),
(33, 'Filippa', 'Jørgensen', '2608482040', 1323627827, 'Lundehusskolen', '4'),
(34, 'Filippa', 'Madsen', '1701581918', 1323627806, 'Lundehusskolen', '11'),
(35, 'Filippa', 'Thomsen', '1601221882', 1323627809, 'Lundehusskolen', '1'),
(36, 'Frederikke', 'Olsen', '2705431974', 1323627904, 'Lundehusskolen', '16'),
(37, 'Frida', 'Andersen', '2802851972', 1323627886, 'Lundehusskolen', '12'),
(38, 'Frida', 'Poulsen', '2205911934', 1323627837, 'Lundehusskolen', '19'),
(39, 'Johan', 'Madsen', '2710501973', 1323627816, 'Lundehusskolen', '2'),
(40, 'Johan', 'Poulsen', '2707872001', 1323627826, 'Lundehusskolen', '1'),
(41, 'Jonathan', 'Andersen', '1608872045', 1323627896, 'Lundehusskolen', '6'),
(42, 'Jonathan', 'Johansen', '0404902039', 1323627821, 'Lundehusskolen', '3'),
(43, 'Jonathan', 'Knudsen', '0808281873', 1323627850, 'Lundehusskolen', '3'),
(44, 'Jonathan', 'Rasmussen', '1111571925', 1323627820, 'Lundehusskolen', '5'),
(45, 'Josefine', 'Larsen', '1509382012', 1323627831, 'Lundehusskolen', '15'),
(46, 'Josefine', 'Pedersen', '0504532030', 1323627900, 'Lundehusskolen', '10'),
(47, 'Julie', 'Knudsen', '1704711868', 1323627856, 'Lundehusskolen', '5'),
(48, 'Karla', 'Andersen', '1111841924', 1323627868, 'Lundehusskolen', '2'),
(49, 'Lea', 'Johansen', '2508221978', 1323627861, 'Lundehusskolen', '4'),
(50, 'Magnus', 'Johansen', '0703501959', 1323627811, 'Lundehusskolen', '15'),
(51, 'Malou', 'Andersen', '0711121998', 1323627887, 'Lundehusskolen', '1'),
(52, 'Malou', 'Hansen', '0109192010', 1323627830, 'Lundehusskolen', '18'),
(53, 'Malthe', 'Christensen', '1805141875', 1323627898, 'Lundehusskolen', '3'),
(54, 'Malthe', 'Rasmussen', '1810662007', 1323627807, 'Lundehusskolen', '6'),
(55, 'Marcus', 'Andersen', '1701492033', 1323627845, 'Lundehusskolen', '5'),
(56, 'Maria', 'Andersen', '0805471944', 1323627819, 'Lundehusskolen', '7'),
(57, 'Maria', 'Christiansen', '1205452054', 1323627858, 'Lundehusskolen', '10'),
(58, 'Maria', 'Sørensen', '1804881966', 1323627899, 'Lundehusskolen', '15'),
(59, 'Mathilde', 'Nielsen', '1303361918', 1323627813, 'Lundehusskolen', '13'),
(60, 'Mikkel', 'Thomsen', '1702741941', 1323627840, 'Lundehusskolen', '4'),
(61, 'Mille', 'Christensen', '2309301886', 1323627881, 'Lundehusskolen', '4'),
(62, 'Mille', 'Møller', '0603411968', 1323627893, 'Lundehusskolen', '2'),
(63, 'Naja', 'Kristensen', '2505752020', 1323627848, 'Lundehusskolen', '1'),
(64, 'Naja', 'Petersen', '2105292026', 1323627841, 'Lundehusskolen', '4'),
(65, 'Nicoline', 'Johansen', '1003152048', 1323627871, 'Lundehusskolen', '12'),
(66, 'Nicoline', 'Olsen', '0907461894', 1323627846, 'Lundehusskolen', '11'),
(67, 'Oliver', 'Poulsen', '2505172011', 1323627874, 'Lundehusskolen', '8'),
(68, 'Olivia', 'Madsen', '2109571876', 1323627817, 'Lundehusskolen', '15'),
(69, 'Philip', 'Pedersen', '1701732049', 1323627835, 'Lundehusskolen', '2'),
(70, 'Philip', 'Sørensen', '0409871977', 1323627885, 'Lundehusskolen', '2'),
(71, 'Rasmus', 'Johansen', '1904801869', 1323627869, 'Lundehusskolen', '19'),
(72, 'Rasmus', 'Poulsen', '2904171983', 1323627815, 'Lundehusskolen', '13'),
(73, 'Rosa', 'Hansen', '0503731980', 1323627860, 'Lundehusskolen', '10'),
(74, 'Rosa', 'Knudsen', '1002501884', 1323627876, 'Lundehusskolen', '18'),
(75, 'Rosa', 'Sørensen', '1505352014', 1323627847, 'Lundehusskolen', '3'),
(76, 'Sander', 'Møller', '1210371869', 1323627829, 'Lundehusskolen', '16'),
(77, 'Sebastian', 'Christensen', '0801341947', 1323627888, 'Lundehusskolen', '4'),
(78, 'Sebastian', 'Sørensen', '1909411879', 1323627870, 'Lundehusskolen', '6'),
(79, 'Signe', 'Andersen', '2606171964', 1323627855, 'Lundehusskolen', '8'),
(80, 'Signe', 'Thomsen', '1904542028', 1323627857, 'Lundehusskolen', '5'),
(81, 'Silas', 'Knudsen', '0308552035', 1323627834, 'Lundehusskolen', '13'),
(82, 'Silje', 'Andersen', '2209651944', 1323627884, 'Lundehusskolen', '7'),
(83, 'Silke', 'Christiansen', '1402142018', 1323627891, 'Lundehusskolen', '18'),
(84, 'Simon', 'Rasmussen', '2510191945', 1323627852, 'Lundehusskolen', '14'),
(85, 'Simon', 'Rasmussen', '1504871871', 1323627864, 'Lundehusskolen', '18'),
(86, 'Sofia', 'Petersen', '1410131910', 1323627894, 'Lundehusskolen', '18'),
(87, 'Sofia', 'Rasmussen', '0807321886', 1323627854, 'Lundehusskolen', '10'),
(88, 'Storm', 'Sørensen', '1705202017', 1323627838, 'Lundehusskolen', '6'),
(89, 'Storm', 'Sørensen', '0805701943', 1323627862, 'Lundehusskolen', '18'),
(90, 'Tilde', 'Poulsen', '1208722034', 1323627810, 'Lundehusskolen', '13'),
(91, 'Tobias', 'Christensen', '1009121871', 1323627808, 'Lundehusskolen', '18'),
(92, 'Tristan', 'Christensen', '1001501903', 1323627839, 'Lundehusskolen', '5'),
(93, 'Tristan', 'Olsen', '2204771957', 1323627842, 'Lundehusskolen', '13'),
(94, 'Tristan', 'Thomsen', '0210191989', 1323627877, 'Lundehusskolen', '17'),
(95, 'Valdemar', 'Kristensen', '0209582023', 1323627901, 'Lundehusskolen', '3'),
(96, 'Valdemar', 'Nielsen', '1211701873', 1323627892, 'Lundehusskolen', '18'),
(97, 'Victor', 'Madsen', '0209242033', 1323627818, 'Lundehusskolen', '3'),
(98, 'Victoria', 'Jørgensen', '1811852020', 1323627832, 'Lundehusskolen', '1'),
(99, 'Victoria', 'Jørgensen', '2908872054', 1323627902, 'Lundehusskolen', '1'),
(100, 'Victoria', 'Kristensen', '1202301928', 1323627880, 'Lundehusskolen', '9');

-- --------------------------------------------------------

--
-- Struktur-dump for tabellen `persontestsuite`
--

CREATE TABLE IF NOT EXISTS `persontestsuite` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `firstname` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `lastname` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `cpr` varchar(10) COLLATE utf8_danish_ci NOT NULL,
  `voter_id` int(11) NOT NULL,
  `polling_venue` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  `polling_table` varchar(100) COLLATE utf8_danish_ci NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_danish_ci AUTO_INCREMENT=4 ;

--
-- Data dump for tabellen `persontestsuite`
--

INSERT INTO `persontestsuite` (`id`, `firstname`, `lastname`, `cpr`, `voter_id`, `polling_venue`, `polling_table`) VALUES
(1, 'Henrik', 'Haugbølle', '0123456789', 3306, 'Venue of Awesome', 'Table of Win'),
(2, 'Christian', 'Olsson', '0123456789', 8889, 'Venue of Shame', 'Table of Fish'),
(3, 'Kåre', 'Sylow Pedersen', '0123456789', 8080, 'Venue of Anger', 'Table of Calmness');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
