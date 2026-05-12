package com.jse.modupos.mapper;

import com.jse.modupos.dto.CategoriaDTO;
import com.jse.modupos.model.Categoria;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.NullValuePropertyMappingStrategy;

@Mapper(componentModel = "spring", nullValuePropertyMappingStrategy = NullValuePropertyMappingStrategy.IGNORE)
public interface CategoriaMapper {

    @Mapping(source = "categoriaPadre.id", target = "categoriaPadreId")
    @Mapping(source = "imagen.imagenUrl", target = "imagenUrl")
    CategoriaDTO toDTO(Categoria categoria);

    @Mapping(target = "categoriaPadre", ignore = true) //el padre se busca en el service
    @Mapping(target = "imagen", ignore = true)
    Categoria toEntity(CategoriaDTO dto);

    @Mapping(target = "id", ignore = true)
    @Mapping(target = "categoriaPadre", ignore = true)
    @Mapping(target = "imagen", ignore = true)
    @Mapping(target = "isDeleted", ignore = true)
    void updateEntityFromDto(CategoriaDTO dto, @MappingTarget Categoria entity);
}
