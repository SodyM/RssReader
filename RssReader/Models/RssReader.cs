using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace RssReader.Models
{
    /// <summary>
    /// Model class for all rss data
    /// </summary>
    public class RssHeader
    {
        public int ID { get; set; }

        [Required]
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual List<Article> Datas { get; set; }                
    }

    /// <summary>
    /// Model class for articles data
    /// </summary>
    public class Article
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime? PublicationDate { get; set; }
    }

    /// <summary>
    /// EF (CF) DB Context
    /// </summary>
    public class RssFeedDBContext : DbContext
    {
        public DbSet<RssHeader> RssHeaders { get; set; }
        public DbSet<Article> RssDatas { get; set; }
    }
}