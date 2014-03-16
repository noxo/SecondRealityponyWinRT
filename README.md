SecondRealityponyWinRT
======================
Small experiment of running Eric Mooneys remake of Second Reality on Windows Runtime (WinRT).

Uses Monogame (& SharpDX) as graphics backend.

Runs nicely on decent x86 hardware. Plays also on 1gen Surface RT ARM tablet, excluding two scenes,
and few graphics glitches.

Should compile nicely with Visual Studio 2012 Express for Windows 8. XNA Content is included
precompiled with project.

Main changes:
=============
+ Microsoft.XNA framework -> Monogame XNA framework
+ Xna.VideoPlayer -> Windows.UI.Xaml.Controls.MediaElement 
+ Monogame XNA Content Pipeline fight
+ System.Threading.Thread -> System.Threading.Tasks.Task

Original project is here: https://github.com/erikmooney/SecondRealipony.
