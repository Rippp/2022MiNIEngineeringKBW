namespace CommonResources.Network
{
    public enum PacketCommandEnum
    {
        GameData,                // Packet contains GameData
        Message,                 // Packet with message to display
        Error,                   // Packet with error to handle
        Ready,                   // Sent by Player to Server 
        MakeMove,                // Sent by Server to Player
        PlayerMove               // Sent by Player to Server
    }
}
