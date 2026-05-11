package com.jse.modupos.dto;

import lombok.Data;

@Data
public class CategoriaDTO {
    private Long id;
    private String nombre;
    private String descripcion;
    private String color;
    private Long categoriaPadreId;
    private String imagenUrl;
}
