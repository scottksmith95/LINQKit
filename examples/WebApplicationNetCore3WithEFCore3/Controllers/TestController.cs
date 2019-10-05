using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplicationNetCore3WithEFCore3.EntityFrameworkCore;
using WebApplicationNetCore3WithEFCore3.EntityFrameworkCore.Entities;

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
            Expression<Func<Guest, bool>> purchasePredicate = g => g.Name.StartsWith("G");
            var query = _context.Guests.AsExpandable().ToList();

            return string.Join(",", query.Select(g => g.Name));
        }
    }
}
