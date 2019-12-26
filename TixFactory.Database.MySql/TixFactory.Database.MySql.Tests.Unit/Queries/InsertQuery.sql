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
	@Name,
	@Description,
	@Value,
	UTC_Timestamp(),
	UTC_Timestamp()
);

SELECT LAST_INSERT_ID() as `ID`;