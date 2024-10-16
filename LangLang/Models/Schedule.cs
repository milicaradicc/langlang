using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.Models
{
    public class Schedule
    {
        [Key]
        public DateOnly Date { get; set; }
        public List<ScheduleItem> ScheduleItems { get; set; }
        public Schedule() { }
    }
}
