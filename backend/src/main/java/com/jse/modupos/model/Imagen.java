package com.jse.modupos.model;

import jakarta.persistence.*;
import lombok.Data;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Table(name = "imagenes")
public class Imagen extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 100)
    private String nombre;

    @Column(name = "imagen_url", nullable = false, unique = true)
    private String imagenUrl;

    @Column(name = "public_id",nullable = false, unique = true)
    private String publicId;
}
