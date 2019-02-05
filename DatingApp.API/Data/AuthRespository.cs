using System;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRespository : IAuthRepository
    {
        private DataContext _context;

        #region ctor
        public AuthRespository(DataContext context)
        {
            _context = context;
        }
        #endregion

        async Task<User> IAuthRepository.Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;

            SecurityHelper.CreatePasswordHash (password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        async Task<bool> IAuthRepository.Exists(string userName)
        {
            if (await _context.Users.AnyAsync(x => x.UserName == userName)) 
            {
                return true;
            }

            return false;
        }

        async Task<User> IAuthRepository.Login(string userName, string password)
        {
            // return the user infomation including the photos for that user
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null) return null;

            // compute the hash
            if (!SecurityHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) 
            {
                return null;
            }

            return user;
        }

        #region Private Members

        #endregion

    }
}