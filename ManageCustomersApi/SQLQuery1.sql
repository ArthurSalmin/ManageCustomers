SELECT DISTINCT cust.Id, cust.Name, cust.FirstName, cust.DateOfBirth, cust.LockState, cust.IdUserLocked, cm1.TimeOfMigration, cm1.Street, cm1.IdCity
FROM CustomerMigrations cm1
INNER JOIN Customer cust ON cm1.IdCustomer =cust.Id
AND cust.Id = 3
ORDER BY cm1.TimeOfMigration