#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace Waveface
{
    public class TabOrderManager
    {
        #region TabSchemeComparer

        private class TabSchemeComparer : IComparer
        {
            private TabScheme comparisonScheme;

            // Create a tab scheme comparer that compares using the given scheme.

            // <param name="scheme"></param>
            public TabSchemeComparer(TabScheme scheme)
            {
                comparisonScheme = scheme;
            }

            #region IComparer Members

            public int Compare(object x, object y)
            {
                Control control1 = x as Control;
                Control control2 = y as Control;

                if (control1 == null || control2 == null)
                {
                    Debug.Assert(false, "Attempting to compare a non-control");
                    return 0;
                }

                if (comparisonScheme == TabScheme.None)
                {
                    // Nothing to do.
                    return 0;
                }

                if (comparisonScheme == TabScheme.AcrossFirst)
                {
                    // The primary direction to sort is the y direction (using the Top property).
                    // If two controls have the same y coordination, then we sort them by their x's.
                    if (control1.Top < control2.Top)
                    {
                        return - 1;
                    }
                    else if (control1.Top > control2.Top)
                    {
                        return 1;
                    }
                    else
                    {
                        return (control1.Left.CompareTo(control2.Left));
                    }
                }
                else // comparisonScheme = TabScheme.DownFirst
                {
                    // The primary direction to sort is the x direction (using the Left property).
                    // If two controls have the same x coordination, then we sort them by their y's.
                    if (control1.Left < control2.Left)
                    {
                        return - 1;
                    }
                    else if (control1.Left > control2.Left)
                    {
                        return 1;
                    }
                    else
                    {
                        return (control1.Top.CompareTo(control2.Top));
                    }
                }
            }

            #endregion
        }

        #endregion

        #region TabScheme

        public enum TabScheme
        {
            None,
            AcrossFirst,
            DownFirst
        }

        #endregion

        private Control container;

        // Hash of controls to schemes so that individual containers can have different ordering strategies than their parents.

        // The tab index we start numbering from when the tab order is applied.
        private int curTabIndex;
        private Hashtable schemeOverrides;

        public TabOrderManager(Control container)
        {
            this.container = container;
            curTabIndex = 0;
            schemeOverrides = new Hashtable();
        }

        private TabOrderManager(Control container, int curTabIndex, Hashtable schemeOverrides)
        {
            this.container = container;
            this.curTabIndex = curTabIndex;
            this.schemeOverrides = schemeOverrides;
        }

        public void SetSchemeForControl(Control c, TabScheme scheme)
        {
            schemeOverrides[c] = scheme;
        }

        public int SetTabOrder(TabScheme scheme)
        {
            // Tab order isn't important enough to ever cause a crash, so replace any exceptions
            // with assertions.
            try
            {
                ArrayList controlArraySorted = new ArrayList();
                controlArraySorted.AddRange(container.Controls);
                controlArraySorted.Sort(new TabSchemeComparer(scheme));

                foreach (Control c in controlArraySorted)
                {
                    c.TabIndex = curTabIndex++;

                    if (c.Controls.Count > 0)
                    {
                        // Control has children -- recurse.
                        TabScheme childScheme = scheme;

                        if (schemeOverrides.Contains(c))
                            childScheme = (TabScheme) schemeOverrides[c];

                        curTabIndex = (new TabOrderManager(c, curTabIndex, schemeOverrides)).SetTabOrder(childScheme);
                    }
                }

                return curTabIndex;
            }
            catch (Exception e)
            {
                Debug.Assert(false, "Exception in TabOrderManager.SetTabOrder:  " + e.Message);
                return 0;
            }
        }
    }
}