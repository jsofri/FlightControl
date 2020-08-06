# FlightControl
An academic project for understanding GUI and MVVM architecture by making a WPF app.
Program enable to operate basic aircraft controls, the "plane" itself .run on a server whereever you'd like 

![app window](https://github.com/yehonatansofri/FlightControl/blob/master/out/app_window.PNG?raw=true)

### Requirements
FlightControl requires .NET framework 4 (at least version 4.6). Compilation with Visual Stuidio 2019.
All resources are self contained.

### Installation
Download FlightSimulator.zip from master branch.
After download completion, double-click on `FlightSimulator.sln`

### Run
When running the application, the first window will ask you to write the server IP and port.
This is the address of server running the plane. The default is for local machine on **port 5402**.
If you just want to move the controls without server, press start after the "no server" error.

### Error handling
The application never crushes! If an error happaned, a new line with the error specifics will be added to the error panel at the bottom of the screen.
