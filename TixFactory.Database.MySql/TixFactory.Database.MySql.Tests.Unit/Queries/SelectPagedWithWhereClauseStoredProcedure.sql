USE `test_database`;
CREATE PROCEDURE `test_table_select_paged`(
	IN _id BIGINT,
	IN _IsAscending BIT,
	IN _ExclusiveStart BIGINT,
	IN _Count INTEGER
)
BEGIN
	IF (_IsAscending = 1) THEN
	    IF (_ExclusiveStart IS NULL) THEN
	        SELECT *
	            FROM `test_database`.`test_table`
				WHERE (`ID` = _id)
	            ORDER BY `ID` ASC
	            LIMIT _Count;
	    ELSE
	        SELECT *
	            FROM `test_database`.`test_table`
	            WHERE (`ID` > _ExclusiveStart) AND (`ID` = _id)
	            ORDER BY `ID` ASC
	            LIMIT _Count;
	    END IF;
	ELSE
	    IF (_ExclusiveStart IS NULL) THEN
	        SELECT *
	            FROM `test_database`.`test_table`
				WHERE (`ID` = _id)
	            ORDER BY `ID` DESC
	            LIMIT _Count;
	    ELSE
	        SELECT *
	            FROM `test_database`.`test_table`
	            WHERE (`ID` < _ExclusiveStart) AND (`ID` = _id)
	            ORDER BY `ID` DESC
	            LIMIT _Count;
	    END IF;
	END IF;
END;