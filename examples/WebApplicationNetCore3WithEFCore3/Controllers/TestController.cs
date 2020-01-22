using System;
using System.Linq;
using System.Linq.Expressions;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplicationNetCore3WithEFCore3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly MyHotelDbContext _context;

        public TestController(ILogger<TestController> logger, MyHotelDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public string Get()
        {
            Expression<Func<Guest, bool>> namePredicate = g => g.Name.StartsWith("G");
            var query = _context.Guests.AsExpandable().ToList();

            return string.Join(",", query.Select(g => g.Name));
        }
    }
}
