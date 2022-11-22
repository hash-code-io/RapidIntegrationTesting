using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Testing.Integration.TestWebApi.Data;

namespace Testing.Integration.TestWebApi.Controllers;

[ApiController]
[Route("api/v1.0/[controller]")]
public class UsersController : ControllerBase
{
    private readonly TestDbContext _context;

    public UsersController(TestDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserData>>> Get()
    {
        List<UserData> data = await _context.UserDatas.ToListAsync();
        return Ok(data);
    }
}