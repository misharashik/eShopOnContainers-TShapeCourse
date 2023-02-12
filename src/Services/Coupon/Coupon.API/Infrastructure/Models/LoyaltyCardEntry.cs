namespace Coupon.API.Infrastructure.Models
{
    public record LoyaltyCardEntry
    {
        public int OrderId { get; init; }
        public decimal EarnedPoints { get; init; }
        public decimal UsedPoints { get; init; }
    }
}
