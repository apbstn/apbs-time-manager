using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Shared.Services;

public class Exxception : IExxception
{
    public string Message { get; set; } = string.Empty; // Initialize to empty string

    public Exxception() { }

    public Exxception(string message)
    {
        Message = message ?? string.Empty;
    }
}
