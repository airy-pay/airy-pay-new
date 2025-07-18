﻿using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Common.Repositories;
using AiryPayNew.Domain.Entities.Purchases;
using AiryPayNew.Domain.Entities.ShopComplaints;
using AiryPayNew.Domain.Entities.Withdrawals;

namespace AiryPayNew.Domain.Entities.Shops;

public interface IShopRepository : IDefaultRepository<ShopId, Shop>, INoTrackRepository<ShopId, Shop>
{
    public Task<IList<Purchase>> GetShopPurchasesAsync(
        ShopId shopId, int amount, CancellationToken cancellationToken);
    public Task<IList<Withdrawal>> GetShopWithdrawalsAsync(
        ShopId shopId, int amount, CancellationToken cancellationToken);
    public Task<IList<ShopComplaint>> GetShopComplaintsAsync(
        ShopId id, CancellationToken cancellationToken, ulong userId = 0);
    public Task BlockAsync(
        ShopId shopId, CancellationToken cancellationToken);
    public Task UnblockAsync(
        ShopId shopId, CancellationToken cancellationToken);
    public Task<OperationResult<ShopId>> UpdateBalanceAsync(
        ShopId shopId, decimal change, CancellationToken cancellationToken);
    public Task<OperationResult> UpdateLanguageAsync(
        ShopId shopId, Language language, CancellationToken cancellationToken);
}