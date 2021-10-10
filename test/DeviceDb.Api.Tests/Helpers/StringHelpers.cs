using System;
using System.Linq;

namespace DeviceDb.Api.Tests.Helpers;

internal class StringHelpers
{
    public static string GetLongString(int validMaxLength) 
        => string.Join("", Enumerable.Repeat(0, validMaxLength + 1).Select(n => (char)new Random().Next(127)));
}
