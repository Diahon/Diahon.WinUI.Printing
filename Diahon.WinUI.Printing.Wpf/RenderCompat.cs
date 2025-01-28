using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Direct2D;
using Windows.Win32.Graphics.Direct2D.Common;
using Windows.Win32.Graphics.Direct3D;
using Windows.Win32.Graphics.Direct3D11;
using Windows.Win32.Graphics.Dxgi;
using Windows.Win32.Graphics.Dxgi.Common;

namespace Diahon.WinUI.Printing.Wpf;

internal sealed class RenderCompat
{
    readonly ID2D1Factory2 d2dFactory;
    readonly ID3D11Device d3dDevice;
    readonly ID2D1Device1 d2dDevice;
    readonly ID2D1DeviceContext1 deviceContext;
    public unsafe RenderCompat()
    {
        d2dFactory = CreateD2DFactory();

        PInvoke.D3D11CreateDevice(
            null,
            D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
            HMODULE.Null,
            D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT,
            null,
            0,
            PInvoke.D3D11_SDK_VERSION,
            out d3dDevice,
            null,
            out _
        ).ThrowOnFailure();

        d2dFactory.CreateDevice(
            (IDXGIDevice)d3dDevice,
            out d2dDevice
        ).ThrowOnFailure();

        d2dDevice.CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            out deviceContext
        ).ThrowOnFailure();

        Marshal.AddRef(Marshal.GetIUnknownForObject(deviceContext));

        static ID2D1Factory2 CreateD2DFactory()
        {
            D2D1_FACTORY_OPTIONS options = default;

            PInvoke.D2D1CreateFactory(
                D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED,
                typeof(ID2D1Factory2).GUID,
                options,
                out var factory
            ).ThrowOnFailure();

            return (ID2D1Factory2)factory;
        }
    }

    public unsafe IDXGISurface CreateSurface(uint width, uint height, double dpiX, double dpiY, byte[] buffer)
    {
        deviceContext.CreateBitmap(
            new D2D_SIZE_U() { width = width, height = height },
            sourceData: null,
            pitch: 0u,
            new D2D1_BITMAP_PROPERTIES1()
            {
                bitmapOptions = D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET,
                dpiX = (float)dpiX,
                dpiY = (float)dpiY,
                pixelFormat = new()
                {
                    alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED,
                    format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM
                }
            },
            out ID2D1Bitmap1 bitmap
        ).ThrowOnFailure();

        int stride = (int)(width * 32);
        fixed (byte* pBuffer = buffer)
        {
            bitmap.CopyFromMemory(dstRect: null, pBuffer, (uint)stride).ThrowOnFailure();
        }

        bitmap.GetSurface(
            out var surface
        ).ThrowOnFailure();

        return surface;
    }

    public static unsafe void Render(uint width, uint height, double dpiX, double dpiY, Visual page, byte[] buffer)
    {
        RenderTargetBitmap renderTarget = new(
           (int)width,
           (int)height,
           dpiX,
           dpiY,
           PixelFormats.Pbgra32
        );
        renderTarget.Render(page);

        fixed (byte* pBuffer = buffer)
        {
            renderTarget.CopyPixels(
                sourceRect: new(0, 0, (int)width, (int)height),
                (nint)pBuffer,
                buffer.Length,
                stride: (int)(width * 32)
            );
        }
    }
}
