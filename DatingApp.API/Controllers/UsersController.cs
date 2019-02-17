using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    // ServiceFilter MUST be the first attribute in the class
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController] 
     public class UsersController : ControllerBase
    {
        private IDatingRepository _repository;
        private IMapper _mapper;

        // IMapper is available for injection via the Startup class as a service
        public UsersController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetUsers() 
        {
            var users = await _repository.GetUsers();
            var usersToReturn = _mapper.Map<System.Collections.Generic.IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id) 
        {
            var user = await _repository.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) 
        {
            // check if the user that is logged in is attempting to update his profile via HttpPut(id)
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();

            // get the user from the repo
            var userFromRepo = await _repository.GetUser(id);

            // update the new user profile coming in (userfromUpdateDto) to the repository user
            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repository.SaveAll()) 
                return NoContent();

            throw new System.Exception($"Updateing user {id} failed on save");
        }
    }

  
}