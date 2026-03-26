using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Customers;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/customers")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(customers, "Lấy danh sách khách hàng thành công"));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy khách hàng"));
            }
            return Ok(ApiResponse<CustomerResponse>.SuccessResponse(customer, "Lấy thông tin khách hàng thành công"));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var customer = await _customerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, 
                ApiResponse<CustomerResponse>.SuccessResponse(customer, "Tạo khách hàng thành công"));
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateCustomerRequest request)
        {
            var customer = await _customerService.UpdateAsync(id, request);
            if (customer == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy khách hàng"));
            }
            return Ok(ApiResponse<CustomerResponse>.SuccessResponse(customer, "Cập nhật khách hàng thành công"));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _customerService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy khách hàng"));
            }
            return Ok(ApiResponse.SuccessResponse("Xóa khách hàng thành công"));
        }
    }
}
