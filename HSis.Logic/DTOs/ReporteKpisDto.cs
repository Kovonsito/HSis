using System;
using System.Collections.Generic;

namespace HSis.Logic.DTOs
{
    public class ReporteKpisDto
    {
        public int TotalCreados { get; set; }
        public int TotalResueltos { get; set; }
        public double TasaCierre { get; set; }
        public double TiempoPromedioAtencionHoras { get; set; }

        public List<PersonalProductividadDto> ProductividadTecnica { get; set; } = new();
        public List<UsuarioDemandaDto> DemandaUsuarios { get; set; } = new();
        public List<DepartamentoMetricaDto> DemandaDepartamentos { get; set; } = new();
        public List<AnalisisTemporalDto> AnalisisTemporal { get; set; } = new();
    }

    public class PersonalProductividadDto
    {
        public string Tecnico { get; set; } = null!;
        public int TicketsAsignados { get; set; }
        public int TicketsResueltos { get; set; }
        public double TasaCierre { get; set; }
    }

    public class UsuarioDemandaDto
    {
        public string Usuario { get; set; } = null!;
        public int TicketsCreados { get; set; }
        public int TicketsResueltos { get; set; }
        public int TicketsPendientes { get; set; }
    }

    public class DepartamentoMetricaDto
    {
        public string Departamento { get; set; } = null!;
        public int TotalTickets { get; set; }
        public int AltaPrioridad { get; set; }
        public double PorcentajeDelTotal { get; set; }
    }

    public class AnalisisTemporalDto
    {
        public string Periodo { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
