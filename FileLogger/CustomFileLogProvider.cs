using System;
using Microsoft.Extensions.Logging;
using FileLogger;
using Lab10_DI_GUI;
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
/// This project is a custom file logger provider that creates a custom file logger. 
/// </summary>

namespace FileLogger
{
    public class CustomFileLogProvider : ILoggerProvider
    {

        private CustomFileLogger logger;

        /// <summary>
        /// creates the custom file logger
        /// </summary>
        /// <param name="categoryName">The name for the custom file logger</param>
        /// <returns> returns the custom Ilogger</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName);
        }

        /// <summary>
        /// disposes of the file logger
        /// </summary>
        public void Dispose()
        {
            this.Dispose();
        }
    }

}