-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Feb 17, 2026 at 10:38 AM
-- Wersja serwera: 10.4.32-MariaDB
-- Wersja PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `fitnesdb`
--

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `classes`
--

CREATE TABLE `classes` (
  `id` int(11) NOT NULL,
  `name` varchar(255) NOT NULL,
  `description` text DEFAULT NULL,
  `instructor` varchar(255) NOT NULL,
  `starts_at` datetime NOT NULL DEFAULT current_timestamp(),
  `people_limit` int(11) DEFAULT NULL,
  `repetition` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

--
-- Dumping data for table `classes`
--

INSERT INTO `classes` (`id`, `name`, `description`, `instructor`, `starts_at`, `people_limit`, `repetition`) VALUES
(1, 'Yoga', 'Super zajęcia z najlepszym prowadzącym', 'Anna Maria Biedronkowska', '2026-02-16 08:15:00', 2, 'Monthly'),
(2, 'Crossfit', 'Lepsze zajęcia niż pani Anny Marii', 'Janusz Bosy', '2026-02-17 09:10:00', NULL, 'Every2Weeks'),
(3, 'Sprint na 200m', 'Najlepsi nauczyciele sprintu z testem sprawnościowym codziennie', 'MPK-Łódź Spółka z o.o.', '2026-02-17 15:30:00', NULL, 'Daily'),
(4, 'Zumba', 'Ani pani Anna Maria ani pan Janusz Bosy nie mają tak dobrych zajęć jak JA!', 'Joanna Syszkowa', '2026-02-10 17:00:00', 2137, 'None'),
(5, 'Bieg z przeszkodami', 'Bieg z losowo generowanymi każdego dnia przeszkodami, w zależności od: daty, godziny, pogody i usterek technicznych. Dworzec Łódź Fabryczna oferuje nieprzewidywalny tor. Każdego dnia losujemy ci jeden z czterech peronów i stawiamy cię pomiędzy dwoma wejściami i szansą na to, że jedno z nich będzie zablokowane. Dreszcz emocji i niepewności z wymuszania biegu wzdłuż peronu i braku informacji o utrudnieniach poprzedzających start. ', 'PKP Polskie Linie Kolejowe S.A.', '2026-02-13 11:30:00', 256, 'Weekly');

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `signups`
--

CREATE TABLE `signups` (
  `id` int(11) NOT NULL,
  `uid` int(11) NOT NULL,
  `cid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

--
-- Dumping data for table `signups`
--

INSERT INTO `signups` (`id`, `uid`, `cid`) VALUES
(8, 1, 1),
(2, 1, 2),
(3, 1, 3),
(10, 1, 5),
(4, 2, 1),
(6, 2, 3),
(7, 3, 3),
(9, 4, 3),
(12, 6, 3),
(11, 6, 5);

-- --------------------------------------------------------

--
-- Struktura tabeli dla tabeli `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `email` varchar(255) NOT NULL,
  `passhash` varchar(255) NOT NULL,
  `username` varchar(50) NOT NULL,
  `refresh_token` text DEFAULT NULL,
  `refresh_token_expires` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_polish_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `email`, `passhash`, `username`, `refresh_token`, `refresh_token_expires`) VALUES
(1, 'wd@g.com', '$2a$11$ed9b6gMCelZKArfluX1ud.qIqTXNefg/UgFNqQVueFi8kQ6xlqCCq', 'MarianKrzyzbox', NULL, NULL),
(2, 'kasztan@c.pl', '$2a$11$0OPBWK4C/BoDW8rHTB7K2ekMx/K38k7Spa.p6iQ42uq0R7PhK8NaO', 'qwe', NULL, NULL),
(3, 'qwerty@c.pl', '$2a$11$1BBClAf9dcWwiVBIYehSIeI5GM5mghm31c.qVRj41Id0oBV7nhqDe', 'Username', NULL, NULL),
(4, 'noelle@deltarune.com', '$2a$11$LdgO4REgr7K7JQaOTeUPwesReoUCoiDknZtKruOVfKDcY/xHm1LYG', 'NOELLE', NULL, NULL),
(5, 'qwe@fw.cw', '$2a$11$/sHht.naE0S.AVUXK1QjyuMnzoxCGrI09r7qfLHbwdxcf0GbvAZ4O', 'qwe', NULL, NULL),
(6, 'qwe@fw.cwf', '$2a$11$sffemWcFUSduv46vmqP8MuEwxAcCA.xtN5art1aXAUrXenH0PK4xW', 'qwe', NULL, NULL);

--
-- Indeksy dla zrzutów tabel
--

--
-- Indeksy dla tabeli `classes`
--
ALTER TABLE `classes`
  ADD PRIMARY KEY (`id`);

--
-- Indeksy dla tabeli `signups`
--
ALTER TABLE `signups`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `unique_signups` (`uid`,`cid`),
  ADD KEY `signups_cid_fk` (`cid`);

--
-- Indeksy dla tabeli `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `unique_email` (`email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `classes`
--
ALTER TABLE `classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `signups`
--
ALTER TABLE `signups`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `signups`
--
ALTER TABLE `signups`
  ADD CONSTRAINT `signups_cid_fk` FOREIGN KEY (`cid`) REFERENCES `classes` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `signups_uid_fk` FOREIGN KEY (`uid`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
