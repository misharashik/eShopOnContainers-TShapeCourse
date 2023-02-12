namespace Coupon.API.Infrastructure.Repositories
{
    using Coupon.API.Infrastructure.Models;
    using System.Threading.Tasks;

    public interface ILoyaltyCardRepository
    {
        public Task<LoyaltyCard> FindLoyaltyCardByBuyerAsync(int buyerId);

        public Task<bool> TryProcessLoyaltyEntryAsync(int buyerId, LoyaltyCardEntry loyaltyEntry);
    }
}
