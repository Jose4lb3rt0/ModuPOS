package com.jse.modupos.mapper;

import com.jse.modupos.dto.ProductoDTO;
import com.jse.modupos.model.Producto;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValuePropertyMappingStrategy;

@Mapper(componentModel = "spring", nullValuePropertyMappingStrategy = NullValuePropertyMappingStrategy.IGNORE)
public interface ProductoMapper {

    @Mapping(source = "categoria.id", target = "categoriaId")
    @Mapping(source = "imagen.imagenUrl", target = "imagenUrl")
    ProductoDTO toDTO(Producto producto);

    @Mapping(target = "categoria", ignore = true)
    @Mapping(target = "imagen", ignore = true)
    Producto toEntity(ProductoDTO dto);

    @Mapping(target = "id", ignore = true)
    @Mapping(target = "categoria", ignore = true)
    @Mapping(target = "imagen", ignore = true)
    @Mapping(target = "isDeleted", ignore = true)
    void updateEntityFromDto(ProductoDTO dto, @MappingTarget Producto producto);
}
