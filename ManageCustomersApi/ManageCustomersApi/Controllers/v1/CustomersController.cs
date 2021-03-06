﻿using System.Threading.Tasks;
using ManageCustomersApi.Interfaces;
using ManageCustomersApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManageCustomersApi.Controllers.v1
{
    /// <summary>
    /// Controller for customer's requests to the api
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [Route("{create}")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CustomerModel customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            var newCustomer = await _customerRepository.PostAsync(customer);
            if (newCustomer != null)
            {
                return Ok(newCustomer);
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();
            if (customers.Count == 0)
                return NotFound();
            else
                return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var customer = await _customerRepository.GetAsync(id);
            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return NotFound();
            }
            
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]CustomerModel customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var newCustomer = await _customerRepository.PutAsync(customer);
            if (newCustomer != null)
            {
                return Ok(newCustomer);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            bool result = await _customerRepository.DeleteAsync(id);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("setStatus")]
        [HttpPut]
        public async Task<IActionResult> SetStatus([FromBody]SetStatusModel model)
        {
            var statusModel = await _customerRepository.SetStatus(model.IdCustomer, model.Status, model.IdUserLocked);
            if (statusModel != null)
                return Ok(statusModel);
            else
                return BadRequest();
        }
        
        [HttpGet("getStatus/{id}")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var status = await _customerRepository.GetStatus(id);
            if (status != null)
                return Ok(status);
            else
                return null;
        }
    }
}