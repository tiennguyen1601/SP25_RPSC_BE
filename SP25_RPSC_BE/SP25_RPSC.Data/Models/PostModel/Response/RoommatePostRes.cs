using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.PostModel.Response
{
    public class RoommatePostRes
    {
        public string PostId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PostOwnerInfo PostOwnerInfo { get; set; }

    }

    public class PostOwnerInfo
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? LifeStyle { get; set; }
        public string? Requirement { get; set; }
        public string? PostOwnerType { get; set; }
    }

    public class RoommatePostSearchReq
    {
        public string? Address { get; set; }
        public decimal? MinBudget { get; set; }
        public decimal? MaxBudget { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? Gender { get; set; }
        public string[]? LifeStyles { get; set; }
        public string[]? CustomerTypes { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }


}
