public class CellProcessor
{
    private Random random = new Random();

    public AgentCellPersonality[,] ProcessCells(AgentCellPersonality[,] _grid)
    {
        AgentCellPersonality[,] grid = DeepCopyGrid(_grid);
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int flipCount = random.Next(1, 11);
        for (int i = 0; i < flipCount; i++)
        {
            List<(int r, int c)> candidates = new List<(int r, int c)>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (HasActiveNeighbor(grid, r, c))
                    {
                        candidates.Add((r, c));
                    }
                }
            }

            if (candidates.Count > 0)
            {
                var target = candidates[random.Next(candidates.Count)];
                grid[target.r, target.c].Type = grid[target.r, target.c].Type == CellType.Empty ? CellType.NonEmpty : CellType.Empty;
            }
        }

        int updateCount = random.Next(1, 11);
        for (int i = 0; i < updateCount; i++)
        {
            int r = random.Next(rows);
            int c = random.Next(cols);
            grid[r, c].Key = (byte)random.Next(0, 64);
        }

        return grid;
    }

    private bool HasActiveNeighbor(AgentCellPersonality[,] grid, int r, int c)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;

                int nr = r + dr;
                int nc = c + dc;

                if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                {
                    if (grid[nr, nc].Type == CellType.NonEmpty) return true;
                }
            }
        }
        return false;
    }

    private AgentCellPersonality[,] DeepCopyGrid(AgentCellPersonality[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);
        AgentCellPersonality[,] target = new AgentCellPersonality[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // 新しいインスタンスを作成し、プロパティをコピーする
                target[r, c] = new AgentCellPersonality(source[r, c].Type)
                {
                    Type = source[r, c].Type,
                    Key = source[r, c].Key
                };
            }
        }
        return target;
    }
}