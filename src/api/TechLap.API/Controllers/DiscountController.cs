using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.DiscountRespones;
using TechLap.API.Exceptions;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories.Discounts;

namespace TechLap.API.Controllers
{
    [Route("/api/discounts")]
    public class DiscountController : BaseController<DiscountController>
    {
        private IDiscountRepository _discountRepository;
        public DiscountController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }
       
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetDiscounts()
        {
            
            var discounts = await _discountRepository.GetAllAsync(d => true);
            var response = LazyMapper.Mapper.Map<IEnumerable<GetAdminDiscountRespones>>(discounts);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
    
        }
        
        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateDiscount(AddAdminDiscountRequest request)
        {
            Discount discount = LazyMapper.Mapper.Map<Discount>(request);
            discount = await _discountRepository.AddAsync(discount);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, "Add discountId " + discount.Id + " successfully");
        }
        
        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null) throw new NotFoundException("Discount not found.");
            await _discountRepository.DeleteAsync(discount);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, "Remove discountId " + discount.Id + " successfully");
        }
        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody]UpdateAdminDiscountRequest request)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            if (discount == null) throw new NotFoundException("Discount not found.");
            
            var updatedDiscount = LazyMapper.Mapper.Map(request, discount);
            

            await _discountRepository.UpdateAsync(updatedDiscount);

            return CreateResponse<string>(true, "Discount updated successfully.", HttpStatusCode.OK, "Updated discountId " + discount.Id);
        }

        [Authorize(Roles = "User")]
        [HttpPost("{discountCode}")]
        public async Task<IActionResult> ApplyDiscount(string discountCode, ApplyUserDiscountRequest request)
        {
            Discount discount = LazyMapper.Mapper.Map<Discount>(request); 
            await _discountRepository.ApplyDiscountAsync(discountCode);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, "Apply discount Code " + discount.DiscountCode + " successfully");

        }
        
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDiscountIdAsync(int id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            var response = LazyMapper.Mapper.Map<Discount>(discount);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }
    }
}
