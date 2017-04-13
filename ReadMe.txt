To install "Metrics Collection System" download two files:
1. MetricsCollectionSystem.msi
2. setup.exe

Prerequisites:
- MSSQL LocalDb Server should be installed on the machine
- .NET Framework 4.0 or higher

Run setup.exe file. You'll see welcome window. Click "Next". In "Select Installation Folder" window select a folder, or leave the default value. In "Confirm Installation" window click "Next" and wait for completeion. Close the window.

The system is installed on the machine. It can be found in the folder, selected for installation. By default the folder is C:\(ProgramFiles of ProgramFiles (x86))\Innopolis University\Metrics Collection System. The launcers are there, and also there're shortcuts on the desktop.

There're two launchers files:
1. MetricsCollectorApplication.exe
2. MetricsSenderApplication.exe

=== MetricsCollectorApplication.exe ===

The application for tracking user activities. Its launch after period without any activities in the system may be rather long because of launching of database server. 

Before launching a user can select the events when the application makes snapshots - reads all the information about current user activity (window title, .exe path of current application, etc.). 
There're 3 events for selection:
1. Foreground Window Change Tracking - the application makes a snapshot each time a user switches from one window to another
2. Mouse Left Click Tracking - the application makes a snapshot each time a user clicks left button
3. State Scanning - the application makes a snapshot each time after a specified State Scanning Interval; each of the above actions resets the timer for state scanning, so it occurs only when user does nothing for at least State Scanning Interval.

In settings a user can specify State Scanning Interval, and Data Saving Interval - the interval of storing into the database. WARNING! When a user changes settings, it's recommended to run program as administrator, otherwise a failure is possible.

=== MetricsSenderApplication.exe ===

The application for observing collected activities, and sending activities to the server.

+ Data Table +
This table is for observing tracked activities. To obtain activities click "Refresh" button. It may take some time to process data, especially if the collector has been working for a long period.

+ Filter +
Filters out activities. Add a string to filter, click "Refresh" - all the activities, whose window title contains that string, will be filtered out.
WARNING! Filter is case sensitive!
Example:

   | Title                 | ..
---|-----------------------|----
   | Folder Manager        |
   | Google Chrome         |
   | Music Player          |
   | Folder Manager        |
   | Video Player          |
   
If you want to filter out all the media player, add "Player" to the filter (exactly "Player", not "player" or something else). Both "Music Player" and "Video Player" will be filtered out.
If you want to filter out folder manager, add "Folder Manager". Both "Folder Manager" activities will be filtered out.
If you want to filter out google chrome, add "Google Chrome". Only one activity Google Chrome will be filtered out.

+ Date Filter +
Chooses only activities that were started within given period.

+ Transmission +
When there's a selection of activities in the table, all the visible activities (and only they) can be transmitted to the server. Click "Transmit" button to transmit activities. Sending data requires authorization (it is requested only for the first transmission within current program session, for logout just close the application). All the successfully transmitted data is deleted from the storage.

+ Settings +
WARNING! When a user changes settings, it's recommended to run program as administrator, otherwise a failure is possible.

Here there're the following settings:
1. "Send Per Once". The number of activities sent in one request to the server. It's not recommended to change this number, but a user can increase it or decrease if sending process takes too much time. 
Decreasing means that less activities is sent per once - more little requests.
Increasing means that more activities is sent per once - less requests, but each request is large and takes much time, progress bar will be updated more slowly.
2. "Authorization Uri". Provided by server administrator, and can be changed manually.
3. "Send Data Uri". Provided by server administrator, and can be changed manually.
4. "Update Xml Uri". Provided by server administrator, and can be changed manually.