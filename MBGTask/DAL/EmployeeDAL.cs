using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using MBGTask.Models;
using MBGTask.Controllers;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MBGTask.DAL
{
    public class EmployeeDAL:IEmployeeDAL
    {
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        public bool AddEmployee(Employee emp)
        {
            bool isSuccess = false;
            try
            {
                //int 
                SqlCommand cmd = new SqlCommand("spEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "insert");
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Designation", emp.Designation);
                con.Open();
                int success = cmd.ExecuteNonQuery();
                if(success == 1)
                {
                    isSuccess = true;
                }
                con.Close();               
            }
            catch{                       
            }
            return isSuccess;
        }

        public Employee SearchEmployee(int id)
        {
            Employee emp = new Employee();
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "view");
                cmd.Parameters.AddWithValue("@EmpId", id);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader(); ;
                while(sdr.Read())
                {
                    emp.empid = Convert.ToInt32(sdr["EmpId"]);
                    emp.Name = sdr["Name"].ToString();
                    emp.Email = sdr["Email"].ToString();
                    emp.Designation = sdr["Designation"].ToString();
                }
                con.Close();
            }
            catch { }          
            return emp;
        }

        public DataSet AllEmployee()
        {
            DataSet ds = new DataSet();
            try
            {               
                SqlCommand cmd = new SqlCommand("spEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "search");              
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);
                con.Close();                
            }
            catch { }
            return ds;
        }

        public bool UpdateEmployee(Employee emp)
        {
            bool isSuccess = false;
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "update");
                cmd.Parameters.AddWithValue("@EmpId", emp.empid);
                cmd.Parameters.AddWithValue("@Name", emp.Name);
                cmd.Parameters.AddWithValue("@Email", emp.Email);
                cmd.Parameters.AddWithValue("@Designation", emp.Designation);
                con.Open();
                int success = cmd.ExecuteNonQuery();
                if (success == 1)
                {
                    isSuccess = true;
                }
                con.Close();
            }
            catch
            {
            }
            return isSuccess;
        }

        public DataSet GetEmployeeDocuments(int empid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployeeDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "searchbyemployee");
                cmd.Parameters.AddWithValue("@EmpId", empid);
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);
                con.Close();
            }
            catch { }
            return ds;
        }

        public bool AddDocument(EmployeeDocument docs)
        {
            bool IsSuccess = false;
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployeeDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "insert");
                cmd.Parameters.AddWithValue("@FileName", docs.documents.FileName);
                cmd.Parameters.AddWithValue("@FilePath", Encoding.UTF8.GetBytes(docs.documents.EncryptFilePath));
                cmd.Parameters.AddWithValue("@EmpId", docs.employee.empid);
                con.Open();
                int success = cmd.ExecuteNonQuery();
                if (success == 1)
                {
                    IsSuccess = true;
                }
                con.Close();
            }
            catch (Exception Ex)
            {
            }
            return IsSuccess;
        }

        public void encrypt_File(string inputFilePath, string outputfilePath)
        {
            string EncryptionKey = "CODINGVILA";
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (FileStream fsOutput = new FileStream(outputfilePath, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fsOutput, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open))
                        {
                            int data;
                            while ((data = fsInput.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                            }
                        }
                    }
                }
            }
        }

        public void decrypt_File(string inputFilePath, string outputfilePath)
        {
            string EncryptionKey = "CODINGVILA";
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open))
                {
                    using (CryptoStream cs = new CryptoStream(fsInput, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (FileStream fsOutput = new FileStream(outputfilePath, FileMode.Create))
                        {
                            int data;
                            while ((data = cs.ReadByte()) != -1)
                            {
                                fsOutput.WriteByte((byte)data);
                            }
                        }
                    }
                }
            }
        }

        public bool download(string docid, out string FileName)
        {
            bool isDownload = false;
            FileName = null;
            SqlCommand cmd = new SqlCommand("spEmployeeDocument", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "searchbyfileid");
            cmd.Parameters.AddWithValue("@EmpDetId", docid);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                FileName = sdr["FilePath"].ToString();
            }
            con.Close();
            return isDownload;
        }

        public bool deletedoc(string docid)
        {
            bool isDownload = false;
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployeeDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "delete");
                cmd.Parameters.AddWithValue("@EmpDetId", docid);
                con.Open();
                int exe = cmd.ExecuteNonQuery();
                if (exe == 1)
                {
                    isDownload = true;
                }
                con.Close();
            }
            catch { }          
            return isDownload;
        }

        public bool DeleteEmployee(int empid)
        {
            bool isDownload = false;
            try
            {
                SqlCommand cmd = new SqlCommand("spEmployee", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "delete");
                cmd.Parameters.AddWithValue("@EmpId", empid);
                con.Open();
                int exe = cmd.ExecuteNonQuery();
                if (exe == 1)
                {
                    isDownload = true;
                }
                con.Close();
            }
            catch { }
            return isDownload;
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          