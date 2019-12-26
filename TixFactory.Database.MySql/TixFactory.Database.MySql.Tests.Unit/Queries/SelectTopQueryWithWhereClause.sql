SELECT *
	FROM `test_database`.`test_table`
	WHERE (`ID` = @id)
	LIMIT @Count;