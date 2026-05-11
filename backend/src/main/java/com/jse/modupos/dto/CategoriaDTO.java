package com.jse.modupos.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;
import lombok.Data;

@Data
public class CategoriaDTO {
    private Long id;

    @NotBlank(message = "El nombre es obligatorio.")
    @Size(max = 100, message = "El nombre no puede exceder los 100 caracteres")
    private String nombre;

    private String descripcion;

    @Pattern(regexp = "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", message = "Formato de color hexadecimal inválido")
    private String color;

    private Long categoriaPadreId;
    private String imagenUrl;
}
