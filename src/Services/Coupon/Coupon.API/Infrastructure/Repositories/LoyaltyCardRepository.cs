namespace Coupon.API.Infrastructure.Repositories
{
    using Coupon.API.Infrastructure.Models;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    public class LoyaltyCardRepository : ILoyaltyCardRepository
    {
        private readonly MongoDbContext _dbContext;

        public string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            if (propertyLambda.Body is not MemberExpression member)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));
            }

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));
            }

            return propInfo.Name;
        }

        public LoyaltyCardRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LoyaltyCard> FindLoyaltyCardByBuyerAsync(int buyerId)
        {
            FilterDefinition<LoyaltyCard> filter = Builders<LoyaltyCard>.Filter.Eq<int>(x => x.BuyerId, buyerId);
            return await _dbContext.LoyaltyCards.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> TryProcessLoyaltyEntryAsync(int buyerId, LoyaltyCardEntry loyaltyEntry)
        {
            FilterDefinition<LoyaltyCard> filter = Builders<LoyaltyCard>.Filter.Eq(x => x.BuyerId, buyerId);
            LoyaltyCard card = await _dbContext.LoyaltyCards.Find(filter).FirstOrDefaultAsync();

            if ((card.TotalEarnedPoints - card.TotalUsedPoints) < loyaltyEntry.UsedPoints)
            {
                return false;
            }

            if (card == null)
            {
                card = new LoyaltyCard()
                {
                    BuyerId = buyerId,
                    TotalEarnedPoints = loyaltyEntry.EarnedPoints,
                    TotalUsedPoints = loyaltyEntry.UsedPoints,
                    LoyaltyEntries = new List<LoyaltyCardEntry>() { loyaltyEntry }
                };
                await _dbContext.LoyaltyCards.InsertOneAsync(card);
                return true;
            }

            if (card.LoyaltyEntries?.Any(t => t.OrderId == loyaltyEntry.OrderId) != true)
            {
                if (card.LoyaltyEntries is null)
                {
                    card.LoyaltyEntries = new List<LoyaltyCardEntry>() { loyaltyEntry };
                }
                else
                {
                    card.LoyaltyEntries.Add(loyaltyEntry);
                }

                UpdateDefinition<LoyaltyCard> update = Builders<LoyaltyCard>.Update
                    .Set(c => c.TotalEarnedPoints, card.TotalEarnedPoints + loyaltyEntry.EarnedPoints)
                    .Set(c => c.TotalUsedPoints, card.TotalUsedPoints + loyaltyEntry.UsedPoints)
                    .Set(c => c.LoyaltyEntries, card.LoyaltyEntries);

                await _dbContext.LoyaltyCards.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
                return true;
            }

            //TODO: Implement loyalty card entry ammend
            return false;
        }
    }
}
