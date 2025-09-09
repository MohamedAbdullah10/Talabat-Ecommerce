using LinkDev.Talabat.APIs.Controllers.Controllers.Base;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Auth;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using LinkDev.Talabat.Core.Application.Abstraction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Account
{
    public class AccountController(IServiceManager manager): BaseApiController
    {
        
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var res =await manager.AuthService.LoginAsync(loginDto);
            return Ok(res);

        }
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var res = await manager.AuthService.RegisterAsync(registerDto);
            return Ok(res);
        }

        [Authorize]
        [HttpGet] // GET : /api/Account
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var result = await manager.AuthService.GetCurrentUserAsync(User);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("address")] // GET : /api/Account/address
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var result = await manager.AuthService.GetUserAddressAsync(User);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("address")] // PUT : /api/Account/address
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var result = await manager.AuthService.UpdateUserAddressAsync(User, address);
            return Ok(result);
        }

        [HttpGet("emailexists")] // GET : /api/Account/emailexists?email=ahmed.nasr@linkdev.com
        public async Task<ActionResult<bool>> EmailExists(string email)
        {
            return await manager.AuthService.EmailExists(email);
        }
    }
}
