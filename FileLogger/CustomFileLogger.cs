using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Author:    Alex Thurgood and Toby Armstrong
/// Date:      March 30, 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Toby Armstrong - This work may not 
///            be copied for use in Academic Coursework.
///
///  Toby Armstrong and Alex Thurgood, certify that we wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// This project is a custom file logger  that creates the logger and sets the constructor of the logger. 
/// </summary>

namespace Lab10_DI_GUI
{
    public class CustomFileLogger : ILogger
    {
        private readonly string _name;
        private readonly string _fileName;
        public CustomFileLogger(string name)
        {
            _name = name;
            _fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
               + Path.DirectorySeparatorChar
               + $"CS3500-{name}.log";
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string logMessage = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {_name}: {formatter(state, exception)}{Environment.NewLine}";
            File.AppendAllText(_fileName, logMessage);

        }
    }
}