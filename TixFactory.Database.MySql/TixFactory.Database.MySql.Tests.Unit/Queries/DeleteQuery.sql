DELETE
	FROM `test_database`.`test_table`
	WHERE (`ID` = @id)
	LIMIT 1;