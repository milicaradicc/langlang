using LangLang.Models;

namespace LangLang.ViewModels.StudentViewModels;

public class StudentDropOutRequestViewModel : StudentViewModel
{
    public string Reason { get; set; }
    
    public StudentDropOutRequestViewModel(Student student, string reason) : base(student)
    {
        Reason = reason;
    }
}