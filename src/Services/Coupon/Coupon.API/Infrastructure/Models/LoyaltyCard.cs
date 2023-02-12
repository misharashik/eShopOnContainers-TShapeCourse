using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Coupon.API.Infrastructure.Models
{
    [BsonIgnoreExtraElements]
    public record LoyaltyCard
    {
        public int BuyerId { get; init; }

        public decimal TotalEarnedPoints { get; init; }
        public decimal TotalUsedPoints { get; init; }

        public ICollection<LoyaltyCardEntry> LoyaltyEntries { get; set; }
    }
}
