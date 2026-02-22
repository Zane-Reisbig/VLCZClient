# WINFORMS-VLCClient

A Windows Forms video player built with LibVLCSharp.

## Features

- Video Playback using VLC's libvlc backend
- Playback Controls (play/pause, mute, timeline seeking)
- Mouse Wheel scrubbing through video
- Watch History with timestamp persistence (resume where you left off)
- Skip Intro (after you mark it)
- Auto Play next in folder
- Alt+F9 will toggle Play/Pause
- F11 to toggle Fullscreen
- Embedded and Custom subtitles accessed via right-click menu.<br/>
    Additionaly, subtitles can be added by dragging and dropping
<br/>
<img src="./viewerplayerpreview.png" alt="Image of Two Windows, A 'Player' with controls and a 'Viewer' where media is displayed.">

## Requirements

- .NET 9.0 (Windows)
- Windows OS

## Dependencies

- [LibVLCSharp](https://github.com/videolan/libvlcsharp) (3.9.5)
- [LibVLCSharp.WinForms](https://github.com/videolan/libvlcsharp) (3.9.5)
- [VideoLAN.LibVLC.Windows](https://www.nuget.org/packages/VideoLAN.LibVLC.Windows) (3.0.23)
