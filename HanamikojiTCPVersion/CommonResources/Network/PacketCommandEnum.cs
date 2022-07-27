namespace CommonResources.Network
{
    public enum PacketCommandEnum
    {
        GameState,               // Packet contains GameState
        Message,                 // Packet with message to display
        Error,                   // Packet with error to handle
        MakeMove,                // Sent by Server to Player
        PlayerMove,              // Sent by Player to Server; constains move
    }
}
