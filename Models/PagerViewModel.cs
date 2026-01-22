namespace WebApp_Anti.Models
{
    public class PagerViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
