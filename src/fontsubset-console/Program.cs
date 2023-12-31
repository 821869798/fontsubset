using Subsetter;

public class Program
{
    static void Main(string[] args)
    {
        RunCommand(args);
    }

    public static void RunCommand(string[] args)
    {
        string fontFile = string.Empty;
        string outputFile = string.Empty;
        string charsSetPath = string.Empty;
        string charsfileMatch = @".+\.txt|.+\.lua|.+\.asset";
        bool containAscii = false;
        bool stripTable = false;


        for (int i = 0; i < args.Length; i++)
        {
            string option = string.Empty;
            if (args[i][0] == '-')
            {
                option = args[i].Substring(1);
            }

            if (!string.IsNullOrEmpty(option))
            {
                if (option.Equals("help") || option.Equals("h"))
                {
                    printUsage();
                    return;
                }
                else if (option.Equals("c") || option.Equals("charsfile"))
                {
                    charsSetPath = args[i + 1];
                    i++;
                }
                else if (option.Equals("r") || option.Equals("regex"))
                {
                    charsfileMatch = args[i + 1];
                    i++;
                }
                else if (option.Equals("a") || option.Equals("ascii"))
                {
                    containAscii = true;
                }
                else if (option.Equals("s") || option.Equals("strip"))
                {
                    stripTable = true;
                }
                else
                {
                    printUsage();
                    return;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(fontFile))
                {
                    fontFile = args[i];
                }
                else
                {
                    outputFile = args[i];
                    break;
                }
            }
        }

        if (!File.Exists(fontFile))
        {
            Console.WriteLine("input font is null");
            return;
        }
        if (!Directory.Exists(charsSetPath))
        {
            Console.WriteLine("input chars set path is null");
            return;
        }

        Console.WriteLine("Start Subset......");
        var result = SubsetterUtil.StartSubset(fontFile, outputFile, charsSetPath, charsfileMatch, containAscii, stripTable).Result;
        if (result != null)
        {
            Console.WriteLine("Subset Failed, Exception:" + result);
        }
        else
        {
            Console.WriteLine("Subset Success");
        }
    }

    private static void printUsage()
    {
        Console.WriteLine("fontsubset-gui [-?|-h|-help] [-a] [-s] -c <char path> [-m string] <fontfile> <outfile>");
        Console.WriteLine("Prototype font subsetter");
        Console.WriteLine("\t-h,-help\tprint this help information");
        Console.WriteLine("\t-c,-charsfile\t chars file path");
        Console.WriteLine("\t-r,-regex\t chars file regex match, default:'.+\\.txt|.+\\.lua|.+\\.asset'");
        Console.WriteLine("\t-a,-ascii\t retain Ascii");
        Console.WriteLine("\t-s,-strip\t strip extra font table");
    }
}