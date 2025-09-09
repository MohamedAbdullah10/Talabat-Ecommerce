using LinkDev.Talabat.APIs.Controllers.Controllers.Base;
using LinkDev.Talabat.APIs.Controllers.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Buggy
{
    public class BuggyController :BaseApiController
    {

        [HttpGet("notfound")] // GET: /api/buggy/notfound
        public IActionResult GetNotFoundError()
        {
            //throw new NotFoundException();
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("servererror")] // GET: /api/buggy/servererror
        public IActionResult GetServerError()
        {
            throw new Exception(); // 500

        }

        [HttpGet("badrequest")] // GET: /api/buggy/badrequest
        public IActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")] // GET: /api/buggy/badrequest/five
        public IActionResult GetValidationError(int id) // => 400
        {
            return Ok();
        }

        //[HttpGet("unauthorized")] // GET: /api/buggy/unauthorized
        //public IActionResult GetUnauthorizedError()
        //{
        //    return Unauthorized(new JsonResult(new ApiResponse(401))); // 401
        //}


        [HttpGet("forbidden")] // GET: /api/buggy/forbidden
        public IActionResult GetForbiddenRequest()
        {
            return Forbid();
        }

        [Authorize]
        [HttpGet("authorized")] // GET: /api/buggy/authorized
        public IActionResult GetAuthorizedError()
        {
            return Ok();
        }
    }
}
