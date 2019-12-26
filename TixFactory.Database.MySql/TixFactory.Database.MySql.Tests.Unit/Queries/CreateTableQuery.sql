CREATE TABLE `test_database`.`test_table` (
	`ID` BIGINT AUTO_INCREMENT PRIMARY KEY,
	`Name` VARBINARY(50),
	`Description` TEXT,
	`Value` INTEGER NULL,
	`Created` DATETIME,
	`Updated` DATETIME
);