using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElectionPortal.Models;
using System.IO;
using System.Drawing;
using System.ComponentModel;

namespace ElectionPortal.Controllers
{
    [Authorize]
    public class CandidatesController : Controller
    {
        private Entities db = new Entities();

        // GET: Candidates
        public ActionResult Index()
        {
            var candidates = db.Candidates.Include(c => c.Post);
            return View(candidates.ToList());
        }

        // GET: Candidates/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            if (candidate.Picture != null)
                ViewBag.Base64String = "data:image/jpg;base64," + Convert.ToBase64String(candidate.Picture, 0, candidate.Picture.Length);

            return View(candidate);
        }

        // GET: Candidates/Create
        public ActionResult Create()
        {
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName");
            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CandidateId,CandidateName,PostId,Picture,ImageMimeType")] Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                db.Candidates.Add(candidate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", candidate.PostId);
            return View(candidate);
        }

        // GET: Candidates/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            if(candidate.Picture != null)
            ViewBag.Base64String = "data:image/jpg;base64," + Convert.ToBase64String(candidate.Picture, 0, candidate.Picture.Length);

            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", candidate.PostId);
            return View(candidate);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateId,CandidateName,PostId,Picture,ImageMimeType")] Candidate candidate, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    candidate.ImageMimeType = Image.ContentType;
                    candidate.Picture = new byte[Image.ContentLength];
                    Image.InputStream.Read(candidate.Picture, 0, Image.ContentLength);
                }
                db.Entry(candidate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.PostId = new SelectList(db.Posts, "PostId", "PostName", candidate.PostId);
            return View(candidate);
        }

        // GET: Candidates/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Candidate candidate = db.Candidates.Find(id);
            db.Candidates.Remove(candidate);
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

        public byte[] ImageNameToBytes(string ImageName)
        {
            if (ImageName != "")
            {
                MemoryStream ImageMemory = new MemoryStream();
                Image UploadedImage = Image.FromFile(ImageName);
                UploadedImage.Save(ImageMemory, UploadedImage.RawFormat);
                ImageMemory.Close();
                return ImageMemory.ToArray();
            }
            return null;
        }

        public static byte[] ImageToBytes(Image GivenImage)
        {
            MemoryStream ImageMemory = new MemoryStream();
            GivenImage.Save(ImageMemory, GivenImage.RawFormat);
            ImageMemory.Close();
            return ImageMemory.ToArray();
        }

        public FileContentResult GetImage(int candId)
        {
            Candidate cand = db.Candidates.Find(candId);
            if (cand != null)
            {
                //MemoryStream PictureStream = new MemoryStream(mPicture);
                
                return File(cand.Picture, cand.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public string ImageToBase64String(Image image)
        {
            try
            {
                Bitmap bmp = new Bitmap(image);
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Bitmap));
                string ImageString = Convert.ToBase64String((byte[])converter.ConvertTo(bmp, typeof(byte[])));
                return ImageString;
            }
            catch (IOException)
            {
                return "";
            }
            
        }

        public Bitmap Base64StringToImage(string value)
        {
            byte[] ImageBytes = Convert.FromBase64String(value);
            MemoryStream ImageMemory = new MemoryStream(ImageBytes);
            return new Bitmap(ImageMemory);
        }
    }
}
