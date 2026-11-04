using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;

namespace HelloWorld;

public class MyModule
{
    readonly IGameApiClient m_GameApiClient;
    readonly ILogger<MyModule> m_Logger;

    public MyModule(IGameApiClient gameApiClient, ILogger<MyModule> logger)
    {
        m_GameApiClient = gameApiClient;
        m_Logger = logger;
    }

    [CloudCodeFunction("SayHello")]
    public string Hello(string name)
    {
        return $"Hello, {name}!";
    }

    [CloudCodeFunction("GetServerTime")]
    public string GetServerTime(IExecutionContext context)
    {
        return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }

    [CloudCodeFunction("HandleNewPlayerNameEntry")]
    public string HandleNewPlayerNameEntry(string newName)
    {
        if (IsPlayerNameValid(newName))
        {
            return newName;
        }

        throw new ArgumentException("Name is not valid.");
    }

    private bool IsPlayerNameValid(string name)
    {
        if (name.Length is < 4 or 16)
        {
            return false;
        }

        // check for special characters using LINQ
        if (!name.All(c => char.IsLetterOrDigit(c)))
        {
            return false;
        }

        return true;
    }
}


