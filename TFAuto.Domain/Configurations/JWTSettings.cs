using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.Domain;

public class JWTSettings
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string IssuerSigningKey { get; set; }
    public int AccessTokenLifetimeInSeconds { get; set; }
    public int RefreshTokenLifetimeInSeconds { get; set; }
}

