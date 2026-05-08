package com.jse.modupos.model;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Getter
@Setter
@Entity
@Table(name = "ventas")
public class Venta extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, unique = true, length = 20)
    private String folio;

    @Column(nullable = false)
    private LocalDateTime fecha = LocalDateTime.now();

    @Column(nullable = false, precision = 18, scale = 4)
    private BigDecimal subtotal;

    @Column(nullable = false, precision = 18, scale = 4)
    private BigDecimal impuestos;

    @Column(name = "descuento_total", nullable = false, precision = 18, scale = 4)
    private BigDecimal descuentoTotal = BigDecimal.ZERO;

    @Column(nullable = false, precision = 18, scale = 4)
    private BigDecimal total;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private EstadoVenta estado = EstadoVenta.COMPLETADA;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "metodo_pago_id", nullable = false)
    private MetodoPago metodoPago;

    @OneToMany(mappedBy = "venta", cascade = CascadeType.ALL, orphanRemoval = true)
    private List<VentaDetalle> detalles = new ArrayList<>();
}
