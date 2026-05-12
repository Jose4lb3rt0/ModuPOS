package com.jse.modupos.dto;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.math.BigDecimal;

@Data
public class DetalleVentaDTO {
    private Long id;

    private Long ventaId;

    @NotNull(message = "El producto es obligatorio.")
    private Long productoId;

    private String productoNombre;

    @NotNull(message = "La cantidad es obligatoria.")
    @Min(value = 1, message = "La cantidad debe ser mayor a 0.")
    private Integer cantidad;

    private BigDecimal precioUnitario;

    private BigDecimal subtotal;
}
