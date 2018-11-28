using System.Threading.Tasks;
using ManageCustomersApi.Interfaces;
using ManageCustomersApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManageCustomersApi.Controllers.v1
{
    /// <summary>
    /// Controller for city's requests to the api
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        public CitiesController(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        [Route("{create}")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CityModel city)
        {
            if (city == null)
            {
                return BadRequest();
            }
            var newCity = await _cityRepository.PostAsync(city);
            if (newCity != null)
            {
                return Ok(newCity);
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _cityRepository.GetAllAsync();
            if (cities != null)
            {
                return Ok(cities);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var city = await _cityRepository.GetAsync(id);
            if (city != null)
            {
                return Ok(city);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]CityModel city)
        {
            if (city == null)
            {
                return BadRequest();
            }

            var newCity = await _cityRepository.PutAsync(city);
            if (newCity != null)
            {
                return Ok(newCity);
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

            bool result = await _cityRepository.DeleteAsync(id);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}