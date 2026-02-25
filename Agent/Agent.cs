//エージェントの個体クラス
using System.Diagnostics;
using System.Numerics;

public class Agent
{
    public Vector2 Position { get; private set; }
    public float Rotation { get; private set; }
    //エージェント全体がアクティブか（アクティブなセルの数が閾値を上回っているか）
    public bool IsActive { get; private set; } = true;

    //自己複製時の自己変異用
    CellProcessor processor;
    //（角）速度
    Vector2 velocity;
    float angularVelocity;
    //重心と慣性モーメント
    Vector2 centerOfMass;
    float inertia;

    List<AgentCell> body = new();

    //パーソナリティとインスタンスの配列フィールド
    public AgentCellPersonality[,] cellPersonalities { get; set; } = new AgentCellPersonality[Settings.mapSize, Settings.mapSize];
    public AgentCell[,] cellInstances { get; set; } = new AgentCell[Settings.mapSize, Settings.mapSize];

    public event Action<Instructor>? InstructorReleased;
    public event Action<Agent>? AgentCreated;

    public Agent(AgentCellPersonality[,] personalities, Vector2 _position)
    {
        Position = _position;
        cellPersonalities = personalities;
        processor = new();

        //初期状態をそれっぽくログ出力する
        //AgentStructureLogger.Log(cellPersonalities);

        //modulesの値からインスタンスを作成し、modulesInstanceに突っ込む
        for (int i = 0; i < cellPersonalities.GetLength(0); i++)
            for (int j = 0; j < cellPersonalities.GetLength(1); j++)
            {
                cellInstances[i, j] = new()
                {
                    GridPosition = new(i, j),
                    Key = cellPersonalities[i, j].Key,
                    Type = cellPersonalities[i, j].Type,
                    Resources = 255 * 0.2f,
                };
            }

        body = AgentUtility.GetNonEmptyCellNumber(cellInstances);

        Vector2 sum = Vector2.Zero;
        foreach (AgentCell pos in cellInstances)
        {
            sum += pos.GridPosition;
        }
        centerOfMass = sum / body.Count;

        float iSum = 0f;
        foreach (AgentCell pos in cellInstances)
        {
            iSum += Vector2.DistanceSquared(pos.GridPosition, centerOfMass);
        }
        inertia = iSum;
    }

    //初期値を設定しない場合、ランダムに生成するオーバーロード
    public Agent() : this(AgentUtility.GetStructureMap(new int[Settings.mapSize, Settings.mapSize]), new System.Numerics.Vector2(System.Random.Shared.Next(-Settings.fieldSize, Settings.fieldSize), System.Random.Shared.Next(-Settings.fieldSize, Settings.fieldSize)))
    {

    }

    public void Start()
    {

    }

    public void Update(List<Instructor> instructors)
    {
        //総リソース量が80%を超過したら自己複製し、すべてのセルのリソース量を半分に
        if(GetTotalResources() >= 255f * 0.5f)
        {
            Replicate();
            for (int i = 0; i < cellInstances.GetLength(0); i++)
                for (int j = 0; j < cellInstances.GetLength(1); j++)
                    cellInstances[i, j].Resources *= 0.5f;
        }

        CellProcess(instructors);

        //（角）速度減衰
        velocity *= 0.98f;
        angularVelocity *= 0.95f;
        //移動
        Position = new Vector2(Math.Clamp(Position.X + velocity.X, -Settings.fieldSize, Settings.fieldSize), Math.Clamp(Position.Y + velocity.Y, -Settings.fieldSize, Settings.fieldSize));
        Rotation += angularVelocity;

        //生存判定
        int activeCellCount = 0;
        for (int i = 0; i < cellInstances.GetLength(0); i++)
            for (int j = 0; j < cellInstances.GetLength(1); j++)
            {
                if (cellInstances[i, j].IsActive && cellInstances[i, j].Type == CellType.NonEmpty)
                    activeCellCount++;
            }
        if (activeCellCount <= 0)
            IsActive = false;
    }

    //各セルの毎フレームの処理
    void CellProcess(List<Instructor> _instructors)
    {
        foreach(AgentCell cell in body)
        {
            if (!cell.IsActive)
                continue;

            cell.Resources -= 1f;

            //接触している命令物質を取得させる
            (bool, Instructor) contacted = cell.ContactedInstructor(_instructors, Position, Rotation, centerOfMass);
            if (contacted.Item1)
                cell.CurrentProcessed = contacted.Item2.Code;

            //命令物質の解読結果に応じて処理を実行する
            (InstructionType, byte) process = cell.ProcessCode();
            switch (process.Item1)
            {
                case InstructionType.Release:
                    ReleaseInstructor(cell, process.Item2);
                    break;

                case InstructionType.Thrust:
                    Thrust(cell, process.Item2);
                    break;

                case InstructionType.Resource:
                    Resource(cell, process.Item2);
                    break;

                default:
                    break;
            }
        }
    }

    void Thrust(AgentCell cell, byte _input)
    {
        Vector2 cellLocalPos = cell.GridPosition - centerOfMass;

        int data = _input & 0b00111111;
        float angle = ((data >> 2) & 0x0F) * (MathF.PI * 2f / 16f);
        float magnitude = (data & 0x03) * 0.1f;

        Vector2 thrustForce = new Vector2(
            MathF.Cos(angle) * magnitude,
            MathF.Sin(angle) * magnitude
        );

        float cos = MathF.Cos(Rotation);
        float sin = MathF.Sin(Rotation);

        Vector2 worldThrust = new Vector2(
            thrustForce.X * cos - thrustForce.Y * sin,
            thrustForce.X * sin + thrustForce.Y * cos
        );

        velocity += worldThrust / body.Count;

        float torque = cellLocalPos.X * thrustForce.Y - cellLocalPos.Y * thrustForce.X;
        float angularAcceleration = torque / inertia;

        angularVelocity += angularAcceleration;

        cell.Resources -= 1;
    }

    //資源量の加減算
    void Resource(AgentCell cell, byte _input)
    {
        cell.Resources += ((_input & 0b00111111) /*- 0b00100000*/);
    }
   
    void ReleaseInstructor(AgentCell cell, byte _input)
    {
        for (int i = 0; i < (_input & 0b00111111) % 4 + 1; i++)
        {
            InstructorReleased?.Invoke(new(_input) { Position = this.Position + (new Vector2(System.Random.Shared.Next(-10, 10) / 10f, System.Random.Shared.Next(-10, 10) / 10f) )});
            cell.Resources -= 1; //コスト
        } 
    }

    float GetTotalResources()
    {
        float counter = 0;

        for (int i = 0; i < cellInstances.GetLength(0); i++)
            for (int j = 0; j < cellInstances.GetLength(1); j++)
            {
                if (cellInstances[i, j].IsActive && cellInstances[i, j].Type == CellType.NonEmpty)
                    counter += cellInstances[i, j].Resources;
            }
        return counter / body.Count;
    }

    //自己複製
    void Replicate()
    {
        Debug.WriteLine("Replicate");
        AgentCreated?.Invoke(new(processor.ProcessCells(cellPersonalities), Position));
    }
}
