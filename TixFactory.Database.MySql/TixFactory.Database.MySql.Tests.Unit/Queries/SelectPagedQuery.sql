IF (@IsAscending = 1) THEN
    IF (@ExclusiveStart IS NULL) THEN
        SELECT *
            FROM `test_database`.`test_table`
            ORDER BY `ID` ASC
            LIMIT @Count;
    ELSE
        SELECT *
            FROM `test_database`.`test_table`
            WHERE (`ID` > @ExclusiveStart)
            ORDER BY `ID` ASC
            LIMIT @Count;
    END IF;
ELSE
    IF (@ExclusiveStart IS NULL) THEN
        SELECT *
            FROM `test_database`.`test_table`
            ORDER BY `ID` DESC
            LIMIT @Count;
    ELSE
        SELECT *
            FROM `test_database`.`test_table`
            WHERE (`ID` < @ExclusiveStart)
            ORDER BY `ID` DESC
            LIMIT @Count;
    END IF;
END IF;