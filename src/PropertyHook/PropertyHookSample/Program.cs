using System;
using System.Threading;

namespace PropertyHookSample
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleHook hook = new SampleHook();
            hook.Start();

            while (!Console.KeyAvailable)
            {
                Console.WriteLine($"Health: {hook.Health,4}");
                if (hook.Health < 200)
                    hook.Health += 200;
                Thread.Sleep(100);
            }

            // Not strictly necessary; the hooking thread is a background thread, so it will exit automatically
            hook.Stop();
        }
    }
}
