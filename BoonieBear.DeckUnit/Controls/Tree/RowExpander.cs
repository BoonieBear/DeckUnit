using System.Windows;
using System.Windows.Controls;

namespace BoonieBear.DeckUnit.Controls.Tree
{
	public class RowExpander : Control
	{
		static RowExpander()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RowExpander), new FrameworkPropertyMetadata(typeof(RowExpander)));
		}
	}
}
