using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDb.Api.Tests.Helpers
{
    internal class StringHelpers
    {
        public static string GetLongString(int validMaxLength) => string.Join("", Enumerable.Repeat(0, validMaxLength + 1).Select(n => (char)new Random().Next(127)));
    }
}
