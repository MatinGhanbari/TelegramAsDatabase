using Newtonsoft.Json;

namespace TelegramAsDatabase.Models;

[Serializable]
public class TDBData<T>
{
    public string Key { get; set; }
    public T Value { get; set; }

    public void Verify()
    {
        if (this == null) throw new ArgumentNullException(nameof(TDBData<T>));
        if (string.IsNullOrWhiteSpace(Key)) throw new ArgumentNullException(nameof(Key));
        if (Value == null) throw new ArgumentNullException(nameof(Value));

        if (TDBConstants.MarkdownCharacters.Any(c => Key.Contains(c)))
            throw new ArgumentNullException(nameof(Key));
    }

    public static implicit operator string(TDBData<T> data) => JsonConvert.SerializeObject(data)!;
    public static implicit operator TDBData<T>(string data) => JsonConvert.DeserializeObject<TDBData<T>>(data)!;
}