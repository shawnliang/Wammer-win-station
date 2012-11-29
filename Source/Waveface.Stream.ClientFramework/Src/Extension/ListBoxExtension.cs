using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

public static class ListBoxExtension
{
	public static void SetAllSelected(this ListBox obj, Boolean value)
	{
        var count = obj.Items.Count;

        try
        {
            obj.SuspendLayout();
            obj.BeginUpdate();
            for (var idx = 0; idx < count; ++idx)
            {
                obj.SetSelected(idx, value);
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            obj.EndUpdate();
            obj.ResumeLayout();
        }
	}

    public static void SelectAll(this ListBox obj)
    {
        obj.SetAllSelected(true);
    }

    public static void UnSelectAll(this ListBox obj)
    {
        obj.SetAllSelected(false);
    }
}
