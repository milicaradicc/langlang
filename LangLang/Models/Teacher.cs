using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.FormTable;
using LangLang.Services;
using Newtonsoft.Json;

namespace LangLang.Models
{
    public class Teacher : User
    {
        [JsonProperty]
        [TableItem(13)]
        public int TotalRating { get; set; }
        
        [JsonProperty]
        [TableItem(12)]
        public int NumberOfReviews { get; set; }

        public Teacher(string? firstName, string? lastName, string? email, string? password, Gender gender, string? phone,
            List<Language> qualifications, DateOnly? dateCreated = null) : base(firstName, lastName, email, password, gender, phone)
        {
            Qualifications = qualifications;
            DateCreated = dateCreated == null ? DateOnly.FromDateTime(DateTime.Now) : dateCreated.GetValueOrDefault();
        }

        public Teacher()
        {
        }

        [TableItem(10)]
        public virtual List<Language> Qualifications { get; set; } = new();

        [TableItem(11)]
        public DateOnly DateCreated { get; set; }
        [TableItem(9)]
        public List<int> CourseIds { get; set; } = new();
        [TableItem(8)]
        public List<int> ExamIds { get; set; } = new();
        
        public double Rating => (double)TotalRating / NumberOfReviews;

        public void AddReview(int rating)
        {
            if (rating is < 1 or > 10)
                throw new InvalidInputException("Rating must be between 1 and 10.");
            
            TotalRating += rating;
            NumberOfReviews++;
        }
    }
}