using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCodeGeneratorNet.Core.Misc
{
    public class DebugEventHandlerArgs
    {
        public string Message { get; private set;}
        public DebugEventHandlerArgs(string message)
        {
            this.Message = message;
        }
    }
    public class GDebug
    {
        public static event EventHandler<DebugEventHandlerArgs> WriteEvent;
        public static event EventHandler ClearEvent;
        public static void Write(object obj)
        {
            if(WriteEvent != null)
            {
                string message;
                if(obj == null)
                {
                    message = "null";
                }
                else
                {
                    message = obj.ToString();
                }
                WriteEvent(null, new DebugEventHandlerArgs(message));
            }
        }

        public static void WriteLine(object obj)
        {
            if (WriteEvent != null)
            {
                string message;
                if (obj == null)
                {
                    message = "null";
                }
                else
                {
                    message = obj.ToString();
                }
                WriteEvent(null, new DebugEventHandlerArgs(message + "\r\n"));
            }
        }

        public static void Clear()
        {
            if(ClearEvent!= null)
            {
                ClearEvent(null, null);
            }
        }
    }
}
