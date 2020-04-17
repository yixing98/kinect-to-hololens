# Kinect to HoloLens
A pipeline that connects a Kinect for Azure and a HoloLens in real-time.

![a figure from paper](kinect-to-hololens.jpg)

# Requirement
- A Windows 10 computer, an Azure Kinect, and a HoloLens (v1).
- CMake, Unity3D 2018.4, and Visual Studio 2019.

# How to Use
Download the examples from https://github.com/hanseuljun/kinect-to-hololens/releases.  
For installing the HoloLens application, see https://docs.microsoft.com/en-us/hololens/holographic-custom-apps.

# Build Instructions
1. git clone --recursive https://github.com/hanseuljun/kinect-to-hololens
2. Using vcpkg, install these libraries: asio, ffmpeg, libvpx, ms-gsl, opencv, and readerwriterqueue.
```powershell
.\vcpkg.exe install asio:x86-windows asio:x64-windows ffmpeg:x86-windows ffmpeg:x64-windows libvpx:x86-windows libvpx:x64-windows ms-gsl:x86-windows ms-gsl:x64-windows opencv:x86-windows opencv:x64-windows readerwriterqueue:x86-windows readerwriterqueue:x64-windows
```
3. Install Kinect for Azure Kinect Sensor SDK 1.4.0 (https://docs.microsoft.com/en-us/azure/Kinect-dk/sensor-sdk-download).
- Not through vcpkg since, currently, vcpkg does not support azure-kinect-sensor-sdk as a static library (i.e. x64-windows-static).
4. Run run-cmake.ps1 in /cpp to build Visual Studio solutions.
5. Run build-plugin.ps1 to build a Unity3D plugin and copy it into the Unity3D project in /unity/KinectToHoloLens.
6. Build executable files with the Visual Studio solution in /cpp/build/x64 and the Unity3D project.

## Special Instructions for Opus
Since Opus decides whether to have avx instructions in the library based on the computer that builds the library, with most modern PCs, there will be avx instructions inside opus.
However, HoloLens does not have avx instructions and this leads the Unity app to crash in HoloLens leaving an "illegal instruction" error.
To fix this, Opus should be built from its source code.
Since automation would become very hectic, currently, while I use CMake to build Opus, I just switch the /arch flag from /arch:AVX to /arch:SSE2 (HoloLens has SSE2) before building the solution.
While this may cause some performance loss, since Opus does not require that much computation for a PC, I will just use the SSE2 version also for both x64, which is for PCs, and x86, which is for HoloLens.

## Floor Detection
Microsoft provides floor detection code as a sample: https://github.com/microsoft/Azure-Kinect-Samples/tree/master/body-tracking-samples/floor_detector_sample. The files needed to use the functionality are copied inside cpp/azure-kinect-samples.

# How to Use
## From the Kinect-side
1. Download kinect-sender-v0.3.
2. Run KinectSender.exe.
3. Allow traffic from both private and public networks.

## From the HoloLens-side
1. Download kinect-viewer-v0.3.
2. Install the app package to your HoloLens (see https://www.microsoft.com/en-us/p/microsoft-hololens/9nblggh4qwnx) and run the app Kinect Viewer.
3. Connect to the HoloLens through the Windows Device Portal.
4. Enter IP address to the KinectSender.exe and press enter through the portal.
5. Place your HoloLens beneth your Kinect on the floor and press space through the portal.
6. Use arrow keys to adjust the position of the scene and press d to remove assisting visuals for the setup.

# To Cite
Jun, H., Bailenson, J.N., Fuchs, H., & Wetzstein, G. (2020). An Easy-to-use Pipeline for an RGBD Camera and an AR Headset. *PRESENCE: Teleoperators and Virtual Environments*.
