using System;
using System.Collections.Generic;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    public struct ConsoleLine {
        public readonly int lineNumber;
        public char[] line;

        internal ConsoleLine(int lineNumber, char[] line) {
            this.lineNumber = lineNumber;
            this.line = line;
        }

        internal void AdjustCols(int cols) {
            if (line.Length < cols) return;

            char[] newLine = new char[cols];

            Array.Copy(line, newLine, Math.Min(cols, line.Length));

            for (int i = line.Length; i < cols; i++) newLine[i] = ' ';

            line = newLine;
        }

        public override string ToString() => new string(line);
    }

    public class KSPConsoleBuffer {
        private static readonly String[] LineSeparators = new String[] {"\r\n", "\n"};

        private readonly LinkedList<ConsoleLine> bufferLines;

        private LinkedListNode<ConsoleLine> topLine;

        private LinkedListNode<ConsoleLine> cursorLine;

        private int maxLines;
        private int visibleRows;
        private int visibleCols;
        private int cursorCol;
        private int cursorRow;

        private object consoleLock = new object();

        public KSPConsoleBuffer(int visibleRows, int visibleCols, int maxLines = 2000) {
            bufferLines = new LinkedList<ConsoleLine>();
            this.visibleRows = Math.Max(visibleRows, 1);
            this.visibleCols = Math.Max(visibleCols, 1);
            this.maxLines = maxLines;

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
                bufferLines.Clear();
                topLine = null;
                AddLines(visibleRows);

                cursorCol = cursorRow = 0;
                cursorLine = topLine;
            }
        }

        public void Print(string message) => PrintLines(message.Split(LineSeparators, StringSplitOptions.None));

        public void PrintLine(string message) => Print(message + "\n");

        public void PrintLines(string[] lines) {
            lock (consoleLock) {
                for (int i = 0; i < lines.Length; i++) {
                    if (i > 0) {
                        cursorCol = 0;
                        if (cursorLine.Next == null) {
                            AddLines(1);
                            cursorLine = bufferLines.Last;
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

                if (bufferLines.Count < visibleRows)
                    AddLines(visibleRows - bufferLines.Count);

                topLine = bufferLines.Last;
                while (topLine.Previous != null && topLine.Value.lineNumber >= bufferLines.Count - visibleRows)
                    topLine = topLine.Previous;

                cursorRow = Math.Min(cursorRow, visibleRows - 1);
                cursorCol = Math.Min(cursorCol, visibleCols);

                cursorLine = topLine;
                for (int i = 0; i < cursorRow && cursorLine.Next != null; i++) cursorLine = cursorLine.Next;
            }
        }

        private void AddLines(int count) {
            for (int i = 0; i < count; i++)
                bufferLines.AddLast(new ConsoleLine(bufferLines.Count, new char[visibleCols]));
            if (topLine == null) topLine = bufferLines.First;
            while (topLine != null && topLine.Value.lineNumber < bufferLines.Count - visibleRows)
                topLine = topLine.Next;
            while (bufferLines.Count > maxLines) bufferLines.RemoveFirst();
        }
    }
}
