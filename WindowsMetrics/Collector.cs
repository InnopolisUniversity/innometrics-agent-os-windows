﻿using System;
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
        private int _stateScanIntervalSec;

        private Guard _guardStateScanner;
        private Task _taskForGuardStateScanner; // where guard works in



        #region Handlers for the events being tracked

        private Registry MakeRegistry(CollectionEvent @event)
        {
            //string url = WinAPI.GetChormeURL("sdf"); // TODO url

            IntPtr winId = WinAPI.GetForegroundWindowId();
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
                MacAddress = new MacAddress() { Value = mac },
                WindowId = winId.ToString()
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
                _guardStateScanner?.Reset();
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
                _guardStateScanner?.Reset();
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



        private void CommonConstructor(Writer writer, int stateScanIntervalSec)
        {
            _writer = writer;
            StateScan += OnGuardStateScan;
            _stateScanIntervalSec = stateScanIntervalSec;
        }

        public Collector(Writer writer, int stateScanIntervalSec)
        {
            CommonConstructor(writer, stateScanIntervalSec);
        }

        /// <param name="writer"></param>
        /// /// <param name="stateScanIntervalSec"></param>
        /// <param name="sync"></param>
        /// <param name="onForegroundWindowChangeAddon">Action with the string that is created when onForegroundWindowChange occurs</param>
        /// <param name="onLeftMouseClickAddon">Action with the string that is created when onLeftMouseClick occurs</param>
        /// <param name="onGuardStateScanAddon">Action with the string that is created when onGuardStateScan occurs</param>
        public Collector(Writer writer, int stateScanIntervalSec, SynchronizationContext sync,
            Action<object> onForegroundWindowChangeAddon, Action<object> onLeftMouseClickAddon, Action<object> onGuardStateScanAddon)
        {
            _onForegroundWindowChangeAddon = onForegroundWindowChangeAddon;
            _onLeftMouseClickAddon = onLeftMouseClickAddon;
            _onGuardStateScanAddon = onGuardStateScanAddon;
            _sync = sync;
            CommonConstructor(writer, stateScanIntervalSec);
        }

        public void Start(bool enableForegroundWindowChangeTracking, bool enableLeftClickTracking, bool enableStateScanning)
        {
            if(enableForegroundWindowChangeTracking)
                _foregroundWindowHook = WinAPI.StartTrackingForegroundWindowChange(OnForegroundWindowChange, out _foregroundWindowHandle);

            if(enableLeftClickTracking)
                _mouseClickHook = WinAPI.StartTrackingLeftClickEvent(OnLeftMouseClick, out _mouseClickHandle);

            if (enableStateScanning)
            {
                _taskForGuardStateScanner = new Task(() =>
                    {
                        _guardStateScanner = new Guard(
                            actionToDoEveryTick: () => StateScan?.Invoke(),
                            secondsToCountdown: _stateScanIntervalSec
                        );
                    }
                );
                _taskForGuardStateScanner.Start();
                while (_taskForGuardStateScanner.Status == TaskStatus.WaitingToRun)
                {
                    Thread.Sleep(1);
                }
                _guardStateScanner?.Start();
            }
        }

        public bool Stop()
        {
            bool foregroundWindowChangeTrackingDeactivated = _foregroundWindowHook == IntPtr.Zero;
            bool mouseClickTrackingDeactivated = _mouseClickHook == IntPtr.Zero;

            if (!foregroundWindowChangeTrackingDeactivated)
                foregroundWindowChangeTrackingDeactivated = WinAPI.StopTrackingForegroundWindowChange(_foregroundWindowHook);
            if (!mouseClickTrackingDeactivated)
                mouseClickTrackingDeactivated = WinAPI.StopTrackingLeftClickEvent(_mouseClickHook);

            _guardStateScanner?.Stop();

            return foregroundWindowChangeTrackingDeactivated && mouseClickTrackingDeactivated;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
