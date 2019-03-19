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

        //  [FromQuery] allows the parameters to be sent in the query string
        [HttpGet()]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams) 
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repository.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            // if the logged in user is male then only return women
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            // perform the search
            var users = await _repository.GetUsers(userParams);
            var usersToReturn = _mapper.Map<System.Collections.Generic.IEnumerable<UserForListDto>>(users);

     

            // add pagination to the response header to the client
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

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

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser (int id, int recipientId)
        {
            // check authorization
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var like = await _repository.GetLike(id, recipientId);
            if (like != null) 
            {
                return BadRequest("You already liked this user");
            }

            if (await _repository.GetUser(recipientId) == null)
                return NotFound();

            // create model and save it to the repository
            like = new Models.Like 
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repository.Add(like);

            if (await _repository.SaveAll()) 
            {
                return Ok();
            }

            return BadRequest("Failed to like user");
        }
    }

  
}