using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MBGTask.Models;

namespace MBGTask.DAL
{
    public interface IEmployeeDAL
    {
        bool AddEmployee(Employee emp);
        DataSet AllEmployee();
        Employee SearchEmployee(int id);
        bool UpdateEmployee(Employee emp);
        bool DeleteEmployee(int empid);
        DataSet GetEmployeeDocuments(int empid);
        bool AddDocument(EmployeeDocument docs);

        void encrypt_File(string inputFilePath, string outputfilePath);
        void decrypt_File(string inputFilePath, string outputfilePath);
        bool download(string docid, out string FileName);
        bool deletedoc(string docid);

    }
}
