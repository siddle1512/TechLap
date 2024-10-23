using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.CustomerDTOs;
using TechLap.API.Exceptions;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    [Route("api/customers")]
    public class CustomerController : BaseController<CustomerController>
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerRepository.GetAllAsync(c => true);

            var response = LazyMapper.Mapper.Map<IEnumerable<CustomerResponse>>(customers);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new NotFoundException("Customer not found.");
            }

            var response = LazyMapper.Mapper.Map<CustomerResponse>(customer);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerRequest request)
        {
            var customer = LazyMapper.Mapper.Map<Customer>(request);
            var newCustomer = await _customerRepository.AddAsync(customer);

            return CreateResponse<string>(true, "Customer created successfully.", HttpStatusCode.OK, "CustomerId " + newCustomer.Id + " created.");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new NotFoundException("Customer not found.");

            var updatedCustomer = LazyMapper.Mapper.Map(request, customer);
            await _customerRepository.UpdateAsync(updatedCustomer);

            return CreateResponse<string>(true, "Customer updated successfully.", HttpStatusCode.OK, "CustomerId " + customer.Id + " updated.");
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new NotFoundException("Customer not found.");

            await _customerRepository.DeleteAsync(customer);

            return CreateResponse<string>(true, "Customer deleted successfully.", HttpStatusCode.OK);
        }
    }
}
