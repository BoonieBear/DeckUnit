using System.Collections.Generic;
using System.Windows;

namespace BoonieBear.DeckUnit.Utilities.Delegates
{
    public delegate bool MessageShowEventHandler(string message = null, string title = null, bool isYesNo=false);
    public delegate MessageBoxResult MessageShowDialogEventHandler(string message = null, MessageBoxButton buttonType = MessageBoxButton.OK, string title = null);
    public delegate void MultiInterfaceMessageShowEventHandler(string message, List<string> interfaceStringList);
    public delegate void PageShowEventHandler(object view, object viewModel);
    public class MessageBoxEvent 
    {
        public static MessageShowEventHandler ShowMessage { get; set; }
        public static MessageShowDialogEventHandler ShowMessageDialog { get; set; }
        public static MultiInterfaceMessageShowEventHandler ShowMultiInterfaceMessage { get; set; }
        public static PageShowEventHandler ShowPage { get; set; }
    }
}
