using AutoMapper;
using TiendaApi.Common;
using TiendaApi.Models.DTOs;
using TiendaApi.Models.Entities;
using TiendaApi.Repositories;

namespace TiendaApi.Services;

/// <summary>
/// Service for Producto using MODERN RESULT PATTERN approach
/// 
/// This service demonstrates the functional programming pattern:
/// - return Result.Success() or AppError.NotFound()
/// - NO exceptions for business logic errors
/// - Explicit error handling in controller
/// 
/// Java comparison:
/// - Similar to Either<Error, Value> from Vavr
/// - Like Optional<T> but with error information
/// - CompletableFuture<Either<Error, T>> pattern
/// 
/// EDUCATIONAL NOTE: Compare this with CategoriaService (Exception-based)
/// 
/// Benefits of Result Pattern:
/// 1. Type-safe error handling
/// 2. No hidden control flow (exceptions)
/// 3. Easier to test (no try/catch needed)
/// 4. Explicit in method signatures what can fail
/// 5. Better performance (no stack unwinding)
/// </summary>
public class ProductoService
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IMapper mapper,
        ILogger<ProductoService> logger)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// Returns Success with list - doesn't fail
    /// Java: Either.right(List<ProductoDto>)
    /// </summary>
    public async Task<Result<IEnumerable<ProductoDto>, AppError>> FindAllAsync()
    {
        _logger.LogInformation("Finding all productos");
        
        var productos = await _productoRepository.FindAllAsync();
        var dtos = _mapper.Map<IEnumerable<ProductoDto>>(productos);
        
        return Result<IEnumerable<ProductoDto>, AppError>.Success(dtos);
    }

    /// <summary>
    /// Find product by ID
    /// RETURNS Result - Success with ProductoDto OR Failure with AppError
    /// 
    /// Java equivalent:
    /// Either<AppError, ProductoDto> findById(Long id)
    /// 
    /// NO EXCEPTIONS thrown - error is returned as value
    /// </summary>
    public async Task<Result<ProductoDto, AppError>> FindByIdAsync(long id)
    {
        _logger.LogInformation("Finding producto with id: {Id}", id);
        
        var producto = await _productoRepository.FindByIdAsync(id);
        
        if (producto == null)
        {
            _logger.LogWarning("Producto with id {Id} not found", id);
            return Result<ProductoDto, AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado")
            );
        }
        
        var dto = _mapper.Map<ProductoDto>(producto);
        return Result<ProductoDto, AppError>.Success(dto);
    }

    /// <summary>
    /// Find products by category
    /// </summary>
    public async Task<Result<IEnumerable<ProductoDto>, AppError>> FindByCategoriaIdAsync(long categoriaId)
    {
        _logger.LogInformation("Finding productos for categoria: {CategoriaId}", categoriaId);
        
        // Verify category exists
        var categoria = await _categoriaRepository.FindByIdAsync(categoriaId);
        if (categoria == null)
        {
            return Result<IEnumerable<ProductoDto>, AppError>.Failure(
                AppError.NotFound($"Categoría con ID {categoriaId} no encontrada")
            );
        }
        
        var productos = await _productoRepository.FindByCategoriaIdAsync(categoriaId);
        var dtos = _mapper.Map<IEnumerable<ProductoDto>>(productos);
        
        return Result<IEnumerable<ProductoDto>, AppError>.Success(dtos);
    }

    /// <summary>
    /// Create new product
    /// RETURNS Result - no exceptions
    /// 
    /// Validation failures return AppError.Validation
    /// Business rule violations return AppError.BusinessRule
    /// 
    /// Java: Either<AppError, ProductoDto> create(ProductoRequestDto dto)
    /// </summary>
    public async Task<Result<ProductoDto, AppError>> CreateAsync(ProductoRequestDto dto)
    {
        _logger.LogInformation("Creating producto: {Nombre}", dto.Nombre);
        
        // Validation using Result Pattern
        var validationResult = await ValidateProductoAsync(dto);
        if (validationResult.IsFailure)
        {
            return Result<ProductoDto, AppError>.Failure(validationResult.Error);
        }
        
        var producto = _mapper.Map<Producto>(dto);
        var saved = await _productoRepository.SaveAsync(producto);
        
        _logger.LogInformation("Producto created with id: {Id}", saved.Id);
        
        var resultDto = _mapper.Map<ProductoDto>(saved);
        return Result<ProductoDto, AppError>.Success(resultDto);
    }

    /// <summary>
    /// Update existing product
    /// RETURNS Result - no exceptions
    /// </summary>
    public async Task<Result<ProductoDto, AppError>> UpdateAsync(long id, ProductoRequestDto dto)
    {
        _logger.LogInformation("Updating producto with id: {Id}", id);
        
        var producto = await _productoRepository.FindByIdAsync(id);
        
        if (producto == null)
        {
            _logger.LogWarning("Producto with id {Id} not found for update", id);
            return Result<ProductoDto, AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado")
            );
        }
        
        // Validation using Result Pattern
        var validationResult = await ValidateProductoAsync(dto);
        if (validationResult.IsFailure)
        {
            return Result<ProductoDto, AppError>.Failure(validationResult.Error);
        }
        
        // Update fields
        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.Imagen = dto.Imagen;
        producto.CategoriaId = dto.CategoriaId;
        
        var updated = await _productoRepository.UpdateAsync(producto);
        
        _logger.LogInformation("Producto updated with id: {Id}", id);
        
        var resultDto = _mapper.Map<ProductoDto>(updated);
        return Result<ProductoDto, AppError>.Success(resultDto);
    }

    /// <summary>
    /// Delete product (soft delete)
    /// RETURNS Result<AppError> - void operation with potential error
    /// </summary>
    public async Task<Result<AppError>> DeleteAsync(long id)
    {
        _logger.LogInformation("Deleting producto with id: {Id}", id);
        
        var producto = await _productoRepository.FindByIdAsync(id);
        
        if (producto == null)
        {
            _logger.LogWarning("Producto with id {Id} not found for delete", id);
            return Result<AppError>.Failure(
                AppError.NotFound($"Producto con ID {id} no encontrado")
            );
        }
        
        await _productoRepository.DeleteAsync(id);
        _logger.LogInformation("Producto deleted with id: {Id}", id);
        
        return Result<AppError>.Success();
    }

    /// <summary>
    /// Validation method using Result Pattern
    /// 
    /// RETURNS Result<AppError> instead of throwing exceptions
    /// 
    /// This is the MODERN approach:
    /// - Validation failures are returned as Result
    /// - No exceptions thrown
    /// - Controller can handle gracefully
    /// 
    /// Java: Either<AppError, Unit> validate(ProductoRequestDto dto)
    /// </summary>
    private async Task<Result<AppError>> ValidateProductoAsync(ProductoRequestDto dto)
    {
        // Validate nombre
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            return Result<AppError>.Failure(
                AppError.Validation("El nombre del producto es requerido")
            );
        }
        
        if (dto.Nombre.Length < 3)
        {
            return Result<AppError>.Failure(
                AppError.Validation("El nombre debe tener al menos 3 caracteres")
            );
        }
        
        if (dto.Nombre.Length > 200)
        {
            return Result<AppError>.Failure(
                AppError.Validation("El nombre no puede exceder 200 caracteres")
            );
        }
        
        // Validate precio
        if (dto.Precio <= 0)
        {
            return Result<AppError>.Failure(
                AppError.Validation("El precio debe ser mayor que 0")
            );
        }
        
        // Validate stock
        if (dto.Stock < 0)
        {
            return Result<AppError>.Failure(
                AppError.Validation("El stock no puede ser negativo")
            );
        }
        
        // Validate categoria exists
        var categoriaExists = await _categoriaRepository.FindByIdAsync(dto.CategoriaId);
        if (categoriaExists == null)
        {
            return Result<AppError>.Failure(
                AppError.NotFound($"Categoría con ID {dto.CategoriaId} no encontrada")
            );
        }
        
        return Result<AppError>.Success();
    }
}
