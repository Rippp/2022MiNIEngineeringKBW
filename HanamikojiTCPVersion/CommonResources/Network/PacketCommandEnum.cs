namespace CommonResources.Network
{
    public enum PacketCommandEnum
    {
        GameState,               // Packet contains GameState
        Message,                 // Packet with message to display
        Error,                   // Packet with error to handle
        Ready,                   // Sent by Player to Server 
        MakeMove,                // Sent by Server to Player
        PlayerMove,              // Sent by Player to Server; constains move
        MoveVerified,            // Sent by Server to Player; validates made move
        MoveInvalid
    }
}
