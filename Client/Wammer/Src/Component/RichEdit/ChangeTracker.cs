#region

using System;
using System.Drawing;
using System.Text;

#endregion

namespace Waveface.Component.RichEdit
{
    // Tracks the changes made to the underlying RichTextBox, since those changes are not directly exposed. 
    // Needed in order to correctly and efficiently implement Undo and Redo. 
    internal class ChangeTracker
    {
        private IEditor m_editor;
        private StringBuilder m_previousText;
        private CharacterRange m_storedSelection;

        public ChangeTracker(IEditor editor)
        {
            m_editor = editor;
            m_storedSelection = editor.SelectionRange;
            m_previousText = new StringBuilder(editor.Text);
        }

        public CharacterRange PreviousSelection
        {
            get { return m_storedSelection; }
        }

        public string Text
        {
            get { return m_previousText.ToString(); }
        }

        // Analyzes the changes made to the editor between the last call to AnalyzeChange
        // and this one. It is important that this method get called every time the selection in the 
        // editor changes, so the tracker state follows the editor state accurately. 
        public ChangeDescription AnalyzeChange()
        {
            // When changes occur, they must occur where the selection is. What we do here is to keep
            // track of where the selection is, and where it was. We know that if the selection started n
            // characters from the front of the string last time, and m characters from the front of the 
            // selection last time, then there can have been no changes in the first min(n, m) characters.
            // Similarly, if the selection ended x characters from the end of the string last time, and ends
            // y characters from the end of the string this time, there can have been no changes in the last
            // min(x, y) characters. Then we simply compare between those two points to figure out what 
            // actually changed. 

            int _length = m_editor.TextLength;

            CharacterRange _selection = m_editor.SelectionRange;

            // In some situations, the selection can extend one character past the end of the text. We just
            // treat it as if it went to the end. 
            if ((_selection.First + _selection.Length) > _length)
            {
                _selection.Length = _length - _selection.First;
            }

            CharacterRange _previousSelection = m_storedSelection;
            m_storedSelection = _selection;


            // Count of characters that are guaranteed the same at the start of the new and old strings
            int _sameUntil = Math.Min(_selection.First, _previousSelection.First);
            
            // Negative of the count of characters that are guaranteed the same at the end of the new and old strings
            int _sameAfter = -Math.Min(_length - _selection.First - _selection.Length,
                                      m_previousText.Length - _previousSelection.First - _previousSelection.Length);

            // If the first n characters are the same, and the last m characters are the same, 
            // and the sum of m and n equals the length of the string
            if ((_sameUntil - _sameAfter) == _length)
            {
                // and the length hasn't changed, 
                if (_length == m_previousText.Length)
                {
                    // then the string is unchanged
                    return new ChangeDescription(_previousSelection, _selection);
                }
            }

            // Grab the subset of the text that's in the "potentially changed" range
            string _contents = m_editor.GetTextRange(new CharacterRange(_sameUntil, _length + _sameAfter - _sameUntil));

            int _frontIndex = _sameUntil;

            // Iterate forward through the string, looking for where it starts to differ from previous
            int _scanTo = _length + _sameAfter;
            
            while (_frontIndex < _scanTo)
            {
                // If the new string is longer, then obviously it differs starting where the old one ends
                if (_frontIndex >= m_previousText.Length)
                {
                    break;
                }

                int _contentsIndex = _frontIndex - _sameUntil;

                // If the old string is longer, then obvious it differs starting wher the new one ends
                if (_contentsIndex >= _contents.Length)
                {
                    break;
                }

                if (m_previousText[_frontIndex] != _contents[_contentsIndex])
                {
                    break;
                }

                ++_frontIndex;
            }

            // Back index is actually a count of characters from the end of the string. Zero indicates the 
            // position just after the last character in the string. Maybe it should be a negative number, 
            // but I chose to make it positive
            int _backIndex = _sameAfter;

            // Iterate backward through the string, looking for where it stops differing from previous
            while (true)
            {
                int _previousIndex = m_previousText.Length + _backIndex - 1;
                int _contentsIndex = _contents.Length + _sameAfter + _backIndex - 1;

                // If we've iterated to a point where we slam into frontIndex, we've already checked
                // this far, and there's no need to go farther
                if (_previousIndex <= _frontIndex)
                {
                    break;
                }

                // If we've iterated to a point where we slam into frontIndex in the new string, we've
                // already checked this far, and there's no need to go farther
                if (_contentsIndex - _sameAfter <= _frontIndex)
                {
                    break;
                }

                // If we've walked off the front of the new string, then the old string was longer, and
                // we can stop looking for differences
                if (_contentsIndex < 0)
                {
                    break;
                }

                if (m_previousText[_previousIndex] != _contents[_contentsIndex])
                {
                    break;
                }

                --_backIndex;
            }

            // Sometimes we can wind up walking past the front index

            // If the new text is shorter than the old text
            if (_length < m_previousText.Length)
            {
                // And if the back index falls before the front index
                if ((_length + _backIndex) < _frontIndex)
                {
                    // Set them to be the same
                    _backIndex = _length - _frontIndex;
                }
            }
                // Otherwise, if the old text is shorter than the new text
            else if (_length > m_previousText.Length)
            {
                // And the back index falls before the front index
                if ((m_previousText.Length + _backIndex) < _frontIndex)
                {
                    // Set them to be the same
                    _backIndex = _frontIndex - m_previousText.Length;
                }
            }

            // If the front and back indices indicate the same point in the current text
            if ((_length + _backIndex) == _frontIndex)
            {
                // and in the previous text
                if (_length == m_previousText.Length)
                {
                    // Then nothing has changed
                    return new ChangeDescription(_previousSelection, _selection);
                }
            }

            int _previousChangeStart = _frontIndex;
            int _previousChangeEnd = m_previousText.Length + _backIndex;

            int _capacity = _previousChangeEnd - _previousChangeStart;

            string _oldText = "";
            
            if (_capacity > 0)
            {
                StringBuilder _bld = new StringBuilder(_previousChangeEnd - _previousChangeStart);

                for (int i = _previousChangeStart; i < _previousChangeEnd; ++i)
                {
                    _bld.Append(m_previousText[i]);
                }

                _oldText = _bld.ToString();
            }

            // Update the string that tracks the previous contents to match the new contents
            int _currentChangeStart = _frontIndex;
            int _currentChangeEnd = _length + _backIndex;

            string _newText =
                m_editor.GetTextRange(new CharacterRange(_currentChangeStart, _currentChangeEnd - _currentChangeStart));

            UpdateText(_previousChangeStart, _previousChangeEnd - _previousChangeStart, _newText);

            return new ChangeDescription(_frontIndex, _previousSelection, _oldText, _selection, _newText);
        }

        public void Redo(ChangeDescription change)
        {
            UpdateText(change.Start, change.TextBefore.Length, change.TextAfter);
            m_storedSelection = change.SelectionAfter;
        }

        public void Undo(ChangeDescription change)
        {
            UpdateText(change.Start, change.TextAfter.Length, change.TextBefore);
            m_storedSelection = change.SelectionBefore;
        }

        private void UpdateText(int start, int length, string text)
        {
            if (length != 0)
            {
                m_previousText.Remove(start, length);
            }

            if (text.Length != 0)
            {
                m_previousText.Insert(start, text);
            }
        }
    }
}