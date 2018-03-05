using Neo.Debugger.Forms;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neo.Debugger.Utils
{
    public class CodeHoverEventArgs : EventArgs
    {
        public Point Pos { get; set; }
        public int Line { get; set; }
        public int Char { get; set; }
        public string Word { get; set; }
    }

    internal class MouseHoverManager : IDisposable
    {
        public static bool Enable = true;

        private static int _hoverTimerInterval;
        private Scintilla _textForm;
        private Point _lastPos = new Point(0, 0);
        private int _lastChar = 0;
        private string _lastWord = String.Empty;

        public CodeHoverEventHandler CodeHovered;
        public delegate void CodeHoverEventHandler(object sender, CodeHoverEventArgs e);

        public MouseHoverManager(Scintilla textForm, int hoverTimerInterval)
        {
            //Set the hover timer interval
            _hoverTimerInterval = hoverTimerInterval;

            //Get the form
            _textForm = textForm;
            _textForm.MouseDwellTime = hoverTimerInterval;
            _textForm.MouseMove += _textForm_MouseMove;
        }

        private void _textForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (Enable)
            {
                _textForm.DwellStart += _textForm_DwellStart;
                _textForm.DwellEnd += _textForm_DwellEnd;
                SetMousePosition(e.Location);
            }
        }

        private void _textForm_DwellEnd(object sender, DwellEventArgs e)
        {
            _textForm.CallTipCancel();
        }

        private void _textForm_DwellStart(object sender, DwellEventArgs e)
        {
            if (!String.IsNullOrEmpty(_lastWord))
                _textForm.CallTipShow(_textForm.CurrentPosition, _lastWord);
            else
                _textForm.CallTipCancel();
        }

        public void SetMousePosition(Point pos)
        {
            _lastPos = pos;
            _lastChar = GetCharUnderCursor(pos);
            _textForm.GotoPosition(_lastChar);
            _lastWord = _textForm.GetWordFromPosition(_lastChar);
        }

        public int GetCharUnderCursor(Point pos)
        {
            return _textForm.CharPositionFromPoint(pos.X, pos.Y);
        }

        public string GetTextUnderCursor(Point pos)
        {
            return string.Empty;
        }

        public void Dispose()
        {
            if (_textForm != null)
                _textForm.Dispose();
        }
    }
}
