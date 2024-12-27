BULK INSERT DivisionMaster
    FROM 'C:\Scripts\DivisionTable.csv'
    WITH
    (
    FIRSTROW = 4,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    ERRORFILE = 'C:\Scripts\DivisionMastererror.csv',
	TABLOCK
	)