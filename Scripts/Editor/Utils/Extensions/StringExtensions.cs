using System.Linq;

namespace UnityIdeEx.Editor.ide_ex.Scripts.Editor.Utils.Extensions
{
    internal static class StringExtensions
    {
        public static string[] SplitJson(this string s)
        {
            return s.Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Select(x => x.Replace(@"""", ""))
                .ToArray();
        }
    }
}