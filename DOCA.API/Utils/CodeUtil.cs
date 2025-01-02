namespace DOCA.API.Utils;

public class CodeUtil
{
    public static string GenarateNextCode(string code, string start)
    {
        
        if (code == null) return start + "01";

        var number = int.Parse(code.Substring(start.Length));
        number++;
        return start + number.ToString().PadLeft(2, '0');
    }
    public static string GenerateWarrantyCode(Guid productId)
    {
        return $"{productId:N}-{DateTime.Now.Ticks}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
}