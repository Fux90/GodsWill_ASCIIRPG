using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GodsWill_ASCIIRPG.View;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GodsWill_ASCIIRPG.UIControls
{
    public partial class LogUserControl : UserControl, ISaveableAtomListener, IPgStoryAtomListener, IScrollable
    {
        private const string rowsSerializationName = "rows";
        private const string lastShownSerializationName = "lastShown";

        public const float FontSize = 10.0f;
        private List<LogRow> rows;
        private int currentLastShown;

        private int VisualizedRowCount { get { return (int)Math.Ceiling(this.Height / (FontSize + 2.0f)); } }

        public LogUserControl()
        {
            InitializeComponent();

            this.Size = new Size();

            this.DoubleBuffered = true;
            this.BackColor = Color.Black;
            this.BorderStyle = BorderStyle.FixedSingle;
            rows = new List<LogRow>();

            currentLastShown = 0;
        }

        public void NotifyMessage(Atom who, string msg)
        {
            //TODO: different colors, given different actions/monster

            if (msg != null && msg != "")
            {
                rows.Add(new LogRow(String.Format("[{0}]: {1}", who.Name, msg))
                {
                    Color = who.Color,
                });
                currentLastShown++;
                this.Refresh();
            }
        }

        public void CleanPreviousMessages()
        {
            rows.Clear();
            currentLastShown = 0;
            this.Refresh();
        }

        public void ScrollUp()
        {
            currentLastShown = Math.Max(VisualizedRowCount - 1, currentLastShown - 1);
            this.Invalidate();
        }

        public void ScrollDown()
        {
            currentLastShown = Math.Min(currentLastShown + 1, rows.Count);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            //var numVisualizedRows = (int)Math.Ceiling(this.Height / (FontSize + 2.0f));
            var numVisualizedRows = VisualizedRowCount;

            var startIndex = 0;
            //if (rows.Count >= numVisualizedRows)
            if (currentLastShown >= numVisualizedRows)
            {
                //startIndex = rows.Count - numVisualizedRows + 1;
                startIndex = currentLastShown - numVisualizedRows + 1;
            }

            var pos = new PointF(0.0f, 0.0f);
            for (int ixR = startIndex; ixR < rows.Count; ixR++)
            {
                g.DrawString(   rows[ixR].Message,
                                rows[ixR].Font,
                                rows[ixR].Brush,
                                pos);
                pos.Y += FontSize;
            }
        }

        public void SaveMessages(Stream outputStream)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter serializer = new BinaryFormatter();

            serializer.Serialize(ms, rows);

            ms.WriteTo(outputStream);
        }

        public bool LoadMessages(Stream inputStream)
        {
            if (inputStream != null)
            {
                var formatter = new BinaryFormatter();
                rows = (List<LogRow>)formatter.Deserialize(inputStream);

                return true;
            }

            return false;
        }

        public void SaveMessagesAsTxt(StreamWriter w)
        {
            var story = new StringBuilder();

            foreach (var row in rows)
            {
                story.AppendLine(row.ToString());
            }

            w.Write(story);
        }
    }

    [Serializable]
    public struct LogRow : ISerializable
    {
        private const string fontSerializableName = "font";
        private const string colorSerializableName = "color";
        private const string msgSerializableName = "msg";

        public Font Font { get; set; }
        public Color Color { get; set; }
        public Brush Brush { get { return new SolidBrush(Color); } }
        public string Message { get; set; }

        public LogRow(string message)
        {
            Font = new Font(FontFamily.GenericMonospace, LogUserControl.FontSize);
            Color = Color.White;
            Message = message;
        }

        public LogRow(SerializationInfo info, StreamingContext context)
        {
            Font = (Font)info.GetValue(fontSerializableName, typeof(Font));
            Color = (Color)info.GetValue(colorSerializableName, typeof(Color));
            Message = (string)info.GetValue(msgSerializableName, typeof(string));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
             info.AddValue(fontSerializableName, Font, typeof(Font));
             info.AddValue(colorSerializableName, Color, typeof(Color));
             info.AddValue(msgSerializableName, Message, typeof(string));
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
