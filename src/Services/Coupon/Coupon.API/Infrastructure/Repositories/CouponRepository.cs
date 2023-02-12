namespace Coupon.API.Infrastructure.Repositories
{
    using Coupon.API.Infrastructure.Models;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public class CouponRepository : ICouponRepository
    {
        private readonly MongoDbContext _dbContext;

        public CouponRepository(MongoDbContext couponContext)
        {
            _dbContext = couponContext;
        }

        public async Task UpdateCouponConsumedByCodeAsync(string code, int orderId)
        {
            FilterDefinition<Coupon> filter = Builders<Coupon>.Filter.Eq("Code", code);
            UpdateDefinition<Coupon> update = Builders<Coupon>.Update
                .Set(coupon => coupon.Consumed, true)
                .Set(coupon => coupon.OrderId, orderId);

            await _dbContext.Coupons.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public async Task UpdateCouponReleasedByOrderIdAsync(int orderId)
        {
            FilterDefinition<Coupon> filter = Builders<Coupon>.Filter.Eq("OrderId", orderId);
            UpdateDefinition<Coupon> update = Builders<Coupon>.Update
                .Set(coupon => coupon.Consumed, false)
                .Set(coupon => coupon.OrderId, 0);

            await _dbContext.Coupons.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
        }

        public async Task<Coupon> FindCouponByCodeAsync(string code)
        {
            FilterDefinition<Coupon> filter = Builders<Coupon>.Filter.Eq("Code", code);
            return await _dbContext.Coupons.Find(filter).FirstOrDefaultAsync();
        }
    }
}
