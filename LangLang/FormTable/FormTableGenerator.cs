using LangLang.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LangLang.FormTable
{
    public class FormTableGenerator<T>
    {
        private readonly Type _type;
        private readonly object _service;
        private readonly IEnumerable<T> _data;
        private readonly int _columnsPerPage;

        public FormTableGenerator(IEnumerable<T> data, object service, int columnsPerPage = 5)
        {
            _data = data;
            _type = typeof(T);
            _service = service;
            _columnsPerPage = columnsPerPage;
        }
        public T GetById(object id)
        {
            var serviceType = _service.GetType();
            var getByIdMethod = serviceType.GetMethod("GetById", new[] { id.GetType() });

            if (getByIdMethod == null)
            {
                Console.WriteLine($"Method 'GetById' with compatible parameter type not found on the service.");
                return default!;
            }

            try
            {
                var result = getByIdMethod.Invoke(_service, new[] { id });
                return (T)result!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting item by ID: {ex.Message}");
                return default!;
            }
        }
        private List<object> GetData(User user)
        {
            var method = _service.GetType().GetMethod("Add");
            if (method == null)
            {
                Console.WriteLine("Method 'Add' not found on the service.");
                return new List<object>();
            }

            return FormTableGenerator<T>.GetMethodArguments(method, user);
        }

        private static List<object> GetMethodArguments(MethodInfo method, User user)
        {
            var arguments = new List<object>();
            var parameters = method.GetParameters();

            foreach (var param in parameters)
            {
                if (param.Name == "teacherId" || param.Name == "creatorId")
                {
                    arguments.Add(user.Id);
                }
                else if (param.ParameterType.IsGenericType && param.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    arguments.Add(null!);
                }
                else
                {
                    arguments.Add(FormTableGenerator<T>.GetArgumentFromUserInput(param));
                }
            }

            return arguments;
        }

        private static object GetArgumentFromUserInput(ParameterInfo param)
        {
            Console.WriteLine($"Enter {param.Name} ({param.ParameterType.Name}): ");

            if (param.ParameterType.IsEnum)
            {
                FormTableGenerator<T>.PrintEnumValues(param.ParameterType);
            }

            string input = Console.ReadLine()!;
            return Memory.GetValueFromInput(input, param.ParameterType);
        }

        private static void PrintEnumValues(Type enumType)
        {
            foreach (var value in Enum.GetValues(enumType))
            {
                Console.WriteLine($">>{value}");
            }
        }

        public T Create(User user)
        {
            var method = _service.GetType().GetMethod("Add");
            if (method == null)
            {
                Console.WriteLine("Method 'Add' not found on the service.");
                return default!;
            }

            List<object> arguments = GetData(user);
            return InvokeServiceMethod<T>(method, arguments);
        }

        private TResult InvokeServiceMethod<TResult>(MethodInfo method, List<object> arguments)
        {
            try
            {
                object result = method.Invoke(_service, arguments.ToArray())!;
                Console.WriteLine("Item successfully added.");
                return (TResult)result!;
            }
            catch (Exception)
            {
                Console.WriteLine("Error! Item not created");
                return default!;
            }
        }

        private Dictionary<string, object> Prompt(T item, MethodInfo updateMethod)
        {
            var values = new Dictionary<string, object>();

            foreach (var parameter in updateMethod.GetParameters())
            {
                var property = _type.GetProperty(parameter.Name!, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var currentValue = property != null ? property.GetValue(item) : FormTableGenerator<T>.GetDefaultValue(parameter.ParameterType);

                if (FormTableGenerator<T>.ShouldPromptForInput(parameter))
                {
                    values[parameter.Name!] = PromptForParameterValue(parameter, currentValue!);
                }
                else
                {
                    values[parameter.Name!] = currentValue!;
                }
            }

            return values;
        }


        private static object? GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private static bool ShouldPromptForInput(ParameterInfo parameter)
        {
            return (parameter.ParameterType.IsPrimitive
                    || typeof(IEnumerable).IsAssignableFrom(parameter.ParameterType)
                    || parameter.ParameterType.IsEnum
                    || parameter.ParameterType == typeof(DateOnly)
                    || parameter.ParameterType == typeof(TimeOnly))
                   && !parameter.Name!.Equals("id", StringComparison.OrdinalIgnoreCase);
        }

        private static object PromptForParameterValue(ParameterInfo parameter, object currentValue)
        {
            string formattedValue = currentValue?.ToString() ?? string.Empty;
            Console.WriteLine($"{parameter.Name} ({parameter.ParameterType.Name}) [Current value: {formattedValue}]: ");

            string input = Console.ReadLine()!;

            if (!string.IsNullOrWhiteSpace(input))
            {
                return Memory.GetValueFromInput(input, parameter.ParameterType);
            }

            return currentValue!;
        }

        public void Update(T item)
        {
            var serviceType = _service.GetType();
            var updateMethod = serviceType.GetMethod("Update");
            if (updateMethod == null)
            {
                Console.WriteLine("Method 'Update' not found on the service.");
                return;
            }

            // to get all new and old values
            var values = Prompt(item, updateMethod);
            // to invoke
            object[] valuesArray = values.Values.ToArray();

            try
            {
                updateMethod.Invoke(_service, valuesArray);
                Console.WriteLine("Item successfully updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item: {ex.Message}");
            }
        }
        public void Delete(int id)
        {
            try
            {
                var serviceType = _service.GetType();
                var method = serviceType.GetMethod("Delete");
                method!.Invoke(_service, new object[] { id });
                Console.WriteLine("Success");
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid input. Action terminated.");
            }

        }


        public void SmartPick(object item)
        {
            var serviceType = _service.GetType();
            var method = serviceType.GetMethod("SmartPick");
            if (method == null)
            {
                Console.WriteLine("Method 'SmartPick' not found on the service.");
                return;
            }

            try
            {
                method.Invoke(_service, new object[] { item });
                Console.WriteLine("SmartPick completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SmartPick: {ex.Message}");
            }
        }


        public void ShowTable()
        {
            var properties = _type.GetProperties();
            int totalColumns = properties.Length;
            int pages = (int)Math.Ceiling((double)totalColumns / _columnsPerPage);

            for (int i = 0; i < pages; i++)
            {
                Console.WriteLine(CreateHeader(i));
                foreach (var item in _data)
                {
                    Console.WriteLine(CreateRow(item, i));
                }
                Console.WriteLine("\nPress Enter to see next page...");
                Console.ReadLine();
            }
        }
        private string CreateHeader(int pageIndex)
        {
            var properties = _type.GetProperties()
                .Where(p => p.IsDefined(typeof(TableItemAttribute), false))
                .OrderBy(p => p.GetCustomAttribute<TableItemAttribute>()!.ColumnOrder)
                .Skip(pageIndex * _columnsPerPage)
                .Take(_columnsPerPage);

            var sb = new StringBuilder();
            foreach (var item in properties)
            {
                sb.Append(item.Name.PadRight(25)).Append(' ');
            }
            return sb.ToString();
        }

        private string CreateRow(T item, int pageIndex)
        {
            var properties = _type.GetProperties()
                .Where(p => p.IsDefined(typeof(TableItemAttribute), false))
                .OrderBy(p => p.GetCustomAttribute<TableItemAttribute>()!.ColumnOrder)
                .Skip(pageIndex * _columnsPerPage)
                .Take(_columnsPerPage);

            var sb = new StringBuilder();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item);
                string formattedValue;
                if (value is IEnumerable enumerable && value is not string)
                {
                    // If the value is enumerable (list, dictionary, etc.), format it for display
                    formattedValue = $"[{string.Join(", ", enumerable.Cast<object>())}]";
                }
                else
                {
                    // Otherwise, use the ToString() method
                    formattedValue = value?.ToString() ?? string.Empty;
                }
                sb.Append(formattedValue.PadRight(25)).Append(' ');
            }
            return sb.ToString();
        }
    }
}
