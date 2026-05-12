package com.jse.modupos.dto;

import jakarta.validation.Valid;
import jakarta.validation.constraints.DecimalMin;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

@Data
public class VentaDTO {
    private Long id;

    private String folio;

    private LocalDateTime fecha;

    private BigDecimal subtotal;

    @DecimalMin(value = "0.0", inclusive = true, message = "El impuesto no puede ser negativo.")
    private BigDecimal impuesto;

    private BigDecimal total;

    @NotNull(message = "El metodo de pago es obligatorio.")
    private Long metodoPagoId;

    private String metodoPagoNombre;

    @NotEmpty(message = "La venta debe incluir al menos un detalle.")
    @Valid
    private List<DetalleVentaDTO> detalles;
}
