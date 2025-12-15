using System.Globalization;
using System.Text;
using System.Text.Json;

namespace MS_EntWatch.Helpers
{
    public static class ServerLocalizer
    {
        static readonly Dictionary<string, Dictionary<string, string>> _locales = new(StringComparer.OrdinalIgnoreCase);

        public static void LoadLocaleFile(string name)
        {
            var file = $"{name}.json";
            var path = Path.Combine(EntWatch._sharpPath!, "locales", file);

            if (!File.Exists(path)) throw new FileNotFoundException("File not found", file);

            var text = File.ReadAllText(path, Encoding.UTF8);

            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(text)
                       ?? throw new InvalidDataException($"Invalid locale file: {name}");
            
            foreach (var (key, kv) in data)
            {
                if (_locales.TryGetValue(key, out _)) continue; //Dublicate
                Dictionary<string, string> NewDict = [];
                foreach (var (lang, value) in kv)
                {
                    NewDict.TryAdd(CultureInfo.GetCultureInfo(lang).Name, value);
                }
                _locales.TryAdd(key, NewDict);
            }
        }

        public static string Format(CultureInfo culture, string key, params ReadOnlySpan<object?> param)
        {
            if (_locales.TryGetValue(key, out var local) && local.TryGetValue(culture.Name, out var value)) return string.Format(value, param);
            return key;
        }
    }
}
