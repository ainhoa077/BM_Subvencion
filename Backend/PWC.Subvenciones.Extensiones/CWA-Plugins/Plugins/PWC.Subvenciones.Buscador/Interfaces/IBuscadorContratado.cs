using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWC.Subvenciones.Buscador.Interfaces
{
    public interface IBuscadorContratado
    {
        void Ejecutar(Entity solicitud);
    }
}
