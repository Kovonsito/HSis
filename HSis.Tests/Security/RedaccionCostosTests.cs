using System.Security.Claims;
using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Server.Security;
using Xunit;

namespace HSis.Tests.Security;

public sealed class RedaccionCostosTests
{
    private static ClaimsPrincipal Usuario(RolUsuarioEnum rol)
        => new(new ClaimsIdentity([new Claim(ClaimTypes.Role, ((int)rol).ToString())], "prueba"));

    [Theory]
    [InlineData(RolUsuarioEnum.Tecnico)]
    [InlineData(RolUsuarioEnum.Cliente)]
    public void NoAdministrador_NoRecibeCostosPeroConservaElResto(RolUsuarioEnum rol)
    {
        var materiales = RedaccionCostos.Para(Usuario(rol), [new MaterialDto { IdMaterial = 1, Nombre = "Toner", StockActual = 5, UnidadMedida = "pza", CostoUnitario = 10m }]);
        var kardex = RedaccionCostos.Para(Usuario(rol), [new KardexMovimientoDto { IdMovimiento = 7, IdMaterial = 1, Cantidad = 2, CostoUnitario = 10m, Motivo = "Compra", TipoMovimiento = "ENTRADA" }]);

        materiales.Should().ContainSingle().Which.Should().BeEquivalentTo(new MaterialDto { IdMaterial = 1, Nombre = "Toner", StockActual = 5, UnidadMedida = "pza", CostoUnitario = 0m });
        kardex.Should().ContainSingle().Which.Should().BeEquivalentTo(new KardexMovimientoDto { IdMovimiento = 7, IdMaterial = 1, Cantidad = 2, CostoUnitario = 0m, Motivo = "Compra", TipoMovimiento = "ENTRADA" });
    }

    [Fact]
    public void Administrador_RecibeCostos()
    {
        var admin = Usuario(RolUsuarioEnum.Administrador);

        RedaccionCostos.Para(admin, [new MaterialDto { CostoUnitario = 10m }]).Single().CostoUnitario.Should().Be(10m);
        RedaccionCostos.Para(admin, [new KardexMovimientoDto { CostoUnitario = 10m }]).Single().CostoUnitario.Should().Be(10m);
    }
}