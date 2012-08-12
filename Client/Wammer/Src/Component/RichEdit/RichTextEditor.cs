#region

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.RichEdit
{
    // A RichTextBox2 derived control that adds support for autocomplete and multilevel undo. 
    //
    // RichTextEditor is designed to address some of the shortcomings that arise when using 
    // RichTextBox as a text editor in programming-like environments. It extends RichTextBox2, 
    // adding support for autocomplete (essentially, Intellisense) and for multilevel undo. 
    //
    // RichTextEditor is meant only to work with textual content. Although RichTextBox supports
    // embedding pictures and other non-textual content, use of those features may cause 
    // RichTextEditor to malfunction.
    public class RichTextEditor : RichTextBox2, IEditor
    {
        // Occurs when the user selects an item from the autocomplete list
        public event AutoCompleteAcceptedHandler AutoCompleteAccepted;

        // Occurs when the user cancels the autocomplete list        
        public event EventHandler AutoCompleteCancelled;

        // Occurs when the state of the <see cref="Modified"/> property changes.        
        public new event EventHandler ModifiedChanged;

        // Occurs when the text in the control changes       
        public event TextChanged2EventHandler TextChanged2;

        private AutoCompleteDialog m_autoCompleteDialog = new AutoCompleteDialog();
        private StringCollection m_autoCompleteEntries = new StringCollection();
        private bool m_autoCompleteShowing;
        private int m_autoCompleteStart;
        private bool m_historyActionUnderway;
        private int m_savePoint;
        private ChangeTracker m_tracker;
        private ArrayList m_undoHistory = new ArrayList();
        private int m_undoIndex;
        private int m_undoLength = 100;

        #region Properties

        // Gets a value indicating whether the autocomplete list is currently showing
        public bool AutoCompleteShowing
        {
            get { return m_autoCompleteShowing; }
        }

        // Gets a value indicating if there are any modifications that can be redone
        public new bool CanRedo
        {
            get { return m_undoIndex < m_undoHistory.Count; }
        }

        // Gets a value indicating whether there are any modifications that can be undone
        public new bool CanUndo
        {
            get { return m_undoIndex > 0; }
        }

        // Gets a value indicating what the currently selected autocomplete item is, 
        // or null if none is selected. 
        public string CurrentAutoCompleteItem
        {
            get
            {
                if (!m_autoCompleteShowing)
                {
                    return null;
                }

                ListView _lv = m_autoCompleteDialog.ListView;

                if (_lv.SelectedIndices.Count == 0)
                {
                    return null;
                }

                return _lv.SelectedItems[0].Text;
            }
        }

        // Gets or sets a value indicating whether the contents have been modified
        // since this property was last set to true. 
        public new bool Modified
        {
            get { return m_savePoint != m_undoIndex; }
            set
            {
                if (Modified == value)
                {
                    return;
                }

                if (value == false)
                {
                    m_savePoint = m_undoIndex;
                }
                else
                {
                    m_savePoint = -1;
                }

                if (ModifiedChanged != null)
                {
                    ModifiedChanged(this, EventArgs.Empty);
                }
            }
        }

        // Gets or sets a value indicating how many modifications will be kept in the undo 
        // buffer. Defaults to 100. 
        public int UndoLength
        {
            get { return m_undoLength; }
            set { m_undoLength = value; }
        }

        #endregion

        public RichTextEditor()
        {
            ListView _lv = m_autoCompleteDialog.ListView;
            _lv.ItemActivate += autoComplete_ItemActivate;

            m_autoCompleteDialog.Enter += autoCompleteDialog_Enter;

            m_tracker = new ChangeTracker(this);
        }

        // Clears the undo history         
        public new void ClearUndo()
        {
            m_undoHistory.Clear();
            m_undoIndex = 0;

            base.ClearUndo();
        }

        // Hides the autocomplete list if it is showing      
        public void HideAutoComplete()
        {
            if (!m_autoCompleteShowing)
            {
                return;
            }

            m_autoCompleteDialog.Hide();
            m_autoCompleteShowing = false;
        }

        // Redoes the next action in the control's Redo queue        
        public new void Redo()
        {
            if (m_undoIndex >= m_undoHistory.Count)
            {
                return;
            }

            bool _wasModified = Modified;

            ChangeDescription _action = (ChangeDescription)m_undoHistory[m_undoIndex];
            ++m_undoIndex;

            PreventRedraw();

            m_historyActionUnderway = true;

            try
            {
                m_tracker.Redo(_action);
                SelectionRange = new CharacterRange(_action.Start, _action.TextBefore.Length);
                SelectedText = _action.TextAfter;
                SelectionRange = _action.SelectionAfter;
            }
            finally
            {
                m_historyActionUnderway = false;
                AllowRedraw();
            }

            if (_wasModified != Modified)
            {
                if (ModifiedChanged != null)
                {
                    ModifiedChanged(this, EventArgs.Empty);
                }
            }

            NotifyTextChanged(_action);
        }

        public void ShowAutoComplete(string[] entries)
        {
            ShowAutoComplete(entries, SelectionStart);
        }

        public void ShowAutoComplete(string[] entries, int start)
        {
            if (m_autoCompleteShowing)
            {
                return;
            }

            if (start < 0)
            {
                start = 0;
            }

            m_autoCompleteStart = start;
            SetAutoCompleteEntries(entries, 5);
            Point _location = GetPositionFromCharIndex(m_autoCompleteStart);
            int _lineHeight = (int)Graphics.FromHwnd(Handle).MeasureString(" ", Font).Height;
            _location.Offset(0, _lineHeight);
            m_autoCompleteDialog.Owner = FindForm();
            m_autoCompleteDialog.Show();
            m_autoCompleteDialog.Location = PointToScreen(_location);
            AutoCompleteHighlight();
            Focus();
            m_autoCompleteShowing = true;
        }

        // Undoes the last edit operation 
        public new void Undo()
        {
            if (m_undoIndex <= 0)
            {
                return;
            }

            // We need to check this before we change the undoIndex
            bool _wasModified = Modified;

            --m_undoIndex;

            ChangeDescription _action = (ChangeDescription)m_undoHistory[m_undoIndex];

            PreventRedraw();

            m_historyActionUnderway = true;

            try
            {
                m_tracker.Undo(_action);
                SelectionRange = new CharacterRange(_action.Start, _action.TextAfter.Length);
                SelectedText = _action.TextBefore;
                SelectionRange = _action.SelectionBefore;
            }
            finally
            {
                m_historyActionUnderway = false;
                AllowRedraw();
            }

            if (Modified != _wasModified)
            {
                if (ModifiedChanged != null)
                {
                    ModifiedChanged(this, EventArgs.Empty);
                }
            }

            NotifyTextChanged(_action, true);
        }

        // Override this method to be notified when the selection changes in the control
        protected override void OnSelectionChanged(EventArgs e)
        {
            OnSelectionChanged(e, m_historyActionUnderway);
        }

        // Override this method to be notified when the text changes in the control
        protected virtual void OnTextChanged2(TextChanged2EventArgs args)
        {
            if (TextChanged2 != null)
            {
                TextChanged2(this, args);
            }
        }

        // Processes a command key.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                if (m_autoCompleteShowing)
                {
                    CancelAutoComplete();
                    return true;
                }
            }
            else if (keyData == Keys.Down)
            {
                if (m_autoCompleteShowing)
                {
                    SelectNextAutoCompleteItem();
                    return true;
                }
            }
            else if (keyData == Keys.Up)
            {
                if (m_autoCompleteShowing)
                {
                    SelectPreviousAutoCompleteItem();
                    return true;
                }
            }
            else if (keyData == Keys.Tab || keyData == Keys.Enter)
            {
                if (m_autoCompleteShowing)
                {
                    AcceptAutoComplete();
                    return true;
                }
            }
            else if (keyData == (Keys.Z | Keys.Control))
            {
                Undo();
                return true;
            }
            else if (keyData == (Keys.Y | Keys.Control))
            {
                Redo();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void AcceptAutoComplete()
        {
            PreventRedraw();

            string item = CurrentAutoCompleteItem;

            if (item != null)
            {
                SelectionRange = new CharacterRange(m_autoCompleteStart, SelectionStart - m_autoCompleteStart);
                SelectedText = item;

                SelectionRange = new CharacterRange(m_autoCompleteStart + item.Length, 0);
            }

            if (AutoCompleteAccepted != null)
            {
                AutoCompleteAcceptedArgs _args = new AutoCompleteAcceptedArgs(item);
                AutoCompleteAccepted(this, _args);
            }

            HideAutoComplete();

            AllowRedraw();

            OnSelectionChanged(EventArgs.Empty);
        }

        private void autoCompleteDialog_Enter(object sender, EventArgs e)
        {
            Focus();
        }

        private void autoComplete_ItemActivate(object sender, EventArgs e)
        {
            AcceptAutoComplete();
        }

        private void AutoCompleteHighlight()
        {
            string _region = GetTextRange(new CharacterRange(m_autoCompleteStart, SelectionStart));

            int _found = -1;

            for (int i = 0; i < m_autoCompleteEntries.Count; ++i)
            {
                if (m_autoCompleteEntries[i].StartsWith(_region))
                {
                    _found = i;
                    break;
                }
            }

            if (_found != -1)
            {
                HighlightAndScrollListView(_found);
            }
            else
            {
                m_autoCompleteDialog.ListView.SelectedItems.Clear();
            }
        }

        private void CancelAutoComplete()
        {
            HideAutoComplete();

            if (AutoCompleteCancelled != null)
            {
                AutoCompleteCancelled(this, EventArgs.Empty);
            }
        }

        private void HighlightAndScrollListView(int index)
        {
            ListView _lv = m_autoCompleteDialog.ListView;
            _lv.BeginUpdate();
            _lv.Items[index].Selected = true;
            _lv.EnsureVisible(index);
            _lv.EndUpdate();
        }

        private void NotifyTextChanged(ChangeDescription change)
        {
            NotifyTextChanged(change, false);
        }

        private void NotifyTextChanged(ChangeDescription change, bool isUndo)
        {
            string _after = "";

            if (change.TextAfter != null)
            {
                _after = change.TextAfter;
            }

            string _before = "";

            if (change.TextBefore != null)
            {
                _before = change.TextBefore;
            }

            // No need to notify if there was no change, even though this method gets called
            // sometimes when text hasn't been altered.
            if (_after.Length == 0 && _before.Length == 0)
            {
                return;
            }

            TextChanged2EventArgs _args;

            if (isUndo)
            {
                _args = new TextChanged2EventArgs(change.Start, _after, _before);
            }
            else
            {
                _args = new TextChanged2EventArgs(change.Start, _before, _after);
            }

            OnTextChanged2(_args);
        }

        private void OnSelectionChanged(EventArgs e, bool isFromHistory)
        {
            CharacterRange _range = SelectionRange;

            if (m_autoCompleteShowing)
            {
                int _wordEnd = FindNextWordBreak(m_autoCompleteStart + 1);

                if (_range.First < m_autoCompleteStart)
                {
                    HideAutoComplete();
                }
                else if ((_range.First + _range.Length) > _wordEnd)
                {
                    HideAutoComplete();
                }
                else
                {
                    AutoCompleteHighlight();
                }
            }

            if (!isFromHistory)
            {
                ChangeDescription _change = m_tracker.AnalyzeChange();
                QueueUndoAction(_change);
                NotifyTextChanged(_change);
            }

            base.OnSelectionChanged(e);
        }

        private void QueueUndoAction(ChangeDescription change)
        {
            if (change == null)
            {
                return;
            }

            if (change.TextAfter == null || change.TextBefore == null || change.Start == -1)
            {
                return;
            }

            // We're not at the end of the undo buffer, so nuke anything from here to the end
            if (m_undoIndex != m_undoHistory.Count)
            {
                m_undoHistory.RemoveRange(m_undoIndex, m_undoHistory.Count - m_undoIndex);

                if (m_savePoint > m_undoIndex)
                {
                    m_savePoint = -1;
                }
            }

            // We need to check this before we change the undoIndex or the savePoint, since 
            // the value depends on those things. 
            bool _wasModified = Modified;

            m_undoHistory.Add(change);

            // Truncate the undo buffer to the correct length      
            int _excess = m_undoHistory.Count - m_undoLength;

            if (_excess > 0)
            {
                _excess = Math.Min(_excess, m_undoHistory.Count);

                if (_excess > 0)
                {
                    m_undoHistory.RemoveRange(0, _excess);

                    m_savePoint -= _excess;
                }
            }

            m_undoIndex = m_undoHistory.Count;

            if (!_wasModified)
            {
                if (ModifiedChanged != null)
                {
                    ModifiedChanged(this, EventArgs.Empty);
                }
            }
        }

        private void SelectNextAutoCompleteItem()
        {
            ListView _lv = m_autoCompleteDialog.ListView;

            if (_lv.SelectedItems.Count == 0)
            {
                HighlightAndScrollListView(0);
            }
            else
            {
                int _index = _lv.SelectedIndices[0];

                ++_index;

                if (_index < _lv.Items.Count)
                {
                    HighlightAndScrollListView(_index);
                }
            }
        }

        private void SelectPreviousAutoCompleteItem()
        {
            ListView _lv = m_autoCompleteDialog.ListView;

            if (_lv.SelectedItems.Count == 0)
            {
                HighlightAndScrollListView(0);
            }
            else
            {
                int _index = _lv.SelectedIndices[0];
                --_index;

                if (_index >= 0)
                {
                    HighlightAndScrollListView(_index);
                }
            }
        }

        private void SetAutoCompleteEntries(string[] entries, int maxShowing)
        {
            if (entries == null)
            {
                return;
            }

            ListView _lv = m_autoCompleteDialog.ListView;
            m_autoCompleteEntries.Clear();
            m_autoCompleteEntries.AddRange(entries);

            _lv.BeginUpdate();
            _lv.Dock = DockStyle.None;
            _lv.Items.Clear();

            foreach (string _entry in entries)
            {
                ListViewItem lvi = new ListViewItem(_entry);
                _lv.Items.Add(_entry);
            }

            if (entries.Length > 0)
            {
                _lv.Items[0].Selected = true;
            }

            _lv.Columns[0].Width = -1;

            int _width = _lv.Width;

            Size _borderSize = SystemInformation.Border3DSize;

            int LVM_FIRST = 0x1000; // ListView messages
            int LVM_APPROXIMATEVIEWRECT = (LVM_FIRST + 64);

            int _count = maxShowing > 1 ? maxShowing : 1;
            _count = entries.Length > maxShowing ? maxShowing : entries.Length;
            int _result = Interop.SendMessage(_lv.Handle, LVM_APPROXIMATEVIEWRECT,
                                             _count, -1);

            short _height = (short)((_result & 0xFFFF00000) >> 16);
            //short _width = (short) (result & 0x0000FFFF); 

            m_autoCompleteDialog.Size = new Size(_width - (2 * _borderSize.Width), _height - (2 * _borderSize.Height));
            _lv.Dock = DockStyle.Fill;
            _lv.EndUpdate();
        }
    }
}