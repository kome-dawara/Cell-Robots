using System;
using System.Text;


public static class AgentStructureLogger
{
    public static void Log(AgentCellPersonality[,] array)
    {
        Console.WriteLine(ArrayToStringShort(array));
    }

    public static string ArrayToStringShort(AgentCellPersonality[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        var sb = new StringBuilder();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                string s = string.Empty;
                if (array[i, j].Type == CellType.NonEmpty)
                    s = "■";
   
                sb.Append(s.PadRight(2));
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
