alter Procedure spEmployeeDocument
@Action nvarchar(50),
@EmpDetId int = null,
@EmpId int = null,
@FileName nvarchar(max) = null,
@FilePath varbinary(max) = null
As
Begin
  if(@Action = 'insert')
   begin
    insert into EmployeeDetails(EmpId,[FileName], FilePath, CreateDate) values (@EmpId, @FileName, @FilePath, GetDate());
   end
  if(@Action = 'searchbyemployee')
  begin
    select convert(varchar(max), FilePath) as FilePath, EmpDetId, EmpId, [FileName], FilePath, CreateDate from EmployeeDetails where EmpId = @EmpId
  end

  if(@Action = 'searchbyfileid')
  begin
    select convert(varchar(max),FilePath) as FilePath, EmpDetId, EmpId, [FileName], FilePath, CreateDate from EmployeeDetails where EmpDetId = @EmpDetId;
  end

  if(@Action = 'delete')
  begin
   delete from EmployeeDetails where EmpDetId = @EmpDetId;
  end
    

  if(@Action = 'update')
    begin
    update EmployeeDetails set [FileName] = @FileName, FilePath = @FilePath where EmpDetId = @EmpDetId;
	end
End