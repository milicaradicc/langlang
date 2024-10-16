using LangLang.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using LangLang.Services;
using System.Collections;

namespace LangLang.FormTable
{
    public class Memory
    {
        private readonly ILanguageService _languageService;
        public Memory(ILanguageService languageService)
        {
            _languageService = languageService;
        }
        public static object GetValueFromInput(string input, Type targetType)
        {
            if (targetType == typeof(string))
            {
                return input;
            }
            else if (targetType.IsPrimitive)
            {
                return Convert.ChangeType(input, targetType);
            }
            else if (Nullable.GetUnderlyingType(targetType) != null)
            {
                return ParseNullableType(input, targetType);
            }
            else if (targetType == typeof(DateOnly))
            {
                return DateOnly.Parse(input);
            }
            else if (targetType == typeof(TimeOnly))
            {
                return TimeOnly.Parse(input);
            }
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return ParseDictionary(input, targetType);
            }
            else if (targetType.IsPrimitive || targetType.IsEnum)
            {
                return ParseEnum(input,targetType);
            }
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = targetType.GetGenericArguments()[0];
                return ParseList(input, elementType);
            }
            else if (!targetType.IsPrimitive && !targetType.IsEnum && !targetType.IsGenericType)
            {
                return ParseComplexType(input, targetType);
            }
            else
            {
                throw new NotSupportedException($"Type {targetType.Name} is not supported");
            }
        }

        private static object ParseList(string input, Type elementType)
        {
            string[] values = input.Split(',');
            Type listType = typeof(List<>).MakeGenericType(elementType);
            IList list = (IList)Activator.CreateInstance(listType)!;

            foreach (string value in values)
            {
                object parsedValue;

                if (elementType.IsEnum)
                {
                    parsedValue = ParseEnum(value, elementType);
                }
                else
                {
                    parsedValue = ParseComplexType(value, elementType);
                }

                list!.Add(parsedValue);
            }

            return list!;
        }

        private static object ParseComplexType(string input, Type targetType)
        {
            string[] propertyValues = input.Trim().Split(' ');

            var constructor = targetType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == propertyValues.Length) ?? throw new Exception($"No suitable constructor found for type {targetType.Name}");
            var parameters = constructor.GetParameters();
            object[] parameterValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                object value;

                if (parameter.ParameterType.IsEnum)
                {
                    value = Enum.Parse(parameter.ParameterType, propertyValues[i]);
                }
                else
                {
                    value = GetValueFromInput(propertyValues[i], parameter.ParameterType);
                }

                parameterValues[i] = value;
            }

            var instance = constructor.Invoke(parameterValues);
            return instance;
        }

        private static object ParseEnum(string input, Type targetType)
        {
            if (!targetType.IsEnum)
            {
                throw new ArgumentException("Target type must be an enum");
            }

            if (Enum.TryParse(targetType, input, out object enumValue))
            {
                return enumValue!;
            }
            else if (Enum.IsDefined(targetType, input))
            {
                return Enum.Parse(targetType, input);
            }
            else
            {
                throw new ArgumentException($"Invalid input '{input}' for enum type '{targetType.Name}'");
            }
        }
        private static object ParseNullableType(string input, Type targetType)
        {
            if (string.IsNullOrEmpty(input) || input.ToLower() == "null")
            {
                return null!;
            }
            Type underlyingType = Nullable.GetUnderlyingType(targetType)!;
            return GetValueFromInput(input, underlyingType);
        }
        private static object ParseDictionary(string input, Type targetType)
        {
            // Example format: key1:value1,key2:value2,key3:value3
            var keyValuePairs = input.Split(',')
                                      .Select(pair => pair.Split(':'))
                                      .ToDictionary(
                                            parts => Convert.ChangeType(parts[0].Trim(), targetType.GetGenericArguments()[0]),
                                            parts => Convert.ChangeType(parts[1].Trim(), targetType.GetGenericArguments()[1])
                                       );
            return keyValuePairs;
        }

        public void ShowLanguages()
        {
            List<Language> languages = _languageService.GetAll();

            Console.WriteLine("{0,-5} {1,-20} {2,-5} {3}", "ID", "Name", "Level", new string('-', 25));

            foreach (Language language in languages)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-5} {3}", language.Id, language.Name, language.Level, new string('-', 25));
            }
        }
    }
}
