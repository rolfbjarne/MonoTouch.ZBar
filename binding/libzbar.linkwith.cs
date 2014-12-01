using System;
using ObjCRuntime;

[assembly: LinkWith ("libzbar.a", LinkTarget.ArmV7 | LinkTarget.Simulator | LinkTarget.Arm64 | LinkTarget.Simulator64, ForceLoad = true, Frameworks = "CoreGraphics AVFoundation CoreMedia CoreVideo QuartzCore", LinkerFlags = "-liconv")]
