using ElectionPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ElectionPortal.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();

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

        public ActionResult ElectionLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ElectionLogin([Bind(Include = "RegNumber")] LoginPanel CastUser)
        {
            if (!ModelState.IsValid)
            {
                new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RegNumber regnumber = db.RegNumbers.Find(CastUser.RegNumber);//find reg number from database 
            if (regnumber == null)
            {
                ViewBag.NotFound = "Registration number not found";
                return View();
            }
           
            string LoginTime = string.Format(DateTime.Today.Day.ToString() + "/" +DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString()
                + " " + FormatHour(DateTime.Now.Hour) + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
           
           
            string userdetails = regnumber.RegId + ";" + LoginTime;

            FormsAuthentication.SetAuthCookie(userdetails, true);//set the passcode and regNumber in the user cookie

            /* HttpCookie usercookie = new HttpCookie("LoggedUser", userdetails);
             usercookie.Expires = DateTime.Now.AddMinutes(40);
             usercookie.Secure = true;
             Request.Cookies.Add(usercookie); */
            
            db.Entry(regnumber).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "VotePanel");
            
        }

        private string FormatHour(int hour)
        {
            if(hour > 12)
            {
                return (hour - 12).ToString() ;
            }
            return hour.ToString();
        }

        public ActionResult Services()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}