using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangLang.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LangLang.Repositories.PostgresRepositories
{
    internal class SchedulePostgresRepository:IScheduleRepository
    {
        private readonly DatabaseContext _databaseContext;

        public SchedulePostgresRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public List<ScheduleItem> GetByDate(DateOnly date)
        {
            Schedule? schedule = _databaseContext.Schedules.Find(date);
            if (schedule == null)
                return new List<ScheduleItem>();
            return schedule.ScheduleItems;
        }

        public void Add(ScheduleItem item)
        {
            Schedule? schedule = _databaseContext.Schedules.Find(item.Date);
            if (schedule == null)
            {
                schedule = new Schedule();
                schedule.Date = item.Date;
                schedule.ScheduleItems = new List<ScheduleItem> { item };
                _databaseContext.Schedules.Add(schedule);
            }
            else
            {
                schedule.ScheduleItems.Add(item);
                _databaseContext.Update(schedule);
            }
            _databaseContext.SaveChanges();
        }

        public void Delete(int id)
        {
            List<Schedule> schedules = _databaseContext.Schedules.ToList();
            bool updated = false;
            foreach(Schedule schedule in schedules)
            {
                if (schedule.ScheduleItems.RemoveAll(scheduleItem => scheduleItem.Id == id) != 0)
                {
                    _databaseContext.Schedules.Update(schedule);
                    updated= true;
                }
            }
            if(updated)
                _databaseContext.SaveChanges();
        }
    }
}
