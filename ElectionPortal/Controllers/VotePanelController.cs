using ElectionPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ElectionPortal.Controllers
{
    [HandleError]
    public class VotePanelController : Controller
    {
        private Entities db = new Entities();
        // GET: VotePanel
        public ActionResult Index()
        {
            if(IsAuthenticated())
            {
                List<Post> posts = db.Posts.ToList();
                List<Candidate> candidate = db.Candidates.ToList();
                VotePanel vp = new VotePanel()
                {
                    CandidatesCollection = candidate,
                    PostsCollection = posts
                };
                return View(vp);
            }          
            return RedirectToAction("ElectionLogin", "Home");     //the user is not authenticated       
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(JsonResult votes)
        {
            return RedirectToAction("");
        }

        public ActionResult SubmitLogout()
        {
            if(IsAuthenticated())
            {
                string loggedUser = this.ControllerContext.HttpContext.Request.Cookies[".ASPXAUTH"].Value;
                this.ControllerContext.HttpContext.Request.Cookies.Remove(".ASPXAUTH");
                this.ControllerContext.HttpContext.Request.Cookies.Clear();
                
                //FormsAuthenticationTicket mmUserTicket = FormsAuthentication.Decrypt("D93AC4B694B984DDBE7248F0605688668406CA82F785494BF258CA80901457C47A584F3825E44563FDA931315F5D2F840C5ECD6C631DB2D16C9C0E7185E87219155086961E42BDD861D4DE766C63B9353F08ACBBC2E7BD70DB3B43248A17C7AE7B891498A7C1ADE31F7BDB0132D3E8F9DFCE753E77872A9FF6EDA8492FE9D2C9");
                FormsAuthenticationTicket UserTicket = FormsAuthentication.Decrypt(loggedUser);//decrypt the cookie to get values               
                    char[] splitters = new char[2] { ';', ' ' };
                    string[] Userdata = UserTicket.Name.Split(splitters).ToArray();
                    string passcode = Userdata[0];//get the passcode from the cookie
                RegNumber reg = db.RegNumbers.Find(passcode);

                reg.IsSubmitted = true;//update isSubmitted property to true
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");//user has been deauthenticated
            }
            return RedirectToAction("ElectionLogin", "Home");//user not authenticated
        }
        
        private bool IsAuthenticated()
        {
            string loggedUser = "";
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(".ASPXAUTH"))
            {
                loggedUser = this.ControllerContext.HttpContext.Request.Cookies[".ASPXAUTH"].Value;
                FormsAuthenticationTicket UserTicket = FormsAuthentication.Decrypt(loggedUser);
                if (UserTicket.Name != "")
                {
                    char[] splitters = new char[1] { ';'};
                    string[] Userdata = UserTicket.Name.Split(splitters).ToArray();
                    long regNumber = long.Parse(Userdata[0]);// get the regNumber from the cookie
                    RegNumber regnumber = db.RegNumbers.Find(regNumber);//find reg number from database          
                    if (regnumber == null)
                    {
                        //return RedirectToAction("ElectionLogin", "Home");//authentication cookie received but regnumber not found
                        return false;
                    }
                    
                    if ((bool)regnumber.IsSubmitted)
                    {
                        //return RedirectToAction("ElectionLogin", "Home");//authentication cookiew received but passcode not found
                        return false;
                    }
                    
                    return true;
                }
                //return RedirectToAction("ElectionLogin", "Home");//authentication cookie sent but value is empty
                return false;
            }
            
            return false;
        }
       
    }
}