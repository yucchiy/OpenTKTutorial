using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SkiaSharp;

namespace OpenTKTutorial
{
    public class Texture2D : System.IDisposable
    {
        public OpenGLTexture GLTexture { get; }
        public int Width { get; }
        public int Height { get; }
        public int Level { get; }

        public Texture2D(string path)
        {
            GLTexture = new OpenGLTexture(1, TextureTarget.Texture2D);

            var image = SKBitmap.FromImage(SKImage.FromEncodedData(Utility.GetEmbeddedResourceStream(path)));
            Utility.Assert(image != null, $"Invalid image path {path}");

            Width = image.Width;
            Height = image.Height;
            Level = 0;

            var data = new List<byte>(Width * Height * 3);
            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var pixel = image.GetPixel(x, y);
                    data.Add(pixel.Red);
                    data.Add(pixel.Green);
                    data.Add(pixel.Blue);
                }
            }

            GLTexture.SetTexture2D(0, Width, Height, Level, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte, data.ToArray());

            GLTexture.SetParameter(0, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GLTexture.SetParameter(0, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GLTexture.SetParameter(0, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GLTexture.SetParameter(0, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public void Bind()
        {
            GLTexture.Bind(0);
        }

        public void Unbind()
        {
            GLTexture.Unbind();
        }

        public void Dispose()
        {
            GLTexture.Dispose();
        }
    }
}