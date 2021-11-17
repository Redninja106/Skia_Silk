// naive example for using SkiaSharp with Silk.NET
// should open a window and draw a blue square

using Silk.NET.Windowing;
using SkiaSharp;

IWindow window;

// color format of our window
SKColorType format = SKColorType.Rgba8888;
// skia backend stuff
GRGlInterface skiaGlInterface = null;
GRContext skiaBackendContext = null;
GRBackendRenderTarget skiaBackendRenderTarget = null;
// basic skia stuff
SKSurface surface = null; // this surface is acually our window's buffer
SKCanvas canvas = null;
// for drawing
SKPaint paint = null;

// create window and listen for events.
window = Window.Create(WindowOptions.Default);

window.Load += () =>
{
    skiaGlInterface = GRGlInterface.CreateOpenGl(name =>
    {
        if (window.GLContext.TryGetProcAddress(name, out nint fn))
            return fn;
        return (nint)0;
    });

    skiaBackendContext = GRContext.CreateGl(skiaGlInterface);

    format = SKColorType.Rgba8888;

    // create a skia backend render target for the window.
    skiaBackendRenderTarget = new GRBackendRenderTarget(
        window.Size.X, window.Size.Y, // window size
        window.Samples ?? 1, window.PreferredStencilBufferBits ?? 16, // 
        new GRGlFramebufferInfo(
            0, // use the window's framebuffer
            format.ToGlSizedFormat()
            )
        );

    // create a surface from the render target
    surface = SKSurface.Create(skiaBackendContext, skiaBackendRenderTarget, format);

    canvas = surface.Canvas;

    paint = new SKPaint();
    paint.Color = new SKColor(0, 0, 255);
};

window.Render += deltaTime =>
{
    // draw a basic rectangle

    canvas.Clear(new SKColor(255, 0, 0));
    canvas.DrawRect(100, 100, 100, 100, paint);
    canvas.Flush(); // wait for commands to finish
};

window.Run();

// clean up resources
canvas.Dispose();
surface.Dispose();
skiaBackendRenderTarget.Dispose();
skiaBackendContext.Dispose();
skiaGlInterface.Dispose();
window.Dispose();