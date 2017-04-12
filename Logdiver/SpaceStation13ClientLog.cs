using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using Logdiver.Util;

namespace Logdiver
{
    public class SpaceStation13ClientLog : IDisposable
    {
        private bool hasBeenRead;

        public static string DELIMITER = "\n";

        public string FileName { get; private set; }

        public string Content { get; private set; }

        private object SyncRoot = new object();

        public EventHandler<LineEventArgs> OnLine;

        private ISynchronizeInvoke synchronize;

        private LogFileMonitor monitor;

        private Dictionary<string, string> _replacementDictionary = new Dictionary<string, string>
        {
            {"<span class='game say'>", "GAMESAY: " },
            {"<span class='ooc'><span class='looc'>", "LOOC: "},
            {"<span class='ooc'>", "OOC: "},
            {"<span class='comradio'>", "COMRADIO: "},
            {"<span class='deadsay'>", "DEADSAY: "},
            {"<span class='secradio'>", "SECRADIO: "},
            {"<span class='medradio'>", "MEDRADIO: "},
            {"<span class='sciradio'>", "SCIRADIO: "},
            {"<span class='supradio'>", "SUPRADIO: "},
            {"<span class='srvradio'>", "SRVRADIO: "},
            {"<span class='syndradio'>", "SYNDRADIO: "},
            {"<span class='airadio'>", "AIRADIO: "},
            {"<span class='centradio'>", "CENTRADIO: "},
            {"<span class='radio'>", "RADIO: "},
            {"<span class='danger'>", "DANGER: "},
            {"<span class='notice'>", "NOTICE: "},
            {"<span class='pm'>", "Admin PM:" },
            {"<span class='mod_channel'>", "MSAY:" },
            {"[J]","" },
            {"(F)","" },
            {"(F|E)","" },
            {"[F]","" },
            {"[B]","" },
            {"[R]","" },
            {"\r", "" }
        };

        public SpaceStation13ClientLog(string fileName, ISynchronizeInvoke synchronize)
        {
            FileName = fileName;
            this.synchronize = synchronize;
        }

        public void Dispose()
        {
            monitor.Stop();
            monitor.OnLine -= MonitorLine;
            monitor = null;
            Content = null;
            FileName = null;
            _replacementDictionary = null;
            OnLine = null;
        }

        private void MonitorLine(object sender, LineEventArgs lineEventArgs)
        {
            lock (SyncRoot)
            {
                var line = ProcessString(lineEventArgs.Line);

                line = $"[{DateTime.Now:HH:mm:ss}]" + line;

                Content += Environment.NewLine + line;
                OnLine?.Invoke(this, new LineEventArgs(line.Trim()));
            }
    }

        public void InitialRead()
        {
            if (hasBeenRead) return;

            hasBeenRead = true;

            string read;

            using (var fileStream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var streamReader = new StreamReader(fileStream))
                read = streamReader.ReadToEnd();

            var size = new FileInfo(FileName).Length;

            Content = ProcessString(read);

            foreach (var line in Content.Split(new[] { DELIMITER }, StringSplitOptions.RemoveEmptyEntries))
            {
                OnLine?.Invoke(this, new LineEventArgs(line.Trim()));
            }

            monitor = new LogFileMonitor(FileName, synchronize, DELIMITER);
            monitor.OnLine += MonitorLine;
            monitor.Start(size);
        }

        private string ProcessString(string text)
        {
            foreach (var entry in _replacementDictionary)
                text = text.Replace(entry.Key, entry.Value);

            text = text.RemoveStyles();
            text = text.StripTagsCharArray();
            text = WebUtility.HtmlDecode(text);

            return text;
        }

        public string GetMatchingContent(string filter)
        {
            var builder = new StringBuilder();
            foreach (var line in Content.Split(new[] { DELIMITER }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(filter))
                {
                    builder.AppendLine(line.Trim());
                }
            }

            return builder.ToString();
        }
    }
}
