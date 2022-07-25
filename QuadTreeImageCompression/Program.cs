// See https://aka.ms/new-console-template for more information

using CommandLine;
using QuadTreeImageCompression;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;



var parserResult = Parser.Default.ParseArguments<Options>(args);
string imagePath = "";
float radius = 0f;
string outputPath = "";
bool showTreeGrid = false;
int depth = 0;
bool parsed = false;
Rgba32 gridColor = new Rgba32();

parserResult.WithParsed((options) =>
{
    imagePath = options.PathToFile;
    radius = options.ColorRadius;
    outputPath = options.OutputPath;
    showTreeGrid = options.ShowGrid;
    depth = options.MaxDepth;
    if (Rgba32.TryParseHex(options.GridColor, out gridColor))
    {
        parsed = true;
    }
    else
    {
        parsed = false;
        Console.WriteLine("Grid color is not in the right format (example: ffffff for color white)");
    }
    if (File.Exists(imagePath))
    {
        parsed = true;
    }
    else
    {
        parsed = false;
        Console.WriteLine("File does not exist");
    }
});
parserResult.WithNotParsed((errors) =>
{
    foreach (var error in errors)  
    {
        Console.WriteLine(error);
    }
});
if (parsed)
{
    Image<Rgba32> im = Image.Load<Rgba32>(imagePath);
    ColorQuadTree tree1 = new ColorQuadTree(radius, new Rectangle(0, 0, im.Width, im.Height), depth, showTreeGrid, gridColor);
    Image<Rgba32> output = new Image<Rgba32>(im.Width, im.Height);
    tree1.Process(im, ref output);
    output.Save(outputPath,
        new PngEncoder() {CompressionLevel = PngCompressionLevel.Level9});
}


class Options
{
    [Value(0, MetaName = "input_path", MetaValue = "string", HelpText = "Path to your input image")]
    public string PathToFile { get; set; }
    
    [Option('r', "radius", Default = 100, HelpText = "Radius of the color sphere in which colors are considered \"same\"")]
    public float ColorRadius { get; set; }
    
    [Option('g', "show-grid", Default = false, HelpText = "Show quadtree grid")]
    public bool ShowGrid { get; set; }
    
    [Option('d', "max-depth", Default = 0, HelpText = "Max quadtree depth (pass 0 for maximum possible depth)")]
    public int MaxDepth { get; set; }
    
    [Option('c', "grid-color", Default = "#ffffffff", HelpText = "Quadtree grid color (in hex)")]
    public string GridColor { get; set; }
    
    [Value(1, MetaName = "output_path", MetaValue = "string", HelpText = "Path to output image (will be created if does not exist)")]
    public string OutputPath { get; set; }
}


    
