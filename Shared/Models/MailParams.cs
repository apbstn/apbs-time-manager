using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models;

public class MailParams
{
    public string ToEmail;
    public string Subject;
    public string Body;
    public bool IsBodyHtml;
}
