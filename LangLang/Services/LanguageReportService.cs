using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LangLang.Models;
using LangLang.Repositories;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace LangLang.Services
{
    internal class LanguageReportService: ReportService
    {
        private const string ReportsFolderName = "Reports";
        private const string LanguageReportSubfolder = "LanguageReports";

        private readonly IExamRepository _examRepository = ServiceProvider.GetRequiredService<IExamRepository>();
        private readonly ILanguageRepository _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
        private readonly IExamGradeRepository _examGradeRepository = ServiceProvider.GetRequiredService<IExamGradeRepository>();
        private readonly ICourseRepository _courseRepository = ServiceProvider.GetRequiredService<ICourseRepository>();
        private readonly IPenaltyPointRepository _penaltyPointRepository = ServiceProvider.GetRequiredService<IPenaltyPointRepository>();
        public LanguageReportService(){}

        public override void GenerateReport()
        {
            Directory.CreateDirectory(Path.Combine(ReportsFolderName, LanguageReportSubfolder));

            Dictionary<int, double> courseCount = GetCourseCount();

            PlotModel courseCountPlotModel = CreateLanguagePlotModel("Course count", courseCount);

            string courseCountPath = Path.Combine(ReportsFolderName, LanguageReportSubfolder, "courseCount.pdf");
            SaveToPdf(courseCountPlotModel, courseCountPath);

            Dictionary<int, double> examCount = GetExamCount();

            PlotModel examCountPlotModel = CreateLanguagePlotModel("Exam count", examCount);

            string examCountPath = Path.Combine(ReportsFolderName, LanguageReportSubfolder, "examCount.pdf");
            SaveToPdf(examCountPlotModel, examCountPath);

            Dictionary<int, double> penaltyAvg = GetPenaltyAvg();

            PlotModel penaltyAvgPlotModel = CreateLanguagePlotModel("Penalty point average", penaltyAvg);

            string penaltyAvgPath = Path.Combine(ReportsFolderName, LanguageReportSubfolder, "penaltyAvg.pdf");
            SaveToPdf(penaltyAvgPlotModel, penaltyAvgPath);

            Dictionary<int, double> examGradeAvg = GetExamGradeAvg();

            PlotModel examGradeAvgPlotModel = CreateLanguagePlotModel("Exam Grade average", examGradeAvg);

            string examGradeAvgPath = Path.Combine(ReportsFolderName, LanguageReportSubfolder, "examGradeAvg.pdf");
            SaveToPdf(examGradeAvgPlotModel, examGradeAvgPath);

            string reportPath = Path.Combine(ReportsFolderName, LanguageReportSubfolder,
                DateTime.Now.ToString("yyyy-MMMM-dd-hh-mm") + ".pdf");

            MergePdf(reportPath, new[] { courseCountPath, examCountPath, penaltyAvgPath, examGradeAvgPath });

            EmailService.SendMessage("Language report", "Today's language report is attached in this email",
                reportPath);
        }

        private Dictionary<int, double> GetExamCount()
        {
            // LanguageId, Exam count
            Dictionary<int, double> examCount = new();

            foreach (Exam exam in _examRepository.GetAll())
            {
                if ((DateTime.Now - exam.Date.ToDateTime(TimeOnly.MinValue)).TotalDays > 365) continue;

                if (!examCount.TryAdd(exam.Language.Id, 1))
                    examCount[exam.Language.Id] += 1;
            }

            return examCount;
        }

        private Dictionary<int, double> GetCourseCount()
        {
            // LanguageId, Course count
            Dictionary<int, double> courseCount = new();

            foreach (Course course in _courseRepository.GetAll())
            {
                if ((DateTime.Now - course.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays > 365) continue;

                if (!courseCount.TryAdd(course.Language.Id, 1))
                    courseCount[course.Language.Id] += 1;
            }

            return courseCount;
        }

        private Dictionary<int, double> GetPenaltyAvg()
        {
            Dictionary<int, int> coursePenaltyCount = new();

            foreach (PenaltyPoint penaltyPoint in _penaltyPointRepository.GetAll())
            {
                if ((DateTime.Now - penaltyPoint.Date.ToDateTime(TimeOnly.MinValue)).TotalDays >
                    365) continue;

                if (!coursePenaltyCount.TryAdd(penaltyPoint.CourseId, 1))
                    coursePenaltyCount[penaltyPoint.CourseId] += 1;
            }

            Dictionary<int, int> languagePenaltyCount = new();
            Dictionary<int, int> languageCourseCount = new();

            foreach (int courseId in coursePenaltyCount.Keys)
            {
                Course course = _courseRepository.GetById(courseId)!;

                if (!languagePenaltyCount.TryAdd(course.Language.Id, coursePenaltyCount[courseId]))
                    languagePenaltyCount[course.Language.Id] += coursePenaltyCount[courseId];

                if (!languageCourseCount.TryAdd(course.Language.Id, 1))
                    languageCourseCount[course.Language.Id] += 1;
            }

            Dictionary<int, double> penaltyAvg = new();

            foreach (int languageId in languagePenaltyCount.Keys)
            {
                penaltyAvg[languageId] = (double)languagePenaltyCount[languageId] / languageCourseCount[languageId];
            }

            return penaltyAvg;
        }

        private Dictionary<int, double> GetExamGradeAvg()
        {
            Dictionary<int, int> languageGradeCount = new();
            Dictionary<int, int> languageGradeSum = new();

            foreach (ExamGrade examGrade in _examGradeRepository.GetAll())
            {
                Exam exam = _examRepository.GetById(examGrade.ExamId)!;

                if ((DateTime.Now - exam.Date.ToDateTime(TimeOnly.MinValue)).TotalDays > 365)
                    continue;

                if (!languageGradeCount.TryAdd(exam.Language.Id, 1))
                    languageGradeCount[exam.Language.Id] += 1;

                if (!languageGradeSum.TryAdd(exam.Language.Id, examGrade.PointsSum))
                    languageGradeSum[exam.Language.Id] += examGrade.PointsSum;
            }

            Dictionary<int, double> examGradeAvg = new();

            foreach (int languageId in languageGradeSum.Keys)
            {
                examGradeAvg[languageId] = (double)languageGradeSum[languageId] / languageGradeCount[languageId];
            }

            return examGradeAvg;
        }

        private PlotModel CreateLanguagePlotModel(string title, Dictionary<int, double> data)
        {
            var plotModel = new PlotModel
            {
                Title = title
            };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left };
            categoryAxis.Labels.AddRange(data.Keys.Select(id => _languageRepository.GetById(id)!.ToString()).ToList());

            var valueAxis = new LinearAxis
            { Position = AxisPosition.Bottom, MinimumPadding = 0, MaximumPadding = 0.06, AbsoluteMinimum = 0 };

            var barSeries = new BarSeries { StrokeColor = OxyColors.Black, StrokeThickness = 1 };
            barSeries.ActualItems.AddRange(data.Values.Select(value => new BarItem { Value = value }).ToList());

            plotModel.Series.Add(barSeries);
            plotModel.Axes.Add(categoryAxis);
            plotModel.Axes.Add(valueAxis);

            return plotModel;
        }

        private new static void SaveToPdf(PlotModel plotModel, string filePath)
        {
            using var stream = File.Create(filePath);
            var pdfExporter = new PdfExporter { Width = 600, Height = 400 };
            pdfExporter.Export(plotModel, stream);
        }

        private new static void MergePdf(string outputFilePath, string[] inputFilePaths)
        {
            PdfDocument outputPdfDocument = new();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (string filePath in inputFilePaths)
            {
                PdfDocument inputPdfDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                outputPdfDocument.Version = inputPdfDocument.Version;
                foreach (PdfPage page in inputPdfDocument.Pages)
                {
                    outputPdfDocument.AddPage(page);
                }
            }

            outputPdfDocument.Save(outputFilePath);

            foreach (string filePath in inputFilePaths)
            {
                File.Delete(filePath);
            }
        }
    }
}
