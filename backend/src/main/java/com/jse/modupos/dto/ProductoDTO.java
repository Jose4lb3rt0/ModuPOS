package com.jse.modupos.dto;

import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.math.BigDecimal;

@Data
public class ProductoDTO {
    private Long id;

    @NotBlank(message = "El nombre es obligatorio.")
    private String nombre;

    private String sku;

    @NotNull(message = "El precio es obligatorio.")
    @DecimalMin(value = "0.0", inclusive = false, message = "El precio debe ser mayor a 0.")
    private BigDecimal precioActual;

    @Min(value = 0, message = "El stock no puede ser negativo.")
    private Integer stock;

    @NotNull(message = "La categoría es obligatoria.")
    private Long categoriaId;

    private String imagenUrl;
}
