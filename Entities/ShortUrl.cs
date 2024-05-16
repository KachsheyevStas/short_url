using System.ComponentModel.DataAnnotations.Schema;

namespace short_url.Entities
{
    public class UrlMapping
    {
        public Guid Id { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public int CountClick { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
