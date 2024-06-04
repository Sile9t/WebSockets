using Client;
using System.Text.Json;

namespace WebSockets
{
    public class Message
    {
        public string? _text {  get; set; }
        public DateTime _date { get; set; }
        public EPInfo _from { get; set; }
        public EPInfo _to { get; set; }
        public Message() { }
        public Message(string text, EPInfo from, EPInfo to)
        {
            _text = text;
            _date = DateTime.Now;
            _from = from;
            _to = to;
        }
        public string SerializeToJson()
            => JsonSerializer.Serialize(this);
        public static Message? DeserializeFromJson(string json)
            => JsonSerializer.Deserialize<Message>(json);
        public override string ToString()
        {
            return $"{_date} From: {_from} To: {_to}  Message: '{_text}'";
        }
    }
}
