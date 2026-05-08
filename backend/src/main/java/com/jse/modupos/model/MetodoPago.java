package com.jse.modupos.model;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Table(name = "metodos_pago")
public class MetodoPago extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 50, unique = true)
    private String nombre;

    @Column(name = "es_digital", nullable = false)
    private Boolean esDigital;
}
