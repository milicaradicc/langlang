using System;
using System.Linq;
using System.Windows.Documents;
using LangLang.Models;
using LangLang.Repositories;
using LangLang.Repositories.FileRepositories;
using LangLang.Repositories.PostgresRepositories;
using LangLang.Services;
using LangLang.ViewModels.CourseViewModels;
using LangLang.ViewModels.DirectorViewModels;
using LangLang.ViewModels.ExamViewModels;
using LangLang.ViewModels.StudentViewModels;
using LangLang.ViewModels.TeacherViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider = LangLang.Models.ServiceProvider;

namespace LangLang.FormTable
{
    public class MainMenu
    {
        static User? user;
        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            var mainMenu = new MainMenu();
            mainMenu.ConfigureServices(serviceCollection);

            ServiceProvider.Instance = serviceCollection.BuildServiceProvider();

            Director director = new Director("Nadja", "Zoric", "nadjazoric@gmail.com", "PatrikZvezdasti011", Gender.Female, "1234567890123");

            IUserRepository userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
            if (userRepository.GetAll().All(user => user.Email != director.Email))
                userRepository.Add(director);

            //var serviceProvider = serviceCollection.BuildServiceProvider();

            IUserService userService = ServiceProvider.Instance.GetRequiredService<IUserService>();
            ITeacherService teacherService = ServiceProvider.Instance.GetRequiredService<ITeacherService>();
            IExamService examService = ServiceProvider.Instance.GetRequiredService<IExamService>();
            ICourseService courseService = ServiceProvider.Instance.GetRequiredService<ICourseService>();

            user = Loggin(userService)!;

            while (true)
            {
                switch (user)
                {
                    case Teacher:
                        TeacherMenu(examService, courseService, userService);
                        break;
                    case Director:
                        DirectorMenu(teacherService, userService, courseService, examService);
                        break;
                    default:
                        break;
                }
            }

        }
        private static User? Loggin(IUserService userService)
        {
            while (true)
            {
                Console.WriteLine("Email >> ");
                string email = Console.ReadLine()!;
                Console.WriteLine("Password >> ");
                string password = Console.ReadLine()!;
                User? user = userService.Login(email!, password!);
                switch (user)
                {
                    case null:
                        Console.WriteLine("Error logging in. Try again? y/n");
                        string option = Console.ReadLine()!;
                        if (option == "n") return null;
                        continue;
                    case Teacher:
                        return user;
                    case Director:
                        return user;
                    default:
                        break;
                }
            }
        }
        private static void TeacherMenu(IExamService examService, ICourseService courseService, IUserService userService)
        {
            try
            {
                Console.Write("" +
                  "1) Create exams\n" +
                  "2) Read exams\n" +
                  "3) Update exams\n" +
                  "4) Delete exams\n" +
                  "5) Create courses\n" +
                  "6) Read courses\n" +
                  "7) Update courses\n" +
                  "8) Delete courses\n" +
                  "9) Logout\n" +
                  "Enter option >> ");
                string option = Console.ReadLine()!;
                switch (option)
                {
                    case "1":
                        new FormTableGenerator<Exam>(examService.GetAll(), examService).Create(user!);
                        break;
                    case "2":
                        var da = examService.GetAll();
                        new FormTableGenerator<Exam>(examService.GetAll(), examService).ShowTable();
                        break;
                    case "3":
                        int id;
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        Exam item = new FormTableGenerator<Exam>(examService.GetAll(), examService).GetById(id);
                        new FormTableGenerator<Exam>(examService.GetAll(), examService).Update(item);
                        break;
                    case "4":
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        new FormTableGenerator<Exam>(examService.GetAll(), examService).Delete(id);
                        break;
                    case "5":
                        new FormTableGenerator<Course>(courseService.GetAll(), courseService).Create(user!);
                        break;
                    case "6":
                        new FormTableGenerator<Course>(courseService.GetAll(), courseService).ShowTable();
                        break;
                    case "7":
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        Course course = new FormTableGenerator<Course>(courseService.GetAll(), courseService).GetById(id);
                        new FormTableGenerator<Course>(courseService.GetAll(), courseService).Update(course);
                        break;
                    case "8":
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        new FormTableGenerator<Course>(courseService.GetAll(), courseService).Delete(id);
                        break;
                    case "9":
                        userService.Logout();
                        Main();
                        break;
                    default: break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid entries. Try again.");
            }
        }
        private static void DirectorMenu(ITeacherService teacherService, IUserService userService, ICourseService courseService, IExamService examService)
        {
            try
            {
                Console.Write("" +
                "1) Create teachers\n" +
                "2) Read teachers\n" +
                "3) Update teachers\n" +
                "4) Delete teachers\n" +
                "5) Create exams - smart pick\n" +
                "6) Create courses - smart pick\n" +
                "7) Log out" +
                "Enter option >> ");
                string option = Console.ReadLine()!;
                switch (option)
                {
                    case "1":
                        new FormTableGenerator<Teacher>(teacherService.GetAll(), userService).Create(user!);
                        break;
                    case "2":
                        new FormTableGenerator<Teacher>(teacherService.GetAll(), teacherService).ShowTable();
                        break;
                    case "3":
                        int id;
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        Teacher teacher = new FormTableGenerator<Teacher>(teacherService.GetAll(), userService).GetById(id);
                        new FormTableGenerator<Teacher>(teacherService.GetAll(), userService).Update(teacher);
                        break;
                    case "4":
                        while (true)
                        {
                            Console.Write("Please enter an ID: ");
                            string input = Console.ReadLine()!;

                            if (int.TryParse(input, out id))
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }
                        }
                        new FormTableGenerator<User>(userService.GetAll(), userService).Delete(id);
                        break;
                    case "5":
                        object exam = new FormTableGenerator<Exam>(examService.GetAll(), examService).Create(user!);
                        new FormTableGenerator<Teacher>(teacherService.GetAll(), teacherService).SmartPick(exam);
                        break;
                    case "6":
                        object item = new FormTableGenerator<Course>(courseService.GetAll(), courseService).Create(user!);
                        new FormTableGenerator<Teacher>(teacherService.GetAll(), teacherService).SmartPick(item);
                        break;
                    case "7":
                        userService.Logout();
                        Main();
                        break;
                    default: break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid entries. Try again.");
            }
        }
        private void ConfigureServices(IServiceCollection services)
        {
            ConfigureFileRepositories(services);
            ConfigureDatabaseRepositories(services);

            services.AddScoped<ICourseGradeService, CourseGradeService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IDirectorService, DirectorService>();
            services.AddScoped<IExamGradeService, ExamGradeService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IPenaltyPointService, PenaltyPointService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IUserService, UserService>();

            services.AddTransient<MainWindow>();
            services.AddTransient<ActiveCoursesViewModel>();
            services.AddTransient<AddCourseViewModel>();
            services.AddTransient<CourseListingViewModel>();
            services.AddTransient<CoursesWithStudentWithdrawalsViewModel>();
            services.AddTransient<StartableCoursesViewModel>();
            services.AddTransient<GradedExamsViewModel>();
            services.AddTransient<ExamListingViewModel>();
            services.AddTransient<AppliedExamListingViewModel>();
            services.AddTransient<StudentExamViewModel>();
            services.AddTransient<StartableExamsViewModel>();
            services.AddTransient<BestStudentsNotificationViewModel>();
            services.AddTransient<CourseListingDirectorViewModel>();
        }

        private static void ConfigureFileRepositories(IServiceCollection services)
        {
            services.AddScoped<ICourseGradeRepository, CourseGradeFileRepository>();
            services.AddScoped<IExamGradeRepository, ExamGradeFileRepository>();
            services.AddScoped<IMessageRepository, MessageFileRepository>();
            services.AddScoped<IPenaltyPointRepository, PenaltyPointFileRepository>();
        }

        private void ConfigureDatabaseRepositories(IServiceCollection services)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            DotNetEnv.Env.Load("../.env"); // Works if the current directory is the LangLang project

            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidInputException("Connection string not found in .env file.");
            services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<ICourseRepository, CoursePostgresRepository>();
            services.AddScoped<ILanguageRepository, LanguagePostgresRepository>();
            services.AddScoped<IUserRepository, UserPostgresRepository>();
            services.AddScoped<IExamRepository, ExamPostgresRepository>();
            services.AddScoped<IScheduleRepository, SchedulePostgresRepository>();
        }
    }
}
