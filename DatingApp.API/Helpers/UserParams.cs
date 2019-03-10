namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;
        public int PageSize 
        {
            get { return _pageSize; }
            set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int UserId { get; set; }

        public string Gender { get; set; }

        public int MinAge {get; set; } = 18;

        public int MaxAge {get; set; } = 99;

        public string OrderBY { get; set; } 
    }
}