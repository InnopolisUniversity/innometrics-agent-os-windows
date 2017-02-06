using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics.Helpers;
using CommonModels;

namespace WindowsMetrics
{
    public class Collector : IDisposable
    {
        private Writer _writer;

        private IntPtr _foregroundWindowHook;
        private IntPtr _mouseClickHook;

        private GCHandle _foregroundWindowHandle;
        private GCHandle _mouseClickHandle;

        private event Action StateScan;

        private Guard _guardStateScanner;
        private Task _taskForGuardStateScanner; // where guard works in

        private const int StateScanIntervalSec = 5; // TODO to config



        #region Handlers for the events being tracked

        private Registry MakeRegistry(CollectionEvent @event)
        {
            //string url = WinAPI.GetChormeURL("sdf"); // TODO url

            string foregroundWinTitle = WinAPI.GetTextOfForegroundWindow();
            string path = WinAPI.GetForegroundWindowExeModulePath();
            string process = WinAPI.GetForegroundWindowProcessName();
            string ip, mac;
            WinAPI.GetAdapters(out ip, out mac);
            string username = WinAPI.GetSystemUserName();
            return new Registry()
            {
                Event = (ushort) @event,
                WindowTitle = foregroundWinTitle,
                ExeModulePath = path,
                ProcessName = process,
                Time = DateTime.Now,
                Username1 = new Username() { Value = username },
                IpAddress = new IpAddress() { Value = ip },
                MacAddress = new MacAddress() { Value = mac }
            };
        }

        private readonly Action<object> _onForegroundWindowChangeAddon = null;
        private void OnForegroundWindowChange()
        {
            Registry registry = MakeRegistry(CollectionEvent.WIN_CHANGE);
            if (registry != null)
            {
                _writer.Add(registry);
                _onForegroundWindowChangeAddon?.Invoke(registry);
                _guardStateScanner.Reset();
            }
        }

        private readonly Action<object> _onLeftMouseClickAddon = null;
        private void OnLeftMouseClick()
        {
            Registry registry = MakeRegistry(CollectionEvent.LEFT_CLICK);
            if (registry != null)
            {
                _writer.Add(registry);
                _onLeftMouseClickAddon?.Invoke(registry);
                _guardStateScanner.Reset();
            }
        }

        private readonly SynchronizationContext _sync;
        private readonly Action<object> _onGuardStateScanAddon = null;
        private void OnGuardStateScan()
        {
            Registry registry = MakeRegistry(CollectionEvent.STATE_SCAN);
            if (registry != null)
            {
                _writer.Add(registry);
                if (_onGuardStateScanAddon != null)
                {
                    SendOrPostCallback c = (state) => { _onGuardStateScanAddon.Invoke(state); };
                    _sync.Post(c, registry);
                }
            }
        }

        #endregion Handlers for the events being tracked



        private void CommonConstructor(Writer writer)
        {
            _writer = writer;
            StateScan += OnGuardStateScan;

            _taskForGuardStateScanner = new Task(() =>
                {
                    _guardStateScanner = new Guard(
                        actionToDoEveryTick: () => StateScan?.Invoke(),
                        secondsToCountdown: StateScanIntervalSec
                    );
                }
            );

            _taskForGuardStateScanner.Start();
        }

        public Collector(Writer writer)
        {
            CommonConstructor(writer);
        }
        
        /// <param name="writer"></param>
        /// <param name="sync"></param>
        /// <param name="onForegroundWindowChangeAddon">Action with the string that is created when onForegroundWindowChange occurs</param>
        /// <param name="onLeftMouseClickAddon">Action with the string that is created when onLeftMouseClick occurs</param>
        /// <param name="onGuardStateScanAddon">Action with the string that is created when onGuardStateScan occurs</param>
        public Collector(Writer writer, SynchronizationContext sync,
            Action<object> onForegroundWindowChangeAddon, Action<object> onLeftMouseClickAddon, Action<object> onGuardStateScanAddon)
        {
            _onForegroundWindowChangeAddon = onForegroundWindowChangeAddon;
            _onLeftMouseClickAddon = onLeftMouseClickAddon;
            _onGuardStateScanAddon = onGuardStateScanAddon;
            _sync = sync;
            CommonConstructor(writer);
        }

        public void Start()
        {
            _foregroundWindowHook = WinAPI.StartTrackingForegroundWindowChange(OnForegroundWindowChange, out _foregroundWindowHandle);
            _mouseClickHook = WinAPI.StartTrackingLeftClickEvent(OnLeftMouseClick, out _mouseClickHandle);

            _guardStateScanner.Start();
        }

        public bool Stop()
        {
            bool success = WinAPI.StopTrackingForegroundWindowChange(_foregroundWindowHook);
            bool success2 = WinAPI.StopTrackingLeftClickEvent(_mouseClickHook);
            _guardStateScanner.Stop();
            return success && success2;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
