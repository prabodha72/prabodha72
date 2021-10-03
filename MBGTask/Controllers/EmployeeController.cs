using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBGTask.Models;
using MBGTask.DAL;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace MBGTask.Controllers
{
    [RoutePrefix("employee")]
    public class EmployeeController : BaseController
    {
        // GET: Employee
        [Route("create")]
        [HttpGet]
        public ActionResult CreateEmployee()
        {
            ViewBag.pagename = "Create Employee";
            return View();
        }

        [Route("create")]
        [HttpPost]
        public ActionResult CreateEmployee(Employee employee)
        {
            ViewBag.pagename = "Create Employee";
            if (ModelState.IsValid)
            {
                EmployeeDAL ed = new EmployeeDAL();
                if (ed.AddEmployee(employee))
                {
                    ModelState.Clear();
                    showErrorMessage("Employee created successfull...", ErrorType.success);
                }
                else
                {
                    showErrorMessage("Employee creation failed...", ErrorType.error);
                }
                
            }
            return View();
        }

        [Route("find")]
        [HttpGet]
        public ActionResult FindEmployee()
        {
            ViewBag.pagename = "Search Employee";
            EmployeeDAL ed = new EmployeeDAL();
            DataSet ds = ed.AllEmployee();
            List<Employee> Employees = new List<Employee>();
            if (ds.Tables.Count > 0)
            {
                if(ds.Tables[0].Rows.Count > 0)
                {
                      Employees = ds.Tables[0].AsEnumerable().Select(
                      dataRow => new Employee
                       {
                          empid = dataRow.Field<int>("EmpId"),
                          Name = dataRow.Field<string>("Name"),
                          Designation = dataRow.Field<string>("Designation"),
                          Email = dataRow.Field<string>("Email")
                      }).ToList();
                }
            }
            return View(Employees);
        }

        [Route("edit")]
        [HttpGet]
        public ActionResult EditEmployee(int id)
        {
            ViewBag.pagename = "Edit Employee";
            Employee emp = new Employee();
            EmployeeDAL ed = new EmployeeDAL();
            emp = ed.SearchEmployee(id);
            return View(emp);
        }

        [Route("delete")]
        [HttpGet]
        public ActionResult DeleteEmployee(int id)
        {
            ViewBag.pagename = "Edit Employee";
            EmployeeDAL ed = new EmployeeDAL();
            DataSet ds = ed.GetEmployeeDocuments(id);
            if(ds.Tables.Count > 0)
            {
                if(ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string file = ds.Tables[0].Rows[i]["FilePath"].ToString();
                        string subFileN = "", subFileS = "";
                        int idx = file.LastIndexOf('.');
                        if (idx >= 0)
                        {
                            subFileN = file.Substring(0, idx);
                            subFileS = file.Substring(idx);
                        }

                        //Get the Input File Name and Extension
                        string fileName = subFileN;
                        string fileExtension = subFileS;

                        //Build the File Path for the original (input) and the decrypted (output) file
                        Random rnd = new Random();
                        string FilePath = Server.MapPath("~/db/Files/") + fileName + fileExtension;
                        if (System.IO.File.Exists(FilePath))
                            System.IO.File.Delete(FilePath);
                    }
                    
                }
            }

            if (ed.DeleteEmployee(id))
            {
                return RedirectToAction("FindEmployee", "Employee", new { id = id });
            }
            else
            {
                return RedirectToAction("FindEmployee", "Employee", new { id = id });
            }           
        }

        [Route("edit")]
        [HttpPost]
        public ActionResult EditEmployee(Employee emp)
        {
            ViewBag.pagename = "Edit Employee";
            EmployeeDAL ed = new EmployeeDAL();
            if (ed.UpdateEmployee(emp))
            {
                ModelState.Clear();
                showErrorMessage("Employee updated successfull...", ErrorType.success);
                return RedirectToAction("ViewEmployee","Employee",new { id=emp.empid});
            }
            else
            {
                showErrorMessage("Employee update failed...", ErrorType.error);
                return View();
            }            
        }

        [Route("view")]
        [HttpGet]
        public ActionResult ViewEmployee(int id)
        {
            ViewBag.pagename = "View Employee";
            Employee emp = new Employee();
            EmployeeDAL ed = new EmployeeDAL();
            emp = ed.SearchEmployee(id);
            TempData["id"] = id;
            return View(emp);
        }

        [Route("doc")]
        [HttpGet]
        public ActionResult DocumentEmployee(int id)
        {
            ViewBag.pagename = "Employee Document";
            EmployeeDocument empd = new EmployeeDocument();
            EmployeeDAL ed = new EmployeeDAL();
            empd.employee = ed.SearchEmployee(id);
            DataSet ds = ed.GetEmployeeDocuments(id);
            List <Documents> empdoc= new List<Documents>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    empdoc = ds.Tables[0].AsEnumerable().Select(
                    dataRow => new Documents
                    {
                        EmpDetId = dataRow.Field<int>("EmpDetId"),
                        EmpId = dataRow.Field<int>("EmpId"),
                        FileName = dataRow.Field<string>("FileName"),
                        CreateDate = dataRow.Field<DateTime>("CreateDate")
                    }).ToList();
                }
            }
            empd.doclist = empdoc;
            TempData["id"] = id;
            return View(empd);
        }

        [Route("doc")]
        [HttpPost]
        public ActionResult DocumentEmployee(EmployeeDocument empd)
        {
            ViewBag.pagename = "Employee Document";
            EmployeeDAL ed = new EmployeeDAL();
            DataSet ds = ed.GetEmployeeDocuments(Convert.ToInt32(empd.documents.EmpId));
            List<Documents> empdoc = new List<Documents>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    empdoc = ds.Tables[0].AsEnumerable().Select(
                    dataRow => new Documents
                    {
                        EmpDetId = dataRow.Field<int>("EmpDetId"),
                        EmpId = dataRow.Field<int>("EmpId"),
                        FileName = dataRow.Field<string>("FileName"),
                        FilePath = dataRow.Field<HttpPostedFileBase>("FilePath"),
                        CreateDate = dataRow.Field<DateTime>("CreateDate")
                    }).ToList();
                }
            }
            empd.doclist = empdoc;
            string encryptfile = null;
            if (empd.documents.FilePath.ContentLength > 0)
            {
                //Get the Input File Name and Extension.
                string fileName = Path.GetFileNameWithoutExtension(empd.documents.FilePath.FileName);
                string filepath = Path.GetPathRoot(empd.documents.FilePath.FileName);
                string fileExtension = Path.GetExtension(empd.documents.FilePath.FileName);
                Random rnd = new Random();
                //Build the File Path for the original (input) and the encrypted (output) file.
                string input = Server.MapPath("~/db/Files/") + fileName + fileExtension;
                encryptfile = fileName + "_Encrypted" + rnd.Next(0, 6) + fileExtension;
                string output = Server.MapPath("~/db/Files/") + encryptfile;
                
                //Save the Input File, Encrypt it and save the encrypted file in output path.
                empd.documents.FilePath.SaveAs(input);
                ed.encrypt_File(input, output);
                if (System.IO.File.Exists(Path.Combine(input)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Path.Combine(input));
                }
                //Response.End();                        
            }

            empd.documents.EncryptFilePath = encryptfile;

            if (ed.AddDocument(empd))
            {
                showErrorMessage("Employee updated successfull...", ErrorType.success);
                return RedirectToAction("DocumentEmployee", "Employee", new { id = empd.employee.empid });
            }
            else
            {
                showErrorMessage("Employee update failed...", ErrorType.error);
                return View();
            }
        }

        [Route("download")]
        [HttpGet]
        public void DownloadDocument(string id, Documents doc)
        {
            EmployeeDAL ed = new EmployeeDAL();
            string file = null;
            ed.download(id, out file);
            string subFileN = "", subFileS = "";

            int idx = file.LastIndexOf('.');

            if (idx >= 0)
            {
                subFileN = file.Substring(0, idx);
                subFileS = file.Substring(idx);
            }

            //Get the Input File Name and Extension
            string fileName = subFileN;
            string fileExtension = subFileS;

            //Build the File Path for the original (input) and the decrypted (output) file
            Random rnd = new Random();
            string input = Server.MapPath("~/db/Files/") + fileName + fileExtension;
            string output = Server.MapPath("~/db/Files/") + fileName + "_Decrypted"+ rnd.Next(0,6) + fileExtension;

            //Save the Input File, Decrypt it and save the decrypted file in output path.
            //fileUpload.SaveAs(input);
            if (System.IO.File.Exists(input))
            {
                ed.decrypt_File(input, output);

                //Download the Decrypted File.

                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(output));
                Response.WriteFile(output);
                Response.Flush();
            }
            else
            {
                showErrorMessage("File not availble...", ErrorType.error);
            }
            

            //Delete the original (input) and the decrypted (output) file.
            //File.Delete(input);
            if(System.IO.File.Exists(output))
                System.IO.File.Delete(output);

            Response.End();
        }

        [Route("viewdoc")]
        [HttpGet]
        public void ViewDocument(string id)
        {
            EmployeeDAL ed = new EmployeeDAL();
            string file = null;
            ed.download(id, out file);
            string subFileN = "", subFileS = "";

            int idx = file.LastIndexOf('.');

            if (idx >= 0)
            {
                subFileN = file.Substring(0, idx);
                subFileS = file.Substring(idx);
            }

            //Get the Input File Name and Extension
            string fileName = subFileN;
            string fileExtension = subFileS;

            //Build the File Path for the original (input) and the decrypted (output) file
            Random rnd = new Random();
            string FilePath = Server.MapPath("~/db/Files/") + fileName + fileExtension;
            string output = Server.MapPath("~/db/Files/") + fileName + "_Decrypted" + rnd.Next(0, 6) + fileExtension;

            //Save the Input File, Decrypt it and save the decrypted file in output path.
            //fileUpload.SaveAs(input);
            if (System.IO.File.Exists(FilePath))
            {
                ed.decrypt_File(FilePath, output);
                if (System.IO.File.Exists(output))
                {
                    WebClient User = new WebClient();
                    Byte[] FileBuffer = User.DownloadData(output);
                    if (FileBuffer != null)
                    {
                        Response.ContentType = "application/"+ fileExtension.TrimStart('.');
                        Response.AddHeader("content-length", FileBuffer.Length.ToString());
                        Response.BinaryWrite(FileBuffer);
                    }
                }
                if (System.IO.File.Exists(output))
                    System.IO.File.Delete(output);
            }
            else
            {
                showErrorMessage("File not availble...", ErrorType.error);
            }
            
        }

        [Route("deletedoc")]
        [HttpGet]
        public ActionResult DeleteDocument(string id, string empid)
        {
            EmployeeDAL ed = new EmployeeDAL();
            string file = null;
            ed.download(id, out file);
            string subFileN = "", subFileS = "";

            int idx = file.LastIndexOf('.');

            if (idx >= 0)
            {
                subFileN = file.Substring(0, idx);
                subFileS = file.Substring(idx);
            }

            //Get the Input File Name and Extension
            string fileName = subFileN;
            string fileExtension = subFileS;

            //Build the File Path for the original (input) and the decrypted (output) file
            Random rnd = new Random();
            string FilePath = Server.MapPath("~/db/Files/") + fileName + fileExtension;
            if (System.IO.File.Exists(FilePath))
                System.IO.File.Delete(FilePath);
            if (ed.deletedoc(id))
            {
                return RedirectToAction("DocumentEmployee", "Employee", new { id = empid });
            }
            else { return View(); }           
        }
    }
}