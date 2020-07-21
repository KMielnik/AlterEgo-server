using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Core.Helpers
{
    public class ProcessAsyncHelper
    {
        public static IAsyncEnumerable<OutputEvent> RunAnimationProcessAsync((string path, string arguments) command, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        public class OutputEvent
        {
            public EventType EventType { get; init; }
            public double Time { get; init; }
            public string Filename { get; init; }
        }

        public class EventType
        {
            public bool IsError { get; init; }
            public string Name { get; init; }
            public string Text { get; init; }
        }
    }
}
