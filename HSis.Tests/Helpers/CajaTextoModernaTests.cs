using System.Runtime.Versioning;
using FluentAssertions;
using HSis.UI.Controls;
using Xunit;

namespace HSis.Tests.Helpers
{
    [SupportedOSPlatform("windows")]
    public class CajaTextoModernaTests
    {
        [Fact]
        public void EsTelefonoValido_SinModoTelefono_SiempreRetornaTrue()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = false, Text = "123" };
            control.EsTelefonoValido.Should().BeTrue();
        }

        [Fact]
        public void EsTelefonoValido_ConTextoVacio_RetornaTrue()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = true, Text = string.Empty };
            control.EsTelefonoValido.Should().BeTrue();
            control.Text.Should().BeEmpty();
        }

        [Fact]
        public void FormatearTextoTelefono_Con10Digitos_AplicaFormatoCorrectoYEsValido()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = true, Text = "6641234567" };

            control.Text.Should().Be("(664) 123-4567");
            control.EsTelefonoValido.Should().BeTrue();
        }

        [Fact]
        public void FormatearTextoTelefono_ConMenosDe10Digitos_EsInvalido()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = true, Text = "66412" };

            control.Text.Should().Be("(664) 12");
            control.EsTelefonoValido.Should().BeFalse();
        }

        [Fact]
        public void FormatearTextoTelefono_ConCaracteresNoNumericos_FiltraYFormateaSoloDigitos()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = true, Text = "abc664xyz123test4567extra" };

            control.Text.Should().Be("(664) 123-4567");
            control.EsTelefonoValido.Should().BeTrue();
        }

        [Fact]
        public void FormatearTextoTelefono_AlBorrarTexto_LimpiaCompletamente()
        {
            var control = new CajaTextoModerna { EsFormatoTelefono = true, Text = "6641234567" };
            control.Text = string.Empty;

            control.Text.Should().BeEmpty();
            control.EsTelefonoValido.Should().BeTrue();
        }
    }
}
