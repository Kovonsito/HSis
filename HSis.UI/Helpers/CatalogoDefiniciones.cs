#nullable enable

namespace HSis.UI.Helpers;

public enum TipoCampoCatalogo
{
    Texto,
    TextoMultilinea,
    Entero,
    Decimal,
    Password,
    Combo
}

public sealed record CatalogoColumna(string Propiedad, string Titulo, int Ancho = 120);

public sealed record CatalogoReferencia(string Entidad, string Clave, string Texto);

public sealed record CatalogoCampo(
    string Propiedad,
    string Etiqueta,
    TipoCampoCatalogo Tipo,
    bool Requerido = false,
    object? ValorInicial = null,
    CatalogoReferencia? Referencia = null);

public sealed record CatalogoDefinicion(
    string Clave,
    string Titulo,
    string EntidadApi,
    string ClavePrimaria,
    IReadOnlyList<CatalogoColumna> Columnas,
    IReadOnlyList<CatalogoCampo> Campos);

public static class CatalogoDefiniciones
{
    private static readonly IReadOnlyList<CatalogoDefinicion> _todos =
    [
        new(
            "inventario",
            "Inventario",
            "Material",
            "IdMaterial",
            [
                new("IdMaterial", "Id", 70),
                new("Nombre", "Nombre", 180),
                new("Costo", "Costo", 100),
                new("Inventario", "Existencias", 100),
                new("UnidadMedida", "Unidad de medida", 140)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("Costo", "Costo", TipoCampoCatalogo.Decimal, true, 0m),
                new("Inventario", "Existencias", TipoCampoCatalogo.Entero, true, 0),
                new("UnidadMedida", "Unidad de medida", TipoCampoCatalogo.Texto, true)
            ]),
        new(
            "usuarios",
            "Usuarios",
            "Usuario",
            "IdUsuario",
            [
                new("IdUsuario", "Id", 70),
                new("Nombre", "Nombre", 180),
                new("IdDepartamento", "Departamento", 110),
                new("IdPuesto", "Puesto", 100),
                new("IdSucursal", "Sucursal", 100),
                new("IdRol", "Rol", 80)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("IdDepartamento", "Departamento", TipoCampoCatalogo.Combo, false, null, new("Departamento", "IdDepartamento", "Nombre")),
                new("IdPuesto", "Puesto", TipoCampoCatalogo.Combo, false, null, new("Puesto", "IdPuesto", "Nombre")),
                new("IdSucursal", "Sucursal", TipoCampoCatalogo.Combo, false, null, new("Sucursal", "IdSucursal", "Nombre")),
                new("IdRol", "Rol", TipoCampoCatalogo.Combo, true, null, new("RolUsuario", "IdRol", "Descripcion")),
                new("Contraseña", "Contraseña", TipoCampoCatalogo.Password, true)
            ]),
        new(
            "departamentos",
            "Departamentos",
            "Departamento",
            "IdDepartamento",
            [
                new("IdDepartamento", "Id", 70),
                new("Nombre", "Nombre", 180),
                new("Descripcion", "Descripción", 280)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("Descripcion", "Descripción", TipoCampoCatalogo.TextoMultilinea)
            ]),
        new(
            "sucursales",
            "Sucursales",
            "Sucursal",
            "IdSucursal",
            [
                new("IdSucursal", "Id", 70),
                new("Nombre", "Nombre", 150),
                new("Calle", "Calle", 150),
                new("Numero", "Número", 80),
                new("Colonia", "Colonia", 130),
                new("Telefono", "Teléfono", 120),
                new("IdEmpresa", "Empresa", 100)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("Calle", "Calle", TipoCampoCatalogo.Texto),
                new("Numero", "Número", TipoCampoCatalogo.Texto),
                new("Colonia", "Colonia", TipoCampoCatalogo.Texto),
                new("Telefono", "Teléfono", TipoCampoCatalogo.Texto),
                new("IdEmpresa", "Empresa", TipoCampoCatalogo.Combo, true, null, new("Empresa", "IdEmpresa", "Nombre"))
            ]),
        new(
            "empresas",
            "Empresas",
            "Empresa",
            "IdEmpresa",
            [
                new("IdEmpresa", "Id", 70),
                new("Nombre", "Nombre", 180),
                new("Calle", "Calle", 150),
                new("Numero", "Número", 80),
                new("Colonia", "Colonia", 130),
                new("Telefono", "Teléfono", 120)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("Calle", "Calle", TipoCampoCatalogo.Texto),
                new("Numero", "Número", TipoCampoCatalogo.Texto),
                new("Colonia", "Colonia", TipoCampoCatalogo.Texto),
                new("Telefono", "Teléfono", TipoCampoCatalogo.Texto)
            ]),
        new(
            "puestos",
            "Puestos",
            "Puesto",
            "IdPuesto",
            [
                new("IdPuesto", "Id", 70),
                new("Nombre", "Nombre", 180),
                new("Descripcion", "Descripción", 280)
            ],
            [
                new("Nombre", "Nombre", TipoCampoCatalogo.Texto, true),
                new("Descripcion", "Descripción", TipoCampoCatalogo.TextoMultilinea)
            ]),
        new(
            "roles",
            "Roles",
            "RolUsuario",
            "IdRol",
            [
                new("IdRol", "Id", 70),
                new("Descripcion", "Descripción", 240)
            ],
            [
                new("Descripcion", "Descripción", TipoCampoCatalogo.Texto, true)
            ])
    ];

    public static IReadOnlyList<CatalogoDefinicion> ObtenerTodos() => _todos;

    public static CatalogoDefinicion Obtener(string clave)
    {
        return _todos.First(definicion =>
            string.Equals(definicion.Clave, clave, StringComparison.OrdinalIgnoreCase));
    }
}
