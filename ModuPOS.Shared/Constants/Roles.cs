using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuPOS.Shared.Constants
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Cajero = "Cajero";
    }

    public static class Policies
    {
        public const string SoloAdmin = "SoloAdmin";
        public const string GestionarInventario = "GestionarInventario";
        public const string RealizarVenta = "RealizarVenta";
    }
}
