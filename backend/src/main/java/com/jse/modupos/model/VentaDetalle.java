package com.jse.modupos.model;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.math.BigDecimal;

@Getter
@Setter
@Entity
@Table(name = "venta_detalles")
public class VentaDetalle extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "venta_id", nullable = false)
    private Venta venta;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "producto_id", nullable = false)
    private Producto producto;

    @Column(nullable = false)
    private Integer cantidad;

    @Column(name = "precio_unitario_historico", nullable = false, precision = 18, scale = 4)
    private BigDecimal precioUnitarioHistorico;

    @Column(nullable = false    , precision = 18, scale = 4)
    private BigDecimal descuento = BigDecimal.ZERO;

    @Column(nullable = false, precision = 18, scale = 4)
    private BigDecimal importe; // (cantidad * precio_unitario) - descuento
}
