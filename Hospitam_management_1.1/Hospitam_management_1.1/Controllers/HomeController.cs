using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hospitam_management_1._1.DAL;
using Hospitam_management_1._1.Models;

namespace Hospitam_management_1._1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Edit(Guid id)
        {
           upading_account u_c=new upading_account();
            List<account_entites>update_model=  u_c.update_account(id).ToList();    
            return View(update_model);
        }
        public ActionResult account()
        {
            DAL_account_entites account_data = new DAL_account_entites();
            List <account_entites> data = account_data.Retrive_records().ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult Create()
        {
            Account_creation new_account = new Account_creation();
            List<account_creation> department_data = new_account.retrieve_department().ToList();
            ViewBag.department_data = new SelectList(department_data, "department_id","Department");
            List<account_creation> doctors = new_account.Retrieve_doctors().ToList();
            ViewBag.doctors = new SelectList(doctors, "doctor_id", "Doctor");           
            return View();
        }
        [HttpPost]
        public ActionResult Create(account_creation ac)
        {
            Account_creation accountobj = new Account_creation();
            accountobj.new_account(ac);
            return RedirectToAction("account");
        }
    }
}