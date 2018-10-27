using System.Collections.Generic;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context) 
        {
            _context = context;
        }

    public void SeedUsers() 
    {
        var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");

        var users = JsonConvert.DeserializeObject<List<User>>(userData);
        users.ForEach(user => 
        {
            byte[] passwordHash, passwordSalt;
        
            SecurityHelper.CreatePasswordHash("password", out passwordHash, out passwordSalt);

            // set the main properties
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UserName = user.UserName.ToLower();

            _context.Users.Add(user);
        });

        // persist to the DB
        _context.SaveChanges();
    }

    }
}