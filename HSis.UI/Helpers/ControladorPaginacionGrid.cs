#nullable enable
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using HSis.UI.Controls;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public class ControladorPaginacionGrid(PaginacionControl control)
    {
        private readonly PaginacionControl _control = control ?? throw new ArgumentNullException(nameof(control));
        private bool _suspenderEventos = false;

        public int PaginaActual
        {
            get => _control.PaginaActual;
            set => _control.PaginaActual = value;
        }

        public int TamanoPagina
        {
            get => _control.TamanoPagina;
            set => _control.TamanoPagina = value;
        }

        public int TotalRegistros
        {
            get => _control.TotalRegistros;
            set => _control.TotalRegistros = value;
        }

        public void Vincular(Func<Task> alCambiarPagina)
        {
            _control.PaginaCambiada += async (s, e) =>
            {
                if (!_suspenderEventos)
                {
                    await alCambiarPagina();
                }
            };
        }

        public void Vincular(Action alCambiarPagina)
        {
            _control.PaginaCambiada += (s, e) =>
            {
                if (!_suspenderEventos)
                {
                    alCambiarPagina();
                }
            };
        }


        public void Actualizar(int totalRegistros, int paginaActual, int tamanoPagina)
        {
            _suspenderEventos = true;
            try
            {
                _control.TotalRegistros = totalRegistros;
                _control.TamanoPagina = tamanoPagina;
                _control.PaginaActual = paginaActual;
                _control.ActualizarInterfaz();
            }
            finally
            {
                _suspenderEventos = false;
            }
        }

        public void Actualizar(int totalRegistros)
        {
            _control.TotalRegistros = totalRegistros;
            _control.ActualizarInterfaz();
        }

        public void ReiniciarAPrimeraPagina()
        {
            _control.PaginaActual = 1;
        }

    }
}

