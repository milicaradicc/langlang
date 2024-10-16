using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Models
{
    public class CourseGrade
    {
        public const int KnowledgeGradeMax = 10;
        public const int ActivityGradeMax = 10;

        private int _knowledgeGrade;
        private int _activityGrade;

        public CourseGrade(int courseId, int studentId, int knowledgeGrade, int activityGrade)
        {
            CourseId = courseId;
            StudentId = studentId;
            KnowledgeGrade = knowledgeGrade;
            ActivityGrade = activityGrade;

        }

        public int Id { get; set; }

        public int CourseId { get; set; }

        public int StudentId { get; set; }

        public int KnowledgeGrade
        {
            get => _knowledgeGrade;
            set
            {
                ValidateKnowledgeGrade(value);
                _knowledgeGrade = value;
            }
        }

        public int ActivityGrade
        {
            get => _activityGrade;
            set
            {
                ValidateActivityGrade(value);
                _activityGrade = value;
            }
        }

        private static void ValidateKnowledgeGrade(int knowledgeGrade)
        {
            if (knowledgeGrade < 0 || knowledgeGrade > KnowledgeGradeMax)
            {
                throw new InvalidInputException("Knowledge grade is not valid");
            }
        }

        private static void ValidateActivityGrade(int activityGrade)
        {
            if (activityGrade < 0 || activityGrade > ActivityGradeMax)
            {
                throw new InvalidInputException("Activity grade is not valid");
            }
        }
    }
}


