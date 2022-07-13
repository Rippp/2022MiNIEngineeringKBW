using Newtonsoft.Json;

namespace CommonResources.Network
{
    public class Packet
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public Packet(string command = "", string message = "")
        {
            Command = command;
            Message = message;
        }

        public override string ToString()
        {
            return string.Format(
                "[Packet:\n" +
                "  Command=`{0}`\n" +
                "  Message=`{1}`]",
                Command, Message);
        }

        public string SerializeToJson()
            => JsonConvert.SerializeObject(this);

        public static Packet DeserializeFromJson(string jsonData)
            => JsonConvert.DeserializeObject<Packet>(jsonData);
    }
}
