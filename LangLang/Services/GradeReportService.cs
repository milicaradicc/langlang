using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LangLang.Models;
using LangLang.Repositories;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace LangLang.Services
{
    public class GradeReportService : ReportService
    {
        private readonly ITeacherService _teacherService = ServiceProvider.GetRequiredService<ITeacherService>();
        private readonly ICourseRepository _courseRepository = ServiceProvider.GetRequiredService<ICourseRepository>();
        private readonly ICourseGradeRepository _courseGradeRepository = ServiceProvider.GetRequiredService<ICourseGradeRepository>();


        private const string ReportsFolderName = "Reports";
        private const string GradeReportSubfolder = "GradeReports";
        public GradeReportService() { }

        public override void GenerateReport()
        {
            Directory.CreateDirectory(Path.Combine(ReportsFolderName, GradeReportSubfolder));

            List<double> gradesAvg = GetGradesAvg();
            gradesAvg.Add(GetTeacherGradeAvg());

            PlotModel gradeAvgPlotModel = CreateGradePlotModel(gradesAvg);

            string gradeAvgPath = Path.Combine(ReportsFolderName, GradeReportSubfolder, DateTime.Now.ToString("yyyy-MMMM-dd-hh-mm") + ".pdf");
            SaveToPdf(gradeAvgPlotModel, gradeAvgPath);

            EmailService.SendMessage("Grade report", "Today's grade report is attached in this email", gradeAvgPath);
        }

        private double GetTeacherGradeAvg()
        {
            double gradeSum = 0;
            int gradeNums = 0;
            List<Teacher> teachers = _teacherService.GetAll();
            foreach(Teacher teacher in teachers)
            {
                if (double.IsNaN(teacher.Rating)) continue;
                gradeSum += teacher.Rating;
                ++gradeNums;
            }
            return gradeNums > 0 ? gradeSum / gradeNums : 0;
        }

        private List<double> GetGradesAvg()
        {
            int knowledgeGradeSum = 0;
            int activityGradeSum = 0;
            int gradeNums = 0;
            CourseGrade? courseGrade;
            List<Course> courses = _courseRepository.GetAll().Where(course => (DateTime.Now -
                         course.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays <= 365).ToList();
            foreach (Course course in courses)
            {
                if (!course.IsFinished) continue;
                courseGrade = _courseGradeRepository.GetById(course.Id);
                knowledgeGradeSum += courseGrade!.KnowledgeGrade;
                activityGradeSum += courseGrade!.ActivityGrade;
                ++gradeNums;
            }

            return gradeNums > 0
                ? new List<double> { (double)knowledgeGradeSum / gradeNums, (double)activityGradeSum / gradeNums }
                : new List<double> { 0, 0 };
        }

        private static PlotModel CreateGradePlotModel(List<double> data)
        {
            var model = new PlotModel { Title = "Grade report"};
            var series = new HistogramSeries();
            series.Items.Add(new HistogramItem(-0.25, 0.25, Math.Round(data[0]) / 2,1));
            series.Items.Add(new HistogramItem(0.75, 1.25, Math.Round(data[1]) / 2, 1));
            series.Items.Add(new HistogramItem(1.75, 2.25, Math.Round(data[2]) / 2, 1));

            model.Series.Add(series);

            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Grades"
            };
            xAxis.Labels.Add("Knowledge");
            xAxis.Labels.Add("Activity");
            xAxis.Labels.Add("Teacher");
            model.Axes.Add(xAxis);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Average" });

            return model;
        }
    }
}


