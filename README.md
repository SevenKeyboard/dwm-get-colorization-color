# dwm-get-colorization-color

Reads the Windows DWM colorization color and opaque blend flag for .NET 10.

```csharp
using SevenKeyboard;
if (DwmGetColorizationColor.TryGet(out uint colorization, out bool opaqueBlend))
    Console.WriteLine($"0x{colorization:X8}; opaque blend: {opaqueBlend}");
```

The color is numeric `0xAARRGGBB`, including alpha, rather than formatted text.
Use the outputs only on success. Failure returns `false` and resets both outputs.
