﻿using Common;
using Data.Repositories;
using Entities.Models.Roles;
using Entities.Models.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Services.Interfaces.Services
{
    public class JwtService : IJwtService
    {
        // using JwtSettings configuration  that we wrote  in Program.cs
        private readonly JwtSettings _settings;
        private readonly IRepository<UserRoles> _repo;
        public JwtService(IOptionsSnapshot<JwtSettings> settings, IRepository<UserRoles> repo)
        {
            _settings = settings.Value;

            _repo = repo;
        }
        public async Task<AccessToken> Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_settings.SecretKey); // it should longer than 16 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            // for encryption and security
            var encryptionkey = Encoding.UTF8.GetBytes(_settings.Encryptkey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            //var certificate = new X509Certificate2("d:\\aaaa2.cer"/*, "P@ssw0rd"*/);
            //var encryptingCredentials = new X509EncryptingCredentials(certificate);

            var claims = getClaims(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_settings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_settings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            //string encryptedJwt = tokenHandler.WriteToken(securityToken);

            return new AccessToken(securityToken);
        }

        private IEnumerable<Claim> getClaims(User user)
        {
            //  JwtRegisteredClaimNames.Sub //

            // generate security stamp
            var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
             
            var list = new List<Claim>()
        {
            new Claim(ClaimTypes.Name , user.FullName),
            new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
            new Claim(ClaimTypes.Email , user.Email.ToString()),
            new Claim(securityStampClaimType, user.SecurityStamp.ToString())
        };
            var roles = _repo.TableNoTracking
                .Where(u => u.UserId == user.Id);

            foreach (var role in roles)
            {
                list.Add(new Claim(ClaimTypes.Role, role.RoleId.ToString()));
            }
            return list;
        }
    }
}
