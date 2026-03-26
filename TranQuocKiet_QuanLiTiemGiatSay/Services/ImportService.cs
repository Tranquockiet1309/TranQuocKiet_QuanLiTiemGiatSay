using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Import;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Orders;

namespace TranQuocKiet_QuanLiTiemGiatSay.Services
{
    public class ImportService : IImportService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<ImportService> _logger;

        public ImportService(IOrderService orderService, ILogger<ImportService> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<ImportResult> ImportOrdersAsync(BatchImportOrderRequest request, long userId)
        {
            var result = new ImportResult
            {
                Total = request.Orders.Count
            };

            const int batchSize = 20;
            for (int i = 0; i < request.Orders.Count; i += batchSize)
            {
                var batch = request.Orders.Skip(i).Take(batchSize).ToList();
                await ProcessBatchAsync(batch, i, userId, result);
            }

            return result;
        }

        private async Task ProcessBatchAsync(List<CreateOrderRequest> batch, int startIndex, long userId, ImportResult result)
        {
            // Process each item in batch; handle failure per item since IOrderService.CreateAsync uses its own transaction
            for (int j = 0; j < batch.Count; j++)
            {
                int rowNumber = startIndex + j + 1;
                try
                {
                    await _orderService.CreateAsync(batch[j], userId);
                    result.Success++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Row {rowNumber} failed: {ex.Message}");
                    result.Failed++;
                    result.Errors.Add(new ImportError { Row = rowNumber, Reason = ex.Message });
                }
            }
        }
    }
}
