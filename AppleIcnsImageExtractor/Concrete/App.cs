using System.Runtime.InteropServices;
using AppleIcnsImageExtractor.Abstract;
using AppleIcnsImageExtractor.Models.AppleIconsFile;

namespace AppleIcnsImageExtractor.Concrete;

public class App : IApp
{
    private readonly IFileOperations fileOperations;
    private readonly IAppleIconsFileParser parser;
    private readonly Func<IExternalProcessRunner> processFactory;
    
    public App(IFileOperations fileOperations, IAppleIconsFileParser parser, Func<IExternalProcessRunner> processFactory)
    {
        this.fileOperations = fileOperations;
        this.parser = parser;
        this.processFactory = processFactory;
    }
    
    public int Execute(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Expected to arguments - the source path, and the output path.");
            return 1;
        }

        if (string.IsNullOrWhiteSpace(args[0]) || !fileOperations.DirectoryExists(args[0]))
        {
            Console.WriteLine("Input path invalid or does not exist.");
            return 2;
        }
        
        if (string.IsNullOrWhiteSpace(args[1]) || fileOperations.DirectoryExists(args[1]))
        {
            Console.WriteLine("Output path invalid or already exists.");
            return 3;
        }

        return PerformExtraction(args[0], args[1]);
    }

    private int PerformExtraction(string sourceDir, string outputDir)
    {
        var sourceFiles = fileOperations.GetFiles(sourceDir, "*.icns", true).OrderByDescending(x => x).ToArray();
        if (sourceFiles.Any())
        {
            fileOperations.CreateDirectory(outputDir);
            int fileCtr = 0;
            int totalImgCtr = 0;
            int errCtr = 0;
            foreach (var sourceFile in sourceFiles)
            {
                fileCtr++;
                Console.WriteLine(new string('-', 80));
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Parsing file {fileCtr} of {sourceFiles.Length}");
                try
                {
                    var icns = parser.Parse(fileOperations.ReadAllBytes(sourceFile));
                    if (icns.Any())
                    {
                        int imgCtr = 0;
                        foreach (var icn in icns)
                        {
                            if (icn.Format != AppleIconFormat.Unknown)
                            {
                                try
                                {
                                    var p1 = Path.GetFileNameWithoutExtension(sourceFile);
                                    var p2 = icn.IconType;
                                    var p3 = icn.Width.ToString() + "x" + icn.Height.ToString();
                                    var p4 = Enum.GetName(icn.Format)?.ToLower() ?? "png";
                                    var fn = p1 + "_" + p2 + "_" + p3 + "." + p4;
                                    var outputFilename = Path.Combine(outputDir, fn);
                                    fileOperations.WriteAllBytesToFile(outputFilename, icn.Data);
                                    imgCtr++;
                                    totalImgCtr++;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(new string('-', 80));
                                    Console.WriteLine($"Failed to save type {icn.IconType} from source file: {sourceFile}\n{ex}");
                                    errCtr++;
                                }
                            }
                        }

                        if (imgCtr == 0)
                        {
                            Console.WriteLine(new string('-', 80));
                            Console.WriteLine($"Nothing extracted from: {sourceFile}");
                        }
                    }
                    else
                    {
                        Console.WriteLine(new string('-', 80));
                        Console.WriteLine($"No compatible images in: {sourceFile}");
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            Console.WriteLine("[macOS] Falling back to SIPS");
                            var proc = processFactory();
                            var outputFilename = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(sourceFile) + ".png");
                            proc.Execute("sips", $"-s format png \"{sourceFile}\" --out \"{outputFilename}\"",
                                CancellationToken.None);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(new string('-', 80));
                    Console.WriteLine($"Failed to parse source file: {sourceFile}\n{ex}");
                    errCtr++;
                }
            }
            
            Console.WriteLine($"Extracted {totalImgCtr} images from {sourceFiles.Length} ICNS files.");
            Console.WriteLine($"Encountered {errCtr} errors.");
            return 0;
        }

        return 4;
    }
}