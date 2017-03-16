﻿using System;
using System.Threading;
using System.Windows.Threading;

namespace CommonLibrary
{
    public static class Extensions
    {
        public static void InvokeIfRequired(this Dispatcher disp, Action dotIt, DispatcherPriority priority)
        {
            if(disp.Thread != Thread.CurrentThread)
            {
                disp.Invoke(priority, dotIt);
            }
            else
                dotIt();
        }
    }
}
