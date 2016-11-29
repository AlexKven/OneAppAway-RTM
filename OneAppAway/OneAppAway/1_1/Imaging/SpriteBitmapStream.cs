using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using static System.Math;

namespace OneAppAway._1_1.Imaging
{
    public class SpriteBitmapStream : System.IO.Stream
    {
        private SpriteBase Sprite;
        private byte[] WidthB = new byte[4];
        private byte[] HeightB = new byte[4];
        private byte[] SizeB = new byte[4];
        private int NumPixels;

        private static void IntToBytes(int num, byte[] bytes)
        {
            for (int i = 0; i < 4; i++)
            {
                bytes[i] = (byte)(num % 256);
                num /= 256;
            }
        }

        public SpriteBitmapStream(SpriteBase sprite)
        {
            Sprite = sprite;
            IntToBytes(sprite.PixelWidth, WidthB);
            IntToBytes(sprite.PixelHeight, HeightB);
            _Length = Sprite.PixelWidth * Sprite.PixelHeight * 4 + 54;
            IntToBytes((int)Length, SizeB);
            NumPixels = sprite.PixelWidth * sprite.PixelHeight;
        }

        private byte GetByte(int index)
        {
            if (index < 54)
            {
                switch (index)
                {
                    case 0:
                        return 66;
                    case 1:
                        return 77;
                    case 2:
                        return SizeB[0];
                    case 3:
                        return SizeB[1];
                    case 4:
                        return SizeB[2];
                    case 5:
                        return SizeB[3];
                    case 10:
                        return 54;
                    case 14:
                        return 40;
                    case 18:
                        return WidthB[0];
                    case 19:
                        return WidthB[1];
                    case 20:
                        return WidthB[2];
                    case 21:
                        return WidthB[3];
                    case 22:
                        return HeightB[0];
                    case 23:
                        return HeightB[1];
                    case 24:
                        return HeightB[2];
                    case 25:
                        return HeightB[3];
                    case 26:
                        return 1;
                    case 28:
                        return 32;
                    case 38:
                        return 196;
                    case 39:
                        return 14;
                    case 42:
                        return 196;
                    case 43:
                        return 14;
                    default:
                        return 0;
                }
            }
            else
            {
                index -= 54;
                int comp = index % 4;
                int pixel = index / 4;
                int normalOrder = NumPixels - pixel - 1;
                int x = normalOrder % Sprite.PixelWidth;
                int y = normalOrder / Sprite.PixelWidth;
                var color = Sprite.Render(x, y);
                switch (comp)
                {
                    case 0:
                        return color.R;
                    case 1:
                        return color.G;
                    case 2:
                        return color.B;
                    default:
                        return color.A;
                }
            }
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        private long _Length;
        public override long Length => _Length;

        public override long Position { get; set; }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position == Length)
                return 0;
            int maximum = (int)(Length - Position);
            int result = Max(maximum, count);
            for (int i = 0; i < result; i++)
            {
                buffer[i] = GetByte((int)(Position + i));
            }
            Position += result;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset - 1;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException("SpriteBitmapStream is read-only.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("SpriteBitmapStream is read-only.");
        }

        public Byte[] GetFullBuffer()
        {
            byte[] result = new byte[Length];
            Read(result, 0, (int)Length);
            return result;
        }
    }
}
