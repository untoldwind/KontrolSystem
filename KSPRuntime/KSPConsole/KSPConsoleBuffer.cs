using System;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public struct ConsoleLine {
        public readonly int lineNumber;
        public char[] line;

        internal ConsoleLine(int _lineNumber, char[] _line) {
            lineNumber = _lineNumber;
            line = _line;
        }

        internal void AdjustCols(int cols) {
            if (line.Length < cols) return;

            char[] new_line = new char[cols];

            Array.Copy(line, new_line, Math.Min(cols, line.Length));

            for (int i = line.Length; i < cols; i++) new_line[i] = ' ';

            line = new_line;
        }

        public override string ToString() => new string(line);
    }

    public class KSPConsoleBuffer {
        private static String[] LINE_SEPARATORS = new String[] { "\r\n", "\n" };

        private readonly LinkedList<ConsoleLine> buffer_lines;

        private LinkedListNode<ConsoleLine> topLine;

        private LinkedListNode<ConsoleLine> cursorLine;

        private int maxLines;
        private int visibleRows;
        private int visibleCols;
        private int cursorCol;
        private int cursorRow;

        private object consoleLock = new object();

        public KSPConsoleBuffer(int _visibleRows, int _visibleCols, int _maxLines = 2000) {
            buffer_lines = new LinkedList<ConsoleLine>();
            visibleRows = Math.Max(_visibleRows, 1);
            visibleCols = Math.Max(_visibleCols, 1);
            maxLines = _maxLines;

            Clear();
        }

        public int CursorCol => cursorCol;

        public int CursorRow => cursorRow;

        public int VisibleCols => visibleCols;

        public int VisibleRows => visibleRows;

        public List<ConsoleLine> VisibleLines {
            get {
                lock (consoleLock) {
                    List<ConsoleLine> lines = new List<ConsoleLine>();
                    LinkedListNode<ConsoleLine> current = topLine;

                    while (current != null) {
                        lines.Add(current.Value);
                        current = current.Next;
                    }
                    return lines;
                }
            }
        }

        public void Clear() {
            lock (consoleLock) {
                buffer_lines.Clear();
                topLine = null;
                AddLines(visibleRows);

                cursorCol = cursorRow = 0;
                cursorLine = topLine;
            }
        }

        public void Print(string message) => PrintLines(message.Split(LINE_SEPARATORS, StringSplitOptions.None));

        public void PrintLine(string message) => Print(message + "\n");

        public void PrintLines(string[] lines) {
            lock (consoleLock) {
                for (int i = 0; i < lines.Length; i++) {
                    if (i > 0) {
                        cursorCol = 0;
                        if (cursorLine.Next == null) {
                            AddLines(1);
                            cursorLine = buffer_lines.Last;
                        } else {
                            cursorLine = cursorLine.Next;
                            cursorRow++;
                        }
                    }
                    string line = lines[i];
                    cursorLine.Value.AdjustCols(visibleCols);
                    for (int j = 0; cursorCol < visibleCols && j < line.Length; j++)
                        cursorLine.Value.line[cursorCol++] = line[j];
                }
            }
        }

        public void MoveCursor(int row, int col) {
            lock (consoleLock) {
                cursorRow = Math.Max(Math.Min(row, visibleRows), 0);
                cursorCol = Math.Max(Math.Min(col, visibleCols), 0);

                cursorLine = topLine;
                for (int i = 0; i < cursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        public void Resize(int rows, int cols) {
            lock (consoleLock) {
                visibleRows = rows;
                visibleCols = cols;

                if (buffer_lines.Count < visibleRows)
                    AddLines(visibleRows - buffer_lines.Count);

                topLine = buffer_lines.Last;
                while (topLine.Previous != null && topLine.Value.lineNumber >= buffer_lines.Count - visibleRows)
                    topLine = topLine.Previous;

                cursorRow = Math.Min(cursorRow, visibleRows - 1);
                cursorCol = Math.Min(cursorCol, visibleCols);

                cursorLine = topLine;
                for (int i = 0; i < cursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        private void AddLines(int count) {
            for (int i = 0; i < count; i++)
                buffer_lines.AddLast(new ConsoleLine(buffer_lines.Count, new char[visibleCols]));
            if (topLine == null) topLine = buffer_lines.First;
            while (topLine != null && topLine.Value.lineNumber < buffer_lines.Count - visibleRows)
                topLine = topLine.Next;
            while (buffer_lines.Count > maxLines) buffer_lines.RemoveFirst();
        }
    }
}
