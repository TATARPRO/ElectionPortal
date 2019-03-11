using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectionPortal.Models;
using OfficeOpenXml;
using System.IO;
using System.Text;

namespace ElectionPortal.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {        
        private Entities db = new Entities();

        // GET: Posts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,PostName")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,PostName")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ExcelRegNumbers()
        {
            return View();
        }

        public ActionResult ExcelRegNumbers(HttpPostedFileBase excelfile)
        {
            if (excelfile != null && excelfile.ContentLength > 0)
            {
                string extension = Path.GetExtension(excelfile.FileName);
                StringBuilder builder = new StringBuilder(extension);

                if (builder.ToString() == ".xlsx" || builder.ToString() == ".xls")
                {

                    using (ExcelPackage expackage = new ExcelPackage(excelfile.InputStream))
                    {
                        ExcelWorksheet currentSheet = expackage.Workbook.Worksheets[1];

                        //Get the number of rows in the sheet    
                        int NumOfRows = currentSheet.Dimension.End.Row;

                        List<RegNumber> RegList = new List<RegNumber>();
                        for (int row = 1; row <= NumOfRows; row++)
                        {
                            if (currentSheet.Row(row) != null)
                            {
                                //Create a new student object to be inserted into the database
                                RegNumber NewReg = new RegNumber();

                                //Check if the Reg number or surname is not empty
                                if (currentSheet.Cells[row, 1].Value != null)
                                {
                                    NewReg.RegId = (long)currentSheet.Cells[row, 1].Value;
                                    RegList.Add(NewReg);
                                }
                            }

                        }

                        foreach (RegNumber r in RegList)
                        {
                            //check if the selected student exists
                            RegNumber existed = db.RegNumbers.Find(r.RegId);
                            if (existed == null)
                            {
                                db.RegNumbers.Add(r);
                                db.SaveChanges();
                            }
                        }
                    }
                    ViewBag.Success = "Reg numbers uploaded successfully!";
                    return View();    // reg numbers uploaded successfully                                        
                }
                ViewBag.CorruptFile = "Uploaded file is not of the appropriate format.";
                return View();
            }

            ViewBag.CorruptFile = "Uploaded error";
            return View();//if this statement is reached then the file is corrupt or there is no file posted

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
