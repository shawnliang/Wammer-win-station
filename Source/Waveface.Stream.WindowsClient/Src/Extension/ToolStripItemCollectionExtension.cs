using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

public static class ToolStripItemCollectionExtension
{
	public static ToolStripItem Add(this ToolStripItemCollection toolStripItem, string text, EventHandler onClick)
	{
		return toolStripItem.Add(text, null, onClick);
	}

    public static ToolStripItem Add(this ToolStripItemCollection toolStripItem,string key, string text, EventHandler onClick)
    {
        var item = toolStripItem.Add(text, onClick);
        item.Name = key;

        return item;
    }
}
