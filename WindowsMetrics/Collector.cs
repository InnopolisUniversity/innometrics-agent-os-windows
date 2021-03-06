﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonModels;
using CommonModels.Helpers;

namespace WindowsMetrics
{
    public class Collector
    {
        private Writer _writer;

        private IntPtr _foregroundWindowHook;
        private IntPtr _mouseClickHook;

        private GCHandle _foregroundWindowHandle;
        private GCHandle _mouseClickHandle;

        private event Action StateScan;
        private int _stateScanIntervalSec;

        private Guard _guardStateScanner;
        private Task _taskForGuardStateScanner; // where guard works in
        private CancellationTokenSource _tokenSource;

        private bool _enableForegroundWindowChangeTracking;
        private bool _enableLeftClickTracking;
        private bool _enableStateScanning;



        #region Handlers for the events being tracked

        private static Registry MakeRegistry(CollectionEvent @event)
        {
            IntPtr winId = WinAPI.GetForegroundWindowId();
            string foregroundWinTitle = WinAPI.GetTextOfForegroundWindow();
            string path = WinAPI.GetForegroundWindowExeModulePath();
            string process = WinAPI.GetForegroundWindowProcessName();
            string ip, mac;
            WinAPI.GetAdapters(out ip, out mac);
            string username = WinAPI.GetSystemUserName();
            
            string url = null;
            if (process.Contains("chrome"))
            {
                url = WinAPI.GetChormeUrl();
                //WinAPI.Tabs(out a);
                //url = Convert.ToString(a);
            }

            var r = new Registry()
            {
                Event = (ushort) @event,
                WindowTitle = foregroundWinTitle,
                ExeModulePath = path,
                ProcessName = process,
                Time = DateTime.Now,
                Username1 = new Username() { Value = username },
                IpAddress = new IpAddress() { Value = ip },
                MacAddress = new MacAddress() { Value = mac },
                WindowId = winId.ToString(),
                Url = url,
                Processed = false
            };

            return r;
        }

        private readonly Action<object> _onForegroundWindowChangeAddon = null;
        [ExcludeFromCodeCoverage]
        private void OnForegroundWindowChange()
        {
            Registry registry = MakeRegistry(CollectionEvent.WIN_CHANGE);
            if (registry != null)
            {
                _writer.Add(registry);
                _onForegroundWindowChangeAddon?.Invoke(registry);
                _guardStateScanner?.Reset();
            }
        }

        private readonly Action<object> _onLeftMouseClickAddon = null;
        [ExcludeFromCodeCoverage]
        private void OnLeftMouseClick()
        {
            Registry registry = MakeRegistry(CollectionEvent.LEFT_CLICK);
            if (registry != null)
            {
                _writer.Add(registry);
                _onLeftMouseClickAddon?.Invoke(registry);
                _guardStateScanner?.Reset();
            }
        }

        private readonly SynchronizationContext _sync;
        private readonly Action<object> _onGuardStateScanAddon = null;
        [ExcludeFromCodeCoverage]
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



        private void CommonConstructor(Writer writer, int stateScanIntervalSec, 
            bool enableForegroundWindowChangeTracking, bool enableLeftClickTracking, bool enableStateScanning)
        {
            _writer = writer;
            StateScan += OnGuardStateScan;
            _stateScanIntervalSec = stateScanIntervalSec;
            _enableForegroundWindowChangeTracking = enableForegroundWindowChangeTracking;
            _enableLeftClickTracking = enableLeftClickTracking;
            _enableStateScanning = enableStateScanning;
        }

        public Collector(Writer writer, int stateScanIntervalSec, 
            bool enableForegroundWindowChangeTracking, bool enableLeftClickTracking, bool enableStateScanning)
        {
            CommonConstructor(writer, stateScanIntervalSec, enableForegroundWindowChangeTracking, enableLeftClickTracking, enableStateScanning);
        }

        /// <param name="writer"></param>
        /// /// <param name="stateScanIntervalSec"></param>
        /// <param name="sync"></param>
        /// <param name="onForegroundWindowChangeAddon">Action with the string that is created when onForegroundWindowChange occurs</param>
        /// <param name="onLeftMouseClickAddon">Action with the string that is created when onLeftMouseClick occurs</param>
        /// <param name="onGuardStateScanAddon">Action with the string that is created when onGuardStateScan occurs</param>
        public Collector(Writer writer, int stateScanIntervalSec, 
            bool enableForegroundWindowChangeTracking, bool enableLeftClickTracking, bool enableStateScanning, 
            SynchronizationContext sync, 
            Action<object> onForegroundWindowChangeAddon, Action<object> onLeftMouseClickAddon, Action<object> onGuardStateScanAddon)
        {
            _onForegroundWindowChangeAddon = onForegroundWindowChangeAddon;
            _onLeftMouseClickAddon = onLeftMouseClickAddon;
            _onGuardStateScanAddon = onGuardStateScanAddon;
            _sync = sync;
            CommonConstructor(writer, stateScanIntervalSec, enableForegroundWindowChangeTracking, enableLeftClickTracking, enableStateScanning);
        }

        [ExcludeFromCodeCoverage]
        public void Start()
        {
            if(_enableForegroundWindowChangeTracking)
                _foregroundWindowHook = WinAPI.StartTrackingForegroundWindowChange(OnForegroundWindowChange, out _foregroundWindowHandle);

            if(_enableLeftClickTracking)
                _mouseClickHook = WinAPI.StartTrackingLeftClickEvent(OnLeftMouseClick, out _mouseClickHandle);

            if (_enableStateScanning)
            {
                _tokenSource = new CancellationTokenSource();
                var cancellation = _tokenSource.Token;
                _taskForGuardStateScanner = new Task((token) =>
                {
                    CancellationToken t = (CancellationToken) token;
                    _guardStateScanner = new Guard(
                        actionToDoEveryTick: () => StateScan?.Invoke(),
                        secondsToCountdown: _stateScanIntervalSec
                    );
                    _guardStateScanner.Start();
                }, cancellation);
                _taskForGuardStateScanner.Start();
            }
        }

        [ExcludeFromCodeCoverage]
        public bool Stop()
        {
            bool foregroundWindowChangeTrackingDeactivated = _foregroundWindowHook == IntPtr.Zero;
            bool mouseClickTrackingDeactivated = _mouseClickHook == IntPtr.Zero;

            if (!foregroundWindowChangeTrackingDeactivated)
                foregroundWindowChangeTrackingDeactivated = WinAPI.StopTrackingForegroundWindowChange(_foregroundWindowHook);
            if (!mouseClickTrackingDeactivated)
                mouseClickTrackingDeactivated = WinAPI.StopTrackingLeftClickEvent(_mouseClickHook);

            if (_guardStateScanner != null)
            {
                _guardStateScanner.Stop();
                _tokenSource.Cancel();
                _taskForGuardStateScanner.Wait();
            }

            return foregroundWindowChangeTrackingDeactivated && mouseClickTrackingDeactivated;
        }
    }
}
