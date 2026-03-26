using Microsoft.EntityFrameworkCore;
using TranQuocKiet_QuanLiTiemGiatSay.Data;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Inventory;
using TranQuocKiet_QuanLiTiemGiatSay.Models;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryTxnResponse>> GetAllAsync()
        {
            return await _context.InventoryTransactions
                .Include(t => t.Creator)
                .AsNoTracking()
                .OrderByDescending(t => t.TxnDate)
                .Select(t => MapToResponse(t))
                .ToListAsync();
        }

        public async Task<InventoryTxnResponse> CreateAsync(CreateInventoryTxnRequest request, long userId)
        {
            var txn = new InventoryTransaction
            {
                ItemName = request.ItemName.Trim(),
                TxnType = request.TxnType.ToUpper().Trim() == "OUT" ? "OUT" : "IN",
                Quantity = request.Quantity,
                Unit = request.Unit.Trim(),
                UnitCost = request.UnitCost,
                ReferenceNote = request.ReferenceNote?.Trim(),
                CreatedBy = userId,
                TxnDate = DateTime.Now
            };

            _context.InventoryTransactions.Add(txn);
            await _context.SaveChangesAsync();

            // Reload to get Creator info if needed, but here we can just map manually since we have userId
            var creator = await _context.Users.FindAsync(userId);
            var response = MapToResponse(txn);
            response.CreatorName = creator?.FullName ?? "Unknown";
            
            return response;
        }

        private static InventoryTxnResponse MapToResponse(InventoryTransaction t)
        {
            return new InventoryTxnResponse
            {
                InventoryTxnId = t.InventoryTxnId,
                TxnDate = t.TxnDate,
                ItemName = t.ItemName,
                TxnType = t.TxnType,
                Quantity = t.Quantity,
                Unit = t.Unit,
                UnitCost = t.UnitCost,
                ReferenceNote = t.ReferenceNote,
                CreatedBy = t.CreatedBy,
                CreatorName = t.Creator?.FullName ?? "Unknown"
            };
        }
    }
}
