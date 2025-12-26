using System.Text.RegularExpressions;

namespace Geo.Smart.AiAgentHub.Services.Helpers;
/// <summary>
/// 身份證字號小幫手
/// https://gist.github.com/yyc1217/3856443
/// </summary>
public static class IdNoHelper
{
    //本國身份證
    private static readonly string _match = "[A-Z]{1}[1-2]{1}[0-9]{8}";

    //居留證-舊式
    private static readonly string _matchOld = "[A-Z]{1}[A-D]{1}[0-9]{8}";

    //居留證-新式
    private static readonly string _matchNew = "[A-Z]{1}[89]{1}[0-9]{8}";

    /// <summary>
    /// 帶入從 Azure Vision 取回的一段文字，擷取出身份證字號
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetIdNo(string input)
    {
        input = input.ToUpper();
        var pattern = $"{_match}|{_matchOld}|{_matchNew}";
        var rgx = new Regex(pattern);
        var matchs = rgx.Match(input);
        if (matchs.Success)
        {
            return matchs.Value;
        }
        return string.Empty;
    }
}