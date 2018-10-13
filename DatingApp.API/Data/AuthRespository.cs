using System;
using System.Threading.Tasks;
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

            CreatePasswordHash (password, out passwordHash, out passwordSalt);

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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (user == null) return null;

            // compute the hash
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) 
            {
                return null;
            }

            return user;
        }

        #region Private Members

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }        

            return true;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion

    }
}