namespace ProductosReactive.Console.Models;

public class ProductoDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public int Stock { get; set; }

    public static ProductoDto FromProducto(Producto producto) => new()
    {
        Id = producto.Id,
        Nombre = producto.Nombre,
        Precio = producto.Precio,
        Categoria = producto.Categoria,
        Stock = producto.Stock
    };

    public Producto ToProducto() => new()
    {
        Id = Id,
        Nombre = Nombre,
        Precio = Precio,
        Categoria = Categoria,
        Stock = Stock
    };

    public override string ToString() =>
        $"ProductoDto{{Id={Id}, Nombre='{Nombre}', Precio={Precio:C}, Categoría='{Categoria}', Stock={Stock}}}";
}
