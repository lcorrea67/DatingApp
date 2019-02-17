using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthRepository _repository;
        IConfiguration _config;
        IMapper _mapper;
        public AuthController(IAuthRepository repository, IConfiguration config, IMapper mapper)
        {
            _repository = repository;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userForRegisterDto ) 
        {
            // validate request

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repository.Exists(userForRegisterDto.Username)) 
            {
                return BadRequest("Username already exists");
            }

            // fill the 
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var createdUser = await _repository.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            // send back location header with the data
            // GetUser is the name of the route in the UsersController GetUser, route name is in the HttpGetAttribute
            // 2nd parameter is an anonymous object 
            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn);
        }

        [HttpPost("login")]
         public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) 
        {
             // do we have a user found in the db
            var userFromRepository = await _repository.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepository == null) return Unauthorized();

            // TOKEN WILL CONTAINS TWO CLAIMS
            // this token is passed on every API request, so we need to keep it small
            // do not add object that do not need to be there
            var claims = new [] 
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepository.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepository.UserName)
            };

            // server needs to sign the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // security token description, contains claims, signing credtenials and token
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepository);

            // pass the token to the api consumer
            // we could also add additional data here, not part of the token 
            // but as a response from the login method
            return Ok(new 
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

    }
}