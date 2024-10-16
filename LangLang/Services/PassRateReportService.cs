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
    public class PassRateReportService : ReportService
    {

        private readonly IExamRepository _examRepository = ServiceProvider.GetRequiredService<IExamRepository>();
        private readonly IExamGradeService _examGradeService = ServiceProvider.GetRequiredService<IExamGradeService>();
        private readonly ICourseService _courseService = ServiceProvider.GetRequiredService<ICourseService>();
        private readonly ICourseGradeService _courseGradeService = ServiceProvider.GetRequiredService<ICourseGradeService>();

        private const string ReportsFolderName = "Reports";
        private const string PassRateReportSubfolder = "PassRateReports";
        public PassRateReportService() {}
        public override void GenerateReport()
        {
            Directory.CreateDirectory(Path.Combine(ReportsFolderName, PassRateReportSubfolder));

            List<double> pointsAvg = AveragePointsInLastYear();
            PlotModel gradeAvgPlotModel = CreatePointsAvgPlotModel(pointsAvg);

            string pointsAvgPath = Path.Combine(ReportsFolderName, PassRateReportSubfolder, "avg" + ".pdf");
            SaveToPdf(gradeAvgPlotModel, pointsAvgPath);

            Dictionary<int, double> attended = GetAttendedCourseAverage();
            PlotModel attendedeAvgPlotModel = CreatePlotModel("Number of students who have attended courses",attended);

            string attendedAvgPath = Path.Combine(ReportsFolderName, PassRateReportSubfolder, "attended" + ".pdf");
            SaveToPdf(attendedeAvgPlotModel, attendedAvgPath);

            Dictionary<int, double> passed = GetPassedCourseAverage();
            PlotModel passedAvgPlotModel = CreatePlotModel("Number of students who have passed courses",passed);

            string passedAvgPath = Path.Combine(ReportsFolderName, PassRateReportSubfolder, "passed" + ".pdf");
            SaveToPdf(passedAvgPlotModel, passedAvgPath);

            Dictionary<int, double> percentage = GetPercentagePassed(attended, passed);
            PlotModel percentageAvgPlotModel = CreatePlotModel("Percentage of students who have passed courses", percentage);

            string percentageAvgPath = Path.Combine(ReportsFolderName, PassRateReportSubfolder, "percentage" + ".pdf");
            SaveToPdf(percentageAvgPlotModel, percentageAvgPath);

            string reportPath = Path.Combine(ReportsFolderName, PassRateReportSubfolder,
                DateTime.Now.ToString("yyyy-MMMM-dd-hh-mm") + ".pdf");

            MergePdf(reportPath, new[] { pointsAvgPath, attendedAvgPath, passedAvgPath, percentageAvgPath });
            EmailService.SendMessage("PassRate report", "Today's PassRate report is attached in this email", reportPath);
        }
        private static PlotModel CreatePointsAvgPlotModel(List<double> data)
        {
            var model = new PlotModel { Title = "Average points on each part of the exam report" };
            var series = new HistogramSeries();
            series.Items.Add(new HistogramItem(-0.25, 0.25, Math.Round(data[0]) / 2, 1));
            series.Items.Add(new HistogramItem(0.75, 1.25, Math.Round(data[1]) / 2, 1));
            series.Items.Add(new HistogramItem(1.75, 2.25, Math.Round(data[2]) / 2, 1));
            series.Items.Add(new HistogramItem(2.75, 3.25, Math.Round(data[3]) / 2, 1));

            model.Series.Add(series);

            var xAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Points"
            };
            xAxis.Labels.Add("ListeningPoints");
            xAxis.Labels.Add("TalkingPoints");
            xAxis.Labels.Add("WritingPoints");
            xAxis.Labels.Add("ReadingPoints");
            model.Axes.Add(xAxis);

            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Average" });

            return model;
        }
        private PlotModel CreatePlotModel(string title, Dictionary<int, double> data)
        {
            var model = new PlotModel { Title = title };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            categoryAxis.Labels.AddRange(data.Keys.Select(id => id.ToString() + " " + _courseService.GetById(id).Language!.ToString() + " " + _courseService.GetById(id).Date!.ToString()).ToList());

            var valueAxis = new LinearAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };

            var barSeries = new BarSeries { StrokeColor = OxyColors.Black, StrokeThickness = 1, FillColor = OxyColors.Red }; // Postavljanje boje na crvenu
            barSeries.ActualItems.AddRange(data.Values.Select(value => new BarItem { Value = value }).ToList());

            model.Series.Add(barSeries);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            return model;
        }

        private Dictionary<int, double> GetAttendedCourseAverage()
        {
            Dictionary<int,double> result = new Dictionary<int,double>();
            foreach(Course course in _courseService.GetAll())
            {
                if (course.IsFinished)
                    result[course.Id] = _courseGradeService.GetByCourseId(course.Id).Count();
            }
            return result;
        }
        private Dictionary<int, double> GetPassedCourseAverage()
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (Course course in _courseService.GetAll())
            {
                if (course.IsFinished)
                {
                    var courseGrades = _courseGradeService.GetByCourseId(course.Id);
                    var passedCount = courseGrades.Count(courseGrade => courseGrade.KnowledgeGrade > 5 && courseGrade.ActivityGrade > 5);
                    result[course.Id] = passedCount;
                }
            }
            return result;
        }

        private Dictionary<int, double> GetPercentagePassed(Dictionary<int, double> listened, Dictionary<int, double> passed)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            foreach (int courseId in listened.Keys)
            {
                if (listened[courseId] != 0)
                    result[courseId] = (passed[courseId] / listened[courseId]) * 100; // Deljenje decimalnim brojem
                else
                    result[courseId] = 0;
            }
            return result;
        }

        /*
      Prosečan broj poena ostvaren na svakom od delova svih ispita u poslednjih
      godinu dana. 
      Koliko je studenata slušalo kurs, a koliko položilo, pored toga
      navesti i procenat studenata koji je položio u odnosu na one koje je slušao
  */
        private List<double> AveragePointsInLastYear()
        {
            List<double> sumOfPoints = new List<double> { 0.0, 0.0, 0.0, 0.0 };
            int gradeCount = 0;

            DateTime oneYearAgo = DateTime.Today.AddYears(-1);

            foreach (Exam exam in _examRepository.GetAll())
            {
                if (exam.Date.ToDateTime(TimeOnly.MinValue) >= oneYearAgo && exam.TeacherGraded)
                {
                    foreach (ExamGrade grade in _examGradeService.GetByExamId(exam.Id))
                    {
                        sumOfPoints[0] += grade.ListeningPoints;
                        sumOfPoints[1] += grade.TalkingPoints;
                        sumOfPoints[2] += grade.WritingPoints;
                        sumOfPoints[3] += grade.ReadingPoints;
                        gradeCount++;
                    }
                }
            }

            if (gradeCount > 0)
            {
                for (int i = 0; i < sumOfPoints.Count; i++)
                {
                    sumOfPoints[i] /= gradeCount;
                }
            }

            return sumOfPoints;
        }
        
    }
}
