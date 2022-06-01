﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchReceiver
{
    #region Overview

    /// *************************
    /// * Overview              *
    /// *************************
    /// The purpose of Input.cs is to contain struct definitions
    /// for all of the structs used by the SendInput method found
    /// in the Mouse.cs file
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput

    #endregion

    #region Definitions
    /// <summary>
    /// Struct representation of keyboard input information.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-keybdinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Struct representation of mouse input information.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventF dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Struct representation of hardware input information.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-hardwareinput
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    /// <summary>
    /// Struct representation of the union between keyboard, mouse, and hardware input
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MouseInput mi;
        [FieldOffset(0)] public KeyboardInput ki;
        [FieldOffset(0)] public HardwareInput hi;
    }

    /// <summary>
    /// Struct representation of synthesized input events.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        public int Type;
        public InputUnion Union;
    }

    /// <summary>
    /// Enum for the different types of input events.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
    /// </summary>
    [Flags]
    public enum InputType
    {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2
    }

    /// <summary>
    /// Enum of all the Key Event values.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-keybdinput
    /// </summary>
    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }

    /// <summary>
    /// Enum of all of the Mouse Event values.
    /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
    /// </summary>
    [Flags]
    public enum MouseEventF : uint
    {
        Absolute = 0x8000,
        HWheel = 0x01000,
        Move = 0x0001,
        MoveNoCoalesce = 0x2000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        VirtualDesk = 0x4000,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100
    }
    #endregion
}
