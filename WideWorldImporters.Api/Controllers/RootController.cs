using System.Collections.Generic;
using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WideWorldImportersWebApi.Controllers
{
    [Route("api/{v:apiversion}")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator) { _linkGenerator = linkGenerator; }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType.Contains("application/vnd.gmcbath.apiroot"))
            {
                var list = new List<Link>
                           {
                               new Link
                               {
                                   Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new { }),
                                   Rel = "self",
                                   Method = "GET"
                               },
                               new Link
                               {
                                   Href = _linkGenerator.GetUriByName(HttpContext, "GetSuppliers", new { }),
                                   Rel = "suppliers",
                                   Method = "GET"
                               }
                           };

                return Ok(list);
            }

            return NoContent();
        }
    }
}