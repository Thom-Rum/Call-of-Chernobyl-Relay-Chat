using Avalonia.Media;

namespace Chernobyl_Relay_Chat
{
    class CRCGameWrapper : ICRCSendable
    {
        public void AddError(string message)
        {
            CRCGame.AddError(message);
        }

        public void AddInformation(string message)
        {
            CRCGame.AddInformation(message);
        }

        // The game has no concept of brush colours; forward as plain information.
        public void AddMessage(string nick, string message, IBrush nickBrush)
        {
            CRCGame.AddInformation(nick + ": " + message);
        }
    }
}
