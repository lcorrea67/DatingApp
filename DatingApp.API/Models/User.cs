using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class User
    {
        public User() {}
        public User(int id, string userName, string gender, DateTime dateOfBirth, string knownAs, DateTime created, DateTime lastActive, string introduction, string lookingFor, string interests, string city, string country)
        {
            this.Id = id;
            this.UserName = userName;
            this.Gender = gender;
            this.DateOfBirth = dateOfBirth;
            this.KnownAs = knownAs;
            this.Created = created;
            this.LastActive = lastActive;
            this.Introduction = introduction;
            this.LookingFor = lookingFor;
            this.Interests = interests;
            this.City = city;
            this.Country = country;

        }
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public ICollection<Like> Likers { get; set; }

        public ICollection<Like> Likees { get; set; }

        public ICollection<Message> MessagesSent { get; set; }

        public ICollection<Message> MessagesReceived { get; set; }
    }
}