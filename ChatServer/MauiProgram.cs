using Microsoft.Extensions.Logging;
/// <summary>
/// Author:    Alex Thurgood and Toby Armstrong
/// Date:      March 30, 2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Toby Armstrong - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Toby Armstrong and Alex Thurgood, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// Methods that build the Chat Server MAUI GUI, not much modified in this except for adding FileLogging statements.
/// </summary>
/// 

namespace ChatServer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
              .Services
        .AddLogging(configure =>
        {
            configure.AddDebug();
            configure.SetMinimumLevel(LogLevel.Debug);
        })
        .AddTransient<MainPage>();

        return builder.Build();
    }
}
