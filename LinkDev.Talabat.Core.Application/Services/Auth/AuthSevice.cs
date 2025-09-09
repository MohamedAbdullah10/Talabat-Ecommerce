using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Auth;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Auth;
using LinkDev.Talabat.Core.Application.Exceptions;
using LinkDev.Talabat.Core.Application.Extensions;
using LinkDev.Talabat.Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Application.Services.Auth
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> manager,IOptions<JwtSettings> jwt,IMapper mapper) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwt.Value;
        public async Task<UserDto> LoginAsync(LoginDto login)
        {
            var res = await userManager.FindByEmailAsync(login.Email);
            if (res == null) throw new UnAuthorizedException("Invalid Email or Password");
            var res2 = await manager.CheckPasswordSignInAsync(res, login.Password, lockoutOnFailure: false);
            if (res2.IsNotAllowed) throw new UnAuthorizedException("Account not confirmed yet.");

            if (res2.IsLockedOut) throw new UnAuthorizedException("Account is locked.");

            //if(result.RequiresTwoFactor) throw new UnAuthorizedException("Requires Two-Factor Authentication.");

            if (!res2.Succeeded) throw new UnAuthorizedException("Invalid Login.");
            var response = new UserDto
            {
                Id = res.Id,
                DisplayName = res.DisplayName,
                Email = res.Email,

                Token =await GenerateTokenAsync(res)




            };
            return response;


        }

        public async Task<UserDto> RegisterAsync(RegisterDto model)
        {
            if (EmailExists(model.Email).Result)
                throw new BadHttpRequestException("This Email is already in use.");

            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) throw new ValidationException() { Errors = result.Errors.Select(E => E.Description) };

            var response = new UserDto()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user),
            };

            return response;

        }
        public async Task<bool> EmailExists(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            var user = await userManager.FindByEmailAsync(email!);

            if (user is null) throw new UnAuthorizedException("");

            var userDto = new UserDto()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user),
            };

            return userDto;
        }

        public async Task<AddressDto> GetUserAddressAsync(ClaimsPrincipal claimsPrincipal)
        {
            var user = await userManager.FindUserWithAddressAsync(claimsPrincipal);

            var addressDto = mapper.Map<AddressDto>(user!.Address);

            return addressDto;
        }

        public async Task<AddressDto> UpdateUserAddressAsync(ClaimsPrincipal claimsPrincipal, AddressDto model)
        {
            var updatedAddress = mapper.Map<Address>(model);

            var user = await userManager.FindUserWithAddressAsync(claimsPrincipal);

            if (user?.Address is not null)
            {
                updatedAddress.Id = user.Address.Id;
            }


            user!.Address = updatedAddress;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new BadRequestException(result.Errors.Select(E => E.Description).Aggregate((x, y) => $"{x}, {y}"));

            return mapper.Map<AddressDto>(updatedAddress);
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user1) {


            var userClaims = await userManager.GetClaimsAsync(user1);
            var rolesAsClaims = new List<Claim>();

            var roles = await userManager.GetRolesAsync(user1);
            foreach (var role in roles)
                rolesAsClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.PrimarySid, user1.Id),
                new Claim(ClaimTypes.Email, user1.Email!),
                new Claim(ClaimTypes.GivenName, user1.DisplayName),
            }
            .Union(userClaims)
            .Union(rolesAsClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var tokenObj = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenObj);







        }
    }
}
