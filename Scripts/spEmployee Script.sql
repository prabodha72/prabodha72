Create Procedure spEmployee
@Action nvarchar(50),
@EmpId int = null,
@Name nvarchar(60) = null,
@Email nvarchar(100) = null,
@Designation nvarchar(50) = null
As
Begin
  if(@Action = 'insert')
   begin
    insert into Employee ([Name],Email, Designation) values (@Name, @Email, @Designation);
   end
  if(@Action = 'search')
  begin
    Declare @sqlwhere nvarchar(max);
	Declare @Sqlq nvarchar(max); 
	set @sqlwhere = ' where';
    if(@Name != null and @Name != '')
	  set @sqlwhere +=  ' [Name] like ''%'+@Name+'%'' and';
	if(@Email != null and @Email != '')
	  set @sqlwhere += ' Email = '+ @Email +' and';
	if(@Designation != null and @Designation != '')
	  set @sqlwhere += ' Designation = '+@Designation+' and';
	set @sqlwhere += ' EmpId <> 0 ';
	set @Sqlq = 'select * from Employee ' + @sqlwhere;
	Exec (@Sqlq);    
  end

  if(@Action = 'view')
  begin
    select * from Employee where EmpId = @EmpId;
  end

  if(@Action = 'delete')
  begin
   delete from EmployeeDetails where EmpId = @EmpId;
   delete from Employee where EmpId = @EmpId;
  end
    

  if(@Action = 'update')
    begin
    update Employee set [Name] = @Name, Email = @Email, Designation = @Designation where EmpId = @EmpId;
	end
End