using System.Collections;
using System.Numerics;

//命令物質
public class Instructor
{
    public int Age { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public Vector2 Position { get; set; }

    //セルに解読されるコード
    public byte Code { get; set; }
    public Instructor(byte _code)
    {
        Code = _code;
    }
}
