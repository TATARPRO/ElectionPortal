using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectionPortal.Models;

namespace ElectionPortal.Controllers
{
    [Authorize]
    public class PasscodesController : Controller
    {
        private Entities db = new Entities();

        // GET: Passcodes
        public ActionResult Index()
        {
            return View(db.Passcodes.ToList());
        }

        // GET: Passcodes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passcode passcode = db.Passcodes.Find(id);
            if (passcode == null)
            {
                return HttpNotFound();
            }
            return View(passcode);
        }

        // GET: Passcodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Passcodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PassCodeId,CodeSerial,RegNumber,IsUsed")] Passcode passcode)
        {
            if (ModelState.IsValid)
            {
                db.Passcodes.Add(passcode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(passcode);
        }

        // GET: Passcodes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passcode passcode = db.Passcodes.Find(id);
            if (passcode == null)
            {
                return HttpNotFound();
            }
            return View(passcode);
        }

        // POST: Passcodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PassCodeId,CodeSerial,RegNumber,IsUsed")] Passcode passcode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passcode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(passcode);
        }

        // GET: Passcodes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passcode passcode = db.Passcodes.Find(id);
            if (passcode == null)
            {
                return HttpNotFound();
            }
            return View(passcode);
        }

        // POST: Passcodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Passcode passcode = db.Passcodes.Find(id);
            db.Passcodes.Remove(passcode);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        public ActionResult DeleteAllPasscodes()
        {
            List<Passcode> AllPasscodes = db.Passcodes.ToList();//Get all the passcodes from the database            
            foreach (Passcode Pass in AllPasscodes)
            {
                Passcode passcode = db.Passcodes.Find(Pass.PassCodeId);//find a specified passcode from the database according to loop
                db.Passcodes.Remove(passcode);//remove the database from the entity model
                db.SaveChanges();//save the changes to the database
            }
            return RedirectToAction("Index");//return to index
        }

        public ActionResult CreateAmount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAmount([Bind(Include = "Number")] CreatePasscode NewPasscodes)
        {
            List<Passcode> Passcodes = new List<Passcode>();//passcode list
            char[] AlphaChars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N' };//alphabet array
            Random RandPosition = new Random();//random object
            if (NewPasscodes.Number > 5000)//validate for maximum amount to create
            {
                ViewBag.MaximumAmount = "Maximum amount of passcodes to create at an instance is 5000";
                return View();
            }
            else
            {
                while (NewPasscodes.Number > 0)
                {
                    string pass = "";//create a temporary passcode
                    for (int i = 0; i < 4; i++)
                    {
                        pass += AlphaChars[RandPosition.Next(13)];//get a random letter from the letter array and append it to pass
                        pass += RandPosition.Next(11, 97);//give a random number interval i.e (from 11 to 97) then append it to pass
                    }
                    if (!(Passcodes.Contains(new Passcode { CodeSerial = pass })))//if passcode has not been already created
                    {
                        Passcodes.Add(new Passcode { PassCodeId = pass, CodeSerial = pass, IsUsed = false, IsSubmitted = false });//Add the newly created passcode to the list
                    }
                    NewPasscodes.Number--;//decrease to show the number created
                }
                foreach (Passcode Ps in Passcodes)
                {
                    db.Passcodes.Add(Ps);//add the passcode to the entity model
                    db.SaveChanges();//save the changes to the database
                }
                return RedirectToAction("Index");//redirect to the passcode list
            }
        }

        public ActionResult SearchPasscode()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPasscode([Bind(Include = "Passcode")] SearchPasscode Passcod)
        {
            if (Passcod.Passcode == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passcode passcode = db.Passcodes.Find(Passcod.Passcode);
            if (passcode == null)
            {
                ViewBag.NotFound = "Passcode not found";
                return View();
            }
            SearchPasscode Found = new SearchPasscode();
            if (passcode.RegNumber == null)
            {
                Found.RegNumber = 0; Found.Passcode = passcode.CodeSerial; Found.IsUsed = (bool)passcode.IsUsed;
            }
            else
            {
                Found.Passcode = passcode.CodeSerial; Found.IsUsed = (bool)passcode.IsUsed; Found.RegNumber = (long)passcode.RegNumber;
            }
            ViewBag.Found = "Passcode found!";
            return View(Found);
        }
    }
}
