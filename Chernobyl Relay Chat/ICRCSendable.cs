using Avalonia.Media;
using System.Collections.Generic;

namespace Chernobyl_Relay_Chat
{
    interface ICRCSendable
    {
        void AddInformation(string message);
        void AddError(string message);
        void AddMessage(string nick, string message, IBrush nickBrush);
    }
}
