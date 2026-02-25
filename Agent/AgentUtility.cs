using System.Collections;

public static class AgentUtility
{
    //ランダムウォーク的にエージェントの初期形状を決定する
    public static AgentCellPersonality[,] GetStructureMap(int[,] weights)
    {
        AgentCellPersonality[,] map = new AgentCellPersonality[Settings.mapSize, Settings.mapSize];

        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                map[i, j] = new AgentCellPersonality(CellType.Empty);

        int cellNumber = Random.Shared.Next((int)Settings.minCellNumber, Settings.maxCellNumber);

        List<(int x, int y)> candidates = new List<(int x, int y)>();
        int count = 0;

        // 起点の設定
        int sx = Settings.mapSize / 2, sy = Settings.mapSize / 2;
        map[sx, sy].Type = CellType.NonEmpty;
        map[sx, sy].Key = (byte)Random.Shared.Next(0, 255);
        count++;

        // 隣接するグリッドを加える
        void AddNeighbors(int x, int y)
        {
            foreach ((int dx, int dy) in new[] { (0, 1), (0, -1), (1, 0), (-1, 0) })
            {
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Settings.mapSize && ny >= 0 && ny < Settings.mapSize && map[nx, ny].Type == CellType.Empty)
                    if (!candidates.Contains((nx, ny))) candidates.Add((nx, ny));
            }
        }

        AddNeighbors(sx, sy);

        while (candidates.Count > 0 && count < cellNumber)
        {
            int maxWeight = -1;
            foreach ((int x, int y) c in candidates)
            {
                if (weights[c.x, c.y] > maxWeight) maxWeight = weights[c.x, c.y];
            }

            List<(int x, int y)> bestCandidates = candidates.FindAll(c => weights[c.x, c.y] == maxWeight);

            int index = Random.Shared.Next(bestCandidates.Count);
            (int px, int py) = bestCandidates[index];

            candidates.Remove((px, py));

            if (map[px, py].Type == CellType.NonEmpty) continue;

            map[px, py].Type = CellType.NonEmpty;
            map[sx, sy].Key = (byte)Random.Shared.Next(1, 255);
            count++;
            AddNeighbors(px, py);
        }

        return map;
    }

    //エージェントのキーの傾向をスコアリングする
    public static int[] GeneticScore(AgentCellPersonality[,] _personalities)
    {
        int[] score = new int[8]; 

        for(int digit = 0; digit < 8; digit++)
        {
            int digitScore = 0;

            for (int i = 0; i < _personalities.GetLength(0); i++)
                for (int j = 0; j < _personalities.GetLength(1); j++)
                {
                    digitScore += (_personalities[i, j].Key >> digit) & 1;
                }

            score[digit] += digitScore;
        }

        return score;
    }

    public static List<AgentCell> GetNonEmptyCellNumber(AgentCell[,] _cells)
    {
        List<AgentCell> result = new();

        for (int i = 0; i < _cells.GetLength(0); i++)
            for (int j = 0; j < _cells.GetLength(1); j++)
            {
                if (_cells[i, j].Type == CellType.NonEmpty)
                    result.Add(_cells[i, j]);
            }

        return result;
    }
}