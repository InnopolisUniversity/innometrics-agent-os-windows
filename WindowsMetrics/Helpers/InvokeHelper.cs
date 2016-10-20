using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsMetrics.Helpers
{
    public static class InvokeHelper
    {
        public static void SafeInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }
            action();
        }

        //public static void SafeInvoke(this Control control, Action action, T obj)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.Invoke(action, obj);
        //        return;
        //    }
        //    action(obj);
        //}

        //public static void SafeInvoke(this Control control, Action action, T1 obj1, T2 obj2, T3 obj3)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.Invoke(action, obj1, obj2, obj3);
        //        return;
        //    }
        //    action(obj1, obj2, obj3);
        //}

        public static void SafeAsyncInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
                return;
            }
            action();
        }

        //public static void SafeAsyncInvoke(this Control control, Action action, T obj)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.BeginInvoke(action, obj);
        //        return;
        //    }
        //    action(obj);
        //}

        //public static void SafeAsyncInvoke(this Control control, Action action, T1 obj1, T2 obj2)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.BeginInvoke(action, obj1, obj2);
        //        return;
        //    }
        //    action(obj1, obj2);
        //}

        //public static void SafeAsyncInvoke(this Control control, Action action, T1 obj1, T2 obj2, T3 obj3)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.BeginInvoke(action, obj1, obj2, obj3);
        //        return;
        //    }
        //    action(obj1, obj2, obj3);
        //}
    }
}
