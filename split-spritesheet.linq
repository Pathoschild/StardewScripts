<Query Kind="Program">
  <NuGetReference>ImageProcessor</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>ImageProcessor</Namespace>
  <Namespace>ImageProcessor.Imaging</Namespace>
  <Namespace>ImageProcessor.Imaging.Formats</Namespace>
</Query>

/*

Overview
-------------------------------------------------
This script splits one or more Stardew Valley spritesheets into individual sprite images,
optionally resizing each sprite and cropping whitespace.


Usage
-------------------------------------------------
To use this script:

1. Open this file in LINQPad. (It's a small app: https://www.linqpad.net/.)
2. Change the settings under 'configuration' below.
3. Click the execute (▶) button to export the sprites.

*/
/*********
** Configuration
*********/
/// <summary>The absolute path to the spritesheet image to split, or a folder containing any number of spritesheets with the same sprite size.</summary>
readonly string SpritesheetPath = @"C:\source\_Stardew\Stardew Valley\Content\Maps\springobjects.png";

/// <summary>The folder in which to export sprites.</summary>
readonly string OutputPath = @"C:\Users\patho\Downloads\spritesheets";

/// <summary>The pixel width/height of each sprite.</summary>
readonly Size SpriteSize = new Size(16, 16);

/// <summary>The zoom multiplication to apply.</summary>
readonly int Zoom = 4;

/// <summary>Whether to crop empty space around each sprite in the exported images.</summary>
readonly bool CropWhitespace = true;


/*********
** Script
*********/
/// <summary>Run the export.</summary>
void Main()
{
	foreach (var pair in this.GetFiles(this.SpritesheetPath))
	{
		string relativeName = pair.Key;
		FileInfo file = pair.Value;
		
		string exportPath = Path.Combine(this.OutputPath, relativeName);
		Directory.CreateDirectory(exportPath);

		int sprites = this.ExportSheet(file.FullName, this.SpriteSize.Width, this.SpriteSize.Height, exportPath);
		$"[{relativeName}] Exported {sprites} sprites to {exportPath}.".Dump();
	}
}

/// <summary>Get the files to split given a file or folder path.</summary>
/// <param name="path">The file or folder path to scan.</param>
private IDictionary<string, FileInfo> GetFiles(string path)
{
	var results = new Dictionary<string, FileInfo>();
	
	FileInfo rootFile = new FileInfo(path);
	DirectoryInfo rootDir = new DirectoryInfo(path);
	if (rootFile.Exists)
		results[Path.GetFileNameWithoutExtension(path)] = rootFile;
	else if (rootDir.Exists)
	{
		foreach (FileInfo file in rootDir.GetFiles("*.png", SearchOption.AllDirectories))
		{
			string relativeName = Path.GetRelativePath(rootDir.FullName, file.FullName);
			relativeName = Path.Combine(Path.GetDirectoryName(relativeName), Path.GetFileNameWithoutExtension(relativeName));
			
			results[relativeName] = file;
		}
	}

	return results;
}

/// <summary>Export the spritesheet with a fixed sprite size.</summary>
/// <param name="filePath">The path to the spritesheet to split.</param>
/// <param name="spriteWidth">The fixed sprite width.</param>
/// <param name="spriteHeight">The fixed sprite height.</param>
/// <param name="exportPath">The folder in which to export sprites.</param>
private int ExportSheet(string filePath, int spriteWidth, int spriteHeight, string exportPath)
{
    // load image
    using var sheetEditor = new ImageFactory().Load(filePath);
    Image sheet = sheetEditor.Image;

    // validate sprite size
    if (!this.IsDivisible(sheet.Width, spriteWidth))
    {
        this.LogError($"The spritesheet width ({sheet.Width}px) can't be evenly divided into sprites of width {spriteWidth}px.");
        return 0;
    }
    if (!this.IsDivisible(sheet.Height, spriteHeight))
    {
        this.LogError($"The spritesheet height ({sheet.Height}px) can't be evenly divided into sprites of height {spriteHeight}px.");
        return 0;
    }

    // split spritesheet
    int index = 0;
    int exported = 0;
    for (int y = 0; y < sheet.Height; y += spriteHeight)
    {
        for (int x = 0; x < sheet.Width; x += spriteWidth)
        {
            if (this.ExportSprite(sheet, x, y, spriteWidth, spriteHeight, exportPath, index++.ToString()))
                exported++;
        }
    }
    
    return exported;
}

/// <summary>Export a single sprite, unless it's empty.</summary>
/// <param name="sheet">The spritesheet image.<param>
/// <param name="x">The sprite's top-left X pixel position.</param>
/// <param name="y">The sprite's top-left Y pixel position.</param>
/// <parm name="width">The sprite's pixel width.</param>
/// <parm name="height">The sprite's pixel height.</param>
/// <param name="exportPath">The folder in which to export sprites.</param>
/// <param name="name">The sprite name for the filename, without extension.</param>
private bool ExportSprite(Image sheet, int x, int y, int width, int height, string exportPath, string name)
{
    using var editor = new ImageFactory()
        .Load(sheet)
        .Crop(new Rectangle(x, y, width, height))
        .ZoomSprite(this.Zoom)
        .CropWhitespace(enabled: this.CropWhitespace);

    if (this.IsEmpty(editor.Image))
        return false;

    editor.Image.Save(Path.Combine(exportPath, name + ".png"));
    return true;
}

/// <summary>Log a formatted error.</summary>
/// <param name="message">The message to log.</param>
private void LogError(string message)
{
    Util.WithStyle(message, "font-weight: bold; color: red;").Dump();
}

/// <summary>Get whether a given sheet dimension is evenly divisible by the given sprite size.</summary>
/// <param name="sheetSize">The spritesheet dimension.</param>
/// <param name="spriteSize">The sprite dimension.</param>
private bool IsDivisible(int sheetSize, decimal spriteSize)
{
    return (sheetSize / spriteSize) % 1 == 0;
}

/// <summary>Get whether an image consists entirely of transparent pixels.</summary>
/// <param name="image">The image to check.</param>
private bool IsEmpty(Image image)
{
    if (image is not Bitmap bitmap)
        throw new InvalidOperationException("Unhandled image format: must be loaded as a bitmap.");
    
    for (int x = 0; x < image.Width; x++)
    {
        for (int y = 0; y < image.Height; y++)
        {
            if (bitmap.GetPixel(x, y).A > 0)
                return false;
        }
    }

    return true;
}

/// <summary>Provides extension methods on <see cref="ImageFactory" />.</summary>
static class ImageFactoryExtensions
{
    /// <summary>Apply custom processing logic to the image.</summary>
    /// <param name="factory">The image editor.</param>
    /// <param name="process">The custom processing logic.</param>
    public static ImageFactory Custom(this ImageFactory factory, Func<ImageFactory, Image> process)
    {
        factory.CurrentImageFormat.ApplyProcessor(process, factory);
        return factory;
    }

    /// <summary>Resize the image using nearest-neighbor interpolation.</summary>
    /// <param name="editor">The image editor.</param>
    /// <param name="zoom">The multiplication by which to resize the image.</param>
    /// <remarks>Derived from <a href="https://stackoverflow.com/a/12522782/262123" />.</remarks>
    public static ImageFactory ZoomSprite(this ImageFactory editor, int zoom)
    {
        if (zoom <= 1)
            return editor;

        return editor
            .Custom(factory =>
            {
                // get new image
                int width = editor.Image.Width * zoom;
                int height = editor.Image.Height * zoom;
                Bitmap result = new Bitmap(width, height);
                using (Graphics graphics = Graphics.FromImage(result))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    graphics.DrawImage(editor.Image, 0, 0, width, height);
                }
    
                // update editor
                return result;
            });
    }

    /// <summary>Crop the image to remove surrounding whitespace, without removing any non-transparent pixels.</summary>
    /// <param name="editor">The image editor.</param>
    /// <param name="enabled">Whether to crop whitespace.</param>
    /// <remarks>Derived from <a href="https://stackoverflow.com/a/10392379/262123" />.</remarks>
    public static ImageFactory CropWhitespace(this ImageFactory editor, bool enabled = true)
    {
        if (!enabled)
            return editor;

        return editor
            .Custom(factory =>
            {
                var image = (Bitmap)factory.Image;
                int width = image.Width;
                int height = image.Height;
    
                bool IsBlankRow(int row)
                {
                    for (int i = 0; i < width; ++i)
                    {
                        if (image.GetPixel(i, row).A != 0)
                            return false;
                    }
                    return true;
                }
    
                bool IsBlankColumn(int col)
                {
                    for (int i = 0; i < height; ++i)
                    {
                        if (image.GetPixel(col, i).A != 0)
                            return false;
                    }
                    return true;
                }

                int topmost = 0;
                for (int row = 0; row < height; ++row)
                {
                    if (IsBlankRow(row))
                        topmost = row;
                    else
                        break;
                }

                int bottommost = 0;
                for (int row = height - 1; row >= 0; --row)
                {
                    if (IsBlankRow(row))
                        bottommost = row;
                    else
                        break;
                }

                int leftmost = 0, rightmost = 0;
                for (int col = 0; col < width; ++col)
                {
                    if (IsBlankColumn(col))
                        leftmost = col;
                    else
                        break;
                }

                for (int col = width - 1; col >= 0; --col)
                {
                    if (IsBlankColumn(col))
                        rightmost = col;
                    else
                        break;
                }

                if (rightmost == 0)
                    rightmost = width; // reached left
                if (bottommost == 0)
                    bottommost = height; // reached top

                int croppedWidth = rightmost - leftmost;
                int croppedHeight = bottommost - topmost;

                if (croppedWidth == 0) // No border on left or right
                {
                    leftmost = 0;
                    croppedWidth = width;
                }

                if (croppedHeight == 0) // No border on top or bottom
                {
                    topmost = 0;
                    croppedHeight = height;
                }

                try
                {
                    Bitmap target = new Bitmap(croppedWidth, croppedHeight);

                    using Graphics graphics = Graphics.FromImage(target);
                    graphics.DrawImage(
                        image: image,
                        destRect: new RectangleF(0, 0, croppedWidth, croppedHeight),
                        srcRect: new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                        srcUnit: GraphicsUnit.Pixel
                    );

                    return target;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not crop image whitespace. Detected values: top={topmost}, bottom={bottommost}, left={leftmost}, right={rightmost}, croppedWidth={croppedWidth}, croppedHeight={croppedHeight}.", ex);
                }
            }
        );
    }
}