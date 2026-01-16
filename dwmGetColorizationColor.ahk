#Requires AutoHotkey v1.1.17+
;==============================================================
; dwmGetColorizationColor — Gets the current DWM colorization color and opaque blend flag
;
; GitHub: https://github.com/SevenKeyboard/dwm-get-colorization-color
; Author: SevenKeyboard Ltd. (2026)
; License: The Unlicense
;
; Documentation / References:
;   DwmGetColorizationColor function (dwmapi.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor
;==============================================================

/*
    HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\DWM\ColorizationColor
*/

class VersionManager_dwmGetColorizationColor
{
    static _ := VersionManager_dwmGetColorizationColor._init()
    _init()    {
        global
        DWMGETCOLORIZATIONCOLOR_VERSION := "1.0.0"
    }
}
dwmGetColorizationColor(byRef colorization:="", byRef opaqueBlend:="")    {
    static S_OK:=0x00000000
    colorization:= opaqueBlend:= ""
    ,varSetCapacity(pcrColorization,4,0), varSetCapacity(pfOpaqueBlend,4,0)
    ,hr:=dllCall("Dwmapi.dll\DwmGetColorizationColor", "Ptr",&pcrColorization, "Ptr",&pfOpaqueBlend, "Int")
    if (bRet:=(hr==S_OK))
        colorization:=format("0x{:08X}",numGet(&pcrColorization,0,"UInt")), opaqueBlend:=numGet(&pfOpaqueBlend,0,"Int")
    return bRet
}