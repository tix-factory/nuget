USE `test_database`;
CREATE PROCEDURE `test_table_insert`(
	IN _Name VARBINARY(50),
	IN _Description TEXT,
	IN _Value INTEGER
)
BEGIN
	INSERT INTO `test_database`.`test_table`
	(
		`Name`,
		`Description`,
		`Value`,
		`Created`,
		`Updated`
	)
	VALUES
	(
		_Name,
		_Description,
		_Value,
		UTC_Timestamp(),
		UTC_Timestamp()
	);
	
	SELECT LAST_INSERT_ID() as `ID`;
END;