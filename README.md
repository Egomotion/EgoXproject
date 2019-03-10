EgoXproject README
==================

## Overview
EgoXproject is an editor plugin for Unity. It automates Xcode project modification for iOS and tvOS projects where Unity does not provide support.

## Targeted Unity Versions
This code was last tested and released using:
* Unity 5.3.4
* Unity 2017.0

## Source Code
This is the source code used to build the EgoXproject dll (version 3.1.1) that is available on the Unity Asset Store.

As work on the project has now been discontinued, I am making the source code available to anyone who wants to continue using and updating EgoXproject.

The code is provided 'as is'. There is no support. No updates to the code will be made, and no merge requests will occur. To continue developing EgoXproject please fork your own version.

## What is in this repository

### EgoXprojectDLL
The source code and project files for building the DLL

### EgoXprojectUnity
This is a Unity project for testing the DLL during development. It also has aditional tools for working with the dll resources.
The Debug build gets copied here during the DLL build process

## Getting set up
* Download and install Unity. Any version from 5.3.4 onwards should be fine. 
* While you can configure EgoXproject on any platform with the Editor, you will need iOS or tvOS modules on macOS to be able build and deploy to these targets.
* Open the EgoXprojectDLL project in Visual Studio for Mac.
* Build the desired target (Release or Debug)
	* The builds can be found in EgoXprojectDLL/EgoXproject/bin
	* The Debug build also gets copied to EgoXprojectUnity
	* You can change this behaviour in the project build settings by adjusting the pre and post build commands

### Deterministic Builds
EgoXproject generates GUIDs for use in the Xcode projects. However, this makes unit testing hard. As such a hack is done that makes EgoXproject use deterministic GUIDS. 
* The Debug build has a special compiler flag, DETERMINISTIC, that makes the DLL try and load a list of GUIDs from Assets/guids.txt. If it is successful it will use this list until it runs out. Then it will return to using correctly generated ones.
* If the file cannot be loaded then EgoXproject generates GUIDs as normal
* Using the known set of GUIDs allows the unit tests to compare generated files against known good ones
* Remove this flag if you want Debug builds without this feature. The unit tests will fail though.

### Unity DLLs
EgoXproject needs to link against the Unity DLLs so it can draw its UI. As such the project needs to have references set to the DLLs from the correct Unity version.
* If you are using EgoXproject with Unity 5.x then you need to reference the DLLs in a Unity 5 install that is the lowest version you want to target
* Similaraly, for Unity 2017, and 2018 you need to link against those versions. There may be compatibility between these Unity DLLs allowing you to build against one version and run on another. But sometimes things change and cause things to break.
* The project is configured to look in `/Applications/Unity/` for the DLLs. The project can be configured to reference different DLLs for differnt builds
* The DLLs can be found in a Unity installation at `Unity.app/Contents/Frameworks/Managed/UnityEditor.dll` and
 `Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

### Testing
There are a suite of Unit tests that can be run in the Unity project using Unity's buit in Test Runner. A lot of the tests write out a project file that is compared to an existing file.

The tests are not exhaustive, more should be added. It was a work in progress.

## Design Philosophy
EgoXproject was born out of the frustration at the lack Xcode configurability in Unity 3, and the need for non trival and hacky scripting to edit the Xcode project files.

Solutions that did exist often had no deep understanding of the Xcode file format or regard to its contents. Often removing whole sections of the project file that they did not understand.

From these frustrations I created EgoXproject to be a UI based editor for manipulating Xcode projects that would aim to do no harm to the Xcode project files.

As such, changes to Xcode project files are inserted at the end of the files where possible, leaving everything else untouched. This also allows easy file comparisons if required. Any sections and values that are not understood are ignored and left untouched. This helps EgoXproject to keep working with new versions of Xcode that have new features it does not understand.

The Xcode project file format is a plist made up of dictionarys. EgoXproject keeps these dictionaries as found, and wraps them up in class implementations. This makes the code a bit more complex that loading into a defined structure, but it ensures that keys that are unknown are not lost, and that only entries it knows about are modified.

## Unfinished features
* Info.plist entries. This version of EgoXproject has a feature that allows users to easily find build settings, and insert them with the right type. A similar system was planned for the Info.plist entries. The common code framework is there, but the code specific to the Info.plist and its master list of keys was not completed.
* Anything introduced to Xcode since February 2018 has not been implemented in EgoXproject
* The UI code needs an overhaul and modernizing
* Internationalization support. This is something I wanted to do for a long time. EgoXproject should now be in a position to implment this feature.

## License
* EgoXproject is licensed under LGPL v3. If this causes you issues, please get in touch for an alternative licence.
* You may use EgoXproject in personal and commercial projects.
* As this is an editor tool that does not ship with your software it does not affect the licencing of your software or require you to release the EgoXproject source code with it.
* If you modify EgoXproject for use inside your organization (including 3rd parties, contractors etc), you do not need to publish those changes.
* You only need to publish changes if you distribute the DLL/source externally, such as part of another Unity plugin.
* You can include the DLL/source in free and paid Unity plugins or similarly distributed software. You only need to make the source available if you change it.
* You may not sell EgoXproject or modified versions on their own, or trade on the EgoXproject name. 
* If you have any questions regarding licensing please contact support@egomotion.co.uk

## Support
* There is no support of this software. It is released as is to allow others to fork it and continue improving it.
* If you need to get in touch please email support@egomotion.co.uk