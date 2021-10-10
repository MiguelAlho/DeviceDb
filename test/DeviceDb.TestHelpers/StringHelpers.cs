namespace DeviceDb.TestHelpers;

public class StringHelpers
{
    public static string GetLongString(int validMaxLength)
        => string.Join("", Enumerable.Repeat(0, validMaxLength + 1).Select(n => (char)new Random().Next(127)));
}
