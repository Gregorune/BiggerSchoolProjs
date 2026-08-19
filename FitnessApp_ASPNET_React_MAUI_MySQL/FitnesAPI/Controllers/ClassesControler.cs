using FitnesAPI.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnesAPI.Controllers;

[ApiController]
[Route("api/v1/classes")]
public class ClassesControler : ControllerBase
{
    private readonly DatabaseContext _dbCtx;
    private readonly JwtHandler _jwtHandler;

    public ClassesControler(DatabaseContext dbCtx, JwtHandler jwtHandler)
    {
        _dbCtx = dbCtx;
        _jwtHandler = jwtHandler;
    }

    #region GET CLASSES
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetClasses()
    {
        var authData = _jwtHandler.GetJwtContent(User);
        User? user = await _dbCtx.Users.FindAsync(authData.Uid);
        
        if (user == null)
            return Unauthorized();
        
        var classes = await _dbCtx.Classes
            .GroupJoin(
               _dbCtx.Signups,
               c => c.Id,
               s => s.ClassId,
               (c, s) => new { Class = c, Signups = s })
            .Select(
                cs => new ClassesResDto(
                    Id: cs.Class.Id,
                    Name: cs.Class.Name ?? "",
                    Description: cs.Class.Description ?? "",
                    Instructor: cs.Class.Instructor ?? "",
                    StartsAt: cs.Class.StartsAt,
                    MaxPeople: cs.Class.PeopleLimit,
                    SignedPeople: cs.Signups.Count(),
                    YouSignedUp: cs.Signups.Any(s => s.UserId == user.Id),
                    Repetition: cs.Class.Repetition.ToString()
                )
            )
            .ToListAsync();
        
        return Ok(classes);
    }

    public record ClassesResDto(
        int Id,
        string Name, 
        string Description, 
        string Instructor, 
        DateTime StartsAt, 
        int? MaxPeople,
        int SignedPeople,
        bool YouSignedUp,
        string Repetition
    );
    #endregion

    [Authorize]
    [HttpPost("{classId:int}/signup")]
    public async Task<IActionResult> SignUpForClass([FromRoute] int classId)
    {
        var tokenContent = _jwtHandler.GetJwtContent(User);

        var classes = await _dbCtx.Classes
            .GroupJoin(
                _dbCtx.Signups,
                c => c.Id,
                s => s.ClassId,
                (c, s) => new { Class = c, Signups = s })
            .Where(cs => cs.Class.Id == classId)
            .Select(cs => new {
                PeopleLimit = cs.Class.PeopleLimit,
                PeopleSigned = cs.Signups.Count()
            })
            .ToListAsync();
        var classInfo = classes.FirstOrDefault();
        if (classInfo?.PeopleLimit != null)
        {
            if (classInfo.PeopleLimit < classInfo.PeopleSigned + 1)
                return StatusCode(406);
        }
        

        Signup signup = new Signup()
        {
            UserId = tokenContent.Uid,
            ClassId = classId
        };
        _dbCtx.Signups.Add(signup);
        await _dbCtx.SaveChangesAsync();
        return Ok();
    }

    [Authorize]
    [HttpPost("{classId:int}/leave")]
    public async Task<IActionResult> LeaveClass([FromRoute] int classId)
    {
        var tokenContent = _jwtHandler.GetJwtContent(User);

        var rowsAffected = await _dbCtx.Signups
            .Where(s => s.ClassId == classId && s.UserId == tokenContent.Uid)
            .ExecuteDeleteAsync<Signup>();
            
        if(rowsAffected == 0)
            return NotFound();
        return Ok();
    }
}
