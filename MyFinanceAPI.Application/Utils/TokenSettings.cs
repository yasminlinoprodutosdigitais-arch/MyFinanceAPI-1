using System;

namespace MyFinanceAPI.Application.Utils;

public class TokenSettings
{
    public string SecretKey {get; set;}

    public DateTime Expires {get; set;}

    public DateTime NotBefore {get; set;}
}
