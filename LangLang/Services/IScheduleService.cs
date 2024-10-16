using LangLang.Models;

namespace LangLang.Services;

public interface IScheduleService
{
    public void Add(ScheduleItem scheduleItem);
    public void Update(ScheduleItem scheduleItem);
    public void Delete(int id);
    public bool ValidateScheduleItem(ScheduleItem scheduleItem, bool toEdit = false);
}