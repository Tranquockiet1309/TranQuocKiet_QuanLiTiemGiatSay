using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranQuocKiet_QuanLiTiemGiatSay.Constants;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs;
using TranQuocKiet_QuanLiTiemGiatSay.DTOs.Services;
using TranQuocKiet_QuanLiTiemGiatSay.Services;

namespace TranQuocKiet_QuanLiTiemGiatSay.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var services = await _serviceService.GetAllAsync(onlyActive);
            return Ok(ApiResponse<IEnumerable<ServiceResponse>>.SuccessResponse(services, "Lấy danh sách dịch vụ thành công"));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy dịch vụ"));
            }
            return Ok(ApiResponse<ServiceResponse>.SuccessResponse(service, "Lấy thông tin dịch vụ thành công"));
        }

        [HttpPost]
        [Authorize(Roles = Roles.OWNER)]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
        {
            var service = await _serviceService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = service.ServiceId }, 
                ApiResponse<ServiceResponse>.SuccessResponse(service, "Tạo dịch vụ thành công"));
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = Roles.OWNER)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateServiceRequest request)
        {
            var service = await _serviceService.UpdateAsync(id, request);
            if (service == null)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy dịch vụ"));
            }
            return Ok(ApiResponse<ServiceResponse>.SuccessResponse(service, "Cập nhật dịch vụ thành công"));
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = Roles.OWNER)]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _serviceService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy dịch vụ"));
            }
            return Ok(ApiResponse.SuccessResponse("Xóa dịch vụ thành công"));
        }
    }
}
