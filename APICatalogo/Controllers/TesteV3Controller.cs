using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("api/teste")]
    [ApiController]
    [ApiVersion(3)]
    [ApiVersion(4)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TesteV3Controller : ControllerBase
    {
        [MapToApiVersion(3)]
        [HttpGet]
        public string GetVersion3()
        {
            return "TesteV3 - GET - Api V3.0";
        }

        [MapToApiVersion(4)]
        [HttpGet]
        public string GetVersion4()
        {
            return "TesteV3 - GET - Api 4.0";
        }
    }
}
