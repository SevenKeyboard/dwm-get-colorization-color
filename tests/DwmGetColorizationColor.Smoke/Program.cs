using System;
using System.Runtime.InteropServices;
using SevenKeyboard;

internal static partial class Program
{
    private const uint Guard = 0xC13FA9A9;

    private static int Main()
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("SKIP: Windows is required.");
            return 2;
        }

        try
        {
            // Bracket the public call so a changing theme does not produce a false failure.
            for (int attempt = 0; attempt < 5; attempt++)
            {
                var before = ReadNative();
                uint color = 0xDEADBEEF;
                bool opaque = true;
                bool success = DwmGetColorizationColor.TryGet(out color, out opaque);
                var after = ReadNative();

                if (!success)
                {
                    Check(color == 0 && !opaque, "Failure must reset both seeded outputs.");
                }

                if (before != after)
                {
                    continue;
                }

                Check(success == (before.HResult == 0), "Success must match native S_OK.");
                if (!success)
                {
                    Console.WriteLine($"SKIP: DWM returned 0x{before.HResult:X8}; failure defaults passed.");
                    return 2;
                }

                Check((color >> 24) == (before.Color >> 24), "The native alpha byte must be preserved.");
                Check(color == before.Color, "All ARGB bits must match the native DWORD.");
                Check(opaque == (before.Opaque != 0), "Opaque blend must match the four-byte native BOOL.");
                Console.WriteLine($"PASS: DWORD/BOOL ABI; ARGB=0x{color:X8}, alpha={color >> 24}, opaque={opaque}.");
                return 0;
            }

            Console.WriteLine("SKIP: Colorization changed throughout the bounded comparison.");
            return 2;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"FAIL: {exception.Message}");
            return 1;
        }
    }

    private static unsafe (int HResult, uint Color, int Opaque) ReadNative()
    {
        // Independently probe two four-byte native outputs, each surrounded by guards.
        uint* buffer = stackalloc uint[6] { Guard, 0xA5A5A5A5, Guard, Guard, 0xA5A5A5A5, Guard };
        int hr = QueryNative(buffer + 1, (int*)(buffer + 4));
        Check(buffer[0] == Guard && buffer[2] == Guard && buffer[3] == Guard && buffer[5] == Guard,
            "Native DWORD/BOOL writes must stay within their four-byte slots.");

        if (hr != 0)
        {
            return (hr, 0, 0);
        }

        return (hr, buffer[1], unchecked((int)buffer[4]));
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    [LibraryImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor")]
    private static unsafe partial int QueryNative(uint* colorization, int* opaqueBlend);
}