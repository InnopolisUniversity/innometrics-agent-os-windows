INSTALLATION

To install "Metrics Collection System" download two files:
1. MetricsCollectionSystem.msi
2. setup.exe

There's a prerequisite for the installation: MSSQL LocalDb Server should be installed on the machine.

Run setup.exe file. You'll see welcome window. Click "Next".
In "Select Installation Folder" window select a folder, or leave the default value.
In "Confirm Installation" window click "Next" and wait for completeion. Close the window.

The system is installed on the machine. Its launchers can be found in the folder, selected for installation. By default the folder is 
C:\(ProgramFiles of ProgramFiles (x86))\Innopolis University\Metrics Collection System

There're two .exe files there:
1. MetricsCollectorApplication.exe
2. MetricsSenderApplication.exe

MetricsCollectorApplication.exe is an application for tracking user activities.
MetricsSenderApplication.exe is an application for observing collected activities, and sending activities to a server.