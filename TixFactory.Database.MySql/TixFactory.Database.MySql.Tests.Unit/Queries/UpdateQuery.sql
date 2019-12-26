UPDATE `test_database`.`test_table`
SET
	`Name` = @Name,
	`Description` = @Description,
	`Value` = @Value,
	`Updated` = UTC_Timestamp()
WHERE (`ID` = @id)
LIMIT 1;