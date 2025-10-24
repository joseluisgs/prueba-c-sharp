namespace TestContainersDemo.Console;

class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("=== 🐳 Ejemplo 10: Docker & TestContainers ===\n");
        System.Console.WriteLine("Este ejemplo demuestra el uso de TestContainers para testing de integración.");
        System.Console.WriteLine("\nPara ver los tests de integración, ejecuta:");
        System.Console.WriteLine("  cd TestContainersDemo.Tests");
        System.Console.WriteLine("  dotnet test\n");
        System.Console.WriteLine("Los tests crearán automáticamente containers Docker para PostgreSQL");
        System.Console.WriteLine("y ejecutarán pruebas de integración contra ellos.");
        System.Console.WriteLine("\n✅ Consulta el README.md para más información y patrones avanzados.");
    }
}
