SELECT --cust.Id, cust.Name, cust.FirstName, cust.DateOfBirth, cust.LockState, cust.IdUserLocked,
cm2.*
FROM customermigrations cm1
INNER JOIN customermigrations cm2 ON cm1.TimeOfMigration < cm2.TimeOfMigration AND cm1.IdCustomer = cm2.IdCustomer
--INNER JOIN customer cust ON cm1.IdCustomer = cust.Id