namespace ProductosReactive.Console.Models;

public class Producto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Stock { get; set; }

    public Producto WithDescuento(decimal porcentaje)
    {
        return new Producto
        {
            Id = Id,
            Nombre = Nombre,
            Precio = Precio * (1 - porcentaje),
            Categoria = Categoria,
            Stock = Stock
        };
    }

    public override string ToString() =>
        $"Producto{{Id={Id}, Nombre='{Nombre}', Precio={Precio:C}, Categoría='{Categoria}', Stock={Stock}}}";
}
