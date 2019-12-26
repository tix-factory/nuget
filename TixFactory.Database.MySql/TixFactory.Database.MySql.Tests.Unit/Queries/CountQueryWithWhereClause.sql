SELECT COUNT(*) as `Count`
	FROM `test_database`.`test_table`
	WHERE (`ID` > @id);