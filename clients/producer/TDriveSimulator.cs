using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace producer
{
    public class TDriveSimulator
    {
        public IList<TDrivePosition> PositionCollection { get; set; }
        public string DriveName { get; set; }
        public EventHubClient EventHubClient { get; set; }
        public int Interval { get; set; } = 1;

        private System.Timers.Timer _timer; 
        private int _current = 0;
        private long _timeSpan;

        public TDriveSimulator()
        {
            _timer = new System.Timers.Timer();
            _timer.Enabled = false;
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_current < PositionCollection.Count &&
                PositionCollection[_current].DateTime.AddSeconds(_timeSpan) <= DateTime.Now )
            {
                try
                {
                    PositionCollection[_current].DateTime = PositionCollection[_current].DateTime.AddSeconds(_timeSpan);
                    EventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PositionCollection[_current]))));
                    _current++;
                    Console.Write('.');
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {ex.Message}");
                }
            }
        }

        public void LoadPositionCollection(string fileName)
        {
            PositionCollection = new List<TDrivePosition>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (sr.Peek() > 0)
                {
                    var source = sr.ReadLine();
                    PositionCollection.Add(new TDrivePosition(source));
                }
            }
            for(int i = 1; i < PositionCollection.Count; i++)
            {
                PositionCollection[i].Distance =  PositionCollection[i].DistanceTo(PositionCollection[i - 1]);
            }
        }

        public void Start()
        {
            _timeSpan = (long)(DateTime.Now - PositionCollection[0].DateTime).TotalSeconds;
            _timer.Interval = Interval * 1000;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
