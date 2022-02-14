using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Homework_third.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MethodController : ControllerBase
{
    [HttpGet("All")]
    public void ForAllUsers()
    {
        Response.WriteAsync("This is for all users");
    }
    
    [HttpGet("Admin"), Authorize(Roles = "Admin, Moderator")]
    public void ForAdminOnly()
    {
        Response.WriteAsync("If you seeing this, this means you are Admin or Moderator");
    }    
}
