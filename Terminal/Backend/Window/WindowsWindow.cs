using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace OxDED.Terminal.Backend.Window;

internal static class Utils {
    internal static Stream GetStream(nint handle) {
        SafeFileHandle fileHandle = new(handle, false);
        FileStream stream = new(fileHandle, FileAccess.ReadWrite);
        return stream;
    }
}

internal static partial class WinAPI {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct KEY_EVENT_RECORD {
        [MarshalAs(UnmanagedType.Bool)]
        internal bool bKeyDown;
        internal ushort wRepeatCount;
        internal ushort wVirtualKeyCode;
        internal ushort wVirtualScanCode;
        private ushort _uChar;
        internal uint dwControlKeyState;
        internal readonly char uChar => (char)_uChar;
    }
    [Flags]
    internal enum ControlKeyState {
        RightAltPressed = 0x0001,
        LeftAltPressed = 0x0002,
        RightCtrlPressed = 0x0004,
        LeftCtrlPressed = 0x0008,
        ShiftPressed = 0x0010,
        NumLockOn = 0x0020,
        ScrollLockOn = 0x0040,
        CapsLockOn = 0x0080,
        EnhancedKey = 0x0100
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct INPUT_RECORD {
        internal ushort EventType;
        internal KEY_EVENT_RECORD keyEvent;
    }
    internal const string ConsoleIn = "CONIN$";
    internal const string ConsoleOut = "CONOUT$";
    internal const string ConsoleError = ConsoleOut;
    internal const int STD_OUTPUT_HANDLE = -11;
    internal const int STD_INPUT_HANDLE = -10;
    internal const int STD_ERROR_HANDLE = -12;

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AllocConsole();
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool FreeConsole();
    [LibraryImport("kernel32.dll")]
    internal static partial nint GetConsoleWindow();
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial nint GetStdHandle(int nStdHandle);
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(nint handle);
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetStdHandle(int nStdHandle, nint hConsoleOutput);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern nint CreateFile(string fileName, uint desiredAccess, int shareMode, nint securityAttributes, int creationDisposition, int flagsAndAttributes, nint templateFile);
    [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetConsoleTitle(string title);
    [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = true, CharSet = CharSet.Auto)]
    internal static extern uint GetConsoleTitle([MarshalAs(UnmanagedType.LPTStr)]out string lpConsoleTitle, uint nSize);
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReadConsoleInput(nint hConsoleInput, out INPUT_RECORD lpBuffer, int nLength, out int numEventsRead);
}

/// <summary>
/// A wrapper for <see cref="TerminalWindow"/> on Windows.
/// </summary>
public class WindowsWindow : TerminalWindow {

    private nint consoleOut;
    private nint consoleIn;
    private nint consoleErr;

    private bool IsDisposed;
    /// <summary>
    /// Creates a new Windows terminal window.
    /// </summary>
    /// <param name="title">The name of the window.</param>
    /// <exception cref="Win32Exception"></exception>
    public WindowsWindow(string title) {
        nint stdOut = WinAPI.GetStdHandle(WinAPI.STD_OUTPUT_HANDLE);
        nint stdIn = WinAPI.GetStdHandle(WinAPI.STD_INPUT_HANDLE);
        nint stdErr = WinAPI.GetStdHandle(WinAPI.STD_ERROR_HANDLE);

        // TODO: Implement more...

        if (WinAPI.GetConsoleWindow() != nint.Zero) {
            if (!WinAPI.FreeConsole()) {
                throw new Win32Exception("Failed to free the console window: "+Marshal.GetLastWin32Error());
            }
        }
        if (!WinAPI.AllocConsole()) {
            throw new Win32Exception("Failed to allocate a console: "+Marshal.GetLastWin32Error());
        }

        consoleOut = WinAPI.CreateFile(WinAPI.ConsoleOut, 0x80000000 | 0x40000000, 2, nint.Zero, 3, 0, nint.Zero);
        consoleIn = WinAPI.CreateFile(WinAPI.ConsoleIn, 0x80000000 | 0x40000000, 1, nint.Zero, 3, 0, nint.Zero);
        consoleErr = WinAPI.CreateFile(WinAPI.ConsoleError, 0x80000000 | 0x40000000, 2, nint.Zero, 3, 0, nint.Zero);

        if (!WinAPI.SetStdHandle(WinAPI.STD_OUTPUT_HANDLE, consoleOut)) {
            throw new Win32Exception("Failed to set the handle for the console out stream: "+Marshal.GetLastWin32Error());
        }

        if (!WinAPI.SetStdHandle(WinAPI.STD_INPUT_HANDLE, consoleIn)) {
            throw new Win32Exception("Failed to set the handle for the console in stream: "+Marshal.GetLastWin32Error());
        }
            
        if (!WinAPI.SetStdHandle(WinAPI.STD_ERROR_HANDLE, consoleErr)) {
            throw new Win32Exception("Failed to set the handle for the console error stream: "+Marshal.GetLastWin32Error());
        }

        if (!WinAPI.CloseHandle(stdOut)) {
            throw new Win32Exception("Failed to close the handle of stdOut: "+Marshal.GetLastWin32Error());
        }
        if (!WinAPI.CloseHandle(stdIn)) {
            throw new Win32Exception("Failed to close the handle of stdIn: "+Marshal.GetLastWin32Error());
        }
        if (!WinAPI.CloseHandle(stdErr)) {
            throw new Win32Exception("Failed to close the handle of stdErr: "+Marshal.GetLastWin32Error());
        }

        outStream = new StreamWriter(Utils.GetStream(consoleOut), Encoding.UTF8);
        inStream = new StreamReader(Utils.GetStream(consoleIn), Encoding.UTF8);
        errStream = new StreamWriter(Utils.GetStream(consoleErr), Encoding.UTF8);

        Title = title;
    }
    private StreamWriter outStream;
    private StreamReader inStream;
    private StreamWriter errStream;

    // /// <inheritdoc/>
    // public override Encoding InEncoding { get => inStream.CurrentEncoding; set { inStream = new StreamReader(Utils.GetStream(consoleIn), value); } }
    // /// <inheritdoc/>
    // public override Encoding OutEncoding { get => outStream.Encoding; set { outStream = new StreamWriter(Utils.GetStream(consoleOut), value); errStream = new StreamWriter(Utils.GetStream(consoleErr), value); } }
    
    /// <inheritdoc/>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <remarks>Gets the first 300 chars of the title.</remarks>
    public override string Title {
        get {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            _ = WinAPI.GetConsoleTitle(out string title, 300);
            return title;
        }
        set {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (!WinAPI.SetConsoleTitle(value)) {
                throw new Win32Exception("Failed to set the title: "+Marshal.GetLastWin32Error());
            }
        }
    }

    private bool isCursorHidden = false;

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException"></exception>
    public override bool HideCursor { 
        set {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (value) {
                outStream.Write(ANSI.CursorInvisible);
            } else {
                outStream.Write(ANSI.CursorVisible);
            }
            isCursorHidden = value;
        } get {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            return isCursorHidden;
        }
    }

    /// <inheritdoc/>
    public override TextReader StandardInput => inStream;
    /// <inheritdoc/>
    public override TextWriter StandardOutput => outStream;
    /// <inheritdoc/>
    public override TextWriter StandardError => errStream;

    /// <inheritdoc/>
    public override Encoding InputEncoding { get => inStream.CurrentEncoding; set { inStream = new StreamReader(Utils.GetStream(consoleIn), value); } }
    /// <inheritdoc/>
    public override Encoding OutputEncoding { get => outStream.Encoding; set { outStream = new StreamWriter(Utils.GetStream(consoleOut), value); } }
    /// <inheritdoc/>
    public override Encoding ErrorEncoding { get => errStream.Encoding; set => errStream = new StreamWriter(Utils.GetStream(consoleErr), value); }
    /// <inheritdoc/>
    public override (int x, int y) CursorPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    
    /// <inheritdoc/>
    public override bool BlockCancelKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    
    /// <inheritdoc/>
    public override (uint Width, uint Height) Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <inheritdoc/>
    /// <exception cref="Win32Exception"></exception>
    public override void Dispose() {
        if (IsDisposed) { return; }

        if (!WinAPI.CloseHandle(consoleOut)) {
            throw new Win32Exception("Failed to close console out: "+Marshal.GetLastWin32Error());
        }
        consoleOut = nint.Zero;
        if (!WinAPI.CloseHandle(consoleIn)) {
            throw new Win32Exception("Failed to close console in: "+Marshal.GetLastWin32Error());
        }
        consoleIn = nint.Zero;
        if (!WinAPI.CloseHandle(consoleErr)) {
            throw new Win32Exception("Failed to close console err: "+Marshal.GetLastWin32Error());
        }
        consoleErr = nint.Zero;

        if (!WinAPI.FreeConsole()) {
            throw new Win32Exception("Failed to free the console window: "+Marshal.GetLastWin32Error());
        }
        Console.SetError(new StreamWriter(Console.OpenStandardError()));
        Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
    /// <inheritdoc/>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public override void WaitForKeyPress() {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (!WinAPI.ReadConsoleInput(consoleIn, out _, 1, out _)) {
            throw new Win32Exception("Failed to read console inputs: "+Marshal.GetLastWin32Error());
        }
    }

    /// <inheritdoc/>
    /// <exception cref="Win32Exception"></exception>
    /// <exception cref="ObjectDisposedException"></exception>
    public override bool ReadKey(out ConsoleKey key, out char keyChar, out bool alt, out bool shift, out bool control) {
        key = default;
        keyChar = default;
        alt = default;
        shift = default;
        control = default;

        ObjectDisposedException.ThrowIf(IsDisposed, this);
        
        if (!WinAPI.ReadConsoleInput(consoleIn, out WinAPI.INPUT_RECORD ev, 1, out int eventsRead)) {
            throw new Win32Exception("Failed to read console inputs: "+Marshal.GetLastWin32Error());
        }

        bool isKeyDown = ev.EventType == 0x0001 && ev.keyEvent.bKeyDown != false;
        char ch = ev.keyEvent.uChar;
        ushort keyCode = ev.keyEvent.wVirtualKeyCode;

        if (!isKeyDown) {
            if (keyCode != 0x12)
                return false;
        }
        if (ch == 0) {
            if ((keyCode >= 0x10 && keyCode <= 0x12) || keyCode == 0x14 || keyCode == 0x90 || keyCode == 0x91)
                return false;
        }
        WinAPI.ControlKeyState state = (WinAPI.ControlKeyState)ev.keyEvent.dwControlKeyState;
        key = (ConsoleKey)keyCode;
        keyChar = ch;
        shift = (state & WinAPI.ControlKeyState.ShiftPressed) != 0;
        alt = (state & (WinAPI.ControlKeyState.LeftAltPressed | WinAPI.ControlKeyState.RightAltPressed)) != 0;
        control = (state & (WinAPI.ControlKeyState.LeftCtrlPressed | WinAPI.ControlKeyState.RightCtrlPressed)) != 0;
        return true;
    }
    /// <inheritdoc/>
    public override (int x, int y) GetCursorPosition() {
        return Console.GetCursorPosition();
    }
}