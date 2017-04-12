using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable StringLastIndexOfIsCultureSpecific.1

namespace Logdiver.Util
{
    public class LineEventArgs : EventArgs
    {
        public LineEventArgs(string line)
        {
            Line = line;
        }

        public string Line { get; set; }
    }

    public class LogFileMonitor
    {
        public EventHandler<LineEventArgs> OnLine;

        // file path + delimiter internals
        readonly string _path;
        readonly string _delimiter;

        // timer object
        Timer _t;

        // buffer for storing data at the end of the file that does not yet have a delimiter
        string _buffer = string.Empty;

        public long CurrentSize { get; private set; }

        // are we currently checking the log (stops the timer going in more than once)
        bool _isCheckingLog;
        private readonly ISynchronizeInvoke _synchronize;

        protected bool StartCheckingLog()
        {
            lock (_t)
            {
                if (_isCheckingLog)
                    return true;

                _isCheckingLog = true;
                return false;
            }
        }

        protected void DoneCheckingLog()
        {
            lock (_t)
                _isCheckingLog = false;
        }

        public LogFileMonitor(string path, ISynchronizeInvoke synchronize, string delimiter = "\n")
        {
            _path = path;
            _delimiter = delimiter;
            CurrentSize = 0;
            _synchronize = synchronize;
        }

        public void Start(long size)
        {
            CurrentSize = size;

            // start the timer
            _t = new Timer {SynchronizingObject = _synchronize, AutoReset = true};
            _t.Elapsed += CheckLog;
            _t.Start();
        }

        public void Start()
        {
            CurrentSize = new FileInfo(_path).Length;

            // start the timer
            _t = new Timer {SynchronizingObject = _synchronize, AutoReset = true};
            _t.Elapsed += CheckLog;
            _t.Start();
        }

        private void CheckLog(object s, ElapsedEventArgs e)
        {
            if (!StartCheckingLog()) return;
            try
            {
                // get the new size
                var newSize = new FileInfo(_path).Length;

                // if they are the same then continue.. if the current size is bigger than the new size continue
                if (CurrentSize >= newSize)
                    return;

                // read the contents of the file
                using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(stream))
                {
                    // seek to the current file position
                    sr.BaseStream.Seek(CurrentSize, SeekOrigin.Begin);

                    // read from current position to the end of the file
                    var newData = _buffer + sr.ReadToEnd();

                    // if we don't end with a delimiter we need to store some data in the buffer for next time
                    if (!newData.EndsWith(_delimiter))
                    {
                        // we don't have any lines to process so save in the buffer for next time
                        if (newData.IndexOf(_delimiter) == -1)
                        {
                            _buffer += newData;
                            newData = string.Empty;
                        }
                        else
                        {
                            // we have at least one line so store the last section (without lines) in the buffer
                            var pos = newData.LastIndexOf(_delimiter) + _delimiter.Length;
                            _buffer = newData.Substring(pos);
                            newData = newData.Substring(0, pos);
                        }
                    }

                    // split the data into lines
                    var lines = newData.Split(new[] { _delimiter }, StringSplitOptions.RemoveEmptyEntries);

                    // send back to caller, NOTE: this is done from a different thread!
                    foreach (var line in lines)
                    {
                        OnLine?.Invoke(this, new LineEventArgs(line));
                    }
                }

                // set the new current position
                CurrentSize = newSize;
            }
            catch (Exception)
            {
                // ignored
            }

            // we done..
            DoneCheckingLog();
        }

        public void Stop()
        {
            _t?.Stop();
        }
    }
}
