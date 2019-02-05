using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloundinaryConfig) {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryConfig = cloundinaryConfig;

            Account acc = new Account (_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) 
        {
            var photoFromRepo = await _repository.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser (int userId, [FromForm]Dtos.PhotoForCreationDto photoForCreationDto) 
        {
            // check if the user that is logged in is attempting to update his profile via HttpPut(id)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repository.GetUser(userId);

            var file = photoForCreationDto.File;
            // get the user from the repo
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) 
            {
                using (var stream = file.OpenReadStream()) 
                {
                    var uploadParams = new ImageUploadParams() 
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            // perform the mapping (shallow copy) to the Photo object
            var photo = _mapper.Map<Photo>(photoForCreationDto);
            if (!userFromRepo.Photos.Any(u => u.IsMain)) 
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            // persist to the DB
            if (await _repository.SaveAll()) 
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

                // return a route specified by the HttpGet GetPhoto method
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id) 
        {
            // check if the user that is logged in is attempting to update his profile via HttpPut(id)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // check that the user is updating their own photo
            var user = await _repository.GetUser(userId);
            
            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(id);
            if (photoFromRepo.IsMain) 
                return BadRequest("This is already the main photo");

            // get the current main photo from the repo and set it to false
            var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
            if (currentMainPhoto != null)
                currentMainPhoto.IsMain = false;

            // set the new photo to be the main photo
            photoFromRepo.IsMain = true;

            if (await _repository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }
        private int PhotoForReturnDto(Photo photo)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            // check if the user that is logged in is attempting to update his profile via HttpPut(id)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // check that the user is updating their own photo
            var user = await _repository.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete the main photo");

            if (photoFromRepo.PublicId != null) 
            {
                // now delete from the database as well as Cloudinary
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deletionParams);
                if (result.Result == "ok")
                {
                    _repository.Delete(photoFromRepo);
                }
            }
            else 
            {
                _repository.Delete(photoFromRepo);
            }

            if (await _repository.SaveAll()) 
                return Ok();

            return BadRequest("Failed to delete the photo");
        }        
    }
}