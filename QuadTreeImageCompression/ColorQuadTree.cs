using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QuadTreeImageCompression;

public class ColorQuadTree
{
    private HashSet<Rgba32> _colors;

    private static bool _showGrid;

    private static Rgba32 _gridColor;
    
    private static int _maxDepth;
    
    public float ColorRadius;
    public Rectangle Rect;
    public int Depth;
    
    public bool Subdivided;
    public ColorQuadTree First;
    public ColorQuadTree Second;
    public ColorQuadTree Third;
    public ColorQuadTree Fourth;

    
    
    // public Image<Rgba32> Image;

    public ColorQuadTree(float colorRadius, Rectangle rect, int maxDepth, bool showGrid, Rgba32 gridColor)
    {
        ColorRadius = colorRadius;
        Rect = rect;
        _showGrid = showGrid;
        _maxDepth = maxDepth;
        Depth = 0;
        _colors = new HashSet<Rgba32>();
        _gridColor = gridColor;
    }

    private ColorQuadTree(float colorRadius, Rectangle rect, int depth)
    {
        ColorRadius = colorRadius;
        Rect = rect;
        Depth = depth;
        _colors = new HashSet<Rgba32>();
    }

    public void Process(Image<Rgba32> image, ref Image<Rgba32> outputImage)
    {
        for (int i = Rect.X; i < Rect.X + Rect.Width; i++)
        {
            for (int j = Rect.Y; j < Rect.Y + Rect.Height; j++)
            {
                if (i >= outputImage.Width || j >= outputImage.Height) continue;
                var p = image[i, j];
                foreach (var clr in _colors)
                {
                    if (Math.Sqrt((clr.R - p.R) * (clr.R - p.R) + (clr.G - p.G) * (clr.G - p.G) + (clr.B - p.B) *
                            (clr.B - p.B)) > Math.Abs(ColorRadius))
                    {
                        if ((Depth <= _maxDepth || _maxDepth == 0) && (Subdivided || Subdivide()))
                        {
                            First.Process(image, ref outputImage);
                            Second.Process(image, ref outputImage);
                            Third.Process(image, ref outputImage);
                            Fourth.Process(image, ref outputImage);
                            return;
                        }

                        break;
                    }
                }

                _colors.Add(p);
            }
        }

        Rgba32 avgClr = new Rgba32();
        int r = 0, g = 0, b = 0;
        int count = 0;
        for (int i = Rect.X; i < Rect.X + Rect.Width; i++)
        {
            for (int j = Rect.Y; j < Rect.Y + Rect.Height; j++)
            {
                r += image[i, j].R;
                g += image[i, j].G;
                b += image[i, j].B;
                count++;
            }
        }
        
        avgClr.R = (byte) (r / count);
        avgClr.G = (byte) (g / count);
        avgClr.B = (byte) (b / count);
        avgClr.A = 255;
        for (int i = Rect.X; i < Rect.X + Rect.Width; i++)
        {
            for (int j = Rect.Y; j < Rect.Y + Rect.Height; j++)
            {
                if (_showGrid && (i == Rect.X || i == Rect.X + Rect.Width - 1 || j == Rect.Y || j ==
                        Rect.Y + Rect.Height - 1))
                {
                    outputImage[i, j] = _gridColor;
                }
                else
                {
                    outputImage[i, j] = avgClr;
                }
            }
        }
    }

    public bool Subdivide()
    {
        if (!(Rect.Width > 1 && Rect.Height > 1))
        {
            return false;
        }

        int width = (int)Math.Ceiling((float)Rect.Width / 2f);
        int height = (int) Math.Ceiling((float) Rect.Height / 2f);
        var firstRect = new Rectangle(Rect.X + Rect.Width / 2, Rect.Y, width, height);
        First = new ColorQuadTree(ColorRadius, firstRect, Depth + 1);        
        var secondRect = new Rectangle(Rect.X, Rect.Y, width, height);
        Second = new ColorQuadTree(ColorRadius, secondRect, Depth + 1);        
        var thirdRect = new Rectangle(Rect.X, Rect.Y + Rect.Height / 2, width, height);
        Third = new ColorQuadTree(ColorRadius, thirdRect, Depth + 1);        
        var fourthRect = new Rectangle(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height / 2, width, height);
        Fourth = new ColorQuadTree(ColorRadius, fourthRect, Depth + 1);
        Subdivided = true;
        return true;
    }
}