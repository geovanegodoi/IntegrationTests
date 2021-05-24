using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _defaultRepository;

        public CustomersController(ICustomerRepository defaultRepository)
        {
            _defaultRepository = defaultRepository;
        }

        // GET: api/<CustomersController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_defaultRepository.ListAll());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var model = _defaultRepository.GetById(id);
                return model == null ? NotFound() : Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST api/<CustomersController>
        [HttpPost]
        public IActionResult Post([FromBody] Customer model)
        {
            try
            {
                _defaultRepository.AddOrUpdate(model);
                return Created($"/api/customers/{model.Id}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Customer model)
        {
            try
            {
                _defaultRepository.AddOrUpdate(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _defaultRepository.Remove(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); 
            }
        }

        private IActionResult InternalServerError(Exception ex)
            => StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
    }
}
