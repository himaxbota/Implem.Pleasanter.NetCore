﻿using Implem.Pleasanter.NetFramework.Filters;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [CheckApiContextAttributes]
    [AllowAnonymous]
    public class BinariesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get(string guid)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.BinariesController();
            var result = controller.Get(
                context: context,
                guid: guid);
            return result.ToHttpResponse(Request);
        }
    }
}