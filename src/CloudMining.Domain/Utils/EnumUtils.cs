using System.ComponentModel;
using System.Reflection;

namespace CloudMining.Application.Utils;

public static class EnumUtils
{
    public static string GetDescription(this Enum e)
    {
        return e.GetType()
            .GetMember(e.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DescriptionAttribute>()?
            .Description ?? e.ToString();
    }
}