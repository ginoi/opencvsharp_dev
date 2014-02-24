opencvsharp_dev
===============

feasibility and development getting use out of opencvsharp


Setup and requirement
=====================

1) Install Visual Studio 2012 for Windows Desktop (Express can be used)

2) Create Project 
- Create New Project --> Templates --> Visual C# --> Windows -> Console Application.

3) Install libs for opencv C#
- Right click on Solution at solution explorer window, and choose Manage NuGet Packages ..
- Seach OpenCVSharp at online. Choose OpenCVSharp x86 (for 32bit) or x64 (64bit).
- Install will be done automatically can can see some .dlls at your project.
- Evaluated with OpenCVSharp 2.4.8 

4) Replace .cs here with Program.cs in the solution.

5) Choose x86 in Any CPU cell if you choose OpenCVSharp x86

6) Check "Allow unsafe code" in Project --> Properties --> Build.

7) F7 to build and check if there is no error.

8) F5 to see the demo.
