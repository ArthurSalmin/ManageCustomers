INSERT INTO Customer (Id, Name, FirstName, DateOfBirth, LockState) 
VALUES ('(SELECT MAX(Id)
FROM CustomerMigrations)','sdfsd','sdfsdf', '2019-01-01T00:00:00','online' )