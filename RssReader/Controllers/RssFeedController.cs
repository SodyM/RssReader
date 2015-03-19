using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RssReader.Models;
using System.Xml.Linq;

namespace RssReader.Controllers
{
    public class RssFeedController : Controller
    {
        private RssFeedDBContext db = new RssFeedDBContext();

        // GET: RssFeed
        public ActionResult Index()
        {
            return View(db.RssHeaders.ToList());
        }
        
        // GET: RssFeed/Details/5
        public ActionResult Details(int? id, string from, string until)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RssHeader rssHeader = db.RssHeaders.Find(id);
            if (rssHeader == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(until))
            {
                DateTime datefrom = new DateTime();
                DateTime.TryParse(from, out datefrom);

                DateTime dateUntil = new DateTime();
                DateTime.TryParse(until, out dateUntil);
                dateUntil = dateUntil.AddHours(23);
                dateUntil = dateUntil.AddMinutes(59);
                dateUntil = dateUntil.AddSeconds(59);


                var filter = from a in rssHeader.Datas
                             where (a.PublicationDate >= datefrom && a.PublicationDate <= dateUntil)
                             select a;

                if (string.IsNullOrEmpty(from))
                {
                    filter = from a in rssHeader.Datas
                             where a.PublicationDate <= dateUntil
                             select a;
                }
                else if(string.IsNullOrEmpty(until))
                {
                    filter = from a in rssHeader.Datas
                                 where a.PublicationDate >= datefrom
                                 select a;
                }

                List<Article> filteredArticles = new List<Article>(filter);
                rssHeader.Datas.Clear();
                rssHeader.Datas.AddRange(filteredArticles);
            }
            
            return View(rssHeader);
        }
        
        // GET: RssFeed/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RssFeed/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Link")] RssHeader rssHeader)
        {
            if (ModelState.IsValid)
            {
                db.RssHeaders.Add(GetData(rssHeader.Link));
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rssHeader);
        }
        
        // GET: RssFeed/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RssHeader rssHeader = db.RssHeaders.Find(id);
            if (rssHeader == null)
            {
                return HttpNotFound();
            }
            return View(rssHeader);
        }

        // POST: RssFeed/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RssHeader rssHeader = db.RssHeaders.Include(x => x.Datas).SingleOrDefault(x => x.ID.Equals(id));
            db.RssHeaders.Remove(rssHeader);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSelectedFeeds(string[] feedIdsToDelete)
        {
            foreach (var id in feedIdsToDelete)
            {
                int currentId = Convert.ToInt16(id);
                RssHeader rssHeader = db.RssHeaders.Include(x => x.Datas).SingleOrDefault(x => x.ID.Equals(currentId));
                db.RssDatas.RemoveRange(rssHeader.Datas);
                db.RssHeaders.Remove(rssHeader);                
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: RssFeed/Error
        public ActionResult Error()
        {
            // simple error page
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

        #region helpers
        DateTime? ConvertToDateTime(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                DateTime result;
                DateTime.TryParse(value, out result);

                return result;
            }
            return null;
        }

        /// <summary>
        /// Extract all data from actual feed
        /// </summary>
        /// <param name="url">url of actual feed</param>
        /// <returns>RssHeader with all data</returns>
        public RssHeader GetData(string url)
        {
            RssHeader result = new RssHeader();
            XDocument feedXml = XDocument.Load(url);
            
            // header data
            result.Link = url;
            var title = feedXml.Descendants("channel").Elements("title").FirstOrDefault();
            if (title != null)
                result.Title = (string)title;

            var description = feedXml.Descendants("channel").Elements("description").FirstOrDefault();
            if (description != null)
                result.Description = (string)description;
            
            // positions
            var feeds = from feed in feedXml.Descendants("item")
                        select new Article
                        {
                            Title = feed.Element("title").Value,
                            Link = feed.Element("link").Value,
                            Description = feed.Element("description").Value,
                            PublicationDate = ConvertToDateTime(feed.Element("pubDate").Value)
                        };

            result.Datas = feeds.ToList();

            return result;
        }
        #endregion
    }
}