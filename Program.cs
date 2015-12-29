using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace clipstack
{
    static class Program
    {

        static Stack<Clip> stack;
        static int ignoreNext = 0; // I REALLY HATE DOING THIS 

        [STAThread]
        static void Main()
        {
            stack = new Stack<Clip>();
            NotificationHelper.Initialize(Keys.Control | Keys.Oem3);
            NotificationHelper.ClipboardHandler += NotificationHelper_ClipboardHandler;
            NotificationHelper.HotkeyHandler += NotificationHelper_HotkeyHandler;
            NotificationHelper.Loop();
        }

        private static void NotificationHelper_HotkeyHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Ctrl-` caught.");
            if (stack.Count > 0)
            {
                Clip c = stack.Pop();

                ignoreNext += 2; // another dirty hack ;___;
                Clipboard.SetData(c.format, c.data);
                Console.WriteLine("Popping " + c.format);
                SendKeys.Send("^{v}");
            }
        }

        private static void NotificationHelper_ClipboardHandler(object sender, EventArgs e)
        {
            
            IDataObject data = Clipboard.GetDataObject();
            if (ignoreNext  == 0)
            {
                if (data != null && data.GetFormats().Length > 0)
                {
                    Console.WriteLine("Clipboard was modified");
                    Clip c;
                    c.format = data.GetFormats()[0]; // IS THIS OK TO DO???
                    c.data = data.GetData(c.format);
                    stack.Push(c);
                    Console.WriteLine("Pushed " + data.GetFormats()[0]);
                }
                
            }
            else
            {
                ignoreNext -= 1;
            }

        }

        struct Clip
        {
            public object data;
            public string format;
        }


    }
}
