using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchReceiver
{
    /// <summary>
    /// Enum for a left or right mouse button press.
    /// </summary>
    public enum MouseButton
    {
        Left,
        Right,
    }

    /// <summary>
    /// The Mouse class's purpose is to abstract sending input to the mouse input stream.
    /// https://www.codeproject.com/Articles/5264831/How-to-Send-Inputs-using-Csharp
    /// </summary>
    public static class Mouse
    {
        /// <summary>
        /// Moves the mouse to the specified locations and clicks the specified mouse button.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="x">Position along the x axis to move the mouse to</param>
        /// <param name="y">Position along the y axis to move the mouse to</param>
        public static void Click(MouseButton button)
        {
            Input[] inputs = new Input[] {
                new Input
                {
                    Type = (int) InputType.Mouse,
                    Union = new InputUnion
                    {
                        mi = new MouseInput
                        {
                            dwFlags = (button == MouseButton.Left ? MouseEventF.LeftDown : MouseEventF.RightDown),
                            mouseData = 0
                        }
                    }
                },
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        /// <summary>
        /// Releases the specified mouse button
        /// </summary>
        /// <param name="button">Mouse button to release</param>
        public static void Release(MouseButton button)
        {
            Input[] inputs = new Input[] {
                new Input
                {
                    Type = (int) InputType.Mouse,
                    Union = new InputUnion
                    {
                        mi = new MouseInput
                        {
                            dwFlags = (button == MouseButton.Left ? MouseEventF.LeftUp : MouseEventF.RightUp)
                        }
                    }
                }
            };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        /// <summary>
        /// Moves the mouse to the specified x/y coordinate
        /// </summary>
        /// <param name="x">Position along the x axis to move the mouse to</param>
        /// <param name="y">Position along the y axis to move the mouse to</param>
        public static void Move(int x, int y)
        {
            SetCursorPos(x, y);
        }

        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
        /// </summary>
        /// <param name="numInputs">Number of structures in the inputs array</param>
        /// <param name="inputs">Array of Input structs that represent an event to be inserted into the keyboard/mouse input stream</param>
        /// <param name="size">Size in bytes of the INPUT struct</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numInputs, Input[] inputs, int size);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetCursorPos(int x, int y);


        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        // For your convenience
        public static Point GetCursorPosition()
        {
            PointInter lpPoint;
            GetCursorPos(out lpPoint);
            return (Point)lpPoint;
        }
    }
}
