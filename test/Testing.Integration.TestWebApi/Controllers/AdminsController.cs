using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Testing.Integration.TestWebApi.Data;

namespace Testing.Integration.TestWebApi.Controllers;

[ApiController]
[Route("api/v1.0/[controller]")]
[Authorize(AppConstants.AdminPolicyName)]
public class AdminsController : ControllerBase
{
    private readonly TestDbContext _context;

    public AdminsController(TestDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminData>>> Get()
    {
        List<AdminData> data = await _context.AdminDatas.ToListAsync();
        return Ok(data);
    }
}