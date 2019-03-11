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
    public class ResultsController : Controller
    {
        private Entities db = new Entities();

        // GET: Results
        public ActionResult Index()
        {
            //CalculateResults();
            var results = db.Results.Include(r => r.Post);
            return View(results.ToList());
        }

        // GET: Results/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // GET: Results/Create
        public ActionResult Create()
        {
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName");
            return View();
        }

        // POST: Results/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CandidateId,CandidateName,PostId,VoteCount")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Results.Add(result);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", result.PostId);
            return View(result);
        }

        // GET: Results/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", result.PostId);
            return View(result);
        }

        // POST: Results/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateId,CandidateName,PostId,VoteCount")] Result result)
        {
            if (ModelState.IsValid)
            {
                db.Entry(result).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", result.PostId);
            return View(result);
        }

        // GET: Results/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Result result = db.Results.Find(id);
            if (result == null)
            {
                return HttpNotFound();
            }
            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Result result = db.Results.Find(id);
            db.Results.Remove(result);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// method below needs to be updated whenever there is an update in the database post and passcode
        /// </summary>
        /// <returns></returns>
        public ActionResult CalculateResults()
        {
            List<Passcode>Votes = db.Passcodes.ToList();
            List<Post> posts = db.Posts.ToList();
            
            //Query the passcodes to get only the ones submitted
            var PasscodeQuery = from PQ in Votes
                                         where PQ.IsSubmitted == true
                                         select PQ;
            
            foreach (Passcode SubmittedResult in PasscodeQuery)
                {

                //this section needs to be updated anytime a post is added or changed in the db
                //===============================================================================//
                //===============================================================================//
                //===============================================================================//
                //1==============President=====================
                Result PresidentResult = db.Results.Find(SubmittedResult.President);
                if(PresidentResult != null)
                {
                    PresidentResult.VoteCount++;
                    db.Entry(PresidentResult).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (SubmittedResult.President != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.President);
                    Result NewResult = new Result {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //2==============vicePresident=====================
                Result VpResult = db.Results.Find(SubmittedResult.VicePresident);
                if (VpResult != null)
                {
                    VpResult.VoteCount++;
                    db.Entry(VpResult).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if(SubmittedResult.VicePresident != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.VicePresident);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                   
                }
                //==========================================================================
                //==========================================================================
                //3==============Gen.Sec=====================
                Result GenSec = db.Results.Find(SubmittedResult.GenSec);
                if (GenSec != null)
                {
                    GenSec.VoteCount++;
                    db.Entry(GenSec).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (SubmittedResult.GenSec != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.GenSec);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //4==============Asst.Gen.Sec=====================
                Result AsstGenSec = db.Results.Find(SubmittedResult.AsstGenSec);
                if (AsstGenSec != null)
                {
                    AsstGenSec.VoteCount++;
                    db.Entry(AsstGenSec).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    if (SubmittedResult.AsstGenSec != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.AsstGenSec);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //5==============Financial.Sec=====================
                Result FinacialSec = db.Results.Find(SubmittedResult.FinSec);
                if (FinacialSec != null)
                {
                    FinacialSec.VoteCount++;
                    db.Entry(FinacialSec).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this candidate
                    if (SubmittedResult.FinSec != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.FinSec);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //6==============Organising Sec=====================
                Result OrganisingSec = db.Results.Find(SubmittedResult.OrgSec);
                if (OrganisingSec != null)
                {
                    OrganisingSec.VoteCount++;
                    db.Entry(OrganisingSec).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this post
                    if (SubmittedResult.OrgSec != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.OrgSec);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //7==============Asst. Organising Sec=====================
                Result AsstOrgSec = db.Results.Find(SubmittedResult.AsstOrgSec);
                if (AsstOrgSec != null)
                {
                    AsstOrgSec.VoteCount++;
                    db.Entry(AsstOrgSec).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this post
                    if (SubmittedResult.AsstOrgSec != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.AsstOrgSec);
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //8==============Treasurer=====================
                Result Treasurer = db.Results.Find(SubmittedResult.Treasurer);
                if (Treasurer != null)
                {
                    Treasurer.VoteCount++;
                    db.Entry(Treasurer).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this post
                    if (SubmittedResult.Treasurer != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.Treasurer);//get the candidate info
                    //so that we can fix it in the results table
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                   
                }
                //==========================================================================
                //==========================================================================
                //9==============PRO1=====================
                Result PRO1 = db.Results.Find(SubmittedResult.PRO1);
                if (PRO1 != null)
                {
                    PRO1.VoteCount++;
                    db.Entry(PRO1).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this post
                    if (SubmittedResult.PRO1 != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.PRO1);//get the candidate info
                    //so that we can fix it in the results table
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
                //==========================================================================
                //==========================================================================
                //10==============PRO2=====================
                Result PRO2 = db.Results.Find(SubmittedResult.PRO2);
                if (PRO2 != null)
                {
                    PRO2.VoteCount++;
                    db.Entry(PRO2).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //this check is done if the user did not vote for this post
                    if (SubmittedResult.PRO2 != null)
                    {
                    Candidate Match = db.Candidates.Find(SubmittedResult.PRO2);//get the candidate info
                    //so that we can fix it in the results table
                    Result NewResult = new Result
                    {
                        CandidateId = Match.CandidateId,
                        CandidateName = Match.CandidateName,
                        PostId = Match.PostId,
                        VoteCount = 1
                    };
                    db.Results.Add(NewResult);
                    db.SaveChanges();
                    }
                    
                }
            }

            //===============================================================================//
            //===============================================================================//
            //===============================================================================//
            return RedirectToAction("Index");
        }

        public ActionResult ResetResults()
        {
            List<Result> AllResults = db.Results.ToList();//Get all the passcodes from the database            
            foreach (Result ress in AllResults)
            {
                Result result = db.Results.Find(ress.CandidateId);//find a specified passcode from the database according to loop
                db.Results.Remove(result);//remove the database from the entity model
                db.SaveChanges();//save the changes to the database
            }
            return RedirectToAction("Index");//return to index
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
