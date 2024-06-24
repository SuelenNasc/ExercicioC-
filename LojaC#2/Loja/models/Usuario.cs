using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loja.models
{
    public class Usuario
    {
        public int Id {get; set;}
        public String Nome {get; set;} = string.Empty;
        public String Email {get; set;} = string.Empty;
        public String Senha {get; set;} = string.Empty;
    }
}