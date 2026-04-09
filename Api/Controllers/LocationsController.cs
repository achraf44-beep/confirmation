using Domain.Locations;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LocationsController : ControllerBase
{
    [HttpGet("wilayas")]
    public ActionResult<IReadOnlyList<object>> GetWilayas()
    {
        var wilayas = AlgerianWilayas.All.Select(w => new { w.Code, w.Name, w.NameAr }).ToList();
        return Ok(wilayas);
    }

    [HttpGet("wilayas/{code:int}")]
    public ActionResult<object> GetWilaya(int code)
    {
        var wilaya = AlgerianWilayas.GetByCode(code);
        return wilaya is null ? NotFound() : Ok(new { wilaya.Code, wilaya.Name, wilaya.NameAr });
    }
}
