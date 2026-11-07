// Source: https://github.com/SevenKeyboard/dwm-get-colorization-color
// Author: SevenKeyboard Ltd. (2026)
// License: The Unlicense (see LICENSE)
// Reference: https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor

using System.Runtime.InteropServices;

namespace SevenKeyboard;

/// <summary>Queries the current DWM colorization color and opaque blend flag.</summary>
public static partial class DwmGetColorizationColor
{
    /// <summary>Returns true only when DWM returns S_OK (HRESULT zero).</summary>
    /// <param name="colorization">The numeric 0xAARRGGBB color, or zero on failure.</param>
    /// <param name="opaqueBlend">Whether the blend is opaque, or false on failure.</param>
    public static bool TryGet(out uint colorization, out bool opaqueBlend)
    {
        colorization = default;
        opaqueBlend = default;

        if (GetColorizationColor(out uint nativeColorization, out int nativeOpaqueBlend) != 0)
        {
            return false;
        }

        colorization = nativeColorization;
        opaqueBlend = nativeOpaqueBlend != 0;
        return true;
    }

    // Win32 BOOL occupies four bytes; convert it to a managed bool after success.
    [LibraryImport("dwmapi.dll", EntryPoint = "DwmGetColorizationColor")]
    private static partial int GetColorizationColor(out uint colorization, out int opaqueBlend);
}