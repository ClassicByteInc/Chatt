using System;
using System.Text;
using System.Runtime.InteropServices;


namespace Chatt.Client;

internal abstract class MouseWheelListener
{
    public abstract int GetWheelDelta();
    public static MouseWheelListener Create()
    {
        var os = Environment.OSVersion.Platform;
        if (os == PlatformID.Win32NT)
            return new WindowsMouseWheelListener();
        else if (os == PlatformID.Unix)
        {
            // macOS Ҳ�� Unix
            if (IsMacOS())
                return new MacOSMouseWheelListener();
            else
                return new LinuxMouseWheelListener();
        }
        else
            return new DummyMouseWheelListener();
    }

    private static bool IsMacOS()
    {
        // .NET 6+ �Ƽ��� OperatingSystem.IsMacOS()��������ݾ�д��
        string osDesc = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        return osDesc.Contains("Darwin");
    }
}

// Windows ʵ��

internal class WindowsMouseWheelListener : MouseWheelListener
{
    const int STD_INPUT_HANDLE = -10;
    const ushort MOUSE_EVENT = 0x0002;
    const uint MOUSE_WHEELED = 0x0004;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool ReadConsoleInput(
        IntPtr hConsoleInput,
        [Out] INPUT_RECORD[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);

    [StructLayout(LayoutKind.Explicit)]
    struct INPUT_RECORD
    {
        [FieldOffset(0)]
        public ushort EventType;
        [FieldOffset(4)]
        public MOUSE_EVENT_RECORD MouseEvent;
    }

    struct MOUSE_EVENT_RECORD
    {
        public COORD dwMousePosition;
        public uint dwButtonState;
        public uint dwControlKeyState;
        public uint dwEventFlags;
    }

    struct COORD
    {
        public short X;
        public short Y;
    }

    public override int GetWheelDelta()
    {
        IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
        INPUT_RECORD[] records = new INPUT_RECORD[1];
        if (ReadConsoleInput(handle, records, 1, out uint read) && read > 0)
        {
            var rec = records[0];
            if (rec.EventType == MOUSE_EVENT && rec.MouseEvent.dwEventFlags == MOUSE_WHEELED)
            {
                return (short)((rec.MouseEvent.dwButtonState >> 16) & 0xffff);
            }
        }
        return 0;
    }
}

// Linux ʵ�֣�α����/��ʾ������ncurses��������⣩
internal class LinuxMouseWheelListener : MouseWheelListener
{
    public override int GetWheelDelta()
    {
        // ������ Mono.Terminal/Terminal.Gui �� P/Invoke ncurses
        // ����ֻ����0��ʵ����Ŀ���õ�������ʵ��
        return 0;
    }
}

// macOS ʵ�֣�ͬLinux��
internal class MacOSMouseWheelListener : MouseWheelListener
{
    public override int GetWheelDelta()
    {
        // ������ Mono.Terminal/Terminal.Gui �� P/Invoke ncurses
        // ����ֻ����0��ʵ����Ŀ���õ�������ʵ��
        return 0;
    }
}

// Ĭ��ʵ�֣���֧�������֣�
internal class DummyMouseWheelListener : MouseWheelListener
{
    public override int GetWheelDelta() => 0;
}