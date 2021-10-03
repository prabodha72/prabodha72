create database MBGtask;

use MBGtask;

create table Employee(
  EmpId INT IDENTITY(1,1)  PRIMARY KEY,
  [Name] nvarchar(60),
  Email nvarchar(100),
  Designation nvarchar(50)
);

Create table EmployeeDetails(
  EmpDetId int IDENTITY(1,1)  PRIMARY KEY,
  EmpId int foreign key references Employee(EmpID) ,
  [FileName] nvarchar(max),
  [FilePath] varbinary(max),
  CreateDate datetime
);
