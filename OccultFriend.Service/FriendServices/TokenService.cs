using Microsoft.IdentityModel.Tokens;
using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OccultFriend.Service.FriendServices
{
    public class TokenService : ITokenService
    {
        private readonly string _secret;

        public TokenService(string secret)
        {
            _secret = secret;
        }

        public string GenerateToken(Friend friend, ClaimsPrincipal user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);
            var identity = new ClaimsIdentity
                (
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, friend.Name),
                        new Claim(ClaimTypes.Email, friend.Email)
                    },
                    authenticationType: "Password"
                );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            user.AddIdentity(identity);

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
