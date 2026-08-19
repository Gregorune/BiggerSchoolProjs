using System.Text.RegularExpressions;

namespace FitnesAPI;

public static class Validators
{
    private static Regex emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    
    public static bool IsEmail(string text)
    {
        return emailRegex.IsMatch(text);
    }
}